using Bource.Common.Utilities;
using Bource.Data.Informations.UnitOfWorks;
using Bource.Models.Data.Common;
using Bource.Models.Data.Enums;
using Bource.Services.Crawlers.Tse;
using Bource.Services.Crawlers.Tsetmc;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bource.Console.Selenium
{
    internal class SeleniumManager
    {
        private readonly ITsetmcUnitOfWork tsetmcUnitOfWork;
        private readonly ITsetmcCrawlerService tsetmcCrawlerService;

        public SeleniumManager(ITsetmcUnitOfWork tsetmcUnitOfWork, ITsetmcCrawlerService tsetmcCrawlerService)
        {
            this.tsetmcUnitOfWork = tsetmcUnitOfWork;
            this.tsetmcCrawlerService = tsetmcCrawlerService;
        }

        internal void GetSymbols()
        {

            tsetmcCrawlerService.GetOrUpdateSymbolGroupsAsync().GetAwaiter().GetResult();

            IWebDriver driver = new ChromeDriver(@"ChromeWebDriver");

            driver.Navigate().GoToUrl("http://www.tsetmc.com/Loader.aspx?ParTree=15131F");
            bool wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60)).Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

            if (wait == true)
            {
                driver.FindElement(By.Id("id1")).Click();
                Task.Delay(3000).GetAwaiter().GetResult();
            }

            driver.FindElement(By.XPath($"//div[@aria-label='نمایش همه نمادها در دیده بان']")).Click();
            Task.Delay(2000).GetAwaiter().GetResult();
            driver.FindElement(By.Id("id1")).Click();

            var list = new List<(PapersTypes?, string)>
            {
                (null,"نمایش همه نمادها در دیده بان"),
                (PapersTypes.OTC,"نمایش اوراق بازار پایه در دیده بان"),
                (PapersTypes.Cash,"نمایش اوراق سهام در دیده بان"),
                (PapersTypes.HousingFacilities,"نمایش اوراق تسهیلات مسکن در دیده بان"),
                (PapersTypes.Option,"نمایش اوراق حق تقدم در دیده بان"),
                (PapersTypes.Debt,"نمایش اوراق بدهی در دیده بان"),
                (PapersTypes.TradeOption,"نمایش اوراق اختیار معامله در دیده بان"),
                (PapersTypes.Future,"نمایش اوراق آتی در دیده بان"),
                (PapersTypes.ETF,"نمایش صندوق های سرمایه گذاری در دیده بان"),
                (PapersTypes.CommodityExchange,"نمایش بورس کالا در دیده بان"),
            };

            foreach (var item in list.Skip(2))
            {
                driver.FindElement(By.XPath($"//div[@aria-label='{item.Item2}']")).Click();
                Task.Delay(1000).GetAwaiter().GetResult();
            }

            foreach (var item in list.Skip(1))
            {
                driver.FindElement(By.XPath($"//div[@aria-label='{item.Item2}']")).Click();
                Task.Delay(3000).GetAwaiter().GetResult();

                // Do save
                var innerHtml = GetTableOfSymbols(driver);
                SaveSymbols(innerHtml, item.Item1.Value);

                System.Console.WriteLine(item);

                driver.FindElement(By.XPath($"//div[@aria-label='{item.Item2}']")).Click();
                Task.Delay(3000).GetAwaiter().GetResult();
            }

            driver.Close();
        }

        private void SaveSymbols(string html, PapersTypes papersType)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var nodes = htmlDoc.DocumentNode.SelectNodes("/div");

            var groups = tsetmcUnitOfWork.GetSymbolGroupsAsync().GetAwaiter().GetResult();
            var symbols = tsetmcUnitOfWork.GetSymbolsAsync().GetAwaiter().GetResult();
            string groupname = null;
            SymbolGroup symbolGroup = null;

            foreach (var node in nodes)
            {
                if (node.HasClass("secSep"))
                {
                    if (symbolGroup is not null)
                        tsetmcUnitOfWork.UpdateSymbolGroupAsync(symbolGroup).GetAwaiter().GetResult();

                    groupname = node.InnerText;
                    symbolGroup = groups.FirstOrDefault(i => Common.Utilities.StringHelper.ComparePersion(i.Title, groupname));

                    if (symbolGroup is null)
                        throw new ApplicationException();

                    if (symbolGroup.Symbols is null)
                        symbolGroup.Symbols = new();

                    continue;
                }

                var htmlDoc2 = new HtmlDocument();
                htmlDoc2.LoadHtml(node.InnerHtml);
                var atags = htmlDoc2.DocumentNode.SelectNodes("//a");

                var sign = atags[0].GetDirectInnerText();
                var name = atags[1].GetDirectInnerText();
                var insCode = Convert.ToInt64(atags[0].GetQueryString("amp;i"));
                var symbol = symbols.FirstOrDefault(i => i.InsCode == insCode);
                if (symbol is not null)
                    symbol.PaperType = papersType;
                else
                {
                    symbol = new Symbol
                    {
                        Sign = sign,
                        Name = name,
                        InsCode = insCode,
                        PaperType = papersType,
                        GroupName = groupname,
                        ExistInType = SymbolExistInType.Tsetmc,
                        GroupId = symbolGroup.Code,
                        CreateDate = DateTime.Now,
                    };
                }

                tsetmcUnitOfWork.AddOrUpdateSymbolAsync(symbol).GetAwaiter().GetResult();
                if (!symbolGroup.Symbols.Any(i => i.InsCode == insCode))
                    symbolGroup.Symbols.Add(new Models.Data.Tsetmc.IndicatorSymbol(sign, name, insCode));
            }

            if (symbolGroup is not null)
                tsetmcUnitOfWork.UpdateSymbolGroupAsync(symbolGroup).GetAwaiter().GetResult();
        }

        private string GetTableOfSymbols(IWebDriver driver, int numberOfTry = 0)
        {
            if (numberOfTry == 3)
                throw new ApplicationException();

            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            string innerHtml = string.Empty;
            if (js != null)
            {
                innerHtml = (string)js.ExecuteScript("return document.getElementById(\"main\").innerHTML;");
            }

            if (IsTrueNode(innerHtml))
                return innerHtml;

            Task.Delay(5000).GetAwaiter().GetResult();

            return GetTableOfSymbols(driver, numberOfTry + 1);
        }

        private bool IsTrueNode(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var nodes = htmlDoc.DocumentNode.SelectNodes("/div");
            return nodes.First().HasClass("secSep");
        }
    }
}
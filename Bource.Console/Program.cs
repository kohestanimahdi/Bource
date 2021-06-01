using Bource.Services.Crawlers.AsanBource;
using Bource.Services.Crawlers.Codal360;
using Bource.Services.Crawlers.FipIran;
using Bource.Services.Crawlers.Tsetmc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Console
{
    internal static class Program
    {
        private static object lockObject = new();

        private static void Main(string[] args)
        {
            //Common.Utilities.ConsoleHelper.WriteProgressBar(0);
            //for (var i = 0; i <= 100; ++i)
            //{
            //    Common.Utilities.ConsoleHelper.WriteProgressBar(i, true);
            //    Thread.Sleep(50);
            //}

            //System.Console.WriteLine();
            //Common.Utilities.ConsoleHelper.WriteProgress(0);
            //for (var i = 0; i <= 100; ++i)
            //{
            //    Common.Utilities.ConsoleHelper.WriteProgress(i, true);
            //    Thread.Sleep(50);
            //}

            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            var httpClient = new HttpClient(handler);
            var httpClient2 = new HttpClient(handler);
            var httpClient3 = new HttpClient(handler);
            var httpClient4 = new HttpClient(handler);
            var httpClient5 = new HttpClient(handler);

            var tseClient = new TseClientService();
            var tse = new TsetmcCrawlerService(httpClient);
            var fipIran = new FipiranCrawlerService(httpClient2);
            var TseSymbolDataProvider = new TseSymbolDataProvider(httpClient3);
            var codal360CrawlerService = new Codal360CrawlerService(httpClient4);
            var asanBourceCrawlerService = new AsanBourceCrawlerService(httpClient5);

            //tse.GetLatestSymbolDataAsync().GetAwaiter().GetResult();

            int n = -1;
            string input;


            PrintTableOfContent();
            while (n != 0)
            {
                System.Console.WriteLine("****************************************************************************");

                do
                {
                    System.Console.WriteLine("Enter Number:");
                    input = System.Console.ReadLine();
                } while (!Int32.TryParse(input, out n));
                var startDate = DateTime.Now;

                try
                {

                    switch (n)
                    {
                        case 1:
                            TseSymbolDataProvider.AddOrUpdateSymbols().GetAwaiter().GetResult();
                            break;
                        case 2:
                            tse.UpdateSymbolsAsync().GetAwaiter().GetResult();
                            break;
                        case 3:
                            tse.GetOrUpdateSymbolGroupsAsync().GetAwaiter().GetResult();
                            break;
                        case 4:
                            tse.GetValueOfMarketAsync().GetAwaiter().GetResult();
                            break;
                        case 5:
                            tse.GetMarketAtGlanceAsync().GetAwaiter().GetResult();
                            break;
                        case 6:
                            tse.GetSelectedIndicatorAsync().GetAwaiter().GetResult();
                            break;
                        case 7:
                            tse.GetMarketWatcherMessage().GetAwaiter().GetResult();
                            break;
                        case 8:
                            tse.GetTopSupplyAndDemandAsync().GetAwaiter().GetResult();
                            break;
                        case 9:
                            tse.GetAllNaturalAndLegalEntityAsync().GetAwaiter().GetResult();
                            break;
                        case 10:
                            tse.GetAllCapitalIncreaseAsync().GetAwaiter().GetResult();
                            break;
                        case 11:
                            tseClient.GetInsturmentsClosingPriceAsync().GetAwaiter().GetResult();
                            break;
                        case 12:
                            fipIran.GetAssociations().GetAwaiter().GetResult();
                            break;
                        case 13:
                            fipIran.GetNews(Models.Data.Enums.FipIranNewsTypes.WorldOfEconomy).GetAwaiter().GetResult();
                            break;
                        case 14:
                            fipIran.GetNews(Models.Data.Enums.FipIranNewsTypes.AssembliesAndCompanies).GetAwaiter().GetResult();
                            break;
                        case 15:
                            tse.FillOneTimeDataAsync().GetAwaiter().GetResult();
                            break;
                        case 16:

                            Task.Run(() =>
                            {
                                while (DateTime.Now.Hour >= 9 && DateTime.Now.Hour <= 13)
                                {
                                    var time = DateTime.Now;
                                    tse.GetLatestSymbolDataAsync().GetAwaiter().GetResult();
                                    var delay = DateTime.Now - time;
                                    if (delay < TimeSpan.FromSeconds(1))
                                        Task.Delay(TimeSpan.FromSeconds(1) - delay).GetAwaiter().GetResult();
                                }
                            });
                            Task.Run(() =>
                            {
                                while (DateTime.Now.Hour >= 9 && DateTime.Now.Hour <= 15)
                                {
                                    TseSymbolDataProvider.SaveSymbolDataFromQueue().GetAwaiter().GetResult();
                                }
                            });

                            break;
                        case 17:
                            tse.GetSymbolsShareHoldersAsync().GetAwaiter().GetResult();
                            break;
                        case 18:
                            tse.GetChangeOfSharesOfActiveShareHoldersAsync().GetAwaiter().GetResult();
                            break;
                        case 19:
                            asanBourceCrawlerService.DownloadSymbolsImageAsync().GetAwaiter().GetResult();
                            break;
                        case 20:
                            codal360CrawlerService.UpdateSymbolsCodalURLAsync().GetAwaiter().GetResult();
                            break;
                        case 21:
                            codal360CrawlerService.UpdateSymbolsCodalImageAsync().GetAwaiter().GetResult();
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    LogException(ex);
                }


                System.Console.WriteLine($"\nTime:{(DateTime.Now - startDate).TotalSeconds} Second");

            }

        }

        static void PrintTableOfContent()
        {
            Common.Utilities.ConsoleHelper.PrintTable(3,
                "Number", "Operation", "PersianOperation",
                "1", "Get Symbols", "دریافت لیست نمادها",
                "2", "Fill Symbol Informations", "تکمیل اطلاعات شناسنامه و معرفی نمادها ",
                "3", "Get Groups", "اطلاعات گروه نمادها",
                "4", "Value Of Market", "ارزش بازار",
                "5", "Market At Glance", "بازار نقدی در یک نگاه",
                "6", "Selected Indicator", "دریافت لحظه ای شاخص های منتخب",
                "7", "Market Watcher Message", "پیغام های ناظر بازار",
                "8", "Top Supply And Demand", "برترین عرضه و تقاضا",
                "9", "Natural And Legal Entity", "حقیقی و حقوقی",
                "10", "Capital Increase", "افزایش سرمایه",
                "11", "Tse Client", "اطلاعات کامل برنامه کلاینت",
                "12", "Associations FipIran", "اطلاعات مجامع فیپ ایران",
                "13", "News FipIran WorldOfEconomy", "خبرهای دنیای اقتصاد فیپ ایران",
                "14", "News FipIran", "خبرهای مجامع و شرکت ها فیپ ایران",
                "15", "First Time Main Symbol", "اطلاعات پایه روزانه نمادها",
                "16", "Main Symbol", "اطلاعات پایه نمادها",
                "17", "Symbols Share Holders", "اطلاعات سهامداران نماد",
                "18", "Main Change Active ShareHolders", "سهامداران فعال نماد",
                "19", "download symbol logo", "دریافت لوگوها",
                "20", "Codal 360 url", "لینک کدال"
                );
        }
        public static void LogException(Exception exception)
        {
            lock (lockObject)
            {
                var content = $"{DateTime.Now.ToLongTimeString()}{Environment.NewLine}";
                content += $"{exception.Message}{Environment.NewLine}";
                content += $"-------------------------------------------------------{Environment.NewLine}";
                content += $"{exception.StackTrace}{Environment.NewLine}";
                content += $"***********************************************************************************************{Environment.NewLine}";
                File.AppendAllText(Path.Combine(Directory.GetCurrentDirectory(), "log.txt"), content);
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine(content);
                System.Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
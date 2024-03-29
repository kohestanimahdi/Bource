﻿using Bource.Common.Models;
using Bource.Data.Informations.UnitOfWorks;
using Bource.Services.Crawlers.AsanBource;
using Bource.Services.Crawlers.Codal360;
using Bource.Services.Crawlers.FipIran;
using Bource.Services.Crawlers.Shakhesban;
using Bource.Services.Crawlers.Tse;
using Bource.Services.Crawlers.Tsetmc;
using Bource.WebConfiguration.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Bource.Console
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

            var applicationSetting = configuration.GetSection("ApplicationSettings").Get<ApplicationSetting>();

            var serviceProvider = new ServiceCollection()
             .AddLogging(builder =>
             {
                 builder.AddConfiguration(configuration.GetSection("Logging"));
                 builder.AddConsole();
                 builder.AddDebug();
             })
             .AddScoped<ITseClientService, TseClientService>()
             .AddScoped<ITsetmcCrawlerService, TsetmcCrawlerService>()
             .AddScoped<IFipiranCrawlerService, FipiranCrawlerService>()
             .AddScoped<ITseSymbolDataProvider, TseSymbolDataProvider>()
             .AddScoped<ICodal360CrawlerService, Codal360CrawlerService>()
             .AddScoped<IAsanBourceCrawlerService, AsanBourceCrawlerService>()
             .AddScoped<ITseCrawlerService, TseCrawlerService>()
             .AddScoped<IShakhesbanCrawlerService, ShakhesbanCrawlerService>()
             .AddScoped<ITsetmcUnitOfWork, TsetmcUnitOfWork>()
             .AddScoped<IFipiranUnitOfWork, FipiranUnitOfWork>();

            serviceProvider.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost";
                options.InstanceName = "RedisJobCache";
            });

            serviceProvider.AddCrawlerHttpClient(applicationSetting);

            serviceProvider.Configure<ApplicationSetting>(configuration.GetSection("ApplicationSettings"));
            serviceProvider.AddSingleton<ApplicationSetting>(applicationSetting);



            var serviceProviderFactory = serviceProvider.BuildServiceProvider();

            var tseClient = serviceProviderFactory.GetService<ITseClientService>();
            var tse = serviceProviderFactory.GetService<ITsetmcCrawlerService>();
            var fipIran = serviceProviderFactory.GetService<IFipiranCrawlerService>();
            var TseSymbolDataProvider = serviceProviderFactory.GetService<ITseSymbolDataProvider>();
            var codal360CrawlerService = serviceProviderFactory.GetService<ICodal360CrawlerService>();
            var asanBourceCrawlerService = serviceProviderFactory.GetService<IAsanBourceCrawlerService>();
            var tseCrawlerService = serviceProviderFactory.GetService<ITseCrawlerService>();
            var shakhesbanCrawlerService = serviceProviderFactory.GetService<IShakhesbanCrawlerService>();
            var tseUnitOfWork = serviceProviderFactory.GetService<ITsetmcUnitOfWork>();

            var logger = serviceProviderFactory.GetService<ILoggerFactory>().CreateLogger(nameof(Program));

            var applicationSettings = serviceProviderFactory.GetService<ApplicationSetting>();

            int n = -1;
            string input;
            tse.SetMarketStatus(true).GetAwaiter().GetResult();

            //ifbCrawlerService.GetPapersAsync().GetAwaiter().GetResult();

            PrintTableOfContent();
            while (n != 0)
            {
                System.Console.WriteLine("\n****************************************************************************");

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
                            tse.GetMarketAtGlanceScheduleEverySecondAsync().GetAwaiter().GetResult();
                            break;

                        case 6:
                            tse.GetSelectedIndicatorEverySecondAsync().GetAwaiter().GetResult();
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
                                    try
                                    {
                                        var time = DateTime.Now;
                                        tse.ScheduleLatestSymbolDataEverySecondAsync().GetAwaiter().GetResult();
                                        var delay = (DateTime.Now - time).TotalSeconds;
                                        if (delay < 1 && delay > 0)
                                            Task.Delay(TimeSpan.FromSeconds(1 - delay)).GetAwaiter().GetResult();
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.LogError(ex, "");
                                    }
                                }
                            });
                            //Task.Run(() =>
                            //{
                            //    while (DateTime.Now.Hour >= 9 && DateTime.Now.Hour <= 14)
                            //        TseSymbolDataProvider.SaveSymbolData().GetAwaiter().GetResult();
                            //});

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
                            fipIran.GetSubjectSymbols().GetAwaiter().GetResult();
                            break;

                        case 22:
                            tse.GetSymbolsOfIndicatorsAsync().GetAwaiter().GetResult();
                            break;

                        case 23:
                            var selenium = new Selenium.SeleniumManager(tseUnitOfWork, tse);
                            selenium.GetSymbols();
                            System.Console.Clear();
                            PrintTableOfContent();
                            break;
                        case 24:
                            var selenium2 = new Selenium.SeleniumManager(new TsetmcUnitOfWork(new ApplicationSetting()
                            {
                                mongoDbSetting = new MongoDbSetting
                                {
                                    DataBaseName = applicationSettings.mongoDbSetting.DataBaseName,
                                    ConnectionString = applicationSettings.mongoDbSetting.ServerConnectionString
                                }
                            }), tse);

                            selenium2.GetSymbols();
                            System.Console.Clear();
                            PrintTableOfContent();
                            break;
                        case 25:
                            shakhesbanCrawlerService.GetSymbolPrioritiesAsync().GetAwaiter().GetResult();
                            break;
                        case 26:
                            tse.RemoveOldSymbolDataAsync().GetAwaiter().GetResult();
                            break;
                        case 27:
                            tse.GetLastSymbolData().GetAwaiter().GetResult();
                            break;
                        case 28:
                            tse.ScheduleLatestSymbolDataEverySecond();
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "");
                }

                logger.LogInformation($"Time:{(DateTime.Now - startDate).TotalSeconds} Second");
            }
        }

        private static void PrintTableOfContent()
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
                "20", "Codal 360 url", "لینک کدال",
                "21", "FipIran Subject", "موضوع فعالیت از فیپ ایران",
                "22", "Symbols of Indicators", "لیست نمادهای شاخص ها ",
                "23", "Get papers", "باز کردن مرورگر ودریافت نوع اوراق ",
                "24", "Get papers and save to server", "باز کردن مرورگر ودریافت نوع اوراق Server",
                "25", "Get priorities", "دریافت حق تقدم ها",
                "26", "remove old symbol datas", "حذف اطلاعات بیشتر از 2 ماه"
                );
        }
    }
}
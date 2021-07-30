using Bource.Common.Models;
using Bource.Data.Informations.UnitOfWorks;
using Bource.Services.Crawlers.AsanBource;
using Bource.Services.Crawlers.Codal360;
using Bource.Services.Crawlers.FipIran;
using Bource.Services.Crawlers.Ifb;
using Bource.Services.Crawlers.Tse;
using Bource.Services.Crawlers.Tsetmc;
using Bource.WebConfiguration.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
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
             .AddScoped<IIfbCrawlerService, IfbCrawlerService>()
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
            var ifbCrawlerService = serviceProviderFactory.GetService<IIfbCrawlerService>();
            var tseUnitOfWork = serviceProviderFactory.GetService<ITsetmcUnitOfWork>();

            var logger = serviceProviderFactory.GetService<ILoggerFactory>().CreateLogger(nameof(Program));

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
                "23", "Get papers", "باز کردن مرورگر ودریافت نوع اوراق "
                );
        }

        private static async Task ConnectToWebSocket()
        {
            do
            {
                using (var socket = new ClientWebSocket())
                    try
                    {
                        await socket.ConnectAsync(new Uri($"ws://api2.tablokhani.com/socket.io/?EIO=4&transport=websocket&sid=1yWSjSC_NpwNxxnMAjx2"), CancellationToken.None);

                        await Send(socket, "boxIndex");
                        await Receive(socket);
                    }
                    catch (Exception ex)
                    {
                    }
            } while (true);
        }

        private static Task Send(ClientWebSocket socket, string data) =>
             socket.SendAsync(Encoding.UTF8.GetBytes(data), WebSocketMessageType.Text, true, CancellationToken.None);

        private static async Task Receive(ClientWebSocket socket)
        {
            var buffer = new ArraySegment<byte>(new byte[2048]);
            do
            {
                WebSocketReceiveResult result;
                using (var ms = new MemoryStream())
                {
                    do
                    {
                        result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    } while (!result.EndOfMessage);

                    if (result.MessageType == WebSocketMessageType.Close)
                        break;

                    ms.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                        System.Console.WriteLine(await reader.ReadToEndAsync());
                }
            } while (true);
        }
    }
}
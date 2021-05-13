using Bource.Services.Crawlers.Tsetmc;
using System;
using System.IO;
using System.Net;
using System.Net.Http;

namespace Bource.Console
{
    internal static class Program
    {
        private static object lockObject = new();

        private static void Main(string[] args)
        {
            var startDate = DateTime.Now;
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            var httpClient = new HttpClient(handler);
            //var fipIran = new FipiranCrawlerService(httpClient);
            //fipIran.GetAssociations().GetAwaiter().GetResult();

            //var tseClient = new TseClientService();
            //tseClient.Test().GetAwaiter().GetResult();

            var tse = new Services.Crawlers.Tsetmc.TsetmcCrawlerService(httpClient);

            //
            //tse.GetAllCapitalIncreaseAsync().GetAwaiter().GetResult();

            try
            {
                tse.GetLatestSymbolDataAsync().GetAwaiter().GetResult();

            }
            catch (Exception ex)
            {

                throw;
            }
            //tse.SaveSymbolData().GetAwaiter().GetResult();
            //var t3 = Task.Run(() =>
            //{
            //    while (true)
            //    {
            //        var startTime = DateTime.Now;

            //        tse.GetCashMarketAtGlanceAsync().GetAwaiter().GetResult();

            //        var finishTime = DateTime.Now;
            //        if ((finishTime - startTime).TotalSeconds < 1)
            //            Task.Delay(finishTime - startTime).GetAwaiter().GetResult();
            //    }
            //});

            // var t1 = Task.Run(() => tse.SaveSymbolData());
            // var t2 = Task.Run(() =>
            // {
            //     while (true)
            //     {
            //         try
            //         {
            //             var startTime = DateTime.Now;

            //             tse.GetLatestSymbolDataAsync().GetAwaiter().GetResult();

            //             var finishTime = DateTime.Now;

            //             System.Console.WriteLine($"{ (finishTime - startTime).TotalSeconds}");
            //             System.Console.WriteLine($"*********************************************");

            //             if ((finishTime - startTime).TotalSeconds < 1)
            //                 Task.Delay(finishTime - startTime).GetAwaiter().GetResult();

            //         }
            //         catch (Exception ex)
            //         {
            //             LogException(ex);
            //         }

            //     }
            // });

            System.Console.WriteLine($"All app:{(DateTime.Now - startDate).TotalSeconds}");
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
            }
        }
    }
}
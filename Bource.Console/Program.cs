using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Console
{
    static class Program
    {
        private static object lockObject = new();
        static void Main(string[] args)
        {


            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            var httpClient = new HttpClient(handler);
            var tse = new Services.Crawlers.Tsetmc.TsetmcCrawlerService(httpClient);

            var startDate = DateTime.Now;
            tse.GetAllCapitalIncreaseAsync().GetAwaiter().GetResult();
            //tse.GetLatestSymbolDataAsync().GetAwaiter().GetResult();
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

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace Bource.Console
{
    static class Program
    {
        private static object lockObject = new object();
        static void Main(string[] args)
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            var httpClient = new HttpClient(handler);
            var tse = new Services.Crawlers.Tsetmc.TsetmcCrawlerService(httpClient);

            while (true)
            {
                try
                {
                    var startDate = DateTime.Now;

                    tse.GetLatestSymbolDataAsync().GetAwaiter().GetResult();

                    System.Console.WriteLine($"{ (DateTime.Now - startDate).TotalSeconds}");
                    System.Console.WriteLine($"*********************************************");

                }
                catch (Exception ex)
                {

                    LogException(ex);
                }

            }
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

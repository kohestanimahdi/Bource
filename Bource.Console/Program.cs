using System;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace Bource.Console
{
    static class Program
    {
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
                tse.GetLatestSymbolDataAsync().GetAwaiter().GetResult();
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }
    }
}

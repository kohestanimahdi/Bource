using System;
using System.Net;
using System.Net.Http;

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
            var tse = new Services.Crawlers.Tsetmc.TsetmcCrawlerService(new HttpClient(handler));
            tse.GetOrUpdateSymbolGroups().GetAwaiter().GetResult();
        }
    }
}

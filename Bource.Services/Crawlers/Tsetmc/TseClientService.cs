using Bource.Common.Models;
using Bource.Data.Informations.UnitOfWorks;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TseClient;

namespace Bource.Services.Crawlers.Tsetmc
{
    public class TseClientService : IScopedDependency
    {
        private readonly ILogger<TseClientService> logger;
        private readonly ITsetmcUnitOfWork tsetmcUnitOfWork;
        private readonly TseClient.WebServiceTseClientSoap tseClientSoap;

        public TseClientService(ILoggerFactory loggerFactory, ITsetmcUnitOfWork tsetmcUnitOfWork)
        {
            logger = loggerFactory?.CreateLogger<TseClientService>() ?? throw new ArgumentNullException(nameof(loggerFactory));

            this.tsetmcUnitOfWork = tsetmcUnitOfWork ?? throw new ArgumentNullException(nameof(tsetmcUnitOfWork));

            tseClientSoap = new WebServiceTseClientSoapClient(WebServiceTseClientSoapClient.EndpointConfiguration.WebServiceTseClientSoap);
        }

        public TseClientService()
        {
            tsetmcUnitOfWork = new TsetmcUnitOfWork(new MongoDbSetting { ConnectionString = "mongodb://localhost:27017/", DataBaseName = "BourceInformation" });

            tseClientSoap = new WebServiceTseClientSoapClient(WebServiceTseClientSoapClient.EndpointConfiguration.WebServiceTseClientSoap);
        }

        public static string Compress(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            MemoryStream stream = new MemoryStream();
            using (var stream2 = new GZipStream(stream, CompressionMode.Compress, true))
            {
                stream2.Write(bytes, 0, bytes.Length);
            }
            stream.Position = 0L;
            var stream1 = new System.IO.MemoryStream();
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            byte[] dst = new byte[buffer.Length + 4];
            Buffer.BlockCopy(buffer, 0, dst, 4, buffer.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(bytes.Length), 0, dst, 0, 4);
            return Convert.ToBase64String(dst);
        }

        public async Task Test(CancellationToken cancellationToken = default(CancellationToken))
        {
            var aa = Compress("IRO3SBZZ0001");
            var a2a = Compress("46348559193224090");
            var aa3 = Compress("IRO1FOLD0001");
            try
            {
                var ab = await tseClientSoap.LastPossibleDevenAsync(new LastPossibleDevenRequest
                {
                    Body = new LastPossibleDevenRequestBody
                    {
                    }
                });

                var a = await tseClientSoap.DecompressAndGetInsturmentClosingPriceAsync(new DecompressAndGetInsturmentClosingPriceRequest
                {
                    Body = new DecompressAndGetInsturmentClosingPriceRequestBody
                    {
                        insCodes = aa3
                        //insCodes = "HAAAAB+LCAAAAAAABADtvQdgHEmWJSYvbcp7f0r1StfgdKEIgGATJNiQQBDswYjN5pLsHWlHIymrKoHKZVZlXWYWQMztnbz33nvvvffee++997o7nU4n99//P1xmZAFs9s5K2smeIYCqyB8/fnwfPyL2P723f3D//sPdh/f29vZ3Hu6M9nb2dnfu7+6Mdv4fkd3d4BwAAAA="
                    }
                });
            }
            catch (Exception ex)
            {
            }
            try
            {
                var ab = await tseClientSoap.InstrumentAndShareAsync(new InstrumentAndShareRequest
                {
                    Body = new InstrumentAndShareRequestBody
                    {
                        DEven = 20210511,
                        LastID = 1891
                    }
                });
            }
            catch (Exception ex)
            {
            }
        }
    }
}
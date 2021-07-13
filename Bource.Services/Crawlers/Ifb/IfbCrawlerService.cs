using Bource.Common.Models;
using Bource.Data.Informations.UnitOfWorks;
using Bource.Services.Crawlers.Ifb.Models;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Microsoft.AspNet.SignalR.Client.Transports;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Services.Crawlers.Ifb
{
    public class IfbCrawlerService : IScopedDependency, IIfbCrawlerService
    {
        #region Properties

        protected readonly HttpClient httpClient;
        protected readonly ILogger<IfbCrawlerService> logger;
        protected readonly ITsetmcUnitOfWork tsetmcUnitOfWork;

        #endregion Properties

        #region Constructors

        public IfbCrawlerService(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory, ITsetmcUnitOfWork tsetmcUnitOfWork, IDistributedCache distributedCache)
        {
            logger = loggerFactory?.CreateLogger<IfbCrawlerService>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            httpClient = httpClientFactory?.CreateClient(nameof(IfbCrawlerService)) ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.tsetmcUnitOfWork = tsetmcUnitOfWork ?? throw new ArgumentNullException(nameof(tsetmcUnitOfWork));
        }

        #endregion Constructors

        public async Task GetPapersAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var connectionSettings = await GetSignalRConnectionAsync(cancellationToken);
            //HubConnection connection = new HubConnectionBuilder()
            //    .WithUrl($"https://www.ifb.ir/signalr/hubs?connectionToken={connectionSettings.ConnectionToken}", options =>
            //     {

            //     }).Build();
            ////connect?transport=serverSentEvents&connectionToken={connectionSettings.ConnectionToken}&connectionData=%5B%7B%22name%22%3A%22myhub%22%7D%5D&tid=4
            //connection.On<string, string>("serverSentEvents", (user, message) =>
            //{
            //    Console.WriteLine(message);
            //});
            //connection.On<string, string>("myhub", (user, message) =>
            //{
            //    Console.WriteLine(message);
            //});

            //await connection.StartAsync();

            try
            {
                var dictonary = new Dictionary<string, string>();
                //dictonary.Add("connectionToken", connectionSettings.ConnectionToken);
                //dictonary.Add("tid", "2");
                //dictonary.Add("_", "1626156317633");
                var hubConnection = new Microsoft.AspNet.SignalR.Client.Hubs.HubConnection("https://www.ifb.ir/signalr/hubs", dictonary);
                //hubConnection.Headers.Add("authority", "www.ifb.ir");
                //hubConnection.Headers.Add("referer", "https://www.ifb.ir/StockQoute.aspx");
                IHubProxy stockTickerHubProxy = hubConnection.CreateHubProxy("myhub");


                stockTickerHubProxy.On<object>("letsStart", stock => Console.WriteLine(stock.ToString()));
                stockTickerHubProxy.On<object>("updateRow", stock => Console.WriteLine(stock.ToString()));
                stockTickerHubProxy.On<object>("updateFirstMarket", stock => Console.WriteLine(stock.ToString()));
                stockTickerHubProxy.On<object>("updateSecondMarket", stock => Console.WriteLine(stock.ToString()));
                stockTickerHubProxy.On<object>("updateThirdMarket", stock => Console.WriteLine(stock.ToString()));
                stockTickerHubProxy.On<object>("updatePaye_Yellow_Market", stock => Console.WriteLine(stock.ToString()));
                stockTickerHubProxy.On<object>("updatePaye_Orange_Market", stock => Console.WriteLine(stock.ToString()));
                stockTickerHubProxy.On<object>("updatePaye_Red_Market", stock => Console.WriteLine(stock.ToString()));
                stockTickerHubProxy.On<object>("updateMaskanMarket", stock => Console.WriteLine(stock.ToString()));
                stockTickerHubProxy.On<object>("updateConstMarket", stock => Console.WriteLine(stock.ToString()));
                stockTickerHubProxy.On<object>("updateETFMarket", stock => Console.WriteLine(stock.ToString()));
                stockTickerHubProxy.On<object>("updateIPMarket", stock => Console.WriteLine(stock.ToString()));
                stockTickerHubProxy.On<object>("updateSMEMarket", stock => Console.WriteLine(stock.ToString()));
                stockTickerHubProxy.On<object>("updateOptionMarket", stock => Console.WriteLine(stock.ToString()));
                stockTickerHubProxy.On<object>("updateMarketValue", stock => Console.WriteLine(stock.ToString()));
                stockTickerHubProxy.On<object>("updateSingleTable", stock => Console.WriteLine(stock.ToString()));

                stockTickerHubProxy.On<object>("letsStartForOragh", stock =>
                // Context is a reference to SynchronizationContext.Current
                Console.WriteLine(stock.ToString())
);


                var a = stockTickerHubProxy.Subscribe("letsStart");
                a.Received += A_Received;

                await hubConnection.Start(new ServerSentEventsTransport()).ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Console.WriteLine("There was an error opening the connection:{0}",
                                          task.Exception.GetBaseException());
                    }
                    else
                    {
                        Console.WriteLine("Connected");
                    }

                });

                await stockTickerHubProxy.Invoke<string>("letsStart", "HELLO World ").ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Console.WriteLine("There was an error calling send: {0}",
                                          task.Exception.GetBaseException());
                    }
                    else
                    {
                        Console.WriteLine(task.Result);
                    }
                });
                //ServicePointManager.
                //await hubConnection.Start();

            }
            catch (Exception ex)
            {

            }


            // Call server method "JoinGroup" from client
            //await stockTickerHubProxy.Invoke("JoinGroup", "SignalRChatRoom");
        }

        private void A_Received(IList<Newtonsoft.Json.Linq.JToken> obj)
        {

        }

        private async Task<SignalRConnectionResponse> GetSignalRConnectionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await httpClient.GetAsync("signalr/negotiate?connectionData=%5B%7B\"name\"%3A\"myhub\"%7D%5D&_=1626156317633", cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<SignalRConnectionResponse>(result);
        }
    }
}

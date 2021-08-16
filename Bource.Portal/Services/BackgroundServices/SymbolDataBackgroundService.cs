using Bource.Portal.Controllers.SignalR.V1;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Portal.Services.BackgroundServices
{
    public class SymbolDataBackgroundService : BackgroundService
    {
        private readonly IHubContext<SymbolsHub> symbolHubContext;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<SymbolDataBackgroundService> logger;
        public SymbolDataBackgroundService(IHubContext<SymbolsHub> symbolHubContext,
                                      IServiceScopeFactory serviceScopeFactory,
                                      ILoggerFactory loggerFactory)
        {
            this.symbolHubContext = symbolHubContext ?? throw new ArgumentNullException(nameof(symbolHubContext));
            this.serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            this.logger = loggerFactory?.CreateLogger<SymbolDataBackgroundService>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        //var billUnitOfWork = scope.ServiceProvider.GetService<BillUnitOfWork>();
                        foreach (var key in SymbolsHub.SymbolDataUsers.Keys)
                        {
                            await symbolHubContext.Clients.Group($"SymbolData-{key}")
                                .SendAsync("SymbolData", new { Name = "Mahdi", InsCode = key }, cancellationToken: stoppingToken);

                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error in SymbolData Background Service");
                }

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}

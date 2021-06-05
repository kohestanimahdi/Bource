using Bource.Services.Crawlers.Tsetmc;
using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Threading;

namespace Bource.WebConfiguration.Configuration
{
    public static class HangfireConfigurationExtensions
    {
        private static ConnectionMultiplexer redis;

        public static void AddCustomHangfire(this IServiceCollection services, string connectionString)
        {
            redis = ConnectionMultiplexer.Connect(connectionString);
            services.AddHangfire(configuration =>
            {
                configuration.UseRedisStorage(redis);
            });

            services.AddHangfireServer();
        }

        //[Conditional("RELEASE")]
        public static void UseCustomHangfire(this IApplicationBuilder app)
        {
            app.UseHangfireServer();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
#if !DEBUG
                Authorization = new[] { new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                {
                    RequireSsl = false,
                    SslRedirect = false,
                    LoginCaseSensitive = true,
                    Users = new []
                    {
                        new BasicAuthAuthorizationUser
                        {
                            Login = "bourceadmin",
                            Password =  new byte[] { 0x5a,0xd5,0x63,0x95,0x6b,0xb4,0xd9,0x7b,0x67,0xb2,0x3c,0xb4,0xc7,0x09,0xb8,0x35,0x63,0x0d,0x0c,0x5f }
                        }
                    }
                }) },
#endif
                DisplayStorageConnectionString = false,
                AppPath = "https://kohestanimahdi.ir/"
            });
            // #if !DEBUG
            //             StartTasks();
            // #endif
            BackgroundJob.Schedule<TseSymbolDataProvider>(app => app.AddOrUpdateSymbols(CancellationToken.None), TimeSpan.FromMinutes(1));
            BackgroundJob.Schedule<TsetmcCrawlerService>(app => app.UpdateSymbolsAsync(CancellationToken.None), TimeSpan.FromMinutes(2));
        }

        private static void StartTasks()
        {
            //BackgroundJob.Enqueue<UserAndRoleDataInitializer>(app => app.InitializeData());
            //BackgroundJob.Schedule<ITseSymbolDataProvider>(app => app.AddOrUpdateSymbols(CancellationToken.None), TimeSpan.FromMinutes(1));
            //BackgroundJob.Schedule<TsetmcCrawlerService>(app => app.UpdateSymbolsAsync(CancellationToken.None), TimeSpan.FromMinutes(2));
            //RecurringJob.AddOrUpdate<PaymentScheduledTask>(nameof(PaymentScheduledTask), app => app.DoTask(), "*/15 * * * *");
            //RecurringJob.AddOrUpdate<PaymentScheduledTask>(nameof(PaymentScheduledTask), app => app.CheckPaymentsStatus(), "0 */1 * * *");
            //RecurringJob.AddOrUpdate<ReminderScheduledTask>(nameof(ReminderScheduledTask), app => app.DoTask(), "0 5-17 * * *");
            //RecurringJob.AddOrUpdate<TsetmcCrawlerService>(nameof(TsetmcCrawlerService.FillOneTimeDataAsync), app => app.FillOneTimeDataAsync(CancellationToken.None), "0 4 * * *");
        }
    }
}

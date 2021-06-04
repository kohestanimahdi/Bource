﻿using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Bource.Configuration.Hangfire
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

        public static void UseCustomHangfire(this IApplicationBuilder app)
        {
            app.UseHangfireServer();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
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
                DisplayStorageConnectionString = false,
                AppPath = "https://kohestanimahdi.ir/"
            });
#if !DEBUG
            StartTasks();
#endif
            //RecurringJob.AddOrUpdate<PaymentScheduledTask>(nameof(PaymentScheduledTask), app => app.CheckPaymentsStatus(), "0 */1 * * *");
        }

        private static void StartTasks()
        {
            //BackgroundJob.Enqueue<UserAndRoleDataInitializer>(app => app.InitializeData());
            //BackgroundJob.Schedule<TurnDataInitializer>(app => app.InitializeData(), TimeSpan.FromMinutes(1));
            //RecurringJob.AddOrUpdate<PaymentScheduledTask>(nameof(PaymentScheduledTask), app => app.DoTask(), "*/15 * * * *");
            //RecurringJob.AddOrUpdate<PaymentScheduledTask>(nameof(PaymentScheduledTask), app => app.CheckPaymentsStatus(), "0 */1 * * *");
            //RecurringJob.AddOrUpdate<ReminderScheduledTask>(nameof(ReminderScheduledTask), app => app.DoTask(), "0 5-17 * * *");
            //RecurringJob.AddOrUpdate<TurnDataInitializer>(nameof(TurnDataInitializer), app => app.InitializeData(), "0 0 * * 5");
        }
    }
}
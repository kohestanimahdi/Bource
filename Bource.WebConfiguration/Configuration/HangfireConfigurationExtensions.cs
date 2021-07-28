using Bource.Services.Crawlers.AsanBource;
using Bource.Services.Crawlers.Codal360;
using Bource.Services.Crawlers.FipIran;
using Bource.Services.Crawlers.Tsetmc;
using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using StackExchange.Redis;
using System;
using System.Reflection;
using System.Threading;

namespace Bource.WebConfiguration.Configuration
{
    public static class HangfireConfigurationExtensions
    {
        public static void AddCustomHangfire(this IServiceCollection services, string connectionString)
        {
            var mongoUrlBuilder = new MongoUrlBuilder(connectionString);
            var mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());
            var profix = Assembly.GetCallingAssembly().GetName().Name;
            // Add Hangfire services. Hangfire.AspNetCore nuget required
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseMongoStorage(mongoClient, "Hangfire", new MongoStorageOptions
                {
                    MigrationOptions = new MongoMigrationOptions
                    {
                        MigrationStrategy = new MigrateMongoMigrationStrategy(),
                        BackupStrategy = new CollectionMongoBackupStrategy()
                    },
                    Prefix = profix,
                    CheckConnection = true,

                })
            );

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

            StartTasks();
        }


        private static void StartTasks()
        {
            AddMarketTimeTasks();

            // دریافت لیست نمادها در شروع برنامه
            BackgroundJob.Schedule<TseSymbolDataProvider>(app => app.AddOrUpdateSymbols(CancellationToken.None), TimeSpan.FromMinutes(1));


            // دریافت لیست نمادها در ساعت 8 صبح
            RecurringJob.AddOrUpdate<TseSymbolDataProvider>(nameof(TseSymbolDataProvider.AddOrUpdateSymbols),
                app => app.AddOrUpdateSymbols(CancellationToken.None), "0 8 * * 6", TimeZoneInfo.Local);

            // دریافت اطلاعات تکمیلی نمادها در ساعت 8/5 صبح
            RecurringJob.AddOrUpdate<TsetmcCrawlerService>(nameof(TsetmcCrawlerService.UpdateSymbolsAsync),
                app => app.UpdateSymbolsAsync(CancellationToken.None), "5 8 * * 6", TimeZoneInfo.Local);

            // دریافت لیست صنایع در ساعت 10 شب
            RecurringJob.AddOrUpdate<TsetmcCrawlerService>(nameof(TsetmcCrawlerService.GetOrUpdateSymbolGroupsAsync),
                app => app.GetOrUpdateSymbolGroupsAsync(CancellationToken.None), "0 9 * * 5", TimeZoneInfo.Local);

            // دریافت لیست شاخص‌ها در ساعت 10 شب
            RecurringJob.AddOrUpdate<TsetmcCrawlerService>(nameof(TsetmcCrawlerService.GetSymbolsOfIndicatorsAsync),
                app => app.GetSymbolsOfIndicatorsAsync(CancellationToken.None), "1 9 * * 5", TimeZoneInfo.Local);

            // دریافت اطلاعات ارزش بازار روزانه یک بار
            RecurringJob.AddOrUpdate<TsetmcCrawlerService>(nameof(TsetmcCrawlerService.GetValueOfMarketAsync), app =>
            app.GetValueOfMarketAsync(CancellationToken.None), "0 0 * * *", TimeZoneInfo.Local);

            // دریافت اطلاعات حقیقی و حقوقی - روزانه یک مرتبه
            RecurringJob.AddOrUpdate<TsetmcCrawlerService>(nameof(TsetmcCrawlerService.GetAllNaturalAndLegalEntityAsync),
                app => app.GetAllNaturalAndLegalEntityAsync(CancellationToken.None), "0 1 * * *", TimeZoneInfo.Local);

            // دریافت اطلاعات افزایش سرمایه- روزانه یک مرتبه
            RecurringJob.AddOrUpdate<TsetmcCrawlerService>(nameof(TsetmcCrawlerService.GetAllCapitalIncreaseAsync),
                app => app.GetAllCapitalIncreaseAsync(CancellationToken.None), "0 2 * * *", TimeZoneInfo.Local);

            // دریافت اطلاعات برنامه کلاینت
            RecurringJob.AddOrUpdate<TseClientService>(nameof(TseClientService.GetInsturmentsClosingPriceAsync),
                app => app.GetInsturmentsClosingPriceAsync(CancellationToken.None), "0 3 * * *", TimeZoneInfo.Local);

            // دریافت اطلاعات مجامع فیپ ایران - هر به یک ساعت
            RecurringJob.AddOrUpdate<FipiranCrawlerService>(nameof(FipiranCrawlerService.GetAssociations), app =>
            app.GetAssociations(CancellationToken.None), "0 * * * *", TimeZoneInfo.Local);

            // دریافت اطلاعات دنیای اقتصاد فیپ ایران - هر به یک ساعت
            RecurringJob.AddOrUpdate<FipiranCrawlerService>(nameof(FipiranCrawlerService.GetAssociations) + "WorldOfEconomy",
                app => app.GetNews(Models.Data.Enums.FipIranNewsTypes.WorldOfEconomy, CancellationToken.None), "0 * * * *", TimeZoneInfo.Local);

            // دریافت اطلاعات مجامع فیپ ایران - هر به یک ساعت
            RecurringJob.AddOrUpdate<FipiranCrawlerService>(nameof(FipiranCrawlerService.GetAssociations) + "AssembliesAndCompanies",
                app => app.GetNews(Models.Data.Enums.FipIranNewsTypes.AssembliesAndCompanies, CancellationToken.None), "0 * * * *", TimeZoneInfo.Local);

            // دریافت اطلاعات سهامداران هر نماد - یک مرتبه در روز
            RecurringJob.AddOrUpdate<TsetmcCrawlerService>(nameof(TsetmcCrawlerService.GetSymbolsShareHoldersAsync),
                app => app.GetSymbolsShareHoldersAsync(CancellationToken.None), "0 4 * * *", TimeZoneInfo.Local);

            // دریافت تصاویر نمادها - هر هفته یک مرتبه
            RecurringJob.AddOrUpdate<AsanBourceCrawlerService>(nameof(AsanBourceCrawlerService.DownloadSymbolsImageAsync),
                app => app.DownloadSymbolsImageAsync(CancellationToken.None), "0 10 * * 5", TimeZoneInfo.Local);

            // دریافت لینک کدال - هر شب یک بار
            RecurringJob.AddOrUpdate<Codal360CrawlerService>(nameof(Codal360CrawlerService.UpdateSymbolsCodalURLAsync),
                app => app.UpdateSymbolsCodalURLAsync(CancellationToken.None), "0 11 * * 5", TimeZoneInfo.Local);

            // دریافت موضوع هر نماد از فیپ ایران - هر شب یک بار
            RecurringJob.AddOrUpdate<FipiranCrawlerService>(nameof(FipiranCrawlerService.GetSubjectSymbols), app =>
            app.GetSubjectSymbols(CancellationToken.None), "45 11 * * 5", TimeZoneInfo.Local);

            // دریافت اطلاعات لحظه ای هر نماد - روزانه در ابتدای تایم بازار
            RecurringJob.AddOrUpdate<TsetmcCrawlerService>(nameof(TsetmcCrawlerService.FillOneTimeDataAsync), app =>
            app.FillOneTimeDataAsync(CancellationToken.None), "15 8 * * *", TimeZoneInfo.Local);
        }

        private static void AddMarketTimeTasks()
        {
            //باز کردن وضعیت بازار
            RecurringJob.AddOrUpdate<TsetmcCrawlerService>(nameof(TsetmcCrawlerService.SetMarketStatus),
                app => app.SetMarketStatus(null, CancellationToken.None), "0-10 9 * * 0,1,2,3,6", TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate<TsetmcCrawlerService>(nameof(TsetmcCrawlerService.SetMarketStatus) + "9",
                app => app.SetMarketStatus(null, CancellationToken.None), "59 8 * * 0,1,2,3,6", TimeZoneInfo.Local);

            // دریافت اطلاعات پیغام‌های ناظر بازار- در ساعت بازار - هر به یک دقیقه
            RecurringJob.AddOrUpdate<TsetmcCrawlerService>(nameof(TsetmcCrawlerService.GetMarketWatcherMessage),
                app => app.GetMarketWatcherMessage(CancellationToken.None), "0 9-14 * * 0,1,2,3,6", TimeZoneInfo.Local);

            // دریافت اطلاعات سهامداران فعال نمادها - در تایم بازار هر به یک دقیقه
            RecurringJob.AddOrUpdate<TsetmcCrawlerService>(nameof(TsetmcCrawlerService.GetChangeOfSharesOfActiveShareHoldersAsync),
                app => app.GetChangeOfSharesOfActiveShareHoldersAsync(CancellationToken.None), "* 9-14 * * 0,1,2,3,6", TimeZoneInfo.Local);

            // دریافت لحظه ای شاخص های منتخب
            RecurringJob.AddOrUpdate<TsetmcCrawlerService>("GetSelectedIndicatorAsync",
                app => app.GetSelectedIndicatorEverySecondAsync(CancellationToken.None), " * 9-14 * * 0,1,2,3,6", TimeZoneInfo.Local);

            // دریافت لحظه ای برترین عرضه و تقاضا
            RecurringJob.AddOrUpdate<TsetmcCrawlerService>(nameof(TsetmcCrawlerService.GetTopSupplyAndDemandAsync),
                app => app.GetTopSupplyAndDemandEverySecondAsync(CancellationToken.None), "* 9-14 * * 0,1,2,3,6", TimeZoneInfo.Local);

            // دریافت اطلاعات لحظه ای هر نماد - ثانیه ای یک بار
            RecurringJob.AddOrUpdate<TsetmcCrawlerService>(nameof(TsetmcCrawlerService.ScheduleLatestSymbolData),
                app => app.ScheduleLatestSymbolDataEverySecondAsync(CancellationToken.None), "* 9-14 * * 0,1,2,3,6", TimeZoneInfo.Local);

            // دریافت اطلاعات بازار نقدی در یک نگاه - ثانیه ای یک بار
            RecurringJob.AddOrUpdate<TsetmcCrawlerService>(nameof(TsetmcCrawlerService.GetMarketAtGlanceScheduleEverySecondAsync),
                app => app.GetMarketAtGlanceScheduleEverySecondAsync(CancellationToken.None), "* 9-14 * * 0,1,2,3,6", TimeZoneInfo.Local);

        }
    }
}
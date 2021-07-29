using Autofac;
using Autofac.Extensions.DependencyInjection;
using Bource.Common.Models;
using Bource.WebConfiguration.Configuration;
using Bource.WebConfiguration.Middlewares;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Sentry.AspNetCore;
using System.IO;

namespace Bource.JobServer
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(env.ContentRootPath)
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                            .AddEnvironmentVariables();

            Configuration = builder.Build();

            applicationSettings = Configuration.GetSection("ApplicationSettings").Get<ApplicationSetting>();
        }

        private readonly ApplicationSetting applicationSettings;
        public IConfigurationRoot Configuration { get; }
        public ILifetimeScope AutofacContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
                options.AddServerHeader = true;
            });

            services.Configure<ApplicationSetting>(Configuration.GetSection("ApplicationSettings"));
            services.AddSingleton<ApplicationSetting>(applicationSettings);
            services.AddCustomHangfire(Configuration.GetConnectionString("HangfireMongoDB"));
            services.AddControllers();
            services.AddCrawlerHttpClient(applicationSettings);

            services.AddRedisCache(Configuration.GetConnectionString("RedisJobCache"), "RedisJobCache");
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.AddServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Sentry.SentrySdk.CaptureMessage("Hello Sentry");

            AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            app.AddCustomResponseHeaders();

            app.UseCustomExceptionHandler();

            app.UseCors("AllowAllOrigins");

            app.UseCustomHangfire();

            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "Contents")),
                RequestPath = "/contents"
            });

            //app.UseDirectoryBrowser(new DirectoryBrowserOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(
            //    Path.Combine(Directory.GetCurrentDirectory(), "files")),
            //    RequestPath = new PathString("/files")
            //});

            app.UseRouting();

            app.UseAuthorization();

            app.UseSentryTracing();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard();
            });
        }
    }
}

// private readonly IHub _sentryHub;

//     public HomeController(IHub sentryHub) => _sentryHub = sentryHub;

//     [HttpGet("/person/{id}")]
//     public IActionResult Person(string id)
//     {
//         var childSpan = _sentryHub.GetSpan()?.StartChild("additional-work");
//         try
//         {
//             // Do the work that gets measured.

//             childSpan?.Finish(SpanStatus.Ok);
//         }
//         catch (Exception e)
//         {
//             childSpan?.Finish(e);
//             throw;
//         }
//     }
using Bource.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Bource.WebConfiguration.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCustomApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;

                options.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader("version"),
                    new UrlSegmentApiVersionReader(), new HeaderApiVersionReader(new[] { "version" }));
            });
        }

        public static void AddCrawlerHttpClient(this IServiceCollection services, ApplicationSetting applicationSetting)
        {
            HttpClientHandler handler = new()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            foreach (var crawler in applicationSetting.CrawlerSettings)
            {
                services.AddHttpClient(crawler.Key)
                            .ConfigureHttpClient(client =>
                            {
                                client.BaseAddress = new Uri(crawler.Url);
                                client.Timeout = TimeSpan.FromSeconds(crawler.Timeout);
                            }).ConfigurePrimaryHttpMessageHandler(() => handler);
            }
        }
        public static void AddAllowAllOriginsCors(this IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy("AllowAllOrigins", b =>
            {
                b.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            }));
        }


        public static void AddCustomServices(this IServiceCollection services, ApplicationSetting applicationSettings, IConfiguration configuration, string applicationTitle, params Assembly[] assemblies)
        {
            services.Configure<ApplicationSetting>(configuration.GetSection(nameof(ApplicationSetting)));


            //if (applicationSettings.ImportSetting.UseContext)
            //    services.AddDbContext(configuration);

            //services.InitializeAutoMapper(assemblies);

            services.AddAllowAllOriginsCors();

            //services.AddCustomIdentity(applicationSettings.IdentitySettings);

            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });


            //services.AddJwtAuthentication(applicationSettings.JwtSettings);

            services.AddCustomApiVersioning();

            //services.AddSwagger(applicationTitle);

            services.AddOptions();

            services.AddLogging();

        }
    }
}

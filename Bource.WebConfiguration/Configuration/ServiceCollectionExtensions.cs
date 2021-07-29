using Bource.Common.Models;
using Bource.Data;
using Bource.Models.Entities.Users;
using Bource.WebConfiguration.Configuration.Swagger;
using Bource.WebConfiguration.CustomMapping;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Bource.WebConfiguration.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRedisCache(this IServiceCollection services, string connectionString, string instanceName)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = connectionString;
                options.InstanceName = instanceName;
            });
        }

        public static void AddJwtAuthentication(this IServiceCollection services, JwtSetting jwtSettings)
        {
            byte[] ByteKey = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
            byte[] EncryptKey = Encoding.UTF8.GetBytes(jwtSettings.Encryptkey);

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = jwtSettings.RequireHttpsMetadata;
                    cfg.SaveToken = jwtSettings.SaveToken;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        RequireSignedTokens = jwtSettings.RequireSignedTokens,
                        RequireExpirationTime = jwtSettings.RequireExpirationTime,
                        ValidateLifetime = jwtSettings.ValidateLifetime,
                        ValidateAudience = jwtSettings.ValidateAudience,
                        ValidateIssuer = jwtSettings.ValidateIssuer,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        ValidateIssuerSigningKey = jwtSettings.ValidateIssuerSigningKey,
                        IssuerSigningKey = new SymmetricSecurityKey(ByteKey),
                        ClockSkew = TimeSpan.Zero,
                        TokenDecryptionKey = new SymmetricSecurityKey(EncryptKey)
                    };

                    cfg.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = ctx =>
                        {
                            if (ctx.Request.Query.ContainsKey("access_token"))
                                ctx.Token = ctx.Request.Query["access_token"];
                            return Task.CompletedTask;
                        }
                    };
                });
        }

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
            foreach (var crawler in applicationSetting.CrawlerSettings)
            {
                services.AddHttpClient(crawler.Key)
                            .ConfigureHttpClient(client =>
                            {
                                client.BaseAddress = new Uri(crawler.Url);
                                client.Timeout = TimeSpan.FromSeconds(crawler.Timeout);
                            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
                            {
                                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                            });
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

            services.InitializeAutoMapper(assemblies);

            services.AddAllowAllOriginsCors();

            services.AddCustomIdentity<ApplicationDbContext, User, Role>(applicationSettings.IdentitySettings);

            //services.AddMvc()
            //    .AddJsonOptions(options =>
            //    {
            //        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            //    });

            //services.AddCustomReteLimiterServices(Configuration);
            services.AddHttpContextAccessor();

            services.AddJwtAuthentication(applicationSettings.JwtSettings);

            services.AddCustomApiVersioning();

            services.AddSwagger(applicationTitle);

            services.AddOptions();

            services.AddLogging();

            services.AddRedisCache(configuration.GetConnectionString("RedisJobCache"), "RedisJobCache");

        }
    }
}
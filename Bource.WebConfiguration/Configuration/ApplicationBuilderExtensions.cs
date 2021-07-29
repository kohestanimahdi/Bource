using Autofac;
using Autofac.Extensions.DependencyInjection;
using Bource.Common.Utilities;
using Bource.Services.DataInitializer;
using Bource.WebConfiguration.Configuration.Swagger;
using Bource.WebConfiguration.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bource.WebConfiguration.Configuration
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseHsts(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            Assert.NotNull(app, nameof(app));
            Assert.NotNull(env, nameof(env));

            if (!env.IsDevelopment())
                app.UseHsts();
        }

        public static void IntializeDatabase(this IApplicationBuilder app)
        {
            //Use C# 8 using variables
            using var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();

            var dataInitializers = scope.ServiceProvider.GetServices<IDataInitializer>();
            foreach (var dataInitializer in dataInitializers)
                dataInitializer.InitializeData();

        }

        public static void AddCustomResponseHeaders(this IApplicationBuilder app)
        {
            app.Use((context, next) =>
            {
                context.Response.Headers.Add("X-Developed-By", "https://kohestanimahdi.ir/");
                return next.Invoke();
            });
        }

        public static void AddBuilders(this IApplicationBuilder app, IWebHostEnvironment env, ILifetimeScope AutofacContainer)
        {
            AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            app.IntializeDatabase();

            app.AddCustomResponseHeaders();

            app.UseHsts(env);

            app.UseCustomExceptionHandler();

            app.UseSwaggerAndUI();
        }
    }
}
using Autofac.Extensions.DependencyInjection;
using Bource.Common.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Bource.JobServer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            FileExtensions.CreateIfNotExists("Contents/SymbolLogos");

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    //#if !DEBUG
                    //webBuilder.UseSentry();
                    //#endif
                });
    }
}
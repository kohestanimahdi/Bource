using Autofac;
using Bource.Common.Models;
using Bource.Data.Informations.Repositories;
using Bource.Models.Data;
using Bource.Services.Crawlers.Tsetmc;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bource.WebConfiguration.Configuration
{
    public static class AutofacConfigurationExtensions
    {
        public static void AddServices(this ContainerBuilder containerBuilder, params Assembly[] assemblies)
        {

            var allAssemblies = new List<Assembly>
            {
                typeof(ApplicationSetting).Assembly,
                typeof(MongoDataEntity).Assembly,
                typeof(MongoRepository<>).Assembly,
                typeof(TsetmcCrawlerService).Assembly,
                typeof(AutofacConfigurationExtensions).Assembly,
                Assembly.GetEntryAssembly()
            };

            if (!(assemblies is null) && assemblies.Any())
                allAssemblies.AddRange(assemblies);

            containerBuilder.RegisterAssemblyTypes(allAssemblies.ToArray())
                .AssignableTo<IScopedDependency>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterAssemblyTypes(allAssemblies.ToArray())
                .AssignableTo<ITransientDependency>()
                .AsImplementedInterfaces()
                .InstancePerDependency();

            containerBuilder.RegisterAssemblyTypes(allAssemblies.ToArray())
                .AssignableTo<ISingletonDependency>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}

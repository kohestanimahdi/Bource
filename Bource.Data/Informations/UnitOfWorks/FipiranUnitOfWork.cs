using Bource.Common.Models;
using Bource.Data.Informations.Repositories.FipIran;
using Bource.Models.Data.Enums;
using Bource.Models.Data.FipIran;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Data.Informations.UnitOfWorks
{
    public class FipiranUnitOfWork : IFipiranUnitOfWork, ISingletonDependency
    {
        private readonly FipIranNewsRepository fipIranNewsRepository;
        private readonly FipIranAssociationRepository fipIranAssociationRepository;

        public FipiranUnitOfWork(IOptionsSnapshot<ApplicationSetting> options)
        {
            fipIranNewsRepository = new(options.Value.mongoDbSetting);
            fipIranAssociationRepository = new(options.Value.mongoDbSetting);
        }

        public FipiranUnitOfWork(MongoDbSetting mongoDbSetting)
        {
            fipIranNewsRepository = new(mongoDbSetting);
            fipIranAssociationRepository = new(mongoDbSetting);
        }

        public async Task AddIfNotExistNewsAsync(FipIranNewsTypes type, List<FipIranNews> fipIranNews, CancellationToken cancellationToken = default(CancellationToken))
        {
            var lastNews = await fipIranNewsRepository.GetByDateAsync(DateTime.Now.AddDays(-15), type, cancellationToken);
            List<FipIranNews> itemsToAdd = new();
            foreach (var news in fipIranNews)
            {
                if (!lastNews.Any(i => i.Equals(news)))
                    itemsToAdd.Add(news);
            }
            if (itemsToAdd.Any())
                await fipIranNewsRepository.AddRangeAsync(itemsToAdd, cancellationToken);
        }

        public async Task AddIfNotExistAssociationAsync(List<FipIranAssociation> fipIranNews, CancellationToken cancellationToken = default(CancellationToken))
        {
            var lastNews = await fipIranAssociationRepository.GetByDateAsync(DateTime.Now.AddDays(-15), cancellationToken);
            List<FipIranAssociation> itemsToAdd = new();
            foreach (var news in fipIranNews)
            {
                if (!lastNews.Any(i => i.Equals(news)))
                    itemsToAdd.Add(news);
            }
            if (itemsToAdd.Any())
                await fipIranAssociationRepository.AddRangeAsync(itemsToAdd, cancellationToken);
        }
    }
}
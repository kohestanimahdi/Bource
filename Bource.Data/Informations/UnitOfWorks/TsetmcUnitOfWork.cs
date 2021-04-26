﻿using Bource.Common.Models;
using Bource.Data.Informations.Repositories;
using Bource.Models.Data.Common;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Data.Informations.UnitOfWorks
{
    public class TsetmcUnitOfWork : ITsetmcUnitOfWork, ISingletonDependency
    {
        private readonly SymbolGroupRepository symbolGroupRepository;

        public TsetmcUnitOfWork(IOptionsSnapshot<ApplicationSetting> options)
        {
            symbolGroupRepository = new SymbolGroupRepository(options.Value.mongoDbSetting);
        }

        public TsetmcUnitOfWork(MongoDbSetting mongoDbSetting)
        {
            symbolGroupRepository = new SymbolGroupRepository(mongoDbSetting);
        }

        public async Task AddOrUpdateSymbolGroups(List<SymbolGroup> symbolGroups, CancellationToken cancellationToken = default(CancellationToken))
        {
            var allGroups = await symbolGroupRepository.GetAllAsync(cancellationToken);

            foreach (var newGroup in symbolGroups)
            {
                var group = allGroups.FirstOrDefault(i => i.Code == newGroup.Code);
                if (group is not null)
                {
                    group.Title = newGroup.Title;
                    await symbolGroupRepository.UpdateAsync(group, cancellationToken);
                }
                else
                    await symbolGroupRepository.AddAsync(newGroup);
            }
        }
    }
}

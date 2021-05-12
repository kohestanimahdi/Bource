using Bource.Common.Models;
using Bource.Models.Data.Enums;
using Bource.Models.Data.Tsetmc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Data.Informations.Repositories.Tsetmc
{
    public class MarketWatcherMessageRepository : MongoRepository<MarketWatcherMessage>
    {
        public MarketWatcherMessageRepository(MongoDbSetting dbSetting) : base(dbSetting)
        {
        }

        public Task<List<MarketWatcherMessage>> GetTodayMessagesAsync(MarketType market, CancellationToken cancellationToken = default(CancellationToken))
        => Table.Find(i => i.Market == market && i.Time > DateTime.Today).ToListAsync(cancellationToken);
    }
}
using Bource.Common.Models;
using Bource.Common.Utilities;
using Bource.Models.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Data.Informations.Repositories
{
    public class MongoRepository<TEntity>
        where TEntity : MongoDataEntity
    {
        public IMongoCollection<TEntity> Table { get; }
        protected IMongoDatabase mongoDatabase { private set; get; }

        public MongoRepository(MongoDbSetting dbSetting)
        {
            var mongoClient = new MongoClient(dbSetting.ConnectionString);

            this.mongoDatabase = mongoClient.GetDatabase(dbSetting.DataBaseName);

            var tableName = typeof(TEntity).Name.PluralizingNameConvention();

            Table = mongoDatabase.GetCollection<TEntity>(tableName);

            var properties = typeof(TEntity).GetProperties();
            var insCodeProperty = properties.FirstOrDefault(i => i.Name.ToLower().Equals("inscode"));
            if (insCodeProperty is not null)
                CreateAscendingIndex(insCodeProperty.Name);

            var insCodeValueProperty = properties.FirstOrDefault(i => i.Name.ToLower().Equals("inscodevalue"));
            if (insCodeValueProperty is not null)
                CreateAscendingIndex(insCodeValueProperty.Name);

        }




        protected void CreateAscendingIndex(string fieldName)
        {
            var symbolBuilder = Builders<TEntity>.IndexKeys;
            var indexModel = new CreateIndexModel<TEntity>(symbolBuilder.Ascending(fieldName));

            Table.Indexes.CreateOne(indexModel);
        }

        #region Async Method

        public virtual Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken))
            => Table.Find(i => true).ToListAsync(cancellationToken);

        public virtual Task<TEntity> GetByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Table.Find(i => i.Id == id).SingleOrDefaultAsync(cancellationToken);
        }

        public virtual Task AddAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            Assert.NotNull(entity, nameof(entity));
            return Table.InsertOneAsync(entity, cancellationToken: cancellationToken);
        }

        public virtual async Task<bool> AnyAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (await Table.Find(i => i.Id == id).SingleOrDefaultAsync(cancellationToken)) != null;
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken))
        {
            Assert.NotNull(entities, nameof(entities));
            if (entities.Any())
                await Table.InsertManyAsync(entities, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public virtual Task UpdateAsync(string id, TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            Assert.NotNull(entity, nameof(entity));
            return Table.ReplaceOneAsync(i => i.Id == id, entity,
                cancellationToken: cancellationToken);
        }

        public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            Assert.NotNull(entity, nameof(entity));
            return Table.ReplaceOneAsync(i => i.Id == entity.Id, entity,
                cancellationToken: cancellationToken);
        }

        public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken))
        {
            Assert.NotNull(entities, nameof(entities));
            foreach (var entity in entities)
                await UpdateAsync(entity.Id, entity, cancellationToken);
        }

        public virtual Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            Assert.NotNull(entity, nameof(entity));
            return Table.DeleteOneAsync(i => i.Id == entity.Id, cancellationToken: cancellationToken);
        }

        public virtual Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken))
        {
            Assert.NotNull(entities, nameof(entities));
            return Table.DeleteManyAsync(ItemWithListOfId(entities.Select(i => new ObjectId(i.Id))), cancellationToken: cancellationToken);
        }

        protected FilterDefinition<TEntity> ItemWithListOfId(IEnumerable<ObjectId> id)
        {
            return Builders<TEntity>.Filter.In("_id", id);
        }

        #endregion Async Method

        #region Sync Methods

        public virtual List<TEntity> GetAll()
        {
            return Table.Find(i => true).ToList();
        }

        public virtual TEntity GetById(string id)
        {
            return Table.Find(i => i.Id == id).SingleOrDefault();
        }

        public virtual void Add(TEntity entity)
        {
            Assert.NotNull(entity, nameof(entity));
            Table.InsertOne(entity);
        }

        public virtual bool Any(string id)
        {
            return Table.Find(i => i.Id == id).SingleOrDefault() != null;
        }

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            Assert.NotNull(entities, nameof(entities));
            Table.InsertMany(entities);
        }

        public virtual void Update(string id, TEntity entity)
        {
            Assert.NotNull(entity, nameof(entity));
            Table.ReplaceOne(i => i.Id == id, entity);
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {
            Assert.NotNull(entities, nameof(entities));
            foreach (var entity in entities)
                Update(entity.Id, entity);
        }

        public virtual void Delete(TEntity entity)
        {
            Assert.NotNull(entity, nameof(entity));
            Table.DeleteOne(i => i.Id == entity.Id);
        }

        public virtual void DeleteRange(IEnumerable<TEntity> entities)
        {
            Assert.NotNull(entities, nameof(entities));
            Table.DeleteMany(ItemWithListOfId(entities.Select(i => new ObjectId(i.Id))));
        }

        #endregion Sync Methods
    }
}
using Bource.Models.Data;

namespace Bource.Data.Informations.Repositories
{
    public interface IMongoRepository<TEntity> where TEntity : MongoDataEntity
    {
    }
}
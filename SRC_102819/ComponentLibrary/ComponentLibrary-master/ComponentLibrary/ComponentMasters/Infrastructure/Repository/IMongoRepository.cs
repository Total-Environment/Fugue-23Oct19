using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository
{

    [Obsolete("Use MongoCollection directly in the repository")]
    public interface IMongoRepository<T> : IQueryable<T> where T : Entity
    {
        Task<T> Add(T entity);
        Task Delete(string id);
        Task<T> FindBy(string columnName, object value);
        Task<T> FindBy(Expression<Func<T, bool>> f);
        Task<T> GetById(string id);
        Task<T> Increment(T entity, string value);
        Task<IEnumerable<T>> FindAllBy(Expression<Func<T, bool>> f);
        Task<T> Update(T entity);
    }
}
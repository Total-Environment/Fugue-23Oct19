using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository
{
    public class MongoRepository<T> : IMongoRepository<T>, IQueryable<T> where T : Entity
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(IMongoCollection<T> collection)
        {
            _collection = collection;
        }

        public Type ElementType => _collection.AsQueryable().ElementType;

        public Expression Expression => _collection.AsQueryable().Expression;

        public IQueryProvider Provider => _collection.AsQueryable().Provider;

        public async Task<T> Add(T entity)
        {
            Func<T, Task<T>> action = async e =>
            {
                e.ObjectId = ObjectId.GenerateNewId();
                await _collection.InsertOneAsync(e);
                return e;
            };
            return await new RetryAsyncExecutor<T, T>().WithResult(action, entity);
        }

        public async Task Delete(string id)
        {
            Func<string, Task> action = async e =>
            {
                await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", e));
            };
            await new RetryAsyncExecutor<string, T>().WithoutResult(action, id);
        }

        public async Task<IEnumerable<T>> FindAllBy(Expression<Func<T, bool>> f)
        {
            Func<Expression<Func<T, bool>>, Task<IEnumerable<T>>> action = async e =>
            {
                return (await _collection.FindAsync(e)).Current;
            };
            return await new RetryAsyncExecutor<Expression<Func<T, bool>>, IEnumerable<T>>().WithResult(action, f);
        }

        public async Task<T> FindBy(string columnName, object value)
        {
            Func<Tuple<string, object>, Task<T>> action = async e =>
             {
                 var result = await _collection.FindAsync(Builders<T>.Filter.Eq(e.Item1, e.Item2));
                 return result.FirstOrDefault();
             };
            return await new RetryAsyncExecutor<Tuple<string, object>, T>().WithResult(action, new Tuple<string, object>(columnName, value));
        }

        public async Task<T> FindBy(Expression<Func<T, bool>> f)
        {
            Func<Expression<Func<T, bool>>, Task<T>> action = async e =>
            {
                return (await _collection.FindAsync(e)).FirstOrDefault();
            };
            return await new RetryAsyncExecutor<Expression<Func<T, bool>>, T>().WithResult(action, f);
        }

        public async Task<T> GetById(string id)
        {
            Func<string, Task<T>> action = async i =>
            {
                ObjectId objectId;
                var succeeded = ObjectId.TryParse(i, out objectId);
                if (!succeeded)
                    throw new ArgumentException("Invalid ID");
                return await GetById(objectId);
            };
            return await new RetryAsyncExecutor<string, T>().WithResult(action, id);
        }

        public async Task<T> GetById(ObjectId objectId)
        {
            Func<ObjectId, Task<T>> action = async e =>
            {
                return await FindBy(r => r.ObjectId == e);
            };
            return await new RetryAsyncExecutor<ObjectId, T>().WithResult(action, objectId);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _collection.AsQueryable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _collection.AsQueryable().GetEnumerator();
        }

        public async Task<T> Increment(T entity, string column)
        {
            Func<Tuple<T, string>, Task<T>> action = async e =>
             {
                 await _collection.UpdateOneAsync(
                 Builders<T>.Filter.Eq("_id", e.Item1.ObjectId),
                 Builders<T>.Update.Inc(e.Item2, 1));
                 return await GetById(e.Item1.ObjectId);
             };
            return await new RetryAsyncExecutor<Tuple<T, string>, T>().WithResult(action, new Tuple<T, string>(entity, column));
        }

        public async Task<T> Update(T entity)
        {
            Func<T, Task<T>> action = async e =>
            {
                await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", e.ObjectId), e);
                return e;
            };
            return await new RetryAsyncExecutor<T, T>().WithResult(action, entity);
        }
    }

    internal class RetryAsyncExecutor<T, TO>
    {
        private const int RetryCount = 5;
        private const int SleepTime = 5 * 1000;

        public async Task<TO> WithResult(Func<T, Task<TO>> action, T input)
        {
            int retryCounter = 1;
            while (retryCounter < RetryCount)
            {
                try
                {
                    return await action(input);
                }
                catch (MongoConnectionException)
                {
                    retryCounter++;
                    await Task.Delay(SleepTime);
                    if (retryCounter == RetryCount)
                    {
                        throw;
                    }
                }
            }
            return default(TO);
        }

        internal async Task WithoutResult(Func<T, Task> action, T input)
        {
            int retryCounter = 1;
            while (retryCounter < 5)
            {
                try
                {
                    await action(input);
                }
                catch (MongoConnectionException)
                {
                    retryCounter++;
                    await Task.Delay(1000);
                    if (retryCounter == 5)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
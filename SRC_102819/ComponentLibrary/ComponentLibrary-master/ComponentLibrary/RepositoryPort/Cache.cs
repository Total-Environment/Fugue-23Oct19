using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
    /// <summary>
    /// The Cache
    /// </summary>
    public static class Cache
    {
        /// <summary>
        /// Gets or add to cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="adder">The adder.</param>
        /// <returns></returns>
        public static async Task<T> GetOrAdd<T>(string name, Func<Task<T>> adder)
        {
            var cacheKey = GetCacheKey<T>(name);
            var cache = MemoryCache.Default;
            if (cache.Contains(cacheKey))
                return (T) cache.Get(cacheKey);
            var newEntry = await adder();
            MemoryCache.Default.AddOrGetExisting(cacheKey, newEntry, DateTimeOffset.Now.AddYears(1));
            return newEntry;
        }

        /// <summary>
        /// Removes the specified name from cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        public static void Remove<T>(string name)
        {
            MemoryCache.Default.Remove(GetCacheKey<T>(name));
        }

        private static string GetCacheKey<T>(string name)
        {
            return $"{typeof(T).Name}_{name}".ToLowerInvariant();
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public static void Clear()
        {
            MemoryCache.Default.Dispose();
        }
    }
}
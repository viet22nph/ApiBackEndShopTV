using Models.DTOs.Cart;
using System;
using System.Threading.Tasks;

namespace Caching
{
    public interface ICacheManager : IDisposable
    {
        Task<string> GetAsync(string cacheKey);
        Task<T> GetAsync<T>(string key, Func<Task<T>> acquire, int? cacheTime = null);

        T Get<T>(string key, Func<T> acquire, int? cacheTime = null);
        /// <summary>
        /// Increment a field value in a hash
        /// </summary>
        /// <param name="key">Redis key</param>
        /// <param name="data">Data save redis</param>
        /// <param name="cacheTime">FromMinutes cache expired</param>
        Task SetAsync(string key, object data, int cacheTime);
        void Set(string key, object data, int cacheTime);
        bool IsSet(string key);
        void Remove(string key);
        void RemoveByPrefix(string prefix);
        void Clear();

        /// <summary>
        /// Increment a field value in a hash
        /// </summary>
        /// <param name="key">Redis key</param>
        /// <param name="hashField">Hash field</param>
        /// <param name="value">Increment value</param>
        Task<long> HashIncrementAsync(string key, string hashField, long value = 1);

        /// <summary>
        /// Hash value in a hash
        /// </summary>
        /// <param name="key">Redis key</param>
        /// <returns name="Dictionary<string, long>">Return data</returns>
        Task<Dictionary<string, long>> HashGetAllAsync(string key);

        /// <summary>
        /// Remove an Hash 
        /// </summary>
        /// <param name="key">Redis key</param>
        /// <param name="hashField">Hash field representing the item</param>
        Task<bool> RemoveHashAsync(string key, string hashField);

        /// <summary>
        /// Get hash value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <returns>async Type T value</returns>
        Task<long> GetHashAsync(string key, string hashField);
    }
}

using Models.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Caching
{
    public partial class RedisCacheManager : ICacheManager
    {
        #region Fields

        private readonly IRedisConnectionWrapper _connectionWrapper;
        private readonly IDatabase _db;
        private readonly RedisSettings _config;
        
        #endregion

        #region Ctor

        public RedisCacheManager(IRedisConnectionWrapper connectionWrapper, RedisSettings config)
        {

            _config = config;
            if (string.IsNullOrEmpty(_config.RedisConnectionString))
                throw new Exception("Redis connection string is empty");

            _connectionWrapper = connectionWrapper;

            _db = _connectionWrapper.GetDatabase(_config.RedisDatabaseId ?? (int)RedisDatabaseNumber.Cache);
        
        }

        #endregion

        #region Utilities

        protected virtual IEnumerable<RedisKey> GetKeys(EndPoint endPoint, string prefix = null)
        {
            var server = _connectionWrapper.GetServer(endPoint);

            var keys = server.Keys(_db.Database, string.IsNullOrEmpty(prefix) ? null : $"{prefix}*");

            keys = keys.Where(key => !key.ToString().Equals(_config.RedisDataProtectionKey, StringComparison.OrdinalIgnoreCase));

            return keys;
        }

        protected virtual async Task<T> GetAsync<T>(string key)
        {
            //get serialized item from cache
            var serializedItem = await _db.StringGetAsync(key);
            if (!serializedItem.HasValue)
                return default(T);

            //deserialize item
            var item = JsonConvert.DeserializeObject<T>(serializedItem);
            if (item == null)
                return default(T);

            return item;
        }

        protected virtual async Task<bool> IsSetAsync(string key)
        {
            return await _db.KeyExistsAsync(key);
        }

        #endregion

        #region Methods

        public async Task<string> GetAsync(string cacheKey)
        {
            var cachedResponse = await _db.StringGetAsync(cacheKey);

            if (cachedResponse.IsNullOrEmpty)
            {
                return null;
            }

            return cachedResponse;
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> acquire, int? cacheTime = null)
        {
            //item already is in cache, so return it
            if (await IsSetAsync(key))
                return await GetAsync<T>(key);

            //or create it using passed function
            var result = await acquire();

            //and set in cache (if cache time is defined)
            if ((cacheTime ?? _config.CacheTime) > 0)
                await SetAsync(key, result, cacheTime ?? _config.CacheTime);

            return result;
        }

        public virtual T Get<T>(string key)
        {

            //get serialized item from cache
            var serializedItem = _db.StringGet(key);
            if (!serializedItem.HasValue)
                return default(T);

            //deserialize item
            var item = JsonConvert.DeserializeObject<T>(serializedItem);
            if (item == null)
                return default(T);


            return item;
        }

        public virtual T Get<T>(string key, Func<T> acquire, int? cacheTime = null)
        {
            //item already is in cache, so return it
            if (IsSet(key))
                return Get<T>(key);

            //or create it using passed function
            var result = acquire();

            //and set in cache (if cache time is defined)
            if ((cacheTime ?? _config.CacheTime) > 0)
                Set(key, result, cacheTime ?? _config.CacheTime);

            return result;
        }

        public async Task SetAsync(string key, object data, int cacheTime)
        {
            if (data == null)
                return;

            //set cache time
            var expiresIn = TimeSpan.FromMinutes(cacheTime);

            //serialize item
            var serializedItem = JsonConvert.SerializeObject(data, new JsonSerializerSettings()
            {
                 ContractResolver = new CamelCasePropertyNamesContractResolver(),
            });

            //and set it to cache
            await _db.StringSetAsync(key, serializedItem, expiresIn);
        }

        public virtual void Set(string key, object data, int cacheTime)
        {
            if (data == null)
                return;

            //set cache time
            var expiresIn = TimeSpan.FromMinutes(cacheTime);

            //serialize item
            var serializedItem = JsonConvert.SerializeObject(data);

            //and set it to cache
            _db.StringSet(key, serializedItem, expiresIn);
        }

        public virtual bool IsSet(string key)
        {

            return _db.KeyExists(key);
        }

        public virtual void Remove(string key)
        {
            //we should always persist the data protection key list
            if (key.Equals(_config.RedisDataProtectionKey, StringComparison.OrdinalIgnoreCase))
                return;

            //remove item from caches
            _db.KeyDelete(key);
        }

        public virtual void RemoveByPrefix(string prefix)
        {

            foreach (var endPoint in _connectionWrapper.GetEndPoints())
            {
                var keys = GetKeys(endPoint, prefix);

                _db.KeyDelete(keys.ToArray());
            }
        }

        public virtual void Clear()
        {
            foreach (var endPoint in _connectionWrapper.GetEndPoints())
            {
                var keys = GetKeys(endPoint).ToArray();

                _db.KeyDelete(keys);
            }
        }

        public virtual void Dispose()
        {
            if (_connectionWrapper != null)
            {
                _connectionWrapper.Dispose();
            }
            // Dispose(true);
        }

        private void Dispose(bool clear)
        {
            GC.SuppressFinalize(this);
        }


        #endregion
        public async Task<long> HashIncrementAsync(string key, string hashField, long value = 1)
        {
            return await _db.HashIncrementAsync(key, hashField, value);
        }
        public async Task<Dictionary<string, long>> HashGetAllAsync(string key)
        {
            var entries = await _db.HashGetAllAsync(key);

            // Convert the Redis hash entries to a dictionary
            return entries.ToDictionary(
                entry => (string)entry.Name,
                entry => (long)entry.Value
            );
        }
        public async Task<bool> RemoveHashAsync(string key, string hashField)
        {
            return await _db.HashDeleteAsync(key, hashField);
        }

        public async Task<long> GetHashAsync(string key, string hashField)
        {
            var serializedItem = await _db.HashGetAsync(key, hashField);
            return serializedItem.HasValue ? (long)serializedItem : 0;
        }


        #region New Methods for Tracking Visits and Searches
        public async Task IncrementVisitCountAsync()
        {
            var key = $"visits:{DateTime.UtcNow:yyyyMMdd}";
            await _db.StringIncrementAsync(key);
        }
        public async Task IncrementUniqueVisitorCountAsync(string ip)
        {
            var key = $"unique_visits:{ip}:{DateTime.UtcNow:yyyyMMdd}";
            await _db.StringSetAsync(key, DateTime.UtcNow.ToString(), TimeSpan.FromDays(1));
        }
        public async Task<long> GetVisitCountAsync(DateTime date)
        {
            var key = $"visits:{date:yyyyMMdd}";
            return (long)await _db.StringGetAsync(key);
        }

        public async Task<long> GetUniqueVisitorCountAsync(DateTime date)
        {
            var server = _connectionWrapper.GetServer(_connectionWrapper.GetEndPoints().First());
            var pattern = $"unique_visits:*:{date:yyyyMMdd}";
            var keys = server.Keys(pattern: pattern);
            return keys.LongCount();
        }

      
        public async Task IncrementProductViewCountAsync(Guid productId)
        {
            string key = $"productViews:{DateTime.UtcNow:yyyyMMdd}:{productId}";
            await _db.StringIncrementAsync(key);
        }
        public async Task<long> GetProductViewCountAsync(Guid productId, DateTime date)
        {
            string key = $"productViews:{date:yyyyMMdd}:{productId}";
            var count = await _db.StringGetAsync(key);
            return count.HasValue ? (long)count : 0;
        }
        public async Task<long> GetTotalProductViewCountAsync(Guid productId)
        {
            long totalCount = 0;
            var keys = _db.Multiplexer.GetServer(_db.Multiplexer.GetEndPoints().First()).Keys(pattern: $"productViews:*:{productId}");

            foreach (var key in keys)
            {
                var count = await _db.StringGetAsync(key);
                totalCount += count.HasValue ? (long)count : 0;
            }

            return totalCount;
        }
        public async Task<Dictionary<Guid, long>> GetTopViewedProductsAsync(DateTime startDate, DateTime endDate)
        {
            var productViewCounts = new Dictionary<Guid, long>();

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var keys = _db.Multiplexer.GetServer(_db.Multiplexer.GetEndPoints().First()).Keys(pattern: $"productViews:{date:yyyyMMdd}:*");

                foreach (var key in keys)
                {
                    var keyParts = key.ToString().Split(':');
                    if (keyParts.Length == 3 && Guid.TryParse(keyParts[2], out var productId))
                    {
                        var count = await _db.StringGetAsync(key);
                        if (count.HasValue)
                        {
                            if (productViewCounts.ContainsKey(productId))
                            {
                                productViewCounts[productId] += (long)count;
                            }
                            else
                            {
                                productViewCounts[productId] = (long)count;
                            }
                        }
                    }
                }
            }

            // Sắp xếp các sản phẩm theo số lượt xem giảm dần
            return productViewCounts.OrderByDescending(p => p.Value).ToDictionary(p => p.Key, p => p.Value);
        }
        #endregion
    }
}

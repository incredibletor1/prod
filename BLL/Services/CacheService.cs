using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class CacheService : ICacheService
    {
        private IMemoryCache _cache;
        private MemoryCacheEntryOptions cacheOptions;
        private IUnitOfWork _unitOfWork;

        private const int cacheSlidingTimeoutInMinutes = 5;

        public CacheService(IMemoryCache memoryCache, IUnitOfWork unitOfWork)
        {
            this._cache = memoryCache;
            _unitOfWork = unitOfWork;

            cacheOptions = new MemoryCacheEntryOptions();
            cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(cacheSlidingTimeoutInMinutes));
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            Func<string[], Task<User>> dbGetMethod = async (param) => (await _unitOfWork.Users.GetUserById(Convert.ToInt32(param[0])));

            return await GetUserFromCacheByKey("userById", dbGetMethod, userId.ToString());
        }

        private async Task<User> GetUserFromCacheByKey(string keyName, Func<string[], Task<User>> methodToGetFromDB, params string[] keyParams)
        {
            return await GetFromCacheByKey<User>(keyName, methodToGetFromDB, keyParams);
        }

        private async Task<T> GetFromCacheByKey<T>(string keyName, Func<string[], Task<T>> methodToGetFromDB, params string[] keyParams)
        {
            // Build cache key from params
            var key = BuiltCacheKey(keyName, keyParams);

            // Look for the key in cache
            T valueFromCache;
            if (!_cache.TryGetValue<T>(key, out valueFromCache))
            {
                // Key not found in cache: get data from database & add it to cache
                valueFromCache = await methodToGetFromDB(keyParams);
                _cache.Set(key, valueFromCache, cacheOptions);
            }

            return valueFromCache;
        }

        private string BuiltCacheKey(string keyName, params string[] keyParams)
        {
            var key = keyName + ">" + string.Join(',', keyParams);
            return key;
        }
    }
}

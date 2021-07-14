using System;
using Microsoft.Extensions.Caching.Memory;

namespace my_new_app.Helpers.Cache
{
    public class InMemoryHandler : ICacheHelper
    {
        private readonly IMemoryCache cacheObj;
        public InMemoryHandler(IMemoryCache _cacheObj)
        {
            this.cacheObj = _cacheObj;
        }
        public object GetItem(string key)
        {
            if (string.IsNullOrEmpty(key)) 
            { return null; }
            
            object result;
            if (cacheObj.TryGetValue(key, out result))
            { return result; }
            return null;
        }

        public void SetItem(string key, object value)
        {
            cacheObj.Set(key, value);
        }

        public void RemoveItem(string key)
        {
            cacheObj.Remove(key);
        }

        public void SetItem(string key, object value, DateTime expireAt)
        {
            var expOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = expireAt,
                Priority = CacheItemPriority.Normal
            };
            cacheObj.Set(key, value, expOptions);
        }

        public void Exists(string key)
        {
            throw new NotImplementedException();
        }
    }
}
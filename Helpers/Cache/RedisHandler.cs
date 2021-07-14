using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Extensions.Caching.Distributed;

namespace my_new_app.Helpers.Cache
{
    public class RedisHandler : ICacheHelper
    {
        private readonly IDistributedCache cacheObj;
        public RedisHandler(IDistributedCache cacheObj)
        {
            this.cacheObj = cacheObj;
        }
        public object GetItem(string key)
        {
            byte[] result = cacheObj.Get(key);
            if (result == null) { return null; }
            return ByteArrayToObject(result);
        }
        public void SetItem(string key, object value)
        {
            cacheObj.Set(key, ObjectToByteArray(value));
        }
        public void SetItem(string key, object value, DateTime expireAt)
        {
            var expOptions = new DistributedCacheEntryOptions { AbsoluteExpiration = expireAt };
            cacheObj.Set(key, ObjectToByteArray(value), expOptions);
        }

        private byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
        private Object ByteArrayToObject(byte[] arrBytes)
        {
            Object obj;
            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                obj = (Object)binForm.Deserialize(memStream);
            }
            return obj;
        }

        public void RemoveItem(string key)
        {
            throw new NotImplementedException();
        }

        public void Exists(string key)
        {
            throw new NotImplementedException();
        }
    }

}
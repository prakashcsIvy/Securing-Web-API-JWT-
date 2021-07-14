using System;

namespace my_new_app.Helpers.Cache
{
    public interface ICacheHelper
    {
        void SetItem(string key, object value);
        void SetItem(string key, object value, DateTime expireAt);
        object GetItem(string key);
        void RemoveItem(string key);
        void Exists(string key);
    }
}
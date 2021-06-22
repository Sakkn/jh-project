using System;
using System.Collections.Generic;
using System.Text;

namespace jh_project
{
    public interface IJhMemoryCache
    {

        void DeleteByKey(string key);
        void FlushDatabase();
        string GetString(string key, TimeSpan? expiry);
        bool KeyExists(string key);
        void SaveString(string key, string value, TimeSpan? expiry);

    }
}

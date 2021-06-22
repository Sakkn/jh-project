using jh_project;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jh_test
{
    public class MockRedisCache :  IJhMemoryCache
    {

        #region Public Methods

        public void DeleteByKey(string key)
        {
            // do nothing
        }

        public void FlushDatabase()
        {
            // do nothing
        }

        public string GetString(string key, TimeSpan? expiry)
        {
            return null;
        }

        public bool KeyExists(string key)
        {
            return false;
        }

        public void InitializeConnection(string connectionString)
        {
            // do nothing
        }

        public void SaveString(string key, string value, TimeSpan? expiry)
        {
            // do nothing
        }

        #endregion

    }
}

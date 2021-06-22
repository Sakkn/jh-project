using StackExchange.Redis;
using System;
using System.Net;

namespace jh_project
{
    public class RedisCache : IJhMemoryCache
    {
        
        public static IJhMemoryCache Singleton = new RedisCache();

        #region Properties

        private ConnectionMultiplexer RedisConnection { get; set; }
        private IDatabase RedisDatabase { get; set; }

        #endregion

        #region Public Methods

        public void DeleteByKey(string key)
        {
            RedisDatabase.KeyDelete(key);
        }

        public void FlushDatabase()
        {
            foreach (EndPoint endpoint in RedisConnection.GetEndPoints())
            {
                RedisConnection.GetServer(endpoint).FlushDatabase();
            }
        }

        public string GetString(string key, TimeSpan? expiry)
        {
            RedisDatabase.KeyExpire(key, expiry);
            return RedisDatabase.StringGet(key);
        }

        public bool KeyExists(string key)
        {
            return RedisDatabase.KeyExists(key);
        }

        public void InitializeConnection(string connectionString)
        {
            try
            {
                RedisConnection = ConnectionMultiplexer.Connect(connectionString);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            // guard clause - connection failed
            if (RedisConnection == null || RedisConnection.IsConnected == false)
            {
                throw new Exception("Failed to connect to Redis cache.");
            }

            RedisDatabase = RedisConnection.GetDatabase();

            // guard clause - database not found
            if (RedisDatabase == null)
            {
                throw new Exception("Failed to connect to Redis cache.");
            }
        }

        public void SaveString(string key, string value, TimeSpan? expiry)
        {
            RedisDatabase.StringSet(key, value);
            RedisDatabase.KeyExpire(key, expiry);
        }

        #endregion

    }
}

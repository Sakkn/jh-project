using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Tweetinvi;
using Tweetinvi.Models.V2;
using Tweetinvi.Streaming.V2;

namespace jh_project
{
    public class JhTwitterClient : IJhClient
    {

        #region Constructors

         public JhTwitterClient(string key, string secret, string token)
        {
            Key = key;
            Secret = secret;
            Token = token;
            TweetCache = new ConcurrentBag<TweetV2>();
        }

        #endregion

        #region Properties
        
        private string Key { get; set; }
        private string Secret { get; set; }
        private string Token { get; set; }
        private TwitterClient TwitterClient {get;set;}

        private ConcurrentBag<TweetV2> TweetCache { get; set; }

        #endregion

        #region Public Methods

        public void Initialize()
        {
            TwitterClient = new TwitterClient(Key, Secret, Token);
        }

        public void BeginStream(IJhSubject subject)
        {
            int batchSize = GetBatchSize();
            int currentCount = 1;

            ISampleStreamV2 sampleStream = TwitterClient.StreamsV2.CreateSampleStream();
            sampleStream.TweetReceived += (sender, args) =>
            {
                Console.WriteLine(args.Tweet.Text); 
              //  Console.WriteLine(currentCount++);
                TweetCache.Add(args.Tweet);
                if (currentCount >= batchSize)
                {
                    subject.Notify(TweetCache);
                    currentCount = 1;
                }

                currentCount++;
            };

            sampleStream.StartAsync().GetAwaiter().GetResult();
        }

        #endregion

        #region Helper Methods

        private int GetBatchSize()
        {
            int batchSize = Convert.ToInt32(Environment.GetEnvironmentVariable(JhConstants.BATCH_SIZE));

            if (batchSize == null || batchSize <= 0)
            {
                JhLogger.Singleton.Log(JhLogLevel.Info, "Batching size not found, defaulting to 100.");
                return 100;
            }

            return batchSize;
        }

        #endregion

    }
}

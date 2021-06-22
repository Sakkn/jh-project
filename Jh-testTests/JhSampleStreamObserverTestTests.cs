using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jh_test;
using System;
using System.Collections.Generic;
using System.Text;
using jh_project;
using System.Collections.Concurrent;
using Tweetinvi.Models.V2;

namespace Jh_test
{
    [TestClass()]
    public class JhSampleStreamObserverTestTests
    {
        
        #region BlueSky Tests

        [TestMethod()]
        public void TestBlueSky_SampleStreamObserver()
        {
            // setup 
            RedisCache.Singleton = new MockRedisCache();
            JhLogger.Singleton = new MockJhLogger();
            JhSampleStreamObserver observer = new JhSampleStreamObserver();

            // exercise
           List<string> results = observer.Update(CreateTestTweetCache());

            // results
            Assert.IsNotNull(results);
            Assert.AreEqual(results[1], "Total Tweets: 101");
            Assert.AreEqual(results[2], "Top Trending Hashtag: TestTag");
            Assert.AreEqual(results[3], "Contains URL Percentage: 50");
            Assert.AreEqual(results[4], "Contains Photo URL Percentage: 0");
            Assert.AreEqual(results[5], "Top Trending Domain: www.w3.org");
        }

        #endregion

        #region Helper Methods

        private ConcurrentBag<TweetV2> CreateTestTweetCache()
        {
            ConcurrentBag<TweetV2> tweetCache = new ConcurrentBag<TweetV2>();

            for (int i = 0; i < 101; i++)
            {
                TweetV2 tweet = new TweetV2();
                tweet.CreatedAt = new DateTimeOffset();
                tweet.Entities = CreateTestEntity();
                tweet.Text = "Test tweet";
                tweetCache.Add(tweet);
            }

            return tweetCache;
        }

        private TweetEntitiesV2 CreateTestEntity()
        {
            TweetEntitiesV2 entity = new TweetEntitiesV2();

            HashtagV2 hashTag = new HashtagV2();
            hashTag.Hashtag = "TestTag";
            entity.Hashtags = new HashtagV2[] { hashTag };

            UrlV2 url = new UrlV2();
            url.Url = "http://www.w3.org/albert/bertram/marie-claude";
            entity.Urls = new UrlV2[] { url };

            return entity;
        }

        #endregion
    }
}
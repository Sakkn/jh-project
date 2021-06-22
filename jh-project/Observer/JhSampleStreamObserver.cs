using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Tweetinvi.Core.Helpers;
using Tweetinvi.Models.V2;

namespace jh_project
{
    public class JhSampleStreamObserver : IJhObserver
    {

        #region Constructors

        public JhSampleStreamObserver()
        {
            TotalTweets = 0;
            TweetsPerHour = 0;
            TweetsPerMinute = 0;
            TweetsPerSecond = 0;
            TopEmoji = string.Empty;
            TopEmojiCount = 0;
            EmojiPercentage = 0;
            TopHashtag = string.Empty;
            TopHashTagCount = 0;
            UrlPercentage = 0;
            PhotoPercentage = 0;
            TopDomain = string.Empty;
            TopDomainCount = 0;
        }

        #endregion

        #region Properties

        private int TotalTweets { get; set; }
        private double TweetsPerHour { get; set; }
        private double TweetsPerMinute { get; set; }
        private double TweetsPerSecond { get; set; }
        private string TopEmoji { get; set; }
        private int TopEmojiCount { get; set; }
        private decimal EmojiPercentage { get; set; }
        private string TopHashtag { get; set; }
        private int TopHashTagCount { get; set; }
        private decimal UrlPercentage { get; set; }
        private decimal PhotoPercentage { get; set; }
        private string TopDomain { get; set; }
        private int TopDomainCount { get; set; }

        #endregion

        #region Public Methods

        public List<string> Update(ConcurrentBag<TweetV2> tweetCache)
        {
            lock(tweetCache)
            {
                TotalTweets = tweetCache.Count;
                SetTweetRate(tweetCache);
                SetTopHashtag(tweetCache);
                UrlPercentage = GetPercentageOfUrlTweets(tweetCache);
                PhotoPercentage = GetPercentageOfPhotoTweets(tweetCache);
                SetTopDomain(tweetCache);
                SetEmojiInformation(tweetCache);
            }

            SaveResultsToRedis();
           return ReportResults();
        }

        #endregion

        #region Helper Methods

        private decimal GetPercentageOfPhotoTweets(ConcurrentBag<TweetV2> tweetCache)
        {
            int totalTweets = tweetCache.Count;
            int photoTweets = 0;

            foreach (TweetV2 tweet in tweetCache)
            {
                // guard clause
                if (tweet.Entities.Urls == null)
                {
                    continue;
                }

                foreach (UrlV2 url in tweet.Entities.Urls)
                {
                    if (url.Url.Contains("pic") || url.Url.Contains("pix") || url.Url.Contains("img") || url.Url.Contains("image"))
                    {
                        photoTweets++;
                    }
                }
            }

            if (photoTweets <= 0)
            {
                return 0;
            }

            decimal firstNumber = (decimal)photoTweets / (decimal)totalTweets;
            return firstNumber * 100;
        }

        private decimal GetPercentageOfUrlTweets(ConcurrentBag<TweetV2> tweetCache)
        {
            int totalTweets = tweetCache.Count;
            int urlTweets = 0;

            foreach (TweetV2 tweet in tweetCache)
            {
                // guard clause
                if (tweet.Entities.Urls == null)
                {
                    continue;
                }

                urlTweets += tweet.Entities.Urls.Length;
            }

            if (urlTweets <= 0)
            {
                return 0;
            }

            decimal firstNumber = (decimal)urlTweets / (decimal)totalTweets;
            return firstNumber * 100;
        }
 
        private List<string> ReportResults()
        {
            List<string> lines = new List<string>();
            string currentDateTime = DateTime.Now.ToString("MM-dd-yyyy-HH-mm-ss");

            lines.Add("Twitter Sample Stream Report: " + currentDateTime);
            lines.Add("Total Tweets: " + TotalTweets.ToString());
            lines.Add("Top Trending Hashtag: " + TopHashtag);
            lines.Add("Contains URL Percentage: " + UrlPercentage.ToString());
            lines.Add("Contains Photo URL Percentage: " + PhotoPercentage.ToString());
            lines.Add("Top Trending Domain: " + TopDomain);
            lines.Add("Tweet Rate Per Hour: " + TweetsPerHour.ToString());
            lines.Add("Tweet Rate Per Minute: " + TweetsPerMinute.ToString());
            lines.Add("Tweet Rate Per Second: " + TweetsPerSecond.ToString());
            lines.Add("Top Emoji: " + TopEmoji);
            lines.Add("Contains Emoji Percentage: " + EmojiPercentage.ToString());

            JhLogger.Singleton.Log(JhLogLevel.Info, "Writing report to file.");
            Directory.CreateDirectory("Reports");
            File.WriteAllLines("Reports/Report_" + currentDateTime + ".txt", lines);

            return lines;
        }

        private void SetTopDomain(ConcurrentBag<TweetV2> tweetCache)
        {
            List<string> domains = new List<string>();

            foreach (TweetV2 tweet in tweetCache)
            {
                // guard clause
                if (tweet.Entities.Urls == null)
                {
                    continue;
                }

                foreach (UrlV2 url in tweet.Entities.Urls)
                {
                    Uri uri = new Uri(url.Url);
                    domains.Add(uri.Host);
                }
            }

            if (domains.Count <=0)
            {
                return;
            }

            var groupsWithCounts = from domain in domains
                                   group domain by domain into g
                                   select new
                                   {
                                       Item = g.Key,
                                       Count = g.Count()
                                   };

            var groupsSorted = groupsWithCounts.OrderByDescending(g => g.Count);

            TopDomain = groupsSorted.First().Item.ToString();
            TopDomainCount = groupsSorted.First().Count;
        }

        private void SetEmojiInformation(ConcurrentBag<TweetV2> tweetCache)
        {
            Regex emojiRegex = new Regex(JhConstants.EMOJI_REGEX);
            List<string> emojis = new List<string>();
            int tweetsContainingEmojis = 0;

            foreach (TweetV2 tweet in tweetCache)
            {
                MatchCollection emojiCollection = emojiRegex.Matches(tweet.Text);
                Match[] foundEmojis = new Match[emojiCollection.Count];
                emojiCollection.CopyTo(foundEmojis, 0);

                if (foundEmojis.Length > 0)
                {
                    tweetsContainingEmojis++;
                }

                foreach (Match emoji in foundEmojis)
                {
                    emojis.Add(emoji.Value);
                }
            }

            if (emojis.Count <=0)
            {
                return;
            }

            var groupsWithCounts = from emoji in emojis
                                   group emoji by emoji into g
                                   select new
                                   {
                                       Item = g.Key,
                                       Count = g.Count()
                                   };

            var groupsSorted = groupsWithCounts.OrderByDescending(g => g.Count);

            TopEmoji = groupsSorted.First().Item.ToString();
            TopEmojiCount = groupsSorted.First().Count;
            
            decimal firstNumber = ((decimal)tweetsContainingEmojis / (decimal)tweetCache.Count);
            EmojiPercentage = firstNumber * 100;
        }

        private void SetTopHashtag(ConcurrentBag<TweetV2> tweetCache)
        {
            List<string> hashTags = new List<string>();

            foreach (TweetV2 tweet in tweetCache)
            {
                // guard clause
                if (tweet.Entities.Hashtags == null)
                {
                    continue;
                }

                foreach (HashtagV2 hashTag in tweet.Entities.Hashtags)
                {
                    hashTags.Add(hashTag.Tag);
                }
            }

            if (hashTags.Count <=0)
            {
                return;
            }

            var groupsWithCounts = from hashTag in hashTags
                                   group hashTag by hashTag into g
                                   select new
                                   {
                                       Item = g.Key,
                                       Count = g.Count()
                                   };

            var groupsSorted = groupsWithCounts.OrderByDescending(g => g.Count);

            TopHashtag = groupsSorted.First().Item.ToString();
            TopHashTagCount = groupsSorted.First().Count;
        }

        private void SetTweetRate(ConcurrentBag<TweetV2> tweetCache)
        {
            int tweetCount = tweetCache.Count;
            List<DateTimeOffset> dates = new List<DateTimeOffset>();

            foreach (TweetV2 tweet in tweetCache)
            {
                dates.Add(tweet.CreatedAt);
            }

            List<DateTimeOffset> orderedList = dates.OrderByDescending(d => d.UtcDateTime).ToList();
            TimeSpan timespan = orderedList.First() - orderedList.Last();
            double totalHours = timespan.TotalHours;
            double totalMinutes = timespan.TotalMinutes;
            double totalSeconds = timespan.TotalSeconds;

            TweetsPerHour = tweetCount / totalHours;
            TweetsPerMinute = tweetCount / totalMinutes;
            TweetsPerSecond = tweetCount / totalSeconds;

        }

        #endregion

        #region Helper Methods - Redis

        private void SaveResultsToRedis()
        {
            TimeSpan expiration = TimeSpan.FromDays(1);

            SaveTweetTotals(expiration);
            SaveTopHashtag(expiration);
            SaveUrlPercentage(expiration);
            SavePhotoPercentage(expiration);
            SaveTopDomain(expiration);
            SaveEmojiPercentage(expiration);
            SaveTopEmoji(expiration);
        }

        private void SavePhotoPercentage(TimeSpan expiration)
        {
            decimal redisPhotoPercentage = Convert.ToDecimal(RedisCache.Singleton.GetString(JhConstants.PHOTO_PERCENTAGE, expiration));
            if (redisPhotoPercentage == null)
            {
                RedisCache.Singleton.SaveString(JhConstants.PHOTO_PERCENTAGE, PhotoPercentage.ToString(), expiration);
            }
            else
            {
                PhotoPercentage = (PhotoPercentage + redisPhotoPercentage) / 2;
                RedisCache.Singleton.SaveString(JhConstants.PHOTO_PERCENTAGE, PhotoPercentage.ToString(), expiration);
            }
        }

        private void SaveTopDomain(TimeSpan expiration)
        {
            string redisTopDomain = RedisCache.Singleton.GetString(JhConstants.TOP_HASHTAG, expiration);
            int redisTopDomainCount = Convert.ToInt32(RedisCache.Singleton.GetString(JhConstants.TOP_HASHTAG_COUNT, expiration));
            if (redisTopDomain == null || redisTopDomainCount == null)
            {
                RedisCache.Singleton.SaveString(JhConstants.TOP_DOMAIN, TopDomain, expiration);
                RedisCache.Singleton.SaveString(JhConstants.TOP_DOMAIN_COUNT, TopDomainCount.ToString(), expiration);
            }
            else
            {
                if (redisTopDomainCount < TopDomainCount)
                {
                    RedisCache.Singleton.SaveString(JhConstants.TOP_DOMAIN, TopDomain, expiration);
                    RedisCache.Singleton.SaveString(JhConstants.TOP_DOMAIN_COUNT, TopDomainCount.ToString(), expiration);
                }
            }
        }

        private void SaveTopEmoji(TimeSpan expiration)
        {
            string redisTopEmoji = RedisCache.Singleton.GetString(JhConstants.TOP_EMOJI, expiration);
            int redisTopEmojiCount = Convert.ToInt32(RedisCache.Singleton.GetString(JhConstants.TOP_EMOJI_COUNT, expiration));
            if (redisTopEmoji == null || redisTopEmojiCount == null)
            {
                RedisCache.Singleton.SaveString(JhConstants.TOP_EMOJI, TopEmoji, expiration);
                RedisCache.Singleton.SaveString(JhConstants.TOP_EMOJI_COUNT, TopEmojiCount.ToString(), expiration);
            }
            else
            {
                if (redisTopEmojiCount < TopDomainCount)
                {
                    RedisCache.Singleton.SaveString(JhConstants.TOP_EMOJI, TopEmoji, expiration);
                    RedisCache.Singleton.SaveString(JhConstants.TOP_EMOJI_COUNT, TopEmojiCount.ToString(), expiration);
                }
            }
        }

        private void SaveEmojiPercentage(TimeSpan expiration)
        {
            decimal redisEmojiPercentage = Convert.ToDecimal(RedisCache.Singleton.GetString(JhConstants.EMOJI_PERCENTAGE, expiration));
            if (redisEmojiPercentage == null)
            {
                RedisCache.Singleton.SaveString(JhConstants.EMOJI_PERCENTAGE, EmojiPercentage.ToString(), expiration);
            }
            else
            {
                EmojiPercentage = (EmojiPercentage + redisEmojiPercentage) / 2;
                RedisCache.Singleton.SaveString(JhConstants.EMOJI_PERCENTAGE, EmojiPercentage.ToString(), expiration);
            }
        }

        private void SaveTopHashtag(TimeSpan expiration)
        {
            string redisTopHashtag = RedisCache.Singleton.GetString(JhConstants.TOP_HASHTAG, expiration);
            int redisTopHashtagCount = Convert.ToInt32(RedisCache.Singleton.GetString(JhConstants.TOP_HASHTAG_COUNT, expiration));
            if (redisTopHashtag == null || redisTopHashtagCount == null)
            {
                RedisCache.Singleton.SaveString(JhConstants.TOP_HASHTAG, TopHashtag, expiration);
                RedisCache.Singleton.SaveString(JhConstants.TOP_HASHTAG_COUNT, TopHashTagCount.ToString(), expiration);
            }
            else
            {
                if (redisTopHashtagCount < TopHashTagCount)
                {
                    RedisCache.Singleton.SaveString(JhConstants.TOP_HASHTAG, TopHashtag, expiration);
                    RedisCache.Singleton.SaveString(JhConstants.TOP_HASHTAG_COUNT, TopHashTagCount.ToString(), expiration);
                }
            }
        }

        private void SaveTweetTotals(TimeSpan expiration)
        {
            string redisTotalTweets = RedisCache.Singleton.GetString(JhConstants.TOTAL_TWEETS, expiration);
            if (redisTotalTweets == null)
            {
                RedisCache.Singleton.SaveString(JhConstants.TOTAL_TWEETS, TotalTweets.ToString(), expiration);
            }
            else
            {
                TotalTweets += Convert.ToInt32(redisTotalTweets);
                RedisCache.Singleton.SaveString(JhConstants.TOTAL_TWEETS, TotalTweets.ToString(), expiration);
            }
        }

        private void SaveUrlPercentage(TimeSpan expiration)
        {
            decimal redisUrlPercentage = Convert.ToDecimal(RedisCache.Singleton.GetString(JhConstants.URL_PERCENTAGE, expiration));
            if (redisUrlPercentage == null)
            {
                RedisCache.Singleton.SaveString(JhConstants.URL_PERCENTAGE, UrlPercentage.ToString(), expiration);
            }
            else
            {
                UrlPercentage = (UrlPercentage + redisUrlPercentage) / 2;
                RedisCache.Singleton.SaveString(JhConstants.URL_PERCENTAGE, UrlPercentage.ToString(), expiration);
            }
        }

        #endregion

    }
}

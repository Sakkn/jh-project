using System;
using System.Collections.Generic;
using System.Text;

namespace jh_project
{
    public class JhClientFactory : IJhClientFactory
    {

        public static IJhClientFactory Singleton { get; set; }

        #region Public Methods

        public IJhClient CreateTwitterClient()
        {
            string authToken = Environment.GetEnvironmentVariable(JhConstants.AUTHORIZATION_TOKEN);
            string consumerKey = Environment.GetEnvironmentVariable(JhConstants.CONSUMER_KEY);
            string consumerSecret = Environment.GetEnvironmentVariable(JhConstants.CONSUMER_SECRET);

            return new JhTwitterClient(consumerKey, consumerSecret, authToken);
        }

        #endregion

    }
}

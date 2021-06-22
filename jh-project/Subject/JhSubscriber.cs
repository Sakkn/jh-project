using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Tweetinvi;
using Tweetinvi.Models.V2;
using Tweetinvi.Streaming.V2;

namespace jh_project
{
    public class JhSubscriber : IJhSubject
    {

        #region Constructors

        public JhSubscriber()
        {
            Observers = new List<IJhObserver>();
        }

        #endregion

        #region Properties

        private List<IJhObserver> Observers { get; set; }

        #endregion

        #region Public Methods

        public void Attach(IJhObserver observer)
        {
            if (observer == null)
            {
                return;
            }
            Observers.Add(observer);
        }

        public void Detach(IJhObserver observer)
        {
            if (observer == null)
            {
                return;
            }
            Observers.Remove(observer);
        }

        public void Notify(ConcurrentBag<TweetV2> tweetCache)
        {
            foreach (IJhObserver observer in Observers)
            {
                observer.Update(tweetCache);
            }

            tweetCache.Clear();
        }

        public void ReadStream()
        {
            Thread streamReadThread = new Thread(new ThreadStart(ReadStreamThread));
            streamReadThread.Start();
        }

        #endregion

        #region Helper Methods

        private async void ReadStreamThread()
        {
            IJhClient client = JhClientFactory.Singleton.CreateTwitterClient();
            client.Initialize();
            client.BeginStream(this);
        }

        #endregion
    }
}
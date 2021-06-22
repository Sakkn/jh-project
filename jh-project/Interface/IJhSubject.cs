using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Tweetinvi.Models.V2;

namespace jh_project
{
    public interface IJhSubject
    {

        void Attach(IJhObserver observer);
        void Detach(IJhObserver observer);
        void Notify(ConcurrentBag<TweetV2> tweetCache);

        void ReadStream();

    }
}

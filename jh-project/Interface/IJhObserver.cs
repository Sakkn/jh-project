using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Tweetinvi.Models.V2;

namespace jh_project
{
    public interface IJhObserver
    {

        List<string> Update(ConcurrentBag<TweetV2> tweetCache);

    }
}

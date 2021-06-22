using System;
using System.Collections.Generic;
using System.Text;
using Tweetinvi.Streaming.V2;

namespace jh_project
{
    public interface IJhClient
    {

        public void Initialize();
        public void BeginStream(IJhSubject subject);

    }
}

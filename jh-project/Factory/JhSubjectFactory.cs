using System;
using System.Collections.Generic;
using System.Text;

namespace jh_project
{
    public class JhSubjectFactory : IJhSubjectFactory
    {

        public static JhSubjectFactory Singleton { get; set; }

        #region Public Methods

        public IJhSubject CreateSampleStreamSubject()
        {
            JhSubscriber subscriber = new JhSubscriber();
            JhSampleStreamObserver sampleStreamObserver = new JhSampleStreamObserver();
            subscriber.Attach(sampleStreamObserver);
            return subscriber;
        }

        #endregion

    }
}

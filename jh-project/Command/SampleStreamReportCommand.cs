using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jh_project
{
    public class SampleStreamReportCommand : IJhCommand
    {

        #region Constructors

        public SampleStreamReportCommand()
        {
            Subscribers = new List<IJhSubject>();
        }

        #endregion

        #region Properties

        private List<IJhSubject> Subscribers { get; set; }

        #endregion

        #region Public Methods

        public Task Execute()
        {
            foreach (IJhSubject subscriber in Subscribers)
            {
                subscriber.ReadStream();
            }

           return Task.CompletedTask;
        }

        public void Initialize()
        {
            IJhSubject subscriber = JhSubjectFactory.Singleton.CreateSampleStreamSubject();
            Subscribers.Add(subscriber);
        }

        #endregion

        #region Helper Methods

        #endregion

    }
}

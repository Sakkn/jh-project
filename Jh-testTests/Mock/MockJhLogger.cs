using jh_project;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jh_test
{
    public class MockJhLogger : ILogger
    {

        public void Log(JhLogLevel level, string message)
        {
            // do nothing
        }

        public void Shutdown()
        {
            // do nothing
        }

    }
}

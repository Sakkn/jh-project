using System;
using System.Collections.Generic;
using System.Text;

namespace jh_project
{
    public interface ILogger
    {

        void Log(JhLogLevel level, string message);
        void Shutdown();

    }
}

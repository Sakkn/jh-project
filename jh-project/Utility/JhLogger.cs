using NLog;
using NLog.Config;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace jh_project
{
    public class JhLogger : ILogger
    {

        public static ILogger Singleton { get; set; }

        #region Constructors

        public JhLogger()
        {
            JhInternalLogger = NLogBuilder.ConfigureNLog("Utility/nlog.config").GetCurrentClassLogger();
        }

        #endregion

        #region Properties

        Logger JhInternalLogger { get; set; }

        #endregion

        #region Public Methods

        public void Log(JhLogLevel loglevel, string message)
        {
            switch (loglevel)
            {
                case JhLogLevel.Info:
                    JhInternalLogger.Info(message);
                    break;
                case JhLogLevel.Debug:
                    JhInternalLogger.Debug(message);
                    break;
                case JhLogLevel.Error:
                    JhInternalLogger.Error(message);
                    break;
                case JhLogLevel.Fatal:
                    JhInternalLogger.Fatal(message);
                    break;
                default:
                    JhInternalLogger.Trace(message);
                    break;
            }
        }

        public void Shutdown()
        {
            LogManager.Shutdown();
        }

        #endregion

    }
}

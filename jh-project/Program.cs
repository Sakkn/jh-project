using System;
using System.Threading;

namespace jh_project
{
    class Program
    {

        #region Public Methods

        static void Main(string[] args)
        {
            try
            {
                if (!AttemptToInitialize())
                {
                    return;
                }

                JhCommandRunner commandRunner = new JhCommandRunner();
                commandRunner.StackCommands();
                commandRunner.Run();

                JhLogger.Singleton.Shutdown();
            }
            catch (Exception ex)
            {
                JhLogger.Singleton.Shutdown();
                DisplayAndExit();
            }

        }

        #endregion

        #region Helper Methods

        private static bool AttemptToInitialize()
        {
            JhLogger.Singleton = new JhLogger();
            JhLogger.Singleton.Log(JhLogLevel.Info, "Initializing...");
            JhClientFactory.Singleton = new JhClientFactory();
            JhSubjectFactory.Singleton = new JhSubjectFactory();

            try
            {
                RedisCache redisCache = new RedisCache();
                redisCache.InitializeConnection(GetFormattedConnectionString());
                RedisCache.Singleton = redisCache;
            }
            catch (Exception ex)
            {
                JhLogger.Singleton.Log(JhLogLevel.Fatal, "Failed to initialize Redis.");
                return false;
            }

            return ValidateCorrectUsage();
        }

        private static void DisplayAndExit()
        {
            Console.WriteLine("Jh processor encountered an error and needs to shutdown.");
            Thread.Sleep(5000); // to read error message
        }

        private static string GetFormattedConnectionString()
        {
            string endpoint = Environment.GetEnvironmentVariable(JhConstants.REDIS_ENDPOINT);
            string port = Environment.GetEnvironmentVariable(JhConstants.REDIS_PORT);

            return string.Format(endpoint + ":" + port);
        }

        private static bool ValidateCorrectUsage()
        {
            bool isValid = true;

            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(JhConstants.CONNECTION_ENDPOINT)))
            {
                JhLogger.Singleton.Log(JhLogLevel.Fatal, JhConstants.CONNECTION_ENDPOINT + "Environment Variable is invalid.");
                isValid = false;
            }

            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(JhConstants.AUTHORIZATION_TOKEN)))
            {
                JhLogger.Singleton.Log(JhLogLevel.Fatal, JhConstants.AUTHORIZATION_TOKEN + "Environment Variable is invalid.");
                isValid = false;
            }

            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(JhConstants.CONSUMER_KEY)))
            {
                JhLogger.Singleton.Log(JhLogLevel.Fatal, JhConstants.CONSUMER_KEY + "Environment Variable is invalid.");
                isValid = false;
            }

            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(JhConstants.CONSUMER_SECRET)))
            {
                JhLogger.Singleton.Log(JhLogLevel.Fatal, JhConstants.CONSUMER_SECRET + "Environment Variable is invalid.");
                isValid = false;
            }

            return isValid;
        }

        #endregion

    }
}

using NLog;

namespace LoggingSystem
{
    public static class LogHelper
    {
        public static Logger logger { get; }

        static LogHelper()
        {
            logger = LogManager.GetCurrentClassLogger();
        }
    }
}
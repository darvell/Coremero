using System;
using NLog;

namespace Coremero
{
    public static class Log
    {
        private static ILogger _logger;

        private static ILogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = LogManager.GetLogger("Coremero");
                }
                return _logger;
            }
        }

        public static void Trace(string message)
        {
            Logger.Trace(message);
        }

        public static void Debug(string message)
        {
            Logger.Debug(message);
        }

        public static void Info(string message)
        {
            Logger.Info(message);
        }

        public static void Warn(string message)
        {
            Logger.Warn(message);
        }

        public static void Exception(Exception e, string message = null)
        {
            Logger.Error(e, message);
        }

        public static void Error(string message)
        {
            Logger.Error(message);
        }

        public static void Fatal(string message)
        {
            Logger.Fatal(message);
        }

        public static void FatalException(Exception e, string message = null)
        {
            Logger.Fatal(e, message);
        }
    }
}
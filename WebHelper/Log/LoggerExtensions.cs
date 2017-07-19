using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Configuration;


namespace WebHelper.Log
{

    /// <summary>
    /// ILogger extension (so we can use ILogger as is without implementation)
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Wrap AppendLogMessage method
        /// </summary>
        private class LoggerWrap : Logger
        {
            public static bool IsRun = false;
            public LoggerWrap()
            {
                var isRun = ConfigurationManager.AppSettings["IsRun"];
                IsRun = String.IsNullOrEmpty(isRun) ? true : isRun.ToUpper().Equals("YES");
            }

            public void WriteMessage(string message, params object[] args)
            {
                if (IsRun)
                {
                    Logger.AppendLogMessage(message, args);
                }
            }
        }

        private static object _lockObj = new object();
        private static LoggerWrap _logger = null;
        private static LoggerWrap Log
        {
            get
            {
                if (_logger == null)
                {
                    lock (_lockObj)
                    {
                        if (_logger == null)
                        {
                            _logger = new LoggerWrap(); //Sigleton
                        }
                    }
                }

                return _logger;
            }
        }
        /// <summary>
        /// Logging message
        /// If arg[0] is LogType <see cref="LogType"/>
        /// than it writes according to log type
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="arg"></param>
        [Conditional("DEBUG")]
        public static void Message(this ILogger logger, string message, params object[] arg)
        {
            Log.WriteMessage(message, arg);
        }
    }
}

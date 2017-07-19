using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace WebHelper.Log
{
    /// <summary>
    /// Simple logger
    /// </summary>
    public class Logger
    {
        private static readonly string currentDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToString("yyyy_MM"));
        private static readonly string fileName = String.Format("log_{0}.txt", DateTime.Now.ToString("yyyy_MM_dd__HH_mm_ss"));
        private static string logFile = Path.Combine(currentDirectory, fileName);
        private static readonly object lockObj = new object();
        public static string LogFilePath
        {
            get
            {
                return logFile;
            }
            set
            {
                logFile = Path.Combine(value, fileName);
            }
        }

        protected static void AppendLogMessage(string message, params object[] arg)
        {
            lock (lockObj)
            {
                Log(message, arg, logFile);
            }
        }

        private static void Log(string message, object[] args, string logFile)
        {
            FileUtils.CreateDirectoryIfNotExists(Path.GetDirectoryName(LogFilePath));
            using (StreamWriter writer = FileUtils.GetOrCreateFile(logFile))
            {
                if (writer == StreamWriter.Null)
                {
                    return;
                }

                string logLine = String.Format("{0}\t{1}{2}", DateTime.Now.ToString("s"), message, Environment.NewLine);
                if (args!= null && args.Length > 0)
                {
                    if (args[0] is LogType)
                    {
                        SetLogLine(args, ref logLine);
                    }
                    else
                    {
                        logLine += String.Format("{0}{1}", string.Join(",", args.Cast<string>().ToArray()), Environment.NewLine);
                    }
                }

                using (TextWriter textWriter = TextWriter.Synchronized(writer))
                {
                    textWriter.WriteLine(logLine);
                }
            }
        }

        private static void SetLogLine(object[] args, ref string startLogLine)
        {
            StringBuilder logLine = new StringBuilder(startLogLine);
            var logType = (LogType)args[0];
            var messArgs = args.Skip(1);
            List<string> messA = new List<string>();
            foreach (var item in messArgs)
            {
                messA.Add(item.ToString());
            }

            var messStr = String.Join(",", messA.ToArray());
            
            switch (logType)
            {
                case LogType.String:
                    logLine.AppendLine(messStr);
                    break;
                case LogType.Exception:
                    foreach (var argItem in messArgs)
                    {
                        if (argItem is Exception)
                        {
                            var theException = argItem as Exception;
                            logLine.AppendLine(GetExceptionInfo(theException));
                        }
                    }
                    break;
                case LogType.Json:
                    foreach (var argItem in messArgs)
                    {
                        var jsonStr = argItem.ToString();
                        logLine.AppendLine(FormatJson(jsonStr));
                    }
                    break;
                default:
                    logLine.AppendLine(logType.ToString()+","+messStr);
                    break;
            }

            startLogLine = logLine.ToString();
        }

        private static string GetExceptionInfo(Exception theException)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(String.Format("Message: {0}", theException.Message));
            stringBuilder.AppendLine((theException.InnerException != null ? "Inner stack trace: " + theException.InnerException.StackTrace : string.Empty));
            stringBuilder.AppendLine(String.Format("Stack trace: {0}", theException.StackTrace));
            stringBuilder.AppendLine(String.Format("Source: {0}", theException.Source));

            return stringBuilder.ToString();
        }

        private static string FormatJson(string json)
        {
            var token = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(token, Formatting.Indented);
        }
    }
}

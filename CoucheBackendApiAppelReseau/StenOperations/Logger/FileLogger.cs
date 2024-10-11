using StenOperations.Utils;

namespace StenOperations.Logger
{
    public class FileLogger: ILogger
    {
        public string LogFilePath { get; set; }

        public Func<string>? Prefix { get; set; }

        public FileLogger(string logFilePath = "output.log", Func<string>? prefix = null)
        {
            LogFilePath = logFilePath;
            Prefix = prefix;
        }

        public void Log(string format, params object?[] args)
        {
            if (Prefix == null)
            {
                Logger.Log(LogFilePath, format, args);
            }
            else
            {
                Logger.Log(LogFilePath, Prefix.Invoke() + " " + format, args);
            }
        }

        public void Log(Exception ex)
        {
            Log("{0}: {1}", ex.GetType().FullName, ex.ExceptionStackTraces());
        }

    }
}

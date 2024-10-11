namespace Connexion.Log
{
    public class FileLogger
    {
        public string LogFilePath { get; set; }

        public Func<string>? Prefix { get; set; }

        public FileLogger(string logFilePath, Func<string>? prefix = null)
        {
            LogFilePath = logFilePath;
            Prefix = prefix;
        }

        public void Log(string format, params object?[] args)
        {
            if (Prefix == null)
            {
                LogUtils.Log(LogFilePath, format, args);
            }
            else
            {
                LogUtils.Log(LogFilePath, Prefix.Invoke() + " " + format, args);
            }
        }

        public void LogAll(string[] logs)
        {
            if (Prefix == null)
            {
                LogUtils.LogAll(LogFilePath, logs);
            }
            else
            {
                LogUtils.LogAll(LogFilePath, logs, Prefix.Invoke());
            }
        }
    }
}

using StenOperations.Utils;

namespace StenOperations.Logger
{
    public class FileAndConsoleLogger : ILogger
    {
        private readonly ConsoleLogger _consoleLogger;
        private readonly FileLogger _fileLogger;

        public Func<string>? Prefix
        {
            get { return _consoleLogger.Prefix; }
            set
            {
                _consoleLogger.Prefix = value;
                _fileLogger.Prefix = value;
            }
        }

        public string LogFilePath
        {
            get { return _fileLogger.LogFilePath; }
            set { _fileLogger.LogFilePath = value; }
        }

        public FileAndConsoleLogger(string logFilePath, Func<string>? prefix = null)
        {
            _consoleLogger = new ConsoleLogger(prefix);
            _fileLogger = new FileLogger(logFilePath, prefix);
        }

        public void Log(string format, params object?[] args)
        {
            _fileLogger?.Log(format, args);
            _consoleLogger?.Log(format, args);
        }

        public void Log(Exception ex)
        {
            Log("{0}: {1}", ex.GetType().FullName, ex.ExceptionStackTraces());
        }
    }
}

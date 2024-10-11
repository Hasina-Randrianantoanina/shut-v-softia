namespace StenOperations.Logger
{
    public static class Logger
    {
        /// <summary>
        /// Append log message to logFilePath file
        /// </summary>
        /// <param name="logFilePath">The path to the log file</param>
        /// <param name="format">A composite format string</param>
        /// <param name="args">An object array that contains zero or more objects to format and write</param>
        public static void Log(string logFilePath, string format, params object?[] args)
        {
            using (var sw = File.AppendText(logFilePath))
            {
                sw.WriteLine(format, args);
                sw.Close();
            }
        }
    }
}

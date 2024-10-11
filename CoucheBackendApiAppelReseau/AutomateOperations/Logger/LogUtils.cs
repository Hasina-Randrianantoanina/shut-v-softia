using AutomateOperations.Utils;

namespace AutomateOperations.Logger
{
    public static class LogUtils
    {
        /// <summary>
        /// Append log message to logFilePath file
        /// </summary>
        /// <param name="logFilePath">The path to the log file</param>
        /// <param name="format">A composite format string</param>
        /// <param name="args">An object array that contains zero or more objects to format and write</param>
        public static void Log(string logFilePath, string format, params object?[] args)
        {
            try
            {
                using (StreamWriter streamWriter = File.AppendText(logFilePath))
                {
                    streamWriter.WriteLine(format, args);
                }

            }
            catch (Exception ex)
            {
                PrintUtils.PrintConsoleException(ex, $"Erreur lors de l'écriture du fichier log {logFilePath}");
                throw new Exception($"Erreur lors de l'écriture du fichier log {logFilePath}", ex);
            }

        }

        public static void LogAll(string logFilePath, string[] logs, string? prefix = null)
        {
            try
            {
                using (StreamWriter streamWriter = File.AppendText(logFilePath))
                {
                    foreach (string log in logs)
                    {
                        if (log == null) { continue; }
                        if (prefix == null)
                        {
                            streamWriter.WriteLine(log);
                        }
                        else
                        {
                            streamWriter.WriteLine(prefix + " " + log);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                PrintUtils.PrintConsoleException(ex, $"Erreur lors de l'écriture du fichier log {logFilePath}");
                throw new Exception($"Erreur lors de l'écriture du fichier log {logFilePath}", ex);
            }

        }

    }
}

using ConnexionTR.Utils;

namespace ConnexionTR.Log
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
                PrintUtils.PrintConsoleException(ex, $"Error when writing the log file {logFilePath} (method Log)");
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
                PrintUtils.PrintConsoleException(ex, $"Error when writing the log file {logFilePath} (method LogAll)");
            }

        }

        /// <summary>
        /// -- errorCode (exception) : operation --<br/>
        /// 202 (SocketException) : creation of the TCP server<br/>
        /// 203 (ArgumentNullException) : creation of the TCP server<br/>
        /// 2011 (?) : ping of the server/router<br/>
        /// 3005 (IOException) : exchanges with the TCP server<br/>
        /// 0 (?) : global unknow exception<br/>
        /// -1 (?) : reading the version<br/>
        /// -2 (?) : reading the analogic config<br/>
        /// -100 (?) : interaction with database (retrieving station)<br/>
        /// -101 (?) : interaction with database (retrieving voies_telemesurees)<br/>
        /// </summary>
        public static void LogException(string logFilePath, Exception exception, string operation, int errorCode, string? prefix = null)
        {
            try
            {
                using (StreamWriter streamWriter = File.AppendText(logFilePath))
                {
                    string errorTxt = errorCode <= 0 ? "" : $" (erreur {errorCode})";
                    string txt = $"{prefix} Arrêt appel sur exception levée lors de l'opération {operation} : {exception.GetType().Name}{errorTxt}";
                    streamWriter.WriteLine(txt);
                }

            }
            catch (Exception ex)
            {
                PrintUtils.PrintConsoleException(ex, $"Error when writing the log file {logFilePath} (method LogException)");
            }
        }
    }
}

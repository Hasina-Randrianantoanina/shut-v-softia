using TestConnexion.Utils;

namespace TestConnexion.Log
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
        /// -1 (?) : asking if there are available datas<br/>
        /// -2 (?) : reading the version<br/>
        /// -3 (?) : reading the analogic config<br/>
        /// -4 (?) : reading the analogic real time datas<br/>
        /// -5 (?) : downloading files by FTP<br/>
        /// -6 (?) : downloading file by TCP (creating the file)<br/>
        /// -7 (?) : downloading file by TCP (exchanges with server)<br/>
        /// -8 (?) : downloading file by TCP (writing into the file)<br/>
        /// -9 (?) : reading the mesures (getting the source file)<br/>
        /// -10 (?) : reading the mesures (copying the source file)<br/>
        /// -11 (?) : reading the mesures (reading the binary file)<br/>
        /// -12 (?) : writing the mesures (creating the file)<br/>
        /// -13 (?) : writing the mesures (writing into the file)<br/>
        /// -20 (?) : interaction with database (retrieving station)<br/>
        /// -21 (?) : interaction with database (updating station)<br/>
        /// -22 (?) : interaction with database (updating voies internes)<br/>
        /// -23 (?) : interaction with database (retrieving active defaut voie interne)<br/>
        /// -24 (?) : interaction with database (inserting all defauts)<br/>
        /// </summary>
        public static void LogException(string logFilePath, Exception exception, string operation, int errorCode, string? prefix = null) 
        {
            try
            {
                using (StreamWriter streamWriter = File.AppendText(logFilePath))
                {
                    string stopTxt = errorCode >= -8 ? "Arrêt appel" : "Arrêt lecture/écriture";
                    if (errorCode <= -20) { stopTxt = "Arrêt gestion defauts"; }
                    string errorTxt = errorCode <= 0 ? "" : $" (erreur {errorCode})";
                    string txt = $"{prefix} {stopTxt} sur exception levée lors de l'opération {operation} : {exception.GetType().Name}{errorTxt}";
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

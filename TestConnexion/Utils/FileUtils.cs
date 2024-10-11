using TestConnexion.Log;

namespace TestConnexion.Utils
{
    public static class FileUtils
    {
        public static void TryDeleteCreateFile(string fileToCreateFullName)
        {
            try
            {
                // If the file already exists, we kill it
                if (File.Exists(fileToCreateFullName)) { File.Delete(fileToCreateFullName); }

                // Creating the file and closing the stream to avoid multi access
                File.Create(fileToCreateFullName).Close();
            }
            catch
            {
                throw;
            }
        }

        public static string[] GetSourceFilesFullNames(string sourceDirectory, string initiales, bool printConsole)
        {
            try
            {
                if (!Directory.Exists(sourceDirectory))
                {
                    Console.WriteLine($"Directory {sourceDirectory} not found");
                    return null;
                }
                if (printConsole) { Console.WriteLine($"Directory {sourceDirectory} exists"); }
                if (initiales.Equals("XY")) { initiales = "CS"; } // Specific case : XY is the M580 D16 into station CS
                string[] sourcesFilesFullNames = Directory.GetFiles(sourceDirectory, $"*{initiales}*"); // Files that contains initiales

                if (sourcesFilesFullNames.Length == 0)
                {
                    Console.WriteLine($"No files found in {sourceDirectory}");
                    return null;
                }

                if (printConsole)
                {
                    foreach (string file in sourcesFilesFullNames)
                    {
                        Console.WriteLine($"SourceFile {file} found");
                    }
                }
                return sourcesFilesFullNames;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public static void TryCopyFile(string fileToCopyFullName, string fileToCreateFullName, bool printConsole)
        {
            try
            {
                File.Copy(fileToCopyFullName, fileToCreateFullName, true);
                if (printConsole) { Console.WriteLine($"\n------ Copy successfull : {fileToCopyFullName} => {fileToCreateFullName}"); }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n--- Error when copying the file {fileToCopyFullName}");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        public static async Task<bool> TryWriteAsync(StreamWriter writer, string text)
        {
            try
            {
                await writer.WriteAsync(text);
                await writer.FlushAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n--- Error when writing \"{text}\"");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

    }
}

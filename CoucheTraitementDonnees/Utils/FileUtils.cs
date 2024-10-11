namespace CoucheTraitementDonnees.Utils
{
    public static class FileUtils
    {

        public static string[] GetSourceFilesFullNames(string initiales)
        {
            string sourceDirectory = $"../CoucheConnexionReseau/Connexion/Downloads/{initiales}/"; // Dev
            try
            {
                if (Directory.Exists(sourceDirectory))
                {
                    Console.WriteLine($"Directory {sourceDirectory} exists");
                    string[] sourcesFilesFullNames = Directory.GetFiles(sourceDirectory, $"*{initiales}*"); // Files that contains initiales

                    if (sourcesFilesFullNames.Length == 0)
                    {
                        Console.WriteLine("No files found");
                        return null;
                    }

                    foreach (string file in sourcesFilesFullNames)
                    {
                        Console.WriteLine($"SourceFile {file} found");
                    }
                    return sourcesFilesFullNames;
                }
                else
                {
                    Console.WriteLine("");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n--- Error while looking for file of the directory {sourceDirectory}");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return null;
            }
        }

        /// <returns>True if the file has been successfully copied</returns>
        public static bool TryCopyFile(string fileToCopyFullName, string fileToCreateFullName)
        {
            try
            {
                File.Copy(fileToCopyFullName, fileToCreateFullName, true);
                Console.WriteLine($"\n------ Copy successfull : {fileToCopyFullName} => {fileToCreateFullName}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n--- Error when copying the file {fileToCopyFullName}");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="fileToCreateFullName"></param>
        /// <returns>True if the file has been successfully deleted and created</returns>
        public static bool TryDeleteCreateFile(string fileToCreateFullName)
        {
            try
            {
                // If the file already exists, we kill it
                if (File.Exists(fileToCreateFullName)) File.Delete(fileToCreateFullName);

                // Creating the file and closing the stream to avoid multi access
                File.Create(fileToCreateFullName).Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n--- Error when deleting and creating the file {fileToCreateFullName}");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return false;
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

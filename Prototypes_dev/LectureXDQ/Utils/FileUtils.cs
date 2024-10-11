namespace LectureXDQ.Utils
{
    public static class FileUtils
    {
        public static string[]? GetSourceFilesFullNames(string sourceDirectory, string initiales, bool printConsole) 
        {
            try
            {
                // Directory
                if (!Directory.Exists(sourceDirectory))
                {
                    Console.WriteLine($"Directory {sourceDirectory} not found");
                    return null;
                }
                if (printConsole) { Console.WriteLine($"Directory {sourceDirectory} exists"); }

                // Files
                string[] sourcesFilesFullNames = Directory.GetFiles(sourceDirectory, "20??????????.xdq");

                if (sourcesFilesFullNames.Length == 0)
                {
                    Console.WriteLine($"No files found in {sourceDirectory}");
                    return null;
                }

                Array.Sort(sourcesFilesFullNames); // Usefull ?

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
                PrintUtils.PrintConsoleException(ex, $"Error when looking for source file in {sourceDirectory}");
                throw; // To break main
            }

        }

    }
}

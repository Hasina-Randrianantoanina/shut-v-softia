namespace IsodaqOperations.Utils
{
    public static class FileUtils
    {
        public static string[]? GetSourceFilesFullNames(string sourceDirectory, string initiales, bool printConsole)
        {
            string stationDirectory = Path.Join(sourceDirectory, initiales, initiales);
            try
            {
                // Directory
                if (!Directory.Exists(stationDirectory))
                {
                    Console.WriteLine($"Directory {stationDirectory} not found");
                    return null;
                }
                if (printConsole) { Console.WriteLine($"Directory {stationDirectory} exists"); }

                // Files
                string[] sourcesFilesFullNames = Directory.GetFiles(stationDirectory, "latest.xdq");

                if (sourcesFilesFullNames.Length == 0)
                {
                    Console.WriteLine($"No files found in {stationDirectory}");
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
                PrintUtils.PrintConsoleException(ex, $"Error when looking for source file in {stationDirectory}");
                throw; // To break main
            }

        }

    }
}

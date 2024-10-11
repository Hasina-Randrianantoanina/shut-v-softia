namespace CoucheLectureMan.Utils
{
    public static class FileUtils
    {
        public static string[] GetSourceFilesFullNames(string initiales, DateTime callDate)
        {
            string sourceDirectory = @"C:/ProdShut/DonnéesShut/CrAppels/AsNet/" + initiales;

            try
            {
                if (Directory.Exists(sourceDirectory))
                {
                    Console.WriteLine($"Directory {sourceDirectory} exists");
                    string callDateFormatted = callDate.ToString("ddMMyyyyHHmm"); // Seconds does not matter
                    string pattern = $"{initiales}*{callDateFormatted}*.man*";
                    string[] sourcesFilesFullNames = Directory.GetFiles(sourceDirectory, pattern); // Files that contains initiales

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

        public static List<string> GetStationsAutomatePremium()
        {
            return new List<string> { "EN", "RF", "RC", "RH", "RR" };
        }

        public static List<string> GetStationsAutomateM580D15()
        {
            return new List<string> { "BA", "BBp", "BD", "CH", "GA", "JE", "LR", "OUp", "QM", "VIp", "153", "156", "172", "173", "175", "176", "177", "178", "179", "184", "191" };
        }

        public static List<string> GetStationsAutomateM580D16()
        {
            return new List<string> { "CV", "HE", "XY" };
        }

        public static string[] ReadAllText(string manFileFullName)
        {
            try
            {
                string[] lines = File.ReadAllLines(manFileFullName);
                Console.WriteLine($"\n--- File {manFileFullName} fully read");
                return lines;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n--- Error when reading the file {manFileFullName}");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return null;
            }
        }
    }
}

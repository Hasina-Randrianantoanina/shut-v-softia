using System.Text.RegularExpressions;

namespace AutomateOperations.Utils
{
    public static class FileUtils
    {
        public static (bool statusDCFile, Erreur? erreur) TryDeleteCreateFile(string fileToCreateFullName)
        {
            try
            {
                // If the file already exists, we kill it
                if (File.Exists(fileToCreateFullName)) { File.Delete(fileToCreateFullName); }

                // Creating the file and closing the stream to avoid multi access
                File.Create(fileToCreateFullName).Close();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ErrorUtils.HandleWritingError(500, ex));
            }
        }

        public static (string[]? names, Erreur? erreur) GetSourceFilesShortNames(string sourceDirectory, string initiales)
        {
            // Check if a folder exists
            if (!Directory.Exists(sourceDirectory)) { return (null, ErrorUtils.HandleReadingError(600)); }

            if (initiales.Equals("XY")) { initiales = "CS"; }  // Specific case : XY is the M580 D16 into station CS
            initiales = initiales.Replace("_test", "");

            try
            {
                string[] sourcesFilesFullNames = Directory.GetFiles(sourceDirectory, $"*{initiales}*"); // Files that contains initiales

                if (sourcesFilesFullNames.Length == 0) { return (null, ErrorUtils.HandleReadingError(601)); }

                // Filter
                List<string> list = new List<string>();
                Regex m580Pattern = new Regex(@"^[0-9]{6}$"); // XXddMMyy => XX initiales, dd day,  MM month, yy year
                char separator = sourcesFilesFullNames[0].Contains(@"\"[0]) ? @"\"[0] : '/';
                foreach (string fileFullName in sourcesFilesFullNames)
                {
                    string fileName = fileFullName.Split(separator).Last();
                    if (fileName.Equals($"Result_{initiales}.bin") || ( fileName.StartsWith(initiales) && m580Pattern.IsMatch(fileName.Substring(initiales.Length)) ))
                    {
                        list.Add(fileName);
                    }
                }

                if (list.Count == 0) { return (null, ErrorUtils.HandleReadingError(601)); }
                string[] sourcesFilesNames = list.ToArray();

                // Sorting by date
                if (sourcesFilesNames.Length > 1) 
                {
                    Array.Sort(sourcesFilesNames, (f1, f2) =>
                    {
                        DateTime d1 = DateTime.Parse(@$"{f1.Substring(2, 2)}/{f1.Substring(4, 2)}/{f1.Substring(6, 2)}");
                        DateTime d2 = DateTime.Parse(@$"{f2.Substring(2, 2)}/{f2.Substring(4, 2)}/{f2.Substring(6, 2)}");
                        return d1.CompareTo(d2);
                    });
                }

                return (sourcesFilesNames, null);
            }
            catch (Exception ex)
            {
                return (null, ErrorUtils.HandleReadingError(602, ex));
            }

        }

        public static (bool statusCopy, Erreur? erreur) TryCopyFile(string fileToCopyFullName, string fileToCreateFullName)
        {
            try
            {
                File.Copy(fileToCopyFullName, fileToCreateFullName, true);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ErrorUtils.HandleReadingError(603, ex));
            }
        }

    }
}

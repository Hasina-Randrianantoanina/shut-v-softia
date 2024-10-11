namespace Connexion.Utils
{
    public static class FileUtils
    {
        /// <returns>True if the file has been successfully deleted and created</returns>
        public static bool TryDeleteCreateFile(string fileToCreateFullName)
        {
            try
            {
                // If the file already exists, we kill it
                if (File.Exists(fileToCreateFullName)) { File.Delete(fileToCreateFullName); }

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

    }
}

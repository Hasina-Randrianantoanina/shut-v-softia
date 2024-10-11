using Connexion.AutomateProtocol;

namespace Connexion.Utils
{
    public static class BinaryFileUtils
    {
        /// <returns>True if the data has been successfully written</returns>
        public static bool TryWriteDatasInBinaryFile(string binaryFileToWriteFullName, AutomateCommand automateCommand, int nbMotsToWrite, bool printConsole)
        {
            try
            {
                // Opening the streams
                using FileStream fileStream = File.Open(binaryFileToWriteFullName, FileMode.Append, FileAccess.Write);
                using BinaryWriter binaryStream = new BinaryWriter(fileStream);

                binaryStream.Write(automateCommand.Response, 9, nbMotsToWrite*2); // 1 mot = 2 bytes

                if (printConsole) { Console.WriteLine($"--- Writing of {nbMotsToWrite} datas ({nbMotsToWrite*2} bytes) in {binaryFileToWriteFullName} is successfull"); }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n--- Error when writing the binary file {binaryFileToWriteFullName}");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return false;
            }

        }
    }
}

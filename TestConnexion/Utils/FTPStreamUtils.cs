using FluentFTP;
using TestConnexion.Log;

namespace TestConnexion.Utils
{
    public static class FTPStreamUtils
    {
        public static async Task<bool> GetFileByFTP(string host, string remoteDataDirectory, string localDownloadDirectory, List<string> namesOfFilesToDownload, FileLogger fileLogger, bool printConsole)
        {
            try
            {
                string loginFTP = "datastorage";
                string passwordFTP = "datadownload";

                if (string.IsNullOrEmpty(loginFTP) || string.IsNullOrEmpty(passwordFTP)) 
                {
                    Console.WriteLine($"\n--- Login and password have to be set");
                    return false; 
                }

                // Create an FTP client and specify the host, username and password
                await using AsyncFtpClient client = new AsyncFtpClient(host, loginFTP, passwordFTP);

                Console.WriteLine($"\n--- Trying to connect to FTP Server {host}");

                // Connect to the server and automatically detect working FTP settings
                if (await client.AutoConnect() == null)
                {
                    Console.WriteLine($"\n--- FTP Connexion failed");
                    fileLogger.Log($"Téléchargements annulés : échec de la connexion FTP");
                    return false;
                } 
                else if (printConsole) { Console.WriteLine($"\n--- FTP Connexion open"); }

                // Check if a folder exists
                if (await client.DirectoryExists(remoteDataDirectory))
                {
                    if (printConsole) { Console.WriteLine($"\n{remoteDataDirectory} <= directory exists"); }
                }
                else
                {
                    Console.WriteLine($"\n---- FTP Connexion closed : {remoteDataDirectory} <= directory does not exist ----");
                    fileLogger.Log($"Téléchargements annulés : dossier distant n'existe pas");
                    return false;
                }

                if (printConsole) { Console.WriteLine("\n--- Looking for files to download"); }

                int numberOfSuccessfullDownload = 0;

                foreach (string name in namesOfFilesToDownload)
                {
                    string fileToDownloadFullName = remoteDataDirectory + name;

                    // Check if a file exists
                    if (!(await client.FileExists(fileToDownloadFullName)))
                    {
                        if (printConsole) { Console.WriteLine($"\n{fileToDownloadFullName} <= file does not exist"); }
                        fileLogger.Log($"Téléchargement de {name} annulé : fichier distant n'existe pas");
                        continue;
                    }
                    if (printConsole) { Console.WriteLine($"\n{fileToDownloadFullName} <= file exists"); }

                    // If the file already exist locally, we compare the last modified time
                    if (File.Exists(localDownloadDirectory + name))
                    {
                        if (printConsole) { Console.WriteLine("There is a file with the same name locally"); }

                        DateTime remoteFileLastModif = await client.GetModifiedTime(fileToDownloadFullName);
                        DateTime localFileLastModif = File.GetLastWriteTime(localDownloadDirectory + name);
                        if (!name.Substring(0, 2).Equals("XY"))
                        {
                            localFileLastModif = localFileLastModif.ToUniversalTime();
                            if (printConsole) { Console.WriteLine("Modifying local file to localtime"); }
                        } // XY is the only M580 in LocalTime

                        if (printConsole) { Console.WriteLine($"remoteFileLastModif(UTC) = {remoteFileLastModif}"); }
                        if (printConsole) { Console.WriteLine($"localFileLastModif(UTC) = {localFileLastModif}"); }

                        if (remoteFileLastModif > localFileLastModif)
                        {
                            if (printConsole) { Console.WriteLine($"The remote file is the most recent... New download for {name}"); }
                        }
                        else
                        {
                            if (printConsole) { Console.WriteLine($"The local file is the most recent... Download skipped {name}"); }
                            fileLogger.Log($"Téléchargement de {name} ignoré : fichier local est plus récent");
                            numberOfSuccessfullDownload++;
                            continue;
                        }
                    }

                    // Download the file
                    FtpStatus status = await client.DownloadFile(localDownloadDirectory + name, fileToDownloadFullName, FtpLocalExists.Overwrite);
                    if (status == FtpStatus.Success)
                    {
                        numberOfSuccessfullDownload++;
                        if (printConsole) { Console.WriteLine($"--- Download successfull for {name}"); }
                        fileLogger.Log($"Téléchargement de {name} réussi");
                    }
                    else if (status == FtpStatus.Failed)
                    {
                        if (printConsole) { Console.WriteLine($"--- Download failed for {name}"); }
                        fileLogger.Log($"Téléchargement de {name} échoué");
                    }
                    else if (printConsole)
                    {
                        Console.WriteLine(status);
                    }
                } // End foreach

                if (printConsole) { Console.WriteLine($"\n--- Successfully download {numberOfSuccessfullDownload} files"); }
                fileLogger.Log($"{numberOfSuccessfullDownload}/{namesOfFilesToDownload.Count} téléchargements réussis/ignorés");
                Console.WriteLine("\n---- FTP Connexion closed : End of FTP transfert without error ----");
                return true;
            }
            catch (Exception ex)
            {
                fileLogger.LogException(ex, "Lecture données par FTP", -5);
                PrintUtils.PrintConsoleException(ex, "Error when trying to download files by FTP");
                return false;
            }
        }

    }
}

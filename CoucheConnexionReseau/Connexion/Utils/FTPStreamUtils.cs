using FluentFTP;

namespace Connexion.Utils
{
    public static class FTPStreamUtils
    {
        public static async Task GetFileByFTP(string host, int portFTP, string remoteDataDirectory, string localDownloadDirectory, List<string> namesOfFilesToDownload, bool printConsole)
        {
            try
            {
                // Create an FTP client and specify the host, username and password
                await using AsyncFtpClient client = new AsyncFtpClient(host,
                    Environment.GetEnvironmentVariable("loginFTP"), Environment.GetEnvironmentVariable("passwordFTP"), portFTP);

                Console.WriteLine($"\n--- Trying to connect to FTP Server {host}:{portFTP}");

                // Connect to the server and automatically detect working FTP settings
                await client.AutoConnect();

                if (!client.IsConnected)
                {
                    Console.WriteLine($"\n--- FTP Connexion failed");
                    return;
                }

                Console.WriteLine($"\n--- FTP Connexion open");

                // Check if a folder exists
                if (await client.DirectoryExists(remoteDataDirectory))
                {
                    if (printConsole) { Console.WriteLine($"\n{remoteDataDirectory} <= directory exists"); }
                }
                else
                {
                    Console.WriteLine($"\n---- FTP Connexion closed : {remoteDataDirectory} <= directory does not exist ----");
                    return;
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
                        continue;
                    }
                    if (printConsole) { Console.WriteLine($"\n{fileToDownloadFullName} <= file exists"); }

                    // If the file already exist locally, we compare the last modified time
                    if (File.Exists(localDownloadDirectory + name))
                    {
                        if (printConsole) { Console.WriteLine("There is a file with the same name locally"); }

                        DateTime remoteFileLastModif = await client.GetModifiedTime(fileToDownloadFullName);
                        DateTime localFileLastModif = File.GetLastWriteTime(localDownloadDirectory + name);
                        if (!name.Substring(0, 2).Equals("XY")) {
                            localFileLastModif = localFileLastModif.ToUniversalTime();
                            Console.WriteLine("modifying localtime");
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
                            continue;
                        }
                    }

                    // Download the file
                    FtpStatus status = await client.DownloadFile(localDownloadDirectory + name, fileToDownloadFullName, FtpLocalExists.Overwrite);
                    if (status == FtpStatus.Success)
                    {
                        numberOfSuccessfullDownload++;
                        if (printConsole) { Console.WriteLine($"--- Download successfull for {name}"); }
                    }
                    else if (status == FtpStatus.Failed)
                    {
                        if (printConsole) { Console.WriteLine($"--- Download failed for {name}"); }
                    }
                    else if (printConsole)
                    {
                        Console.WriteLine(status);
                    }
                } // End foreach

                if (printConsole) { Console.WriteLine($"\n--- Successfully download {numberOfSuccessfullDownload} files"); }
                Console.WriteLine("\n---- FTP Connexion closed : End of FTP transfert without error ----");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n--- Error when trying to download files by FTP");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

    }
}

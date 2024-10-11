using FluentFTP;
using AutomateOperations.Logger;

namespace AutomateOperations.Utils
{
    public static class FTPStreamUtils
    {
        public static async Task<(bool statusDownload, Erreur? erreur)> GetFileByFTP(string host, string remoteDataDirectory, string localDownloadDirectory, List<string> namesOfFilesToDownload, FileLogger fileLogger)
        {
            string loginFTP = "datastorage";
            string passwordFTP = "datadownload";

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(loginFTP) || string.IsNullOrEmpty(passwordFTP)) 
            { 
                return (false, ErrorUtils.HandleFTPError(300)); 
            }

            try
            {
                // Create an FTP client and specify the host, username and password
                await using AsyncFtpClient client = new AsyncFtpClient(host, loginFTP, passwordFTP);

                // Connect to the server and automatically detect working FTP settings
                if (await client.AutoConnect() == null)
                {
                    return (false, ErrorUtils.HandleFTPError(301));
                } 

                // Check if a folder exists
                if (!await client.DirectoryExists(remoteDataDirectory))
                {
                    return (false, ErrorUtils.HandleFTPError(302));
                }

                int numberOfSuccessfullDownload = 0;

                foreach (string name in namesOfFilesToDownload)
                {
                    string fileToDownloadFullName = remoteDataDirectory + name;
                    string fileLocalFullName = localDownloadDirectory + name;

                    // Check if a file exists
                    if (!(await client.FileExists(fileToDownloadFullName)))
                    {
                        fileLogger.Log($"Téléchargement de {name} annulé : fichier distant n'existe pas");
                        continue;
                    }

                    // Download the file
                    FtpStatus status = await client.DownloadFile(fileLocalFullName, fileToDownloadFullName, FtpLocalExists.Overwrite);
                    if (status == FtpStatus.Success)
                    {
                        numberOfSuccessfullDownload++;
                        fileLogger.Log($"Téléchargement de {name} réussi");
                    }
                    else if (status == FtpStatus.Failed)
                    {
                        fileLogger.Log($"Téléchargement de {name} échoué");
                    }
                } // End foreach

                fileLogger.Log($"{numberOfSuccessfullDownload}/{namesOfFilesToDownload.Count} téléchargements réussis");
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ErrorUtils.HandleFTPError(303, ex));
            }
        }

    }
}

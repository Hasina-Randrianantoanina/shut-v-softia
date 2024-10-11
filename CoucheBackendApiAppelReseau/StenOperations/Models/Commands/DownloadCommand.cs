
using FluentFTP;
using FluentFTP.Exceptions;
using StenOperations.Logger;
using StenOperations.Models.Entities;
using StenOperations.Utils;
using System.Net;

namespace StenOperations.Models.Commands
{
    public class DownloadCommand: DefaultCommand
    {
        public string Filename { get; set; }
        public string DownloadUrl { get; set; }
        //public string FtpAddress { get; set; }
        public string? FtpUsername { get; set; }
        public string? FtpPassword { get; set; }

        public DownloadCommand(ILogger logger, string filename, string downloadUrl, string? ftpUsername, string? ftpPassword)
            :base(logger, new Station("")) 
        {
            Filename = filename;
            DownloadUrl = downloadUrl;
            FtpUsername = ftpUsername;
            FtpPassword = ftpPassword;
        }

        public override void Execute()
        {
            SyncDownload();
        }
        
        private void SyncDownload()
        {
            var url = new Uri(DownloadUrl);
            //Logger.Log("AbsolutePath: {0}", url.AbsolutePath);
            //Logger.Log("AbsoluteUri: {0}", url.AbsoluteUri);
            //Logger.Log("DnsSafeHost: {0}", url.DnsSafeHost);
            //Logger.Log("Fragment: {0}", url.Fragment);
            //Logger.Log("Host: {0}", url.Host);
            //Logger.Log("IdnHost: {0}", url.IdnHost);
            //Logger.Log("LocalPath: {0}", url.LocalPath);
            //Logger.Log("PathAndQuery: {0}", url.PathAndQuery);
            //Logger.Log("Query: {0}", url.Query);
            //Logger.Log("Scheme: {0}", url.Scheme);
            //Logger.Log("Segments: [{0}]", string.Join(',', url.Segments));
            //Logger.Log("UserInfo: {0}", url.UserInfo);
            try
            {
                Logger.Log("D-Download: {0}", Filename);
                var uri = string.Format("{0}://{1}", url.Scheme, url.Host);
                Logger.Log("DOWNLOAD {0}", DownloadUrl);
                using (var ftpClient = new FtpClient(uri, FtpUsername, FtpPassword))
                {
                    // Do not forget ReadTimeout
                    //ftpClient.Config.ConnectTimeout = 15000;
                    //ftpClient.Config.ReadTimeout = 25000;
                    //ftpClient.Config.DataConnectionReadTimeout = 25000;
                    ftpClient.Config.DataConnectionType = FtpDataConnectionType.PORT;
                    //ftpClient.Config.LogToConsole = true;

                    // Connect to the ftp server
                    ftpClient.Connect();

                    if (!ftpClient.IsConnected)
                    {
                        ErrorCode = 2003;
                        ErrorText = $"Impossible de se connecter au ftp";
                        Logger.Log("Erreur {0} : {1}", ErrorCode, ErrorText);
                        return;
                    }

                    // Check file
                    //if (!ftpClient.FileExists(url.AbsolutePath))
                    //{
                    //    ErrorCode = 2003;
                    //    ErrorText = $"Fichier {url.AbsolutePath} introuvable sur {url.Host}.";
                    //    Logger.Log("Erreur {0} : {1}", ErrorCode, ErrorText);
                    //    return;                
                    //}

                    var isDownloaded = false;
                    // Donwload stream
                    using (var fs = new FileStream(Filename, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        isDownloaded = ftpClient.DownloadStream(
                            fs,
                            url.AbsolutePath.Substring(1), // Url without root path
                            0,
                            (p) =>
                            {
                            });
                    }

                    // OpenRead stream
                    /*using (var netStream = ftpClient.OpenRead(url.AbsolutePath))
                    {
                        using (var fs = new FileStream(Filename, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            netStream.CopyTo(fs);
                            isDownloaded = true;
                        }
                    }*/

                    // Download the file
                    Logger.Log("D-Download: lecture flux ");
                    //var status = ftpClient.DownloadFile(
                    //    Filename,
                    //    url.AbsolutePath.Substring(1), // Ne pas utiliser l'absolute path sinon ça bugue
                    //    FtpLocalExists.Overwrite,
                    //    FtpVerify.None,
                    //    (progress) =>
                    //    {
                    //    });
                    //if (status == FtpStatus.Success)
                    if (isDownloaded)
                    {
                        if (File.Exists(Filename))
                        {
                            var fi = new FileInfo(Filename);
                            if (fi.Length == 0)
                            {
                                ErrorCode = 2003;
                                ErrorText = $"Fichier {Filename} vide.";
                                Logger.Log("Erreur {0} : {1}", ErrorCode, ErrorText);
                            }
                            else
                            {
                                Logger.Log("D-Download: nb octets lus = {0}", fi.Length);
                            }
                        }
                    }
                    //else if (status == FtpStatus.Skipped)
                    //{
                    //    ErrorCode = 2003;
                    //    ErrorText = $"D-Download: fichier {Filename} existant.";
                    //}
                    else //if (status == FtpStatus.Failed) 
                    {
                        ErrorCode = 2003;
                        ErrorText = $"D-Download: Echec de téléchargement du fichier {url.AbsolutePath}.";
                        Logger.Log("Erreur {0}  : {1}", ErrorCode, ErrorText);
                    }
                }
            }
            catch (FtpMissingObjectException ex)
            {
                ErrorCode = 2003;
                ErrorText = ex.ExceptionMessages();
                Logger.Log("Erreur {0} : {1}", ErrorCode, ErrorText);
            }
            catch (Exception ex) 
            {
                ErrorCode = 2003;
                ErrorText = ex.ExceptionMessages();
                Logger.Log("Erreur {0} : {1}", ErrorCode, ErrorText);
            }
        }

        public override async Task ExecuteAsync()
        {
            var url = new Uri(DownloadUrl);
            //Logger.Log("AbsolutePath: {0}", url.AbsolutePath);
            //Logger.Log("AbsoluteUri: {0}", url.AbsoluteUri);
            //Logger.Log("DnsSafeHost: {0}", url.DnsSafeHost);
            //Logger.Log("Fragment: {0}", url.Fragment);
            //Logger.Log("Host: {0}", url.Host);
            //Logger.Log("IdnHost: {0}", url.IdnHost);
            //Logger.Log("LocalPath: {0}", url.LocalPath);
            //Logger.Log("PathAndQuery: {0}", url.PathAndQuery);
            //Logger.Log("Query: {0}", url.Query);
            //Logger.Log("Scheme: {0}", url.Scheme);
            //Logger.Log("Segments: [{0}]", string.Join(',', url.Segments));
            //Logger.Log("UserInfo: {0}", url.UserInfo);
            try
            {
                Logger.Log("D-Download: {0}", Filename);
                var uri = string.Format("{0}://{1}", url.Scheme, url.Host);
                Logger.Log("DOWNLOAD {0}", DownloadUrl);

                var token = new CancellationToken();
                using (var ftpClient = new AsyncFtpClient(uri, FtpUsername, FtpPassword))
                {
                    // Do not forget ReadTimeout
                    //ftpClient.Config.ConnectTimeout = 15000;
                    //ftpClient.Config.ReadTimeout = 25000;
                    //ftpClient.Config.DataConnectionReadTimeout = 25000;
                    ftpClient.Config.DataConnectionType = FtpDataConnectionType.PORT;

                    // Connect to the ftp server
                    await ftpClient.Connect(token);

                    //// List files under root path

                    // Download the file
                    var progress = new Progress<FtpProgress>(p =>
                    {
                        if (p.Progress == 1)
                        {
                            // Done
                        }
                        else
                        {
                            // percent done = (p.Progress * 100)
                        }
                    });

                    Logger.Log("D-Download: lecture flux ");
                    var isDownloaded = false;
                    using (var fs = new FileStream(Filename, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        isDownloaded = await ftpClient.DownloadStream(
                            fs,
                            url.AbsolutePath.Substring(1), // Url without root path
                            0,
                            progress);
                    }

                    if (isDownloaded)
                    {
                        if (File.Exists(Filename))
                        {
                            var fi = new FileInfo(Filename);
                            if (fi.Length == 0)
                            {
                                ErrorCode = 2003;
                                ErrorText = $"Fichier {Filename} vide.";
                                Logger.Log("Erreur {0} : {1}", ErrorCode, ErrorText);
                            }
                            else
                            {
                                Logger.Log("D-Download: nb octets lus = {0}", fi.Length);
                            }
                        }
                    }
                    else
                    {
                        ErrorCode = 2003;
                        ErrorText = $"D-Download: Echec de téléchargement du fichier {url.AbsolutePath.Substring(1)}";
                        Logger.Log("Erreur {0}  : {1}", ErrorCode, ErrorText);
                    }
                }
            }
            catch (FtpMissingObjectException ex)
            {
                ErrorCode = 2003;
                ErrorText = ex.ExceptionMessages();
                Logger.Log("Erreur {0} : {1}", ErrorCode, ErrorText);
            }
            catch (Exception ex)
            {
                ErrorCode = 2003;
                ErrorText = ex.ExceptionMessages();
                Logger.Log("Erreur {0} : {1}", ErrorCode, ErrorText);
            }
        }

        private void OriginalDown()
        {
            var nbol = 0;
            try
            {
                FtpWebRequest downloadRequest = (FtpWebRequest)WebRequest.Create(DownloadUrl);
                downloadRequest.Credentials = new NetworkCredential(FtpUsername, FtpPassword);
                downloadRequest.UsePassive = false;
                downloadRequest.KeepAlive = false;
                downloadRequest.Proxy = null;

                var getResponseOk = false;
                var cb = new AsyncCallback((result) => { });
                var returnValue = downloadRequest.BeginGetResponse(cb, downloadRequest);
                returnValue.AsyncWaitHandle.WaitOne(5000, false);

                if (returnValue.IsCompleted)
                    getResponseOk = true;

                if (!getResponseOk)
                {
                    ErrorCode = 2003;
                    ErrorText = "pb download file";
                    Logger.Log($"{ErrorCode}: {ErrorText}");
                }

                FtpWebResponse downloadResponse = (FtpWebResponse)downloadRequest.EndGetResponse(returnValue);
                using (var responseStream = downloadResponse.GetResponseStream())
                {
                    var remoteFileName = Path.GetFileName(downloadRequest.RequestUri.AbsolutePath);
                    if (string.IsNullOrEmpty(remoteFileName))
                    {

                    }
                    else
                    {
                        using (var fileStream = File.Create(Filename))
                        {
                            var buffer = new byte[2048];
                            var bytesRead = 0;

                            while (true)
                            {
                                bytesRead = 0;
                                getResponseOk = false;
                                returnValue = responseStream.BeginRead(buffer, 0, 2048, cb, responseStream);
                                // Attente
                                var t1 = DateTime.Now.AddSeconds(5);
                                do
                                {
                                    if (returnValue.IsCompleted) { getResponseOk = true; }
                                }
                                while (t1 > DateTime.Now && !getResponseOk);

                                if (!getResponseOk)
                                {
                                    ErrorCode = 2003;
                                    ErrorText = "pb lecture async flux";
                                }
                                bytesRead = responseStream.EndRead(returnValue);

                                if (bytesRead == 0)
                                {
                                    Logger.Log($"D-Download: nb octets lus = {nbol}");
                                    break;
                                }
                                else
                                {
                                    nbol += bytesRead;
                                }
                                fileStream.Write(buffer, 0, bytesRead);
                            } // using (var fileStream = File.Create(Filename))
                            if (nbol == 0)
                            {
                                ErrorCode = 2003;
                                ErrorText = $"Fichier {Filename} vide.";
                                Logger.Log($"Erreur {ErrorCode} : {ErrorText}");
                            }
                        } // using (var fileStream ...
                    }
                } // using (var responseStream = downloadResponse.GetResponseStream())
            }
            catch (UriFormatException ex)
            {
                ErrorCode = 2003;
                ErrorText = ex.ExceptionMessages();
                Logger.Log($"Erreur {ErrorCode} : {ErrorText}");
            }
            catch (WebException ex)
            {
                ErrorCode = 2003;
                ErrorText = ex.ExceptionMessages();
                Logger.Log($"Erreur {ErrorCode} : {ErrorText}");
            }
            catch (IOException ex) 
            {
                ErrorCode = 2012;
                ErrorText = ex.ExceptionMessages() + $" (nombre d'octets téléchargés = {nbol})";
                Logger.Log($"Erreur {ErrorCode} : {ErrorText}");
            }
            catch (Exception ex)
            {
                ErrorCode = 2003;
                ErrorText = ex.ExceptionMessages();
                Logger.Log($"Erreur {ErrorCode} : {ErrorText}");
            }
        }

    }
}

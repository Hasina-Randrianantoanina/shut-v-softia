using FluentFTP;
using FluentFTP.Exceptions;
using StenOperations.Logger;
using StenOperations.Models.Entities;
using StenOperations.Utils;
using System.Net;

namespace StenOperations.Models.Commands
{
    public class UploadCommand: DefaultCommand
    {
        public string Filename { get; set; }
        public string UploadUrl { get; set; }
        //public string FtpAddress { get; set; }
        public string? FtpUsername { get; set; }
        public string? FtpPassword { get; set; }

        public UploadCommand(ILogger logger, string filename, string uploadUrl, string? ftpUsername, string? ftpPassword)
            :base(logger, new Station("")) 
        {
            Filename = filename;
            UploadUrl = uploadUrl;
            FtpUsername = ftpUsername;
            FtpPassword = ftpPassword;
        }

        public override void Execute()
        {
            SyncUpload();
        }

        private void SyncUpload()
        {
            var url = new Uri(UploadUrl);
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
                Logger.Log("D-Upload : {0}", Filename);
                var uri = string.Format("{0}://{1}", url.Scheme, url.Host);
                Logger.Log($"UPLOAD {UploadUrl}");
                using (var ftpClient = new FtpClient(uri, FtpUsername, FtpPassword))
                {
                    // Do not forget ReadTimeout
                    //ftpClient.Config.ConnectTimeout = 15000;
                    //ftpClient.Config.ReadTimeout = 25000;
                    //ftpClient.Config.DataConnectionReadTimeout = 25000;
                    //ftpClient.Config.UploadDataType = FtpDataType.Binary;
                    ftpClient.Config.DataConnectionType = FtpDataConnectionType.PORT;
                    ftpClient.Config.LogToConsole = true;

                    // Connect to ftp server
                    ftpClient.Connect();
                    if (!ftpClient.IsConnected)
                    {
                        ErrorCode = 2003;
                        ErrorText = $"Impossible de se connecter au ftp";
                        Logger.Log("Erreur {0} : {1}", ErrorCode, ErrorText);
                        return;
                    }

                    FtpStatus status = FtpStatus.Skipped;
                    // Upload stream
                    using (var fs = new FileStream(Filename, FileMode.Open, FileAccess.Read))
                    {
                        status = ftpClient.UploadStream(
                            fs,
                            url.AbsolutePath.Substring(1), // Url without root path
                            FtpRemoteExists.OverwriteInPlace,
                            createRemoteDir: false,
                            (progress) => { });
                    }


                    if (status == FtpStatus.Success) 
                    {
                        ErrorCode = 0;
                        ErrorText = "";
                    }
                    else if (status == FtpStatus.Failed)
                    {
                        ErrorCode = 2003;
                        ErrorText = "pb uploadfile";
                    }
                    else if (status == FtpStatus.Skipped)
                    {
                        ErrorCode = 2003;
                        ErrorText = $"Téléversement annulé car le fichier existe déjà {url.AbsolutePath}";
                    }
                }
            }
            catch (FtpException ex)
            {
                ErrorCode = 2003;
                ErrorText = ex.Message;
            }
            catch (Exception ex) 
            {
                ErrorCode = 2003;
                ErrorText = ex.Message;
            }
        }

        private void OriginalUpload()
        {
            Logger.Log("D-Upload : {0}", Filename);
            FtpWebResponse? uploadResponse = null;
            try
            {
                FtpWebRequest uploadRequest = (FtpWebRequest)WebRequest.Create(UploadUrl);
                uploadRequest.Credentials = new NetworkCredential(FtpUsername, FtpPassword);
                uploadRequest.Method = WebRequestMethods.Ftp.UploadFile;
                uploadRequest.UsePassive = false;
                //uploadRequest.KeepAlive = false;
                uploadRequest.Proxy = null;

                using (var requestStream = uploadRequest.GetRequestStream())
                {
                    using (var fileStream = File.Open(Filename, FileMode.Open))
                    {
                        var buffer = new byte[1024];
                        var bytesRead = 0;
                        while (true)
                        {
                            bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                            if (bytesRead == 0)
                            {
                                break;
                            }
                            requestStream.Write(buffer, 0, bytesRead);
                        }

                    } // using (var fileStream ...
                } // using (var requestStream ...

                // At this time, the request stream must be closed before getting the response

                var callback = new AsyncCallback((r) => { });
                var getResponseOk = false;
                var returnValue = uploadRequest.BeginGetResponse(callback, uploadRequest);
                returnValue.AsyncWaitHandle.WaitOne(5000, false);

                if (returnValue.IsCompleted)
                {
                    getResponseOk = true;
                }
                if (!getResponseOk)
                {
                    uploadRequest.Abort();
                    ErrorCode = 2003;
                    ErrorText = "pb uploadfile";
                    Logger.Log($"{ErrorCode} : {ErrorText}");
                }
                uploadResponse = (FtpWebResponse)uploadRequest.EndGetResponse(returnValue);
            }
            catch (UriFormatException ex) 
            {
                ErrorCode = 2003;
                ErrorText = ex.ExceptionMessages();
                //Logger.Log($"Erreur {ErrorCode} : {ErrorText}");
                Logger.Log(ex);
            }
            catch (IOException ex)
            {
                ErrorCode = 2003;
                ErrorText = ex.ExceptionMessages();
                //Logger.Log($"Erreur {ErrorCode} : {ErrorText}");
                Logger.Log(ex);
            }
            catch (WebException ex) 
            {
                ErrorCode = 2003;
                ErrorText = ex.ExceptionMessages();
                //Logger.Log($"Erreur {ErrorCode} : {ErrorText}");
                Logger.Log(ex);
            }
            catch (Exception ex)
            {
                ErrorCode = 2003;
                ErrorText = ex.ExceptionMessages();
                //Logger.Log($"Erreur {ErrorCode} : {ErrorText}");
                Logger.Log(ex);
            }
            finally
            {
                if (uploadResponse != null)
                {
                    uploadResponse.Dispose();
                }
            }
            Logger.Log("Fin put");
        }

        public override async Task ExecuteAsync()
        {
            var url = new Uri(UploadUrl);
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
                Logger.Log("D-Upload : {0}", Filename);
                var uri = string.Format("{0}://{1}", url.Scheme, url.Host);
                Logger.Log($"UPLOAD {UploadUrl}");
                var token = new CancellationToken();
                using (var ftpClient = new AsyncFtpClient(uri, FtpUsername, FtpPassword))
                {
                    // Do not forget ReadTimeout
                    //ftpClient.Config.ConnectTimeout = 15000;
                    //ftpClient.Config.ReadTimeout = 25000;
                    //ftpClient.Config.DataConnectionReadTimeout = 25000;
                    ftpClient.Config.DataConnectionType = FtpDataConnectionType.PORT;

                    // Connect to ftp server
                    await ftpClient.Connect(token);

                    // Upload the file
                    FtpStatus status = FtpStatus.Skipped;
                    using (var fs = new FileStream(Filename, FileMode.Open, FileAccess.Read))
                    {
                        status = await ftpClient.UploadStream(
                            fs,
                            url.AbsolutePath.Substring(1), // Url without root path
                            FtpRemoteExists.OverwriteInPlace,
                            createRemoteDir: false);
                    }

                    //var status = await ftpClient.UploadFile(
                    //    Filename,
                    //    url.AbsolutePath,
                    //    FtpRemoteExists.Overwrite,
                    //    false,
                    //    FtpVerify.None,
                    //    new Progress<FtpProgress>((progress) => { }),
                    //    token);
                    if (status == FtpStatus.Success)
                    {
                        ErrorCode = 0;
                        ErrorText = "";
                    }
                    else if (status == FtpStatus.Failed)
                    {
                        ErrorCode = 2003;
                        ErrorText = "pb uploadfile";
                    }
                    else if (status == FtpStatus.Skipped)
                    {
                        ErrorCode = 2003;
                        ErrorText = $"Téléversement annulé car le fichier existe déjà {url.AbsolutePath}";
                    }
                }
            }
            catch (FtpException ex)
            {
                ErrorCode = 2003;
                ErrorText = ex.ExceptionMessages();
            }
            catch (Exception ex)
            {
                ErrorCode = 2003;
                ErrorText = ex.ExceptionMessages();
            }
        }
   }
}

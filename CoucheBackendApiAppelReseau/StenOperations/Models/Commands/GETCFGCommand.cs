

using StenOperations.Logger;
using StenOperations.Models.Entities;

namespace StenOperations.Models.Commands
{
    public class GETCFGCommand: DefaultCommand
    {
        public GETCFGCommand(ILogger logger, Station station)
            : base(logger, station) 
        {
        }

        public override void Execute()
        {
            var nomFicRep = $"{Context.RootDirPath}/FTP_CFG_{Station.Initiales.Trim()}_REP.txt";
            if (File.Exists(nomFicRep)) 
            {
                File.Delete(nomFicRep);
            }

            var cmdFtp = "GET CONFIG.TXT cfg";
            Logger.Log(cmdFtp);

            // DownloadCmd
            // TODO Download CONFIG.TXT url
            var downloadUrl = $"ftp://{Station.FtpAdresse}/CONFIG.TXT";
            //var downloadUrl = $"ftp://{Station.FtpAdresse}/CONFIG_{Station.Initiales.Trim()}.TXT";
            var downloadCmd = new DownloadCommand(Logger, nomFicRep, downloadUrl, Station.FtpUserName, Station.FtpPassword);
            downloadCmd.Execute();

            if (downloadCmd.ErrorCode == 0) 
            {
                if (File.Exists(nomFicRep)) 
                {
                    // Décodage RS
                    var decodageCmd = new DecodageRSCommand(Logger, nomFicRep, Station) { Context = Context };
                    decodageCmd.Execute();
                    Context = decodageCmd.Context;
                    ErrorCode = decodageCmd.ErrorCode;
                    ErrorText = decodageCmd.ErrorText;
                    if (Context.DiffLTR)
                    {
                        Context.Operation = "PUTLTR";
                    }
                    else
                    {
                        Context.Operation = "TEST_Pertes";
                    }
                 }
                else
                {
                    Context.Repetitions++;
                    ErrorCode = 2008;
                    ErrorText = "Défaut fichier config.txt";
                    Logger.Log(ErrorText);
                }
            }
            else
            {
                Context.Repetitions++;
                Logger.Log(ErrorText);
            }
        }

        public override async Task ExecuteAsync()
        {
            var nomFicRep = $"{Context.RootDirPath}/FTP_CFG_{Station.Initiales.Trim()}_REP.txt";
            if (File.Exists(nomFicRep))
            {
                File.Delete(nomFicRep);
            }

            var cmdFtp = "GET CONFIG.TXT cfg";
            Logger.Log(cmdFtp);

            // DownloadCmd
            // TODO Download CONFIG.TXT url
            var downloadUrl = $"ftp://{Station.FtpAdresse}/CONFIG.TXT";
            //var downloadUrl = $"ftp://{Station.FtpAdresse}/CONFIG_{Station.Initiales.Trim()}.TXT";
            var downloadCmd = new DownloadCommand(Logger, nomFicRep, downloadUrl, Station.FtpUserName, Station.FtpPassword);
            await downloadCmd.ExecuteAsync();

            if (downloadCmd.ErrorCode == 0)
            {
                if (File.Exists(nomFicRep))
                {
                    // Décodage RS
                    var decodageCmd = new DecodageRSCommand(Logger, nomFicRep, Station) { Context = Context };
                    decodageCmd.Execute();
                    Context = decodageCmd.Context;
                    ErrorCode = decodageCmd.ErrorCode;
                    ErrorText = decodageCmd.ErrorText;
                    if (Context.DiffLTR)
                    {
                        Context.Operation = "PUTLTR";
                    }
                    else
                    {
                        Context.Operation = "TEST_Pertes";
                    }
                }
                else
                {
                    Context.Repetitions++;
                    ErrorCode = 2008;
                    ErrorText = "Défaut fichier config.txt";
                    Logger.Log(ErrorText);
                }
            }
            else
            {
                Context.Repetitions++;
                Logger.Log(ErrorText);
            }
        }
    }
}


using StenOperations.Logger;
using StenOperations.Models.Entities;

namespace StenOperations.Models.Commands
{
    public class GETRAZCommand: DefaultCommand
    {
        public GETRAZCommand(ILogger logger, Station station) 
            : base(logger, station) 
        {
        }

        public override void Execute()
        {
            var nomFicRep = $"{Context.RootDirPath}/FTP_RAZ_{Station.Initiales.Trim()}_REP.txt";
            if (File.Exists(nomFicRep))
            {
                File.Delete(nomFicRep);
            }
            Logger.Log("GET CONFIG raz");

            var downloadUrl = $"ftp://{Station.FtpAdresse}/CONFIG.TXT";
            var downloadCmd = new DownloadCommand(Logger, nomFicRep, downloadUrl, Station.FtpUserName, Station.FtpPassword);
            downloadCmd.Execute();
            if (downloadCmd.ErrorCode == 0) 
            {
                if (File.Exists(nomFicRep))
                {
                    Context.Repetitions = 0;
                    var decodageRSCmd = new DecodageRSCommand(Logger, nomFicRep, Station) { Context = Context };
                    decodageRSCmd.Execute();
                    Context = decodageRSCmd.Context;
                    Station.DateDerTrfBase = Station.DateInit;
                    Context.Operation = "FIN";
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
                Logger.Log(downloadCmd.ErrorText);
            }
        }

        public override async Task ExecuteAsync()
        {
            var nomFicRep = $"{Context.RootDirPath}/FTP_RAZ_{Station.Initiales.Trim()}_REP.txt";
            if (File.Exists(nomFicRep))
            {
                File.Delete(nomFicRep);
            }
            Logger.Log("GET CONFIG raz");

            var downloadUrl = $"ftp://{Station.FtpAdresse}/CONFIG.TXT";
            var downloadCmd = new DownloadCommand(Logger, nomFicRep, downloadUrl, Station.FtpUserName, Station.FtpPassword);
            await downloadCmd.ExecuteAsync();
            if (downloadCmd.ErrorCode == 0)
            {
                if (File.Exists(nomFicRep))
                {
                    Context.Repetitions = 0;
                    var decodageRSCmd = new DecodageRSCommand(Logger, nomFicRep, Station) { Context = Context };
                    decodageRSCmd.Execute();
                    Context = decodageRSCmd.Context;
                    Station.DateDerTrfBase = Station.DateInit;
                    Context.Operation = "FIN";
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
                Logger.Log(downloadCmd.ErrorText);
            }
        }
    }
}

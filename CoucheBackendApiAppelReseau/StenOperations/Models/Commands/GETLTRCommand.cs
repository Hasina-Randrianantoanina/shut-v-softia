
using StenOperations.Logger;
using StenOperations.Models.Entities;

namespace StenOperations.Models.Commands
{
    public class GETLTRCommand: DefaultCommand
    {
        public GETLTRCommand(ILogger logger, Station station)
            :base(logger, station) 
        {
        }

        public override void Execute()
        {
            var nomFicRep = $"{Context.RootDirPath}/FTP_LTR_{Station.Initiales.Trim()}_REP.txt";
            if (File.Exists(nomFicRep)) 
            {
                File.Delete(nomFicRep);
            }
            var cdeFtp = "GET CONFIG ltr";
            Logger.Log(cdeFtp);

            // TODO: URL for CONFIG LTR
            var url = $"ftp://{Station.FtpAdresse}/CONFIG.TXT";
            //var url = $"ftp://{Station.FtpAdresse}/CONFIG_LTR_{Station.Initiales.Trim()}.TXT";
            var downloadCmd = new DownloadCommand(Logger, nomFicRep, url, Station.FtpUserName, Station.FtpPassword);
            downloadCmd.Execute();
            if (downloadCmd.ErrorCode == 0) 
            {
                if (File.Exists(nomFicRep)) 
                {
                    Context.Operation = "TEST_Pertes";
                    Context.Repetitions = 0;
                    var decodageRSCmd = new DecodageRSCommand(Logger, nomFicRep, Station) { Context = Context };
                    decodageRSCmd.Execute();
                    // TODO: delete file on prod
                    //File.Delete(nomFicRep);
                }
                else
                {
                    Context.Repetitions++;
                    ErrorCode = 2008;
                    ErrorText = "Défaut de réponse à GetLtr";
                    Logger.Log(ErrorText);
                }
            }
            else
            {
                Logger.Log(downloadCmd.ErrorText);
            }
        }

        public override async Task ExecuteAsync()
        {
            var nomFicRep = $"{Context.RootDirPath}/FTP_LTR_{Station.Initiales.Trim()}_REP.txt";
            if (File.Exists(nomFicRep))
            {
                File.Delete(nomFicRep);
            }
            var cdeFtp = "GET CONFIG ltr";
            Logger.Log(cdeFtp);

            // TODO: URL for CONFIG LTR
            var url = $"ftp://{Station.FtpAdresse}/CONFIG.TXT";
            //var url = $"ftp://{Station.FtpAdresse}/CONFIG_LTR_{Station.Initiales.Trim()}.TXT";
            var downloadCmd = new DownloadCommand(Logger, nomFicRep, url, Station.FtpUserName, Station.FtpPassword);
            await downloadCmd.ExecuteAsync();
            if (downloadCmd.ErrorCode == 0)
            {
                if (File.Exists(nomFicRep))
                {
                    Context.Operation = "TEST_Pertes";
                    Context.Repetitions = 0;
                    var decodageRSCmd = new DecodageRSCommand(Logger, nomFicRep, Station) { Context = Context };
                    decodageRSCmd.Execute();
                    // TODO: delete file on prod
                    File.Delete(nomFicRep);
                }
                else
                {
                    Context.Repetitions++;
                    ErrorCode = 2008;
                    ErrorText = "Défaut de réponse à GetLtr";
                    Logger.Log(ErrorText);
                }
            }
            else
            {
                Logger.Log(downloadCmd.ErrorText);
            }
        }
    }
}



using StenOperations.Logger;
using StenOperations.Models.Entities;

namespace StenOperations.Models.Commands
{
    public class GETREQCommand: DefaultCommand
    {

        public GETREQCommand(ILogger logger, Station station)
        :base(logger, station)
        {
        }

        public override void Execute()
        {
            Thread.Sleep(3000);
            var nomFicRep = $"{Context.RootDirPath}/Result_{Station.Initiales.Trim()}_{DateTime.Now.ToString("dd-MM-yyyy_HHmmss")}.bin";
            if (File.Exists(nomFicRep)) 
            {
                File.Delete(nomFicRep);
            }
            Logger.Log("GET RESULT.BIN");
            // TODO: url RESULT.BIN to be used on real test or prod
            var downloadUrl = $"ftp://{Station.FtpAdresse}/RESULT.BIN";
            //var downloadUrl = $"ftp://{Station.FtpAdresse}/RESULT_{Station.Initiales.Trim()}.BIN";
            var downloadCmd = new DownloadCommand(Logger, nomFicRep, downloadUrl, Station.FtpUserName, Station.FtpPassword);
            downloadCmd.Execute();
            if (downloadCmd.ErrorCode == 0 && File.Exists(nomFicRep))
            {
                Context.NomFicBin = nomFicRep;
                Context.FicBinRecu = true;
                Context.Operation = (Context.RazMemoire) ? "PUTRAZ" : "RESULT";
            }
            else
            {
                Context.Repetitions = 3;
            }
        }

        public override async Task ExecuteAsync()
        {
            var nomFicRep = $"{Context.RootDirPath}/Result_{Station.Initiales.Trim()}_{DateTime.Now.ToString("dd-MM-yyyy_HHmmss")}.bin";
            if (File.Exists(nomFicRep))
            {
                File.Delete(nomFicRep);
            }
            Logger.Log("GET RESULT.BIN");
            // TODO: url RESULT.BIN to be used on real test or prod
            var downloadUrl = $"ftp://{Station.FtpAdresse}/RESULT.BIN";
            //var downloadUrl = $"ftp://{Station.FtpAdresse}/RESULT_{Station.Initiales.Trim()}.BIN";
            var downloadCmd = new DownloadCommand(Logger, nomFicRep, downloadUrl, Station.FtpUserName, Station.FtpPassword);
            await downloadCmd.ExecuteAsync();
            if (downloadCmd.ErrorCode == 0 && File.Exists(nomFicRep))
            {
                Context.NomFicBin = nomFicRep;
                Context.FicBinRecu = true;
                Context.Operation = (Context.RazMemoire) ? "PUTRAZ" : "RESULT";
            }
            else
            {
                Context.Repetitions = 3;
            }
        }

    }
}

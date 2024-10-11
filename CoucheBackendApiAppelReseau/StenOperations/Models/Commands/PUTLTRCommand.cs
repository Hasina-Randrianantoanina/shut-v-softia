
using StenOperations.Logger;
using StenOperations.Models.Entities;
using System.Text;

namespace StenOperations.Models.Commands
{
    public class PUTLTRCommand: DefaultCommand
    {

        public PUTLTRCommand(ILogger logger, Station station)
        :base(logger, station) 
        {
        }

        public override void Execute()
        {
            var nomFicCde = $"{Context.RootDirPath}/FTP_LTR_{Station.Initiales.Trim()}_CDE.txt";
            if (File.Exists(nomFicCde))
            {
                File.Delete(nomFicCde);
            }
            var cdeFtp = "PUT CONFIG.TXT ltr";
            Logger.Log(cdeFtp);

            // Build CONFIG.TXT
            var configContent = MakeConfigContent();
            File.WriteAllText(nomFicCde, configContent);

            // TODO: URL for CONFIG LTR CONFIG.TXT
            var uploadUrl = $"ftp://{Station.FtpAdresse}/CONFIG.TXT";
            //var uploadUrl = $"ftp://{Station.FtpAdresse}/CONFIG_LTR.TXT";
            Logger.Log($"->UPLOAD: '{nomFicCde}', url: '{uploadUrl}'");
            Logger.Log(configContent);

            var uploadCmd = new UploadCommand(Logger, nomFicCde, uploadUrl, Station.FtpUserName, Station.FtpPassword);
            uploadCmd.Execute();
            if (uploadCmd.ErrorCode == 0)
            {
                Context.Repetitions = 0;
                // TODO: delete file command on prod 
                File.Delete(nomFicCde);
                Context.Operation = "GETLTR";
            }
            else
            {
                Context.Repetitions++;
                Logger.Log(uploadCmd.ErrorText);
            }
            ErrorCode = uploadCmd.ErrorCode;
            ErrorText = uploadCmd.ErrorText;
        }

        public override async Task ExecuteAsync()
        {
            var nomFicCde = $"{Context.RootDirPath}/FTP_LTR_{Station.Initiales.Trim()}_CDE.txt";
            if (File.Exists(nomFicCde))
            {
                File.Delete(nomFicCde);
            }
            var cdeFtp = "PUT CONFIG.TXT ltr";
            Logger.Log(cdeFtp);

            // Build CONFIG.TXT
            var configContent = MakeConfigContent();
            File.WriteAllText(nomFicCde, configContent);

            // TODO: URL for CONFIG LTR CONFIG.TXT
            var uploadUrl = $"ftp://{Station.FtpAdresse}/CONFIG.TXT";
            //var uploadUrl = $"ftp://{Station.FtpAdresse}/CONFIG_LTR.TXT";
            Logger.Log($"->UPLOAD: '{nomFicCde}', url: '{uploadUrl}'");
            Logger.Log(configContent);

            var uploadCmd = new UploadCommand(Logger, nomFicCde, uploadUrl, Station.FtpUserName, Station.FtpPassword);
            await uploadCmd.ExecuteAsync();
            if (uploadCmd.ErrorCode == 0)
            {
                Context.Repetitions = 0;
                // TODO: delete file command on prod 
                File.Delete(nomFicCde);
                Context.Operation = "GETLTR";
            }
            else
            {
                Context.Repetitions++;
                Logger.Log(uploadCmd.ErrorText);
            }
            ErrorCode = uploadCmd.ErrorCode;
            ErrorText = uploadCmd.ErrorText;
        }

        private string MakeConfigContent()
        {
            var cdup = '.';
            var sb = new StringBuilder();
            var tx = $"<L3400{new string(cdup, 19)}"; // date shut
            sb.AppendLine(tx);
            // TODO: source de Station.n_vint_u, Station.n_et_u ??
            var nbo = 5 + Station.n_vint_u * 9;
            tx = $"<L23{Station.n_vint_u.ToString("D2")}{new string(cdup, nbo)}"; // voies internes
            sb.AppendLine(tx);
            nbo = 5 + Station.n_et_u;
            tx = $"<L29{Station.n_et_u.ToString("D2")}{new string(cdup, nbo)}"; // voies etat
            sb.AppendLine(tx);

            tx = $"<L33{Context.JourCourant.ToString("D2")}{new string(cdup, 15)}"; // enrg jour courant
            sb.AppendLine(tx);

            tx = $"<L6000{new string(cdup, 19)}"; // date go
            sb.AppendLine(tx);
            tx = $"<L6100{new string(cdup, 19)}"; // date init
            sb.AppendLine(tx);
            tx = $"<L6200{new string(cdup, 19)}"; // date acq
            sb.AppendLine(tx);
            tx = $"<L6300{new string(cdup, 19)}"; // date enrg
            sb.AppendLine(tx);
            tx = $"<L6400{new string(cdup, 19)}"; // date stop
            sb.AppendLine(tx);

            return sb.ToString();
        }
    }
}


using StenOperations.Logger;
using StenOperations.Models.Entities;
using System.Text;

namespace StenOperations.Models.Commands
{
    public class PUTRAZCommand: DefaultCommand
    {
        public PUTRAZCommand(ILogger logger, Station station)
            : base(logger, station) 
        {
        }

        public override void Execute()
        {
            var nomFicCde = $"{Context.RootDirPath}/FTP_RAZ_{Station.Initiales.Trim()}_CDE.txt";
            if (File.Exists(nomFicCde)) 
            {
                File.Delete(nomFicCde);
            }
            Logger.Log("PUT CONFIG.TXT raz");

            // Make Config Content
            var content = MakeConfigContent();
            File.WriteAllText(nomFicCde, content);

            var uploadUrl = $"ftp://{Station.FtpAdresse}/CONFIG.TXT";
            Logger.Log($"->UPLOAD: '{nomFicCde}', url: '{uploadUrl}'");
            Logger.Log(content);

            // Upload
            var uploadCmd = new UploadCommand(Logger, nomFicCde, uploadUrl, Station.FtpUserName, Station.FtpPassword);
            uploadCmd.Execute();
            if (uploadCmd.ErrorCode == 0) 
            {
                Context.Repetitions = 0;
                // TODO: delete file when on prod
                File.Delete(nomFicCde);
                Context.Operation = "GETRAZ";
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
            var nomFicCde = $"{Context.RootDirPath}/FTP_RAZ_{Station.Initiales.Trim()}_CDE.txt";
            if (File.Exists(nomFicCde))
            {
                File.Delete(nomFicCde);
            }
            Logger.Log("PUT CONFIG.TXT raz");

            // Make Config Content
            var content = MakeConfigContent();
            File.WriteAllText(nomFicCde, content);

            var uploadUrl = $"ftp://{Station.FtpAdresse}/CONFIG.TXT";
            Logger.Log($"->UPLOAD: '{nomFicCde}', url: '{uploadUrl}'");
            Logger.Log(content);

            // Upload
            var uploadCmd = new UploadCommand(Logger, nomFicCde, uploadUrl, Station.FtpUserName, Station.FtpPassword);
            await uploadCmd.ExecuteAsync();
            if (uploadCmd.ErrorCode == 0)
            {
                Context.Repetitions = 0;
                // TODO: delete file when on prod
                File.Delete(nomFicCde);
                Context.Operation = "GETRAZ";
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
            sb.AppendLine($"<E9000{new string(cdup, 19)}");
            sb.AppendLine($"<L6100{new string(cdup, 19)}"); // date param
            sb.AppendLine($"<L2500{new string(cdup, 19)}"); // info enrg en cours
            // sb.AppendLine($"<L2600{new string(cdup, 19)}"); // info enrg en cours
            
            return sb.ToString();
        }
    }
}

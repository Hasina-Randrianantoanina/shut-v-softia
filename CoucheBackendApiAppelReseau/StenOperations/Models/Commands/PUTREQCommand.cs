
using StenOperations.Logger;
using StenOperations.Models.Entities;
using System.Runtime.InteropServices;
using System.Text;

namespace StenOperations.Models.Commands
{
    public class PUTREQCommand: DefaultCommand
    {
        public PUTREQCommand(ILogger logger, Station station)
            : base(logger, station) 
        { 
        }

        public override void Execute()
        {
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (isWindows)
            {
                ExecuteWinCode();
            }
            else
            {
                ExecuteLinuxCode();
            }
        }

        public override async Task ExecuteAsync()
        {
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (isWindows)
            {
                await ExecuteWinAsync();
            }
            else
            {
                ExecuteLinuxCode();
            }
        }

        private void ExecuteWinCode()
        {
            var nomFicCde = $"{Context.RootDirPath}/Requete_{Station.Initiales.Trim()}.txt";
            if (File.Exists(nomFicCde)) 
            {
                File.Delete(nomFicCde);
            }
            File.WriteAllText(nomFicCde, MakeRequeteContent());

            Logger.Log("PUT REQUETE.TXT");
            var dateStr = Station.DateDerTrfBase.ToString("dd/MM/yyyy");
            Logger.Log($"=> DEB='{dateStr}', FIN='{dateStr}'");

            // TODO: URL of REQUETE.TXT
            var uploadUrl = $"ftp://{Station.FtpAdresse}/REQUETE.TXT";
            //var uploadUrl = $"ftp://{Station.FtpAdresse}/REQUETE_{Station.Initiales.Trim()}.TXT";
            var uploadCmd = new UploadCommand(Logger, nomFicCde, uploadUrl, Station.FtpUserName, Station.FtpPassword);
            uploadCmd.Execute();
            if (uploadCmd.ErrorCode == 0) 
            {
                Context.Operation = "DIRREQ";
                Context.Operation = "GETREQ";
            }
            else
            {
                Context.Repetitions++;
            }
        }

        private async Task ExecuteWinAsync()
        {
            var nomFicCde = $"{Context.RootDirPath}/Requete_{Station.Initiales.Trim()}.txt";
            if (File.Exists(nomFicCde))
            {
                File.Delete(nomFicCde);
            }
            File.WriteAllText(nomFicCde, MakeRequeteContent());

            Logger.Log("PUT REQUETE.TXT");
            var dateStr = Station.DateDerTrfBase.ToString("dd/MM/yyyy");
            Logger.Log($"=> DEB='{dateStr}', FIN='{dateStr}'");

            // TODO: URL of REQUETE.TXT
            var uploadUrl = $"ftp://{Station.FtpAdresse}/REQUETE.TXT";
            //var uploadUrl = $"ftp://{Station.FtpAdresse}/REQUETE_{Station.Initiales.Trim()}.TXT";
            var uploadCmd = new UploadCommand(Logger, nomFicCde, uploadUrl, Station.FtpUserName, Station.FtpPassword);
            await uploadCmd.ExecuteAsync();
            if (uploadCmd.ErrorCode == 0)
            {
                Context.Operation = "DIRREQ";
                Context.Operation = "GETREQ";
            }
            else
            {
                Context.Repetitions++;
            }
        }

        private void ExecuteLinuxCode()
        {
            // Build REQUETE.TXT
            var nomFicCde = $"{Context.RootDirPath}/Requete_{Station.Initiales.Trim()}.txt";
            if (File.Exists(nomFicCde))
            {
                File.Delete(nomFicCde);
            }
            File.WriteAllText(nomFicCde, MakeRequeteContent());

            Logger.Log("PUT REQUETE.TXT");
            var dateStr = Station.DateDerTrfBase.ToString("dd/MM/yyyy");
            Logger.Log($"=> DEB='{dateStr}', FIN='{dateStr}'");

            // Build Script Bash
            var nomScript = $"{Context.RootDirPath}/ftp_script_{Station.Initiales.Trim()}.sh";
            if (File.Exists(nomScript))
            {
                File.Delete(nomScript);
            }
            File.WriteAllText(nomScript, MakeRequeteScript(nomFicCde));

            // Execute Script ftp to upload the REQUETE.TXT
            // make the script executable
            var execProgCmd = new ExecProgCommand(Logger, Station, "chmod", $"+x {nomScript}");
            execProgCmd.Execute();
            if (!string.IsNullOrEmpty(execProgCmd.ErrorText))
            {
                Context.Repetitions++;
                return;
            }

            // execute the script
            execProgCmd = new ExecProgCommand(Logger, Station, "bash", $"{nomScript}");
            execProgCmd.Execute();
            if (string.IsNullOrEmpty(execProgCmd.ErrorText))
            {
                Context.Operation = "DIRREQ";
                Context.Operation = "GETREQ";
            }
            else
            {
                Context.Repetitions++;
            }
        }

        private string MakeRequeteContent()
        {
            var sb = new StringBuilder();
            var dateStr = Station.DateDerTrfBase.ToString("dd/MM/yyyy");
            sb.AppendLine($"DEB={dateStr}");
            sb.AppendLine($"FIN={dateStr}");

            return sb.ToString();
        }

        private string MakeRequeteScript(string requeteLocalPath, string filename = "REQUETE.TXT")
        {
            var sb = new StringBuilder();
            sb.AppendLine("#!/bin/bash");
            sb.AppendLine("");
            sb.AppendLine($"HOST={Station.FtpAdresse}");
            sb.AppendLine($"USERNAME={Station.FtpUserName}");
            sb.AppendLine($"PASSWORD={Station.FtpPassword}");
            sb.AppendLine($"FILE={filename}");
            sb.AppendLine($"LOCAL={requeteLocalPath}");
            sb.AppendLine("");
            sb.AppendLine("ftp -inv $HOST <<EOF");
            sb.AppendLine("user $USERNAME $PASSWORD");
            sb.AppendLine("passive off");
            sb.AppendLine("put $LOCAL $FILE");
            sb.AppendLine("bye");
            sb.AppendLine("EOF");

            return sb.ToString();
        }
    }
}

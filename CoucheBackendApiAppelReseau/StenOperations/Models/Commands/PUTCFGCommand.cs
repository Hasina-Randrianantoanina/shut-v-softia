
using StenOperations.Logger;
using StenOperations.Models.Entities;
using System.Text;

namespace StenOperations.Models.Commands
{
    public class PUTCFGCommand: DefaultCommand
    {
        public PUTCFGCommand(ILogger logger, Station station)
            : base(logger, station) 
        {
        }

        public override void Execute()
        {
            // Delete existing file
            var nomFicCde = $"{Context.RootDirPath}/FTP_CFG_{Station.Initiales.Trim()}_CDE.txt";            
            if (File.Exists(nomFicCde)) 
            {
                File.Delete(nomFicCde);
            }

            // Make file content
            var content = MakeFileContent();
            File.WriteAllText(nomFicCde, content);

            // Create config file
            Logger.Log("PUT CONFIG.TXT cfg");

            // Upload the file
            var uploadUrl = $"ftp://{Station.FtpAdresse}/CONFIG.TXT";
            Logger.Log($"->UPLOAD: '{nomFicCde}', url: '{uploadUrl}'");
            Logger.Log(content);

            var uploadCmd = new UploadCommand(Logger, nomFicCde, uploadUrl, Station.FtpUserName, Station.FtpPassword);
            uploadCmd.Execute();
            if (uploadCmd.ErrorCode == 0)
            {
                Context.Repetitions = 0;
                // TODO: delete nomFicCde on prod
                //File.Delete(nomFicCde);
                Context.Operation = "GETCFG";
            }
            else
            {
                Context.Repetitions++;
                Logger.Log(ErrorText);
            }
        }

        public override async Task ExecuteAsync()
        {
            // Delete existing file
            var nomFicCde = $"{Context.RootDirPath}/FTP_CFG_{Station.Initiales.Trim()}_CDE.txt";
            if (File.Exists(nomFicCde))
            {
                File.Delete(nomFicCde);
            }

            var content = MakeFileContent();
            File.WriteAllText(nomFicCde, content);

            // Create config file
            Logger.Log("PUT CONFIG.TXT cfg");

            // Upload the file
            var uploadUrl = $"ftp://{Station.FtpAdresse}/CONFIG.TXT";
            Logger.Log($"->UPLOAD: '{nomFicCde}', url: '{uploadUrl}'");
            Logger.Log(content);

            var uploadCmd = new UploadCommand(Logger, nomFicCde, uploadUrl, Station.FtpUserName, Station.FtpPassword);
            await uploadCmd.ExecuteAsync();
            if (uploadCmd.ErrorCode == 0)
            {
                Context.Repetitions = 0;
                // TODO: delete nomFicCde on prod
                File.Delete(nomFicCde);
                Context.Operation = "GETCFG";
            }
            else
            {
                Context.Repetitions++;
                Logger.Log(ErrorText);
            }
        }

        private string MakeFileContent()
        {
            var sb = new StringBuilder();
            var cdup = '.';

            var tx = "<L3600" + (new string(cdup, 40)); // version logiciel
            sb.AppendLine(tx);
            tx = "<L3900" + (new string(cdup, 19)); // date Version
            sb.AppendLine(tx);
            tx = "<L1000" + (new string(cdup, 79)); // stockage et ve
            sb.AppendLine(tx);
            tx = "<L0400" + (new string(cdup, 71)); // ordre v internes
            sb.AppendLine(tx);
            tx = "<L1400" + (new string(cdup, 23)); // ordre v etat
            sb.AppendLine(tx);
            tx = "<L2600" + (new string(cdup, 27)); // date vidage
            sb.AppendLine(tx);
            tx = "<L3500" + (new string(cdup, 19)); // date param
            sb.AppendLine(tx);
            tx = "<L2500" + (new string(cdup, 19)); // infos enrg en cours
            sb.AppendLine(tx);

            tx = "<L3400" + (new string(cdup, 19)); // date shut
            sb.AppendLine(tx);

            // TODO: D'où vient Station.n_vint_u: calcul à partir des données de base (compte le nombre de voies internes)
            var nbo = 5 + Station.n_vint_u * 9;
            // vb.net equivalent of Format(Station.n_vint_u, "00"): Station.n_vint_u.ToString("D2")
            tx = "<L23" + Station.n_vint_u.ToString("D2") + (new string(cdup, nbo)); // voies internes
            sb.AppendLine(tx);
            // TODO: D'où vient Station.n_et_u: calcul à partir des données de base (compte le nombre de voies etats)
            nbo = 5 + Station.n_et_u;
            // vb.net equivalent of Format(BD.sta.n_et_u, "00"): Station.n_et_u.ToString("D2")
            tx = "<L29" + Station.n_et_u.ToString("D2") + (new string(cdup, nbo)); // voies etat
            sb.AppendLine(tx);

            // tx = "<L33" & Format(JourCourant, "00") + (new string(cdup, 15, cdup)  ' enrg jour courant
            // sb.AppendLine(tx);

            tx = "<L3064" + (new string(cdup, 261));
            sb.AppendLine(tx);
            tx = "<L3164" + (new string(cdup, 261));
            sb.AppendLine(tx);
            tx = "<L3264" + (new string(cdup, 261));
            sb.AppendLine(tx);

            tx = "<L6000" + (new string(cdup, 19)); // date go
            sb.AppendLine(tx);
            tx = "<L6100" + (new string(cdup, 19)); // date init
            sb.AppendLine(tx);
            tx = "<L6200" + (new string(cdup, 19)); // date acq
            sb.AppendLine(tx);
            tx = "<L6300" + (new string(cdup, 19)); // date enrg
            sb.AppendLine(tx);
            tx = "<L6400" + (new string(cdup, 19)); // date stop
            sb.AppendLine(tx);

            return sb.ToString();
        }
    }
}



using StenOperations.Data;
using StenOperations.Logger;
using StenOperations.Models.Entities;

namespace StenOperations.Models.Commands
{
    public class GETSTATUSCommand: DefaultCommand
    {
        public PersistenceService Repository { get; set; }

        public GETSTATUSCommand(ILogger logger, Station station, PersistenceService repository)
        :base(logger, station)
        {
            Repository = repository;
        }

        public override void Execute()
        {
            var nomFicRep = $"{Context.RootDirPath}/FTP_status_{Station.Initiales.Trim()}.txt";
            if (File.Exists(nomFicRep)) 
            {
                File.Delete(nomFicRep);
            }
            Logger.Log("GET STATUS.TXT {0}", nomFicRep);
            // TODO: URL to get STATUS.TXT
            var url = $"ftp://{Station.FtpAdresse}/STATUS.TXT";
            //var url = $"ftp://{Station.FtpAdresse}/STATUS_{Station.Initiales.Trim()}.TXT";
            var downloadCmd = new DownloadCommand(Logger, nomFicRep, url, Station.FtpUserName, Station.FtpPassword);
            downloadCmd.Execute();
            ErrorCode = downloadCmd.ErrorCode;
            ErrorText = downloadCmd.ErrorText;
            if (downloadCmd.ErrorCode == 0) 
            {
                if (File.Exists(nomFicRep))
                {
                    var lignes = "";
                    using (var sr = new StreamReader(nomFicRep))
                    {
                        lignes = sr.ReadToEnd();
                    }
                    Logger.Log($"->DOWNLOAD: '{nomFicRep}', url: '{url}'");
                    Logger.Log(lignes);        
                    // TODO: Ne pas supprimer le fichier pour voir le contenu
                    //File.Delete(nomFicRep);

                    var taches = "";
                    string[]? etats = null;
                    // Get Etat Taches
                    var x = lignes.IndexOf("Etat Taches : ");
                    if (x >= 0)
                    {
                        taches = lignes.Substring(x, x + 14);
                        var y = taches.IndexOf("LISTE DE LA TABLE DES JOURS");
                        if (y >= 0)
                            taches = taches.Substring(0, y - 1);
                        taches = taches.Replace("  ", "")
                            .Replace("\r", "")
                            .Replace((char)10, ' ')
                            .Replace("  ", "");
                        if (y < 0)
                            taches = taches.Substring(0, 95);
                        etats = taches.Split(" ");
                    }
                    if ((etats == null) || (etats.Length < 16) || etats[15] == null)
                    {
                        taches = "";
                    }
                    Station.Taches = taches;
                    // TODO: Gestion Alerte Taches
                    Repository.GestionAlerteTaches(Station);

                    // Get Version
                    x = lignes.IndexOf("VERSION=");
                    if (x >= 0) 
                    {
                        lignes = lignes.Substring(x + 8, 11);
                        Station.VersionEnrg = lignes;
                        var versionDialog = 0;
                        int.TryParse(lignes.Substring(1, 2), out versionDialog);
                        Station.VersionDialog = versionDialog;
                        Station.VersionEnregistrement = lignes.Substring(3, 3);
                        Logger.Log(lignes);
                        if (Station.VersionDialog >= 10)
                        {
                            Context.Operation = "PUTCFG";
                        }
                        else
                        {
                            Context.Operation = "PUTREQ";
                        }
                    }
                    else 
                    {
                        Context.Operation = "PUTREQ";
                    }
                }
                else
                {
                    Context.Repetitions++;
                    ErrorCode = 2008;
                    ErrorText = "Défaut fichier Status.txt";
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
            var nomFicRep = $"{Context.RootDirPath}/FTP_status_{Station.Initiales.Trim()}.txt";
            if (File.Exists(nomFicRep))
            {
                File.Delete(nomFicRep);
            }
            Logger.Log("GET STATUS.TXT {0}", nomFicRep);
            // TODO: URL to get STATUS.TXT
            var url = $"ftp://{Station.FtpAdresse}/STATUS.TXT";
            //var url = $"ftp://{Station.FtpAdresse}/STATUS_{Station.Initiales.Trim()}.TXT";
            var downloadCmd = new DownloadCommand(Logger, nomFicRep, url, Station.FtpUserName, Station.FtpPassword);
            await downloadCmd.ExecuteAsync();
            ErrorCode = downloadCmd.ErrorCode;
            ErrorText = downloadCmd.ErrorText;
            if (downloadCmd.ErrorCode == 0)
            {
                if (File.Exists(nomFicRep))
                {
                    var lignes = "";
                    using (var sr = new StreamReader(nomFicRep))
                    {
                        lignes = sr.ReadToEnd();
                    }
                    Logger.Log($"->DOWNLOAD: '{nomFicRep}', url: '{url}'");
                    Logger.Log(lignes);
                    // TODO: Ne pas supprimer le fichier pour voir le contenu
                    //File.Delete(nomFicRep);

                    var taches = "";
                    string[]? etats = null;
                    // Get Etat Taches
                    var x = lignes.IndexOf("Etat Taches : ");
                    if (x >= 0)
                    {
                        taches = lignes.Substring(x, x + 14);
                        var y = taches.IndexOf("LISTE DE LA TABLE DES JOURS");
                        if (y >= 0)
                            taches = taches.Substring(0, y - 1);
                        taches = taches.Replace("  ", "")
                            .Replace("\r", "")
                            .Replace((char)10, ' ')
                            .Replace("  ", "");
                        if (y < 0)
                            taches = taches.Substring(0, 95);
                        etats = taches.Split(" ");
                    }
                    if ((etats == null) || (etats.Length < 16) || etats[15] == null)
                    {
                        taches = "";
                    }
                    Station.Taches = taches;
                    // TODO: Gestion Alerte Taches
                    await Repository.GestionAlerteTaches(Station);

                    // Get Version
                    x = lignes.IndexOf("VERSION=");
                    if (x >= 0)
                    {
                        lignes = lignes.Substring(x + 8, 11);
                        Station.VersionEnrg = lignes;
                        var versionDialog = 0;
                        int.TryParse(lignes.Substring(1, 2), out versionDialog);
                        Station.VersionDialog = versionDialog;
                        Station.VersionEnregistrement = lignes.Substring(3, 3);
                        Logger.Log(lignes);
                        if (Station.VersionDialog >= 10)
                        {
                            Context.Operation = "PUTCFG";
                        }
                        else
                        {
                            Context.Operation = "PUTREQ";
                        }
                    }
                    else
                    {
                        Context.Operation = "PUTREQ";
                    }
                }
                else
                {
                    Context.Repetitions++;
                    ErrorCode = 2008;
                    ErrorText = "Défaut fichier Status.txt";
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

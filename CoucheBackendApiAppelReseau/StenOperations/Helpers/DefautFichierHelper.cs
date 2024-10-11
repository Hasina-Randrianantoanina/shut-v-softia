
using StenOperations.Data;
using StenOperations.Logger;
using StenOperations.Models.Entities;
using System.Data;

namespace StenOperations.Helpers
{
    public class DefautFichierHelper
    {
        private int ErrorCode { get; set; }
        private string evtSrc = "";
        private string[] lignesDef = new string[300];
        private bool flagFSrc = false;
        private bool finFicSrc = false;
        public List<DefautVoie> DefautVoies { get; private set; } = new List<DefautVoie>();
        public DateTime DateSrc { get; set; }
        public Station Station { get; set; }
        public ILogger Logger { get; set; }
        public PersistenceService Repository { get; set; }

        public DefautFichierHelper(ILogger logger, Station station, PersistenceService repository)
        {
            Station = station ?? throw new ArgumentNullException($"{nameof(station)} is Null");
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<int> TraiterDefautFichierSource2(string nomFicMan)
        {
            // Compte le nombre de $XXhXX a9999 bZZZZ
            var nbdef = CompteDefautsFichierSource(nomFicMan);
            if (nbdef > 0 && nbdef < 300)
            {
                // Ecrit le nombre de défauts dans un fichier
                var utilFic = $"DefautsCapteurs_{Station.Initiales.Trim()}.txt";
                File.WriteAllLines(utilFic, lignesDef);

                // Comptage apparitions
                DateTime.TryParse(lignesDef[0].Substring(0, 19), out var date1erDef);
                Repository.LoadPertesEnrg(Station, date1erDef);
                nbdef = await TraiterApparitions(lignesDef);

                // Comptage disparitions
                Repository.LoadPertesEnrgNoDate(Station, date1erDef);
                await TraiterDisparitions(lignesDef);
            }
            return nbdef;
        }

        public async Task<int> TraiterDefautFichierSource(string nomFicMan)
        {
            // Compte le nombre de $XXhXX a9999 bZZZZ
            var nbdef = CompteDefautsFichierSource(nomFicMan);
            if (nbdef > 0 && nbdef < 300)
            {
                // Ecrit le nombre de défauts dans un fichier
                var utilFic = $"DefautsCapteurs_{Station.Initiales.Trim()}.txt";
                File.WriteAllLines(utilFic, lignesDef);

                // Comptage apparitions
                var date1erDef = (DefautVoies.Count > 0) ? DefautVoies[0].DateDefaut : DateTime.Now;
                await Repository.PertesEtatCaptApparitions(Station, date1erDef, DefautVoies);
                await Repository.PertesEtatCaptDisparitions(Station, DefautVoies);
            }

            return nbdef;
        }

        private int CompteDefautsFichierSource(string nomFicMan)
        {
            if (!File.Exists(nomFicMan))
            {
                return 0;
            }
            var readText = File.ReadAllLines(nomFicMan);
            if (readText.Length == 0)
            {
                return 0;
            }

            var nbDefauts = 0;
            var i = 0;
            var ok = false;
            do
            {
                var lgSrc = readText[i];
                if (!ok)
                {
                    // Cherche la 1ère ligne E pour valider une valeur jour
                    if (lgSrc.StartsWith("E"))
                    {
                        // Test si ligne E correcte et retourne DateSrc
                        DHlgSrc(lgSrc);    
                        ok = true;
                    }
                }
                else
                {
                    // Cherche DateSrc
                    DHlgSrc(lgSrc);
                    var x = lgSrc.IndexOf("a9999 b");
                    if ((evtSrc == "$") && (x >= 0) && (lgSrc.Length >= 18))
                    {
                        lignesDef[nbDefauts] = $"{DateSrc.ToString()} {lgSrc.Substring(14)}";
                        DefautVoies.Add(new DefautVoie { DateDefaut = DateSrc, RawData = lgSrc.Substring(14).Trim() });
                        nbDefauts++;
                        if (nbDefauts >= 300)
                        {
                            break;
                        }
                    }
                }
                i++;
            }
            while (i < readText.Length && !finFicSrc);

            finFicSrc = false;
            flagFSrc = false;

            return nbDefauts;
        }

        private bool DHlgSrc(string lgSrc)
        {
            var dh = DateTime.MinValue;
            evtSrc = lgSrc.Substring(0, 1);
            var errDate = false;
            switch(evtSrc)
            {
                case "#":
                    break;
                case "D":
                    {
                        if (lgSrc.Length != 23) { ErrorCode = 5; }
                        var splits = lgSrc.Substring(1).Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        errDate = true;
                        if (splits.Length >= 2)
                        {
                            var dateStr = $"{splits[0]} {splits[1].Replace("h", ":")}:00";
                            errDate = !DateTime.TryParse(dateStr, out dh);
                        }
                    }
                    break;
                case "E":
                    {
                        if (lgSrc.Length != 17) { ErrorCode = 5; }
                        var splits = lgSrc.Substring(1).Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        errDate = true;
                        if (splits.Length > 0)
                        {
                            var dateStr = splits[0];
                            errDate = !DateTime.TryParse(dateStr, out dh);
                        }
                    }
                    break;
                case "F": 
                    {
                        if (lgSrc.Length != 18) { ErrorCode = 5; }
                        var splits = lgSrc.Substring(1).Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        errDate = true;
                        if (splits.Length >= 2)
                        {
                            var dateStr = $"{splits[0]} {splits[1].Replace("h", ":")}:00";
                            errDate = !DateTime.TryParse(dateStr, out dh);
                        }
                        flagFSrc = true;
                    }
                    break;
                default:
                    {
                        if (lgSrc.Length < 6) { ErrorCode = 5; }
                        var hourStr = lgSrc.Replace("h", ":").Substring(1, 5);
                        var dateStr = $"{DateSrc.ToString("dd/MM/yyyy")} {hourStr}:00";
                        errDate = !DateTime.TryParse(dateStr, out dh);
                    }
                    break;
            }

            if (dh.Ticks != 0 && !errDate) { DateSrc = dh; } // Si pas d'erreur de date
            var etoile = lgSrc.IndexOf("*");
            finFicSrc = finFicSrc || ErrorCode != 0 || etoile != -1 || flagFSrc || errDate;
            return false;
        }



        private async Task<int> TraiterApparitions(string[] lignesDef)
        {
            TimeSpan ofx = default(TimeSpan);
            var l = 0;
            var nomVoie = "";
            var ok = 0;
            var insertions = 0;
            while (!string.IsNullOrEmpty(lignesDef[l]))
            {
                var lgSrc = lignesDef[l];
                DateTime.TryParse(lgSrc.Substring(0, 19), out var dateSrc);
                DateSrc = dateSrc;
                var defaut = 0;
                int.TryParse(lgSrc.Substring(20, 2), out defaut);
                var typeDefaut = ((int)(defaut / 10)) * 10;
                var noVoie = 0;
                int.TryParse(lgSrc.Substring(22, 2), out noVoie);
                
                switch(defaut)
                {
                    case 11: // Voie etat
                        {
                            nomVoie = $"Voie Etat {noVoie}";
                            var ve = Station.VEtat!.Where(v => v.ETnum != 0 && v.ETmodule == noVoie.ToString())
                                    .FirstOrDefault();
                            if (ve != null) 
                            {
                                nomVoie = ve.ETlibel;
                                // TODO Insérer PertesEnrg: typeDefaut, defaut, noVoie, nomVoie, dateSrc, ofx (TimeSpan), aga (false)
                                // ok: nb database insertions
                                Logger.Log($"PerteEtatCapteurApp: TypeDefaut: {typeDefaut}, Défaut: {defaut}, Voie: {noVoie} - {nomVoie}, DateSrc: {dateSrc}, aga: {false}");
                                //ok = Repository.PerteEtatCaptApp(Station, typeDefaut, defaut, noVoie, nomVoie ?? "", DateSrc, ofx, aga: false);

                                // New version
                                ok = await Repository.PerteEtatCaptApp(Station, "ETAT", null, ve, DateSrc, ofx, aga: false);
                            }
                        }
                        break;
                    case 21: // Voie interne
                        {
                            noVoie++;
                            var vi = (noVoie < Station.VInt!.Count) ? Station.VInt[noVoie] : null;
                            nomVoie = (noVoie < Station.VInt!.Count) ? Station.VInt[noVoie].Libel : "";
                            if (!string.IsNullOrEmpty(nomVoie))
                            {
                                // TODO Insérer PertesEnrg: typeDefaut, defaut, noVoie, nomVoie, DateSrc, ofx (TimeSpan), aga (Station.VInt[noVoie].aga)
                                Logger.Log($"PerteEtatCapteurApp: TypeDefaut: {typeDefaut}, Défaut: {defaut}, Voie: {noVoie} - {nomVoie}, DateSrc: {dateSrc}, aga: {Station.VInt[noVoie].Aga}");
                                //ok = Repository.PerteEtatCaptApp(Station, typeDefaut, defaut, noVoie, nomVoie, DateSrc, ofx, aga: Station.VInt[noVoie].Aga);

                                // New version
                                ok = await Repository.PerteEtatCaptApp(Station, "CAPTEUR", vi, null, DateSrc, ofx, aga: false);
                            }
                        }
                        break;
                    default:
                        break;
                } // switch (defaut)
                l++;
                if (ok == 1) { insertions++; }
                ok = 0;
            }
            return insertions;
        }



        private async Task<bool> TraiterDisparitions(string[] lignesDef)
        {
            var l = 0;
            var ok = 0;
            var ofx = default(TimeSpan);
            var nomVoie = "";
            while (!string.IsNullOrEmpty(lignesDef[l]))
            {
                var lgSrc = lignesDef[l];
                DateTime.TryParse(lgSrc.Substring(0, 19), out var dateSrc);
                DateSrc = dateSrc;
                var defaut = 0;
                int.TryParse(lgSrc.Substring(20, 2), out defaut);
                var typeDefaut = ((int)(defaut / 10)) * 10;
                var noVoie = 0;
                int.TryParse(lgSrc.Substring(22, 2), out noVoie);
                switch (defaut)
                {
                    case 10: // Voie etat
                        {
                            nomVoie = $"Voie Etat {noVoie}";
                            var ve = Station.VEtat!.Where(v => v.ETnum != 0 && v.ETmodule == noVoie.ToString())
                                    .FirstOrDefault();
                            if (ve != null)
                            {
                                nomVoie = ve.ETlibel;
                                // TODO Update PertesEnrg: typeDefaut, defaut, noVoie, nomVoie, dateSrc, ofx (TimeSpan), aga (false)
                                // ok: nb database insertions
                                //Logger.Log($"PerteEtatCapteurDisp: TypeDefaut: {typeDefaut}, Défaut: {defaut}, Voie: {noVoie} - {nomVoie}, DateSrc: {dateSrc}, aga: {false}");
                                //ok = Repository.PerteEtatCaptDisp(Station, typeDefaut, defaut, noVoie, nomVoie, DateSrc, ofx, aga: false);

                                // new version
                                ok = await Repository.PerteEtatCaptDisp(Station, "ETAT", null, ve, DateSrc, ofx, aga: false);
                            }
                        }
                        break;
                    case 20: // Voie interne
                        {
                            noVoie++;
                            var vi = (noVoie < Station.VInt!.Count) ? Station.VInt[noVoie] : null;
                            nomVoie = (noVoie < Station.VInt!.Count) ? Station.VInt[noVoie].Libel : "";
                            if (!string.IsNullOrEmpty(nomVoie))
                            {
                                // TODO Update PertesEnrg: typeDefaut, defaut, noVoie, nomVoie, DateSrc, ofx (TimeSpan), aga (Station.VInt[noVoie].aga)
                                //Logger.Log($"PerteEtatCapteurDisp: TypeDefaut: {typeDefaut}, Défaut: {defaut}, Voie: {noVoie} - {nomVoie}, DateSrc: {dateSrc}, aga: {Station.VInt[noVoie].Aga}");
                                //ok = Repository.PerteEtatCaptDisp(Station, typeDefaut, defaut, noVoie, nomVoie, DateSrc, ofx, Station.VInt[noVoie].Aga);
                                ok = await Repository.PerteEtatCaptDisp(Station, "CAPTEUR", vi, null, DateSrc, ofx, vi.Aga);
                            }
                        }
                        break;
                    default:
                        break;
                } // switch (defaut)
                l++;
            }
            return true;
        }

    }
}

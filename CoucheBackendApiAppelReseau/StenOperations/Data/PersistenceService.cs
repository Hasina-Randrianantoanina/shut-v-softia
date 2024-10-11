

using Dapper;
using StenOperations.Helpers;
using StenOperations.Logger;
using StenOperations.Models.Entities;
using SHUT.Core.Data;
using System.Data;
using System.Text;
using System.Data.Common;
using StenOperations.Utils;
using static System.Formats.Asn1.AsnWriter;

// Codes Erreur
// 100 - 199 => PING
// 100 - 109 => format
// 110 - 119 => serveur
// 120 - 129 => routeur
// 200 - 299 => TCP
// 300 - 399 => FTP
// 300 - 319 Download
// 320 - 399 Upload
// 400 - 499 => ECRITURE_BINAIRE
// 500 - 599 => LECTURE_BINAIRE
// 600 - 699 => LECTURE_STATUS_CONFIG
// 700 - 799 => LECTURE XDQ
// 800 - 899 => DATABASE

namespace StenOperations.Data
{
    public class PersistenceService
    {
        public ILogger Logger { get; set; }

        private AppDbContext AppDbContext { get; set; }
        private ArchiveDbContext ArchiveDbContext { get; set; }

        private readonly Dictionary<string, int> Etats_Modules = new()
        {
            { "Etat_2", 1 },
            { "Etat_3", 2 },
            { "Etat_7", 9 },
            { "Etat_12", 10 },
            { "Etat_13", 3 },
            { "Etat_14", 4 },
        };

        public PersistenceService(ILogger logger, AppDbContext appContext, ArchiveDbContext archiveDbContext)
        {
            Logger = logger ?? throw new ArgumentNullException($"Logger not set to a valid ILogger object");
            AppDbContext = appContext;
            ArchiveDbContext = archiveDbContext;
        }

        /// <summary>
        /// Retourne les informations (générale, voies internes, voies enregistréés, voies etats) de la station
        /// </summary>
        /// <param name="initiale">Initiale de la station</param>
        /// <returns></returns>
        public async Task<Station> GetStation(string initiale)
        {
            Logger.Log("-> GetStation()");
            // Read station information
            var req = $@"
                SELECT 
                s.id AS Id,
                s.initiales AS Initiales, 
                s.bassin_versant AS BassinVersant,
                (e.dernier_transfert AT TIME ZONE 'UTC' AT TIME ZONE 'Europe/Paris') AS DateDerTrfBase,
                (e.dernier_transfert AT TIME ZONE 'UTC' AT TIME ZONE 'Europe/Paris') AS DateDerTrfBase,
                e.adresse_ip AS FtpAdresse
                FROM reseau.stations s
                INNER JOIN reseau.enregistreurs e 
                ON s.enregistreur_id = e.id
                WHERE s.initiales='{initiale}'
                ";
            var connexion = AppDbContext.CreateConnection();
            var stations = await connexion.QueryAsync<Station>(req);

            var station = stations.Count() > 0 ? stations.First() : new Station(initiale);

            var sb = new StringBuilder();
            sb.AppendLine($"Initiale:{station.Initiales}");
            sb.AppendLine($"IP: {station.FtpAdresse}, Username: '{station.FtpUserName}', Password: '{station.FtpPassword}'");
            sb.AppendLine($"DateDerTrfBase: '{station.DateDerTrfBase.ToString("dd/MM/yyyy HH:mm:ss")}', Kind: {station.DateDerTrfBase.Kind}, ToLocalTime(): '{station.DateDerTrfBase.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss")}'");

            // Read Voies internes
            sb.Append("VInt= ");
            req = $@"
                SELECT 
                vt.id AS Id,    
                vt.numero AS Num, 
                vt.libelle AS Libel, 
                0 AS aga, 
                vt.voie_enregistree AS ve, 
                vt.virgule
                FROM reseau.voies_telemesurees vt
                INNER JOIN reseau.stations s
                ON s.id = vt.station_id
                WHERE (s.initiales='{initiale}') ORDER BY vt.numero;
                ";
            var vis = await connexion.QueryAsync<VoiesAnalogiques>(req);
            for (var i = 0; i < station.VInt!.Count; i++)
            {
                var vi = vis.Where(v => v.Num == i).FirstOrDefault();
                if (vi != null)
                {
                    sb.Append($"{vi.Num}:{vi.Libel.Trim()}:{vi.Aga}:{vi.Ve}:{vi.Virgule} ");
                    station.VInt[i] = vi;
                    station.n_vint_u++;
                }
            }
            sb.AppendLine();
            sb.AppendLine($"v_int_u:{station.n_vint_u}");

            // Read Voies enregistrées
            sb.Append("VEnrg= ");
            req = $@"
                SELECT vt.numero AS Num, 
                vt.voie_enregistree AS ve, 
                vt.libelle AS Libel, 
                vt.virgule AS Virgule
                FROM reseau.voies_telemesurees vt
                INNER JOIN reseau.stations s
                ON s.id = vt.station_id
                WHERE (s.initiales='{initiale}' AND vt.voie_enregistree>0) 
                ORDER BY vt.voie_enregistree;
                ";
            var ves = await connexion.QueryAsync<VoiesAnalogiques>(req);
            for (var i = 0; i < station.VEnrg!.Count; i++)
            {
                var ve = ves.Where(v => v.Ve == i).FirstOrDefault();
                if (ve != null)
                {
                    sb.Append($"{ve.Num}:{ve.Ve}:{ve.Libel.Trim()}:{ve.Virgule} ");
                    station.n_enrg++;
                    station.VEnrg[i] = ve;
                }
            }
            sb.AppendLine();
            sb.AppendLine($"n_enrg:{station.n_enrg}");

            // Read Voies Etat
            sb.Append("VEtat= ");
            var ini = "ASnet.ini";

            req = $@"
                SELECT
                vt.id AS Id,
                vt.numero AS ETnum, 
                vt.ordre AS ETordre, 
                vt.libelle AS ETlibel 
                FROM reseau.voies_tor vt
                INNER JOIN reseau.stations s
                ON s.id = vt.station_id
                WHERE ((s.initiales='{initiale}') AND (vt.type='ETAT') AND (vt.ordre>0)) 
                ORDER BY vt.ordre
                ";
            var vetat = await connexion.QueryAsync<VoiesEtat>(req);
            if (vetat.Count() <= station.VEtat!.Count)
            {
                for (var i = 0; i < station.VEtat.Count; i++)
                {
                    station.VEtat[i].ETnum = 0;
                    station.VEtat[i].ETlibel = "???";
                    var vee = vetat.Where(v => v.ETordre == i).FirstOrDefault();
                    if (vee != null)
                    {
                        // TODO: to add from INI file
                        string Xstr = "Etat_" + vee.ETnum.ToString("0");

                        string resXstr = Etats_Modules.ContainsKey(Xstr) ? Etats_Modules[Xstr].ToString() : "";

                        //int.TryParse(Xstr, out var xval);
                        vee.ETmodule = resXstr;

                        station.VEtat[i] = vee;
                        station.n_et_u++;
                        sb.Append($"{vee.ETnum}:{vee.ETordre}:{vee.ETlibel?.Trim()} ");
                    }
                }
                sb.AppendLine();
                sb.AppendLine($"n_et_u:{station.n_et_u}");
            }

            // Voies Tor
            sb.Append("VTor= ");

            req = $@"
                SELECT
                vt.id AS Id,
                vt.numero AS Numero, 
                vt.ordre AS Ordre, 
                vt.libelle AS Libelle,
                vt.type AS TypeTor
                FROM reseau.voies_tor vt
                INNER JOIN reseau.stations s
                ON s.id = vt.station_id
                WHERE ((s.initiales='{initiale}') AND (vt.type='TS' OR vt.type='TC')) 
                ORDER BY vt.ordre
                ";
            var vestor = await connexion.QueryAsync<VoiesTor>(req);
            station.VTor.Clear();
            foreach (var es in vestor)
            {
                station.VTor.Add(es);
                sb.Append($"{es.Numero}:{es.Ordre}:{es.Libelle}:{es.TypeTor} ");
            }

            Logger.Log("{0}{1}", station.Initiales, new string('-', 40));
            Logger.Log(sb.ToString());
            Logger.Log("{0}{1}", station.Initiales, new string('-', 40));

            return station;
        }

        
        public async Task<bool> GestionAlerteTaches(Station sta)
        {
            Logger.Log("-> GestionAlerteTaches()");

            var connexion = AppDbContext.CreateConnection();
            var typeAlerte = 12; // Action en cours
            var parametreAlerte = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            // Recherche si Alerte existe
            var sql = $@"
                SELECT 
                a.Id,    
                s.initiales AS Debug_Initiales, 
                a.acquitter AS Acquitter,
                a.commentaire AS Commentaire,
                a.date_alerte AS DateAlerte,
                a.description_alerte AS DescriptionAlerte,
                a.parametre_action AS ParametreAction,
                a.parametre_alerte AS ParametreAlerte,
                a.station_id AS StationId,
                a.utilisateur_id AS Utilisateur_Id,
                a.old_numero AS Old_Numero,
                a.debug_initiales AS Debug_Initiales
                FROM defaut.alertes a
                INNER JOIN reseau.stations s
                ON a.station_id = s.id
                WHERE (s.initiales='{sta.Initiales}') 
                AND (type={typeAlerte});
                ";
            var result = await connexion.QueryAsync<Alerte>(sql);
            //var dtAlertes = Tools.Get(sql, "TmpAlertes");
            var exists = result != null && result.Count() > 0;

            // Traitement
            // A ce stade de l'opération:
            // Si la property Taches n'est pas vide, Taches a la forme suivant:
            // Etat Taches : T02=5 T03=5 T04=5 T05=5 T06=5 T13=5 T16=5 T17=5
            //               T36=5 T37=5 T39=5 T40=7 T41=7 T42=7 T43=7 T63=5
            //
            var tachesHS = "";
            var etats = sta.Taches?.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i <= 14; i++)
            {
                // ne pas traiter la tache 63-place 15         -> i va de 0 à 14
                // ne pas traiter la tache 41-place 12 sten ip -> MDF 08/04/10
                if (i == 12) { i++; }
                if (etats == null || i >= etats.Length)
                {
                    break;
                }
                var etcode = etats[i].Substring(etats[i].Length - 1);
                var etval = etats[i].Substring(0, Math.Min(3, etats[i].Length));
                switch (etcode)
                {
                    case "0":
                        // EtatT = " HS "
                        tachesHS += $",{etval}";
                        break;
                    case "1":
                    case "2":
                    case "3":
                        // EtatT = " HS (inhibée) "
                        tachesHS += $",{etval}";
                        break;
                    case "8":
                    case "9":
                    case "A":
                    case "B":
                    case "C":
                    case "D":
                    case "E":
                    case "F":
                        // EtatT = " HS (non créée) "
                        tachesHS += $",{etval}";
                        break;
                    case "4":
                        // EtatT = " OK (suspendue et endormie) "
                        tachesHS += $",{etval}";
                        break;
                    case "5":
                        // EtatT = " OK (en service et endormie) "
                        break;
                    case "6":
                        // EtatT = " OK (suspendue et réveillée) "
                        tachesHS += $",{etval}";
                        break;
                    case "7":
                        // EtatT = " OK (en service et réveillée) "
                        break;
                    default:
                        break;
                }
            } // for (var i = 0; i <= 14; i++) 

            if (!string.IsNullOrEmpty(tachesHS))
            {
                var texteAlerte = $"Alerte tache(s) HS : {tachesHS.Substring(1)}";
                if (!exists)
                {
                    sql = $@"
                        INSERT INTO defaut.alertes 
                        (date_alerte, station_id, description_alerte, 
                        type, parametre_alerte, parametre_action, acquitter)
                        VALUES
                        ('{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}', {sta.Id}, '{texteAlerte}', 
                        {typeAlerte}, '{parametreAlerte}', '0', false)
                    "; // Parametre_alerte, parametre_action=0, acquitter = false
                }
                else
                {
                    sql = $@"
                        UPDATE defaut.alertes SET 
                        parametre_alerte='{parametreAlerte}',
                        description_alerte='{texteAlerte}' 
                        WHERE station_id='{sta.Id}'
                        AND type={typeAlerte};
                    ";
                }
                // TODO: to decomment on real test or prod
                await connexion.ExecuteAsync(sql);
            }
            else
            {
                if (exists)
                {
                    sql = $@"
                        DELETE FROM defaut.alertes
                        WHERE station_id='{sta.Id}'
                        AND type={typeAlerte};
                    ";
                    // TODO: to decomment on real test or prod
                    await connexion.ExecuteAsync(sql);
                }
            }
            return true;
        }

        public async Task<bool> DefautsCapteursEnLigne(Station sta)
        {
            Logger.Log("-> DefautsCapteursEnLigne()");

            var connexion = AppDbContext.CreateConnection();

            //var sql = "SELECT Defauts.* FROM Defauts " +
            //    $"WHERE ((Initiales='{sta.Initiales}') AND (NumDefaut=26));";
            var sql = $@"
                SELECT da.* FROM defaut.defauts_actifs da
                WHERE da.station_id={sta.Id} AND da.type='CAPTEUR';
            ";

            //var defauts = Tools.Get(sql, "DefautsCapteurs");
            var defautsPg = await connexion.QueryAsync<DefautActif>(sql);

            // Delete Defauts
            //sql = "DELETE Defauts.* FROM Defauts " +
            //    $"WHERE ((Initiales='{sta.Initiales}') AND (NumDefaut=26));";
            sql = $@"
                DELETE FROM defaut.defauts_actifs da
                WHERE da.station_id={sta.Id} AND da.type='CAPTEUR';
            ";
            // TODO: to decomment on real test or prod
            await connexion.ExecuteAsync(sql);

            //// Select PertesEnrg
            //sql = "SELECT voie, Numdéf FROM PertesEnrg " +
            //    $"WHERE (Initiales='{sta.Initiales}') AND ([Type]=20) AND (validation=NO);";
            //var pertes = Tools.Get(sql, "LesPertes");

            var etatCapteur = false;
            var defCapteur = false;
            for (var i = 1; i <= 32; i++)
            {
                etatCapteur = sta.VInt![i].Defaut == 1;

                if (etatCapteur)
                {
                    var tx = sta.VInt[i].Libel;
                    var idx0 = tx.IndexOf((char)9);
                    var idx1 = tx.IndexOf((char)13);
                    var idx2 = tx.IndexOf((char)0);
                    if (idx0 != -1 || idx1 != -1 || idx2 != -1)
                    {
                        tx = $"voie interne : {i}";
                    }

                    var dateDebutDefaut = DateTime.Now;
                    var commentaire = tx;
                    //var selected = defauts?.Select($"voie_telemesuree_id={sta.VInt[i].Id}");
                    var selected = defautsPg.Where(x=> x.Voie_Telemesuree_Id == sta.VInt[i].Id).ToArray();
                    if (selected != null && selected.Count() > 0)
                    {
                        dateDebutDefaut = selected[0].Appel;                        
                        commentaire = selected[0].Description_defaut;
                    }
                    defCapteur = true;

                    // Insertion de la voie en question
                    //var sql = "INSERT INTO Defauts(Initiales, NumDefaut, Defaut, DateHeure, Comment) VALUES " +
                    //    $"('{sta.Initiales}', 26, '{tx}', '{dateDebutDefaut.ToString("MM/dd/yy HH:mm:ss")}', '{commentaire}');";
                    sql = $@"
                        INSERT INTO defaut.defauts_actifs 
                        (station_id, type, voie_telemesuree_id, appel, description_defaut)
                        VALUES
                        ({sta.Id}, 'CAPTEUR', {sta.VInt[i].Id}, '{dateDebutDefaut.ToString("dd/MM/yyyy HH:mm:ss")}', '{commentaire}');
                    ";

                    // TODO: to decomment on real test or prod

                    var ins = await connexion.ExecuteAsync(sql);
                }
            } // for (var i = 1; i <= 32; i++)

            if (defCapteur)
            {
                sta.CrCapteur = 1;
                sta.DefautCapteur = 1;
            }
            return true;
        }
        //List<DefautActif> defautsEtats = new();
        public record DefautEtat(int NumDef, DateTime Debut, DateTime Appel, int StationId,string Initiales, int VoieTorId,  int VoieTorNum, string VoieTorLibelle, int VoieTorOrdre);
        public async Task<bool> DefautsEtatsEnLigne(Station sta)
        {
            Logger.Log("-> DefautsEtatsEnLigne()");

            var connexion = AppDbContext.CreateConnection();
            //var sql = "SELECT Defauts.* FROM Defauts " +
            //    $"WHERE ((Initiales='{sta.Initiales}') AND (NumDefaut=26));";
            var sql = $@"
                SELECT da.* FROM defaut.defauts_actifs da
                WHERE da.station_id={sta.Id} AND da.type='ETAT';
            ";
            var result = await connexion.QueryAsync<DefautActif>(sql);
            var defautsEtats = result.ToArray();
            //var defauts = Tools.Get(sql, "DefautsEtats");

            //sql = "DELETE Defauts.* FROM Defauts " +
            //    $"WHERE ((Initiales='{sta.Initiales}') AND (NumDefaut=27));";
            sql = $@"
                DELETE FROM defaut.defauts_actifs da
                WHERE da.station_id={sta.Id} AND da.type='ETAT';
            ";
            // TODO: to decomment on real test or prod
            var deleted = await connexion.ExecuteAsync(sql);

            // TODO: impossible à mettre à jour car on n'a pas d'info sur la voie en question
            //sql = "SELECT voie, Numdéf FROM PertesEnrg " +
            //    $"WHERE (Initiales='{sta.Initiales}') AND ([Type]=10) AND (validation=NO);";
            sql = $@"
                SELECT d.id AS NumDef, d.debut AS Debut, d.appel AS Appel, 
                s.id AS StationId, s.initiales AS Initiales, d.voie_tor_id AS VoieTorId,
                vt.numero AS VoieTorNum, vt.libelle AS VoieTorLibelle, vt.ordre AS VoieTorOrdre
                FROM defaut.defauts d
                INNER JOIN reseau.voies_tor vt ON vt.id = d.voie_tor_id
                INNER JOIN reseau.stations s ON vt.station_id = s.id
                WHERE s.initiales = '{sta.Initiales}' AND d.type='ETAT'
                ORDER BY d.debut DESC;
            ";
            var resultPertes = await connexion.QueryAsync<DefautEtat>(sql);
            var dtPertes = resultPertes.ToList();

            sta.DefautEtat = false;
            var defEtat = false;
            for (var i = 1; i <= 16; i++)
            {
                if (sta.VEtat![i].ETnum == 0)
                    break;

                var m = sta.VEtat[i].ETmodule;
                int.TryParse(m, out var nvoie);
                if (nvoie != 0)
                {
                    // TODO: Voir où ETetat est mis à jour, comment mettre à jour cette valeur
                    // TODO: Bien vérifier en test que 
                    TraiteDefVoieEC(dtPertes, sta, "ETAT", nvoie, sta.VEtat[i].ETlibel, sta.VEtat[i].ETetat, sta.DateDerTrfSta, false);
                }
                if (sta.VEtat[i].ETetat != 0)
                {
                    var tx = sta.VEtat[i].ETlibel ?? "";
                    var idx0 = tx.IndexOf((char)9);
                    var idx1 = tx.IndexOf((char)13);
                    var idx2 = tx.IndexOf((char)0);
                    if (idx0 != -1 && idx1 != -1 && idx2 != -1)
                    {
                        tx = $"voie etat : {sta.VEtat[i].ETnum}";
                    }

                    defEtat = true;
                    var dateDebutDefaut = DateTime.Now;
                    var selected = defautsEtats.Where(x => x.Voie_Tor_Id==sta.VEtat[i].Id).FirstOrDefault();
                    if (selected != null )
                    {
                        
                            dateDebutDefaut = selected.Appel;
                        
                    }

                    //sql = "INSERT INTO Defauts(Initiales, NumDefaut, Defaut, DateHeure) " +
                    //    $"VALUES ('{sta.Initiales}', 27, '{tx}', #{dateDebutDefaut.ToString("MM/dd/yy HH:mm:ss")}#);";
                    sql = $@"
                        INSERT INTO defaut.defauts_actifs 
                        (station_id, type, voie_tor_id, appel, description_defaut)
                        VALUES
                        ({sta.Id}, 'ETAT', {sta.VEtat[i].Id}, '{dateDebutDefaut.ToString("dd/MM/yyyy HH:mm:ss")}', '{tx}');
                    ";
                    // TODO: to decomment on real test or prod
                    var ins = await connexion.ExecuteAsync(sql);
                }
            } // for (var i = 1; i <= 16; i++) 

            if (defEtat)
            {
                sta.CrEtat = 1;
                sta.DefautEtat = true;
            }
            return true;
        }

        private async Task<bool> TraiteDefVoieEC(List<DefautEtat> dtPertes, Station sta, string typeDef, int nvoie, string? nomVoie, int etat, DateTime dateDef, bool aga)
        {
            Logger.Log("-> TraiteDefVoieEC()");
            var connexion = AppDbContext.CreateConnection();
            var perte = dtPertes.Where(x => x.VoieTorNum ==nvoie).FirstOrDefault();
            var numDef = 0;
            if (perte != null)
            {
                int.TryParse(perte.NumDef.ToString(), out numDef);
            }
            if (numDef == 0 && perte != null)
            {
                if (etat != 0)
                {
                    //var sql = "INSERT INTO PertesEnrg " +
                    //    "(DateDéfaut, Initiales, Type, Voie, NomVoie, Début, Fin, " +
                    //    "Défaut, Cause, Remède, VersionEnrg, EtatdesTaches, aga) " +
                    //    $"SELECT #{dateDef.ToString("MM/dd/yyyy HH:mm")}#, '{sta.Initiales}', " +
                    //    $"{typeDef}, {nvoie}, '{nomVoie}', " +
                    //    $"#{dateDef.ToString("MM/dd/yyyy HH:mm")}#, '{DateTime.Now.ToString("MM/dd/yyyy HH:mm")}', '?', '?', '?', " +
                    //    $"'{sta.VersionEnrg}', '{sta.Taches}', {aga};";
                    var sql = $@"
                        INSERT INTO defaut.defauts
                        (appel, type, voie_tor_id, old_libelle_voie, debut, fin)
                        VALUES 
                        (
                            '{dateDef.ToString("MM/dd/yyyy HH:mm:ss")}',
                            'ETAT',
                            {perte.VoieTorId.ToString()},
                            '{perte.VoieTorLibelle}',
                            '{dateDef.ToString("MM/dd/yyyy HH:mm:ss")}',
                            '{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}'
                        )
                    ";
                    // TODO: To decomment on real test or prod
                    await connexion.ExecuteAsync(sql);
                    //Tools.Execute(sql);
                }
            }
            else
            {
                if (etat != 0)
                {
                    sta.Perte_Fin = DateTime.Now;
                    sta.Perte_val = false;
                }
                else
                {
                    sta.Perte_Fin = dateDef;
                    sta.Perte_val = true;
                }
                //var sql = $"UPDATE PertesEnrg SET Fin=#{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}#, Validation={sta.Perte_val}" +
                //    $" WHERE (NumDéf={numDef})";
                var sql = $@"
                    UPDATE defaut.defauts
                    SET fin='{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}'
                    WHERE id={numDef};
                ";
                // TODO: to decomment on real test or prod
                await connexion.ExecuteAsync(sql);
                //Tools.Execute(sql);
            }
            return true;
        }

        public async Task<int> PertesEnrg(Station sta, bool enrg)
        {
            Logger.Log("-> PertesEnrg()");
            var connexion = AppDbContext.CreateConnection();
            sta.Perte_type = 0;
            sta.Perte_Defaut = "?";
            sta.Perte_Cause = "?";
            sta.Perte_Remede = "?";

            var ecart1 = new TimeSpan(0, 3, 0);
            if (sta.DateInit.ToOADate() == 0 ||
                sta.DateInit.Subtract(ecart1) > sta.DateDerTrfBase)
            {
                if (sta.DateStop > sta.DateDerTrfBase &&
                    sta.DateGo.Subtract(ecart1) > sta.DateDerTrfBase)
                {
                    // Pb dateGo & dateInit
                    sta.Perte_type = 7;
                    sta.Perte_Deb = sta.DateDerTrfBase;
                    sta.Perte_Fin = sta.DateInit;
                }
                else
                {
                    // Pb dateInit
                    sta.Perte_type = 2;
                    sta.Perte_Deb = sta.DateDerTrfBase;
                    sta.Perte_Fin = sta.DateInit;
                }
            }
            else if (sta.DateGo.ToOADate() == 0 ||
                sta.DateGo.Subtract(ecart1) > sta.DateDerTrfBase)
            {
                // Pb dateGo
                sta.Perte_type = 1;
                sta.Perte_Deb = sta.DateStop;
                sta.Perte_Fin = sta.DateGo;
            }
            else
            {
                var ecart2 = DateTime.Now.ToOADate() - sta.EcartHorloges - sta.DateAcq.ToOADate();
                var time = new DateTime(1, 1, 1, 0, 1, 2);
                if (sta.DateAcq.ToOADate() == 0 ||
                    ecart2 > time.ToOADate())
                {
                    // Pb dateAcq
                    sta.Perte_type = 3;
                    sta.Perte_Deb = sta.DateEnrg;
                    sta.Perte_Fin = DateTime.FromOADate(0);
                }
                else
                {
                    ecart2 = DateTime.Now.ToOADate() - sta.EcartHorloges - sta.DateEnrg.ToOADate();
                    time = new DateTime(1, 1, 1, 0, 3 * sta.CadEnrg, 0);
                    if (sta.DateEnrg.ToOADate() == 0 ||
                        ecart2 > time.ToOADate())
                    {
                        // pb dateEnrg
                        sta.Perte_type = 4;
                        sta.Perte_Deb = sta.DateEnrg;
                        sta.Perte_Fin = DateTime.FromOADate(0);
                    }
                    else
                    {
                        ecart1 = new TimeSpan(0, 10, 0);
                        time = new DateTime(1, 1, 1, 0, 10, 0);
                        if (Math.Abs(sta.EcartHorloges) > time.ToOADate())
                        {
                            // pb ecart Horloge
                            sta.Perte_type = 5;
                            sta.Perte_Deb = sta.DateEnrg;
                            sta.Perte_Fin = DateTime.FromOADate(0);
                        }
                    }
                }
            }

            if (enrg && sta.Perte_type != 0)
            {
                var tmpH = DateTime.Now.ToOADate() - sta.EcartHorloges;
                var tmpDate = DateTime.FromOADate(tmpH);

                //var sql = "SELECT NomVoie, Début, Fin FROM EvtsPluvieux " +
                //    $"WHERE ((Début<=#{sta.Perte_Deb?.ToString("MM/dd/yyyy HH:mm:ss")}#) " +
                //    $"AND (Fin >=#{sta.Perte_Deb?.ToString("MM/dd/yyyy HH:mm:ss")}#) " +
                //    $"AND (NomVoie='EVT@PLU&HYD_{sta.BassinVersant}'));";

                //var dtEvtsPluvieux = Tools.Get(sql, "EvtsPluvieux");
                //sta.Evt_encours = "Non";
                //if (dtEvtsPluvieux != null && dtEvtsPluvieux.Rows.Count > 0)
                //{
                //    sta.Evt_encours = "Oui";
                //}

                if (sta.Evt_encours == "Oui" && sta.METEO == "temps sec")
                {
                    sta.Critique = "Non";
                }
                else if (sta.Evt_encours == "Non" && sta.METEO == "temps de pluie")
                {
                    sta.Critique = "Non";
                }
                else
                {
                    sta.Critique = "Oui";
                }
                if (sta.Evt_encours == "Inv")
                {
                    sta.Critique = "?";
                }

                //sql = "INSERT INTO PertesEnrg(" +
                //    "DateDéfaut, Initiales, Type, Voie, HorlCible, DiffHorl, " +
                //    "derTrf, [Go], init, [Stop], acq, Enrg, Début, Fin, " +
                //    "Défaut, Cause, Remède, VersionEnrg, EtatdesTaches, " +
                //    "Evt_encours, Meteo, critique) " +
                //    $"SELECT #{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}#, '{sta.Initiales}', {sta.Perte_type}, 0, " +
                //    $"#{tmpDate.ToString("MM/dd/yyyy HH:mm:ss")}#, " +
                //    $"{sta.EcartHorloges.ToString().Replace(",", ".")}, " +
                //    $"#{sta.DateDerTrfBase.ToString("MM/dd/yyyy HH:mm:ss")}#, " +
                //    $"#{sta.DateGo.ToString("MM/dd/yyyy HH:mm:ss")}#, " +
                //    $"#{sta.DateInit.ToString("MM/dd/yyyy HH:mm:ss")}#, " +
                //    $"#{sta.DateStop.ToString("MM/dd/yyyy HH:mm:ss")}#, " +
                //    $"#{sta.DateAcq.ToString("MM/dd/yyyy HH:mm:ss")}#, " +
                //    $"#{sta.DateEnrg.ToString("MM/dd/yyyy HH:mm:ss")}#, " +
                //    $"#{sta.Perte_Deb?.ToString("MM/dd/yyyy HH:mm:ss")}#, " +
                //    $"#{sta.Perte_Fin?.ToString("MM/dd/yyyy HH:mm:ss")}#, " +
                //    $"'?', '?', '?', '{sta.VersionEnrg}', " +
                //    $"'{sta.Taches}', " +
                //    $"'{sta.Evt_encours}', " +
                //    $"'{sta.METEO}', " +
                //    $"'{sta.Critique}';";
                var critique = sta.Critique == "Oui" ? true : false;

                // INSERT INTO defaut.pertes
                // (
                // appel, station_id, [PerteType], [HorlCible], diff_horloge,
                // [derTrf], date_go, date_init, [stop], date_acquisition,
                // date_enregistrement, debut, fin,
                // defaut, cause, remede, [VersionEnrg], etat_des_taches,
                // [Evt_encours], [meteo], critique
                // )
                var sql = $@"
                    INSERT INTO defaut.pertes
                    (
                    appel, station_id, diff_horloge,
                    date_go, date_init, date_acquisition,
                    date_enregistrement, debut, fin,
                    defaut, cause, remede, 
                    etat_des_taches, critique
                    )
                    VALUES
                    (
                    '{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}', 
                    {sta.Id}, 
                    {sta.EcartHorloges.ToString().Replace(",", ".")},
                    '{sta.DateGo.ToString("dd/MM/yyyy HH:mm:ss")}',
                    '{sta.DateInit.ToString("dd/MM/yyyy HH:mm:ss")}',
                    '{sta.DateAcq.ToString("dd/MM/yyyy HH:mm:ss")}',
                    '{sta.DateEnrg.ToString("dd/MM/yyyy HH:mm:ss")}',
                    '{sta.Perte_Deb?.ToString("dd/MM/yyyy HH:mm:ss")}',
                    '{sta.Perte_Fin?.ToString("dd/MM/yyyy HH:mm:ss")}',
                    '?', '?', '?',
                    '{sta.Taches}',
                    {critique}
                    )
                ";
                // TODO: to decomment on real test or prod
                await connexion.ExecuteAsync(sql);
                //Tools.Execute(sql);
            }
            return sta.Perte_type;
        }

        public async Task<int> InsDefaut(Station sta, int errCode, string errText)
        {
            Logger.Log("-> InsDefaut()");
            var connexion = AppDbContext.CreateConnection();
            var err = errText.Substring(0, errText.Length < 75 ? errText.Length : 75);
            err = err.Replace("'", "?").Replace('"', '?');
            var code = errCode == 58 ? "PERTE" : errCode.ToString();
            //var sql = "INSERT INTO Defauts(DateHeure, Initiales, NumDefaut, Defaut) " +
            //    $"SELECT '{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}', '{sta.Initiales}', {errCode}, '{err}';";
            var sql = $@"
                INSERT INTO defaut.defauts_actifs
                (appel, station_id, type, description_defaut)
                VALUES
                ('{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}', {sta.Id}, 'PERTE', '{err}');
            ";
            // TODO: to decomment on real test or prod
            return await connexion.ExecuteAsync(sql);
            //return Tools.Execute(sql);
        }

        List<Defaut> defautLoadPertesEnreg = new();
        public async Task<bool> LoadPertesEnrg(Station sta, DateTime da)
        {
            Logger.Log("-> LoadPertesEnrg()");
            var connexion = AppDbContext.CreateConnection();
            //var sql = "SELECT * FROM PertesEnrg " +
            //    $"WHERE ((Initiales='{sta.Initiales}') " +
            //    "AND ([Type]=10 OR [Type]=20) " +
            //    "AND ((validation=NO) OR " +
            //    $"((validation=YES) AND (Début >= #{da.ToString("MM/dd/yyyy HH:mm:ss")}#)))) " +
            //    "ORDER BY Début";
            //Tools.Get(sql, "Table");

            var voie_telemesuree_ids = sta.VInt!.Select(v => v.Id.ToString())
                .Distinct()
                .ToList();
            var voie_tor_ids = sta.VEtat!.Select(v => v.Id.ToString())
                .Distinct()
                .ToList();

            var where_voie = "";
            if (voie_telemesuree_ids.Count > 0)
            {
                where_voie = $"voie_telemesuree_id IN ({string.Join(",", voie_telemesuree_ids)}) ";
            }
            if (voie_tor_ids.Count > 0)
            {
                if (!string.IsNullOrEmpty(where_voie))
                {
                    where_voie += " OR ";
                }
                where_voie += $" voie_tor_id IN ({string.Join(",", voie_tor_ids)})";
            }

            var sql = $@"
                SELECT * FROM defaut.defauts
                WHERE actif = TRUE AND
                (
                    {where_voie}
                ) AND
                debut >= '{da.ToString("MM/dd/yyyy HH:mm:ss")}'
                ORDER BY debut;
            ";
            var result = await connexion.QueryAsync<Defaut>(sql);
            defautLoadPertesEnreg = result.ToList();
            //Tools.Get(sql, "Table");
            return true;
        }

        public async Task<bool> LoadPertesEnrgNoDate(Station sta, DateTime da)
        {
            Logger.Log("-> LoadPertesEnrgNoDate()");
            var connexion = AppDbContext.CreateConnection();
            //var sql = "SELECT * FROM PertesEnrg " +
            //    $"WHERE ((Initiales='{sta.Initiales}') " +
            //    "AND ([Type]=10 OR [Type]=20) " +
            //    "AND (validation=NO)) " +
            //    "ORDER BY Début";

            var voie_telemesuree_ids = sta.VInt!.Select(v => v.Id.ToString())
                .Distinct()
                .ToList();
            var voie_tor_ids = sta.VEtat!.Select(v => v.Id.ToString())
                .Distinct()
                .ToList();

            var where_voie = "";
            if (voie_telemesuree_ids.Count > 0)
            {
                where_voie = $"voie_telemesuree_id IN ({string.Join(",", voie_telemesuree_ids)}) ";
            }
            if (voie_tor_ids.Count > 0)
            {
                if (!string.IsNullOrEmpty(where_voie))
                {
                    where_voie += " OR ";
                }
                where_voie += $" voie_tor_id IN ({string.Join(",", voie_tor_ids)})";
            }

            // Ordonner par ordre descendante pour récupérer celle qui a la dernière date en premier
            var sql = "";
            if (string.IsNullOrEmpty(where_voie))
            {
                sql = $@"
                    SELECT * FROM defaut.defauts
                    WHERE actif = TRUE 
                    ORDER BY debut DESC;
                ";
            }
            else
            {
                sql = $@"
                    SELECT * FROM defaut.defauts
                    WHERE actif = TRUE AND
                    (
                       {where_voie}
                    ) 
                    ORDER BY debut DESC;
                ";
            }
            var result = await connexion.QueryAsync<Defaut>(sql);
            defautLoadPertesEnreg = result.ToList();
            //Tools.Get(sql, "Table");
            return true;
        }

        public int PerteEtatCaptApp(Station sta, int typeDefaut, int defaut, int noVoie, string nomVoie, DateTime da, TimeSpan ofx, bool aga)
        {
            Logger.Log("-> PerteEtatCaptApp()");
            //var dtPertes = Tools.DataSet.Tables["Table"];
            //if (dtPertes == null)
            //{
            //    return 0;
            //}
            //var dh = da.Subtract(ofx);
            //var inserer = true;
            //var valPerte = default(bool);
            //var tvs = dtPertes.Select($"Type = {typeDefaut} AND voie = {noVoie}");
            //if (tvs != null)
            //{
            //    sta.Perte_Deb = tvs[0]["Début"] as DateTime?;
            //    sta.Perte_Fin = tvs[0]["Fin"] as DateTime?;
            //    if (dh < sta.Perte_Deb) { }
            //    if (dh >= sta.Perte_Deb && dh <= sta.Perte_Fin) { inserer = false; }
            //}
            //if (inserer)
            //{
            //    sta.Perte_Deb = dh;
            //    sta.Perte_Fin = dh;
            //    sta.Perte_Defaut = "?";
            //    var sql = "INSERT INTO PertesEnrg" +
            //        "(DateDéfaut, Initiales, Type, Voie, NomVoie, " +
            //        "Début, Fin, Défaut, validation, Cause, Remède, VersionEnrg, EtatdesTaches, aga) " +
            //        "SELECT " +
            //        $"#{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}#, " +
            //        $"'{sta.Initiales}', {typeDefaut}, {noVoie}, '{nomVoie}', " +
            //        $"#{sta.Perte_Deb?.ToString("MM/dd/yyyy HH:mm:ss")}#, " +
            //        $"#{sta.Perte_Fin?.ToString("MM/dd/yyyy HH:mm:ss")}#, " +
            //        $"'{sta.Perte_Defaut}', " +
            //        $"{valPerte}, " +
            //        $"'?', '?', '{sta.VersionEnrg}', " +
            //        $"'{sta.Taches}', {aga};";
            //    var ins = Tools.Execute(sql);
            //    return (ins > 0) ? 1 : 0;
            //}
            return 0;
        }

        public async Task<int> PerteEtatCaptApp(Station sta, string typeDefaut, VoiesAnalogiques? voieInt, VoiesEtat? voieEtat, DateTime da, TimeSpan ofx, bool aga)
        {
            Logger.Log("-> PerteEtatCaptApp()");
            var dtPertes = defautLoadPertesEnreg;
            //var dtPertes = Tools.DataSet.Tables["Table"];

            if (dtPertes == null)
            {
                return 0;
            }
            var filter = "";
            int noVoie = 0;
            var type = "";
            var nomCol = "";
            Func<Defaut, bool> filterFn = null;
            switch (typeDefaut)
            {
                case "ETAT":
                    {
                        if (voieEtat == null)
                        {
                            return 0;
                        }
                        noVoie = voieEtat.Id;
                        type = "ETAT";
                        nomCol = "voie_tor_id";
                        filterFn = (d) => { return d.Type == typeDefaut && d.Voie_Tor_Id == noVoie; };
                    }
                    break;
                case "CAPTEUR":
                    {
                        if (voieInt == null)
                        {
                            return 0;
                        }
                        noVoie = voieInt.Id;
                        type = "CAPTEUR";
                        nomCol = "voie_telemesuree_id";
                        filterFn = (d) => { return d.Type == typeDefaut && d.Voie_Telemesuree_Id == noVoie; };
                    }
                    break;
                default:
                    return 0;
            }
            filter = $"type = '{type}' AND {nomCol} = {noVoie}";
            var dh = da.Subtract(ofx);
            var inserer = true;
            var valPerte = default(bool);
            var tvs = dtPertes.Where(filterFn).FirstOrDefault();
            if (tvs != null)
            {
                sta.Perte_Deb = tvs.Debut;
                sta.Perte_Fin = tvs.Fin;
                if (dh < sta.Perte_Deb) { }
                if (dh >= sta.Perte_Deb && dh <= sta.Perte_Fin) { inserer = false; }
            }
            if (inserer)
            {
                sta.Perte_Deb = dh;
                sta.Perte_Fin = dh;
                sta.Perte_Defaut = "?";
                //var sql = "INSERT INTO PertesEnrg" +
                //    "(DateDéfaut, Initiales, Type, Voie, NomVoie, " +
                //    "Début, Fin, Défaut, validation, Cause, Remède, VersionEnrg, EtatdesTaches, aga) " +
                //    "SELECT " +
                //    $"#{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}#, " +
                //    $"'{sta.Initiales}', {typeDefaut}, {noVoie}, '{nomVoie}', " +
                //    $"#{sta.Perte_Deb?.ToString("MM/dd/yyyy HH:mm:ss")}#, " +
                //    $"#{sta.Perte_Fin?.ToString("MM/dd/yyyy HH:mm:ss")}#, " +
                //    $"'{sta.Perte_Defaut}', " +
                //    $"{valPerte}, " +
                //    $"'?', '?', '{sta.VersionEnrg}', " +
                //    $"'{sta.Taches}', {aga};";
                var sql = $@"
                    INSERT INTO defaut.defauts
                    (appel, {nomCol}, type, debut, fin, commentaire, temp_validation)
                    VALUES
                    (
                        '{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}',
                        {noVoie},
                        '{type}',
                        '{sta.Perte_Deb?.ToString("MM/dd/yyyy HH:mm:ss")}',
                        '{sta.Perte_Fin?.ToString("MM/dd/yyyy HH:mm:ss")}',
                        '{sta.Perte_Defaut}',
                        {valPerte}
                    )
                ";
                var connexion = AppDbContext.CreateConnection();
                var ins = await connexion.ExecuteAsync(sql);
                return ins > 0 ? 1 : 0;
            }
            return 0;
        }

        public int PerteEtatCaptDisp(Station sta, int typeDefaut, int defaut, int noVoie, string nomVoie, DateTime da, TimeSpan ofx, bool aga)
        {
            Logger.Log("-> PerteEtatCaptDisp()");
            //var dtPertes = Tools.DataSet.Tables["Table"];
            //if (dtPertes == null)
            //{
            //    return 0;
            //}
            //var valPerte = false;
            //var dh = da.Subtract(ofx);
            //var tvs = dtPertes.Select($"Type={typeDefaut} AND voie={noVoie}");
            //if (tvs != null && tvs.Length > 0) 
            //{
            //    sta.Perte_Deb = tvs[0]["Début"] as DateTime?;
            //    sta.Perte_Fin = tvs[0]["Fin"] as DateTime?;
            //    var numDef = tvs[0]["NumDéf"] as int?;
            //    sta.Perte_Defaut = "?";
            //    var validation = tvs[0]["validation"] as bool? ?? false;
            //    if (!validation) 
            //    {
            //        valPerte = false;
            //    }
            //    if (dh < sta.Perte_Fin)
            //    {
            //        return 0;
            //    }
            //    if (dh > sta.Perte_Fin)
            //    {
            //        sta.Perte_Fin = dh;
            //        valPerte = true;
            //    }
            //    if (valPerte) 
            //    {
            //        tvs[0]["validation"] = true;
            //        var sql = "UPDATE PertesEnrg SET " +
            //            $"Début=#{sta.Perte_Deb?.ToString("MM/dd/yyyy HH:mm:ss")}#, " +
            //            $"Fin=#{sta.Perte_Fin?.ToString("MM/dd/yyyy HH:mm:ss")}#, " +
            //            $"Défaut='{sta.Perte_Defaut}', " +
            //            $"validation={valPerte} " +
            //            $"WHERE (NumDéf={numDef});";
            //        var res = Tools.Execute(sql);
            //        return (res > 0) ? 2 : 0;
            //    }
            //}

            return 0;
        }

        public async Task<int> PerteEtatCaptDisp(Station sta, string typeDefaut, VoiesAnalogiques? voieInt, VoiesEtat? voieEtat, DateTime da, TimeSpan ofx, bool aga)
        {
            Logger.Log("-> PerteEtatCaptDisp()");
            var filter = "";
            int noVoie = 0;
            var type = "";
            var nomCol = "";

            Func<Defaut, bool> filterFn = null;
            switch (typeDefaut)
            {
                case "ETAT":
                    {
                        if (voieEtat == null)
                        {
                            return 0;
                        }
                        noVoie = voieEtat.Id;
                        type = "ETAT";
                        nomCol = "voie_tor_id";
                        filterFn = (d) => { return d.Type == typeDefaut && d.Voie_Tor_Id == noVoie; };
                    }
                    break;
                case "CAPTEUR":
                    {
                        if (voieInt == null)
                        {
                            return 0;
                        }
                        noVoie = voieInt.Id;
                        type = "CAPTEUR";
                        nomCol = "voie_telemesuree_id";
                        filterFn = (d) => { return d.Type == typeDefaut && d.Voie_Telemesuree_Id == noVoie; };
                    }
                    break;
                default:
                    return 0;
            }


            filter = $"type = '{type}' AND {nomCol} = {noVoie}";

            var dtPertes = defautLoadPertesEnreg;

            if (dtPertes == null)
            {
                return 0;
            }
            var valPerte = false;
            var dh = da.Subtract(ofx);
            var tvs = dtPertes.Where(filterFn).FirstOrDefault();
            if (tvs != null)
            {
                sta.Perte_Deb = tvs.Debut;
                sta.Perte_Fin = tvs.Fin;
                var numDef = tvs.Id;
                sta.Perte_Defaut = "?";
                var validation = tvs.Debug_Validation??false;
                if (!validation)
                {
                    valPerte = false;
                }
                if (dh < sta.Perte_Fin)
                {
                    return 0;
                }
                if (dh > sta.Perte_Fin)
                {
                    sta.Perte_Fin = dh;
                    valPerte = true;
                }
                if (valPerte)
                {
                    tvs.Debug_Validation = true;
                    //var sql = "UPDATE PertesEnrg SET " +
                    //    $"Début=#{sta.Perte_Deb?.ToString("MM/dd/yyyy HH:mm:ss")}#, " +
                    //    $"Fin=#{sta.Perte_Fin?.ToString("MM/dd/yyyy HH:mm:ss")}#, " +
                    //    $"Défaut='{sta.Perte_Defaut}', " +
                    //    $"validation={valPerte} " +
                    //    $"WHERE (NumDéf={numDef});";
                    var sql = $@"
                        UPDATE defaut.defauts SET
                        debut='{sta.Perte_Deb?.ToString("MM/dd/yyyy HH:mm:ss")}',
                        fin='{sta.Perte_Fin?.ToString("MM/dd/yyyy HH:mm:ss")}',
                        debug_validation={valPerte}
                        WHERE id={numDef};
                    ";
                    var connexion = AppDbContext.CreateConnection();
                    var res = await connexion.ExecuteAsync(sql);
                    return res > 0 ? 2 : 0;
                }
            }

            return 0;
        }


        private async Task<IEnumerable<Defaut>> LoadDefauts(Station sta, DateTime? date1erDef, List<DefautVoie> defautVoies, bool orderDesc = false)
        {
            if (defautVoies.Count == 0)
            {
                return new List<Defaut>();
            }
            // Charges les defauts actifs en cours
            // Ici, on filtre uniquement avec les voies en defaut
            // pour ne pas à récupérer les données
            var defautVoiesTelemesurees = defautVoies.Where(d => d.TypeDefaut == TypeDefaut.VOIE_INTERNE)
                .Select(d => d.NumeroVoie)
                .Distinct()
                .ToList();
            var defautVoiesTors = defautVoies.Where(d => d.TypeDefaut == TypeDefaut.VOIE_ETAT)
                .Select(d => d.NumeroVoie.ToString())
                .Distinct()
                .ToList();

            var voie_telemesuree_ids = sta.VInt!.Select((v, index) => (v, index))
                .Where(vi => defautVoiesTelemesurees.Contains(vi.index))
                .Select(vi => vi.v.Id.ToString()).ToList();
            var voie_tor_ids = sta.VEtat!
                .Where(v => defautVoiesTors.Contains(v.ETmodule))
                .Select(v => v.Id.ToString())
                .ToList();

            var where_voies = string.Empty;
            if (voie_telemesuree_ids.Count > 0)
            {
                where_voies = $"voie_telemesuree_id IN ({string.Join(",", voie_telemesuree_ids)}) ";
            }
            if (voie_tor_ids.Count > 0)
            {
                where_voies = string.IsNullOrEmpty(where_voies)
                    ? $"voie_tor_id IN ({string.Join(",", voie_tor_ids)})"
                    : $"{where_voies} OR voie_tor_id IN ({string.Join(",", voie_tor_ids)})";
            }
            var where_date = "";
            if (date1erDef != null)
            {
                where_date = $" AND debut >= '{date1erDef?.ToUniversalTime().ToString("MM/dd/yyyy HH:mm:ss")}'";
            }
            var orderByDesc = "";
            if (orderDesc)
            {
                orderByDesc = "DESC";
            }
            var sql = $@"
                SELECT
				id, actif,
				(appel AT TIME ZONE 'UTC' AT TIME ZONE 'Europe/Paris') AS appel,
				commentaire,
				(debut AT TIME ZONE 'UTC' AT TIME ZONE 'Europe/Paris') AS debut,
				enregistreur_id,
				(fin AT TIME ZONE 'UTC' AT TIME ZONE 'Europe/Paris') AS fin,
				type,
				utilisateur_id,
				version_enregistreur,
				voie_telemesuree_id,
				voie_tor_id,
				old_libelle_voie,
				old_numero,
				debug_validation,
				debug_initiales
                FROM defaut.defauts
                WHERE actif = FALSE AND
                (
                    {where_voies}
                ) 
                {where_date}
                ORDER BY debut {orderByDesc};
            ";
            var connexion = AppDbContext.CreateConnection();
            var defauts = await connexion.QueryAsync<Defaut>(sql);
            return defauts;
        }


        public async Task<int> PertesEtatCaptApparitions(Station sta, DateTime date1erDef, List<DefautVoie> defautVoies)
        {
            var connexion = AppDbContext.CreateConnection();
            if (defautVoies.Count == 0)
                return 0;
            var dtPertes = await LoadDefauts(sta, date1erDef, defautVoies);

            // Regarder Apparitions / Disparitions dans DefautVoies
            var disparitions = defautVoies.Where(d => !d.IsStartDefaut)
                .ToList();

            // Faire la correspondance entre le début et la fin des défauts de la liste
            foreach (var dv in defautVoies)
            {
                if (!dv.IsStartDefaut)
                    continue;
                var end = disparitions.Where(d =>
                    d.TypeDefaut == dv.TypeDefaut &&
                    d.NumeroVoie == dv.NumeroVoie &&
                    dv.DateDebut <= d.DateFin)
                    .FirstOrDefault();
                if (end != null)
                {
                    dv.DateFin = end.DateFin;
                    end.DateDebut = dv.DateDebut;
                }
            }

            // Avec cette liste les apparitions
            var apparitions = defautVoies.Where(d => d.IsStartDefaut)
                .ToList();
            date1erDef = apparitions.Count > 0 ? apparitions[0].DateDefaut : DateTime.Now;

            // Filtre les defauts existants
            foreach (var a in apparitions)
            {
                Logger.Log($"=> Defaut: Start: '{a.DateDebut?.ToString("dd/MM/yyyy HH:mm:ss")}', End: '{a.DateFin?.ToString("dd/MM/yyyy HH:mm:ss")}', Type: {a.TypeDefaut}, Voie: {a.NumeroVoie}");
                Func<Defaut, bool> filtre = (d) => { return false; };
                var type = "";
                var voieCol = "";
                var voieId = 0;

                switch (a.TypeDefaut)
                {
                    case TypeDefaut.VOIE_INTERNE:
                        {
                            var vi = a.NumeroVoie < sta.VInt!.Count ? sta.VInt[a.NumeroVoie] : null;
                            if (vi == null)
                                continue;
                            type = "CAPTEUR";
                            filtre = (df) => { return df.Type == type && df.Voie_Telemesuree_Id == vi.Id && df.Debut == a.DateDebut; };
                            //filtre = $"type='CAPTEUR' AND voie_telemesuree_id={vi.Id}";
                            voieCol = "voie_telemesuree_id";
                            voieId = vi.Id;
                        }
                        break;
                    case TypeDefaut.VOIE_ETAT:
                        {
                            var ve = sta.VEtat!.Where(v => v.ETnum != 0 && v.ETmodule == a.NumeroVoie.ToString())
                                        .FirstOrDefault();
                            if (ve == null)
                                continue;
                            type = "ETAT";
                            filtre = (df) => { return df.Type == type && df.Voie_Tor_Id == ve.Id && df.Debut == a.DateDebut; };
                            //filtre = $"type='ETAT' AND voie_tor_id={ve.Id}";
                            voieCol = "voir_tor_id";
                            voieId = ve.Id;
                        }
                        break;
                    default:
                        break;
                }
                var das = dtPertes.Where(filtre)
                    .ToList();
                var inserer = true;
                var sql = "";
                if (das != null && das.Count() > 0)
                {
                    inserer = false;
                    // Il existe déjà un défaut actif, on met à jour la date de fin.
                    // Normalement, comme c'est actif, alors date_fin doit être forcément null,
                    // sinon, c'est une erreur d'enregistrement
                    var defaut = das[0];
                    var defautDebut = defaut.Debut;
                    var defautFin = defaut.Fin;
                    sta.Perte_Deb = defautDebut;
                    sta.Perte_Fin = defautFin;
                    int.TryParse(defaut.Id.ToString(), out var defautId);
                    // Sinon, si la date de début correspond, on met juste à jour 
                    // cette ligne de la base de données
                    var dateFin = a.DateFin == null || a.DateFin == DateTime.MinValue ? "NULL" : $"'{a.DateFin?.ToUniversalTime().ToString("dd/MM/yyyy HH:mm:ss")}'";
                    sql = $@"
                            UPDATE defaut.defauts SET
                            fin={dateFin},
                            actif={a.Actif}
                            WHERE id={defautId};
                        ";
                    await connexion.ExecuteAsync(sql);
                    //Tools.Execute(sql);
                    //Tools.Execute(sql, new { Id = defautId, Fin = a.DateFin, Actif = a.Actif });
                }
                if (inserer)
                {
                    sta.Perte_Deb = a.DateDebut;
                    sta.Perte_Fin = a.DateFin;
                    sta.Perte_Defaut = "?";
                    // Il n'y a pas de défaut actif donc on insère
                    var appel = $"'{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}'";
                    var dateDebut = a.DateDebut == null || a.DateDebut == DateTime.MinValue ? "NULL" : $"'{a.DateDebut?.ToUniversalTime().ToString("dd/MM/yyyy HH:mm:ss")}'";
                    var dateFin = a.DateFin == null || a.DateFin == DateTime.MinValue ? "NULL" : $"'{a.DateFin?.ToUniversalTime().ToString("dd/MM/yyyy HH:mm:ss")}'";
                    sql = $@"
                            INSERT INTO defaut.defauts
                            (appel, {voieCol}, debut, fin, type, actif)
                            VALUES
                            ({appel}, {voieId}, {dateDebut}, {dateFin}, '{type}', {a.Actif});
                        ";
                    await connexion.ExecuteAsync(sql);
                    //Tools.Execute(sql);
                    //Tools.Execute(sql, new { Appel = DateTime.Now, NoVoie = voieId, Debut = a.DateDebut, Fin = a.DateFin, Type = type, Actif = a.Actif });
                }

            } //foreach (var a in apparitions)

            return 0;
        }

        public async Task<int> PertesEtatCaptDisparitions(Station sta, List<DefautVoie> defautVoies)
        {
            if (defautVoies.Count == 0) { return 0; }

            // Regarder les disparitions
            // Regarder Apparitions / Disparitions dans DefautVoies
            var disparitions = defautVoies.Where(d => !d.IsStartDefaut)
                .ToList();

            // Faire la correspondance entre le début et la fin des défauts de la liste
            // et mise à jour de la liste disparitions
            foreach (var dv in defautVoies)
            {
                if (!dv.IsStartDefaut)
                    continue;
                var end = disparitions.Where(d =>
                    d.TypeDefaut == dv.TypeDefaut &&
                    d.NumeroVoie == dv.NumeroVoie &&
                    dv.DateDebut <= d.DateFin)
                    .FirstOrDefault();
                if (end != null)
                {
                    dv.DateFin = end.DateFin;
                    end.DateDebut = dv.DateDebut;
                }
            }

            // liste des disparitions sans debut
            var disparitionsWithDebut = disparitions.Where(d => d.DateDebut != null)
                .ToList();
            var disparitionsWithoutDebut = disparitions.Where(d => d.DateDebut == null)
                .ToList();
            await ProcessDefautFinWithoutDebut(sta, disparitionsWithoutDebut);
            await ProcessDefautFinWithDebut(sta, disparitionsWithDebut);

            return 0;
        }


        private async Task<int> ProcessDefautFinWithoutDebut(Station sta, List<DefautVoie> defautVoieFinSansDebut)
        {
            // On charge les défauts en ordonant par date de début de manière à
            // récupérer la dernière en date en premier
            var connexion = AppDbContext.CreateConnection();
            var dtPertes = await LoadDefauts(sta, null, defautVoieFinSansDebut, true);
            foreach (var d in defautVoieFinSansDebut)
            {
                Func<Defaut, bool> filtre = (d) => { return false; };
                var type = "";
                var voieCol = "";
                var voieId = 0;
                switch (d.TypeDefaut)
                {
                    case TypeDefaut.VOIE_INTERNE:
                        {
                            var vi = d.NumeroVoie < sta.VInt!.Count ? sta.VInt[d.NumeroVoie] : null;
                            if (vi == null)
                                continue;
                            type = "CAPTEUR";
                            filtre = (df) => { return df.Type == type && df.Voie_Telemesuree_Id == vi.Id && (df.Fin == null || df.Fin == DateTime.MinValue); };
                            //filtre = $"type='CAPTEUR' AND voie_telemesuree_id={vi.Id}";
                            voieCol = "voie_telemesuree_id";
                            voieId = vi.Id;
                        }
                        break;
                    case TypeDefaut.VOIE_ETAT:
                        {
                            var ve = sta.VEtat!.Where(v => v.ETnum != 0 && v.ETmodule == d.NumeroVoie.ToString())
                                        .FirstOrDefault();
                            if (ve == null)
                                continue;
                            type = "ETAT";
                            filtre = (df) => { return df.Type == type && df.Voie_Tor_Id == ve.Id && (df.Fin == null || df.Fin == DateTime.MinValue); };
                            //filtre = $"type='ETAT' AND voie_tor_id={ve.Id}";
                            voieCol = "voir_tor_id";
                            voieId = ve.Id;
                        }
                        break;
                    default:
                        break;
                }
                var das = dtPertes.Where(filtre)
                    .ToList();
                var sql = "";
                if (das != null && das.Count() > 0)
                {
                    // Le premier trouvé est le dernier défaut ayant DateFin = null
                    var defaut = das[0];
                    sql = $@"
                            UPDATE defaut.defauts SET
                            fin='{d.DateFin?.ToUniversalTime().ToString("dd/MM/yyyy HH:mm:ss")}',
                            actif=false
                            WHERE id={defaut.Id};
                        ";

                    await connexion.ExecuteAsync(sql);
                    //Tools.Execute(sql);
                }
                else
                {
                    // Si on ne trouve pas, alors on insère un défaut avec fin mais sans début connu
                    // S'il ne trouve pas la ligne avec DateDebut
                    // alors, il insère un nouveau défaut
                    var appel = $"'{DateTime.Now.ToUniversalTime().ToString("dd/MM/yyyy HH:mm:ss")}'";
                    var dateFin = d.DateFin == null || d.DateFin == DateTime.MinValue ? "NULL" : $"'{d.DateFin?.ToUniversalTime().ToString("dd/MM/yyyy HH:mm:ss")}'";
                    sql = $@"
                        INSERT INTO defaut.defauts
                        (appel, {voieCol}, debut, fin, type, actif)
                        VALUES
                        ({appel}, {voieId}, NULL, {dateFin}, '{type}', false);
                    ";
                    await connexion.ExecuteAsync(sql);
                    //Tools.Execute(sql);
                    //Tools.Execute(sql, new { Appel = DateTime.Now, NoVoie = voieId, Fin = d.DateFin, Type = type, Actif = false });
                }
            } // foreach(var d in defautVoieFinSansDebut)
            return 0;
        }

        private async Task<int> ProcessDefautFinWithDebut(Station sta, List<DefautVoie> defautVoiesFinAvecDebutConnu)
        {
            // Charges les defauts actifs en cours
            var connexion = AppDbContext.CreateConnection();
            var dtPertes = await LoadDefauts(sta, null, defautVoiesFinAvecDebutConnu);
            foreach (var d in defautVoiesFinAvecDebutConnu)
            {
                Func<Defaut, bool> filtre = (d) => { return false; };
                var type = "";
                var voieCol = "";
                var voieId = 0;
                switch (d.TypeDefaut)
                {
                    case TypeDefaut.VOIE_INTERNE:
                        {
                            var vi = d.NumeroVoie < sta.VInt!.Count ? sta.VInt[d.NumeroVoie] : null;
                            if (vi == null)
                                continue;
                            type = "CAPTEUR";
                            filtre = (df) => { return df.Type == type && df.Voie_Telemesuree_Id == vi.Id && df.Debut == d.DateDebut; };
                            //filtre = $"type='CAPTEUR' AND voie_telemesuree_id={vi.Id}";
                            voieCol = "voie_telemesuree_id";
                            voieId = vi.Id;
                        }
                        break;
                    case TypeDefaut.VOIE_ETAT:
                        {
                            var ve = sta.VEtat!.Where(v => v.ETnum != 0 && v.ETmodule == d.NumeroVoie.ToString())
                                        .FirstOrDefault();
                            if (ve == null)
                                continue;
                            type = "ETAT";
                            filtre = (df) => { return df.Type == type && df.Voie_Tor_Id == ve.Id && df.Debut == d.DateDebut; };
                            //filtre = $"type='ETAT' AND voie_tor_id={ve.Id}";
                            voieCol = "voir_tor_id";
                            voieId = ve.Id;
                        }
                        break;
                    default:
                        break;
                }
                var das = dtPertes?.Where(filtre)
                    .ToList();
                var sql = "";
                if (das != null && das.Count > 0)
                {
                    // S'il trouve une ligne avec DateDebut
                    // met à jour la date de fin du défaut
                    var defaut = das[0];
                    var defautDebut = defaut.Debut;
                    var defautFin = defaut.Fin;
                    int.TryParse(defaut.Id.ToString(), out var defautId);

                    var dateFin = d.DateFin == null || d.DateFin == DateTime.MinValue ? "NULL" : $"'{d.DateFin?.ToUniversalTime().ToString("dd/MM/yyyy HH:mm:ss")}'";
                    sql = $@"
                        UPDATE defaut.defauts SET
                        fin={dateFin},
                        actif=false
                        WHERE id={defautId};
                    ";
                    await connexion.ExecuteAsync(sql);
                    //Tools.Execute(sql);
                    //Tools.Execute(sql, new { Id = defautId, Fin = d.DateFin });
                }
                else
                {
                    // S'il ne trouve pas la ligne avec DateDebut
                    // alors, il insère un nouveau défaut
                    var appel = $"'{DateTime.Now.ToUniversalTime().ToString("dd/MM/yyyy HH:mm:ss")}'";
                    var dateDebut = d.DateDebut == null || d.DateDebut == DateTime.MinValue ? "NULL" : $"'{d.DateDebut?.ToUniversalTime().ToString("dd/MM/yyyy HH:mm:ss")}'";
                    var dateFin = d.DateFin == null || d.DateFin == DateTime.MinValue ? "NULL" : $"'{d.DateFin?.ToUniversalTime().ToString("dd/MM/yyyy HH:mm:ss")}'";
                    sql = $@"
                        INSERT INTO defaut.defauts
                        (appel, {voieCol}, debut, fin, type, actif)
                        VALUES
                        ({appel}, {voieId}, {dateDebut}, {dateFin}, '{type}', false);
                    ";
                    await connexion.ExecuteAsync(sql);
                    //Tools.Execute(sql);
                    //Tools.Execute(sql, new { Appel = DateTime.Now, NoVoie = voieId, Fin = d.DateFin, Type = type, Actif = false });
                }
            } // foreach(var d in disparitions)
            return 0;
        }



        public void GestionAlerteDefauts1121(Station sta)
        {
            Logger.Log("-> GestionAlerteDefauts1121()");
            //var typeAlerte = 10;
            //var PE1 = DateTime.Now;
            //// Recherche si l'alerte existe
            //var sql = "SELECT * FROM Alertes " +
            //    $"WHERE (Initiales='{sta.Initiales}') " +
            //    $"AND (Type={typeAlerte});";
            //var dtAlertes = Tools.Get(sql, $"AlertesTmp_{sta.Initiales}");
            //var exists = (dtAlertes != null && dtAlertes.Rows.Count > 0);
            //if (!exists)
            //{
            //    var texteAlerte = "Alerte défauts états ou capteurs > 300 : traitement non effectué";
            //    sql = "INSERT INTO Alertes (DateHeure, Initiales, Texte, Type, PE1, PS1, Acq) " +
            //        "SELECT " +
            //        $"#{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}#, " +
            //        $"'{sta.Initiales}', '{texteAlerte}', {typeAlerte}, " +
            //        $"'{PE1}', '0', no;"; // PE1, PS1=0, Acq=No
            //    Tools.Execute(sql);
            //} 
        }

        public void SaveMesures(ContextMesure mesures)
        {
            throw new NotImplementedException("To be implemented. Check async version");
        }

        public async Task<(bool, string)> SaveMesuresAsync(ContextMesure mesures)
        {
            // Check input
            if (mesures == null || 
                mesures.Station == null || 
                mesures.Events.Count == 0)
            {
                return new (false, $"Mesures, or Station, or no Events in the mesures object.");
            }
            // Ensure that schema exists
            var check = await EnsureSchemaExists(mesures.Station.Initiales);
            if (!check)
            {
                return new(false, $"Failed to create {mesures.Station.Initiales} schema");
            }

            var connection = ArchiveDbContext.CreateConnection();
            foreach (var ev in mesures.Events)
            {
                // Si l'évènement n'a pas de mesures, on le sauve sans les mesures
                // sinon on sauve les mesures
                if (ev.Mesures.Count == 0)
                {
                    // TODO: evts sans mesure,
                    // sb.AppendLine($"('{mesures.Station.Initiales}', '', '{ev.Time.ToString("MM/dd/yyyy HH:mm:ss")}', " +
                    //     $"'{ev.TypeEvt}', 0, '');");
                }
                else
                {
                    switch (ev.TypeEvt)
                    {
                        case "$":
                            {
                                if (ev.Mesures.Count == 2)
                                {
                                    var mes0 = ev.Mesures[0].ValeurBrute;
                                    var mes1 = ev.Mesures[1].ValeurBrute;
                                    if (mes0 == "a0000" && mes1 == "b0001")
                                    {
                                        continue;
                                    }
                                }
                            }
                            break;
                        case "K":
                            {
                                var zeroCents = ev.Mesures
                                    .Select(m => m as ZeroCentMesure)
                                    .Where(m => m != null)
                                    .ToList();

                                foreach (var mes in zeroCents)
                                {
                                    var voieHeader = mesures.Headers
                                        .Where(m => m.Code == mes!.CodeVoie)
                                        .FirstOrDefault();
                                    if (voieHeader == null || string.IsNullOrEmpty(voieHeader.Libelle))
                                    {
                                        continue;
                                    }
                                    var check1 = await EnsureTableExistsAndGetMaxHorodate(mesures.Station.Initiales, voieHeader.Libelle);
                                    if (check1.Item1)
                                    {
                                        // Only save mesures after maxhorodate
                                        if (check1.Item2 != null && mes!.Time <= check1.Item2)
                                        {
                                            continue;
                                        }
                                        var sb = new StringBuilder();
                                        sb.AppendLine($"INSERT INTO {mesures.Station.Initiales.ToLower()}.{voieHeader.Libelle.ToLower()}" +
                                            $"(horodate, evenement, mesure, mesure_brute) VALUES");
                                        sb.AppendLine($"('{mes!.Time.ToString("dd/MM/yyyy HH:mm:ss")}', " +
                                            $"'{ev.TypeEvt}', {mes.ValeurZero.ToString().Replace(",", ".")}, '{mes.ZeroBrute}'),");
                                        sb.AppendLine($"('{mes.Time.ToString("dd/MM/yyyy HH:mm:ss")}', " +
                                            $"'{ev.TypeEvt}', {mes.ValeurCent.ToString().Replace(",", ".")}, '{mes.CentBrute}');");

                                        var sql = sb.ToString();
                                        Logger?.Log(sql);
                                        await connection.ExecuteAsync(sql);
                                    }
                                } // for (var i = 0; i < ev.Mesures.Count; i += 2)
                            }
                            break;
                        case "S":
                        case "R":
                        case "O":
                        case "&":
                            {
                                foreach(var mes in ev.Mesures)
                                {
                                    var prefix = (ev.TypeEvt == "R" || ev.TypeEvt == "S") ? "e" : "s";
                                    var valeurTors = DecodeTorMesures(mes, prefix);
                                    var station = mesures.Station;
                                    
                                    foreach (var tor in valeurTors)
                                    {
                                        var libelle = station.VTor
                                            .Where(v => v.Numero == tor.Numero)
                                            .Select(v => v.Libelle)
                                            .FirstOrDefault();
                                        if (!string.IsNullOrEmpty(libelle))
                                        {
                                            tor.Libelle = libelle;
                                        }
                                        var check1 = await EnsureTableExistsAndGetMaxHorodate(mesures.Station.Initiales, tor.Libelle.Replace("@", "_"));
                                        if (check1.Item1)
                                        {
                                            // Only save mesures after maxhorodate
                                            if (check1.Item2 != null && mes.Time <= check1.Item2)
                                            {
                                                continue;
                                            }
                                            var sb = new StringBuilder();
                                            //if (tor.Libelle.Replace("@", "_").ToLower() == "")
                                            sb.AppendLine($"INSERT INTO {mesures.Station.Initiales.ToLower()}.{tor.Libelle.Replace("@", "_").ToLower()}" +
                                                $"(horodate, evenement, mesure, mesure_brute) VALUES");
                                            sb.AppendLine($"('{mes.Time.ToString("dd/MM/yyyy HH:mm:ss")}', " +
                                                $"'{ev.TypeEvt}', {tor.BitValeur}, '{mes.ValeurBrute}');");

                                            var sql = sb.ToString();
                                            Logger?.Log(sql);
                                            await connection.ExecuteAsync(sql);
                                        }
                                    }
                                } // foreach(var mes in ev.Mesures)
                            }
                            break;
                        default:
                            {
                                foreach (var mes in ev.Mesures)
                                {
                                    var voieHeader = mesures.Headers
                                        .Where(m => m.Code == mes.CodeVoie)
                                        .FirstOrDefault();
                                    if (voieHeader == null || string.IsNullOrEmpty(voieHeader.Libelle))
                                    {
                                        continue;
                                    }
                                    var check1 = await EnsureTableExistsAndGetMaxHorodate(mesures.Station.Initiales, voieHeader.Libelle);
                                    if (check1.Item1)
                                    {
                                        // Only save mesures after maxhorodate
                                        if (check1.Item2 != null && mes.Time <= check1.Item2)
                                        {
                                            continue;
                                        }
                                        var sb = new StringBuilder();
                                        sb.AppendLine($"INSERT INTO {mesures.Station.Initiales.ToLower()}.{voieHeader.Libelle.ToLower()}" +
                                            $"(horodate, evenement, mesure, mesure_brute) VALUES");
                                        sb.AppendLine($"('{mes.Time.ToString("dd/MM/yyyy HH:mm:ss")}', " +
                                            $"'{ev.TypeEvt}', {mes.Valeur.ToString().Replace(",", ".")}, '{mes.ValeurBrute}');");

                                        var sql = sb.ToString();
                                        Logger?.Log(sql);
                                        await connection.ExecuteAsync(sql);
                                    }
                                }// foreach (var mes in ev.Mesures)
                            }
                            break;
                    } // switch (ev.TypeEvt)
                }
            }
            return (true, "Success inserting mesures data.");
        }

        private class TorMesure
        {
            public int Numero { get; set; }
            public string Libelle { get; set; }
            public int BitValeur { get; set; }
        }

        private List<TorMesure> DecodeTorMesures(Mesure mes, string prefix)
        {
            var indexCarte = (int)mes.CodeVoie[0] - 97;
            if (mes.ValeurBrute.Length != 4)
            {
                throw new Exception("Size not match");
            }
            var scale = 16;
            var numOfBits = 16;
            var binTor = Convert.ToString(Convert.ToInt32(mes.ValeurBrute, scale), 2).PadLeft(numOfBits, '0');
            var numVoies = new List<TorMesure>();
            for (var i = 0; i < 16; i++)
            {
                numVoies.Add(new TorMesure()
                {
                    Numero = 16 * indexCarte + i + 1,
                    Libelle = $"voie_{prefix}tor_{16 * indexCarte + i + 1}",
                    BitValeur = (binTor[15 - i] == '1') ? 1 : 0
                });
            }
            return numVoies;
        }

        private async Task<(bool, DateTime?)> EnsureTableExistsAndGetMaxHorodate(string stationInitiales, string NomVoie)
        {
            var connection = ArchiveDbContext.CreateConnection();
            string query = $"CREATE TABLE IF NOT EXISTS {stationInitiales.ToLower()}.{NomVoie.ToLower()} (horodate TIMESTAMP WITHOUT TIME ZONE NOT NULL, evenement TEXT, mesure DOUBLE PRECISION, mesure_brute TEXT);";
            try
            {
                await connection.ExecuteAsync(query);
            }
            catch (Exception ex)
            {
                Logger?.Log(ex.ExceptionStackTraces());
                return (false, null);
            }

            query = $"SELECT MAX(horodate) AS horodate FROM {stationInitiales.ToLower()}.{NomVoie.ToLower()}";
            try
            {
                var result = await connection.QueryAsync<DateTime?>(query);
                return (true, result.FirstOrDefault());
            }
            catch (Exception ex)
            {
                Logger?.Log(ex.ExceptionStackTraces());
                return (false, null);
            }
        }

        private async Task<bool> EnsureSchemaExists(string stationInitiales)
        {
            var connection = ArchiveDbContext.CreateConnection();
            string query = $"CREATE SCHEMA IF NOT EXISTS {stationInitiales.ToLower()}";
            try
            {
                await connection.ExecuteAsync(query);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}


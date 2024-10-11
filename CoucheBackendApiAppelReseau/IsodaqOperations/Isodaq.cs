using System.Xml;
using IsodaqOperations.Entities;
using IsodaqOperations.Utils;
using SHUT.Core.Data;
using SHUT.Core.Domain.Reseau;

namespace IsodaqOperations
{
    public class Isodaq
    {
        private StationsReseau stationR { get; set; }

        //private FileLogger fileLogger { get; set; }

        public AppDbContext AppDbContext { get; set; }
        public ArchiveDbContext ArchiveDbContext { get; set; }

        public string pathExchangeFiles { get; set; }

        public string XDQDirectory { get; set; }

        public Isodaq(
            StationsReseau stationReseau,
            AppDbContext appContext,
            ArchiveDbContext archiveContext,
            string path
        )
        {
            //station = new Station();
            ArchiveDbContext = archiveContext;
            AppDbContext = appContext;
            stationR = stationReseau;
            DataBaseUtils.AppDbContext = appContext;
            DataBaseUtils.ArchiveDbContext = archiveContext;
            XDQDirectory = @"/mnt/XdqServer/data/";
            pathExchangeFiles = path;
            //string fileLoggerName = $@"Downloads/{stationR.Initiales}/Logs/log_{stationR.Initiales}_{DateTime.Now.ToString("ddMMyyyy'_'HHmmss")}.txt";
            //fileLogger = new FileLogger(fileLoggerName, () => { return DateTime.Now.ToString("HH:mm:ss.fff"); });
        }

        public async Task<(bool success, string errorMessage)> Execute()
        {
            const bool PrintConsole = true;
            //const string XDQDirectory = @"C:\shut-refonte\Prototypes_dev\LectureXDQ\Sources";
            DateTime appel = DateTime.Now; // Args

            // --------- Do not change anything after this ---------------
            try
            {
                // -------- Step 1 : Get station and files --------
                // Get files
                string[]? filesFullNames = FileUtils.GetSourceFilesFullNames(
                    XDQDirectory,
                    stationR.Initiales,
                    PrintConsole
                );

                // Get Station
                Station? station = await DataBaseUtils.GetStation(stationR.Initiales);
                if (filesFullNames == null || station == null)
                {
                    Console.WriteLine(
                        $"Abandon de la lecture : aucun fichier trouvé ou station {stationR.Initiales} inconnue"
                    );
                    return (
                        false,
                        $"Aucun fichier trouvé ou station {stationR.Initiales} inconnue"
                    );
                }
                else if (
                    station.Enregistreur.TypeLiaison != null
                    && !station.Enregistreur.TypeLiaison.Equals("XX")
                )
                {
                    Console.WriteLine(
                        $"Abandon de la lecture : type de liaison {station.Enregistreur.TypeLiaison} non pris en charge pour la station {stationR.Initiales}"
                    );
                    return (
                        false,
                        $"Type de liaison {station.Enregistreur.TypeLiaison} non pris en charge pour la station {stationR.Initiales}"
                    );
                }
                station.VoiesInternes = await DataBaseUtils.GetVoiesInternes(station.Id);
                if (station.VoiesInternes.Count == 0)
                {
                    Console.WriteLine(
                        $"Abandon de la lecture : la station {station.Initiales} n'a pas de voies internes"
                    );
                    return (false, $"La station {station.Initiales} n'a pas de voies internes");
                }

                DefautsActifsUtils.AddInactiveDefaut(station.VoiesInternes, appel);

                station.PrintStation();
                if (PrintConsole)
                {
                    PrintUtils.PrintISODAQConfigVoiesInternes(station.VoiesInternes);
                }

                List<Mesure> mesuresOfThisStation = new List<Mesure>();

                // For each file => reading to get List<Mesure>
                foreach (string filefullName in filesFullNames)
                {
                    Console.WriteLine($"\n------ Reading file {filefullName}...");

                    List<Mesure> mesuresOfThisFile = new List<Mesure>();

                    XmlDocument? document = XMLUtils.GetDocument(filefullName);
                    if (document == null)
                    {
                        return (false, $"Impossible de lire le document XML : {filefullName}");
                    }

                    // ------------ Step 2 : Get enregistreur version ------------
                    string version = ISODAQUtils.GetEnregistreurVersion(document);
                    station.Enregistreur.Version = version;
                    if (PrintConsole)
                    {
                        Console.WriteLine($"version = {version}");
                    }

                    // Voies
                    XmlNodeList? voiesXML = XMLUtils.GetVoies(document);
                    if (voiesXML == null)
                    {
                        Console.WriteLine(
                            $"Abandon de la lecture pour ce fichier : le fichier ne contient pas de voies internes"
                        );
                        continue;
                    }

                    // For each voie => reading the reference table and calculating the mesures
                    for (int i = 0; i < voiesXML.Count; i++)
                    {
                        XmlNode voieXML = voiesXML[i]!;
                        Console.WriteLine($"\n------ Reading voie n°{i + 1}...");

                        // ------------ Step 3 : Get Mesures for each voie -----------
                        List<Mesure>? mesuresOfThisVoie = ISODAQUtils.GetMesures(
                            voieXML,
                            station.VoiesInternes[i],
                            PrintConsole
                        );
                        if (mesuresOfThisVoie == null)
                        {
                            continue;
                        }

                        mesuresOfThisFile.AddRange(mesuresOfThisVoie);
                    }

                    PrintUtils.PrintMesures(mesuresOfThisFile);
                    mesuresOfThisStation.AddRange(mesuresOfThisFile); // Mesures with defauts (9999)

                    //TODO: Gestion Alertes Batterie

                    (double seuil, string niveau) InfoBattery = ISODAQUtils.GetBatteryInfo(
                        document
                    );
                    if (string.IsNullOrEmpty(InfoBattery.niveau))
                    {
                        continue;
                    }
                    else
                    {
                        var resultAlerte = await DataBaseUtils.InsertAlerte(
                            InfoBattery.seuil,
                            InfoBattery.niveau,
                            station.Id
                        );
                        if (!resultAlerte)
                        {
                            //Strategie de remediation pour ne pas perdre la data qui n'a pas pu etre inseree
                            PrintUtils.PrintConsoleException(null, "Pb Insertion alerte en base");
                        }
                    }
                } // End foreach file

                //TODO : Persist mesure data
                var resultMesure = await DataBaseUtils.InsertMesures(
                    mesuresOfThisStation,
                    station.Initiales
                );
                if (!resultMesure)
                {
                    //Strategie de remediation pour ne pas perdre la data qui n'a pas pu etre inseree
                    PrintUtils.PrintConsoleException(null, "Pb Insertion mesure en base");
                }

                // ------------------ Step 4 : Handle defauts ----------------
                // Update VoieInterne.DefautActif
                DefautsActifsUtils.ReadDefautsActifs(station.VoiesInternes, mesuresOfThisStation); // Update VoieInterne.DefautActif

                // Update or delete defaut.defauts_actifs
                await DefautsActifsUtils.UpdateOrDeleteDefautsActifs(
                    station.VoiesInternes,
                    appel,
                    PrintConsole
                );
                return (true, "Opération réussie");
            }
            catch (Exception ex)
            {
                if (ex is ArgumentNullException || ex is NullReferenceException)
                {
                    PrintUtils.PrintConsoleException(ex, "Unexpected null exception");
                }
                PrintUtils.PrintConsoleException(ex, "Erreur inconnue"); //Dev
                return (false, $"Une erreur est survenue : {ex.Message}");
            }
        }
    }
}

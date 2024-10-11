using System.Diagnostics;
using System.Net.Sockets;
using TestConnexion.AutomateProtocol;
using TestConnexion.Entities;
using TestConnexion.Entities.Infos;
using TestConnexion.Log;
using TestConnexion.Utils;

namespace TestConnexion
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            const bool LocalMode = true; // If false will use the real ip address
            const bool PremiumStopQuestion4 = false;
            const bool PrintConsole = true;
            const bool PrintInfoDetails = true;
            const bool HandleDateInversions = false;

            // ------ Do not change anything after this ------------

            // M580 => XY (D16 RU)
            // Premium => EN (D13 RU)
            Console.WriteLine("Which station do you want to connect to ?");
            string initiales = Console.ReadLine().Trim().ToUpper();
            if (initiales == null)
            {
                Console.WriteLine("\n--- Initiales could not be null");
                return;
            }
            const int PortTCP = 502;

            string fileLoggerName = $@"Downloads/{initiales}/Logs/log_{initiales}_{DateTime.Now.ToString("ddMMyyyy'_'HHmmss")}.txt";
            FileLogger fileLogger = new FileLogger(fileLoggerName, () => { return DateTime.Now.ToString("HH:mm:ss.fff"); });
            Stopwatch mainClock = Stopwatch.StartNew();

            Station station = await DataBaseUtils.GetStation(initiales, fileLogger, LocalMode);
            if (station == null)
            {
                fileLogger.Log($"Annulation appel : station {initiales} inconnue ou inutilisable");
                return;
            }
            station.PrintStation();
            fileLogger.Log($"Début appel station {station}");

            if (!station.TypeLiaison!.Equals("AP"))
            {
                fileLogger.Log($"Arrêt appel : liaison {station.TypeLiaison} inconnue ou inutilisable");
                return;
            }

            // Ping the station
            if (!await PingUtils.TryPingStation(station.AdresseIP!, fileLogger, PrintConsole)) { return; }

            try
            {
                // -------------------------------------------------------------
                // ----------- First : Get binary file from station ------------
                // -------------------------------------------------------------

                Console.WriteLine($"\n------ Trying to connect to TCP Server {station.AdresseIP}:{PortTCP}");

                // Connexion to the TCP server
                using TcpClient client = new TcpClient();
                await client.ConnectAsync(station.AdresseIP!, PortTCP);
                NetworkStream stream = client.GetStream();

                Console.WriteLine($"\n------ TCP Connexion open");

                // -------------- Automate Shared Protocol -------------

                SharedAutomateProtocol sharedAutomateProtocol = new SharedAutomateProtocol(station, stream, fileLogger);
                if (PrintConsole) { Console.WriteLine("\n------------- Shared Automate protocol -------------"); }

                // Question n°1 : Are there available datas ?
                bool areDatasAvailable = await sharedAutomateProtocol.AreDatasAvailable(PrintConsole);
                if (PrintConsole) { Console.WriteLine($"AreDatasAvailable: {areDatasAvailable}"); }
                if (!areDatasAvailable)
                {
                    Console.WriteLine("\n------ TCP Connexion closed : Error in areDatasAvailable or No datas available ----\n");
                    return;
                }
                fileLogger.Log($"Données disponibles");

                // Question n°2 : "LECT CONFIG_VER"
                bool getVersionSuccessfull = await sharedAutomateProtocol.TryReadVersion(PrintConsole);
                if (!getVersionSuccessfull)
                {
                    Console.WriteLine("\n------ TCP Connexion closed : Error in LECT VERSION ----\n");
                    return;
                }
                if (PrintConsole) { Console.WriteLine($"Version of {station.Initiales}: {station.Version}"); }
                fileLogger.Log($"Version = {station.Version}");

                // Instanciate Specific Automate Protocol
                ISpecificAutomateProtocol specificAutomateProtocol = sharedAutomateProtocol.GetSpecificAutomateProtocol(PrintConsole);

                if (specificAutomateProtocol == null)
                {
                    Console.WriteLine("\n------ TCP Connexion closed : Unknow version ----\n");
                    fileLogger.Log($"Arrêt appel : version {station.Version} non prise en charge par le protocole automate");
                    return;
                }

                bool premiumProtocol = specificAutomateProtocol is AutomatePremiumProtocol;
                specificAutomateProtocol = premiumProtocol ? (AutomatePremiumProtocol)specificAutomateProtocol : (AutomateM580Protocol)specificAutomateProtocol;

                // -------------- Specific Protocol ---------------

                // Question n°3 : "LECT CONFIG_ANA"
                bool getAnalogicConfigSuccessfull = await specificAutomateProtocol.TryRead60AnalogicConfig(PrintConsole);
                if (!getAnalogicConfigSuccessfull)
                {
                    Console.WriteLine("\n------ TCP Connexion closed : Error in LECT CONFIG_ANA ----\n");
                    return;
                }
                if (PrintConsole) { PrintUtils.PrintAutomateConfigVoiesInternes(station.VoiesInternes); }

                // Question n°4 : "LECT VALTR_ANA"
                bool getRTDatasSuccessfull = await specificAutomateProtocol.TryRead60AnalogicRealTimeDatas(PrintConsole);
                if (!getRTDatasSuccessfull)
                {
                    Console.WriteLine("\n------ TCP Connexion closed : Error in LECT VALTR_ANA ----\n");
                    return;
                }
                if (PrintConsole) { PrintUtils.PrintAutomateRealTimeDatasVoiesInternes(station.VoiesInternes); }

                // Question n°5 : LECT DATA
                bool getDatasSuccessfull = false;
                if (!PremiumStopQuestion4 || !premiumProtocol)
                {
                    getDatasSuccessfull = await specificAutomateProtocol.TryGetDatas(PrintConsole);
                }
                if (!getDatasSuccessfull)
                {
                    Console.WriteLine("\n------ TCP Connexion closed : Error in  LECT DATA ----\n");
                    return;
                }

                // Closing the connexion as soon as possible
                client.Dispose();

                // Exit without error
                Console.WriteLine("\n------ TCP Connexion closed : End of discussion without error ----\n");
                mainClock.Stop();
                double mainTimeSpan = mainClock.ElapsedMilliseconds/1000.0;
                Console.WriteLine($"Calling time spend : {mainTimeSpan}s");
                fileLogger.Log($"Fin appel station {station.Initiales} sans erreur ({mainTimeSpan}s)");

                // -------------------------------------------------------------
                // ---------------- Second : bin to POCO & csv -----------------
                // -------------------------------------------------------------

                fileLogger.Log($"LECT/ECRITURE MESURES");
                mainClock = Stopwatch.StartNew();

                // Input files
                string sourceDirectory = $"Downloads/{station.Initiales}/"; // Dev
                string[] sourceFilesFullNames = null;
                try
                {
                    sourceFilesFullNames = FileUtils.GetSourceFilesFullNames(sourceDirectory, station.Initiales, PrintConsole);
                }
                catch (Exception ex)
                {
                    fileLogger.LogException(ex, "Lecture des mesures (recherche fichiers sources)", -9);
                    PrintUtils.PrintConsoleException(ex, $"Error while looking for source file in {sourceDirectory}");
                    return;
                }

                if (sourceFilesFullNames == null)
                {
                    fileLogger.Log($"Arrêt lecture des mesures : aucun fichier de données trouvé");
                    return;
                }

                // Output files
                string binaryFileDirectory = $"Downloads/{station.Initiales}/Binary";

                // 1 binary file = 1 csv file
                int successfullReadingWritingMesures = 0;
                List<Mesure> mesuresWithDefaut = new List<Mesure>();
                for (int i = 0; i < sourceFilesFullNames.Length; i++)
                {
                    DateTime today = DateTime.Now;
                    string binaryFileShortName = $"{station.Initiales}_{today.ToString("ddMMyyyyHHmmss")}.bin";
                    string binaryFileFullName = @$"{binaryFileDirectory}/{binaryFileShortName}";
                    fileLogger.Log($"Début lecture des mesures depuis {binaryFileShortName}");

                    // Step n°1 : copying the file
                    try
                    {
                        FileUtils.TryCopyFile(sourceFilesFullNames[i], binaryFileFullName, PrintConsole);
                    }
                    catch (Exception ex)
                    {
                        fileLogger.LogException(ex, $"Lecture des mesures (Copie du fichier source) {sourceFilesFullNames[i]}", -10);
                        PrintUtils.PrintConsoleException(ex, $"Error while copying the file {sourceFilesFullNames} into {binaryFileFullName}");
                        continue;
                    }

                    // Step n°2 : reading the datas in the binary file
                    Stopwatch secondClock = Stopwatch.StartNew();

                    // Creating the list of mesures of this file
                    List<Mesure> mesuresOfThisFile = specificAutomateProtocol.GetMesuresFromBinaryFile(binaryFileFullName, PrintConsole);
                    if (mesuresOfThisFile == null) { continue; }

                    secondClock.Stop();
                    double readingTimeSpan = secondClock.ElapsedMilliseconds/1000.0;
                    fileLogger.Log($"Lecture de {mesuresOfThisFile.Count} données depuis {binaryFileShortName} en {readingTimeSpan}s");

                    // Deleting mesures showing date inversions
                    if (HandleDateInversions)
                    {
                        int oldMesuresCount = mesuresOfThisFile.Count;
                        mesuresOfThisFile = AutomateCsvFileUtils.DeleteDateInversions(mesuresOfThisFile, PrintConsole, PrintInfoDetails);
                        int deletedMesuresCount = oldMesuresCount - mesuresOfThisFile.Count;
                        if (deletedMesuresCount > 0) { fileLogger.Log($"Suppression de {deletedMesuresCount} mesures avec inversion de date ({mesuresOfThisFile.Count} mesures restantes)"); }
                    }

                    fileLogger.LogAll(PrintUtils.GetMesures(mesuresOfThisFile));
                    if (PrintConsole) { PrintUtils.PrintMesures(mesuresOfThisFile, PrintInfoDetails); }

                    // Step n°3 : writing the data in .csv file
                    fileLogger.Log($"Début écriture des mesures dans fichier csv");
                    secondClock = Stopwatch.StartNew();

                    bool csvFileWritingSuccessfull = await specificAutomateProtocol.TryWriteCsvFile(binaryFileFullName, mesuresOfThisFile, PrintConsole);
                    if (!csvFileWritingSuccessfull) { continue; }

                    secondClock.Stop();
                    double writingTimeSpan = secondClock.ElapsedMilliseconds/1000.0;
                    fileLogger.Log($"Fin écriture des mesures depuis {binaryFileShortName} en {writingTimeSpan}s");
                    if (PrintConsole)
                    {
                        Console.WriteLine($"\nTime spend reading datas : {readingTimeSpan}s");
                        Console.WriteLine($"Time spend writing the .csv : {writingTimeSpan}s");
                    }
                    successfullReadingWritingMesures++;
                    // Adding the mesures with defaut of this file to the big list
                    mesuresWithDefaut.AddRange(mesuresOfThisFile.Where(m =>
                    {
                        return
                        (m.Info is InfoAnalogicCalcul  && (((InfoAnalogicCalcul)m.Info).DebutDefaut || ((InfoAnalogicCalcul)m.Info).FinDefaut))
                        ||
                        (m.Info is InfoAnalogicCapteur  && (((InfoAnalogicCapteur)m.Info).DebutDefaut || ((InfoAnalogicCapteur)m.Info).FinDefaut));
                    }));
                } // End For loop

                mainClock.Stop();
                double readingWritingTimeSpan = mainClock.ElapsedMilliseconds/1000.0;
                Console.WriteLine($"Reading/Writing mesures time spend : {readingWritingTimeSpan}s");
                fileLogger.Log($"Fin lecture et écriture des mesures de {station.Initiales} sans erreur ({readingWritingTimeSpan}s) : {successfullReadingWritingMesures}/{sourceFilesFullNames.Length} fichiers lus et écrits");

                // -------------------------------------------------------------
                // ----------------- Third : Handling defauts ------------------
                // -------------------------------------------------------------
                fileLogger.Log("Début gestion défauts des voies internes");
                mainClock = Stopwatch.StartNew();

                List<Defaut> defautsOfThisStation = await DefautUtils.HandleMesureWithDefaut(mesuresWithDefaut, station.VoiesInternes, fileLogger);

                // Updating voies internes and inserting defauts
                await DataBaseUtils.UpdateStation(station, fileLogger);
                await DataBaseUtils.UpdateAllVoiesInternes(station.VoiesInternes, fileLogger);
                await DataBaseUtils.InsertAllDefautsOfThisStation(defautsOfThisStation, fileLogger);

                int activeDefauts = 0;
                for (int i = 0; i < 32; i++)
                {
                    if (station.VoiesInternes[i].Defaut) { activeDefauts++; }
                }

                mainClock.Stop();
                double handlingDefautTimeSpan = mainClock.ElapsedMilliseconds/1000.0;
                fileLogger.LogAll(PrintUtils.GetDefauts(defautsOfThisStation));

                if (PrintConsole) 
                {
                    PrintUtils.PrintDefauts(defautsOfThisStation);
                    Console.WriteLine($"Handling defauts time spend : {handlingDefautTimeSpan}s");
                }
                fileLogger.Log($"Fin gestion défauts des voies internes : {activeDefauts}/32 voies internes avec défaut actif ({handlingDefautTimeSpan}s)");
            }
            catch (Exception ex)
            {
                if (ex is SocketException)
                {
                    Console.WriteLine("\n---- Unable to connect to the server ----");
                    fileLogger.LogException(ex, $"Création du serveur TCP {station.AdresseIP}.{PortTCP}", 202);
                }
                else if (ex is ArgumentNullException)
                {
                    Console.WriteLine($"\n---- Unable to connect to the server : adresseIp is null");
                    fileLogger.LogException(ex, $"Création du serveur TCP {station.AdresseIP}.{PortTCP}", 203);
                }
                else if (ex is IOException)
                {
                    Console.WriteLine("\n---- Disconnected from the server ----");
                    fileLogger.LogException(ex, $"Echanges avec le serveur TCP", 3005);
                }
                else
                {
                    Console.WriteLine("\n---- Unknown exception ----");
                    fileLogger.LogException(ex, $"Intéraction avec le serveur TCP", -1);
                }
            }
        } // End Main
    }
}

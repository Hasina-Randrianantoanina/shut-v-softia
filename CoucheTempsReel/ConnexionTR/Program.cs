using ConnexionTR.AutomateProtocol;
using ConnexionTR.Entities;
using ConnexionTR.Log;
using ConnexionTR.Utils;
using System.Diagnostics;
using System.Net.Sockets;

namespace ConnexionTR
{
    public class Program
    {
        static async Task Main(string[] args)
        {

            // Ouvrir, OuvrirAPI_RO,  Fermer, FermerAPI, FermerAPI_RO,LectureCFG, LectureAPI_RO, LectureXDQ_RO, LectureCFG_RO
            // EcritureCFG, EcritureAPI_RO, EcritureCFG_RO, CdeInit, STENINIT, VidageJour, VidageJourType2

            // -----------------------------------------------------------------------------------
            // VERSION = LECT CONFIG_VER
            // CONFIG = LECT CONFIG_ANA -> LECT CONFIG_ECH -> LECT CONFIG_TOR
            // TEMPS_REEL = LECT VALTR_ANA -> LECT VALTR_TOR -> LECT VALTR_mA -> LECT VALTR_ECH 
            // ECRITURE = ECR_CONFIG_ANA -> ECR_CONFIG_ANA2 -> ECR CONFIG_TOR 
            // -----------------------------------------------------------------------------------

            // LectureAPI = VERSION + CONFIG
            // OuvrirAPI = VERSION + CONFIG + TEMPS_REEL 
            // EcritureAPI = VERSION + CONFIG + ECRITURE + CONFIG
            const string Action = "OuvrirAPI";
            const bool LocalMode = true;
            const bool PrintConsole = true;

            // ------ Do not change anything after this ------------

            // M580 => XY (D16 RU)
            // Premium => EN (D13 RU)
            Console.WriteLine("Which station do you want to connect to ?");
            string initiales = "FH";
            //initiales = Console.ReadLine().Trim().ToUpper();
            //if (initiales == null)
            //{
            //    Console.WriteLine("\n--- Initiales could not be null");
            //    return;
            //}
            const int PortTCP = 502;

            string fileLoggerName = $@"RealTime/{initiales}/Logs/log_{initiales}_{DateTime.Now.ToString("ddMMyyyy'_'HHmmss")}.txt";
            FileLogger fileLogger = new FileLogger(fileLoggerName, () => { return DateTime.Now.ToString("HH:mm:ss.fff"); });
            Stopwatch mainClock = Stopwatch.StartNew();

            Station station = await DataBaseUtils.GetStation(initiales, fileLogger, LocalMode);
            station.VoiesInternesStation = await DataBaseUtils.GetVoiesInternes(station.Id, fileLogger);
            if (station == null)
            {
                fileLogger.Log($"Annulation appel : station {initiales} inconnue ou inutilisable");
                return;
            }
            station.PrintStation();
            PrintUtils.PrintAutomateConfigVoiesInternes(station.VoiesInternesStation);

            fileLogger.Log($"Début appel station {station} avec action {Action}");

            if (!station.EnregistreurBase.TypeLiaison.Equals("AP"))
            {
                fileLogger.Log($"Arrêt appel : liaison {station.EnregistreurBase.TypeLiaison} inconnue ou inutilisable");
                return;
            }

            // Ping the station
            if (!await PingUtils.TryPingStation(station.EnregistreurBase.AdresseIP, fileLogger, PrintConsole)) { return; }

            try
            {
                Console.WriteLine($"\n------ Trying to connect to TCP Server {station.EnregistreurBase.AdresseIP}:{PortTCP}");

                // Connexion to the TCP server
                using TcpClient client = new TcpClient();
                await client.ConnectAsync(station.EnregistreurBase.AdresseIP, PortTCP);
                NetworkStream stream = client.GetStream();

                Console.WriteLine($"\n------ TCP Connexion open");

                //// -------------- Automate Shared Protocol -------------

                SharedAutomateProtocol sharedAutomateProtocol = new SharedAutomateProtocol(station, stream, fileLogger);
                if (PrintConsole) { Console.WriteLine("\n------------- Shared Automate protocol -------------"); }

                // --------------- Common 1 : LECT CONFIG_VER ------------------
                bool getVersionSuccessfull = await sharedAutomateProtocol.TryReadVersion(PrintConsole);
                if (!getVersionSuccessfull)
                {
                    Console.WriteLine("\n------ TCP Connexion closed : Error in LECT VERSION ----\n");
                    return;
                }
                if (PrintConsole) { Console.WriteLine($"Version of {station.Initiales}: {station.EnregistreurStation.Version}"); }
                fileLogger.Log($"Version = {station.EnregistreurStation.Version}");

                // Instanciate Specific Automate Protocol
                ISpecificAutomateProtocol specificAutomateProtocol = sharedAutomateProtocol.GetSpecificAutomateProtocol(PrintConsole);

                if (specificAutomateProtocol == null)
                {
                    Console.WriteLine("\n------ TCP Connexion closed : Unknow version ----\n");
                    fileLogger.Log($"Arrêt appel : version {station.EnregistreurStation.Version} non prise en charge par le protocole automate");
                    return;
                }

                bool premiumProtocol = specificAutomateProtocol is AutomatePremiumProtocol;
                specificAutomateProtocol = premiumProtocol ? (AutomatePremiumProtocol)specificAutomateProtocol : (AutomateM580Protocol)specificAutomateProtocol;

                // -------------- Specific Protocol ---------------

                // --------------- Common 2 : LECT CONFIG_ANA ------------------
                bool getAnalogicConfigSuccessfull = await specificAutomateProtocol.TryRead60AnalogicConfig(PrintConsole);
                if (!getAnalogicConfigSuccessfull)
                {
                    Console.WriteLine("\n------ TCP Connexion closed : Error in LECT CONFIG_ANA ----\n");
                    return;
                }
                if (PrintConsole) { PrintUtils.PrintAutomateConfigVoiesInternes(station.VoiesInternesStation); }

                // --------------- Common 3 : LECT CONFIG_ECH ------------------

                // --------------- Common 4 : LECT CONFIG_TOR ------------------

            }
            catch (Exception ex)
            {
                if (ex is SocketException)
                {
                    Console.WriteLine("\n---- Unable to connect to the server ----");
                    fileLogger.LogException(ex, $"Création du serveur TCP {station.EnregistreurBase.AdresseIP}.{PortTCP}", 202);
                }
                else if (ex is ArgumentNullException)
                {
                    Console.WriteLine($"\n---- Unable to connect to the server : adresseIp is null");
                    fileLogger.LogException(ex, $"Création du serveur TCP {station.EnregistreurBase.AdresseIP}.{PortTCP}", 203);
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

        }
    }
}

using Connexion.AutomateProtocol;
using Connexion.Entities;
using Connexion.Log;
using Connexion.Utils;
using System.Diagnostics;
using System.Net.Sockets;

namespace Connexion
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // -------------- Choose the initiales -----------------

            // M580 => GA (D15 RU) or HE (D16 RU) or 172 (D15 RO) or 178 (D15 RO with Mesure ECH)
            // Premium => BP ("" RU) or EN (D13 RU) or RC ("" RU, defauts)
            // Load Test Premium => LTP
            const string Initiales = "EN";
            const bool PrintConsole = true;
            const bool TestMode = true; // false to use 18/07/2024 prod data
            const bool TestVM = false; // true to test in VM preprod DEA

            // ------ Do not change anything after this ------------

            // Loading env variables
            DotEnv.Load(".env");

            const int PortTCP = 502;
            const int PortFTP = 6200;

            string fileLoggerName = $@"Downloads/{Initiales}/Logs/log_{Initiales}_{DateTime.Now.ToString("ddMMyyyy'_'HHmmss")}.txt";
            FileLogger fileLogger = new FileLogger(fileLoggerName, () => { return DateTime.Now.ToString("HH:mm:ss.fff"); });

            Stopwatch mainClock = Stopwatch.StartNew();

            Station station = TestMode ? DataBaseUtils.GetStationTest(Initiales, TestVM) : DataBaseUtils.GetStation(Initiales);
            if (station == null) { return; }
            station.PrintStation();

            fileLogger.Log($"Début appel station {station.ToString()}");

            if (!station.TypeLiaison.Equals("AP"))
            {
                fileLogger.Log($"Liaison {station.TypeLiaison} inconnue ou inutilisable");
                return;
            }

            // Ping the station
            if (!(await PingUtils.PingStation(station.AdresseIP, fileLogger, PrintConsole))) { return; }

            try
            {
                Console.WriteLine($"\n------ Trying to connect to TCP Server {station.AdresseIP}:{PortTCP}");

                // Connexion to the TCP server
                using TcpClient client = new TcpClient();
                await client.ConnectAsync(station.AdresseIP, PortTCP);
                NetworkStream stream = client.GetStream();

                Console.WriteLine($"\n------ TCP Connexion open");

                // -------------- Automate Shared Protocol -------------

                SharedAutomateProtocol sharedAutomateProtocol = new SharedAutomateProtocol(stream, fileLogger);
                if (PrintConsole) { Console.WriteLine("\n------------- Shared Automate protocol -------------"); }

                // Question n°1 : Are there available datas ?
                bool areDatasAvailable = await sharedAutomateProtocol.AreDatasAvailable(PrintConsole);
                if (PrintConsole) { Console.WriteLine($"AreDatasAvailable: {areDatasAvailable}"); }
                if (!areDatasAvailable)
                {
                    fileLogger.Log("Pas de données");
                    Console.WriteLine("\n------ TCP Connexion closed : No datas available ----\n");
                    return;
                }
                fileLogger.Log($"Données disponibles");

                // Question n°2 : "LECT CONFIG_VER"
                station.Version = await sharedAutomateProtocol.ReadVersion(PrintConsole);
                if (station.Version == "") { station.Version = "MANQUE"; }
                if (PrintConsole) { Console.WriteLine($"Version of {station.Initiales}: {station.Version}"); }
                fileLogger.Log($"Version = {station.Version}");

                // Instanciate Specific Automate Protocol
                ISpecificAutomateProtocol specificAutomateProtocol = sharedAutomateProtocol.GetSpecificAutomateProtocol(station, PrintConsole);

                if (specificAutomateProtocol == null)
                {
                    Console.WriteLine("\n------ TCP Connexion closed : Error in LECT VERSION ----\n");
                    return;
                }

                bool premiumProtocol = specificAutomateProtocol is AutomatePremiumProtocol;
                specificAutomateProtocol = premiumProtocol ? (AutomatePremiumProtocol)specificAutomateProtocol : (AutomateM580Protocol)specificAutomateProtocol;

                // -------------- Specific Protocol ---------------

                // Question n°3 : "LECT CONFIG_ANA"
                station.VoiesInternes = await specificAutomateProtocol.Read60AnalogicConfig(PrintConsole);
                if (station.VoiesInternes == null)
                {
                    Console.WriteLine("\n------ TCP Connexion closed : Error in LECT CONFIG_ANA ----\n");
                    return;
                }
                if (PrintConsole) { PrintUtils.PrintAutomateConfigVoiesInternes(station.VoiesInternes); }

                // Question n°4 : "LECT VALTR_ANA"
                await specificAutomateProtocol.Read60AnalogicRealTimeDatas(station.VoiesInternes, PrintConsole);
                if (PrintConsole) { PrintUtils.PrintAutomateRealTimeDatasVoiesInternes(station.VoiesInternes); }

                // Question n°5 : LECT DATA
                await specificAutomateProtocol.GetDatas(station, PortFTP, PrintConsole, TestMode, TestVM);

                // Exit without error
                Console.WriteLine("\n------ TCP Connexion closed : End of discussion without error ----\n");
            }
            catch (Exception ex)
            {
                if (ex is SocketException) { Console.WriteLine("\n---- Unable to connect to the server ----"); }
                else if (ex is IOException) { Console.WriteLine("\n---- Disconnected from the server ----"); }
                else { Console.WriteLine("\n---- Unknown exception ----"); }
            }

            mainClock.Stop();
            double mainTimeSpan = mainClock.ElapsedMilliseconds/1000.0;
            Console.WriteLine($"Time spend : {mainTimeSpan}s");
            fileLogger.Log($"Fin appel station {station.Initiales} sans erreur ({mainTimeSpan}s)");
        } // End Main

    }
}

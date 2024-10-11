using Reseau.Entities;
using Reseau.Servers;
using Reseau.Utils;

namespace Reseau
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            // -------------- Choose the initiales -----------------

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

            // M580 => FH (D16 RU)
            const string Initiales = "FH";

            // ------ Do not change anything after this ------------

            const int PortTCP = 502;

            Station station = DatabaseUtils.GetStation(Initiales);
            if (station == null) return;

            TCPServer tcpserver = CreateTCPServer(station, PortTCP, Action);

            // Waiting for the stopping signal
            Console.WriteLine("Servers started. Press any key to stop...");
            Console.ReadKey();

            // Stopping the servers
            tcpserver.Stop();
        }

        private static TCPServer CreateTCPServer(Station station, int startingPortTCP, string action)
        {
            if (station.TypeLiaison != "AP") return null;

            string serverTCPId = $"TcpServer ({station.Initiales}) with action = {action}";
            int portTCP = startingPortTCP;
            TCPServer tcpserver = new TCPServer(serverTCPId, station, portTCP, action);
            tcpserver.Start();

            return tcpserver;
        }

    }
}

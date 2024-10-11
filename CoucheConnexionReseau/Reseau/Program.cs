using Reseau.Entities;
using Reseau.Servers;
using Reseau.Utils;

namespace Reseau
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
            const int NumberOfDatasToDownload = 1000 * 1000 * 10; // If LTP

            // ------ Do not change anything after this ------------

            const int PortTCP = 502;

            Station station = DatabaseUtils.GetStation(Initiales);
            if (station == null && !Initiales.Equals("LTP")) return;

            TCPServer tcpserver = null;
            TCPServerLTP tCPServerLTP = null;

            if (Initiales.Equals("LTP"))
            {
                tCPServerLTP = CreateTCPServerLTP(PortTCP, NumberOfDatasToDownload);
            }
            else
            {
                tcpserver = CreateTCPServer(station, PortTCP);
            }

            // Waiting for the stopping signal
            Console.WriteLine("Servers started. Press any key to stop...");
            Console.ReadKey();

            // Stopping the servers
            StopServer(tcpserver);
            StopServer(tCPServerLTP);
        }

        private static TCPServerLTP CreateTCPServerLTP(int startingPortTCP, int numberOfDatasToDownload)
        {
            Station station = new Station
            {
                Initiales = "LTP",
                AdresseIP = "127.0.0.1",
                TypeLiaison = "AP"
            };
            string serverTCPId = $"TcpServer ({station.Initiales})";
            int portTCP = startingPortTCP;
            TCPServerLTP tcpserver = new TCPServerLTP(serverTCPId, station, portTCP, numberOfDatasToDownload);
            tcpserver.Start();

            return tcpserver;
        }

        private static TCPServer CreateTCPServer(Station station, int startingPortTCP)
        {
            if (station.TypeLiaison != "AP") return null;

            string serverTCPId = $"TcpServer ({station.Initiales})";
            int portTCP = startingPortTCP;
            TCPServer tcpserver = new TCPServer(serverTCPId, station, portTCP);
            tcpserver.Start();

            return tcpserver;
        }

        private static void StopServer(Object server)
        {
            if (server is TCPServer)
            {
                ((TCPServer)server).Stop();
            }
            else if (server is TCPServerLTP)
            {
                ((TCPServerLTP)server).Stop();
            }
        }

    }
}

using Connexion.Log;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text;

namespace Connexion.Utils
{
    public static class PingUtils
    {
        public static async Task<bool> PingStation(string adresseIP, FileLogger fileLogger, bool printConsole)
        {

            using Ping sender = new Ping();
            PingOptions options = new PingOptions();
            options.DontFragment = true;

            string data = new String('a', 32);
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 120;
            try
            {
                Stopwatch clock = Stopwatch.StartNew();
                // Try ping server
                PingReply reply = await sender.SendPingAsync(adresseIP, timeout, buffer, options);
                clock.Stop();
                if (reply.Status == IPStatus.Success)
                {

                    fileLogger.Log($"Ping serveur {reply.Address} : réussite ({clock.ElapsedMilliseconds/1000.0}s)");
                    if (printConsole) { Console.WriteLine("--- Ping Server OK"); }
                    return true;
                }
                fileLogger.Log($"Ping serveur {adresseIP} : échec");
                if (printConsole) { Console.WriteLine("--- Ping Server NOK"); }

                // Try ping router, always return false
                try
                {
                    clock = Stopwatch.StartNew();
                    string adresseIPRouter = $"{adresseIP.Split(".")[0]}.{adresseIP.Split(".")[1]}.{adresseIP.Split(".")[2]}.33";
                    reply = await sender.SendPingAsync(adresseIPRouter, timeout, buffer, options);
                    clock.Stop();
                    if (reply.Status == IPStatus.Success)
                    {
                        fileLogger.Log($"Ping routeur {reply.Address} : réussite ({clock.ElapsedMilliseconds/1000.0}s)");
                        fileLogger.Log($"Arrêt appel : défaut ping serveur (2011)");
                        if (printConsole) { Console.WriteLine("--- Ping Router OK"); }
                    }
                    else
                    {
                        fileLogger.Log($"Ping routeur {adresseIP} : échec");
                        fileLogger.Log($"Arrêt appel : défaut ping routeur (2011)");
                        if (printConsole) { Console.WriteLine("--- Ping Router NOK"); }
                    }
                }
                catch (Exception ex)
                {
                    fileLogger.Log($"Ping routeur {adresseIP} : exception");
                    Console.WriteLine($"\n--- Error when pinging router {adresseIP}");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }

                return false;
            }
            catch (Exception ex)
            {
                fileLogger.Log($"Ping serveur {adresseIP} : exception");
                Console.WriteLine($"\n--- Error when pinging server {adresseIP}");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }
    }
}

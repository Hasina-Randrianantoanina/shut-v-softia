using TestConnexion.Log;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;

namespace TestConnexion.Utils
{
    public static class PingUtils
    {
        public static async Task<bool> TryPingStation(string adresseIP, FileLogger fileLogger, bool printConsole)
        {
            Regex patternAdresseIP = new Regex(@"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$");
            if (!patternAdresseIP.IsMatch(adresseIP))
            {
                fileLogger.Log($"Arrêt appel : adresse IP {adresseIP} n'a pas le bon format");
                Console.WriteLine($"\n--- AdresseIP does not match the expected pattern");
                return false;
            }

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
                    string[] partsOfAdresseIP = adresseIP.Split("."); 
                    string adresseIPRouter = $"{partsOfAdresseIP[0]}.{partsOfAdresseIP[1]}.{partsOfAdresseIP[2]}.33";
                    reply = await sender.SendPingAsync(adresseIPRouter, timeout, buffer, options);
                    clock.Stop();
                    if (reply.Status == IPStatus.Success)
                    {
                        fileLogger.Log($"Ping routeur {reply.Address} : réussite ({clock.ElapsedMilliseconds/1000.0}s)");
                        fileLogger.Log($"Arrêt appel : défaut ping serveur (erreur 2011)");
                        if (printConsole) { Console.WriteLine("--- Ping Router OK"); }
                    }
                    else
                    {
                        fileLogger.Log($"Ping routeur {adresseIP} : échec");
                        fileLogger.Log($"Arrêt appel : défaut ping routeur (erreur 2011)");
                        if (printConsole) { Console.WriteLine("--- Ping Router NOK"); }
                    }
                }
                catch (Exception ex)
                {
                    fileLogger.LogException(ex, $"Ping routeur {adresseIP}", 2011);
                    PrintUtils.PrintConsoleException(ex, $"Error when pinging router {adresseIP}");
                }

                return false;
            }
            catch (Exception ex)
            {
                fileLogger.LogException(ex, $"Ping serveur {adresseIP}", 2011);
                PrintUtils.PrintConsoleException(ex, $"Error when pinging server {adresseIP}");
                return false;
            }
        }
    }
}

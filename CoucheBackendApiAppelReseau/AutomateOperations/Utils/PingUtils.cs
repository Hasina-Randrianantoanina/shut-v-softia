using AutomateOperations.Logger;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace AutomateOperations.Utils
{
    public static class PingUtils
    {
        public static async Task<(bool statusPing, Erreur? erreur)> TryPingStation(string? adresseIP, FileLogger fileLogger)
        {
            Regex patternAdresseIP = new Regex(@"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$");
            if (adresseIP == null || !patternAdresseIP.IsMatch(adresseIP))
            {
                Console.WriteLine($"\n--- AdresseIP n'a pas le bon format");
                return (false, ErrorUtils.HandlePingError(100, null, adresseIP));
            }

            using Ping sender = new Ping();

            try
            {
                Stopwatch clock = Stopwatch.StartNew();
                // Try ping server
                PingReply reply = await sender.SendPingAsync(adresseIP);
                clock.Stop();
                if (reply.Status == IPStatus.Success)
                {

                    fileLogger.Log($"Ping serveur {reply.Address} : réussite ({clock.ElapsedMilliseconds/1000.0}s)");
                    return (true, null);
                }

                // Try ping router, always return false
                string[] partsOfAdresseIP = adresseIP.Split(".");
                string adresseIPRouter = $"{partsOfAdresseIP[0]}.{partsOfAdresseIP[1]}.{partsOfAdresseIP[2]}.33";
                try
                {
                    clock = Stopwatch.StartNew();
                    reply = await sender.SendPingAsync(adresseIPRouter);
                    clock.Stop();
                    if (reply.Status == IPStatus.Success)
                    {
                        fileLogger.Log($"Ping routeur {reply.Address} : réussite ({clock.ElapsedMilliseconds/1000.0}s)");
                        return (false, ErrorUtils.HandlePingError(110, null, adresseIP)); // Error server
                    }
                    else
                    {
                        return (false, ErrorUtils.HandlePingError(120, null, adresseIPRouter)); // Error router
                    }
                }
                catch (Exception ex)
                {
                    return (false, ErrorUtils.HandlePingError(121, ex, adresseIPRouter)); // Exception router
                }
            }
            catch (Exception ex)
            {
                return (false, ErrorUtils.HandlePingError(111, ex, adresseIP));// Exception server
            }
        }
    }
}

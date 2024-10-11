using StenOperations.Logger;
using StenOperations.Models.Entities;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace StenOperations.Models.Commands
{
    public class COMCommand: DefaultCommand
    {
        public string FtpAddress { get; set; }
        public int FtpTimeout { get; set; }

        public COMCommand(ILogger logger, string? ftpAddress, int ftpTimeout = 1000)
        : base(logger, new Station(""))
        {
            if (string.IsNullOrEmpty(ftpAddress))
            {
                throw new ArgumentException($"« {nameof(ftpAddress)} » ne peut pas être vide ou avoir la valeur Null.", nameof(ftpAddress));
            }
            FtpAddress = ftpAddress;
            FtpTimeout = ftpTimeout;
        }

        public override void Execute()
        {
            using (var ping = new Ping())
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var pingReplay = ping.Send(FtpAddress, FtpTimeout);
                if (pingReplay.Status != IPStatus.Success)
                {
                    Logger.Log("ping (essai 2) {0}", FtpAddress);
                    pingReplay = ping.Send(FtpAddress, FtpTimeout);
                    if (pingReplay.Status != IPStatus.Success) 
                    {
                        ErrorCode = 2011;
                        ErrorText = string.Format("défaut ping serveur {0}", FtpAddress);
                        var ipv4 = FtpAddress.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                        if (ipv4.Length >= 4) { ipv4[3] = "33"; }
                        var ipRouter = string.Join('.', ipv4);

                        pingReplay = ping.Send(ipRouter, FtpTimeout);
                        if (pingReplay.Status != IPStatus.Success)
                        {
                            ErrorCode = 2011;
                            ErrorText = string.Format("défaut ping routeur {0}", ipRouter);
                        }
                    }
                }
                stopWatch.Stop();
                Logger.Log("ping {0} {1}s", FtpAddress, stopWatch.Elapsed.TotalSeconds);

                if (ErrorCode == 0)
                {
                    Context.Operation = "GETSTATUS";
                }
                else
                {
                    Context.Operation = "FERMER";
                    Logger.Log(ErrorText);
                }
            }
        }

        public override async Task ExecuteAsync()
        {
            using (var ping = new Ping())
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var pingReplay = await ping.SendPingAsync(FtpAddress, FtpTimeout);
                if (pingReplay.Status != IPStatus.Success)
                {
                    Logger.Log("ping (essai 2) {0}", FtpAddress);
                    pingReplay = ping.Send(FtpAddress, FtpTimeout);
                    if (pingReplay.Status != IPStatus.Success)
                    {
                        ErrorCode = 2011;
                        ErrorText = string.Format("défaut ping serveur {0}", FtpAddress);
                        var ipv4 = FtpAddress.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                        if (ipv4.Length >= 4) { ipv4[3] = "33"; }
                        var ipRouter = string.Join('.', ipv4);

                        pingReplay = await ping.SendPingAsync(ipRouter, FtpTimeout);
                        if (pingReplay.Status != IPStatus.Success)
                        {
                            ErrorCode = 2011;
                            ErrorText = string.Format("défaut ping routeur {0}", ipRouter);
                        }
                    }
                }
                stopWatch.Stop();
                Logger.Log("ping {0} {1}s", FtpAddress, stopWatch.Elapsed.TotalSeconds);

                if (ErrorCode == 0)
                {
                    Context.Operation = "GETSTATUS";
                }
                else
                {
                    Context.Operation = "FERMER";
                    Logger.Log(ErrorText);
                }
            }
        }
    }
}

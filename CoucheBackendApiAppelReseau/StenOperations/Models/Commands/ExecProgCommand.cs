
using StenOperations.Logger;
using StenOperations.Models.Entities;
using System.Diagnostics;

namespace StenOperations.Models.Commands
{
    /// <summary>
    /// Note: la property ErrorCode contient la valeur de retour du processus exécutant le programme,
    /// et ErrorText contient l'erreur s'il y en a.
    /// </summary>
    public class ExecProgCommand: DefaultCommand
    {
        public string Filename { get; set; }
        public string Args { get; set; }

        public ExecProgCommand(ILogger logger, Station station, string filename, string args)
            : base(logger, station)
        {
            Filename = filename;
            Args = args;
        }

        public override void Execute()
        {
            Logger.Log($"-> Executing: {Filename} {Args}");
            var procInfo = new ProcessStartInfo
            {
                FileName = Filename,
                Arguments = Args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var proc = new Process { StartInfo = procInfo }) 
            {
                proc.Start();
                proc.WaitForExit();

                var output = proc.StandardOutput.ReadToEnd();                
                var error = proc.StandardError.ReadToEnd();
                Logger.Log($"{output}\nProcess Exit Code: {proc.ExitCode}");
                ErrorCode = proc.ExitCode;
                if (!string.IsNullOrEmpty(error)) 
                {
                    ErrorText = error;
                    Logger.Log(error);
                }
            }
        }
    }
}

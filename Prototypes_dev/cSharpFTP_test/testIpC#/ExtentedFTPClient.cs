using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using FluentFTP;

namespace testIpC_
{
    public class ExtendedFTPClient
    {
        private static IConfiguration Configuration { get; set; } = null!;

        public static void Run()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var ftpSettings = Configuration.GetSection("FtpSettings").Get<FtpConfig>();

            if (ftpSettings == null)
            {
                throw new InvalidOperationException("FTP settings could not be loaded from configuration.");
            }

            using (FtpClient client = new FtpClient(ftpSettings.Server, ftpSettings.User, ftpSettings.Password))
            {
                client.Port = ftpSettings.Port;
                client.Config.DataConnectionType = FtpDataConnectionType.AutoActive;
                client.Config.LogToConsole = true;

                try
                {
                    // Connexion au serveur FTP
                    client.Connect();
                    Console.WriteLine("Connecté au serveur FTP");

                    GetStatus(client);
                    GenerateRequestFile(); 
                    ExecuteFtpScript(ftpSettings);  
                    GetResult(client);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Erreur : " + e.Message);
                    if (e.InnerException != null)
                    {
                        Console.WriteLine("Inner Exception : " + e.InnerException.Message);
                    }
                    Console.WriteLine(e.StackTrace);
                }
                finally
                {

                    if (client.IsConnected)
                    {
                        client.Disconnect();
                        Console.WriteLine("Déconnecté du serveur FTP");
                    }
                }
            }
        }

        private static void GetStatus(FtpClient client)
        {
            Console.WriteLine("Téléchargement de STATUS.TXT...");
            FtpStatus status = client.DownloadFile("STATUS.TXT", "STATUS.TXT", FtpLocalExists.Overwrite);
            if (status == FtpStatus.Success)
            {
                Console.WriteLine("STATUS.TXT téléchargé avec succès");
            }
            else
            {
                throw new IOException($"Échec du téléchargement de STATUS.TXT. Status: {status}");
            }
        }

        private static void GenerateRequestFile()
        {
            Console.WriteLine("Génération du fichier REQUETE.TXT...");

            DateTime aujourdhui = DateTime.Now;
            DateTime dixJoursAvant = aujourdhui.AddDays(-1);
            string contenu = $"DEB={dixJoursAvant:dd/MM/yyyy}\nFIN={aujourdhui:dd/MM/yyyy}\n";

            string filePath = "./REQUETE.TXT";
            
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            File.WriteAllText(filePath, contenu);

            Console.WriteLine("REQUETE.TXT généré avec succès.");
        }

        private static void ExecuteFtpScript(FtpConfig ftpSettings)
        {
            string scriptPath = Path.Combine(Path.GetTempPath(), "ftp_script.sh");

            string ftpScript = $@"
#!/bin/bash
HOST={ftpSettings.Server}
USER={ftpSettings.User}
PASSWD={ftpSettings.Password}
FILE=REQUETE.TXT
LOCAL=./upload/REQUETE.TXT

ftp -inv $HOST <<EOF
user $USER $PASSWD
passive off
put $LOCAL $FILE
bye
EOF
";
            File.WriteAllText(scriptPath, ftpScript);
            
            MakeFileExecutable(scriptPath);
            
            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = scriptPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = processInfo })
            {
                process.Start();
                
                // Lire la sortie et l'erreur standard
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    Console.WriteLine("Script exécuté avec succès:\n" + output);
                }
                else
                {
                    Console.WriteLine("Erreur lors de l'exécution du script:\n" + error);
                }
            }

            // Supprimer le script après utilisation
            File.Delete(scriptPath);
        }

        private static void MakeFileExecutable(string filePath)
        {
            var processInfo = new ProcessStartInfo("chmod", $"+x {filePath}")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(processInfo))
            {
                process.WaitForExit();
            }
        }

        private static void GetResult(FtpClient client)
        {
            Console.WriteLine("Téléchargement de RESULT.BIN...");

            if (client.FileExists("RESULT.BIN"))
            {
                FtpStatus status = client.DownloadFile("RESULT.BIN", "RESULT.BIN", FtpLocalExists.Overwrite);
                if (status == FtpStatus.Success)
                {
                    Console.WriteLine("RESULT.BIN téléchargé avec succès");
                }
                else
                {
                    throw new IOException($"Échec du téléchargement de RESULT.BIN. Status: {status}");
                }
            }
            else
            {
                Console.WriteLine("RESULT.BIN non trouvé.");
                throw new FileNotFoundException("RESULT.BIN non trouvé sur le serveur");
            }
        }
    }
}

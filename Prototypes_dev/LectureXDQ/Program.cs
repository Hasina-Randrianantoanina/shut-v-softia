using LectureXDQ.Entities;
using LectureXDQ.Utils;
using System.Xml;

namespace LectureXDQ
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // ------------------ Choose the initiales -------------------
            const string Initiales = "194";
            const string XDQDirectory = @"C:\shut-refonte\Prototypes_dev\LectureXDQ\Sources";
            const bool PrintConsole = true;
            DateTime appel = DateTime.Now; // Args

            // --------- Do not change anything after this ---------------
            try
            {
                // -------- Step 1 : Get station and files --------
                // Get files
                string[]? filesFullNames = FileUtils.GetSourceFilesFullNames(XDQDirectory, Initiales, PrintConsole);

                // Get Station
                Station? station = await DataBaseUtils.GetStation(Initiales);
                if (filesFullNames == null || station == null)
                {
                    Console.WriteLine($"Aborting reading : no files found or station {Initiales} unknown");
                    return;
                }
                else if (station.Enregistreur.TypeLiaison != null && !station.Enregistreur.TypeLiaison.Equals("XX"))
                {
                    Console.WriteLine($"Aborting reading : liaison type {station.Enregistreur.TypeLiaison} not supported for station {Initiales}");
                    return;
                }
                station.VoiesInternes = await DataBaseUtils.GetVoiesInternes(station.Id);
                if (station.VoiesInternes.Count == 0) 
                {   
                    Console.WriteLine($"Aborting reading : station {Initiales} does not have any voies internes");
                    return;
                }

                DefautsActifsUtils.AddInactiveDefaut(station.VoiesInternes, appel);

                station.PrintStation();
                if (PrintConsole) { PrintUtils.PrintISODAQConfigVoiesInternes(station.VoiesInternes); }

                List<Mesure> mesuresOfThisStation = new List<Mesure>();

                // For each file => reading to get List<Mesure>
                foreach (string filefullName in filesFullNames)
                {
                    Console.WriteLine($"\n------ Reading file {filefullName}...");

                    List<Mesure> mesuresOfThisFile = new List<Mesure>();

                    XmlDocument? document = XMLUtils.GetDocument(filefullName);
                    if (document == null) { return; }

                    // ------------ Step 2 : Get enregistreur version ------------ 
                    string version = ISODAQUtils.GetEnregistreurVersion(document);
                    station.Enregistreur.Version = version;
                    if (PrintConsole) { Console.WriteLine($"version = {version}"); }

                    // Voies
                    XmlNodeList? voiesXML = XMLUtils.GetVoies(document);
                    if (voiesXML == null)
                    {
                        Console.WriteLine($"Aborting reading for this file : file does not have any voies internes");
                        continue;
                    }

                    // For each voie => reading the reference table and calculating the mesures
                    for (int i = 0; i < voiesXML.Count; i++)
                    {
                        XmlNode voieXML = voiesXML[i]!;
                        Console.WriteLine($"\n------ Reading voie n°{i+1}...");

                        // ------------ Step 3 : Get Mesures for each voie -----------
                        List<Mesure>? mesuresOfThisVoie = ISODAQUtils.GetMesures(voieXML, station.VoiesInternes[i], PrintConsole);
                        if (mesuresOfThisVoie == null) { continue; }

                        mesuresOfThisFile.AddRange(mesuresOfThisVoie);
                    }

                    PrintUtils.PrintMesures(mesuresOfThisFile);
                    mesuresOfThisStation.AddRange(mesuresOfThisFile); // Mesures with defauts (9999)

                } // End foreach file

                // ------------------ Step 4 : Handle defauts ----------------
                // Update VoieInterne.DefautActif
                DefautsActifsUtils.ReadDefautsActifs(station.VoiesInternes, mesuresOfThisStation); // Update VoieInterne.DefautActif
                
                // Update or delete defaut.defauts_actifs
                await DefautsActifsUtils.UpdateOrDeleteDefautsActifs(station.VoiesInternes, appel, PrintConsole);

            }
            catch  (Exception ex)
            {
                if (ex is ArgumentNullException || ex is NullReferenceException)
                {
                    PrintUtils.PrintConsoleException(ex, "Unexpected null exception");
                }
                PrintUtils.PrintConsoleException(ex, "????"); // Dev
            }

        } // End main
    }
}

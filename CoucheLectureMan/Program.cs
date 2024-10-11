using CoucheLectureMan.Entities;
using CoucheLectureMan.Utils;

namespace CoucheLectureMan
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // ----------- Choose the initiales & date -------------

            List<string> initiales = FileUtils.GetStationsAutomatePremium();
            initiales.AddRange(FileUtils.GetStationsAutomateM580D15());
            initiales.AddRange(FileUtils.GetStationsAutomateM580D16());
            string callDateStr = "01/01/2022 05:20:00";

            // ------ Do not change anything after this ------------
            DateTime callDate = DateTime.Parse(callDateStr);
            List<Station> stations = new List<Station>();

            for (int i = 0; i < initiales.Count; i++) // Loop on each station
            {
                Console.WriteLine($"\n------ Analyzing station {initiales[i]}");
                Station station = DataBaseUtils.GetStation(initiales[i]);
                if (station == null) 
                {
                    Console.WriteLine($"Skipping the station {initiales[i]}");
                        continue;
                }
                
                stations.Add(station);

                // Input files => man files
                string[] sourceFilesFullNames = FileUtils.GetSourceFilesFullNames(station.Initiales, callDate);
                if (sourceFilesFullNames == null) continue;

                for (int j = 0; j < sourceFilesFullNames.Length; j++) // Loop on each man file of this station
                {
                    // ----------------------- ISODAQ ----------------------
                    if (station.TypeLiaison == "?")
                    {
                        // TODO : ISODAQProtocol

                    }

                    // --------------------- STEN --------------------------
                    else if (station.TypeLiaison == "IP")
                    {
                        // TODO : STENProtocol

                    }

                    // --------------------- Automate ----------------------
                    else if (station.TypeLiaison == "AP")
                    {
                        // Step n°1 : getting the full texte split in lines
                        string[] lines = FileUtils.ReadAllText(sourceFilesFullNames[j]);
                        if (lines == null) continue;

                        // Step n°1 : comparing the analogic configuration
                        List<VoieInterne> unexpectedVoieInternes = AutomateManFileUtils.GetAnalogicConfigFromManFile(station, lines);
                        if (unexpectedVoieInternes.Count > 0) PrintUtils.PrintAutomateConfigVoiesInternes(unexpectedVoieInternes);

                        // Step n°2 : reading the mesures
                        List<Mesure> mesuresOfThisFile = AutomateManFileUtils.GetMesuresFromManFile(station, lines);
                        if (mesuresOfThisFile.Count > 0) {
                            // Modif ValeurALEchelle with Virgule
                            AutomateManFileUtils.UpdateValeurALEchelle(station.VoiesInternes, mesuresOfThisFile);
                            // Adding the mesures
                            station.Mesures.AddRange(mesuresOfThisFile);
                            // Print
                            PrintUtils.PrintMesures(mesuresOfThisFile, true, sourceFilesFullNames[j]);
                        }

                    }
                } // End loop on each man file of this station
            } // End loop on each station
        }
    }
}

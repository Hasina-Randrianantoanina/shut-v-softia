using TestConnexion.Entities;
using TestConnexion.Log;

namespace TestConnexion.Utils
{
    public static class AutomateCsvFileUtils
    {
        /// <summary>
        /// If date inversions, we keep the datas closer to the end of the file<br/>
        /// ex :<br/>
        /// 8h20<br/>
        /// 8h35<br/>
        /// 8h40<br/>
        /// 8h30<br/>
        /// 9h<br/>
        /// 8h30 after 8h40 shows an inversion => we keep 8h30 and we delete 8h35 and 8h40
        /// </summary>
        public static List<Mesure> DeleteDateInversions(List<Mesure> mesures, bool printConsole, bool printInfoDetails)
        {
            if(mesures.Count == 0) { return mesures; }

            if (printConsole) { Console.WriteLine("\n--- Looking for date inversions..."); }

            DateTime dateOfLastValidMesure = DateTime.MinValue;
            List<Mesure> invalidMesures = new List<Mesure>();

            for (int i = 0; i < mesures.Count; i++)
            {
                if (mesures[i].Time < dateOfLastValidMesure)
                {
                    // Inversion => looking for the invalid mesures to delete later
                    for (int j = i-1; j >= 0; j--)
                    {
                        if (mesures[j].Time > mesures[i].Time)
                        {
                            invalidMesures.Add(mesures[j]);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                dateOfLastValidMesure = mesures[i].Time;
            }

            if (invalidMesures.Count == 0)
            {
                if (printConsole) { Console.WriteLine($"No inversion found, 0 Mesures has been deleted"); }
            }
            else
            {
                // Deleting the invalid mesures
                foreach (Mesure invalidMesure in invalidMesures)
                {
                    if (printConsole) { PrintUtils.PrintOneMesure(invalidMesure, printInfoDetails); }
                    mesures.Remove(invalidMesure);
                }
                if (printConsole) { Console.WriteLine($"{invalidMesures.Count} Mesures has been deleted"); }
            }

            return mesures;
        }

        public static async Task<bool> TryWriteCsvFile(Station station, string csvFileFullName, List<Mesure> mesures, FileLogger fileLogger, bool printConsole)
        {
            try 
            {
                await using StreamWriter streamWriter = new StreamWriter(csvFileFullName, true);

                // First : header
                string header = "Horodate;NumeroVoie;Info;Type;Valeur\n";
                if (!(await FileUtils.TryWriteAsync(streamWriter, header))) 
                {
                    Console.WriteLine($"\n------ End of writing : errors when writing the header of the csv file");
                    return false;
                }

                // Second : mesures
                foreach (Mesure mesure in mesures)
                {
                    string info = ConversionUtils.IntToStringHexa4(mesure.Info.Value);
                    string mesureStr = $"{mesure.Time.ToString()};{mesure.NumeroVoie};{info};{mesure.Info.TypeMesure};{mesure.ValeurALEchelle}\n";
                    if (!(await FileUtils.TryWriteAsync(streamWriter, mesureStr)))
                    {
                        Console.WriteLine($"\n------ End of writing : errors when writing in the csv file : {mesureStr}");
                        return false;
                    }
                }

                if (printConsole) { Console.WriteLine($"\n------ End of writing : successfully wrote {csvFileFullName}"); }
                return true;
            }
            catch (Exception ex)
            {
                fileLogger.LogException(ex, "Ecriture des mesures (Ecriture dans le fichier)", -13);
                PrintUtils.PrintConsoleException(ex, "Error when trying to write the mesures (writing into the file)");
                return false;
            }
        }
    }
}

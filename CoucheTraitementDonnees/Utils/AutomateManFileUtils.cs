using CoucheTraitementDonnees.Entities;
using CoucheTraitementDonnees.Entities.Infos;

namespace CoucheTraitementDonnees.Utils
{
    public static class AutomateManFileUtils
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
        public static List<Mesure> DeleteDateInversions(List<Mesure> mesures, bool printInfoDetails)
        {
            Console.WriteLine("\n--- Looking for date inversions...");

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
                Console.WriteLine($"No inversion found, 0 Mesures has been deleted");
            }
            else
            {
                // Deleting the invalid mesures
                foreach (Mesure invalidMesure in invalidMesures)
                {
                    PrintUtils.PrintOneMesure(invalidMesure, printInfoDetails);

                    mesures.Remove(invalidMesure);
                }

                Console.WriteLine($"{invalidMesures.Count} Mesures has been deleted");
            }

            return mesures;
        }

        public static async Task<bool> TryWriteHeader(string manFileFullName, Station station, List<Mesure> mesures)
        {
            try
            {
                StreamWriter streamWriter = new StreamWriter(manFileFullName, true);
                string line;
                bool lineSuccessfullyWrite;

                // Voies internes
                foreach (VoieInterne voieInterne in station.VoiesInternes)
                {
                    if (voieInterne.NumeroVoie > 32) return false; // Règle métier

                    string sign = voieInterne.Virgule > 0 ? "-" : "";
                    line = $"#{GetCodeVoie(voieInterne.NumeroVoie)}\t{voieInterne.Libelle}\t{sign}{voieInterne.Virgule}\n";
                    lineSuccessfullyWrite = await FileUtils.TryWriteAsync(streamWriter, line);

                    if (lineSuccessfullyWrite == false) return false;
                }

                // D (Début de fichier de vidage)
                line = $"D{mesures[0].Time.ToString("dd/MM/yyyy")}  00h00  {station.Numero.ToString("000")}\n";
                lineSuccessfullyWrite = await FileUtils.TryWriteAsync(streamWriter, line);
                if (lineSuccessfullyWrite == false) return false;

                // E (Changement de jour)
                line = $"E{mesures[0].Time.ToString("dd/MM/yyyy")}  {GetNumberOfEvents(mesures).ToString("0000")}\n";
                lineSuccessfullyWrite = await FileUtils.TryWriteAsync(streamWriter, line);
                if (lineSuccessfullyWrite == false) return false;

                streamWriter.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n--- Error when writing the file {manFileFullName}");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

        public static async Task<bool> TryWriteMesures(string manFileFullName, Station station, List<Mesure> mesures)
        {
            try
            {
                StreamWriter streamWriter = new StreamWriter(manFileFullName, true);
                List<Mesure> mesuresOnThisLine = new List<Mesure>();
                string line;
                bool lineSuccessfullyWrite;

                foreach (Mesure mesure in mesures)
                {
                    int indexLastMesure = mesuresOnThisLine.Count -1;

                    if (indexLastMesure < 0 || isAnalogicMesure(mesuresOnThisLine[indexLastMesure]) && isAnalogicMesure(mesure) && mesuresOnThisLine[indexLastMesure].Time == mesure.Time)
                    {
                        // What if there are multiple analogic record on the same min ?
                        // Empty list or mesures in the list and current mesure are analogic and were recorded at the same time
                        mesuresOnThisLine.Add(mesure);
                        continue;
                    }

                    // Step 1 : Writing the line
                    line = CreateLine(mesuresOnThisLine, station.VoiesInternes);

                    lineSuccessfullyWrite = await FileUtils.TryWriteAsync(streamWriter, line);
                    if (lineSuccessfullyWrite == false) return false;

                    // Step 2 : adding the current mesure in a new list
                    mesuresOnThisLine = new List<Mesure> { mesure };
                }

                // Step 3 : Writing the last mesure
                line = CreateLine(mesuresOnThisLine, station.VoiesInternes);

                lineSuccessfullyWrite = await FileUtils.TryWriteAsync(streamWriter, line);
                if (lineSuccessfullyWrite == false) return false;

                streamWriter.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n--- Error when writing the file {manFileFullName}");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

        public static async Task<bool> TryWriteLastLine(string manFileFullName, List<Mesure> mesures)
        {
            try
            {
                StreamWriter streamWriter = new StreamWriter(manFileFullName, true);
                string line;
                bool lineSuccessfullyWrite;

                // F (Fin de fichier de vidage)
                line = $"F{mesures[mesures.Count -1].Time.ToString("dd/MM/yyyy  HH'h'mm")}\n";
                lineSuccessfullyWrite = await FileUtils.TryWriteAsync(streamWriter, line);
                if (lineSuccessfullyWrite == false) return false;

                streamWriter.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n--- Error when writing the file {manFileFullName}");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

        private static string CreateLine(List<Mesure> mesuresOnThisLine, List<VoieInterne> voiesInternes)
        {
            string line;
            Mesure lastMesure = mesuresOnThisLine[mesuresOnThisLine.Count-1];

            if (isAnalogicMesure(lastMesure))
            {
                line = CreateNewLinesAnalogic(mesuresOnThisLine, voiesInternes);
            }
            else if (lastMesure.Info.TypeMesure.Equals("TC"))
            {
                line = CreateNewLineTC(mesuresOnThisLine);
            }
            else if (lastMesure.Info.TypeMesure.Equals("TS"))
            {
                line = CreateNewLineTS(mesuresOnThisLine);
            }
            else if (lastMesure.Info.TypeMesure.Contains("ECH"))
            {
                line = CreateNewLineEchelle(mesuresOnThisLine, voiesInternes);
            }
            else
            {
                line = "--- Error : Unknown TypeMesure";
            }

            return line + "\n";
        }

        /// <summary>
        /// E = 1<br/>
        /// TS/TC/ECH = 1<br/>
        /// CAL/CAP = 1<br/>
        /// DebutDefaut (CAL/CAP) = 1<br/> 
        /// FintDefaut (CAL/CAP) = 1<br/> 
        /// End of line Analogic with Positiv value = 1 (except the last line of the file)<br/>
        /// End of line Analogic with Negativ value = 1 (except the last line of the file)<br/>
        /// </summary>
        /// <param name="mesures"></param>
        /// <returns></returns>
        private static int GetNumberOfEvents(List<Mesure> mesures)
        {
            int numberOfEvents = 0;
            List<Mesure> currentLine = new List<Mesure>();

            numberOfEvents++;// E (Changement de jour) count as 1 event

            foreach (Mesure mesure in mesures)
            {
                numberOfEvents++; // Mesure event
                bool newLine = false;
                int indexLastMesure = currentLine.Count - 1;

                // End of analogic line event 
                if (indexLastMesure >= 0 && isAnalogicMesure(currentLine[indexLastMesure]))
                {
                    if (isAnalogicMesure(mesure) == false)
                    {
                        // Last mesure is analogic and the current is not => end of line(s)
                        if (isThereAPositivAnalogicLine(currentLine)) numberOfEvents++;
                        if (isThereANegativAnalogicLine(currentLine)) numberOfEvents++;

                        currentLine = new List<Mesure> { mesure };
                        continue;
                    }
                    else if (currentLine[indexLastMesure].Time != mesure.Time)
                    {
                        // Last mesure is analogic but older than the current => end of line(s)
                        // Works too if the time are inverted in the binary file
                        if (isThereAPositivAnalogicLine(currentLine)) numberOfEvents++;
                        if (isThereANegativAnalogicLine(currentLine)) numberOfEvents++;
                        newLine = true;
                    }
                }

                // Defaut analogic event
                if (mesure.Info.TypeMesure.Equals("CAL"))
                {
                    InfoAnalogicCalcul infoAnalogicCalcul = (InfoAnalogicCalcul)mesure.Info;
                    if (infoAnalogicCalcul.DebutDefaut) numberOfEvents++;
                    if (infoAnalogicCalcul.FinDefaut) numberOfEvents++;
                }
                else if (mesure.Info.TypeMesure.Equals("CAP"))
                {
                    InfoAnalogicCapteur infoAnalogicCapteur = (InfoAnalogicCapteur)mesure.Info;
                    if (infoAnalogicCapteur.DebutDefaut) numberOfEvents++;
                    if (infoAnalogicCapteur.FinDefaut) numberOfEvents++;
                }

                if (newLine)
                {
                    currentLine = new List<Mesure> { mesure };
                }
                else
                {
                    currentLine.Add(mesure);
                }
            }

            return numberOfEvents;
        }

        /// <summary>
        /// a9999 b21XX = debut defaut voie XX+1<br/>
        /// a9999 b20XX = fin defaut voie XX+1<br/>
        /// J = enregistrement a minuit<br/>
        /// </summary>
        /// <param name="mesuresOnThisLine"></param>
        /// <param name="voiesInternes"></param>
        /// <returns></returns>
        private static string CreateNewLinesAnalogic(List<Mesure> mesuresOnThisLine, List<VoieInterne> voiesInternes)
        {
            // x lines of defauts
            string newLineAnalogic = GetLinesWithDefauts(mesuresOnThisLine);

            // 1 line for positiv value + 1 line for negativ value
            newLineAnalogic += GetLinesWithValues(mesuresOnThisLine, voiesInternes);

            return newLineAnalogic;
        }

        /// <summary>
        /// </summary>
        /// <param name="mesuresOnThisLine"></param>
        /// <returns>string with all the defauts lines</returns>
        private static string GetLinesWithDefauts(List<Mesure> mesuresOnThisLine)
        {
            string defautsLines = "";
            bool debutDefaut = false;
            bool finDefaut = false;
            foreach (Mesure mesure in mesuresOnThisLine)
            {
                if (mesure.Info.TypeMesure.Equals("CAL"))
                {
                    InfoAnalogicCalcul infoAnalogicCalcul = (InfoAnalogicCalcul)mesure.Info;
                    if (infoAnalogicCalcul.DebutDefaut) debutDefaut = true;
                    if (infoAnalogicCalcul.FinDefaut) finDefaut = true;
                }
                else
                {
                    InfoAnalogicCapteur infoAnalogicCapteur = (InfoAnalogicCapteur)mesure.Info;
                    if (infoAnalogicCapteur.DebutDefaut) debutDefaut = true;
                    if (infoAnalogicCapteur.FinDefaut) finDefaut = true;
                }

                int numeroVoieMod = mesure.NumeroVoie -1; // Historically used as an index in the defaut detection by ASNet
                if (debutDefaut) defautsLines += $"${mesure.Time.ToString("HH'h'mm")} a9999 b21{numeroVoieMod.ToString("00")}\n";
                if (finDefaut) defautsLines +=  $"${mesure.Time.ToString("HH'h'mm")} a9999 b20{numeroVoieMod.ToString("00")}\n";

                debutDefaut = false;
                finDefaut = false;
            }
            return defautsLines;
        }

        /// <summary>
        /// </summary>
        /// <param name="mesuresOnThisLine"></param>
        /// <returns>string with 2 lines : positiv values and negativ values</returns>
        private static string GetLinesWithValues(List<Mesure> mesuresOnThisLine, List<VoieInterne> voiesInternes)
        {
            string lineWithPositivValue = "";
            string lineWithNegativValue = "";
            string time = mesuresOnThisLine[0].Time.ToString("HH'h'mm");

            foreach (Mesure mesure in mesuresOnThisLine)
            {
                // Calculating valeurALEchelle
                int virgule = 0;
                foreach (VoieInterne voieInterne in voiesInternes)
                {
                    if (voieInterne.NumeroVoie == mesure.NumeroVoie)
                    {
                        virgule = voieInterne.Virgule;
                        break;
                    }
                }

                double valeurALEchelle = Math.Round(Math.Pow(10, virgule) * mesure.ValeurALEchelle);
                if (valeurALEchelle > 9999) valeurALEchelle = 9998; // 9999 can not be used (code défaut)

                // Adding the mesure in the line
                if (valeurALEchelle >= 0)
                {
                    // Positiv value
                    lineWithPositivValue += $" {GetCodeVoie(mesure.NumeroVoie)}{valeurALEchelle.ToString("0000")}";
                }
                else
                {
                    // Negativ value
                    lineWithNegativValue += $" {GetCodeVoie(mesure.NumeroVoie)}{Math.Abs(valeurALEchelle).ToString("0000")}";
                }
            }

            // Adding the beginning of each line
            bool isLineWithPositivValueEmpty = lineWithPositivValue.Equals("");
            bool isLineWithNegativValueEmpty = lineWithNegativValue.Equals("");
            string linesToReturn;

            if (isLineWithPositivValueEmpty == false) // + line not empty
            {
                string firstLetter = isThisEnregistrementAMinuit(mesuresOnThisLine[0]) ? "J" : " ";
                lineWithPositivValue = $"{firstLetter}{time}{lineWithPositivValue}";
            }

            if (isLineWithNegativValueEmpty == false) lineWithNegativValue = $"-{time}{lineWithNegativValue}"; // - line not empty


            if (isLineWithPositivValueEmpty || isLineWithNegativValueEmpty)
            {
                linesToReturn = $"{lineWithPositivValue}{lineWithNegativValue}";
            }
            else
            {
                linesToReturn = $"{lineWithPositivValue}\n{lineWithNegativValue}";
            }

            return linesToReturn;
        }

        /// <summary>
        /// $aXXXX b20 = apparition defaut battement<br/>
        /// $aXXXX b21 = disparition defaut battement<br/>
        /// o = EnregistrementAMinuit<br/>
        /// c = /
        /// </summary>
        /// <param name="mesure"></param>
        /// <returns></returns>
        private static string CreateNewLineTC(List<Mesure> mesuresOnThisLine)
        {
            string newLine;
            Mesure onlyMesureOnThisLine = mesuresOnThisLine[0]; // 1 mesure TC per line
            InfoTC infoTC = (InfoTC)onlyMesureOnThisLine.Info;

            double valeurALEchelle = Math.Round((double)onlyMesureOnThisLine.ValeurALEchelle);
            if (valeurALEchelle > 9999) valeurALEchelle = 9998; // 9999 can not be used (code défaut)

            if (infoTC.ApparitionDefautBattement)
            {
                newLine = $"${onlyMesureOnThisLine.Time.ToString("HH'h'mm")} a{onlyMesureOnThisLine.NumeroVoie.ToString("0000")} b2100";
            }
            else if (infoTC.DisparitionDefautBattement)
            {
                newLine = $"${onlyMesureOnThisLine.Time.ToString("HH'h'mm")} a{onlyMesureOnThisLine.NumeroVoie.ToString("0000")} b2000";
            }
            else
            {
                newLine = infoTC.EnregistrementAMinuit ? "o" : "c";
                newLine += $"{onlyMesureOnThisLine.Time.ToString("HH'h'mm")} a{onlyMesureOnThisLine.NumeroVoie.ToString("0000")} b{valeurALEchelle.ToString("0000")}";
            }

            return newLine;
        }

        /// <summary>
        /// $aXXXX b20 = apparition defaut battement<br/>
        /// $aXXXX b21 = disparition defaut battement<br/>
        /// r = EnregistrementAMinuit<br/>
        /// s = /
        /// </summary>
        /// <param name="mesure"></param>
        /// <returns></returns>
        private static string CreateNewLineTS(List<Mesure> mesuresOnThisLine)
        {
            string newLine;
            Mesure onlyMesureOnThisLine = mesuresOnThisLine[0]; // 1 mesure TS per line
            InfoTS infoTS = (InfoTS)onlyMesureOnThisLine.Info;

            double valeurALEchelle = Math.Round((double)onlyMesureOnThisLine.ValeurALEchelle);
            if (valeurALEchelle > 9999) valeurALEchelle = 9998; // 9999 can not be used (code défaut)

            if (infoTS.ApparitionDefautBattement)
            {
                newLine = $"${onlyMesureOnThisLine.Time.ToString("HH'h'mm")} a{onlyMesureOnThisLine.NumeroVoie.ToString("0000")} b2100";
            }
            else if (infoTS.DisparitionDefautBattement)
            {
                newLine = $"${onlyMesureOnThisLine.Time.ToString("HH'h'mm")} a{onlyMesureOnThisLine.NumeroVoie.ToString("0000")} b2000";
            }
            else
            {
                newLine = infoTS.EnregistrementAMinuit ? "r" : "s";
                newLine += $"{onlyMesureOnThisLine.Time.ToString("HH'h'mm")} a{onlyMesureOnThisLine.NumeroVoie.ToString("0000")} b{valeurALEchelle.ToString("0000")}";
            }

            return newLine;
        }

        /// <summary>
        /// k = valeur 0%<br/>
        /// m = valeur 100%
        /// </summary>
        /// <param name="mesuresOnThisLine"></param>
        /// <param name="voiesInternes"></param>
        /// <returns></returns>
        private static string CreateNewLineEchelle(List<Mesure> mesuresOnThisLine, List<VoieInterne> voiesInternes)
        {
            Mesure onlyMesureOnThisLine = mesuresOnThisLine[0]; // 1 mesure Echelle per line

            // NumeroVoie is in base 32 because (converted during the reading of the binary file)
            int numeroVoieBase32 = onlyMesureOnThisLine.NumeroVoie;

            // Calculating valeurALEchelle
            int virgule = 0;
            foreach (VoieInterne voieInterne in voiesInternes)
            {
                if (voieInterne.NumeroVoie == numeroVoieBase32)
                {
                    virgule = voieInterne.Virgule;
                    break;
                }
            }
            double valeurALEchelle = Math.Round(Math.Pow(10, virgule) * onlyMesureOnThisLine.ValeurALEchelle);
            if (valeurALEchelle > 9999) valeurALEchelle = 9998; // 9999 can not be used (code défaut)

            // Returning the string
            string firstLetter;
            if (onlyMesureOnThisLine.Info.TypeMesure.Equals("ECH100%"))
            {
                firstLetter = "m";
            }
            else if (onlyMesureOnThisLine.Info.TypeMesure.Equals("ECH0%"))
            {
                firstLetter = "k";
            }
            else
            {
                Console.WriteLine($"--- Error Unknow type of mesure : {onlyMesureOnThisLine.Info.TypeMesure}");
                firstLetter = "Error TypeMesure --- ";
            }
            return $"{firstLetter}{onlyMesureOnThisLine.Time.ToString("HH'h'mm")} {GetCodeVoie(numeroVoieBase32)}{valeurALEchelle.ToString("0000")}";
        }

        private static char GetCodeVoie(int numeroVoie)
        {
            char[] lastCodes = { '(', ')', '{', '}', '[', ']' };
            return (numeroVoie < 27) ? Convert.ToChar(96 + numeroVoie) : lastCodes[numeroVoie - 27];
        }

        private static bool isThereAPositivAnalogicLine(List<Mesure> mesures)
        {
            foreach (Mesure mesureInLine in mesures)
            {
                if (mesureInLine.ValeurALEchelle >= 0) return true;
            }
            return false;
        }

        private static bool isThereANegativAnalogicLine(List<Mesure> mesures)
        {
            foreach (Mesure mesureInLine in mesures)
            {
                if (mesureInLine.ValeurALEchelle < 0) return true;
            }
            return false;
        }

        private static bool isAnalogicMesure(Mesure mesure)
        {
            return mesure.Info.TypeMesure.Equals("CAL") || mesure.Info.TypeMesure.Equals("CAP");
        }

        private static bool isThisEnregistrementAMinuit(Mesure mesure)
        {
            if (mesure.Info.TypeMesure.Equals("CAL"))
            {
                InfoAnalogicCalcul infoAnalogicCalcul = (InfoAnalogicCalcul)mesure.Info;
                return infoAnalogicCalcul.EnregistrementAMinuit;
            }
            else
            {
                InfoAnalogicCapteur infoAnalogicCapteur = (InfoAnalogicCapteur)mesure.Info;
                return infoAnalogicCapteur.EnregistrementAMinuit;
            }
        }
    }
}

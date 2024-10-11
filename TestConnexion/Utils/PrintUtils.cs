using TestConnexion.Entities;

namespace TestConnexion.Utils
{
    public static class PrintUtils
    {
        public static void PrintAutomateConfigVoiesInternes(List<VoieInterne> voiesInternes)
        {
            Console.WriteLine("\n-------- Print Analogic Config --------\n");

            foreach (string configVoieInterneStr in GetAutomateConfigVoiesInternes(voiesInternes))
            {
                if (configVoieInterneStr == null) { continue; }
                Console.WriteLine(configVoieInterneStr);
            }

            Console.WriteLine("\n------ End Print Analogic Config ------");
        }

        public static void PrintAutomateRealTimeDatasVoiesInternes(List<VoieInterne> voiesInternes)
        {
            Console.WriteLine("\n-------- Print Analogic RT Datas --------\n");

            foreach (string realTimeDatasVoiesInternesStr in GetAutomateRealTimeDatasVoiesInternes(voiesInternes))
            {
                if (realTimeDatasVoiesInternesStr == null) { continue; }
                Console.WriteLine(realTimeDatasVoiesInternesStr);
            }

            Console.WriteLine("\n------ End Print Analogic RT Datas ------");
        }

        public static string[] GetAutomateConfigVoiesInternes(List<VoieInterne> voiesInternes, int startingIndex = 0)
        {
            string[] configVoiesInternesStr = new string[voiesInternes.Count];
            for (int i = 0; i < voiesInternes.Count; i++)
            {
                if (voiesInternes[i].Info == 0) { continue; } // Voie does not exist
                configVoiesInternesStr[i] = $"Voie ANA {i+startingIndex+1}, {voiesInternes[i].AdresseBES}, {voiesInternes[i].Info}";
            }
            return configVoiesInternesStr;
        }

        public static string[] GetAutomateRealTimeDatasVoiesInternes(List<VoieInterne> voiesInternes, int startingIndex = 0)
        {
            string[] realTimeDatasVoiesInternesStr = new string[voiesInternes.Count];
            for (int i = 0; i < voiesInternes.Count; i++)
            {
                if (voiesInternes[i].Info == 0) { continue; } // Voie does not exist
                realTimeDatasVoiesInternesStr[i] = $"Voie ANA {i+startingIndex+1}, {voiesInternes[i].SeuilBas}";
            }
            return realTimeDatasVoiesInternesStr;
        }

        public static void PrintMesures(List<Mesure> mesures, bool printInfoDetails)
        {
            Console.WriteLine("\n-------- Print Mesures --------\n");

            foreach (string mesureStr in GetMesures(mesures, printInfoDetails))
            {
                Console.WriteLine(mesureStr);
            }

            Console.WriteLine("\n------ End Print Mesures ------");
        }

        public static void PrintOneMesure(Mesure mesure, bool printInfoDetails)
        {
            Console.WriteLine(GetOneMesure(mesure, printInfoDetails));
        }

        /// <returns>String array of mesures</returns>
        public static string[] GetMesures(List<Mesure> mesures, bool printInfoDetails = false)
        {
            string[] mesuresStr = new string[mesures.Count];
            for (int i = 0; i < mesures.Count; i++)
            {
                string nomVoie = (mesures[i].NomVoie != null) ? $" {mesures[i].NomVoie}" : ""; // M580 version D16
                string numeroVoie = mesures[i].NumeroVoie.ToString("0000");
                string info = ConversionUtils.IntToStringHexa4(mesures[i].Info.Value);
                if (printInfoDetails)
                {
                    mesuresStr[i] = GetOneMesure(mesures[i], true);
                }
                else
                {
                    mesuresStr[i] = GetOneMesure(mesures[i], false);
                }
            }
            return mesuresStr;
        }

        private static string GetOneMesure(Mesure mesure, bool printInfoDetails)
        {
            string nomVoie = (mesure.NomVoie != null) ? $" {mesure.NomVoie}".Replace("\0", "") : ""; // M580 version D16
            string numeroVoie = mesure.NumeroVoie.ToString("0000");
            string info = ConversionUtils.IntToStringHexa4(mesure.Info.Value);
            string mesureStr;
            if (printInfoDetails)
            {
                mesureStr = $"{mesure.Time} {numeroVoie} {mesure.Info} {mesure.ValeurALEchelle}{nomVoie}";
            }
            else
            {
                mesureStr = $"{mesure.Time} {numeroVoie} {info} {mesure.Info.TypeMesure} {mesure.ValeurALEchelle}{nomVoie}";
            }
            return mesureStr;
        }

        public static void PrintDefauts(List<Defaut> defauts)
        {
            if (defauts.Count == 0) 
            {
                Console.WriteLine("\n-------- No Defauts to print --------\n");
                return;
            }

            Console.WriteLine("\n-------- Print Defauts --------\n");

            foreach (string defautStr in GetDefauts(defauts))
            {
                Console.WriteLine(defautStr);
            }

            Console.WriteLine("\n------ End Print Defauts ------");
        }

        public static string[] GetDefauts(List<Defaut> defauts)
        {
            string[] defautsStr = new string[defauts.Count];
            for (int i = 0; i < defauts.Count; i++)
            {
                Defaut defaut = defauts[i];
                string activeStr = defaut.Active ? "Actif" : "Inactif";
                bool instantDefaut = defaut.DebutDefaut == defaut.FinDefaut;
                string timeStr = $"début{(instantDefaut ? ", fin" : "")} : {defaut.DebutDefaut.ToString("dd/MM/yyyy HH'h'mm")}";
                if (defaut.FinDefaut.HasValue && !instantDefaut) {
                    timeStr += ", fin : " + ((DateTime)defaut.FinDefaut).ToString("dd/MM/yyyy HH'h'mm");
                }
                defautsStr[i] = $"{activeStr} : voie {defaut.NumeroVoie}, {timeStr}";
            }
            return defautsStr;
        }

        public static void PrintConsoleException(Exception ex, string message)
        {
            Console.WriteLine("\n--- " + message);
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }
}

using AutomateOperations.Entities;

namespace AutomateOperations.Utils
{
    public static class PrintUtils
    {
        public static string[] GetAutomateConfigVoiesTelemesurees(List<VoieTelemesuree> voiesTelemesurees, int startingIndex = 0)
        {
            string[] configVoiesTelemesureesStr = new string[voiesTelemesurees.Count];
            for (int i = 0; i < voiesTelemesurees.Count; i++)
            {
                if (voiesTelemesurees[i].Info == 0) { continue; } // Voie does not exist
                configVoiesTelemesureesStr[i] = $"Voie ANA {i+startingIndex+1}, {voiesTelemesurees[i].AdresseBES}, {voiesTelemesurees[i].Info}";
            }
            return configVoiesTelemesureesStr;
        }

        /// <returns>String array of mesures</returns>
        public static string[] GetMesures(List<Mesure> mesures)
        {
            string[] mesuresStr = new string[mesures.Count];
            for (int i = 0; i < mesures.Count; i++)
            {
                    mesuresStr[i] = GetOneMesure(mesures[i]);
            }
            return mesuresStr;
        }

        private static string GetOneMesure(Mesure mesure)
        {
            string nomVoie = (mesure.NomVoieM580 != null) ? $" {mesure.NomVoieM580}".Replace("\0", "") : ""; // M580 version D16
            string numeroVoie = mesure.NumeroVoie.ToString("0000");
            string info = ConversionUtils.IntToStringHexa4(mesure.Info.Value);

            return $"{mesure.Time} {numeroVoie} {info} {mesure.Info.TypeMesure} {mesure.ValeurALEchelle}{nomVoie}";
        }

        public static string[] GetDefauts(List<Defaut> defauts)
        {
            string[] defautsStr = new string[defauts.Count];
            for (int i = 0; i < defauts.Count; i++)
            {
                Defaut defaut = defauts[i];
                string activeStr = defaut.Actif ? "Actif" : "Inactif";
                string timeStr;
                if (defaut.DebutDefaut.HasValue) 
                {
                    timeStr = $"Début= {((DateTime)defaut.DebutDefaut).ToString("dd/MM/yyyy HH'h'mm")}"; 
                }
                else {
                    timeStr = "Début= null";
                }

                if (defaut.FinDefaut.HasValue)
                {
                    timeStr += ", Fin= " + ((DateTime)defaut.FinDefaut).ToString("dd/MM/yyyy HH'h'mm");
                }
                else 
                {
                    timeStr += ", Fin= null";
                }
                defautsStr[i] = $"{activeStr} : voie {defaut.VoieTelemesureeId}, {timeStr}";
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

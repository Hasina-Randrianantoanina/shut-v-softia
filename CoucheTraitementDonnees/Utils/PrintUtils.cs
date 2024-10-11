using CoucheTraitementDonnees.Entities;

namespace CoucheTraitementDonnees.Utils
{
    public static class PrintUtils
    {
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
        private static string[] GetMesures(List<Mesure> mesures, bool printInfoDetails = false)
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
            string nomVoie = (mesure.NomVoie != null) ? $" {mesure.NomVoie}" : ""; // M580 version D16
            string numeroVoie = mesure.NumeroVoie.ToString("0000");
            string info = ConversionUtils.IntToStringHexa4(mesure.Info.Value);
            string mesureStr;
            if (printInfoDetails)
            {
                mesureStr = $"{mesure.Time} {numeroVoie} {mesure.Info.ToString()} {mesure.ValeurALEchelle}{nomVoie}";
            }
            else
            {
                mesureStr = $"{mesure.Time} {numeroVoie} {info} {mesure.Info.TypeMesure} {mesure.ValeurALEchelle}{nomVoie}";
            }
            return mesureStr;
        }
    }
}

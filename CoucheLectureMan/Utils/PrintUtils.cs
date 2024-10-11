using CoucheLectureMan.Entities;

namespace CoucheLectureMan.Utils
{
    public static class PrintUtils
    {
        public static void PrintAutomateConfigVoiesInternes(List<VoieInterne> voiesInternes)
        {
            Console.WriteLine("\n-------- Print unexpected difference in Voies Analogic --------\n");

            for (int i = 0; i < voiesInternes.Count; i++)
            {
                Console.WriteLine($"Voie Analogic {voiesInternes[i].Libelle}, n°{voiesInternes[i].NumeroVoie}, virgule {voiesInternes[i].Virgule} ");
            }

            Console.WriteLine("\n------ End print unexpected differences in Voies Analogic ------");
        }

        public static void PrintMesures(List<Mesure> mesures, bool printInfoDetails, string manFileFullName)
        {
            Console.WriteLine($"\n-------- Print Mesures for the file {manFileFullName} --------\n");

            foreach (Mesure mesure in mesures)
            {
                PrintOneMesure(mesure, printInfoDetails);
            }

            Console.WriteLine($"\n------ End Print Mesures for the file {manFileFullName} ------");
        }

        private static void PrintOneMesure(Mesure mesure, bool printInfoDetails)
        {
            string nomVoie = (mesure.NomVoie != null) ? $" --- {mesure.NomVoie}" : ""; // M580 version D16
            string numeroVoie = mesure.NumeroVoie.ToString("0000");
            string info = ConversionUtils.IntToStringHexa4(mesure.Info.Value);

            if (printInfoDetails)
            {
                Console.WriteLine($"{mesure.Time} {numeroVoie} {mesure.Info.ToString()} {mesure.ValeurALEchelle}{nomVoie}");
            }
            else
            {
                Console.WriteLine($"{mesure.Time} {numeroVoie} {info} {mesure.Info.TypeMesure} {mesure.ValeurALEchelle}{nomVoie}");
            }
        }
    }
}

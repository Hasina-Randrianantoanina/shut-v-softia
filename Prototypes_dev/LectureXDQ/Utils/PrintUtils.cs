using LectureXDQ.Entities;

namespace LectureXDQ.Utils
{
    public static class PrintUtils
    {
        public static void PrintISODAQConfigVoiesInternes(List<VoieInterne> voiesInternes)
        {
            Console.WriteLine("\n-------- Print Analogic Config --------\n");

            foreach (string configVoieInterneStr in GetISODAQConfigVoiesInternes(voiesInternes))
            {
                if (configVoieInterneStr == null) { continue; }
                Console.WriteLine(configVoieInterneStr);
            }

            Console.WriteLine("\n------ End Print Analogic Config ------");
        }

        public static string[] GetISODAQConfigVoiesInternes(List<VoieInterne> voiesInternes, int startingIndex = 0)
        {
            string[] configVoiesInternesStr = new string[voiesInternes.Count];
            for (int i = 0; i < voiesInternes.Count; i++)
            {
                configVoiesInternesStr[i] = voiesInternes[i].ToString();
            }
            return configVoiesInternesStr;
        }

        public static void PrintMesures(List<Mesure> mesures)
        {
            Console.WriteLine("\n-------- Print Mesures --------\n");

            foreach (string mesureStr in GetMesures(mesures))
            {
                Console.WriteLine(mesureStr);
            }

            Console.WriteLine("\n------ End Print Mesures ------");
        }

        private static string[] GetMesures(List<Mesure> mesures)
        {
            string[] mesuresStr = new string[mesures.Count];
            for (int i = 0; i < mesures.Count; i++)
            {
                mesuresStr[i] = $"{mesures[i].Time} {mesures[i].NumeroVoie.ToString("0000")} {mesures[i].ValeurRelative}";
            }
            return mesuresStr;
        }

        public static void PrintConsoleException(Exception ex, string message)
        {
            Console.WriteLine("\n--- " + message);
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }
}

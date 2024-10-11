using Connexion.Entities;

namespace Connexion.Utils
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

    }
}

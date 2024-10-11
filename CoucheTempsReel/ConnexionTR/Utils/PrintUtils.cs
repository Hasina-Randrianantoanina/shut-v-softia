using ConnexionTR.Entities;

namespace ConnexionTR.Utils
{
    public class PrintUtils
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

        public static string[] GetAutomateConfigVoiesInternes(List<VoieInterne> voiesInternes, int startingIndex = 0)
        {
            string[] configVoiesInternesStr = new string[voiesInternes.Count];
            for (int i = 0; i < voiesInternes.Count; i++)
            {
                if (voiesInternes[i].Info == 0) { continue; } // Automate so voie does not exist
                //configVoiesInternesStr[i] = $"Voie ANA {i+startingIndex+1}, {voiesInternes[i].AdresseBES}, {voiesInternes[i].Info}";
                configVoiesInternesStr[i] = voiesInternes[i].ToString();
            }
            return configVoiesInternesStr;
        }

        public static void PrintConsoleException(Exception ex, string message)
        {
            Console.WriteLine("\n--- " + message);
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }
}

using System.Net.Sockets;
using ConnexionTR.Entities;
using ConnexionTR.Log;
using ConnexionTR.Utils;

namespace ConnexionTR.AutomateProtocol
{
    public class AutomatePremiumProtocol : ISpecificAutomateProtocol
    {
        private Station _station;
        private NetworkStream _stream;
        private FileLogger _fileLogger;

        public AutomatePremiumProtocol(Station station, NetworkStream stream, FileLogger fileLogger)
        {
            _station = station;
            _stream = stream;
            _fileLogger = fileLogger;
        }

        public async Task<bool> TryRead60AnalogicConfig(bool printConsole)
        {
            if (printConsole) { Console.WriteLine("\n--> Question n°2 : LECT CONFIG_ANA"); }
            _fileLogger.Log("LECT CONFIG VOIES ANALOGIQUES");

            // Creating the questions
            int adresseAna = 6000;
            List<VoieInterne> voiesInternes = new List<VoieInterne>();

            //60 voies = 4*117 mots + 72 mots => 5 commands
            for (int i = 0; i < 5; i++)
            {
                // Creating the question
                string nbMotsHexa = (i == 4) ? ConversionUtils.IntToStringHexa4(72) : ConversionUtils.IntToStringHexa4(117);
                string adresseAnaHexa = ConversionUtils.IntToStringHexa4(adresseAna);
                string questionStr = "0103" + adresseAnaHexa + nbMotsHexa;
                int motsToRead = int.Parse(questionStr.Substring(questionStr.Length-2, 2), System.Globalization.NumberStyles.HexNumber);
                adresseAna += 117;

                _fileLogger.Log($"{questionStr} ; lecture de {motsToRead} mots");

                try
                {
                    // Command
                    AutomateCommand automateCommand = new AutomateCommand();
                    automateCommand.Question = ConversionUtils.QuestionAutomateToByteArray(questionStr);

                    await automateCommand.SendQuestion(_stream, printConsole);
                    automateCommand.Response = await automateCommand.ReadResponse(_stream, printConsole);

                    _fileLogger.Log($"{ConversionUtils.ByteArrayToShortString(automateCommand.Response, motsToRead)} ; réponse automate");

                    // Adding VoieInterne
                    int startingIndex = i*13; // 13 voies per command
                    int endingIndex = (i == 4) ? startingIndex + 8 : startingIndex + 13; // Last command = 8 voies

                    List<VoieInterne> voiesInternesOfThisResponse = AutomateCommandUtils.GetAnalogicConfig(_station.Initiales, automateCommand.Response, startingIndex, endingIndex, 18); // 13 voies per command, 18bytes per voie

                    _fileLogger.LogAll(PrintUtils.GetAutomateConfigVoiesInternes(voiesInternesOfThisResponse, startingIndex));

                    voiesInternes.AddRange(voiesInternesOfThisResponse); // 13 voies per command; 18bytes per voie
                }
                catch (Exception ex)
                {
                    _fileLogger.LogException(ex, "Lecture config voies analogiques", -2);
                    PrintUtils.PrintConsoleException(ex, "Error when trying to read the analogic config");
                    return false;
                }
            }
            _station.VoiesInternesStation = voiesInternes;
            return true;
        }

    }
}

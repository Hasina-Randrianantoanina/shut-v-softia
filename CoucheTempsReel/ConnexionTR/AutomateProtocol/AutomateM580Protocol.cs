using System.Net.Sockets;
using ConnexionTR.Entities;
using ConnexionTR.Log;
using ConnexionTR.Utils;

namespace ConnexionTR.AutomateProtocol
{
    public class AutomateM580Protocol : ISpecificAutomateProtocol
    {
        private Station _station;
        private NetworkStream _stream;
        private FileLogger _fileLogger;

        public AutomateM580Protocol(Station station, NetworkStream stream, FileLogger fileLogger)
        {
            _station = station;
            _stream = stream;
            _fileLogger = fileLogger;
        }

        public async Task<bool> TryRead60AnalogicConfig(bool printConsole)
        {
            if (printConsole) { Console.WriteLine("\n--> Question n°2 : LECT CONFIG_ANA"); }
            _fileLogger.Log("LECT CONFIG VOIES ANALOGIQUES");

            int adresseAna = 6000;
            List<VoieInterne> voiesInternes = new List<VoieInterne>();

            // 60 voies = 5*120 mots => 5 commands
            for (int i = 0; i < 5; i++)
            {
                // Creating the question
                string nbMotsHexa = ConversionUtils.IntToStringHexa4(120);
                string adresseAnaHexa = ConversionUtils.IntToStringHexa4(adresseAna);
                string questionStr = "0103" + adresseAnaHexa + nbMotsHexa;
                int motsToRead = int.Parse(questionStr.Substring(questionStr.Length-2, 2), System.Globalization.NumberStyles.HexNumber);
                adresseAna += 120;

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
                    int startingIndex = i*12; // 12 voies per command
                    int endingIndex = startingIndex + 12;

                    List<VoieInterne> voiesInternesOfThisResponse = AutomateCommandUtils.GetAnalogicConfig(_station.Initiales, automateCommand.Response, startingIndex, endingIndex, 20);

                    _fileLogger.LogAll(PrintUtils.GetAutomateConfigVoiesInternes(voiesInternesOfThisResponse, startingIndex));

                    voiesInternes.AddRange(voiesInternesOfThisResponse); // 12 voies per command; 20bytes per voie
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
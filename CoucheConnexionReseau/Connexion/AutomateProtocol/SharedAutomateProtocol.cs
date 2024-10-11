using Connexion.Entities;
using Connexion.Log;
using Connexion.Utils;
using System.Net.Sockets;

namespace Connexion.AutomateProtocol
{
    public class SharedAutomateProtocol
    {
        private NetworkStream _stream;
        private FileLogger _fileLogger;

        public SharedAutomateProtocol(NetworkStream stream, FileLogger fileLogger)
        {
            _stream = stream;
            _fileLogger = fileLogger;
        }

        public async Task<bool> AreDatasAvailable(bool printConsole)
        {
            string questionStr = "010313920003";
            int motsToRead = int.Parse(questionStr.Substring(questionStr.Length-2, 2), System.Globalization.NumberStyles.HexNumber);

            if (printConsole) { Console.WriteLine("\n--> Question n°1 : AreDatasAvailable"); }

            _fileLogger.Log("LECT DISPONIBILITE DONNEES");

            AutomateCommand automateCommand = new AutomateCommand();
            automateCommand.Question = ConversionUtils.QuestionAutomateToByteArray(questionStr);

            _fileLogger.Log($"{questionStr} ; lecture de {motsToRead} mots");

            await automateCommand.SendQuestion(_stream, printConsole);
            automateCommand.Response = await automateCommand.ReadResponse(_stream, printConsole);

            _fileLogger.Log($"{ConversionUtils.ByteArrayToShortString(automateCommand.Response, motsToRead)} ; réponse automate");

            return AutomateCommandUtils.AreDatasAvailable(automateCommand.Response);
        }

        public async Task<string> ReadVersion(bool printConsole)
        {
            string questionStr = "010313740008";
            int motsToRead = int.Parse(questionStr.Substring(questionStr.Length-2, 2), System.Globalization.NumberStyles.HexNumber);

            if (printConsole) { Console.WriteLine("\n--> Question n°2 : LECT CONFIG_VER"); }

            _fileLogger.Log("LECT VERSION ENREGISTREUR");

            AutomateCommand automateCommand = new AutomateCommand();
            automateCommand.Question = ConversionUtils.QuestionAutomateToByteArray(questionStr);

            _fileLogger.Log($"{questionStr} ; lecture de {motsToRead} mots");

            await automateCommand.SendQuestion(_stream, printConsole);
            automateCommand.Response = await automateCommand.ReadResponse(_stream, printConsole);

            _fileLogger.Log($"{ConversionUtils.ByteArrayToShortString(automateCommand.Response, motsToRead)} ; réponse automate");

            return AutomateCommandUtils.GetVersion(automateCommand.Response);
        }

        public ISpecificAutomateProtocol GetSpecificAutomateProtocol(Station station, bool printConsole)
        {
            switch (station.Version)
            {
                case "MANQUE": // Premium
                    if (printConsole) { Console.WriteLine("\n------------- Switch to Premium protocol -------------"); }
                    return new AutomatePremiumProtocol(_stream, _fileLogger);
                case "D13A53Aa01": // Premium
                    if (printConsole) { Console.WriteLine("\n------------- Switch to Premium protocol -------------"); }
                    return new AutomatePremiumProtocol(_stream, _fileLogger);
                case "D15A55Ca01": // M580 (old version)
                    if (printConsole) { Console.WriteLine("\n------------- Switch to M580 protocol -------------"); }
                    return new AutomateM580Protocol(_stream, _fileLogger);
                case "D16A56Ca01": // M580 & M340
                    if (printConsole) { Console.WriteLine("\n------------- Switch to M580 protocol -------------"); }
                    return new AutomateM580Protocol(_stream, _fileLogger);
                default:
                    return null;
            }
        }

    }
}

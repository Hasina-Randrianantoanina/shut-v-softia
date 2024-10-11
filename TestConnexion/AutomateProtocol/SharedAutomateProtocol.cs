using System.Net.Sockets;
using TestConnexion.Entities;
using TestConnexion.Log;
using TestConnexion.Utils;

namespace TestConnexion.AutomateProtocol
{
    public class SharedAutomateProtocol
    {
        private Station _station;
        private NetworkStream _stream;
        private FileLogger _fileLogger;

        public SharedAutomateProtocol(Station station, NetworkStream stream, FileLogger fileLogger)
        {
            _station = station;
            _stream = stream;
            _fileLogger = fileLogger;
        }

        public async Task<bool> AreDatasAvailable(bool printConsole)
        {
            string questionStr = "010313920003";
            int motsToRead = int.Parse(questionStr.Substring(questionStr.Length-2, 2), System.Globalization.NumberStyles.HexNumber);

            if (printConsole) { Console.WriteLine("\n--> Question n°1 : AreDatasAvailable"); }

            _fileLogger.Log("LECT DISPONIBILITE DONNEES");

            try 
            {
                AutomateCommand automateCommand = new AutomateCommand();
                automateCommand.Question = ConversionUtils.QuestionAutomateToByteArray(questionStr);

                _fileLogger.Log($"{questionStr} ; lecture de {motsToRead} mots");

                await automateCommand.SendQuestion(_stream, printConsole);
                automateCommand.Response = await automateCommand.ReadResponse(_stream, printConsole);

                _fileLogger.Log($"{ConversionUtils.ByteArrayToShortString(automateCommand.Response, motsToRead)} ; réponse automate");

                return AutomateCommandUtils.AreDatasAvailable(automateCommand.Response);
            }
            catch (Exception ex)
            {
                _fileLogger.LogException(ex, "Lecture disponiblité données", -2);
                PrintUtils.PrintConsoleException(ex, "Error when trying to asking if there are available datas");
                return false;
            }
        }

        public async Task<bool> TryReadVersion(bool printConsole)
        {
            string questionStr = "010313740008";
            int motsToRead = int.Parse(questionStr.Substring(questionStr.Length-2, 2), System.Globalization.NumberStyles.HexNumber);

            if (printConsole) { Console.WriteLine("\n--> Question n°2 : LECT CONFIG_VER"); }
            _fileLogger.Log("LECT VERSION ENREGISTREUR");

            try
            {
                AutomateCommand automateCommand = new AutomateCommand();
                automateCommand.Question = ConversionUtils.QuestionAutomateToByteArray(questionStr);

                _fileLogger.Log($"{questionStr} ; lecture de {motsToRead} mots");

                await automateCommand.SendQuestion(_stream, printConsole);
                automateCommand.Response = await automateCommand.ReadResponse(_stream, printConsole);

                _fileLogger.Log($"{ConversionUtils.ByteArrayToShortString(automateCommand.Response, motsToRead)} ; réponse automate");

                _station.Version = AutomateCommandUtils.GetVersion(automateCommand.Response);
                if (_station.Version.Equals("")) { _station.Version = "MANQUE"; }
                return true;
            }
            catch (Exception ex)
            {
                _fileLogger.LogException(ex, "Lecture version enregistreur", -1);
                PrintUtils.PrintConsoleException(ex, "Error when trying to read the version");
                return false;
            }
        }

        public ISpecificAutomateProtocol GetSpecificAutomateProtocol(bool printConsole)
        {
            switch (_station.Version)
            {
                case "MANQUE": // Premium
                    if (printConsole) { Console.WriteLine("\n------------- Switch to Premium protocol -------------"); }
                    return new AutomatePremiumProtocol(_station, _stream, _fileLogger);
                case "D13A53Aa01": // Premium
                    if (printConsole) { Console.WriteLine("\n------------- Switch to Premium protocol -------------"); }
                    return new AutomatePremiumProtocol(_station, _stream, _fileLogger);
                case "D15A55Ca01": // M580 (old version)
                    if (printConsole) { Console.WriteLine("\n------------- Switch to M580 protocol -------------"); }
                    return new AutomateM580Protocol(_station, _stream, _fileLogger);
                case "D16A56Ca01": // M580 & M340
                    if (printConsole) { Console.WriteLine("\n------------- Switch to M580 protocol -------------"); }
                    return new AutomateM580Protocol(_station, _stream, _fileLogger);
                default:
                    return null;
            }
        }

    }
}


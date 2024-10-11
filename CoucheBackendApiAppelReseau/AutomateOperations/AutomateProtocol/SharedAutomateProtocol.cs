using System.Net.Sockets;
using AutomateOperations.Entities;
using AutomateOperations.Logger;
using AutomateOperations.Utils;

namespace AutomateOperations.AutomateProtocol
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

        public async Task<(bool statusDatasAvailability, Erreur? erreur)> AreDatasAvailable()
        {
            string questionStr = "010313920003";
            int motsToRead = int.Parse(questionStr.Substring(questionStr.Length-2, 2), System.Globalization.NumberStyles.HexNumber);

            _fileLogger.Log("LECT DISPONIBILITE DONNEES");

            AutomateCommand automateCommand = new AutomateCommand();
            automateCommand.Question = ConversionUtils.QuestionAutomateToByteArray(questionStr);

            _fileLogger.Log($"{questionStr} ; lecture de {motsToRead} mots");

            var resultSend = await automateCommand.SendQuestion(_stream);
            if (!resultSend.statusWriting) { return (false, resultSend.erreur); }

            var resultRead = await automateCommand.ReadResponse(_stream);
            if (resultRead.response != null) { automateCommand.Response = resultRead.response; }
            else { return (false, resultRead.erreur); }

            _fileLogger.Log($"{ConversionUtils.ByteArrayToShortString(automateCommand.Response, motsToRead)} ; réponse automate");

            try
            {
                bool areDatasAvailable = AutomateCommandUtils.AreDatasAvailable(automateCommand.Response);
                return (areDatasAvailable, null);
            }
            catch (Exception ex) 
            {
                return (false, ErrorUtils.HandleStatusError(400, ex));
            }
        }

        public async Task<(bool statusVersion, Erreur? erreur)> TryReadVersion()
        {
            string questionStr = "010313740008";
            int motsToRead = int.Parse(questionStr.Substring(questionStr.Length-2, 2), System.Globalization.NumberStyles.HexNumber);

            _fileLogger.Log("LECT VERSION ENREGISTREUR");

            AutomateCommand automateCommand = new AutomateCommand();
            automateCommand.Question = ConversionUtils.QuestionAutomateToByteArray(questionStr);

            _fileLogger.Log($"{questionStr} ; lecture de {motsToRead} mots");

            var resultSend = await automateCommand.SendQuestion(_stream);
            if (!resultSend.statusWriting) { return (false, resultSend.erreur); }

            var resultRead = await automateCommand.ReadResponse(_stream);
            if (resultRead.response != null) { automateCommand.Response = resultRead.response; }
            else { return (false, resultRead.erreur); }

            _fileLogger.Log($"{ConversionUtils.ByteArrayToShortString(automateCommand.Response, motsToRead)} ; réponse automate");

            try
            {
                _station.Enregistreur.Version = AutomateCommandUtils.GetVersion(automateCommand.Response);
                if (_station.Enregistreur.Version.Equals("")) { _station.Enregistreur.Version = "MANQUE"; }

                return (true, null);
            }
            catch (Exception ex) 
            {
                return (false, ErrorUtils.HandleStatusError(410, ex));
            }
        }

        public ISpecificAutomateProtocol GetSpecificAutomateProtocol()
        {
            switch (_station.Enregistreur.Version)
            {
                case "MANQUE": // Premium
                case "D13A53Aa01": // Premium
                    return new AutomatePremiumProtocol(_station, _stream, _fileLogger);
                case "D15A55Ca01": // M580 (old version)
                case "D16A56Ca01": // M580 & M340
                    return new AutomateM580Protocol(_station, _stream, _fileLogger);
                default:
                    return null;
            }
        }

    }
}


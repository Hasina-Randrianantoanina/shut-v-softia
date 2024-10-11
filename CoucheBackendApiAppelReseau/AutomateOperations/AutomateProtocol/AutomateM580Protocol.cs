using System.Diagnostics.Eventing.Reader;
using System.Net.Sockets;
using AutomateOperations.Entities;
using AutomateOperations.Logger;
using AutomateOperations.Utils;

namespace AutomateOperations.AutomateProtocol
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

        public async Task<(bool statusConfig, Erreur? erreur)> TryRead60AnalogicConfig()
        {
            _fileLogger.Log("LECT CONFIG VOIES ANALOGIQUES");

            // Creating the questions
            int adresseAna = 6000;
            List<VoieTelemesuree> voiesTelemesurees = new List<VoieTelemesuree>();

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

                // Command
                AutomateCommand automateCommand = new AutomateCommand();
                automateCommand.Question = ConversionUtils.QuestionAutomateToByteArray(questionStr);

                var resultSend = await automateCommand.SendQuestion(_stream);
                if (!resultSend.statusWriting) { return (false, resultSend.erreur); }

                var resultRead = await automateCommand.ReadResponse(_stream);
                if (resultRead.response != null) { automateCommand.Response = resultRead.response; }
                else { return (false, resultRead.erreur); }

                _fileLogger.Log($"{ConversionUtils.ByteArrayToShortString(automateCommand.Response, motsToRead)} ; réponse automate");

                // Adding VoieTelemesuree
                int startingIndex = i*12; // 12 voies per command
                int endingIndex = startingIndex + 12;

                try
                {
                    List<VoieTelemesuree> voiesTelemesureesOfThisResponse = AutomateCommandUtils.GetAnalogicConfig(automateCommand.Response, startingIndex, endingIndex, 20);

                    _fileLogger.LogAll(PrintUtils.GetAutomateConfigVoiesTelemesurees(voiesTelemesureesOfThisResponse, startingIndex));

                    voiesTelemesurees.AddRange(voiesTelemesureesOfThisResponse); // 12 voies per command; 20bytes per voie
                }
                catch (Exception ex)
                {
                    return (false, ErrorUtils.HandleStatusError(420, ex));
                }
            }

            // Update values
            foreach (VoieTelemesuree voieStation in voiesTelemesurees)
            {
                if (voieStation.Info == 0) { continue; }
                foreach (VoieTelemesuree voieBase in _station.VoiesTelemesurees)
                {
                    if (voieStation.Numero == voieBase.Numero)
                    {
                        voieBase.AdresseBES = voieStation.AdresseBES;
                        voieBase.Info = voieStation.Info;
                        voieBase.SeuilBas = voieStation.SeuilBas;
                        voieBase.SeuilHaut = voieStation.SeuilHaut;
                        voieBase.ValeurDelta = voieStation.ValeurDelta;
                        voieBase.Groupe = voieStation.Groupe;
                        continue;
                    }
                }
            }

            return (true, null);
        }

        public async Task<(bool statusGetDatas, Erreur? erreur)> TryGetDatas(string downloadDirectory)
        {
            _fileLogger.Log("LECT DONNEES PAR FTP");

            // FTP Server settings
            string host = _station.Enregistreur.AdresseIP!;
            string remoteDataDirectory = @"SDCA/DataStorage/";

            // Files to download 
            DateTime today;
            DateTime dayOfInterrest;
            if (_station.Enregistreur.TypeHeure.Equals("LOCALE"))
            {
                // Locale
                today = DateTime.Now;
                dayOfInterrest = _station.Enregistreur.DernierTransfert.ToLocalTime();
            }
            else
            {
                today = DateTime.Now.ToUniversalTime();
                dayOfInterrest = _station.Enregistreur.DernierTransfert;
            }
            List<string> namesOfFilesToDownload = new List<string>();
            string initiales = _station.Initiales!;
            if (_station.Initiales!.Equals("XY")) { initiales = "CS"; }  // Specific case : XY is the M580 D16 into station CS
            initiales = initiales.Replace("_test", "");

            while (dayOfInterrest.Date <= today.Date)
            {
                string nameOfFileToDownload = initiales + dayOfInterrest.ToString("ddMMyy");
                namesOfFilesToDownload.Add(nameOfFileToDownload);
                dayOfInterrest = dayOfInterrest.AddDays(1);
            }

            // Local settings
            string localDownloadDirectory = downloadDirectory;

            var resultDownload = await FTPStreamUtils.GetFileByFTP(host, remoteDataDirectory, localDownloadDirectory, namesOfFilesToDownload, _fileLogger);

            if (!resultDownload.statusDownload) { return (false, resultDownload.erreur); }

            _fileLogger.Log("Fin lect données par FTP");
            return (true, null);
        }

        public (List<Mesure>? mesures, Erreur? erreur) ReadMesuresInBinaryFile(string binaryFileFullName)
        {
            if (!File.Exists(binaryFileFullName)) { return (null, ErrorUtils.HandleReadingError(604, null, binaryFileFullName)); }

            switch (_station.Enregistreur.Version)
            {
                case "D15A55Ca01": // 4 lines per Mesure
                    return AutomateBinaryFileUtils.ReadAllMesuresM580(binaryFileFullName, 4);
                case "D16A56Ca01": // 5 lines per Mesure
                    return AutomateBinaryFileUtils.ReadAllMesuresM580(binaryFileFullName, 5);
                default: // Impossible case
                    return (null, ErrorUtils.HandleReadingError(605, null, _station.Enregistreur.Version));
            }
        }

    }
}
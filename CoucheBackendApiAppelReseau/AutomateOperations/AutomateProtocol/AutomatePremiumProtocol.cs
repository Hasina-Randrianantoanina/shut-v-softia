using System.Net.Sockets;
using AutomateOperations.Entities;
using AutomateOperations.Logger;
using AutomateOperations.Utils;

namespace AutomateOperations.AutomateProtocol
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

        public async Task<(bool statusConfig, Erreur? erreur)> TryRead60AnalogicConfig()
        {
            _fileLogger.Log("LECT CONFIG VOIES ANALOGIQUES");

            // Creating the questions
            int adresseAna = 6000;
            List<VoieTelemesuree> voiesTelemesurees = new List<VoieTelemesuree>();

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

                // Command
                AutomateCommand automateCommand = new AutomateCommand();
                automateCommand.Question = ConversionUtils.QuestionAutomateToByteArray(questionStr);

                var resultSend = await automateCommand.SendQuestion(_stream);
                if (!resultSend.statusWriting) { return (false, resultSend.erreur); }

                var resultRead = await automateCommand.ReadResponse(_stream);
                if (resultRead.response != null) { automateCommand.Response = resultRead.response; }
                else { return (false, resultRead.erreur); }

                _fileLogger.Log($"{ConversionUtils.ByteArrayToShortString(automateCommand.Response, motsToRead)} ; réponse automate");

                // Adding VoieInterne
                int startingIndex = i*13; // 13 voies per command
                int endingIndex = (i == 4) ? startingIndex + 8 : startingIndex + 13; // Last command = 8 voies

                try
                {
                    List<VoieTelemesuree> voiesTelemesureesOfThisResponse = AutomateCommandUtils.GetAnalogicConfig(automateCommand.Response, startingIndex, endingIndex, 18); // 13 voies per command, 18bytes per voie

                    _fileLogger.LogAll(PrintUtils.GetAutomateConfigVoiesTelemesurees(voiesTelemesureesOfThisResponse, startingIndex));

                    voiesTelemesurees.AddRange(voiesTelemesureesOfThisResponse); // 13 voies per command; 18bytes per voie
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
            // 1)  getStatusQuestion
            // 2)  startingTransfertQuestion
            // 3)  getStatusQuestion + readDatasQuestion [+ continueTransfertQuestion)] => n times
            // 4)  endingTransfertQuestion

            _fileLogger.Log("LECT DONNEES PAR TCP");

            string getStatusQuestionStr = "010313920003";
            string endingTransfertQuestionStr = "010613920002";
            string nextQuestionStr = getStatusQuestionStr;
            string status = string.Empty;
            string binaryFileToWriteName = $"Result_{_station.Initiales.Replace("_test", "")}.bin";

            bool writingResponseInBinaryFile = false;
            int dataReceived = 0;
            int dataWritten = 0;

            // Local settings
            string binaryFileToWriteFullName = $"{downloadDirectory}{binaryFileToWriteName}";

            var resultDCFile = FileUtils.TryDeleteCreateFile(binaryFileToWriteFullName);
            if (!resultDCFile.statusDCFile) { return (false, resultDCFile.erreur); }


            try
            {
                while (!status.Equals("00") && !status.Equals("10") && !nextQuestionStr.Equals("Unknown case"))
                {
                    // Creating the command
                    AutomateCommand automateCommand = new AutomateCommand();
                    automateCommand.Question = ConversionUtils.QuestionAutomateToByteArray(nextQuestionStr);

                    int motsToRead;
                    bool writingQuestion = nextQuestionStr[3] == '6';
                    if (writingQuestion)
                    {
                        // Writing 1 mot
                        _fileLogger.Log($"{nextQuestionStr} ; écriture de 1 mot");
                        motsToRead = 1;
                    }
                    else
                    {
                        // Reading
                        motsToRead = int.Parse(nextQuestionStr.Substring(nextQuestionStr.Length-2, 2), System.Globalization.NumberStyles.HexNumber);
                        _fileLogger.Log($"{nextQuestionStr} ; lecture de {motsToRead} mots");
                    }

                    // Sending question
                    var resultSend = await automateCommand.SendQuestion(_stream);
                    if (!resultSend.statusWriting) { return (false, resultSend.erreur); }

                    // Reading response
                    var resultRead = await automateCommand.ReadResponse(_stream);
                    if (resultRead.response != null) { automateCommand.Response = resultRead.response; }
                    else { return (false, resultRead.erreur); }

                    _fileLogger.Log($"{ConversionUtils.ByteArrayToShortString(automateCommand.Response, motsToRead, writingQuestion)} ; réponse automate");

                    if (nextQuestionStr.Equals(endingTransfertQuestionStr)) { break; } // No data left

                    // Getting the status of the datas
                    var resultStatus = AutomateCommandUtils.GetStatusOfDatas(automateCommand.Response, nextQuestionStr, status);

                    if (resultStatus.status == null) { return (false, resultStatus.erreur); }
                    status = resultStatus.status;
                    if (nextQuestionStr.Equals(getStatusQuestionStr)) { _fileLogger.Log($"status {status}"); }

                    // Calculating the next question
                    var resultNextQuestion = AutomateCommandUtils.CalculateNextQuestionStr(automateCommand.Response, status!, writingResponseInBinaryFile);
                    if (resultNextQuestion.nextQuestion == null) { return (false, resultNextQuestion.erreur); }
                    nextQuestionStr = resultNextQuestion.nextQuestion;


                    // Writing datas in the binary file
                    if (writingResponseInBinaryFile)
                    {
                        int nbMotsToWrite = automateCommand.Question[11];
                        dataReceived += nbMotsToWrite/6; // 1 mesure = 6 mots = 12 bytes
                        var resultWriting = AutomateBinaryFileUtils.TryWriteDatasInBinaryFile(binaryFileToWriteFullName, automateCommand, nbMotsToWrite);
                        if (!resultWriting.statusWriting) { return (false, resultWriting.erreur); }
                        dataWritten = dataReceived;

                        // -------- TEMP --------
                        if (dataWritten % 100 == 0) { Console.WriteLine($"{dataWritten} mesures écrites"); }
                        // -------- /TEMP --------

                        writingResponseInBinaryFile = false;
                        continue;
                    }

                    if ((status!.Equals("12") || status.Equals("13")) && !nextQuestionStr.Equals(getStatusQuestionStr))
                    {
                        // Next response will be datas
                        writingResponseInBinaryFile = true;
                    }
                }
            } // End while
            catch (Exception ex)
            {
                return (false, ErrorUtils.HandleWritingError(504, ex));
            }

            _fileLogger.Log($"Fin lecture données par TCP, {dataWritten}/{dataReceived} mesures écrites");
            return (true, null);
        }

        public (List<Mesure>? mesures, Erreur? erreur) ReadMesuresInBinaryFile(string binaryFileFullName)
        {
            if (!File.Exists(binaryFileFullName)) { return (null, ErrorUtils.HandleReadingError(604, null, binaryFileFullName)); }
            
            return AutomateBinaryFileUtils.ReadAllMesuresPremium(binaryFileFullName);
        }

    }
}

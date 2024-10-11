using Connexion.Entities;
using Connexion.Log;
using Connexion.Utils;
using System.Net.Sockets;

namespace Connexion.AutomateProtocol
{
    public class AutomatePremiumProtocol : ISpecificAutomateProtocol
    {
        private NetworkStream _stream;
        private FileLogger _fileLogger;
        public AutomatePremiumProtocol(NetworkStream stream, FileLogger fileLogger)
        {
            _stream = stream;
            _fileLogger = fileLogger;
        }

        public async Task<List<VoieInterne>> Read60AnalogicConfig(bool printConsole)
        {
            if (printConsole) { Console.WriteLine("\n--> Question n°3 : LECT CONFIG_ANA"); }

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

                // Command
                AutomateCommand automateCommand = new AutomateCommand();
                automateCommand.Question = ConversionUtils.QuestionAutomateToByteArray(questionStr);

                await automateCommand.SendQuestion(_stream, printConsole);
                automateCommand.Response = await automateCommand.ReadResponse(_stream, printConsole);

                _fileLogger.Log($"{ConversionUtils.ByteArrayToShortString(automateCommand.Response, motsToRead)} ; réponse automate");

                // Adding VoieInterne
                int startingIndex = i*13; // 13 voies per command
                int endingIndex = (i == 4) ? startingIndex + 8 : startingIndex + 13; // Last command = 8 voies

                List<VoieInterne> voiesInternesOfThisResponse = AutomateCommandUtils.GetAnalogicConfig(automateCommand.Response, startingIndex, endingIndex, 18); // 13 voies per command, 18bytes per voie

                _fileLogger.LogAll(PrintUtils.GetAutomateConfigVoiesInternes(voiesInternesOfThisResponse, startingIndex));

                voiesInternes.AddRange(voiesInternesOfThisResponse); // 13 voies per command; 18bytes per voie

            }
            return voiesInternes;
        }

        public async Task Read60AnalogicRealTimeDatas(List<VoieInterne> voiesInternes, bool printConsole)
        {
            if (printConsole) { Console.WriteLine("\n--> Question n°4 : LECT VALTR_ANA"); }

            _fileLogger.Log("LECT VALEURS TR ANALOGIQUES");

            // Creating the questions
            int adresseAna = 4500;
            Byte[][] questions = new Byte[1][];

            // 60 voies * 2 mots = 120 mots per command
            for (int i = 0; i < 1; i++)
            {
                // Creating the question
                string nbMotsHexa = ConversionUtils.IntToStringHexa4(120);
                string adresseAnaHexa = ConversionUtils.IntToStringHexa4(adresseAna);
                string questionStr = "0103" + adresseAnaHexa + nbMotsHexa;
                int motsToRead = int.Parse(questionStr.Substring(questionStr.Length-2, 2), System.Globalization.NumberStyles.HexNumber);
                adresseAna += 120;

                _fileLogger.Log($"{questionStr} ; lecture de {motsToRead} mots");

                AutomateCommand automateCommand = new AutomateCommand();
                automateCommand.Question = ConversionUtils.QuestionAutomateToByteArray(questionStr);

                await automateCommand.SendQuestion(_stream, printConsole);
                automateCommand.Response = await automateCommand.ReadResponse(_stream, printConsole);

                _fileLogger.Log($"{ConversionUtils.ByteArrayToShortString(automateCommand.Response, motsToRead)} ; réponse automate");

                // Writing the values in the list
                AutomateCommandUtils.Get60AnalogicRealTimeDatas(automateCommand.Response, voiesInternes);

                _fileLogger.LogAll(PrintUtils.GetAutomateRealTimeDatasVoiesInternes(voiesInternes));
            }
        }

        public async Task GetDatas(Station station, int portFTP, bool printConsole, bool testMode, bool testVM)
        {
            // 1)  getStatusQuestion
            // 2)  startingTransfertQuestion
            // 3)  getStatusQuestion + readDatasQuestion [+ continueTransfertQuestion)] => n times
            // 4)  endingTransfertQuestion

            if (printConsole) { Console.WriteLine("\n--> Question n°5 : LECT DATA"); }

            _fileLogger.Log("LECT DONNEES PAR TCP");

            Byte[] getStatusQuestion = ConversionUtils.QuestionAutomateToByteArray("010313920003");
            Byte[] continueTransfertQuestion = ConversionUtils.QuestionAutomateToByteArray("010613920003");
            Byte[] endingTransfertQuestion = ConversionUtils.QuestionAutomateToByteArray("010613920002");
            Byte[] nextQuestion = getStatusQuestion;
            string status = null;
            string binaryFileToWriteName = "Result_" + station.Initiales + ".bin";
            bool writingResponseInBinaryFile = false;
            int dataReceived = 0;
            int dataWritten = 0;

            // Local settings
            string localDownloadDirectory = @"Downloads/" + station.Initiales + @"/"; // Dev

            if (printConsole) { Console.WriteLine($"\n------ Downloading datas by TCP into {localDownloadDirectory + binaryFileToWriteName}..."); }

            if (FileUtils.TryDeleteCreateFile(localDownloadDirectory + binaryFileToWriteName))
            {
                if (printConsole) { Console.WriteLine($"\nSuccessfully created {binaryFileToWriteName} in {localDownloadDirectory}"); }
            }
            else
            {
                if (printConsole) { Console.WriteLine($"\n--- Error when deleting and creating the file {binaryFileToWriteName}"); }
                return;
            }

            while (true)
            {
                // Creating the command, sending the question and reading the response
                AutomateCommand automateCommand = new AutomateCommand();
                automateCommand.Question = nextQuestion;

                await automateCommand.SendQuestion(_stream, printConsole);
                automateCommand.Response = await automateCommand.ReadResponse(_stream, printConsole);

                if (nextQuestion.SequenceEqual(endingTransfertQuestion))
                {
                    // There are no datas left, normal end of discussion
                    if (printConsole) { Console.WriteLine("\n------ There is no data left, end of discussion without error"); }
                    break;
                }

                // Getting the status of the datas
                if (nextQuestion.SequenceEqual(getStatusQuestion))
                {
                    status = AutomateCommandUtils.GetStatusOfDatas(automateCommand.Response);
                    if (printConsole) { Console.WriteLine($"New status = {status}"); }

                    if (status == null) { break; } // Error
                }
                else if (printConsole)
                {
                    Console.WriteLine($"Old status = {status}");
                }

                // Calculating the next question
                nextQuestion = AutomateCommandUtils.CalculateNextQuestion(automateCommand.Response, status);

                // Writing datas in the binary file
                if (writingResponseInBinaryFile)
                {
                    int nbMotsToWrite = automateCommand.Question[11];
                    bool successfullWrite = BinaryFileUtils.TryWriteDatasInBinaryFile(localDownloadDirectory + binaryFileToWriteName, automateCommand, nbMotsToWrite, printConsole);
                    dataReceived += nbMotsToWrite/6; // 1 mesure = 6 mots = 12 bytes

                    if (!successfullWrite) break; // Error
                    dataWritten = dataReceived;

                    // Overriding the next question
                    if (status.Equals("13"))
                    {
                        nextQuestion = continueTransfertQuestion; // In the automate, erasing these datas and sending the next
                    }
                    else if (status.Equals("12"))
                    {
                        nextQuestion = endingTransfertQuestion;  // In the automate, erasing these datas, end of transfert
                    }

                    writingResponseInBinaryFile = false;
                    continue;
                }

                if (status.Equals("12") || status.Equals("13") && !nextQuestion.SequenceEqual(getStatusQuestion))
                {
                    // Next response will be datas
                    writingResponseInBinaryFile = true;
                    if (printConsole) { Console.WriteLine("---The next response will be written in the file"); }
                }
                else if (status.Equals("00") || status.Equals("10"))
                {
                    // There are no datas, normal end of discussion
                    if (printConsole) { Console.WriteLine("\n------ There is no data, skipping download without error"); }
                    break;
                }
            }

            if (printConsole)
            {
                Console.WriteLine($"\nDatas received: {dataReceived}");
                Console.WriteLine($"Datas written: {dataWritten}");
            }
        }

    }
}

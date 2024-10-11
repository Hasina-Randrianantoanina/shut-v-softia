using System.Net.Sockets;
using TestConnexion.Entities;
using TestConnexion.Log;
using TestConnexion.Utils;

namespace TestConnexion.AutomateProtocol
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
                    _fileLogger.LogException(ex, "Lecture config voies analogiques", -3);
                    PrintUtils.PrintConsoleException(ex, "Error when trying to read the analogic config");
                    return false;
                }
            }
            _station.VoiesInternes = voiesInternes;
            return true;
        }

        public async Task<bool> TryRead60AnalogicRealTimeDatas(bool printConsole)
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

                try
                {
                    AutomateCommand automateCommand = new AutomateCommand();
                    automateCommand.Question = ConversionUtils.QuestionAutomateToByteArray(questionStr);

                    await automateCommand.SendQuestion(_stream, printConsole);
                    automateCommand.Response = await automateCommand.ReadResponse(_stream, printConsole);

                    _fileLogger.Log($"{ConversionUtils.ByteArrayToShortString(automateCommand.Response, motsToRead)} ; réponse automate");

                    // Writing the values in the list
                    AutomateCommandUtils.Get60AnalogicRealTimeDatas(automateCommand.Response, _station.VoiesInternes);

                    _fileLogger.LogAll(PrintUtils.GetAutomateRealTimeDatasVoiesInternes(_station.VoiesInternes));
                }
                catch (Exception ex)
                {
                    _fileLogger.LogException(ex, "Lecture valeurs TR analogiques", -4);
                    PrintUtils.PrintConsoleException(ex, "Error when trying to read the analogic real time datas");
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> TryGetDatas(bool printConsole)
        {
            // 1)  getStatusQuestion
            // 2)  startingTransfertQuestion
            // 3)  getStatusQuestion + readDatasQuestion [+ continueTransfertQuestion)] => n times
            // 4)  endingTransfertQuestion

            if (printConsole) { Console.WriteLine("\n--> Question n°5 : LECT DATA"); }

            _fileLogger.Log("LECT DONNEES PAR TCP");

            string getStatusQuestionStr = "010313920003";
            string continueTransfertQuestionStr = "010613920003";
            string endingTransfertQuestionStr = "010613920002";
            string nextQuestionStr = getStatusQuestionStr;
            string status = null;
            string binaryFileToWriteName = "Result_" + _station.Initiales + ".bin";
            bool writingResponseInBinaryFile = false;
            int dataReceived = 0;
            int dataWritten = 0;

            // Local settings
            string localDownloadDirectory = @"Downloads/" + _station.Initiales + @"/"; // Dev
            string binaryFileToWriteFullName = localDownloadDirectory + binaryFileToWriteName;

            if (printConsole) { Console.WriteLine($"\n------ Downloading datas by TCP into {localDownloadDirectory + binaryFileToWriteName}..."); }

            try
            {
                FileUtils.TryDeleteCreateFile(binaryFileToWriteFullName);
                if (printConsole) { Console.WriteLine($"\nSuccessfully created {binaryFileToWriteName} in {localDownloadDirectory}"); }
            }
            catch (Exception ex)
            {
                _fileLogger.LogException(ex, "Lecture données par TCP (Création du fichier binaire)", -6);
                PrintUtils.PrintConsoleException(ex, "Error when trying to download files by TCP (creating the file)");
                return false;
            }

            while (true)
            {
                try
                {
                    // Creating the command, sending the question and reading the response
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

                    await automateCommand.SendQuestion(_stream, printConsole);
                    automateCommand.Response = await automateCommand.ReadResponse(_stream, printConsole);

                    _fileLogger.Log($"{ConversionUtils.ByteArrayToShortString(automateCommand.Response, motsToRead, writingQuestion)} ; réponse automate");

                    if (nextQuestionStr.Equals(endingTransfertQuestionStr))
                    {
                        // There are no datas left, normal end of discussion
                        if (printConsole) { Console.WriteLine("\n------ There is no data left, end of discussion without error"); }
                        break;
                    }

                    // Getting the status of the datas
                    if (nextQuestionStr.Equals(getStatusQuestionStr))
                    {
                        status = AutomateCommandUtils.GetStatusOfDatas(automateCommand.Response);
                        if (printConsole) { Console.WriteLine($"New status = {status}"); }

                        if (status == null) { return false; } // Error
                        _fileLogger.Log($"status {status}");
                    }
                    else if (printConsole)
                    {
                        Console.WriteLine($"Old status = {status}");
                    }

                    // Calculating the next question
                    nextQuestionStr = AutomateCommandUtils.CalculateNextQuestionStr(automateCommand.Response, status!);

                    // Writing datas in the binary file
                    if (writingResponseInBinaryFile)
                    {
                        int nbMotsToWrite = automateCommand.Question[11];
                        dataReceived += nbMotsToWrite/6; // 1 mesure = 6 mots = 12 bytes
                        bool successfullWrite = AutomateBinaryFileUtils.TryWriteDatasInBinaryFile(binaryFileToWriteFullName, automateCommand, nbMotsToWrite, _fileLogger, printConsole);
                        if (!successfullWrite) { return false; } // Error
                        dataWritten = dataReceived;

                        // Overriding the next question
                        if (status!.Equals("13"))
                        {
                            nextQuestionStr = continueTransfertQuestionStr; // In the automate, erasing these datas and sending the next
                        }
                        else if (status.Equals("12"))
                        {
                            nextQuestionStr = endingTransfertQuestionStr;  // In the automate, erasing these datas, end of transfert
                        }

                        writingResponseInBinaryFile = false;
                        continue;
                    }

                    if (status!.Equals("12") || status.Equals("13") && !nextQuestionStr.Equals(getStatusQuestionStr))
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
                catch (Exception ex)
                {
                    _fileLogger.LogException(ex, "Lecture données par TCP (Echanges avec serveur)", -7);
                    PrintUtils.PrintConsoleException(ex, "Error when trying to download files by TCP (exchanges with server)");
                    return false;
                }
            } // End while

            if (printConsole)
            {
                Console.WriteLine($"\nDatas received: {dataReceived}");
                Console.WriteLine($"Datas written: {dataWritten}");
            }
            _fileLogger.Log($"Fin lecture données par TCP, {dataWritten}/{dataReceived} mesures écrites");
            return true;
        }

        public List<Mesure> GetMesuresFromBinaryFile(string binaryFileFullName, bool printConsole)
        {
            if (!File.Exists(binaryFileFullName))
            {
                Console.WriteLine($"\n--- {binaryFileFullName} is missing, no reading done");
                _fileLogger.Log($"Lecture de {binaryFileFullName} annulée : fichier n'existe pas");
                return new List<Mesure>();
            }

            if (printConsole) { Console.WriteLine($"\n------ Starting to read the binary file {binaryFileFullName}..."); }

            try
            {
                return AutomateBinaryFileUtils.ReadAllMesuresPremium(binaryFileFullName, _fileLogger, printConsole);
            }
            catch (Exception ex)
            {
                _fileLogger.LogException(ex, $"Lecture des mesures (Lecture du fichier binaire)", -11);
                PrintUtils.PrintConsoleException(ex, $"Error when reading the binary file {binaryFileFullName}");
                return null;
            }
        }

        public async Task<bool> TryWriteCsvFile(string binaryFileFullName, List<Mesure> mesures, bool printConsole)
        {
            // Output files
            string csvFileDirectory = $"Downloads/{_station.Initiales}/Csv";
            string csvFileName = Path.GetFileName(binaryFileFullName).Replace(".bin", ".csv");
            string csvFileFullName = @$"{csvFileDirectory}/{csvFileName}";

            if (mesures.Count == 0)
            {
                Console.WriteLine($"\n--- Creating of the csv file {csvFileName} aborted : there are no mesures to write");
                _fileLogger.Log($"Ecriture de {csvFileName} annulée : pas de mesures à écrire");
                return true; // Not an error
            }

            if (printConsole) { Console.WriteLine($"\n------ Creating the csv file {csvFileName}"); }

            // Creating the file
            try
            {
                FileUtils.TryDeleteCreateFile(csvFileFullName);
                if (printConsole) { Console.WriteLine($"\nSuccessfully created {csvFileName} in {csvFileDirectory}"); }
            }
            catch (Exception ex)
            {
                _fileLogger.LogException(ex, "Ecriture des mesures (Création du fichier csv)", -12);
                PrintUtils.PrintConsoleException(ex, "Error when trying to write the mesures (creating the file)");
                return false;
            }

            // Writing into the file
            return await AutomateCsvFileUtils.TryWriteCsvFile(_station, csvFileFullName, mesures, _fileLogger, printConsole);
        }
    }
}

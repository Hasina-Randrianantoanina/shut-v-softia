using System.Net.Sockets;
using TestConnexion.Entities;
using TestConnexion.Log;
using TestConnexion.Utils;

namespace TestConnexion.AutomateProtocol
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
            if (printConsole) { Console.WriteLine("\n--> Question n°3 : LECT CONFIG_ANA"); }
            _fileLogger.Log("LECT CONFIG VOIES ANALOGIQUES");

            int adresseAna = 6000;
            List<VoieInterne> voiesInternes = new List<VoieInterne>();

            // 60 voies = 6*100 mots => 6 commands
            for (int i = 0; i < 6; i++)
            {
                // Creating the question
                string nbMotsHexa = ConversionUtils.IntToStringHexa4(100);
                string adresseAnaHexa = ConversionUtils.IntToStringHexa4(adresseAna);
                string questionStr = "0103" + adresseAnaHexa + nbMotsHexa;
                int motsToRead = int.Parse(questionStr.Substring(questionStr.Length-2, 2), System.Globalization.NumberStyles.HexNumber);
                adresseAna += 100;

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
                    int startingIndex = i*10; // 10 voies per command
                    int endingIndex = startingIndex + 10;

                    List<VoieInterne> voiesInternesOfThisResponse = AutomateCommandUtils.GetAnalogicConfig(_station.Initiales, automateCommand.Response, startingIndex, endingIndex, 20);

                    _fileLogger.LogAll(PrintUtils.GetAutomateConfigVoiesInternes(voiesInternesOfThisResponse, startingIndex));

                    voiesInternes.AddRange(voiesInternesOfThisResponse); // 10 voies per command; 20bytes per voie
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
            if (printConsole) { Console.WriteLine("\n--> Question n°5 : LECT DATA"); }

            _fileLogger.Log("LECT DONNEES PAR FTP");

            // FTP Server settings
            string host = _station.AdresseIP!;
            string remoteDataDirectory = @"SDCA/DataStorage/";

            // Files to download 
            DateTime today = DateTime.Now;

            List<string> namesOfFilesToDownload = new List<string>();
            DateTime dayOfInterrest = _station.DernierTransfert;
            string initiales = _station.Initiales.Equals("XY") ? "CS" : _station.Initiales;  // Specific case : XY is the M580 D16 into station CS
            while (dayOfInterrest.Date <= today.Date)
            {
                if (printConsole) { Console.WriteLine(dayOfInterrest.ToShortDateString()); }
                string nameOfFileToDownload = initiales + dayOfInterrest.ToString("ddMMyy");
                namesOfFilesToDownload.Add(nameOfFileToDownload);
                dayOfInterrest = dayOfInterrest.AddDays(1);
            }

            // Local settings
            string localDownloadDirectory = @"Downloads/" + _station.Initiales + @"/"; // Dev

            if (printConsole) { Console.WriteLine($"\n------ Downloading fichier jour starting : {_station.DernierTransfert.ToShortDateString()}..."); }

            bool downloadSuccessfull = await FTPStreamUtils.GetFileByFTP(host, remoteDataDirectory, localDownloadDirectory, namesOfFilesToDownload, _fileLogger, printConsole);
            
            if (downloadSuccessfull) { _fileLogger.Log("Fin lect données par FTP"); }
            return downloadSuccessfull;
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
                switch (_station.Version)
                {
                    case "D15A55Ca01": // 4 lines per Mesure
                        return AutomateBinaryFileUtils.ReadAllMesuresM580(binaryFileFullName, 4, _fileLogger, printConsole);
                    case "D16A56Ca01": // 5 lines per Mesure
                        return AutomateBinaryFileUtils.ReadAllMesuresM580(binaryFileFullName, 5, _fileLogger, printConsole);
                    default: // Impossible case
                        return null;
                }
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
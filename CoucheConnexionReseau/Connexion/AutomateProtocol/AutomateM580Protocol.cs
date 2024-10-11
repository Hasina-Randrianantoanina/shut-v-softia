using Connexion.Entities;
using Connexion.Log;
using Connexion.Utils;
using System.Net.Sockets;

namespace Connexion.AutomateProtocol
{
    public class AutomateM580Protocol : ISpecificAutomateProtocol
    {
        private NetworkStream _stream;
        private FileLogger _fileLogger;

        public AutomateM580Protocol(NetworkStream stream, FileLogger fileLogger)
        {
            _stream = stream;
            _fileLogger = fileLogger;   
        }

        public async Task<List<VoieInterne>> Read60AnalogicConfig(bool printConsole)
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

                // Command
                AutomateCommand automateCommand = new AutomateCommand();
                automateCommand.Question = ConversionUtils.QuestionAutomateToByteArray(questionStr);

                await automateCommand.SendQuestion(_stream, printConsole);
                automateCommand.Response = await automateCommand.ReadResponse(_stream, printConsole);

                _fileLogger.Log($"{ConversionUtils.ByteArrayToShortString(automateCommand.Response, motsToRead)} ; réponse automate");

                // Adding VoieInterne
                int startingIndex = i*10; // 10 voies per command
                int endingIndex = startingIndex + 10;

                List<VoieInterne> voiesInternesOfThisResponse = AutomateCommandUtils.GetAnalogicConfig(automateCommand.Response, startingIndex, endingIndex, 20);

                _fileLogger.LogAll(PrintUtils.GetAutomateConfigVoiesInternes(voiesInternesOfThisResponse, startingIndex));
                
                voiesInternes.AddRange(voiesInternesOfThisResponse); // 10 voies per command; 20bytes per voie
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
            if (printConsole) { Console.WriteLine("\n--> Question n°5 : LECT DATA"); }

            _fileLogger.Log("LECT DONNEES PAR FTP");

            // FTP Server settings
            string host = station.AdresseIP;
            string remoteDataDirectory = @"SDCA/DataStorage/";

            // Files to download 
            DateTime today = DateTime.Now;
            if (testMode && !testVM) 
            {
                switch (station.Initiales)
                {
                    case "GA":
                        today = DateTime.Parse("01/01/2022"); // DL 31/12/21 & 01/01/22
                        break;
                    case "HE":
                        today = DateTime.Parse("01/01/2022"); // DL 01/01/22
                        break;
                    case "172":
                        today = DateTime.Parse("28/12/2021"); // DL 28/12/21
                        break;
                    case "178":
                        today = DateTime.Parse("29/12/2021"); // DL 29/12/21
                        break;
                }
            }

            List<string> namesOfFilesToDownload = new List<string>();
            DateTime dayOfInterrest = station.DernierTransfertFichiersJours;
            while (dayOfInterrest.Date <= today.Date)
            {
                if (printConsole) { Console.WriteLine(dayOfInterrest.ToShortDateString()); }
                string nameOfFileToDownload = station.Initiales + dayOfInterrest.ToString("ddMMyy");
                namesOfFilesToDownload.Add(nameOfFileToDownload);
                dayOfInterrest = dayOfInterrest.AddDays(1);
            }

            // Local settings
            string localDownloadDirectory = @"Downloads/" + station.Initiales + @"/"; // Dev

            if (printConsole) { Console.WriteLine($"\n------ Downloading fichier jour starting : {station.DernierTransfertFichiersJours.ToShortDateString()}..."); }

            await FTPStreamUtils.GetFileByFTP(host, portFTP, remoteDataDirectory, localDownloadDirectory, namesOfFilesToDownload, printConsole);
        }
    }
}
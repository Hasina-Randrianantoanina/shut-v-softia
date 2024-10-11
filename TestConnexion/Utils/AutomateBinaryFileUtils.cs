using TestConnexion.Entities.Infos;
using TestConnexion.Entities;
using System.Globalization;
using TestConnexion.Log;
using TestConnexion.AutomateProtocol;

namespace TestConnexion.Utils
{
    public static class AutomateBinaryFileUtils
    {
        /// <returns>True if the data has been successfully written</returns>
        public static bool TryWriteDatasInBinaryFile(string binaryFileToWriteFullName, AutomateCommand automateCommand, int nbMotsToWrite, FileLogger fileLogger, bool printConsole)
        {
            try
            {
                // Opening the streams
                using FileStream fileStream = File.Open(binaryFileToWriteFullName, FileMode.Append, FileAccess.Write);
                using BinaryWriter binaryStream = new BinaryWriter(fileStream);

                binaryStream.Write(automateCommand.Response, 9, nbMotsToWrite*2); // 1 mot = 2 bytes

                if (printConsole) { Console.WriteLine($"--- Writing of {nbMotsToWrite} datas ({nbMotsToWrite*2} bytes) in {binaryFileToWriteFullName} is successfull"); }
                return true;
            }
            catch (Exception ex)
            {
                fileLogger.LogException(ex, "Lecture données par TCP (Ecriture du fichier binaire)", -8);
                PrintUtils.PrintConsoleException(ex, $"Error when trying to download files by TCP (writing the binary file {binaryFileToWriteFullName})");
                return false;
            }
        }

        /// <summary>
        /// 1 Mesure = 12 bytes
        /// </summary>
        public static List<Mesure> ReadAllMesuresPremium(string fileFullName, FileLogger fileLogger, bool printConsole)
        {
            List<Mesure> mesures = new List<Mesure>();
            int nbBytesPerMesure = 12;
            int mesureReadCount = 0;

            try
            {
                // Opening the streams
                FileStream fileStream = File.Open(fileFullName, FileMode.Open, FileAccess.Read);
                BinaryReader binaryStream = new BinaryReader(fileStream);

                while (true)
                {
                    Byte[] bytesOfAMesure = binaryStream.ReadBytes(nbBytesPerMesure);

                    if (bytesOfAMesure.Length == 0)
                    {
                        // No more byte to read => no mesure left
                        break;
                    }
                    else if (bytesOfAMesure.Length != nbBytesPerMesure)
                    {
                        // Not enough byte => error
                        Console.WriteLine($"\n---- Error while reading mesure n°{mesureReadCount} : not enough byte to define a Mesure, stopping reading");
                        break;
                    }

                    Mesure mesure = ReadOneMesureWith12Bytes(bytesOfAMesure);

                    if (mesure.Info == null)
                    {
                        Console.WriteLine($"\n---- Error while reading mesure n°{mesureReadCount} : mesure type unknow, skipping to next mesure...");
                        continue; // Skipping to next mesure
                    }

                    // Adding the mesure to the list
                    mesures.Add(mesure);
                    mesureReadCount++;
                }

                // Closing the streams
                binaryStream.Close();
                fileStream.Close();
            }
            catch
            {
                throw;
            }

            if (printConsole) { Console.WriteLine($"--- For the file {fileFullName}, {mesureReadCount} Mesure has been read"); }

            return mesures;
        }

        /// <summary>
        /// [D15A55Ca01] 1 Mesure = 4 * 16 bytes (char)<br/>
        /// [D16A56Ca01] 1 Mesure = 5 * 16 bytes (char)
        /// </summary>
        public static List<Mesure> ReadAllMesuresM580(string fileFullName, int nbLines, FileLogger fileLogger, bool printConsole)
        {
            List<Mesure> mesures = new List<Mesure>();
            int nbBytesPerLine = 16;
            int mesureReadCount = 0;

            try
            {
                // Opening the streams
                FileStream fileStream = File.Open(fileFullName, FileMode.Open, FileAccess.Read);
                BinaryReader binaryStream = new BinaryReader(fileStream);

                bool stopReading = false;

                while (true)
                {
                    Byte[][] linesOfAMesure = new Byte[nbLines][];
                    for (int i = 0; i < nbLines; i++)
                    {
                        linesOfAMesure[i] = binaryStream.ReadBytes(nbBytesPerLine);

                        if (linesOfAMesure[i].Length == 0)
                        {
                            // Empty line = no more mesure left
                            stopReading = true;
                            break;
                        }
                        else if (linesOfAMesure[i].Length != nbBytesPerLine)
                        {
                            // Not enough byte => error
                            Console.WriteLine($"\n---- Error while reading mesure n°{mesureReadCount} : not enough byte in a line, stopping reading");
                            stopReading = true;
                            break;
                        }
                    }

                    if (stopReading) break; // Empty line = no more mesure left

                    // Reading 1 mesure
                    Mesure mesure;
                    if (nbLines == 4)
                    {
                        // D15A55Ca01
                        mesure = ReadOneMesureWith4Lines(linesOfAMesure);
                    }
                    else if (nbLines == 5)
                    {
                        // D16A56Ca01
                        mesure = ReadOneMesureWith5Lines(linesOfAMesure);
                    }
                    else
                    {
                        Console.WriteLine($"\n---- Error while reading : unable to read Mesure with {nbLines} lines, stopping reading");
                        break;
                    }

                    if (mesure.Info == null)
                    {
                        Console.WriteLine($"\n---- Error while reading mesure n°{mesureReadCount} : mesure type unknow, skipping to next mesure...");
                        continue; // Skipping to next mesure
                    }

                    // Adding the mesure to the list
                    mesures.Add(mesure);
                    mesureReadCount++;
                }

                // Closing the streams
                binaryStream.Close();
                fileStream.Close();
            }
            catch
            {
                throw;
            }

            if (printConsole) { Console.WriteLine($"--- For the file {fileFullName}, {mesureReadCount} Mesure has been read"); }

            return mesures;
        }

        /// <summary>
        /// Used for Premium Protocol (1 Mesure = 12 bytes)
        /// </summary>
        private static Mesure ReadOneMesureWith12Bytes(Byte[] bytesOfAMesure)
        {
            // Parsing byte to Mesure
            // time => byte 0 to 3
            // numeroVoie => byte 4, 5
            // info => byte 6, 7
            // ve => byte 8 to 11
            DateTime time = DateTime.Parse("01/01/2013").AddSeconds(ConversionUtils.ByteToUInt32(bytesOfAMesure, 0));
            int numeroVoie = ConversionUtils.ByteToUInt16(bytesOfAMesure, 4);
            UInt16 infoValue = ConversionUtils.ByteToUInt16(bytesOfAMesure, 6);
            float valeurALEchelle = ConversionUtils.ByteToFloat(bytesOfAMesure, 8);

            string typeMesure = GetTypeMesure(infoValue);

            return ConvertNumeroVoieBase64ToBase32(new Mesure()
            {
                Time = time,
                NumeroVoie = numeroVoie,
                Info = Info.GetInfoOfThisMesure(typeMesure, infoValue),
                ValeurALEchelle = valeurALEchelle,
            });
        }

        /// <summary>
        /// Used for M580 Protocol D15 (1 Mesure = 4 lines of 16 bytes (each of them is the ASCII hexadecimal code of a char))
        /// </summary>
        private static Mesure ReadOneMesureWith4Lines(Byte[][] linesOfAMesure)
        {
            // Parsing lines of byte to Mesure
            // time => line 0
            // numeroVoie => line 1
            // info => line 2
            // ve => line 3
            DateTime time = DateTime.Parse("01/01/2013").AddSeconds(long.Parse(ConversionUtils.ByteArrayToASCIIString(linesOfAMesure[0])));
            int numeroVoie = int.Parse(ConversionUtils.ByteArrayToASCIIString(linesOfAMesure[1]));
            Int16 infoValue = Int16.Parse(ConversionUtils.ByteArrayToASCIIString(linesOfAMesure[2]));
            float valeurALEchelle = float.Parse(ConversionUtils.ByteArrayToASCIIString(linesOfAMesure[3]), NumberStyles.Float, CultureInfo.InvariantCulture);

            string typeMesure = GetTypeMesure(infoValue);

            // If Calcul, we only keep the last 3 hexadecimal digits
            if (typeMesure.Equals("CAL"))
            {
                string infoValueHexa = ConversionUtils.IntToStringHexa4(infoValue);
                infoValue = ConversionUtils.StringHexaToInt16(infoValueHexa.Substring(infoValueHexa.Length - 3));
            }

            return ConvertNumeroVoieBase64ToBase32(new Mesure()
            {
                Time = time,
                NumeroVoie = numeroVoie,
                Info = Info.GetInfoOfThisMesure(typeMesure, infoValue),
                ValeurALEchelle = valeurALEchelle,
            });
        }

        /// <summary>
        /// Used for M580 Protocol D16 (1 Mesure = 5 lines of 16 bytes (each of them is the ASCII hexadecimal code of a char))
        /// </summary>
        private static Mesure ReadOneMesureWith5Lines(Byte[][] linesOfAMesure)
        {
            // Parsing lines of byte to Mesure
            // time => line 0
            // nomVoie => line 1
            // numeroVoie => line 2
            // info => line 3
            // ve => line 4
            DateTime time = DateTime.ParseExact(ConversionUtils.ByteArrayToASCIIString(linesOfAMesure[0]).Substring(0, 13), "ddMMyy_HHmmss", System.Globalization.CultureInfo.CurrentCulture);
            string nomVoie = ConversionUtils.ByteArrayToASCIIString(linesOfAMesure[1]);
            int numeroVoie = int.Parse(ConversionUtils.ByteArrayToASCIIString(linesOfAMesure[2]));
            Int16 infoValue = Int16.Parse(ConversionUtils.ByteArrayToASCIIString(linesOfAMesure[3]));
            float valeurALEchelle = float.Parse(ConversionUtils.ByteArrayToASCIIString(linesOfAMesure[4]), NumberStyles.Float, CultureInfo.InvariantCulture);

            string typeMesure = GetTypeMesure(infoValue);

            // If Calcul, we only keep the last 3 hexadecimal digits
            if (typeMesure.Equals("CAL"))
            {
                string infoValueHexa = ConversionUtils.IntToStringHexa4(infoValue);
                infoValue = ConversionUtils.StringHexaToInt16(infoValueHexa.Substring(infoValueHexa.Length - 3));
            }

            return ConvertNumeroVoieBase64ToBase32(new Mesure()
            {
                Time = time,
                NomVoie = nomVoie,
                NumeroVoie = numeroVoie,
                Info = Info.GetInfoOfThisMesure(typeMesure, infoValue),
                ValeurALEchelle = valeurALEchelle,
            });
        }

        /// <summary>
        /// Used for Premium protocol
        /// </summary>
        private static string GetTypeMesure(UInt16 info)
        {
            if (ConversionUtils.IsThisBitValueOne(info, 15)) return "CAL"; // Calcul

            if (ConversionUtils.IsThisBitValueOne(info, 14)) return "CAP"; // Capteur

            if (ConversionUtils.IsThisBitValueOne(info, 13)) return "TC"; // Telecommande, STOR

            if (ConversionUtils.IsThisBitValueOne(info, 12)) return "TS"; // Telesurveillee, ETOR

            if (ConversionUtils.IsThisBitValueOne(info, 11)) return "ECH"; // Echelle

            return null;
        }

        /// <summary>
        /// Used for M580 protocol
        /// </summary>
        private static string GetTypeMesure(Int16 info)
        {
            if (ConversionUtils.IsThisBitValueOne(info, 15)) return "CAL"; // Calcul

            if (ConversionUtils.IsThisBitValueOne(info, 14)) return "CAP"; // Capteur

            if (ConversionUtils.IsThisBitValueOne(info, 13)) return "TC"; // Telecommande, STOR

            if (ConversionUtils.IsThisBitValueOne(info, 12)) return "TS"; // Telesurveillee, ETOR

            if (ConversionUtils.IsThisBitValueOne(info, 11)) return "ECH"; // Echelle

            return null;
        }

        /// <summary>
        /// ECH mesure only : convert the base64 numeroVoie into a base32 numeroVoie<br/>
        /// Also change the TypeMesure to ECH0% or ECH100%
        /// </summary>
        private static Mesure ConvertNumeroVoieBase64ToBase32(Mesure mesure)
        {
            if (!mesure.Info.TypeMesure.Equals("ECH")) return mesure; // NumeroVoie already in base 32

            // NB : when the mesure is ECH, each Voie get 2 num (the first for a 0% modif, the second for a 100% modif) => base64
            // ex :
            // Voie 1 (code 'a') => onlyMesureOnThisLine.NumeroVoie = 1 for 0% modif 
            // Voie 1 (code 'a') => onlyMesureOnThisLine.NumeroVoie = 2 for 100% modif
            // Voie 2 (code 'b') => onlyMesureOnThisLine.NumeroVoie = 3 for 0% modif
            // Voie 2 (code 'b') => onlyMesureOnThisLine.NumeroVoie = 4 for 100% modif
            int numeroVoieBase64 = mesure.NumeroVoie;
            int numeroVoieBase32 = (int)Math.Ceiling(numeroVoieBase64 / 2d); // real numeroVoie

            mesure.NumeroVoie = numeroVoieBase32;
            mesure.Info.TypeMesure = (numeroVoieBase64 % 2 == 0) ? "ECH100%" : "ECH0%"; // [Base64] Even number is a 100% modif

            return mesure;
        }
    }
}

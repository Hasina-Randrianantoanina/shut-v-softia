using AutomateOperations.Entities.Infos;
using AutomateOperations.Entities;
using System.Globalization;
using AutomateOperations.AutomateProtocol;

namespace AutomateOperations.Utils
{
    public static class AutomateBinaryFileUtils
    {

        /// <returns>True if the data has been successfully written</returns>
        public static (bool statusWriting, Erreur? erreur) TryWriteDatasInBinaryFile(string binaryFileToWriteFullName, AutomateCommand automateCommand, int nbMotsToWrite)
        {
            try
            {
                // Opening the streams
                using FileStream fileStream = File.Open(binaryFileToWriteFullName, FileMode.Append, FileAccess.Write);
                using BinaryWriter binaryStream = new BinaryWriter(fileStream);

                binaryStream.Write(automateCommand.Response, 9, nbMotsToWrite*2); // 1 mot = 2 bytes

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ErrorUtils.HandleWritingError(503, ex));
            }
        }

        /// <summary>
        /// 1 Mesure = 12 bytes
        /// </summary>
        public static (List<Mesure>? mesures, Erreur? erreur) ReadAllMesuresPremium(string fileFullName)
        {
            List<Mesure> mesures = new List<Mesure>();
            int nbBytesPerMesure = 12;

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
                        return (null, ErrorUtils.HandleReadingError(606, null, $"{nbBytesPerMesure - bytesOfAMesure.Length}"));
                    }

                    var resultMesure = ReadOneMesureWith12Bytes(bytesOfAMesure);
                    if (resultMesure.mesure == null) 
                    {
                        return (null, resultMesure.erreur!);
                    }

                    // Adding the mesure to the list
                    mesures.Add(resultMesure.mesure);
                }

                // Closing the streams
                binaryStream.Close();
                fileStream.Close();

                return (mesures, null);
            }
            catch (Exception ex)
            {
                return (null, ErrorUtils.HandleReadingError(609, ex));
            }
        }

        /// <summary>
        /// [D15A55Ca01] 1 Mesure = 4 * 16 bytes (char)<br/>
        /// [D16A56Ca01] 1 Mesure = 5 * 16 bytes (char)
        /// </summary>
        public static (List<Mesure>? mesures, Erreur? erreur) ReadAllMesuresM580(string fileFullName, int nbLines)
        {
            List<Mesure> mesures = new List<Mesure>();
            int nbBytesPerLine = 16;

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
                            return (null, ErrorUtils.HandleReadingError(606, null, $"{nbBytesPerLine - linesOfAMesure[i].Length}"));
                        }
                    }

                    if (stopReading) break; // Empty line = no more mesure left

                    // Reading 1 mesure
                    var resultMesure = nbLines == 4 ? ReadOneMesureWith4Lines(linesOfAMesure) : ReadOneMesureWith5Lines(linesOfAMesure);
                    if (resultMesure.mesure == null)
                    {
                        return (null, resultMesure.erreur!);
                    }

                    // Adding the mesure to the list
                    mesures.Add(resultMesure.mesure);
                }

                // Closing the streams
                binaryStream.Close();
                fileStream.Close();

                return (mesures, null);
            }
            catch (Exception ex)
            {
                return (null, ErrorUtils.HandleReadingError(609, ex));
            }
        }

        /// <summary>
        /// Used for Premium Protocol (1 Mesure = 12 bytes)
        /// </summary>
        private static (Mesure? mesure, Erreur? erreur) ReadOneMesureWith12Bytes(Byte[] bytesOfAMesure)
        {
            // Parsing byte to Mesure
            // time => byte 0 to 3
            // numeroVoie => byte 4, 5
            // info => byte 6, 7
            // ve => byte 8 to 11
            try 
            {
                DateTime time = DateTime.Parse("01/01/2013").AddSeconds(ConversionUtils.ByteToUInt32(bytesOfAMesure, 0));
                int numeroVoie = ConversionUtils.ByteToUInt16(bytesOfAMesure, 4);
                UInt16 infoValue = ConversionUtils.ByteToUInt16(bytesOfAMesure, 6);
                float valeurALEchelle = ConversionUtils.ByteToFloat(bytesOfAMesure, 8);

                string? typeMesure = GetTypeMesure(infoValue);

                Mesure mesure = new Mesure()
                {
                    Time = time,
                    NumeroVoie = numeroVoie,
                    Info = Info.GetInfoOfThisMesure(typeMesure, infoValue),
                    ValeurALEchelle = valeurALEchelle,
                };

                if (mesure.Info == null) { return (null, ErrorUtils.HandleReadingError(607, null, infoValue.ToString())); }

                mesure = ConvertNumeroVoieBase64ToBase32(mesure);
                mesure.Evenement = mesure.CaculateEvenement();

                return (mesure, null);
            }
            catch (Exception ex) 
            {
                return (null, ErrorUtils.HandleReadingError(608, ex, "12 octets"));
            }

        }

        /// <summary>
        /// Used for M580 Protocol D15 (1 Mesure = 4 lines of 16 bytes (each of them is the ASCII hexadecimal code of a char))
        /// </summary>
        private static (Mesure? mesure, Erreur? erreur) ReadOneMesureWith4Lines(Byte[][] linesOfAMesure)
        {
            // Parsing lines of byte to Mesure
            // time => line 0
            // numeroVoie => line 1
            // info => line 2
            // ve => line 3
            try 
            {
                DateTime time = DateTime.Parse("01/01/2013").AddSeconds(long.Parse(ConversionUtils.ByteArrayToASCIIString(linesOfAMesure[0])));
                int numeroVoie = int.Parse(ConversionUtils.ByteArrayToASCIIString(linesOfAMesure[1]));
                Int16 infoValue = Int16.Parse(ConversionUtils.ByteArrayToASCIIString(linesOfAMesure[2]));
                float valeurALEchelle = float.Parse(ConversionUtils.ByteArrayToASCIIString(linesOfAMesure[3]), NumberStyles.Float, CultureInfo.InvariantCulture);

                string typeMesure = GetTypeMesure(infoValue);

                // If Calcul, we only keep the last 3 hexadecimal digits
                if (typeMesure != null && typeMesure.Equals("CAL"))
                {
                    string infoValueHexa = ConversionUtils.IntToStringHexa4(infoValue);
                    infoValue = ConversionUtils.StringHexaToInt16(infoValueHexa.Substring(infoValueHexa.Length - 3));
                }

                Mesure mesure = new Mesure()
                {
                    Time = time,
                    NumeroVoie = numeroVoie,
                    Info = Info.GetInfoOfThisMesure(typeMesure, infoValue),
                    ValeurALEchelle = valeurALEchelle,
                };

                if (mesure.Info == null) { return (null, ErrorUtils.HandleReadingError(607, null, infoValue.ToString())); }

                mesure = ConvertNumeroVoieBase64ToBase32(mesure);
                mesure.Evenement = mesure.CaculateEvenement();

                return (mesure, null);
            }
            catch (Exception ex)
            {
                return (null, ErrorUtils.HandleReadingError(608, ex, "4 lignes"));
            }
        }

        /// <summary>
        /// Used for M580 Protocol D16 (1 Mesure = 5 lines of 16 bytes (each of them is the ASCII hexadecimal code of a char))
        /// </summary>
        private static (Mesure? mesure, Erreur? erreur) ReadOneMesureWith5Lines(Byte[][] linesOfAMesure)
        {
            // Parsing lines of byte to Mesure
            // time => line 0
            // nomVoie => line 1
            // numeroVoie => line 2
            // info => line 3
            // ve => line 4
            try
            {
                DateTime time = DateTime.ParseExact(ConversionUtils.ByteArrayToASCIIString(linesOfAMesure[0]).Substring(0, 13), "ddMMyy_HHmmss", System.Globalization.CultureInfo.CurrentCulture);
                string nomVoie = ConversionUtils.ByteArrayToASCIIString(linesOfAMesure[1]);
                int numeroVoie = int.Parse(ConversionUtils.ByteArrayToASCIIString(linesOfAMesure[2]));
                Int16 infoValue = Int16.Parse(ConversionUtils.ByteArrayToASCIIString(linesOfAMesure[3]));
                float valeurALEchelle = float.Parse(ConversionUtils.ByteArrayToASCIIString(linesOfAMesure[4]), NumberStyles.Float, CultureInfo.InvariantCulture);

                string typeMesure = GetTypeMesure(infoValue);

                // If Calcul, we only keep the last 3 hexadecimal digits
                if (typeMesure != null && typeMesure.Equals("CAL"))
                {
                    string infoValueHexa = ConversionUtils.IntToStringHexa4(infoValue);
                    infoValue = ConversionUtils.StringHexaToInt16(infoValueHexa.Substring(infoValueHexa.Length - 3));
                }

                Mesure mesure = new Mesure()
                {
                    Time = time,
                    NomVoieM580 = nomVoie,
                    NumeroVoie = numeroVoie,
                    Info = Info.GetInfoOfThisMesure(typeMesure, infoValue),
                    ValeurALEchelle = valeurALEchelle,
                };
                
                if (mesure.Info == null) { return (null, ErrorUtils.HandleReadingError(607, null, infoValue.ToString())); }

                mesure = ConvertNumeroVoieBase64ToBase32(mesure);
                mesure.Evenement = mesure.CaculateEvenement();

                return (mesure, null);
            }
            catch (Exception ex)
            {
                return (null, ErrorUtils.HandleReadingError(608, ex, "5 lignes"));
            }
        }

        /// <summary>
        /// Used for Premium protocol
        /// </summary>
        private static string? GetTypeMesure(UInt16 info)
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
        private static string? GetTypeMesure(Int16 info)
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
            if (!mesure.Info.TypeMesure!.Equals("ECH")) return mesure; // NumeroVoie already in base 32

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

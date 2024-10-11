using AutomateOperations.Entities;

namespace AutomateOperations.Utils
{
    public static class AutomateCommandUtils
    {

        /// <summary>
        /// Array length is 15
        /// </summary>
        public static bool AreDatasAvailable(Byte[] array)
        {
            byte last2BitsOfByte11 = ConversionUtils.GetRightBitsOfAByte(array[10], 2);

            byte last2BitsOfByte13 = ConversionUtils.GetRightBitsOfAByte(array[12], 2);

            if (last2BitsOfByte11 == 0 && last2BitsOfByte13 == 0) { return false; }

            return true;
        }

        /// <summary>
        /// Array length is 25
        /// </summary>
        public static string GetVersion(Byte[] array)
        {
            return ConversionUtils.ByteArrayToString(array, 9);
        }

        /// <summary>
        /// M580 : Array length is 249 (120 mots * 2 bytes + 9)
        /// <br/>
        /// Premium : Array length is 243 (117 mots * 2 bytes + 9) or 153 (72 mots * 2 bytes + 9)
        /// </summary>
        public static List<VoieTelemesuree> GetAnalogicConfig(Byte[] array, int startingIndex, int endingIndex, int nbBytesPerVoie)
        {
            List<VoieTelemesuree> voiesTelemesurees = new List<VoieTelemesuree>();

            int byteIndex = 9; // début table

            for (int i = startingIndex; i < endingIndex; i++)
            {
                VoieTelemesuree voie = new VoieTelemesuree
                {
                    Numero = i+1,
                    AdresseBES = ConversionUtils.ByteToInt16(array, byteIndex),
                    Info = ConversionUtils.ByteToInt16(array, byteIndex+2),
                    SeuilBas = ConversionUtils.ByteToFloat(array, byteIndex+4),
                    SeuilHaut = ConversionUtils.ByteToFloat(array, byteIndex+8),
                    ValeurDelta = ConversionUtils.ByteToFloat(array, byteIndex+12),
                    Groupe = ConversionUtils.ByteToInt16(array, byteIndex+16)
                };
                voiesTelemesurees.Add(voie);

                byteIndex += nbBytesPerVoie;
            }

            return voiesTelemesurees;
        }

        /// <summary>
        /// Array length is greater or equal to 13
        /// </summary>
        public static (string? status, Erreur? erreur) GetStatusOfDatas(Byte[] response, string nextQuestionStr, string status)
        {
            string getStatusQuestionStr = "010313920003";

            try
            {
                if (!nextQuestionStr.Equals(getStatusQuestionStr)) { return (status, null); }

                byte last2BitsOfByte11 = ConversionUtils.GetRightBitsOfAByte(response[10], 2);

                byte last2BitsOfByte13 = ConversionUtils.GetRightBitsOfAByte(response[12], 2);

                string newStatus = last2BitsOfByte11.ToString() + last2BitsOfByte13.ToString();

                return (newStatus, null);
            }
            catch (Exception ex)
            {
                return (null, ErrorUtils.HandleWritingError(501, ex));
            }
        }

        public static (string? nextQuestion, Erreur? erreur) CalculateNextQuestionStr(Byte[] response, string status, bool isNextQuestionWriting)
        {
            string getStatusQuestionStr = "010313920003";
            string startingTransfertQuestionStr = "010613920001";
            string continueTransfertQuestionStr = "010613920003";
            string endingTransfertQuestionStr = "010613920002";
            string nbMots = response.Length > 14 ? ConversionUtils.ByteToStringHexa2(response[14]) : "00";
            string readDatasQuestionStr = "01031396" + "00" + nbMots;

            try
            {
                if (response[7] == (byte)6)
                {
                    return (getStatusQuestionStr, null); // Previous question was a writing => always read after a writing
                }

                string nextQuestionStr = string.Empty;
                switch (status)
                {
                    // Data availability status
                    case "00": // No datas, next question will not be send
                    case "10": // No more datas
                        break;
                    case "01": // There are datas => Writing 01 to get the datas
                    case "31": // There are datas =>  Writing 01 to get the datas
                        nextQuestionStr = startingTransfertQuestionStr;
                        break;

                    // Buffer status
                    case "12": // There is no datas left =>  6 to 114 mots = 1 to 19 mesures
                        nextQuestionStr = isNextQuestionWriting ? endingTransfertQuestionStr : readDatasQuestionStr;
                        break;
                    case "13": // The buffer is full of datas =>  120 mots = 20 mesures
                        nextQuestionStr = isNextQuestionWriting ? continueTransfertQuestionStr : readDatasQuestionStr;
                        break;

                    // Waiting the automate
                    case "11": // Attente api - data dispo
                    case "22": // Attente api - fin opérations
                    case "33": // Attente api - nouveau cycle
                        nextQuestionStr = getStatusQuestionStr;
                        break;
                    default:
                        nextQuestionStr = "Unknown case";
                        break;
                }
                return (nextQuestionStr, null);
            }
            catch (Exception ex)
            {
                return (null, ErrorUtils.HandleWritingError(502, ex));
            }
        }
    }
}

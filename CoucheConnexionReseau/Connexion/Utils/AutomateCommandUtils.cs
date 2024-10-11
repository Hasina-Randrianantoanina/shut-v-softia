using Connexion.Entities;

namespace Connexion.Utils
{
    public static class AutomateCommandUtils
    {

        /// <summary>
        /// Array length is 15
        /// </summary>
        public static bool AreDatasAvailable(Byte[] array)
        {
            if (array.Length < 13) { return false; } // Out of bonds

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
        /// M580 : Array length is 209 (100 mots * 2 bytes + 9)
        /// <br/>
        /// Premium : Array length is 243 (117 mots * 2 bytes + 9) or 153 (72 mots * 2 bytes + 9)
        /// </summary>
        public static List<VoieInterne> GetAnalogicConfig(Byte[] array, int startingIndex, int endingIndex, int nbBytesPerVoie)
        {
            List<VoieInterne> voieInternes = new List<VoieInterne>();

            int byteIndex = 9; // début table

            for (int i = startingIndex; i < endingIndex; i++)
            {
                VoieInterne voie = new VoieInterne
                {
                    AdresseBES = ConversionUtils.ByteToInt16(array, byteIndex), // Adresse de la donnée dans table d'échange BES
                    Info = ConversionUtils.ByteToInt16(array, byteIndex+2), // informations sur la variable
                    SeuilBas = ConversionUtils.ByteToFloat(array, byteIndex+4),
                    SeuilHaut = ConversionUtils.ByteToFloat(array, byteIndex+8),
                    ValeurDelta = ConversionUtils.ByteToFloat(array, byteIndex+12),
                    Groupe = ConversionUtils.ByteToInt16(array, byteIndex+16)
                };
                voieInternes.Add(voie);

                byteIndex += nbBytesPerVoie;
            }

            return voieInternes;
        }

        /// <summary>
        /// Array length is 249 (120 mots)
        /// </summary>
        public static void Get60AnalogicRealTimeDatas(Byte[] array, List<VoieInterne> voiesInternes)
        {
            int byteIndex = 9; // début table

            for (int i = 0; i < 60; i++)
            {
                voiesInternes[i].SeuilBas = ConversionUtils.ByteToFloat(array, byteIndex+4);

                byteIndex += 4; // 1 voie = 4 bytes (2 mots)
            }
        }

        /// <summary>
        /// Array length is greater or equal to 13
        /// </summary>
        public static string GetStatusOfDatas(Byte[] response)
        {
            if (response.Length < 13)
            {
                Console.WriteLine("\n----- Error : Array length < 13");
                return null;
            }

            byte last2BitsOfByte11 = ConversionUtils.GetRightBitsOfAByte(response[10], 2);

            byte last2BitsOfByte13 = ConversionUtils.GetRightBitsOfAByte(response[12], 2);

            return last2BitsOfByte11.ToString() + last2BitsOfByte13.ToString();
        }

        public static Byte[] CalculateNextQuestion(Byte[] response, string status)
        {
            Byte[] getStatusQuestion = ConversionUtils.QuestionAutomateToByteArray("010313920003");
            Byte[] startingTransfertQuestion = ConversionUtils.QuestionAutomateToByteArray("010613920001");
            string nbMots = response.Length > 14 ? ConversionUtils.ByteToStringHexa2(response[14]) : "00";
            Byte[] readDatasQuestion = ConversionUtils.QuestionAutomateToByteArray("01031396" + "00" + nbMots);

            Byte[] nextQuestion = null;

            if (response[7] == (byte)6)
            {
                // Previous question was a writing => always read after a writing
                return getStatusQuestion;
            }

            switch (status)
            {
                // Data availability status
                case "00": // No datas, next question will not be send
                    break;
                case "01": // There are datas => Writing 01 to get the datas
                    nextQuestion = startingTransfertQuestion;
                    break;
                case "31": // There are datas =>  Writing 01 to get the datas
                    nextQuestion = startingTransfertQuestion;
                    break;

                // Buffer status
                case "12": // There is no datas left => Reading 6*x mots
                    // 6 to 114 mots = 1 to 19 mesures
                    nextQuestion = readDatasQuestion;
                    break;
                case "13": // The buffer is full of datas
                    // 120 mots = 20 mesures
                    nextQuestion = readDatasQuestion;
                    break;

                // Waiting the automate
                case "11": // Attente api - data dispo
                    // Waiting 10 ms before reading again => maybe not usefull with async TCP method
                    //Console.WriteLine("Waiting 10ms...");
                    //Thread.Sleep(10);
                    nextQuestion = getStatusQuestion;
                    break;
                case "22": // Attente api - fin opérations
                    // Waiting 10 ms before reading again => maybe not usefull with async TCP method
                    //Console.WriteLine("Waiting 10ms...");
                    //Thread.Sleep(10);
                    nextQuestion = getStatusQuestion;
                    break;
                case "33": // Attente api - nouveau cycle
                    // Waiting 10 ms before reading again => maybe not usefull with async TCP method
                    //Console.WriteLine("Waiting 10ms...");
                    //Thread.Sleep(10);
                    nextQuestion = getStatusQuestion;
                    break;

                // Other cases
                case "10": // No more datas
                    break;
            }

            return nextQuestion;
        }

    }
}

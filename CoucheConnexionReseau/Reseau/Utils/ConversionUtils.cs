namespace Reseau.Utils
{
    public static class ConversionUtils
    {

        /// <summary>
        /// In CR ASNet, response = array of hexadecimal formatted byte without "-" (2 hexa = 1 byte)
        /// </summary>
        /// <param name="responseAutomate"></param>
        /// <returns></returns>
        public static Byte[] ResponseAutomateToByteArray(string responseAutomate)
        {
            return Enumerable.Range(0, responseAutomate.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(responseAutomate.Substring(x, 2), 16))
                             .ToArray();
        }

        /// <summary>
        /// In CR ASNet, questionAutomate is not the question sent but a string used to create the question
        /// </summary>
        /// <param name="questionAutomate"></param>
        /// <returns></returns>
        public static Byte[] QuestionAutomateToByteArray(string questionAutomate)
        {
            Byte[] array = new Byte[256]; // NB : array size 256 for development
            array[0] = 0; // transactionID / 256
            array[1] = 0; // transactionID % 256
            array[2] = 0;
            array[3] = 0;
            array[4] = 0;
            array[5] = 6;
            array[6] = StringHexaToByte(questionAutomate.Substring(0, 2));
            array[7] = StringHexaToByte(questionAutomate.Substring(2, 2));
            array[8] = StringHexaToByte(questionAutomate.Substring(4, 2));
            array[9] = StringHexaToByte(questionAutomate.Substring(6, 2));
            array[10] = StringHexaToByte(questionAutomate.Substring(8, 2));
            array[11] = StringHexaToByte(questionAutomate.Substring(10, 2));

            switch (questionAutomate.Substring(2, 2))
            {
                case "03":
                    // lecture x mots
                    break;
                case "06":
                    // ecriture 1 mot
                    break;
                case "10":
                    // ecriture x mots
                    byte nbo = StringHexaToByte(questionAutomate.Substring(12, 2));
                    array[5] = (byte)(nbo + 7);
                    array[12] = nbo;
                    for (int i = 0; i < nbo -1; i++)
                    {
                        array[i+13] = StringHexaToByte(questionAutomate.Substring(14+i*2, 2));
                    }
                    break;
            }

            return array;
        }

        public static byte StringHexaToByte(string hexaValue)
        {
            return Convert.ToByte(hexaValue.Substring(0, 2), 16);
        }

    }
}
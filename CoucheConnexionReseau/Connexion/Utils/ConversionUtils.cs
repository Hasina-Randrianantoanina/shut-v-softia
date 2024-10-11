using System.Text;

namespace Connexion.Utils
{
    public static class ConversionUtils
    {

        /// <summary>
        /// Convert byte array to string<br/>
        /// Also switch the chars 2 by 2
        /// </summary>
        public static string ByteArrayToString(Byte[] array, int startIndex)
        {
            string message = "";
            char[] chars = Encoding.ASCII.GetChars(array, 0, array.Length);

            for (int i = startIndex; i+1 < chars.Length; i += 2)
            {
                if (chars[i+1] == 0) { break; }
                message += chars[i+1];

                if (chars[i] == 0) { break; }
                message += chars[i];
            }

            return message;
        }

        /// <summary>
        /// In CR ASNet, questionAutomate is not the question sent but a string used to create the question
        /// </summary>
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

        /// <summary>
        /// hexaValue's length is 2 max
        /// </summary>
        public static byte StringHexaToByte(string hexaValue)
        {
            if (hexaValue.Length > 2 || hexaValue.Length == 0) { return 0; } // Out of bonds ( 1 byte = 2 hexa digits)
            return Convert.ToByte(hexaValue, 16);
        }

        /// <summary>
        /// hexaValue's length is 4 max
        /// </summary>
        public static Int16 StringHexaToInt16(string hexaValue)
        {
            if (hexaValue.Length > 4 || hexaValue.Length == 0) { return 0; } // Out of bonds ( Int16 = 2 bytes = 4 hexa digits)
            return Convert.ToInt16(hexaValue, 16);
        }

        /// <returns>Hexadecimal string of size 2 minimum</returns>
        public static string ByteToStringHexa2(byte value)
        {
            string hexa = Convert.ToString(value, 16).ToUpper();
            return hexa.PadLeft(2, '0');
        }

        /// <returns>Hexadecimal string of size 4 minimum</returns>
        public static string IntToStringHexa4(int value)
        {
            string hexa = Convert.ToString(value, 16).ToUpper();
            return hexa.PadLeft(4, '0');
        }

        /// <summary>
        /// 1 int16 = 2 bytes
        /// <br/>
        /// Also switch the 2 bytes
        /// </summary>
        /// <returns>Int16</returns>
        public static Int16 ByteToInt16(Byte[] array, int byteIndex)
        {
            if (array.Length -1 < byteIndex +1) { return 0; } // Out of bonds

            // Switch the 2 bytes
            byte temp = array[byteIndex];
            array[byteIndex] = array[byteIndex+1];
            array[byteIndex+1] = temp;

            return BitConverter.ToInt16(array, byteIndex);
        }

        /// <summary>
        /// 1 int16 = 2 bytes
        /// <br/>
        /// Also switch the 2 bytes
        /// </summary>
        /// <returns>Unsigned Int16</returns>
        public static UInt16 ByteToUInt16(Byte[] array, int byteIndex)
        {
            if (array.Length -1 < byteIndex +1) { return 0; } // Out of bonds

            // Switch the 2 bytes
            byte temp = array[byteIndex];
            array[byteIndex] = array[byteIndex+1];
            array[byteIndex+1] = temp;

            return BitConverter.ToUInt16(array, byteIndex);
        }

        /// <summary>
        /// 1 uint32 = 4 bytes
        /// <br/>
        /// Also switch the bytes 2 by 2
        /// </summary>
        /// <returns>Unsigned Int32</returns>
        public static UInt32 ByteToUInt32(Byte[] array, int byteIndex)
        {
            if (array.Length -1 < byteIndex +3) { return 0; } // Out of bonds

            // Switch the first 2 bytes
            byte temp = array[byteIndex];
            array[byteIndex] = array[byteIndex+1];
            array[byteIndex+1] = temp;

            // Switch the last 2 bytes
            temp = array[byteIndex+2];
            array[byteIndex+2] = array[byteIndex+3];
            array[byteIndex+3] = temp;

            return BitConverter.ToUInt32(array, byteIndex);
        }

        /// <summary>
        /// 1 float = 4 bytes
        /// <br/>
        /// Also switch the bytes 2 by 2
        /// </summary>
        public static float ByteToFloat(Byte[] array, int byteIndex)
        {
            if (array.Length -1 < byteIndex +3) { return 0; } // Out of bonds

            // Switch the first 2 bytes
            byte temp = array[byteIndex];
            array[byteIndex] = array[byteIndex+1];
            array[byteIndex+1] = temp;

            // Switch the last 2 bytes
            temp = array[byteIndex+2];
            array[byteIndex+2] = array[byteIndex+3];
            array[byteIndex+3] = temp;

            return BitConverter.ToSingle(array, byteIndex);
        }

        /// <summary>
        /// numberOfBitsWanted = 1 To 8 bits
        /// </summary>
        public static byte GetRightBitsOfAByte(byte value, int numberOfBitsWanted)
        {
            if (numberOfBitsWanted < 1) { return 0; }
            if (numberOfBitsWanted > 7) { return value; } // 8 bits wanted = full byte

            byte bitMask = (byte)(Math.Pow(2, numberOfBitsWanted) -1);
            return (byte)(value & bitMask);
        }

        /// <summary>
        /// bitIndexFromTheRight = 0 To 15
        /// <br/>
        /// Premium protocol Info
        /// </summary>
        public static bool IsThisBitValueOne(UInt16 value, int bitIndexFromTheRight)
        {
            if (bitIndexFromTheRight < 0 || bitIndexFromTheRight > 15) { return false; } // Out of bonds

            Byte[] array = BitConverter.GetBytes(value);

            // From 0 to 7 => byte 0, from 8 to 15 => byte 1
            byte byteToCompare = (bitIndexFromTheRight > 7) ? array[1] : array[0];

            byte bitMask = (byte)Math.Pow(2, bitIndexFromTheRight % 8);

            return (byteToCompare & bitMask) > 0;
        }

        /// <summary>
        /// bitIndexFromTheRight = 0 To 15
        /// <br/>
        /// M580 protocol Info
        /// </summary>
        public static bool IsThisBitValueOne(Int16 value, int bitIndexFromTheRight)
        {
            if (bitIndexFromTheRight < 0 || bitIndexFromTheRight > 15) { return false; } // Out of bonds

            Byte[] array = BitConverter.GetBytes(value);

            // From 0 to 7 => byte 0, from 8 to 15 => byte 1
            byte byteToCompare = (bitIndexFromTheRight > 7) ? array[1] : array[0];

            byte bitMask = (byte)Math.Pow(2, bitIndexFromTheRight % 8);

            return (byteToCompare & bitMask) > 0;
        }

        /// <summary>
        /// Each byte is the hexa code of the char using ASCII encoding
        /// </summary>
        public static string ByteArrayToASCIIString(Byte[] array)
        {
            return new string(Encoding.ASCII.GetChars(array));
        }

        /// <summary>
        /// Used by fileLogger
        /// </summary>
        /// <returns>String of bytes formatted in hexadecimal that contains the wanted data</returns>
        public static string ByteArrayToShortString(Byte[] array, int motsToRead)
        {
            if (motsToRead < 0) { return ""; } // Out of bonds

            string arrayStr = BitConverter.ToString(array).Replace("-", "");

            if (motsToRead > 123) { return arrayStr; } // Out of bonds, return full arrayStr

            return arrayStr.Substring(0, (motsToRead*2 + 9)*2); // 1 mots = 2 bytes = 4 hexa formatted char
        }
    }
}
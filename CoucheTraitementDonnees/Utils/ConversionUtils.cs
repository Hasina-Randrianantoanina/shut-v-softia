using System.Text;

namespace CoucheTraitementDonnees.Utils
{
    public static class ConversionUtils
    {
        /// <summary>
        /// </summary>
        /// <param name="hexaValue">Length 4 max</param>
        /// <returns></returns>
        public static Int16 StringHexaToInt16(string hexaValue)
        {
            if (hexaValue.Length > 4) return 0; // Out of bonds ( Int16 = 2 bytes = 4 hexa digits)
            return Convert.ToInt16(hexaValue, 16);
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <returns>hexadecimal string of size 4 minimum</returns>
        public static string IntToStringHexa4(int value)
        {
            string hexa = Convert.ToString(value, 16).ToUpper();
            while (hexa.Length < 4)
            {
                hexa = "0" + hexa;
            }
            return hexa;
        }

        /// <summary>
        /// 1 int16 = 2 bytes
        /// <br/>
        /// Also switch the 2 bytes
        /// </summary>
        /// <param name="array"></param>
        /// <param name="byteIndex"></param>
        /// <returns>integer 16bits</returns>
        public static Int16 ByteToInt16(Byte[] array, int byteIndex)
        {
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
        /// <param name="array"></param>
        /// <param name="byteIndex"></param>
        /// <returns>integer 16bits</returns>
        public static UInt16 ByteToUInt16(Byte[] array, int byteIndex)
        {
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
        /// <param name="array"></param>
        /// <param name="byteIndex"></param>
        /// <returns>unsigned integer 32bits</returns>
        public static UInt32 ByteToUInt32(Byte[] array, int byteIndex)
        {
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
        /// <param name="array"></param>
        /// <param name="byteIndex"></param>
        /// <returns></returns>
        public static float ByteToFloat(Byte[] array, int byteIndex)
        {
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
        /// bitIndexFromTheRight = 0 To 15</br>
        /// Premium protocol Info
        /// </summary>
        /// <param name="value"></param>
        /// <param name="bitIndexFromTheRight"></param>
        /// <returns></returns>
        public static bool IsThisBitValueOne(UInt16 value, int bitIndexFromTheRight)
        {
            if (bitIndexFromTheRight < 0 || bitIndexFromTheRight > 15) return false; // Out of bonds

            Byte[] array = BitConverter.GetBytes(value);

            // From 0 to 7 => byte 0, from 8 to 15 => byte 1
            byte byteToCompare = (bitIndexFromTheRight > 7) ? array[1] : array[0];

            byte bitMask = (byte)Math.Pow(2, bitIndexFromTheRight % 8);

            return (byteToCompare & bitMask) > 0;
        }

        /// <summary>
        /// bitIndexFromTheRight = 0 To 15</br>
        /// M580 protocol Info
        /// </summary>
        /// <param name="value"></param>
        /// <param name="bitIndexFromTheRight"></param>
        /// <returns></returns>
        public static bool IsThisBitValueOne(Int16 value, int bitIndexFromTheRight)
        {
            if (bitIndexFromTheRight < 0 || bitIndexFromTheRight > 15) return false; // Out of bonds

            Byte[] array = BitConverter.GetBytes(value);

            // From 0 to 7 => byte 0, from 8 to 15 => byte 1
            byte byteToCompare = (bitIndexFromTheRight > 7) ? array[1] : array[0];

            byte bitMask = (byte)Math.Pow(2, bitIndexFromTheRight % 8);

            return (byteToCompare & bitMask) > 0;
        }

        /// <summary>
        /// Each byte is the hexa code of the char using ASCII encoding<br/>
        /// </summary>
        /// <returns>String's length si the same as the array</returns>
        public static string ByteArrayToASCIIString(Byte[] array)
        {
            return new string(Encoding.ASCII.GetChars(array));
        }

    }
}

namespace CoucheLectureMan.Utils
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

        public static Int16 GetIntegerValueOfThisBit(int bitIndexFromTheRight) 
        {
            if (bitIndexFromTheRight < 0 || bitIndexFromTheRight > 15) return 0; // Out of bonds
            return (Int16)Math.Pow(2,bitIndexFromTheRight);
        }
    }
}

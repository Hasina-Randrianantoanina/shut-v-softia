using Reseau.Utils;

namespace Reseau.AutomateProtocol
{
    // Automate M580 (D16 RU)
    public class HEProtocol : IAutomateProtocol
    {
        public readonly Dictionary<string, Byte[]> dictionnary = CreateDictionnary();

        public Byte[] CalculResponse(string question)
        {
            if (dictionnary.TryGetValue(question, out Byte[] response))
            {
                return response;
            }
            else
            {
                return null;
            }

        }

        private static Dictionary<string, Byte[]> CreateDictionnary()
        {
            Dictionary<string, Byte[]> dictionnary = new Dictionary<string, Byte[]>();

            // ------------------------ 1st command : Are there available datas ?
            // connexion -> reseau
            Byte[] availableDatasQuestion = ConversionUtils.QuestionAutomateToByteArray("010313920003");
            string availableDatasQuestionStr = BitConverter.ToString(availableDatasQuestion).Replace("-", "");
            // reseau -> connexion
            Byte[] availableDatasResponse = ConversionUtils.ResponseAutomateToByteArray("000000000009010306000000010000");
            dictionnary.Add(availableDatasQuestionStr, availableDatasResponse);

            // ------------------------ 2nd command : LECT CONFIG_VER
            // connexion -> reseau
            Byte[] lectureVersionQuestion = ConversionUtils.QuestionAutomateToByteArray("010313740008");
            string lectureVersionQuestionStr = BitConverter.ToString(lectureVersionQuestion).Replace("-", "");
            // reseau -> connexion
            Byte[] lectureVersionResponse = ConversionUtils.ResponseAutomateToByteArray("00000000001301031031444136363561433130000000000000");
            dictionnary.Add(lectureVersionQuestionStr, lectureVersionResponse);

            // ------------------------ 3rd command : LECT CONFIG_ANA 1/6
            // connexion -> reseau
            Byte[] lectureConfigAnaQuestion1 = ConversionUtils.QuestionAutomateToByteArray("010317700064");
            string lectureConfigAnaQuestion1Str = BitConverter.ToString(lectureConfigAnaQuestion1).Replace("-", "");
            // reseau -> connexion
            Byte[] lectureConfigAnaResponse1 = ConversionUtils.ResponseAutomateToByteArray("0000000000CB0103C811940005CCCD41BACCCD41EAD70A3C2300000000119600057AE141BC3D714206D70A3C2300000000119800050000C1A0000041A0CCCD3D4C00000000119E00070000C1A0000041A0D70A3C230000000011A000070000C0A0000040A0126F3A830000000011A200070000C0A0000040A0126F3A830000000011A400070000C0A0000040A0126F3A830000000011A600070000C0A0000040A0126F3A830000000011A800070000C0A0000040A0126F3A830000000011AA00070000C0A0000040A0126F3A8300000000");
            dictionnary.Add(lectureConfigAnaQuestion1Str, lectureConfigAnaResponse1);

            // ------------------------ 4th command : LECT CONFIG_ANA 2/6
            // connexion -> reseau
            Byte[] lectureConfigAnaQuestion2 = ConversionUtils.QuestionAutomateToByteArray("010317D40064");
            string lectureConfigAnaQuestion2Str = BitConverter.ToString(lectureConfigAnaQuestion2).Replace("-", "");
            // reseau -> connexion
            Byte[] lectureConfigAnaResponse2 = ConversionUtils.ResponseAutomateToByteArray("0000000000CB0103C811AC00070000C0A0000040A0126F3A830000000011AE00070000C000000042C8000041200000000011B000070000C000000042C8000041200000000011B200070000C000000042C8000041200000000011B400070000BF80000042C8000041200000000011B600070000C000000042CA000041200000000011B800070000C000000042C8000041200000000011BA00070000C000000042C8000041200000000011BC00070000BF80000042C8000041A00000000011BE00070000BF80000042C8000041A000000000");
            dictionnary.Add(lectureConfigAnaQuestion2Str, lectureConfigAnaResponse2);

            // ------------------------ 5th command : LECT CONFIG_ANA 3/6
            // connexion -> reseau
            Byte[] lectureConfigAnaQuestion3 = ConversionUtils.QuestionAutomateToByteArray("010318380064");
            string lectureConfigAnaQuestion3Str = BitConverter.ToString(lectureConfigAnaQuestion3).Replace("-", "");
            // reseau -> connexion
            Byte[] lectureConfigAnaResponse3 = ConversionUtils.ResponseAutomateToByteArray("0000000000CB0103C811C000070000BF80000042C8000041A00000000011C200070000BF80000042C8000041A00000000011C400070000BF80000042C8000041A00000000011C600070000BF80000042C8000041A00000000011C800070000BF80000042C8000041A00000000011CA00070000BF8000004270D70A3C23000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            dictionnary.Add(lectureConfigAnaQuestion3Str, lectureConfigAnaResponse3);

            // ------------------------ 6th command : LECT CONFIG_ANA 4/6
            // connexion -> reseau
            Byte[] lectureConfigAnaQuestion4 = ConversionUtils.QuestionAutomateToByteArray("0103189C0064");
            string lectureConfigAnaQuestion4Str = BitConverter.ToString(lectureConfigAnaQuestion4).Replace("-", "");
            // reseau -> connexion
            Byte[] lectureConfigAnaResponse4 = ConversionUtils.ResponseAutomateToByteArray("0000000000CB0103C80000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            dictionnary.Add(lectureConfigAnaQuestion4Str, lectureConfigAnaResponse4);

            // ------------------------ 7th command : LECT CONFIG_ANA 5/6
            // connexion -> reseau
            Byte[] lectureConfigAnaQuestion5 = ConversionUtils.QuestionAutomateToByteArray("010319000064");
            string lectureConfigAnaQuestion5Str = BitConverter.ToString(lectureConfigAnaQuestion5).Replace("-", "");
            // reseau -> connexion
            Byte[] lectureConfigAnaResponse5 = ConversionUtils.ResponseAutomateToByteArray("0000000000CB0103C80000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            dictionnary.Add(lectureConfigAnaQuestion5Str, lectureConfigAnaResponse5);

            // ------------------------ 8th command : LECT CONFIG_ANA 6/6
            // connexion -> reseau
            Byte[] lectureConfigAnaQuestion6 = ConversionUtils.QuestionAutomateToByteArray("010319640064");
            string lectureConfigAnaQuestion6Str = BitConverter.ToString(lectureConfigAnaQuestion6).Replace("-", "");
            // reseau -> connexion
            Byte[] lectureConfigAnaResponse6 = ConversionUtils.ResponseAutomateToByteArray("0000000000CB0103C80000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            dictionnary.Add(lectureConfigAnaQuestion6Str, lectureConfigAnaResponse6);

            // ------------------------ 9th command : LECT VALTR_ANA
            // connexion -> reseau
            Byte[] lectureValeurTRAnaQuestion = ConversionUtils.QuestionAutomateToByteArray("010311940078");
            string lectureValeurTRAnaQuestionStr = BitConverter.ToString(lectureValeurTRAnaQuestion).Replace("-", "");
            // reseau -> connexion
            Byte[] lectureValeurTRAnaResponse = ConversionUtils.ResponseAutomateToByteArray("0000000000F30103F0B1C441BA4FDF41BCD70BBD2300000000000000000000000000000000000000000000000000000000000000000000000000000000000042680000425C000042600000426400004280000042740000427C00004110000041200000413000004160000040A00000410000004100341403EF0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            dictionnary.Add(lectureValeurTRAnaQuestionStr, lectureValeurTRAnaResponse);

            return dictionnary;
        }

    }
}

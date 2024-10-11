using Reseau.Utils;

namespace Reseau.AutomateProtocol
{
    // Automate M580 (D15 RU)
    public class GAProtocol : IAutomateProtocol
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
            Byte[] lectureVersionResponse = ConversionUtils.ResponseAutomateToByteArray("00000000001301031031444135353561433130000000000000");
            dictionnary.Add(lectureVersionQuestionStr, lectureVersionResponse);

            // ------------------------ 3rd command : LECT CONFIG_ANA 1/6
            // connexion -> reseau
            Byte[] lectureConfigAnaQuestion1 = ConversionUtils.QuestionAutomateToByteArray("010317700064");
            string lectureConfigAnaQuestion1Str = BitConverter.ToString(lectureConfigAnaQuestion1).Replace("-", "");
            // reseau -> connexion
            Byte[] lectureConfigAnaResponse1 = ConversionUtils.ResponseAutomateToByteArray("0000000000CB0103C8119400050000BF8000004290D70A3CA300000000119600050000BF800000428AD70A3CA300000000119800050000BF800000428AD70A3CA300000000119A00050000BF8000004292D70A3CA300000000119C00050000BF8000004292CCCD3D4C0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            dictionnary.Add(lectureConfigAnaQuestion1Str, lectureConfigAnaResponse1);

            // ------------------------ 4th command : LECT CONFIG_ANA 2/6
            // connexion -> reseau
            Byte[] lectureConfigAnaQuestion2 = ConversionUtils.QuestionAutomateToByteArray("010317D40064");
            string lectureConfigAnaQuestion2Str = BitConverter.ToString(lectureConfigAnaQuestion2).Replace("-", "");
            // reseau -> connexion
            Byte[] lectureConfigAnaResponse2 = ConversionUtils.ResponseAutomateToByteArray("0000000000CB0103C80000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            dictionnary.Add(lectureConfigAnaQuestion2Str, lectureConfigAnaResponse2);

            // ------------------------ 5th command : LECT CONFIG_ANA 3/6
            // connexion -> reseau
            Byte[] lectureConfigAnaQuestion3 = ConversionUtils.QuestionAutomateToByteArray("010318380064");
            string lectureConfigAnaQuestion3Str = BitConverter.ToString(lectureConfigAnaQuestion3).Replace("-", "");
            // reseau -> connexion
            Byte[] lectureConfigAnaResponse3 = ConversionUtils.ResponseAutomateToByteArray("0000000000CB0103C80000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
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
            Byte[] lectureValeurTRAnaResponse = ConversionUtils.ResponseAutomateToByteArray("0000000000F30103F0BBFC428689D54284784C4284062A428AB22D425400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            dictionnary.Add(lectureValeurTRAnaQuestionStr, lectureValeurTRAnaResponse);

            return dictionnary;
        }

    }
}

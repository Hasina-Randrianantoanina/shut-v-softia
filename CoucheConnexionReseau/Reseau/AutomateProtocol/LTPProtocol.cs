using Reseau.Utils;

namespace Reseau.AutomateProtocol
{
    public class LTPProtocol : IAutomateProtocol
    {

        public readonly Dictionary<string, Byte[][]> Dictionnary;
        public int GetStatusCount { get; set; }
        public int Read120MotsCount { get; set; }

        public LTPProtocol(int numberOfRepetition)
        {
            Dictionnary = CreateDictionnary(numberOfRepetition);
        }

        public Byte[] CalculResponse(string question)
        {
            if (Dictionnary.TryGetValue(question, out Byte[][] response))
            {
                Byte[] result;
                if (question.Equals(BitConverter.ToString(ConversionUtils.QuestionAutomateToByteArray("010313920003")).Replace("-", "")))
                {
                    result = response[GetStatusCount];
                    GetStatusCount++;
                }
                else if (question.Equals(BitConverter.ToString(ConversionUtils.QuestionAutomateToByteArray("010313960078")).Replace("-", "")))
                {
                    result = response[Read120MotsCount];
                    Read120MotsCount++;
                }
                else
                {
                    result = response[0];
                }

                return result;
            }
            else
            {
                return null;
            }

        }

        private static Dictionary<string, Byte[][]> CreateDictionnary(int numberOfRepetition)
        {
            Dictionary<string, Byte[][]> dictionnary = new Dictionary<string, Byte[][]>();

            // ------------------------ 1st command : Are there available datas ?
            // connexion -> reseau
            Byte[] availableDatasQuestion = ConversionUtils.QuestionAutomateToByteArray("010313920003");
            string availableDatasQuestionStr = BitConverter.ToString(availableDatasQuestion).Replace("-", "");
            // reseau -> connexion
            Byte[] availableDatasResponse = ConversionUtils.ResponseAutomateToByteArray("000000000009010306000000010000");

            // ------------------------ 2nd command : LECT CONFIG_VER
            // connexion -> reseau
            Byte[] lectureVersionQuestion = ConversionUtils.QuestionAutomateToByteArray("010313740008");
            string lectureVersionQuestionStr = BitConverter.ToString(lectureVersionQuestion).Replace("-", "");
            // reseau -> connexion
            Byte[] lectureVersionResponse = ConversionUtils.ResponseAutomateToByteArray("00000000001301031031444133333561413130000000000000");
            dictionnary.Add(lectureVersionQuestionStr, new Byte[][] { lectureVersionResponse });

            // ------------------------ 3rd command : LECT CONFIG_ANA 1/5
            // connexion -> reseau
            Byte[] lectureConfigAnaQuestion1 = ConversionUtils.QuestionAutomateToByteArray("010317700075");
            string lectureConfigAnaQuestion1Str = BitConverter.ToString(lectureConfigAnaQuestion1).Replace("-", "");
            // reseau -> connexion
            Byte[] lectureConfigAnaResponse1 = ConversionUtils.ResponseAutomateToByteArray("0000000000ED0103EA119400050000BF8000004248D70A3CA30000119600050000BF8000004248D70A3CA30000119800050000BF8000004248000000000000119A00050000BF8000004248CCCD3D4C0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            dictionnary.Add(lectureConfigAnaQuestion1Str, new Byte[][] { lectureConfigAnaResponse1 });

            // ------------------------ 4th command : LECT CONFIG_ANA 2/5
            // connexion -> reseau
            Byte[] lectureConfigAnaQuestion2 = ConversionUtils.QuestionAutomateToByteArray("010317E50075");
            string lectureConfigAnaQuestion2Str = BitConverter.ToString(lectureConfigAnaQuestion2).Replace("-", "");
            // reseau -> connexion
            Byte[] lectureConfigAnaResponse2 = ConversionUtils.ResponseAutomateToByteArray("0000000000ED0103EA000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            dictionnary.Add(lectureConfigAnaQuestion2Str, new Byte[][] { lectureConfigAnaResponse2 });

            // ------------------------ 5th command : LECT CONFIG_ANA 3/5
            // connexion -> reseau
            Byte[] lectureConfigAnaQuestion3 = ConversionUtils.QuestionAutomateToByteArray("0103185A0075");
            string lectureConfigAnaQuestion3Str = BitConverter.ToString(lectureConfigAnaQuestion3).Replace("-", "");
            // reseau -> connexion
            Byte[] lectureConfigAnaResponse3 = ConversionUtils.ResponseAutomateToByteArray("0000000000ED0103EA000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            dictionnary.Add(lectureConfigAnaQuestion3Str, new Byte[][] { lectureConfigAnaResponse3 });

            // ------------------------ 6th command : LECT CONFIG_ANA 4/5
            // connexion -> reseau
            Byte[] lectureConfigAnaQuestion4 = ConversionUtils.QuestionAutomateToByteArray("010318CF0075");
            string lectureConfigAnaQuestion4Str = BitConverter.ToString(lectureConfigAnaQuestion4).Replace("-", "");
            // reseau -> connexion
            Byte[] lectureConfigAnaResponse4 = ConversionUtils.ResponseAutomateToByteArray("0000000000ED0103EA000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            dictionnary.Add(lectureConfigAnaQuestion4Str, new Byte[][] { lectureConfigAnaResponse4 });

            // ------------------------ 7th command : LECT CONFIG_ANA 5/5
            // connexion -> reseau
            Byte[] lectureConfigAnaQuestion5 = ConversionUtils.QuestionAutomateToByteArray("010319440048");
            string lectureConfigAnaQuestion5Str = BitConverter.ToString(lectureConfigAnaQuestion5).Replace("-", "");
            // reseau -> connexion
            Byte[] lectureConfigAnaResponse5 = ConversionUtils.ResponseAutomateToByteArray("000000000093010390000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            dictionnary.Add(lectureConfigAnaQuestion5Str, new Byte[][] { lectureConfigAnaResponse5 });

            // ------------------------ 8th command : LECT VALTR_ANA
            // connexion -> reseau
            Byte[] lectureValeurTRAnaQuestion = ConversionUtils.QuestionAutomateToByteArray("010311940078");
            string lectureValeurTRAnaQuestionStr = BitConverter.ToString(lectureValeurTRAnaQuestion).Replace("-", "");
            // reseau -> connexion
            Byte[] lectureValeurTRAnaResponse = ConversionUtils.ResponseAutomateToByteArray("0000000000F30103F0C28F41D9A99341DA3D7141D62F1B3F9D0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            dictionnary.Add(lectureValeurTRAnaQuestionStr, new Byte[][] { lectureValeurTRAnaResponse });

            // DATA

            // lecture de 3 mots (010613920003)
            Byte[] status1 = ConversionUtils.ResponseAutomateToByteArray("000000000009010306000000010000");
            Byte[] status11 = ConversionUtils.ResponseAutomateToByteArray("000000000009010306000100010078");
            Byte[] status13 = ConversionUtils.ResponseAutomateToByteArray("000000000009010306000100030078");

            // LTP
            Byte[][] getStatusQuestions = new Byte[numberOfRepetition*2][];
            getStatusQuestions[0] = availableDatasResponse;

            for (int i = 1; i < numberOfRepetition*2 -1; i+=2)
            {
                getStatusQuestions[i] = status13;
                getStatusQuestions[i+1] = status11;
            }
            getStatusQuestions[numberOfRepetition*2-1] = status13;

            dictionnary.Add(availableDatasQuestionStr, getStatusQuestions);

            // starting
            Byte[] startingTransfertQuestion = ConversionUtils.QuestionAutomateToByteArray("010613920001");
            string startingTransfertQuestionStr = BitConverter.ToString(startingTransfertQuestion).Replace("-", "");
            Byte[] startingTransfertQuestionResponse = ConversionUtils.ResponseAutomateToByteArray("000000000006010613920001");
            dictionnary.Add(startingTransfertQuestionStr, new Byte[][] { startingTransfertQuestionResponse });

            // continue transfert
            Byte[] continueTransfertQuestion = ConversionUtils.QuestionAutomateToByteArray("010613920003");
            string continueTransfertQuestionStr = BitConverter.ToString(continueTransfertQuestion).Replace("-", "");
            Byte[] continueTransfertQuestionResponse = ConversionUtils.ResponseAutomateToByteArray("000000000006010613920003");
            dictionnary.Add(continueTransfertQuestionStr, new Byte[][] { continueTransfertQuestionResponse });

            // ending
            Byte[] endingTransfertQuestion = ConversionUtils.QuestionAutomateToByteArray("010613920002");
            string endingTransfertQuestionStr = BitConverter.ToString(endingTransfertQuestion).Replace("-", "");
            Byte[] endingTransfertQuestionResponse = ConversionUtils.ResponseAutomateToByteArray("000000000006010613920002");
            dictionnary.Add(endingTransfertQuestionStr, new Byte[][] { endingTransfertQuestionResponse });

            // reading 120 mots
            Byte[] reading120motsQuestion = ConversionUtils.QuestionAutomateToByteArray("010313960078");
            string reading120motsQuestionStr = BitConverter.ToString(reading120motsQuestion).Replace("-", "");

            // LTP
            Byte[] responseAutomateToRepeat = ConversionUtils.ResponseAutomateToByteArray("0000000000F30103F0A6FD155F000340013D7141D6A739155F000340013D7141D6A739155F0004400153F83F83A775155F000340013D7141D6A776155F001D100000000000A7B1155F000340013D7141D6A7EE155F000340013D7141D6A82A155F000340013D7141D6A866155F000340013D7141D6A8A2155F000340013D7141D6A8A2155F001D100000003F80A8DE155F000340013D7141D6A91A155F000340013D7141D6A956155F000340013D7141D6A992155F000340013D7141D6A992155F000440017EFA3F8AA9CE155F001D100000000000A9CE155F000340013D7141D6AA0B155F000240017EFA41DAAA0B155F000340013D7141D6");

            Byte[][] read120motsQuestions = new Byte[numberOfRepetition][];
            for (int i = 0; i < numberOfRepetition; i++)
            {
                read120motsQuestions[i] = responseAutomateToRepeat;
            }

            dictionnary.Add(reading120motsQuestionStr, read120motsQuestions);

            return dictionnary;
        }
    }
}

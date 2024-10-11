using Reseau.Utils;

namespace Reseau.AutomateProtocol
{
    // Automate Premium ("" RU, defauts)
    public class RCProtocol : IAutomateProtocol
    {

        public readonly Dictionary<string, Byte[][]> dictionnary = CreateDictionnary();
        public int GetStatusCount { get; set; }
        public int Read120MotsCount { get; set; }

        public Byte[] CalculResponse(string question)
        {
            if (dictionnary.TryGetValue(question, out Byte[][] response))
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

        private static Dictionary<string, Byte[][]> CreateDictionnary()
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
            Byte[] lectureVersionResponse = ConversionUtils.ResponseAutomateToByteArray("00000000001301031000000000000000000000000000000000");
            dictionnary.Add(lectureVersionQuestionStr, new Byte[][] { lectureVersionResponse });

            // ------------------------ 3rd command : LECT CONFIG_ANA 1/5
            // connexion -> reseau
            Byte[] lectureConfigAnaQuestion1 = ConversionUtils.QuestionAutomateToByteArray("010317700075");
            string lectureConfigAnaQuestion1Str = BitConverter.ToString(lectureConfigAnaQuestion1).Replace("-", "");
            // reseau -> connexion
            Byte[] lectureConfigAnaResponse1 = ConversionUtils.ResponseAutomateToByteArray("0000000000ED0103EA119400050000BF800000421CD70A3CA30001119600050000BF800000421CD70A3CA30000119800050000BF800000421CCCCD3D4C0000119A00050000BF80000041A0D70A3CA30000119C00050000BF80000044FA000041200000119E00050000BF800000421CCCCD3D4C000111A400070000BF8000004248CCCD3D4C000011A600070000C120000041A0D70A3CA3000011A800070000C0C0000040A0CCCD3D4C000011AA00070000C0C0000040A0CCCD3D4C000011AC00070000BF80000042CA00004120000011AE00070000BF80000042CA00004120000011B000070000BF80000042C8000042480000");
            dictionnary.Add(lectureConfigAnaQuestion1Str, new Byte[][] { lectureConfigAnaResponse1 });

            // ------------------------ 4th command : LECT CONFIG_ANA 2/5
            // connexion -> reseau
            Byte[] lectureConfigAnaQuestion2 = ConversionUtils.QuestionAutomateToByteArray("010317E50075");
            string lectureConfigAnaQuestion2Str = BitConverter.ToString(lectureConfigAnaQuestion2).Replace("-", "");
            // reseau -> connexion
            Byte[] lectureConfigAnaResponse2 = ConversionUtils.ResponseAutomateToByteArray("0000000000ED0103EA11B200070000BF80000042C8000042480000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
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
            Byte[] lectureValeurTRAnaResponse = ConversionUtils.ResponseAutomateToByteArray("0000000000F30103F00903420E1C43420E3AFC41F49BA63C44999A40D93334420E000000000000000000000000D6F5BAAD01673BC8F162BB7B00000000000042C800004254000041700000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            dictionnary.Add(lectureValeurTRAnaQuestionStr, new Byte[][] { lectureValeurTRAnaResponse });

            // DATA

            // lecture de 3 mots (010613920003)
            Byte[] status1 = ConversionUtils.ResponseAutomateToByteArray("000000000009010306000000010000");
            Byte[] status11 = ConversionUtils.ResponseAutomateToByteArray("000000000009010306000100010078");
            Byte[] status12 = ConversionUtils.ResponseAutomateToByteArray("00000000000901030600010002006C"); // last 108 mots
            Byte[] status13 = ConversionUtils.ResponseAutomateToByteArray("000000000009010306000100030078");

            Byte[] getStatus1 = status1;
            Byte[] getStatus2 = status13;
            Byte[] getStatus3 = status13;
            Byte[] getStatus4 = status13;
            Byte[] getStatus5 = status11;
            Byte[] getStatus6 = status12;

            Byte[][] getStatusQuestions = new Byte[][] {
            availableDatasResponse,
            getStatus1,
            getStatus2,
            getStatus3,
            getStatus4,
            getStatus5,
            getStatus6,
            };
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

            Byte[][] read120motsQuestions = new Byte[][] {
            ConversionUtils.ResponseAutomateToByteArray("0000000000F30103F09DF210FD000B80010000426C9E2E10FD000B8001000042C89EE210FD000B8001000040C09F1E10FD00098080000000009F5A10FD00044001126F3D039F5A10FD0008800168D23C819F5A10FD00098100AF3C3C4C9F5A10FD000B8001000042C29F9610FD00044001126F3B839F9610FD00088001E258BC249F9610FD000B800100000000A04B10FD000B80010000424CA08710FD00044001C2903CF5A08710FD000880012B2D3C7FA08710FD000B800100004290A0C310FD000240011894420EA0C310FD000B8001000042AEA0FF10FD000B800100004298A17710FD000440019BA63BC4A17710FD00088001DCA2BC2D"),
            ConversionUtils.ResponseAutomateToByteArray("0000000000F30103F0A17710FD000B800100004210A1B310FD000B800100000000A1EF10FD000981809A493C21A1EF10FD000B8001000042C6A22B10FD000B800100004248A26710FD000B800100003F80A2A310FD0009818047983BB5A2E010FD00044001C2903CF5A2E010FD0008800107953C72A31C10FD000B800100004218A35810FD000B800100004150A39410FD000B800100004298A3D010FD000B8001000042AEA40C10FD00044001126F3B03A40C10FD00088001C2BDBC18A40C10FD000B800100000000A48410FD00054001999A4215A4C010FD00054001000040E0A4FD10FD0009818032DF3BB4A4FD10FD000B8001000042B6"),
            ConversionUtils.ResponseAutomateToByteArray("0000000000F30103F0A53910FD000B8001000041F0A57510FD0004400174BDBC93A57510FD000880017F40BD0EA57510FD000B800100000000A5ED10FD0009808000000000A66510FD00044001126F3B83A66510FD00088001AC0CBC12A66510FD0009810029603BDBA66510FD000B8001000041D0A6A110FD000B8001000042BAA6DD10FD000B800100004000A71910FD000440013959BCB4A71910FD0008800114BABD10A75510FD0009808000000000A79210FD0004400100000000A79210FD000880012A57BC77A79210FD000981005E5C3BADA80A10FD0009808000000000A88210FD0009810098FF3BEBA88210FD000B800100004254"),
            };
            dictionnary.Add(reading120motsQuestionStr, read120motsQuestions);

            // reading 108 mots
            Byte[] reading96motsQuestion = ConversionUtils.QuestionAutomateToByteArray("01031396006C");
            string reading96motsQuestionStr = BitConverter.ToString(reading96motsQuestion).Replace("-", "");
            Byte[] reading96motsQuestionResponse = ConversionUtils.ResponseAutomateToByteArray("0000000000DB0103D8A8BE10FD000B800100000000A8FA10FD00044001FDF43CD4A8FA10FD00088001DB7B3C3EA8FA10FD00098180B42A3C1AA8FA10FD000B8001000042A8A93610FD000B8001000042C0A97210FD000B800100004140A9AE10FD00044001126F3B83A9AE10FD000B800100000000AA2610FD000B8001000042C4AADB10FD000B800100004248AB1710FD000B800100003F80AB5310FD000440013959BCB4AB5310FD00088001743ABD10AB8F10FD00054001999A4201ABCB10FD00044001126F3C03ABCB10FD00054001666740E6ABCB10FD000880013095BBD2");
            dictionnary.Add(reading96motsQuestionStr, new Byte[][] { reading96motsQuestionResponse });

            return dictionnary;
        }
    }
}

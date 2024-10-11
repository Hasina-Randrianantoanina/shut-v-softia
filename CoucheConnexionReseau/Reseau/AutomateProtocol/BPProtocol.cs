using Reseau.Utils;

namespace Reseau.AutomateProtocol
{
    // Automate Premium ("" RU)
    public class BPProtocol : IAutomateProtocol
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
            Byte[] lectureConfigAnaResponse1 = ConversionUtils.ResponseAutomateToByteArray("0000000000ED0103EA119400050000BF8000004274CCCD3D4C0000119600050000BF800000426CCCCD3D4C0000119800050000BF8000004270CCCD3D4C0000119A00050000BF8000004270CCCD3D4C0000119C00050000BF8000004270D70A3CA30001119E00050000BF80000041A8CCCD3D4C000011A000050000BF8000004270D70A3CA3000011A200050000BF8000004180CCCD3D4C000011A400050000BF8000004270D70A3CA3000011A600050000BF8000004270D70A3CA3000011B400070000BF80000042C8CCCD3D4C000011B600070000BF80000042C8CCCD3D4C000011B800070000BF80000042C8CCCD3D4C0000");
            dictionnary.Add(lectureConfigAnaQuestion1Str, new Byte[][] { lectureConfigAnaResponse1 });

            // ------------------------ 4th command : LECT CONFIG_ANA 2/5
            // connexion -> reseau
            Byte[] lectureConfigAnaQuestion2 = ConversionUtils.QuestionAutomateToByteArray("010317E50075");
            string lectureConfigAnaQuestion2Str = BitConverter.ToString(lectureConfigAnaQuestion2).Replace("-", "");
            // reseau -> connexion
            Byte[] lectureConfigAnaResponse2 = ConversionUtils.ResponseAutomateToByteArray("0000000000ED0103EA11BA00070000BF80500047C300004120000011BC00070000BF80500047C300004120000011BE00070000BF804000461C00004120000011C000070000BF804000461C00004120000011C200070000BF80000042C8CCCD3D4C000011C600070000BF80000041A8CCCD3D4C000111C800070000C0A0000040A0D70A3CA3000111CA00070000C0A0000040A0D70A3CA3000111CC00070000C0A0000040A0D70A3CA3000111CE00070000BF8000004180CCCD3D4C000111D000070000C0A0000040A0D70A3CA3000111D200070000C0A0000040A0D70A3CA3000111D400070000C0A0000040A0D70A3CA30001");
            dictionnary.Add(lectureConfigAnaQuestion2Str, new Byte[][] { lectureConfigAnaResponse2 });

            // ------------------------ 5th command : LECT CONFIG_ANA 3/5
            // connexion -> reseau
            Byte[] lectureConfigAnaQuestion3 = ConversionUtils.QuestionAutomateToByteArray("0103185A0075");
            string lectureConfigAnaQuestion3Str = BitConverter.ToString(lectureConfigAnaQuestion3).Replace("-", "");
            // reseau -> connexion
            Byte[] lectureConfigAnaResponse3 = ConversionUtils.ResponseAutomateToByteArray("0000000000ED0103EA11E200070000BF80000042CA000042C8000111E400070000BF80000042CA000042CA000111E600070000BF80000042CA000042C8000111E800070000BF80000042CA000042C8000111EA00070000BF80000042CA000042C8000111EC00070000BF80000042CA000042C8000111D600020000BF80000042CA000042C8000011D800020000BF80000042CA000042C8000011DA00020000BF80000042CA000042C8000011DC00020000BF80000042CA000042C8000011DE00020000BF80000042CA000042C8000011E000020000BF80000042CA000042C80000000000000000000000000000000000000000");
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
            Byte[] lectureValeurTRAnaResponse = ConversionUtils.ResponseAutomateToByteArray("0000000000F30103F010CC424D2B6B425BE872424C71AA424B1DB24251B646BE7306A7425651ECBE384BC7425813DD425400000000000000000000000000000000000000000000000008313CAC00000000000000007B0043164D004525EA0042739A804347126F3B03000000000000000013EF3DA100000000000000000000000000000000000000000000000000004218000042B2000042AE000042B0000042AC000042A6000042C80000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            dictionnary.Add(lectureValeurTRAnaQuestionStr, new Byte[][] { lectureValeurTRAnaResponse });

            // DATA

            // lecture de 3 mots (010613920003)
            Byte[] status1 = ConversionUtils.ResponseAutomateToByteArray("000000000009010306000000010000");
            Byte[] status11 = ConversionUtils.ResponseAutomateToByteArray("000000000009010306000100010078");
            Byte[] status12 = ConversionUtils.ResponseAutomateToByteArray("000000000009010306000100020060"); // last 96 mots
            Byte[] status13 = ConversionUtils.ResponseAutomateToByteArray("000000000009010306000100030078");

            Byte[] getStatus1 = status1;
            Byte[] getStatus2 = status13;
            Byte[] getStatus3 = status11;
            Byte[] getStatus4 = status13;
            Byte[] getStatus5 = status11;
            Byte[] getStatus6 = status13;
            Byte[] getStatus7 = status11;
            Byte[] getStatus8 = status13;
            Byte[] getStatus9 = status11;
            Byte[] getStatus10 = status13;
            Byte[] getStatus11 = status11;
            Byte[] getStatus12 = status13;
            Byte[] getStatus13 = status11;
            Byte[] getStatus14 = status13;
            Byte[] getStatus15 = status11;
            Byte[] getStatus16 = status12;

            Byte[][] getStatusQuestions = new Byte[][] {
            availableDatasResponse,
            getStatus1,
            getStatus2,
            getStatus3,
            getStatus4,
            getStatus5,
            getStatus6,
            getStatus7,
            getStatus8,
            getStatus9,
            getStatus10,
            getStatus11,
            getStatus12,
            getStatus13,
            getStatus14,
            getStatus15,
            getStatus16
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
            ConversionUtils.ResponseAutomateToByteArray("0000000000F30103F01E84110700034001D2F1424A1EFC1107000340013439424B1EFC1107000E8001E80042AC1F741107000340017DF3424B1F741107000E80015E0042C91F941107005E100000003F801FB11107000E8001D20042E51FED110700034001BF7D424B20291107000E8001400043082065110700034001F8D5424B20A21107000E80017B00431620DE1107000140011F8A424D20DE1107000340012D0E424C20DE1107000440011894424A211A1107000E8001D200432B217F11070045100000003F80218A11070042100000003F8021931107000340016B85424C21CF1107000E80010D00433A220B110700054200147B4251"),
            ConversionUtils.ResponseAutomateToByteArray("0000000000F30103F0220B11070013820000000000220B1107001482008AC23D8E220B11070015820000000000220B11070016820000000000220B11070017820000000000220B11070018820000000000220B11070019820000000000220B1107001A820000000000220B1107001B820000004240220B1107001C820000004140220B1107001D820000004140220B1107001E820000004170220B1107001F820000004110220B1107002082000000416022841107000E8001D200432B22AF1107005E10000000000022FC1107000340019EB8424C23391107000E80019800431D23B11107000E80015D00430F24291107000E800123004301"),
            ConversionUtils.ResponseAutomateToByteArray("0000000000F30103F024A2110700014001E52C424C24A21107000E8001D20042E5251A1107000340013127424C2556110700034001F9DB424B2593110700034001B74B424B25931107000E80015E0042C925CF11070003400178D5424B260B1107000340013439424B2647110700034001E147424A267E11070042100000000000267E1107004510000000000026841107000340018A3D424A2738110700034001D3F8424A27B11107000340012B02424B282911070003400176C8424B28651107000E8001D20042E528A2110700034001B852424B28A21107000E800123004301291A110700034001F0A4424B291A1107000440014ED9424A"),
            ConversionUtils.ResponseAutomateToByteArray("0000000000F30103F0291A1107000E80015D00430F292D1107005B100000003F8029561107000140011C78424D29561107000E80019800431D29CF1107000340013B64424C29CF1107000E8001D200432B2A4D11070045100000003F802A5711070048100000003F802A8411070003400173B6424C2A841107000E80012A0043412B381107000E8001F00043322B3F1107005B1000000000002BED1107000E8001B50043242C651107000E80017B0043162CDE1107000E8001400043082D561107000E80010C0042F42DCF110700014001E0DF424C2E0B1107000340011168424C2E0B1107000E8001960042D72E47110700034001C8B4424B"),
            ConversionUtils.ResponseAutomateToByteArray("0000000000F30103F02E83110700034001872B424B2EC01107000340013D70424B2EFC110700034001E76C424A2F2F110700451000000000002F2F110700481000000000002F381107000340018831424A2F381107000440018313424A2FED110700034001D0E5424A30291107000340010418424B30291107000E8001220042BB30A21107000340014BC6424B30DE1107000E8001D20042E5311A1107000340018625424B31561107000E8001230043013192110700034001BF7D424B31921107000E80015D00430F31CA1107005E100000003F8031CF1107000140011653424D320B1107000E8001B500432432471107000340010C49424C"),
            ConversionUtils.ResponseAutomateToByteArray("0000000000F30103F032831107000E8001F000433232FC1107000140014D02424D32FC1107000340014BC6424C32FC1107000E80012A004341336811070045100000003F80337311070042100000003F8033ED1107000340018625424C34E41107005E100000000000351A1107000E8001F0004332360B1107000E8001B500432436C0110700044001BB65424A36FC11070001400115B6424D36FC1107000E80017B00431637ED1107000E80014000430838A21107000E80010C0042F43993110700014001DFA5424C39931107000340013B64424C3A0B110700034001F2B0424B3A0B1107000E8001960042D73A841107000340019893424B"),
            ConversionUtils.ResponseAutomateToByteArray("0000000000F30103F03AFC11070003400145A2424B3B741107005B100000003F803B75110700034001F3B6424A3BE6110700421000000000003BE6110700451000000000003BED1107000340019168424A3C65110700034001C9BA424A3CA2110700044001F4BD424A3CDE1107000340011CAC424B3D561107000340015D2F424B3D561107000E8001230043013DCF1107000140011519424D3DCF1107000340019893424B3DCF1107000E80017B0043163E0B1107000E8001B50043243E541107005B1000000000003E84110700034001EA7F424B3E841107000E80010D00433A3EC01107000140014F77424D3F381107000340013439424C")
            };
            dictionnary.Add(reading120motsQuestionStr, read120motsQuestions);

            // reading 96 mots
            Byte[] reading96motsQuestion = ConversionUtils.QuestionAutomateToByteArray("010313960060");
            string reading96motsQuestionStr = BitConverter.ToString(reading96motsQuestion).Replace("-", "");
            Byte[] reading96motsQuestionResponse = ConversionUtils.ResponseAutomateToByteArray("0000000000C30103C03F381107000E8001470043483FB11107000E8001810043563FE511070045100000003F803FED1107000340016C8B424C3FEF11070048100000003F80411A110700034001A5E3424C42481107000440012F1B424B42841107000E80014700434843ED110700034001DE35424C44A21107000E80010D00433A44EF1107005E100000003F8046481107000E8001D200432B47751107000E80019800431D47ED1107000440016667424B48111107005E100000000000482A1107000140011AA0424D");
            dictionnary.Add(reading96motsQuestionStr, new Byte[][] { reading96motsQuestionResponse });

            return dictionnary;
        }
    }
}

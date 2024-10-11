using CoucheLectureMan.Utils;

namespace CoucheLectureMan.Entities.Infos
{
    public class InfoTC : Info
    {
        //  Bit index       Meaning
        //      0       ApparitionDefautBattement
        //      1       DisparitionDefautBattement
        //      2       EnregistrementAMinuit
        //      3
        //      4
        //      5
        //      6
        //      7
        //      8
        //      9
        //      10
        public bool ApparitionDefautBattement { get; set; }
        public bool DisparitionDefautBattement { get; set; } // Not sure
        public bool EnregistrementAMinuit { get; set; }
        public InfoTC(UInt16 value)
        {
            Value = value;
            TypeMesure = "TC";
            ApparitionDefautBattement = ConversionUtils.IsThisBitValueOne(value, 0);
            DisparitionDefautBattement = ConversionUtils.IsThisBitValueOne(value, 1);
            EnregistrementAMinuit = ConversionUtils.IsThisBitValueOne(value, 2);
        }

        public InfoTC(Int16 value)
        {
            Value = value;
            TypeMesure = "TC";
            ApparitionDefautBattement = ConversionUtils.IsThisBitValueOne(value, 0);
            DisparitionDefautBattement = ConversionUtils.IsThisBitValueOne(value, 1);
            EnregistrementAMinuit = ConversionUtils.IsThisBitValueOne(value, 2);
        }

        public override string ToString()
        {
            string print = "";
            if (ApparitionDefautBattement) print +=  " ApparitionDefautBattement";
            if (DisparitionDefautBattement) print +=  " DisparitionDefautBattement";
            if (EnregistrementAMinuit) print += " EnregistrementAMinuit";
            if (print.Length > 0) print = ", events=" + print;

            return $"Info[val={ConversionUtils.IntToStringHexa4(Value)}, type={TypeMesure}{print}] ";
        }
    }
}

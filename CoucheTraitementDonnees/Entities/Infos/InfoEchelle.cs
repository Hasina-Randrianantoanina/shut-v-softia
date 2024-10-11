using CoucheTraitementDonnees.Utils;

namespace CoucheTraitementDonnees.Entities.Infos
{
    public class InfoEchelle : Info
    {
        //  Bit index       Meaning
        //      0       
        //      1       
        //      2       EnregistrementAMinuit
        //      3
        //      4
        //      5
        //      6
        //      7
        //      8
        //      9
        //      10
        public bool EnregistrementAMinuit { get; set; } // Useless ?
        public InfoEchelle(UInt16 value)
        {
            Value = value;
            TypeMesure = "ECH";
            EnregistrementAMinuit = ConversionUtils.IsThisBitValueOne(value, 2);
        }

        public InfoEchelle(Int16 value)
        {
            Value = value;
            TypeMesure = "ECH";
            EnregistrementAMinuit = ConversionUtils.IsThisBitValueOne(value, 2);
        }

        public override string ToString()
        {
            string print = "";
            if (EnregistrementAMinuit) print += " EnregistrementAMinuit";
            if (print.Length > 0) print = ", events=" + print;

            return $"Info[val={ConversionUtils.IntToStringHexa4(Value)}, type={TypeMesure}{print}] ";
        }
    }
}

using CoucheTraitementDonnees.Utils;

namespace CoucheTraitementDonnees.Entities.Infos
{
    public class InfoAnalogicCalcul : Info
    {
        //  Bit index       Meaning
        //      0       DepassementDelta
        //      1       DepassementSeuil
        //      2       EnregistrementAMinuit
        //      3
        //      4
        //      5
        //      6
        //      7       DebutDefaut
        //      8       FinDefaut
        //      9       EnregistrementSurDefautGroupe
        //      10
        public bool DepassementDelta { get; set; }
        public bool DepassementSeuil { get; set; }
        public bool EnregistrementAMinuit { get; set; }
        public bool DebutDefaut { get; set; }
        public bool FinDefaut { get; set; }
        public bool EnregistrementSurDefautGroupe { get; set; }

        public InfoAnalogicCalcul(UInt16 value)
        {
            Value = value;
            TypeMesure = "CAL";
            DepassementDelta = ConversionUtils.IsThisBitValueOne(value, 0);
            DepassementSeuil = ConversionUtils.IsThisBitValueOne(value, 1);
            EnregistrementAMinuit = ConversionUtils.IsThisBitValueOne(value, 2);
            DebutDefaut = ConversionUtils.IsThisBitValueOne(value, 7);
            FinDefaut = ConversionUtils.IsThisBitValueOne(value, 8);
            EnregistrementSurDefautGroupe = ConversionUtils.IsThisBitValueOne(value, 9);
        }

        public InfoAnalogicCalcul(Int16 value)
        {
            Value = value;
            TypeMesure = "CAL";
            DepassementDelta = ConversionUtils.IsThisBitValueOne(value, 0);
            DepassementSeuil = ConversionUtils.IsThisBitValueOne(value, 1);
            EnregistrementAMinuit = ConversionUtils.IsThisBitValueOne(value, 2);
            DebutDefaut = ConversionUtils.IsThisBitValueOne(value, 7);
            FinDefaut = ConversionUtils.IsThisBitValueOne(value, 8);
            EnregistrementSurDefautGroupe = ConversionUtils.IsThisBitValueOne(value, 9);
        }

        public override string ToString()
        {
            string print = "";
            if (DepassementDelta) print +=  " DepassementDelta";
            if (DepassementSeuil) print +=  " DepassementSeuil";
            if (EnregistrementAMinuit) print += " EnregistrementAMinuit";
            if (DebutDefaut) print += " DebutDefaut";
            if (FinDefaut) print += " FinDefaut";
            if (EnregistrementSurDefautGroupe) print += " EnregistrementSurDefautGroupe";
            if (print.Length > 0) print = ", events=" + print;

            return $"Info[val={ConversionUtils.IntToStringHexa4(Value)}, type={TypeMesure}{print}] ";
        }
    }
}

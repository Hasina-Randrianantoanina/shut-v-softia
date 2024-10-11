namespace CoucheTraitementDonnees.Entities.Infos
{
    public class Info
    {
        public int Value { get; set; }
        public string TypeMesure { get; set; }

        public static Info GetInfoOfThisMesure(string typeMesure, UInt16 value)
        {
            //  Bit index       Meaning
            //      11      TypeMesure = ECH
            //      12      TypeMesure = TS
            //      13      TypeMesure = TC
            //      14      TypeMesure = CAP
            //      15      TypeMesure = CAL
            switch (typeMesure)
            {
                case "CAL":
                    return new InfoAnalogicCalcul(value);
                case "CAP":
                    return new InfoAnalogicCapteur(value);
                case "TC":
                    return new InfoTC(value);
                case "TS":
                    return new InfoTS(value);
                case "ECH":
                    return new InfoEchelle(value);
                default:
                    return null;
            }
        }

        public static Info GetInfoOfThisMesure(string typeMesure, Int16 value)
        {
            //  Bit index       Meaning
            //      11      TypeMesure = ECH
            //      12      TypeMesure = TS
            //      13      TypeMesure = TC
            //      14      TypeMesure = CAP
            //      15      TypeMesure = CAL
            switch (typeMesure)
            {
                case "CAL":
                    return new InfoAnalogicCalcul(value);
                case "CAP":
                    return new InfoAnalogicCapteur(value);
                case "TC":
                    return new InfoTC(value);
                case "TS":
                    return new InfoTS(value);
                case "ECH":
                    return new InfoEchelle(value);
                default:
                    return null;
            }
        }

    }
}
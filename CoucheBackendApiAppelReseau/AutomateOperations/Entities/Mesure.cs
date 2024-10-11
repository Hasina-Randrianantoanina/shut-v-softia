using AutomateOperations.Entities.Infos;

namespace AutomateOperations.Entities
{
    public class Mesure
    {
        public DateTime Time { get; set; }
        public string NomVoieM580 { get; set; }  // M580 version D16
        public string NomVoieBase { get; set; }
        public int NumeroVoie { get; set; }
        public float ValeurALEchelle { get; set; }
        public Info Info { get; set; }
        public string Evenement { get; set; }

        public string CaculateEvenement()
        {
            switch (Info.TypeMesure) 
            {
                case "CAL":
                    InfoAnalogicCalcul infoCAL = (InfoAnalogicCalcul)Info;
                    if (infoCAL.DebutDefaut || infoCAL.FinDefaut) { return "$"; }
                    if (infoCAL.EnregistrementAMinuit) { return "J"; }
                    return ValeurALEchelle >= 0 ? " " : "-"; // DepassementDelta, DepassementSeuil, EnregistrementSurDefautGroupe
                case "CAP":
                    InfoAnalogicCapteur infoCAP = (InfoAnalogicCapteur)Info;
                    if (infoCAP.DebutDefaut || infoCAP.FinDefaut) { return "$"; }
                    if (infoCAP.EnregistrementAMinuit) { return "J"; }
                    return ValeurALEchelle >= 0 ? " " : "-"; // DepassementDelta, DepassementSeuil, EnregistrementSurDefautGroupe
                case "TC":
                    InfoTC infoTC = (InfoTC)Info;
                    if (infoTC.ApparitionDefautBattement || infoTC.DisparitionDefautBattement) { return "$"; }
                    if (infoTC.EnregistrementAMinuit) { return "o"; }
                    return "c";
                case "TS":
                    InfoTS infoTS = (InfoTS)Info;
                    if (infoTS.ApparitionDefautBattement || infoTS.DisparitionDefautBattement) { return "$"; }
                    if (infoTS.EnregistrementAMinuit) { return "r"; }
                    return "s";
                case "ECH100%":
                    return "m";
                case "ECH0%":
                    return "k";
                default:
                    return string.Empty;
            }
        }

        public string? GetNomVoieBase(Station station) 
        {
            switch (Info.TypeMesure)
            {
                case "ECH100%":
                case "ECH0%":
                case "CAL":
                case "CAP":
                    foreach (VoieTelemesuree voie in station.VoiesTelemesurees) 
                    {
                        if (voie.Numero == NumeroVoie) { return voie.Libelle; }
                    }
                    return null;
                case "TC":
                case "TS":
                    foreach (VoieTOR voie in station.VoiesTOR)
                    {
                        if (voie.Numero == NumeroVoie) { return voie.Libelle; }
                    }
                    return null;
                default:
                    return null;
            }
        }
    }
}

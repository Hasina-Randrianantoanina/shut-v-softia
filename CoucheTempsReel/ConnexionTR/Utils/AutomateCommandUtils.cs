using ConnexionTR.Entities;

namespace ConnexionTR.Utils
{
    public static class AutomateCommandUtils
    {
        /// <summary>
        /// Array length is 25
        /// </summary>
        public static string GetVersion(Byte[] array)
        {
            return ConversionUtils.ByteArrayToString(array, 9);
        }

        /// <summary>
        /// M580 : Array length is 249 (120 mots * 2 bytes + 9)
        /// <br/>
        /// Premium : Array length is 243 (117 mots * 2 bytes + 9) or 153 (72 mots * 2 bytes + 9)
        /// </summary>
        public static List<VoieInterne> GetAnalogicConfig(string initiales, Byte[] array, int startingIndex, int endingIndex, int nbBytesPerVoie)
        {
            List<VoieInterne> voieInternes = new List<VoieInterne>();

            int byteIndex = 9; // début table

            for (int i = startingIndex; i < endingIndex; i++)
            {
                Int16 info = ConversionUtils.ByteToInt16(array, byteIndex+2);
                int numero = i+1;

                VoieInterne voie = new VoieInterne
                {
                    //InitialesStation = initiales,
                    Numero = numero,
                    AdresseBES = ConversionUtils.ByteToInt16(array, byteIndex), // Adresse de la donnée dans table d'échange BES
                    Info = info, // informations sur la variable
                    SeuilBas = ConversionUtils.ByteToFloat(array, byteIndex+4),
                    SeuilHaut = ConversionUtils.ByteToFloat(array, byteIndex+8),
                    ValeurDelta = ConversionUtils.ByteToFloat(array, byteIndex+12),
                    Groupe = ConversionUtils.ByteToInt16(array, byteIndex+16),

                    // VisuNet specific
                    VoieEnregistree = ConversionUtils.IsThisBitValueOne(info, 0) ? numero : 0, // enregistrer
                    Capteur = ConversionUtils.IsThisBitValueOne(info, 1), // capteur
                    EnregistrementMinuit = ConversionUtils.IsThisBitValueOne(info, 2), // enrg minuit
                    SensSeuil = ConversionUtils.IsThisBitValueOne(info, 3), // Sens seuil
                    EnregistrementStandard = ConversionUtils.IsThisBitValueOne(info, 4), // enrg standard

                };
                voieInternes.Add(voie);

                byteIndex += nbBytesPerVoie;
            }

            return voieInternes;
        }
    }
}

using CoucheLectureMan.Entities;

namespace CoucheLectureMan.Utils
{
    public static class DataBaseUtils
    {
        public static Station GetStation(string initiales)
        {
            string typeLiaison;
            string version;
            int numero;

            switch (initiales)
            {
                case "BP":  // Premium "" RU
                    typeLiaison = "AP";
                    version = "MANQUE";
                    numero = 145;
                    break;
                case "EN": // Premium D13 RU
                    typeLiaison = "AP";
                    version = "D13A53Aa01";
                    numero = 20;
                    break;
                case "GA": // M580 D15 RU
                    typeLiaison = "AP";
                    version = "D15A55Ca01";
                    numero = 120;
                    break;
                case "HE": // M580 D16 RU
                    version = "D16A56Ca01";
                    typeLiaison = "AP";
                    numero = 90;
                    break;
                case "RC": // Premium "" RU with Defauts
                    typeLiaison = "AP";
                    version = "MANQUE";
                    numero = 104;
                    break;
                case "172": // M580 D15 RO
                    typeLiaison = "AP";
                    version = "D15A55Ca01";
                    numero = 172;
                    break;
                case "178": // M580 D15 RO with Mesure ECH
                    typeLiaison = "AP";
                    version = "D15A55Ca01";
                    numero = 178;
                    break;
                default:
                    Console.WriteLine("---- Station unknown----");
                    return null;
            }

            return new Station
            {
                Initiales = initiales,
                Numero = numero,
                TypeLiaison = typeLiaison,
                Version = version,
                VoiesInternes = GetVoiesInternes(initiales)
            };
        }

        private static List<VoieInterne> GetVoiesInternes(string initiales)
        {
            List<VoieInterne> voiesInternes = new List<VoieInterne>();

            switch (initiales)
            {
                case "BP":  // Premium "" RU
                    voiesInternes.Add(new VoieInterne { Libelle = "YBA_B1_BP", Virgule = 2, NumeroVoie = 1 });
                    voiesInternes.Add(new VoieInterne { Libelle = "YBA_B2_BP", Virgule = 2, NumeroVoie = 2 });
                    voiesInternes.Add(new VoieInterne { Libelle = "YBP_BP1_BP", Virgule = 2, NumeroVoie = 3 });
                    voiesInternes.Add(new VoieInterne { Libelle = "YBP_BP2_BP", Virgule = 2, NumeroVoie = 4 });
                    voiesInternes.Add(new VoieInterne { Libelle = "YCO_POU_BP", Virgule = 2, NumeroVoie = 5 });
                    voiesInternes.Add(new VoieInterne { Libelle = "QCO_POU_BP", Virgule = 2, NumeroVoie = 6 });
                    voiesInternes.Add(new VoieInterne { Libelle = "YCO_MARE_BP", Virgule = 2, NumeroVoie = 7 });
                    voiesInternes.Add(new VoieInterne { Libelle = "QCO_MARE_BP", Virgule = 2, NumeroVoie = 8 });
                    voiesInternes.Add(new VoieInterne { Libelle = "YAV_V6_BP", Virgule = 2, NumeroVoie = 9 });
                    voiesInternes.Add(new VoieInterne { Libelle = "YCO_CANAL_BP", Virgule = 2, NumeroVoie = 10 });
                    voiesInternes.Add(new VoieInterne { Libelle = "QBP_EU_BP", Virgule = 2, NumeroVoie = 11 });
                    voiesInternes.Add(new VoieInterne { Libelle = "QBP_EP_BP", Virgule = 2, NumeroVoie = 12 });
                    voiesInternes.Add(new VoieInterne { Libelle = "QTR_V6_BP", Virgule = 2, NumeroVoie = 13 });
                    voiesInternes.Add(new VoieInterne { Libelle = "VOL_B1_BP", Virgule = 0, NumeroVoie = 14 });
                    voiesInternes.Add(new VoieInterne { Libelle = "VOL_B2_BP", Virgule = 0, NumeroVoie = 15 });
                    voiesInternes.Add(new VoieInterne { Libelle = "VOL_MARE_BP", Virgule = 0, NumeroVoie = 16 });
                    voiesInternes.Add(new VoieInterne { Libelle = "VOL_POU_BP", Virgule = 0, NumeroVoie = 17 });
                    voiesInternes.Add(new VoieInterne { Libelle = "QBP_DRAIN_BP", Virgule = 2, NumeroVoie = 18 });
                    voiesInternes.Add(new VoieInterne { Libelle = "QCO_POU_UF_BP", Virgule = 2, NumeroVoie = 19 });
                    voiesInternes.Add(new VoieInterne { Libelle = "VCO_1POU_BP", Virgule = 3, NumeroVoie = 20 });
                    voiesInternes.Add(new VoieInterne { Libelle = "VCO_2POU_BP", Virgule = 3, NumeroVoie = 21 });
                    voiesInternes.Add(new VoieInterne { Libelle = "VCO_3POU_BP", Virgule = 3, NumeroVoie = 22 });
                    voiesInternes.Add(new VoieInterne { Libelle = "QCO_MARE_UF_BP", Virgule = 2, NumeroVoie = 23 });
                    voiesInternes.Add(new VoieInterne { Libelle = "VCO_1MARE_BP", Virgule = 3, NumeroVoie = 24 });
                    voiesInternes.Add(new VoieInterne { Libelle = "VCO_2MARE_BP", Virgule = 3, NumeroVoie = 25 });
                    voiesInternes.Add(new VoieInterne { Libelle = "VCO_3MARE_BP", Virgule = 3, NumeroVoie = 26 });
                    voiesInternes.Add(new VoieInterne { Libelle = "IQ_1POU_BP", Virgule = 1, NumeroVoie = 27 });
                    voiesInternes.Add(new VoieInterne { Libelle = "IQ_2POU_BP", Virgule = 1, NumeroVoie = 28 });
                    voiesInternes.Add(new VoieInterne { Libelle = "IQ_3POU_BP", Virgule = 1, NumeroVoie = 29 });
                    voiesInternes.Add(new VoieInterne { Libelle = "IQ_1MARE_BP", Virgule = 1, NumeroVoie = 30 });
                    voiesInternes.Add(new VoieInterne { Libelle = "IQ_2MARE_BP", Virgule = 1, NumeroVoie = 31 });
                    voiesInternes.Add(new VoieInterne { Libelle = "IQ_3MARE_BP", Virgule = 1, NumeroVoie = 32 });
                    break;
                case "EN":  // Premium D13 RU
                    voiesInternes.Add(new VoieInterne { Libelle = "YAM_VS_EN", Virgule = 2, NumeroVoie = 1 });
                    voiesInternes.Add(new VoieInterne { Libelle = "YAV_VS_EN", Virgule = 2, NumeroVoie = 2 });
                    voiesInternes.Add(new VoieInterne { Libelle = "YAV_V1_EN", Virgule = 2, NumeroVoie = 3 });
                    voiesInternes.Add(new VoieInterne { Libelle = "QAV_V1_EN", Virgule = 2, NumeroVoie = 4 });
                    break;
                case "GA": // M580 D15 RU
                    voiesInternes.Add(new VoieInterne { Libelle = "YAM_V1_GA", Virgule = 2, NumeroVoie = 1 });
                    voiesInternes.Add(new VoieInterne { Libelle = "YAM_V2_GA", Virgule = 2, NumeroVoie = 2 });
                    voiesInternes.Add(new VoieInterne { Libelle = "YAM_V3_GA", Virgule = 2, NumeroVoie = 3 });
                    voiesInternes.Add(new VoieInterne { Libelle = "YAM_DV3_GA", Virgule = 2, NumeroVoie = 4 });
                    voiesInternes.Add(new VoieInterne { Libelle = "YBA_B_GA", Virgule = 2, NumeroVoie = 5 });
                    voiesInternes.Add(new VoieInterne { Libelle = "POV_V1_GA", Virgule = 2, NumeroVoie = 6 });
                    voiesInternes.Add(new VoieInterne { Libelle = "POV_V2_GA", Virgule = 2, NumeroVoie = 7 });
                    voiesInternes.Add(new VoieInterne { Libelle = "POV_V3_GA", Virgule = 2, NumeroVoie = 8 });
                    break;
                case "HE": // M580 D16 RU
                    voiesInternes.Add(new VoieInterne { Libelle = "YCO_1DPLB_HE", Virgule = 2, NumeroVoie = 1 });
                    voiesInternes.Add(new VoieInterne { Libelle = "YCO_2DPLB_HE", Virgule = 2, NumeroVoie = 2 });
                    voiesInternes.Add(new VoieInterne { Libelle = "QCO_DPLB_HE", Virgule = 2, NumeroVoie = 3 });
                    voiesInternes.Add(new VoieInterne { Libelle = "QCO_DPLB_UF_HE", Virgule = 2, NumeroVoie = 4 });
                    voiesInternes.Add(new VoieInterne { Libelle = "VCO_1DPLB_HE", Virgule = 3, NumeroVoie = 5 });
                    voiesInternes.Add(new VoieInterne { Libelle = "VCO_2DPLB_HE", Virgule = 3, NumeroVoie = 6 });
                    voiesInternes.Add(new VoieInterne { Libelle = "VCO_3DPLB_HE", Virgule = 3, NumeroVoie = 7 });
                    voiesInternes.Add(new VoieInterne { Libelle = "VCO_4DPLB_HE", Virgule = 3, NumeroVoie = 8 });
                    voiesInternes.Add(new VoieInterne { Libelle = "VCO_5DPLB_HE", Virgule = 3, NumeroVoie = 9 });
                    voiesInternes.Add(new VoieInterne { Libelle = "VCO_6DPLB_HE", Virgule = 3, NumeroVoie = 10 });
                    voiesInternes.Add(new VoieInterne { Libelle = "VCO_7DPLB_HE", Virgule = 3, NumeroVoie = 11 });
                    voiesInternes.Add(new VoieInterne { Libelle = "G_1DPLB_HE", Virgule = 0, NumeroVoie = 12 });
                    voiesInternes.Add(new VoieInterne { Libelle = "G_2DPLB_HE", Virgule = 0, NumeroVoie = 13 });
                    voiesInternes.Add(new VoieInterne { Libelle = "G_3DPLB_HE", Virgule = 0, NumeroVoie = 14 });
                    voiesInternes.Add(new VoieInterne { Libelle = "G_4DPLB_HE", Virgule = 0, NumeroVoie = 15 });
                    voiesInternes.Add(new VoieInterne { Libelle = "G_5DPLB_HE", Virgule = 0, NumeroVoie = 16 });
                    voiesInternes.Add(new VoieInterne { Libelle = "G_6DPLB_HE", Virgule = 0, NumeroVoie = 17 });
                    voiesInternes.Add(new VoieInterne { Libelle = "G_7DPLB_HE", Virgule = 0, NumeroVoie = 18 });
                    voiesInternes.Add(new VoieInterne { Libelle = "IQ_1DPLB_HE", Virgule = 0, NumeroVoie = 19 });
                    voiesInternes.Add(new VoieInterne { Libelle = "IQ_2DPLB_HE", Virgule = 0, NumeroVoie = 20 });
                    voiesInternes.Add(new VoieInterne { Libelle = "IQ_3DPLB_HE", Virgule = 0, NumeroVoie = 21 });
                    voiesInternes.Add(new VoieInterne { Libelle = "IQ_4DPLB_HE", Virgule = 0, NumeroVoie = 22 });
                    voiesInternes.Add(new VoieInterne { Libelle = "IQ_5DPLB_HE", Virgule = 0, NumeroVoie = 23 });
                    voiesInternes.Add(new VoieInterne { Libelle = "IQ_6DPLB_HE", Virgule = 0, NumeroVoie = 24 });
                    voiesInternes.Add(new VoieInterne { Libelle = "IQ_7DPLB_HE", Virgule = 0, NumeroVoie = 25 });
                    voiesInternes.Add(new VoieInterne { Libelle = "YCO_UF_HE", Virgule = 2, NumeroVoie = 26 });
                    break;
                case "RC": // Premium "" RU with Defauts
                    voiesInternes.Add(new VoieInterne { Libelle = "YEP_MARNE_RC", Virgule = 2, NumeroVoie = 1 });
                    voiesInternes.Add(new VoieInterne { Libelle = "YAM_V2_RC", Virgule = 2, NumeroVoie = 2 });
                    voiesInternes.Add(new VoieInterne { Libelle = "YBP_BP_RC", Virgule = 2, NumeroVoie = 3 });
                    voiesInternes.Add(new VoieInterne { Libelle = "QEP_MARNE_RC", Virgule = 2, NumeroVoie = 4 });
                    voiesInternes.Add(new VoieInterne { Libelle = "TEP_MARNE_RC", Virgule = 0, NumeroVoie = 5 });
                    voiesInternes.Add(new VoieInterne { Libelle = "YCO_MARNE_RC", Virgule = 2, NumeroVoie = 6 });
                    voiesInternes.Add(new VoieInterne { Libelle = "QBP_EP_RC", Virgule = 2, NumeroVoie = 7 });
                    voiesInternes.Add(new VoieInterne { Libelle = "QEP_MARN_UF_RC", Virgule = 2, NumeroVoie = 8 });
                    voiesInternes.Add(new VoieInterne { Libelle = "VEP_1MARNE_RC", Virgule = 3, NumeroVoie = 9 });
                    voiesInternes.Add(new VoieInterne { Libelle = "VEP_2MARNE_RC", Virgule = 3, NumeroVoie = 10 });
                    voiesInternes.Add(new VoieInterne { Libelle = "IQ_1MARNE_RC", Virgule = 0, NumeroVoie = 11 });
                    voiesInternes.Add(new VoieInterne { Libelle = "IQ_2MARNE_RC", Virgule = 0, NumeroVoie = 12 });
                    voiesInternes.Add(new VoieInterne { Libelle = "G_1MARNE_RC", Virgule = 0, NumeroVoie = 13 });
                    voiesInternes.Add(new VoieInterne { Libelle = "G_2MARNE_RC", Virgule = 0, NumeroVoie = 14 });
                    break;
                case "172": // M580 D15 RO
                    voiesInternes.Add(new VoieInterne { Libelle = "YUN_OV_172", Virgule = 2, NumeroVoie = 1 });
                    voiesInternes.Add(new VoieInterne { Libelle = "VUN_1OV_172", Virgule = 3, NumeroVoie = 2 });
                    break;
                case "178": // M580 D15 RO with Mesure ECH
                    voiesInternes.Add(new VoieInterne { Libelle = "YEP_OV_178", Virgule = 2, NumeroVoie = 1 });
                    voiesInternes.Add(new VoieInterne { Libelle = "YAM_DVEUEP_218", Virgule = 2, NumeroVoie = 2 });
                    break;
            }

            return voiesInternes;
        }
    }
}

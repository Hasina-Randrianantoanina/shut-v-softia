using Connexion.Database;
using Connexion.Entities;

namespace Connexion.Utils
{
    public static class DataBaseUtils
    {
        public static Station GetStation(string initiales)
        {
            using ReseauService reseauService = new ReseauService();
            return reseauService.GetOneStation(initiales);
        }

        public static Station GetStationTest(string initiales, bool testVM)
        {
            if (testVM) { return GetStationTestVM(initiales); }

            string dernierTransfertFichiersJours;

            switch (initiales)
            {
                case "BP": // Premium "" RU
                    dernierTransfertFichiersJours = "20/01/2022 11:17:00";
                    break;
                case "EN": // Premium D13 RU
                    dernierTransfertFichiersJours = "13/05/2024 08:22:00";
                    break;
                case "GA": // M580 D15 RU
                    dernierTransfertFichiersJours = "31/12/2021 21:43:00";
                    break;
                case "HE": // M580 D16 RU
                    dernierTransfertFichiersJours = "01/01/2022 02:20:00";
                    break;
                case "RC": // Premium "" RU with Defauts
                    dernierTransfertFichiersJours = "13/01/2022 06:20:00";
                    break;
                case "172": // M580 D15 RO
                    dernierTransfertFichiersJours = "28/12/2021 13:14:00";
                    break;
                case "178": // M580 D15 RO with Mesure ECH
                    dernierTransfertFichiersJours = "29/12/2021 13:06:00";
                    break;
                case "LTP": // Load Test Premium
                    dernierTransfertFichiersJours = "01/01/2000 12:00:00";
                    break;
                default:
                    Console.WriteLine("\n---- Station unknown----\n");
                    return null;
            }

            return new Station
            {
                Initiales = initiales,
                AdresseIP = "127.0.0.1",
                TypeLiaison = "AP",
                DernierTransfertFichiersJours = DateTime.Parse(dernierTransfertFichiersJours),
            };
        }

        private static Station GetStationTestVM(string initiales)
        {
            string adresseIP;

            switch (initiales)
            {
                case "EN": // Premium Heure locale
                    adresseIP = "192.168.20.1";
                    break;
                case "XY": // M580 D16 Heure locale
                    adresseIP = "192.168.34.1"; // Same as prod
                    break;
                default:
                    Console.WriteLine("\n---- Station not compatible with a test in VM preprod DEA----\n");
                    return null;
            }

            return new Station
            {
                Initiales = initiales,
                AdresseIP = adresseIP,
                TypeLiaison = "AP",
                DernierTransfertFichiersJours = DateTime.Parse("01/01/2024 00:00:00"),
            };
        }
    }
}

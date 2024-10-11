using Reseau.Entities;

namespace Reseau.Utils
{
    public static class DatabaseUtils
    {
        public static Station GetStation(string initiales)
        {
            string typeLiaison;
            switch (initiales)
            {
                case "BP": // Premium "" RU
                    typeLiaison = "AP";
                    break;
                case "EN": // Premium D13 RU
                    typeLiaison = "AP";
                    break;
                case "GA": // M580 D15 RU
                    typeLiaison = "AP";
                    break;
                case "HE": // M580 D16 RU
                    typeLiaison = "AP";
                    break;
                case "RC": // Premium "" RU with Defauts
                    typeLiaison = "AP";
                    break;
                case "172": // M580 D15 RO
                    typeLiaison = "AP";
                    break;
                case "178": // M580 D15 RO with Mesure ECH
                    typeLiaison = "AP";
                    break;
                default:
                    Console.WriteLine("---- Station unknown----");
                    return null;
            }

            return new Station
            {
                Initiales = initiales,
                AdresseIP = "127.0.0.1",
                TypeLiaison = typeLiaison
            };
        }
    }
}

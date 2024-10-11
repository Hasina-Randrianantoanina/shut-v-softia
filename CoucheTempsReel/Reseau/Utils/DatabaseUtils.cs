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
                case "FH": // M580 D16 RU
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

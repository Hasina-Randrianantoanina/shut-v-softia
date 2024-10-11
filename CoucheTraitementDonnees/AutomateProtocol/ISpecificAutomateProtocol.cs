using CoucheTraitementDonnees.Entities;

namespace CoucheTraitementDonnees.AutomateProtocol
{
    public interface ISpecificAutomateProtocol
    {
        public List<Mesure> GetMesuresFromBinaryFile(string binaryFileFullName, string versionEnregistreur);

        public Task<bool> TryWriteManFile(Station station, string binaryFileFullName, List<Mesure> mesures);

        public static ISpecificAutomateProtocol GetSpecificAutomateProtocol(Station station)
        {
            switch (station.Version)
            {
                case "MANQUE": // Premium
                    Console.WriteLine("\n------------- Switch to Premium protocol -------------");
                    return new AutomatePremiumProtocol();
                case "D13A53Aa01": // Premium
                    Console.WriteLine("\n------------- Switch to Premium protocol -------------");
                    return new AutomatePremiumProtocol();
                case "D15A55Ca01": // M580
                    Console.WriteLine("\n------------- Switch to M580 protocol -------------");
                    return new AutomateM580Protocol();
                case "D16A56Ca01": // M580
                    Console.WriteLine("\n------------- Switch to M580 protocol -------------");
                    return new AutomateM580Protocol();
                default:
                    return null;
            }
        }
    }
}

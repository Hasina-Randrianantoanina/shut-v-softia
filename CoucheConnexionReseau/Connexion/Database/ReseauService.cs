using Connexion.Entities;

namespace Connexion.Database
{
    public class ReseauService : Dal
    {
        public Station GetOneStation(string initiales)
        {
            if (_bddContext.Stations is null) {
                Console.WriteLine("\n--- DbSet<Station> is null"); 
                return null; 
            }

            Station station = _bddContext.Stations
                .Where(s => s.Initiales.Equals(initiales))
                .FirstOrDefault();

            if (station == null)
            {
                Console.WriteLine("\n--- Station not found");
            }
            else if (station.TypeLiaison == null)
            {
                Console.WriteLine("\n--- Station does not have any liaison type");
                station = null;
            }
            else if (station.AdresseIP == null)
            {
                Console.WriteLine("\n--- Station does not have any IP address");
                station = null;
            }

            return station;
        }

}
}

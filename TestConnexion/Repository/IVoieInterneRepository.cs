using TestConnexion.Entities;

namespace TestConnexion.Repository
{
    public interface IVoieInterneRepository : IRepository<VoieInterne>
    {
        Task<VoieInterne> GetOneAsync(string initialesStation, int numero);
        Task<List<VoieInterne>> GetAllOfAStationAsync (string initialesStation);
    }
}

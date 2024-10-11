using ConnexionTR.Entities;

namespace ConnexionTR.Repository
{
    public interface IVoieInterneRepository : IRepository<VoieInterne>
    {
        Task<List<VoieInterne>> GetAllOfAStationAsync(int stationId);
    }
}

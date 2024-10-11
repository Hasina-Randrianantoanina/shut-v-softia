using IsodaqOperations.Entities;

namespace IsodaqOperations.Repository
{
    public interface IVoieInterneRepository : IRepository<VoieInterne>
    {
        Task<List<VoieInterne>> GetAllOfAStationAsync(int stationId);
    }
}

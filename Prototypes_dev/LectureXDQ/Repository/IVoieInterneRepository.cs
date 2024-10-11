using LectureXDQ.Entities;

namespace LectureXDQ.Repository
{
    public interface IVoieInterneRepository : IRepository<VoieInterne>
    {
        Task<List<VoieInterne>> GetAllOfAStationAsync(int stationId);
    }
}

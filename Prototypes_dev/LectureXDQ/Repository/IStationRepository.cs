using LectureXDQ.Entities;

namespace LectureXDQ.Repository
{
    public interface IStationRepository : IRepository<Station>
    {
        Task<Station?> GetOneAsync(string initiales);
    }
}

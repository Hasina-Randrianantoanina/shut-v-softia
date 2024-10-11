using IsodaqOperations.Entities;

namespace IsodaqOperations.Repository
{
    public interface IStationRepository : IRepository<Station>
    {
        Task<Station?> GetOneAsync(string initiales);
    }
}

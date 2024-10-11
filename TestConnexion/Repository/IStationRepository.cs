using TestConnexion.Entities;

namespace TestConnexion.Repository
{
    public interface IStationRepository : IRepository<Station>
    {
        Task<Station> GetOneAsync(string initiales);
        Task<int> DeleteAsync(string initiales);
    }
}

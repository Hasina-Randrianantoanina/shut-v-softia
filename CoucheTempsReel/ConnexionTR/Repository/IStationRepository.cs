using ConnexionTR.Entities;

namespace ConnexionTR.Repository
{
    public interface IStationRepository : IRepository<Station>
    {
        Task<Station> GetOneAsync(string initiales);
    }
}

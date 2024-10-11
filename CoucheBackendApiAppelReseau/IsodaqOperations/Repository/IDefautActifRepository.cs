using IsodaqOperations.Entities;

namespace IsodaqOperations.Repository
{
    public interface IDefautActifRepository : IRepository<DefautActif>
    {
        Task<int> AddAsync(DefautActif defaut, int voieTelemesureeId);
        Task<int> DeleteAsync(int voieTelemesureeId);
    }
}

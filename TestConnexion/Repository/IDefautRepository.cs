using TestConnexion.Entities;

namespace TestConnexion.Repository
{
    public interface IDefautRepository : IRepository<Defaut>
    {
        Task<Defaut> GetActiveDefautOfThisVoieAsync(string initialesStation, int numeroVoie);
    }
}
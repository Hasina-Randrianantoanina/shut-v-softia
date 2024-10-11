namespace TestConnexion.Repository
{
    public interface IRepository<T> : IAsyncDisposable where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<int> AddAsync(T entity);
        Task<int> UpdateAsync(T entity);
    }
}

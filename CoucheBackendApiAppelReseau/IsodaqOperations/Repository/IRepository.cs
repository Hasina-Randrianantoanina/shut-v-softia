namespace IsodaqOperations.Repository
{
    public interface IRepository<T> : IAsyncDisposable where T : class
    {
    }
}

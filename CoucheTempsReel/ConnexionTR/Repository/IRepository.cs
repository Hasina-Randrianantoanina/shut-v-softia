namespace ConnexionTR.Repository
{
    public interface IRepository<T> : IAsyncDisposable where T : class
    {
    }
}

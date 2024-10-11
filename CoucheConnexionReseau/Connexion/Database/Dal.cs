namespace Connexion.Database
{
    public class Dal : IDisposable
    {
        protected BddContext _bddContext;

        public Dal()
        {
            _bddContext = new BddContext();
        }

        public void Dispose()
        {
            _bddContext.Dispose();
        }

    }
}

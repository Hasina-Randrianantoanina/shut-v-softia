namespace StenOperations.Logger
{
    public interface ILogger
    {
        void Log(string format, params object?[] args);

        void Log(Exception ex);
    }
}

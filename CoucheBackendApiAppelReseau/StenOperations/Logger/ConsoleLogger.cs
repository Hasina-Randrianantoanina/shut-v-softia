using StenOperations.Utils;

namespace StenOperations.Logger
{
    public class ConsoleLogger: ILogger
    {
        public Func<string>? Prefix { get; set; }

        public ConsoleLogger(Func<string>? prefix = null)
        {
            Prefix = prefix;
        }

        public void Log(string format, params object?[] args)
        {
            if (Prefix == null)
            {
                Console.WriteLine(format, args);
            }
            else
            {
                Console.WriteLine(Prefix.Invoke() + " " + format, args);
            }
        }

        public void Log(Exception ex)
        {
            Log("{0}: {1}", ex.GetType().FullName, ex.ExceptionStackTraces());
        }

    }
}

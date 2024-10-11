using System.Text;

namespace StenOperations.Utils
{
    public static class Utils
    {
        public static string Left(this string str, int length)
        {
            return str.Substring(0, Math.Min(length, str.Length));
        }

        public static string ExceptionMessages<TException>(this TException ex)
            where TException : Exception
        {
            var sb = new StringBuilder();
            sb.AppendLine(ex.Message);
            var innerEx = ex.InnerException;
            while (innerEx != null) 
            {
                sb.AppendLine(innerEx.Message);
                innerEx = innerEx.InnerException;
            }

            return sb.ToString();
        }

        public static string ExceptionStackTraces<TException>(this TException ex)
            where TException : Exception
        {
            var sb = new StringBuilder();

            sb.AppendLine($"MESSAGE: {ex.Message}");
            sb.AppendLine($"STACKTRACE: {ex.StackTrace}");
            var innerEx = ex.InnerException;
            while (innerEx != null)
            {
                sb.AppendLine($"MESSAGE: {innerEx.Message}");
                sb.AppendLine($"STACKTRACE: {innerEx.StackTrace}");
                innerEx = innerEx.InnerException;
            }

            return sb.ToString();
        }
    }

}

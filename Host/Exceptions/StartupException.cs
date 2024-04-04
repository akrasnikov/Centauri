using System.Globalization;

namespace Host.Exceptions
{
    public class StartupException : Exception
    {
        public StartupException() : base()
        {
        }

        public StartupException(string message) : base(message)
        {
        }

        public StartupException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }

        public StartupException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

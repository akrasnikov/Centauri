using System.Globalization;

namespace Host.Exceptions
{
    public class IntegrationException : Exception
    {
        public IntegrationException() : base()
        {
        }

        public IntegrationException(string message) : base(message)
        {
        }

        public IntegrationException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }

        public IntegrationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

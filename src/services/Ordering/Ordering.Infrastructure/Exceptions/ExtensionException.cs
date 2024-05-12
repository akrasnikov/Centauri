using System.Globalization;

namespace Ordering.Infrastructure.Exceptions
{
    public class ExtensionException : Exception
    {
        public ExtensionException() : base()
        {
        }

        public ExtensionException(string message) : base(message)
        {
        }

        public ExtensionException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }

        public ExtensionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

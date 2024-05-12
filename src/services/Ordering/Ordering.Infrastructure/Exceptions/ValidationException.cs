using FluentValidation.Results;

namespace Ordering.Infrastructure.Exceptions
{
    public class ValidationException : Exception
    {
        private ValidationException() : base("One or more validation failures have occurred.")
        {
            Errors = new List<string>();
        }

        public List<string>? Errors { get; }

        public ValidationException(IEnumerable<ValidationFailure> failures)
            : this()
        {
            foreach (var failure in failures)
            {
                Errors?.Add(failure.ErrorMessage);
            }
        }

        public ValidationException(string message) : base(message)
        {
        }

        public ValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

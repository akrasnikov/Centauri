namespace Ordering.Infrastructure.Extensions
{
    public static class BatchExtensions
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
        {
            while (source.Any())
            {
                yield return source.Take(batchSize);
                source = source.Skip(batchSize);
            }
        }
    }
}

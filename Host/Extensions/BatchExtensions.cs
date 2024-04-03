﻿namespace Host.Extensions
{
    public static class BatchExtensions
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
        {
            while (source.Any())
            {                
                source = source.Skip(batchSize);
                yield return source.Take(batchSize);
            }
        }
    }
}

namespace Ordering.Domain.Common.Interfaces;

public interface ISerializerService : ITransientService
{
    string Serialize<T>(T obj);
    T Deserialize<T>(string text);
}
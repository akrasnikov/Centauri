using Host.Common.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace Host.Common.Services;

public class MicrosoftSerializerService : ISerializerService
{
    public T Deserialize<T>(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException($"'{nameof(text)}' cannot be null or whitespace.", nameof(text));
        }
        return JsonSerializer.Deserialize<T>(text);
    }

    public string Serialize<T>(T obj)
    {
        return JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            //PropertyNameCaseInsensitive = true,
            //PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            //DefaultIgnoreCondition = JsonIgnoreCondition.Always            
        }) ;
    }
}
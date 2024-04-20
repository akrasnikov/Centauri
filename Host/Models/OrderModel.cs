using System.Text.Json.Serialization;

namespace Host.Models
{
    public class OrderModel
    {
        [JsonPropertyName("Id")]
        public string Id { get; set; }

    }
}

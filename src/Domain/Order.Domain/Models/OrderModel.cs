using System.Text.Json.Serialization;

namespace Ordering.Domain.Models
{
    public class OrderModel
    {
        [JsonPropertyName("Id")]
        public string Id { get; set; }

    }
}

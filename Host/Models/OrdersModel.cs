using Host.Entity;
using System.Text.Json.Serialization;

namespace Host.Models
{
    public class OrdersModel
    {
        [JsonPropertyName("Id")]
        public string Id { get; set; }

        [JsonPropertyName("To")]
        public string To { get; set; }

        [JsonPropertyName("From")]
        public string From { get; set; }

        [JsonPropertyName("Time")]
        public DateTime Time { get; set; }

        [JsonPropertyName("Orders")]
        public IEnumerable<Order> Orders { get; set; }


        [JsonPropertyName("Progress")]
        public int Progress { get; set; }

    }
}

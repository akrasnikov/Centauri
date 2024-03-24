using System.ComponentModel.DataAnnotations;

namespace Host.Requests
{
    public class SearchRequest
    {
        [Required]
        public required string Departure { get; set; }

        [Required]
        public required string Destination { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public double? MinPrice { get; set; }

        public double? MaxPrice { get; set; }

        public string? AirLine { get; set; }
    }
}

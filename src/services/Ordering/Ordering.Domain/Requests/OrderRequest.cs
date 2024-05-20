using System.ComponentModel.DataAnnotations;

namespace Ordering.Domain.Requests
{
#nullable disable
    public class OrderRequest
    {
        [Required] public DateTime Time { get; set; }
        [Required] public string From { get; set; }
        [Required] public string To { get; set; }
    }
}

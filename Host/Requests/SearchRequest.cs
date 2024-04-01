using System.ComponentModel.DataAnnotations;

namespace Host.Requests
{
    public class SearchRequest
    {  
        [Required] public DateTime Time { get; set; }
        [Required] public string From { get; set; }
        [Required] public string To { get; set; } 
    }
}

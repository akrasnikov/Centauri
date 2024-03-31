using Host.Entities;

namespace Host.Models
{
    public class AggregatedDataModel
    {
        public List<Order> Items { get; set; }
        public int ProgressCounter { get; set; }

        public AggregatedDataModel()
        {
            Items = [];
            ProgressCounter = 0;
        }
    }
}

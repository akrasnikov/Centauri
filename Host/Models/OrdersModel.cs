using Host.Entities;

namespace Host.Models
{
    public class OrdersModel
    {
        public List<Order> Items { get; set; }
        public int ProgressCounter { get; set; }

        public OrdersModel()
        {
            Items = [];
            ProgressCounter = 0;
        }
    }
}

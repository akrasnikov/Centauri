﻿namespace Host.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTime Time { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}

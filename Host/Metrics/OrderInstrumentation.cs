﻿using System.Diagnostics.Metrics;

namespace Host.Metrics
{
    public class OrderInstrumentation
    {       
        private Counter<int> OrdersCounter { get; }
        private Counter<int> TotalOrdersCounter { get; }
       
        private Histogram<double> OrdersPriceHistogram { get; }
      

        public OrderInstrumentation(IMeterFactory meterFactory, IConfiguration configuration)
        {
            var meter = meterFactory.Create("orders.meter");

            OrdersCounter = meter.CreateCounter<int>("orders-counter", "order");
            TotalOrdersCounter = meter.CreateCounter<int>("total-orders", "orders");
            OrdersPriceHistogram = meter.CreateHistogram<double>("orders-price", "euros", "delta price orders");
            
        }
        public void AddOrder() => OrdersCounter.Add(1);
        public void AddTotalOrder() => TotalOrdersCounter.Add(1);
      
        public void RecordOrderTotalPrice(double price) => OrdersPriceHistogram.Record(price);
        public void IncreaseTotalOrders(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                throw new ArgumentException($"'{nameof(city)}' cannot be null or whitespace.", nameof(city));
            }

            TotalOrdersCounter.Add(1, KeyValuePair.Create<string, object>(key: "city", city));
        }
    }
}

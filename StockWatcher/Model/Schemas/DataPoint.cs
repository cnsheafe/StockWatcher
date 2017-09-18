using System;

namespace StockWatcher.Model.Schemas
{
    public class DataPoint
    {
        public string TimeStamp { get; set; }

        private double _price;
        public double Price
        {
            get => Math.Round(_price, 2, MidpointRounding.ToEven);
            set => _price = value;
        }
    }
}
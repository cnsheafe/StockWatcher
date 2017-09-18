// Contains Schema NotificationsController Input-Model
using System;
using System.ComponentModel.DataAnnotations;

namespace StockWatcher.Model.Schemas
{
    public class Stock
    {

        [Required]
        public string Symbol { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        private double price;

        [Required]
        public double Price
        {
            get => price;
            set => price = Math.Round(value, 2, MidpointRounding.ToEven);
        }
        public string RequestId
        {
            get => Symbol + price.ToString();
            set { }
        }
    }
}
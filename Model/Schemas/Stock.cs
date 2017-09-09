// Contains Model Schema for Stock
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockWatcher.Model.Schemas
{

    [Table("StockRequests")]
    public class Stock
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        public string Username { get; set; }

        [Required]
        [StringLength(4)]
        public string Equity { get; set; }

        private double price;

        [Required]
        public double Price
        {
            get => price;
            set => price = Math.Round(value, 2, MidpointRounding.ToEven);
        }
        public string RequestId
        {
            get => Equity + price.ToString();
            set { }
        }

    }
}
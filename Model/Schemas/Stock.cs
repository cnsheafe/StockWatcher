// Contains Model Schema for Stock
using System;
using System.ComponentModel.DataAnnotations;

namespace StockWatcher.Model.Schemas {
   public class Stock {

    [Required]
    [StringLength(30)]
    public string Username {get; set;}

    [Required]
    [StringLength(4)]
    public string Equity {get; set;}
    
    private double price;

    [Required]
    public double Price {
        get => Math.Round(price, 2, MidpointRounding.ToEven);
        set => price = value;
    } 
    public string RequestId {
        get => Equity + price.ToString();
    }

   } 
}
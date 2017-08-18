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
        get => price;
        set => price = Math.Round(value,2,MidpointRounding.ToEven);
    } 
    public string RequestId {
        get => Equity + price.ToString();
    }

   } 
}
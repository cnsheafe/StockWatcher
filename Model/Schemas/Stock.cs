// Contains Model Schema for Stock
using System;

namespace StockWatcher.Model.Schemas {
   public class Stock {
       public string Username {get; set;}
       public string Equity {get; set;}

       private double price;
       public double Price {
           get => Math.Round(price, 2, MidpointRounding.ToEven);
           set => price = value;
       }
       public string RequestId {
           get => Equity + Price.ToString();
       }

   } 
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockWatcher.Model.Schemas
{
    [Table("companies")]
    public class Company
    {
        [Key]
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string LastSale { get; set; }
        public string MarketCap { get; set; }
        public string Adrtso { get; set; }

        public string IPOyear { get; set; }

        public string Sector { get; set; }

        public string Industry { get; set; }
        public string SummaryQuote { get; set; }
    }
}
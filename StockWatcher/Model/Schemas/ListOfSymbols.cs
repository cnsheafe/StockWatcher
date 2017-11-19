using System.ComponentModel.DataAnnotations;
namespace StockWatcher.Model.Schemas
{
    public class ListOfSymbols
    {
        [Required]
        public string[] Symbols { get; set;}
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockWatcher.Model.Schemas
{

    [Table("limit_count")]
    public class LimitCount
    {

        [Key]
        [Required]
        public string PhoneHash { get; set; }

        [Required]
        public int Count { get; set; }

        [Required]
        public string Date { get; set; }
    }
}
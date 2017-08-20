using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockWatcher.Model.Schemas 
{
    [Table("Users")]
    public class User 
    {
        [Key]
        public int Id {get;set;}

        [Required]
        [StringLength(30)]
        public string Username {get; set;}

        [Required]
        [StringLength(30)]
        public string Password {get; set;}

        [Required]
        [StringLength(12)]
        public string Phone {get; set;}
        public string Email {get; set;}
        public string Uuid {get; set;}
    }
}
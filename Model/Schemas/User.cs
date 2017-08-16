using System.ComponentModel.DataAnnotations;

namespace StockWatcher.Model.Schemas {
    public class User {
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
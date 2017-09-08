using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockWatcher.Model.Schemas
{
    [Table("Users")]
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        public string Username { get; set; }

        [Required]
        [StringLength(30)]
        public string Password { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public string Uuid { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace StockWatcher.Model.Schemas
{
    public class Login
    {

        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
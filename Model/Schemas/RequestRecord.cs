using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockWatcher.Model.Schemas
{
    [Table("Requests")]
    public class RequestRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string RequestId { get; set; }

        [Required]
        public string TwilioBinding { get; set; }

        [Required]
        public double Price { get; set; }

    }

}
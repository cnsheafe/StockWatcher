using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockWatcher.Model.Schemas
{
    public class Query
    {
        [Required]
        public string SearchPhrase { get; set; }

    }
}
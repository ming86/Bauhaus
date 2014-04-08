using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Bauhaus.Models
{
    public class Quantity
    {
        [Key]
        public int ID { get; set; }
        [Display(Name = "Qty CS")]
        public double QtyCS { get; set; }
        [Display(Name = "Qty SU")]
        public double QtySU { get; set; }
        [Display(Name = "Net WT")]
        public double NetWeight { get; set; }
        public double Volume { get; set; }
    }
}
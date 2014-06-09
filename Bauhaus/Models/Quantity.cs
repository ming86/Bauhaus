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
        [Display(Name = "CS")]
        public double CS { get; set; }
        [Display(Name = "SU")]
        public double SU { get; set; }
        [Display(Name = "Net WT")]
        public double NetWeight { get; set; }
        public double Volume { get; set; }
    }
}
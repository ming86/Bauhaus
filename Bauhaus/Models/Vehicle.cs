using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bauhaus.Models
{
    public class Vehicle
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public String Plate { get; set; }
        [Required]
        public String Type { get; set; }
        public virtual Contact Driver { get; set; }
    }
}
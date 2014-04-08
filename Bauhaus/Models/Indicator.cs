using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bauhaus.Models
{
    public class Indicator
    {
        [Key]
        public int ID { get; set; }
        public Boolean Value { get; set; }
        public String Reason { get; set; }
        public Boolean Confirmed { get; set; }
    }
}
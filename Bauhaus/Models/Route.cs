using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Bauhaus.Models
{
    public class Route
    {
        [Key]
        public int ID { get; set; }
        [Display(Name = "Route")]
        public String Name { get; set; }
        public Int64 LeadTimeTicks { get; set; }
        [NotMapped]
        public TimeSpan LeadTime
        {
            get { return TimeSpan.FromTicks(LeadTimeTicks); }
            set { LeadTimeTicks = value.Ticks; }
        }
        public String Plant { get; set; }
    }
}
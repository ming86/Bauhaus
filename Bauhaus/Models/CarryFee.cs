using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Bauhaus.Models
{
    public class CarryFee
    {
        [Key]
        public int ID { get; set; }
        public String Route { get; set; }
        [Display(Name="Vehicle Type")]
        public String VehicleType { get; set; }
        public String Cost { get; set; }
    }
}

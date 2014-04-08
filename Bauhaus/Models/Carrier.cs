using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Bauhaus.Models
{
    public class Carrier
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }
        [Display(Name="Carrier")]
        public string Name { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
        public virtual ICollection<Driver> Drivers { get; set; }
        public virtual ICollection<CarryFee> CarryFees { get; set; }
    }
}
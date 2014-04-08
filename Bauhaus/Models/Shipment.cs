using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Bauhaus.Models
{
    public class Shipment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name="Shipment")]
        public long ID { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Date { get; set; }
        public String Turn { get; set; }
        [Display(Name="Carry Fee")]
        public String CarryFee { get; set; }
        [Display(Name = "Transit Data")]
        public virtual ICollection<Input> TransitData { get; set; }
        [Display(Name="Carrier Information")]
        public virtual Carrier Carrier { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public int OrdersToGo { get; set; }
    }
}

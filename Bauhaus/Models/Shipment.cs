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

        /// <summary>
        /// Given a Volume quantity it returns a Vehicle description
        /// with the right capacity to carry it.
        /// </summary>
        /// <param name="volume">Order Volume</param>
        /// <returns>Vehicle Type</returns>
        internal void CalculateVehicle()
        {
            if(this.Vehicle==null)
            {
                this.Vehicle = new Models.Vehicle();
                this.Vehicle.Plate = "N/A";
            }
                
            double volume = this.Orders.Sum(x => x.Products.Sum(y => y.Qty.Volume));
            if (volume <= 10000)
                this.Vehicle.Type = "VE01";
            else
                if (volume <= 19000)
                    this.Vehicle.Type = "VE02";
                else
                    if (volume <= 29000)
                        this.Vehicle.Type = "VE03";
                    else
                        if (volume <= 43000)
                            this.Vehicle.Type = "VE04";
                        else
                            this.Vehicle.Type = "VE05";
        }
    }
}

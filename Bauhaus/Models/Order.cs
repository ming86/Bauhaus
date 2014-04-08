using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Bauhaus.Models
{
    public class Order
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SapID { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name="Doc. Date")]
        public DateTime DocDate { get; set; }
        public string Type { get; set; }
        public string PayTerm { get; set; }
        [Required]
        public virtual Customer Customer { get; set; }
        [Display(Name="Cust. PO")]
        public string CustomerPO { get; set; }
        public virtual RDDF RDDF { get; set; }
        public virtual Quantity Quantities { get; set; }
        [Display(Name="VEH Type")]
        public string VehicleType { get; set; }
        [Display(Name="Canceled CS")]
        public double CancelRejectCS { get; set; }
        public virtual Delivery Delivery { get; set; }
        public virtual Status Status { get; set; }
        public virtual Shipment Shipment { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set;}
        public virtual POD POD { get; set; }


        /// <summary>
        /// Calculates Indicators on orders with loaded POD
        /// </summary>
        /// <returns>0 if Indicators Updated Correctly, 1 Otherwise.</returns>
        public int calculateIndicators()
        {
            // Order Needs POD for indicators to be calculated
            if(POD == null)
                return 1;
            else
            {
                if(POD.CSOT==null)
                {
                    POD.CSOT = new Indicator();
                    if (POD.Date <= RDDF.Original)   // Inside Time Window.
                    {
                        POD.CSOT.Value = true; // HIT
                        POD.CSOT.Confirmed = true;
                    }
                    else
                    {
                        POD.CSOT.Value = false; // MISS
                    }
                }

                if(POD.OT == null)
                {
                    POD.OT = new Indicator();
                    if (POD.Date <= RDDF.DSSDate || // Inside Time Window
                        Quantities.QtyCS < 200)
                    {
                        POD.OT.Value = true;
                        POD.OT.Confirmed = true;
                    }
                    else
                    {
                        POD.OT.Value = false;
                    }

                }

                return 0;
            }
        }
    }
}
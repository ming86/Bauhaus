using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Bauhaus.Models
{
    public class Customer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name="Customer #")]
        public long ID { get; set; }
        [Display(Name="Cust. Name")]
        public String Name { get; set; }
        [Display(Name="Sale Zone")]
        public int SaleZone { get; set; }
        public int Unit { get; set; }
        public virtual Route Route { get; set; }
        [Display(Name="Max. Vehycle")]
        public String MaxVEH { get; set; }
        public String Transport { get; set; }
        [Display(Name="CSR OM")]
        public virtual Contact MainCSROM { get; set; }
        [Display(Name="CSR OM Backup")]
        public virtual Contact BackupCSROM { get; set; }
        public String Team { get; set; }
        [Display(Name="CBD Rep.")]
        public virtual Contact CBDRep { get; set; }
        public virtual Contact GU { get; set; }
        public String Region { get; set; }
        public String City { get; set; }
        public String Address { get; set; }
        [Display(Name="Pay Type")]
        public String PayType { get; set; }
        [Display(Name="Pay Term")]
        public String PayTerm { get; set; }
        [Display(Name="Pay Desc.")]
        public String PayDescription { get; set; }
        public String Discount { get; set; }
        public long Payer { get; set; }
        [Display(Name="CSR AR")]
        public virtual Contact MainCSRAR { get; set; }
        [Display(Name="CSR AR Backup")]
        public virtual Contact BackupCSRAR { get; set; }
        public String Observation { get; set; }

        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }    
}
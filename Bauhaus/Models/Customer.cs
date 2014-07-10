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
        [Display(Name = "Customer #")]
        public long ID { get; set; }
        [Display(Name = "Cust. Name")]
        public String Name { get; set; }
        [Display(Name = "Sale Zone")]
        public int SaleZone { get; set; }
        public int Unit { get; set; }
        public virtual Route Route { get; set; }
        [Display(Name = "Max. Vehycle")]
        public String MaxVEH { get; set; }
        public String Transport { get; set; }
        [Display(Name = "CSR OM")]
        public virtual Contact MainCSROM { get; set; }
        [Display(Name = "CSR OM Backup")]
        public virtual Contact BackupCSROM { get; set; }
        public String Team { get; set; }
        [Display(Name = "CBD Rep.")]
        public virtual Contact CBDRep { get; set; }
        public virtual Contact GU { get; set; }
        public String Region { get; set; }
        public String City { get; set; }
        public String Address { get; set; }
        [Display(Name = "Pay Type")]
        public String PayType { get; set; }
        [Display(Name = "Pay Term")]
        public String PayTerm { get; set; }
        [Display(Name = "Pay Desc.")]
        public String PayDescription { get; set; }
        public String Discount { get; set; }
        public long Payer { get; set; }
        [Display(Name = "CSR AR")]
        public virtual Contact MainCSRAR { get; set; }
        [Display(Name = "CSR AR Backup")]
        public virtual Contact BackupCSRAR { get; set; }
        public String Observation { get; set; }

        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }

        public int updateContact(String area, String name, String tel, String mail, BauhausEntities db)
        {
            if (!String.IsNullOrWhiteSpace(area) && !String.IsNullOrWhiteSpace(name))
            {
                // Use real area to look in DB
                string auxArea = "";
                switch (area)
                {
                    case "MAIN CSR OM":
                        auxArea = "CSR OM";
                        break;
                    case "BACKUP CSR OM":
                        auxArea = "CSR OM";
                        break;
                    case "MAIN CSR AR":
                        auxArea = "CSR AR";
                        break;
                    case "BACKUP CSR AR":
                        auxArea = "CSR AR";
                        break;
                    default:
                        auxArea = area;
                        break;
                }
                // Look for contact in DB
                Contact newCont = db.Contacts.Where(x => x.Area == auxArea && x.Name == name).FirstOrDefault();
                // Contact in DB
                if (newCont != null)
                {
                    if (!String.IsNullOrWhiteSpace(tel) && newCont.Telephone != tel)
                        newCont.Telephone = tel;
                    if (!String.IsNullOrWhiteSpace(mail) && newCont.Email != mail)
                        newCont.Email = mail;
                }
                // Contact not in DB
                else
                {
                    newCont = new Contact();
                    newCont.Area = auxArea;
                    newCont.Name = name;
                    newCont.Telephone = tel;
                    newCont.Email = mail;
                }
                // Assign contact where it belongs
                switch (area)
                {
                    case "MAIN CSR OM":
                        if (MainCSROM != newCont)
                            MainCSROM = newCont;
                        break;
                    case "BACKUP CSR OM":
                        if (BackupCSROM != newCont)
                            BackupCSROM = newCont;
                        break;
                    case "MAIN CSR AR":
                        if (MainCSRAR != newCont)
                            MainCSRAR = newCont;
                        break;
                    case "BACKUP CSR AR":
                        if (BackupCSRAR != newCont)
                            BackupCSRAR = newCont;
                        break;
                    case "CBD":
                        if (CBDRep != newCont)
                            CBDRep = newCont;
                        break;
                    case "GU":
                        if (GU != newCont)
                            GU = newCont;
                        break;
                    default:
                        if (!Contacts.Contains(newCont))
                            Contacts.Add(newCont);
                        break;
                }
                return 0;
            }
            return 1;
        }
    }
}
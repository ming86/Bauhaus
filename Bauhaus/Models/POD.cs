using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bauhaus.Models
{
    public class POD
    {
        [Key]
        public int ID { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "POD Date")]
        public DateTime Date { get; set; }
        public virtual Indicator CSOT { get; set; }
        public virtual Indicator OT { get; set; }
    }
}
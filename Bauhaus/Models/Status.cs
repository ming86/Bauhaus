using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bauhaus.Models
{
    public class Status
    {
        [Key]
        public int ID { get; set; }
        [Display(Name="Status")]
        public int Code { get; set; }
        [Display(Name = "Stage")]
        public int Stage { get; set; }
        public int State { get; set; }
        public int Reason { get; set; }
        public bool OpenItem { get; set; }
        public int Report { get; set; }
        public String Comment { get; set; }
    }
}
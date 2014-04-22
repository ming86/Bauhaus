using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Resources;
using System.Linq;
using System.Web;
using Amib.Threading;

namespace Bauhaus.Models
{
    public class Report
    {
        [Key]
        public int ReportID { get; set; }
        public string Name { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name="Creation Date")]
        public DateTime CreationDate { get; set; }
        public int Status { get; set; }
        public string Uploader { get; set; }
        public string Path { get; set; }
        public string Remark { get; set; }
        public double ProcessTime { get; set; }
    }
}
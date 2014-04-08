using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bauhaus.Models
{
    public class Input
    {
        [Key]
        public int ID { get; set; }
        public int Stage { get; set; }
        public int State { get; set; }
        public int Reason { get; set; }
        public string Comment { get; set; }
        public String Author { get; set; }
        public DateTime Time { get; set; }
    }
}

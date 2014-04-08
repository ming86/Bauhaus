using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
namespace Bauhaus.Models
{
    public class HistIndicator
    {
        [Key]
        public int ID { get; set; }
        public String Name { get; set; }
        public DateTime Date { get; set; }
        public double Value { get; set; }
    }
}
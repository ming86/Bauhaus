using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bauhaus.Models
{
    public class Driver
    {
        [Key]
        public int ID { get; set; }
        public String Name { get; set; }
        public String Telephone { get; set; }
    }
}
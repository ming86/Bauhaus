using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bauhaus.Models
{
    public class Product
    {
        [Key]
        public int ID { get; set; }
        public long SKU { get; set; }
        public String Description { get; set; }
        public String Category { get; set; }
        public String Brand { get; set; }
        public virtual Quantity Qty { get; set; }
        public virtual Quantity DSSQty { get; set; }
    }
}
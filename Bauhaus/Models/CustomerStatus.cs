using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bauhaus.Models
{
    public class CustomerStatus
    {
        public String CustomerName { get; set; }
        public int Stage { get; set; }
        public int Status { get; set; }
        public int Reason { get; set; }
    }
}
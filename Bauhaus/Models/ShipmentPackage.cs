using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bauhaus.Models
{
    public class ShipmentPackage
    {
        public Shipment Shipment { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
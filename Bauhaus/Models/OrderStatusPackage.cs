using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bauhaus.Models
{
    public class OrderCount
    {
        public String Class;
        public int Count;
    }

    public class OrderStatusPackage
    {
        public int TotalOrders;
        public List<OrderCount> Stage;
        public List<OrderCount> PlanSt;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bauhaus.Models
{
    // Compressed Status Package
    public class CSP
    {
        public int Status10Orders { get; set; }
        public int Status20Orders { get; set; }
        public int Status30Orders { get; set; }
        public int Status40Orders { get; set; }
        public int Status50Orders { get; set; }
        public int Status99Orders { get; set; }
        public DateTime LastUpdated { get; set; }
        public String LastUpdater { get; set; }
    }
}
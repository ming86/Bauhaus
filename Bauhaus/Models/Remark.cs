using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Bauhaus.Models
{
    public class Remark
    {
        [Key]
        public int ID { get; set; }
        public int associatedStatus { get; set; }
        public string input { get; set; }
        [Timestamp]
        public Byte[] RegisterTime { get; set; }

    }
}

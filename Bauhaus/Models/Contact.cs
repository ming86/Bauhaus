using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Bauhaus.Models
{
    public class Contact
    {
        [Key]
        public int ID { get; set; }
        public String Area { get; set; }
        public String Name { get; set; }
        public String Telephone {get; set;}
        [DataType(DataType.EmailAddress)]
        public String Email { get; set; }
    }


}

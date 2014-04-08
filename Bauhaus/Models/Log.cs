﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bauhaus.Models
{
    public class Log
    {
        [Key]
        public int ID { get; set; }
        public String Source { get; set; }
        public String UserName { get; set; }
        public String Type { get; set; }
        public String Description { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Date { get; set; }
    }
}
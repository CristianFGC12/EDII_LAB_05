﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDII_LAB_05.Models
{
    public class Ingreso
    {
        public string name { get; set; }
        public string dpi { get; set; }
        public string dateBirth { get; set; }
        public string address { get; set; }
        public List<Compania> companies { get; set; }
        public string recluiter { get; set; }
    }
}

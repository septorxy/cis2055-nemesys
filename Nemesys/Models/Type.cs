﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nemesys.Models
{
    public class Type
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Report> Reports { get; set; }
    }
}

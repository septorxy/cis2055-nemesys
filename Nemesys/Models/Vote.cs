using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nemesys.Models
{ 
    public class Vote
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public int ReportId { get; set; }
        public Report Report { get; set; }
        public int vote { get; set; }
    }
}

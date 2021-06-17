using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nemesys.Models
{
    public class Vote
    {
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string ReportId { get; set; }
        public Report Report { get; set; }
        public bool vote { get; set; }
    }
}

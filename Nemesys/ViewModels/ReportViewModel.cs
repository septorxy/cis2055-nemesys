using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nemesys.ViewModels
{
    public class ReportViewModel
    {
        public int Id { get; set; }
        public DateTime ReportDate { get; set; }
        public DateTime HazardDate { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string PhotoUrl { get; set; }
        public int Upvotes { get; set; }
        public UserViewModel User { get; set; }
        public ViewInvestigationViewModel Investigation { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nemesys.ViewModels
{
    public class ReportViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Report Date")]
        public DateTime ReportDate { get; set; }
        [Display(Name = "Hazard Date")]
        public DateTime HazardDate { get; set; }
        [Display(Name = "Location")]
        public string Location { get; set; }
        public string Description { get; set; }
        [Display(Name = "Image")]
        public string PhotoUrl { get; set;} 
        public int Upvotes { get; set; }
        public int vote { get; set; }
        public UserViewModel User { get; set; }
        public ViewInvestigationViewModel Investigation { get; set; }
        public ListViewModel Status { get; set; }
        public ListViewModel Type { get; set; }
    }
}

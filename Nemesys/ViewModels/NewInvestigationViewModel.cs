using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nemesys.ViewModels
{
    public class NewInvestigationViewModel
    {
        public int Id { get; set; }
        public ReportViewModel Report { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [StringLength(1500, ErrorMessage = "Report must not be longer than 1500 characters")]
        public string Description { get; set; }

        [Display(Name = "Date of Action")]
        [Required(ErrorMessage = "This field is required")]
        public DateTime DateOfAction { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public int StatusId { get; set; }

        public List<ListViewModel> StatusList { get; set; }
        
    }
}

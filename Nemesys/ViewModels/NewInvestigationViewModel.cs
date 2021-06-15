using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nemesys.ViewModels
{
    public class NewInvestigationViewModel
    {
        public int Id { get; set; }
        public ReportViewModel Report { get; set; }
        public string Description { get; set; }
        public DateTime DateOfAction { get; set; }
    }
}

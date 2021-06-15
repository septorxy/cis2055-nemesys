using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nemesys.ViewModels
{
    public class ViewInvestigationViewModel
    {
            public int Id { get; set; }
            public string Description { get; set; }
            public DateTime DateOfAction { get; set; }
            public UserViewModel Investigator { get; set; }
    }

}


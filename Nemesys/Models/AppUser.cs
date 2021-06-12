using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nemesys.Models
{
    public class AppUser : IdentityUser
    {
        [PersonalData]
        public int TotalReports { get; set;}
    }
}

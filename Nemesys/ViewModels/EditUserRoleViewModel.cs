using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nemesys.ViewModels
{
    [Keyless]
    public class EditUserRoleViewModel
    {
        public string UserName { get; set; }
        public string Role { get; set; }
    }
}

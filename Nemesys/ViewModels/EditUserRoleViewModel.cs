using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nemesys.ViewModels
{
    public class EditUserRoleViewModel
    {
        [Key]
        public string UserName { get; set; }
        public string Role { get; set; }
    }
}

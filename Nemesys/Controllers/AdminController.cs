using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Nemesys.Models;
using Nemesys.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nemesys.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly INemesysRepository _nemesysRepository;

        public AdminController(INemesysRepository nemesysRepository, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            this._roleManager = roleManager;
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._nemesysRepository = nemesysRepository;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProjectRole role)
        {
            var roleExist = await _roleManager.RoleExistsAsync(role.RoleName);
            if (!roleExist)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(role.RoleName));
            }
            return View();
        }

        public IActionResult Assign()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Assign(string role, string username)
        {
            IdentityUser User = _nemesysRepository.GetUserByUsername(username);
            if(User != null) {
                var currRole = await _userManager.GetRolesAsync(User);
                var res = await _userManager.RemoveFromRoleAsync(User, currRole[0]);
                var result = await _userManager.AddToRoleAsync(User, role);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "User Not Found");
            }
            
            return View();
        }
    }
}

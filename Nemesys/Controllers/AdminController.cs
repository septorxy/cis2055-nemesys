using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly INemesysRepository _nemesysRepository;
        private readonly ILogger<AdminController> _logger;

        public AdminController(INemesysRepository nemesysRepository, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<AdminController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _nemesysRepository = nemesysRepository;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(ProjectRole role)
        {
            try
            {
                var roleExist = await _roleManager.RoleExistsAsync(role.RoleName);
                if (!roleExist)
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(role.RoleName));
                }
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Assign()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Assign(string role, string username)
        {
            try
            {
                AppUser User = _nemesysRepository.GetUserByUsername(username);
                if (User != null)
                {
                    var currRole = await _userManager.GetRolesAsync(User);
                    await _userManager.RemoveFromRoleAsync(User, currRole[0]);
                    await _userManager.AddToRoleAsync(User, role);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User Not Found");
                    return View();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult CreateStatus()
        {
            return View();
        }


        [HttpPost]
        public IActionResult CreateStatus(string name)
        {
            try
            {
                Boolean exists = false;
                IEnumerable<Status> statuses = _nemesysRepository.GetAllStatuses();
                foreach (Status status in statuses)
                {
                    if (status.Name.Equals(name))
                    {
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    var newStatus = new Status()
                    {
                        Name = name
                    };
                    _nemesysRepository.CreateStatus(newStatus);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Status Already Exists");
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult CreateType()
        {
            return View();
        }


        [HttpPost]
        public IActionResult CreateType(string name)
        {
            try
            {
                Boolean exists = false;
                IEnumerable<Models.Type> types = _nemesysRepository.GetAllTypes();
                foreach (Models.Type type in types)
                {
                    if (type.Name.Equals(name))
                    {
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    var newType = new Models.Type()
                    {
                        Name = name
                    };
                    _nemesysRepository.CreateType(newType);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Type Already Exists");
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                return View("Error");
            }
        }

    }


}

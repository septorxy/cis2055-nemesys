using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Nemesys.Models;
using Nemesys.Models.Interfaces;
using Nemesys.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nemesys.Controllers
{
    [Authorize(Roles = "Admin,Investigator")]
    public class InvestigatorController : Controller
    {
        private readonly INemesysRepository _nemesysRepository;
        private readonly UserManager<AppUser> _userManager;

        public InvestigatorController(INemesysRepository nemesysRepository, UserManager<AppUser> userManager)
        {
            _nemesysRepository = nemesysRepository;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Investigate(int Id)
        {
            Report report = _nemesysRepository.GetReportById(Id);
            var model = new InvestigateViewModel()
            {
                Report = new ReportViewModel()
                {
                    Id = report.Id,
                    ReportDate = report.ReportDate,
                    HazardDate = report.HazardDate,
                    Location = report.Location,
                    Type = report.Type,
                    Description = report.Description,
                    Status = report.Status,
                    PhotoUrl = report.PhotoUrl,
                    Upvotes = report.Upvotes,
                    User = new UserViewModel()
                    {
                        Id = report.UserId,
                        UserName = report.User.UserName
                    }
                }
            };

            return View(model); ;
        }

        [HttpPost]
        public IActionResult Investigate([Bind("Description, DateOfAction")] InvestigateViewModel newInvestigation, int Id)
        {
            if (ModelState.IsValid)
            {
                Report report = _nemesysRepository.GetReportById(Id);
                Investigation investigate = new Investigation()
                {
                    Id = Id,
                    DateOfAction = newInvestigation.DateOfAction,
                    Description = newInvestigation.Description,
                    ReportId = report.Id,
                    UserId = _userManager.GetUserId(User)
                };
                _nemesysRepository.CreateInvestigation(investigate);
                return RedirectToAction("Index");
            }
            else
            {
                return View(newInvestigation);
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
            AppUser User = _nemesysRepository.GetUserByUsername(username);
            if (User != null)
            {
                var currRole = await _userManager.GetRolesAsync(User);
                if (currRole.Equals("User") || currRole.Equals("Investigator"))
                {
                    await _userManager.RemoveFromRoleAsync(User, currRole[0]);
                    await _userManager.AddToRoleAsync(User, role);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "You can't edit the role of Admins from this page");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "User Not Found");
            }

            return View();
        }
    }
}

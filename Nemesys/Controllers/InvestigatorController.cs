using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
        private readonly IEmailSender _emailSender;

        public InvestigatorController(INemesysRepository nemesysRepository, UserManager<AppUser> userManager, IEmailSender emailSender)
        {
            _nemesysRepository = nemesysRepository;
            _userManager = userManager;
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Investigate(int Id)
        {
            Report report = _nemesysRepository.GetReportById(Id);
            var model = new NewInvestigationViewModel()
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
        public IActionResult Investigate([Bind("Description, DateOfAction")] NewInvestigationViewModel newInvestigation, int Id)
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
                _emailSender.SendEmailAsync(report.User.Email, "Your Report is being Investigated!", $"Dear {report.User.UserName},\n Please refer to <a href='https://universitynemesys.azurewebsites.net/Reports/Details/"+report.Id+"'>this link</a> to view the current status of your report.\nSincerely,\nThe Investigation Team");
                return RedirectToAction("Details", "Reports", new { id = report.Id});
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

        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                Report report = _nemesysRepository.GetReportById(id);
                Investigation investigation = _nemesysRepository.GetInvestigationByReport(id);
                if (investigation != null)
                {
                    var model = new NewInvestigationViewModel()
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
                        },
                        Id = investigation.Id,
                        Description = investigation.Description,
                        DateOfAction = investigation.DateOfAction

                    };

                    return View(model);

                }
                else
                    return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind("Description, DateOfAction")] NewInvestigationViewModel editedInvestigation, [FromRoute] int id)
        {
            try
            {
                var modelToUpdate = _nemesysRepository.GetInvestigationByReport(id);
                if (modelToUpdate == null)
                {
                    return NotFound();
                }
                if (ModelState.IsValid)
                {
                    modelToUpdate.Description = editedInvestigation.Description;
                    modelToUpdate.DateOfAction= editedInvestigation.DateOfAction;
                    modelToUpdate.UserId = _userManager.GetUserId(User);

                    _nemesysRepository.UpdateInvestigation(modelToUpdate);

                    return RedirectToAction("Details", "Reports", new { id = modelToUpdate.Id });
                }
                else
                {
                    return View(editedInvestigation);
                }
            }
            catch (Exception ex)
            { 
                return View("Error");
            }
        }
    }
}

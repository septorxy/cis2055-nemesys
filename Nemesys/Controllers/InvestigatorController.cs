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

        [HttpGet]
        public IActionResult Investigate(int Id)
        {
            Report report = _nemesysRepository.GetReportById(Id);
            var statusList = _nemesysRepository.GetAllStatuses().Select(s => new ListViewModel()
            {
                Id = s.Id,
                Name = s.Name
            }).ToList();
            Status currStatus = _nemesysRepository.GetStatusById(report.StatusId);
            Models.Type currType = _nemesysRepository.GetTypeById(report.TypeId);
            var model = new NewInvestigationViewModel()
            {
                StatusList = statusList,
                StatusId = report.StatusId,
                Report = new ReportViewModel()
                {
                    Id = report.Id,
                    ReportDate = report.ReportDate,
                    HazardDate = report.HazardDate,
                    Location = report.Location,
                    Longitude = report.Longitude,
                    Latitiude = report.Latitude,
                    Type = new ListViewModel()
                    {
                        Name = currStatus.Name
                    },
                    Description = report.Description,
                    Status = new ListViewModel() 
                    { 
                        Name = currType.Name
                    },
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
        public IActionResult Investigate([Bind("Description, DateOfAction, StatusId")] NewInvestigationViewModel newInvestigation, int Id)
        {
            Report report = _nemesysRepository.GetReportById(Id);
            if (ModelState.IsValid){ 
                Investigation investigate = new Investigation()
                {
                    Id = Id,
                    DateOfAction = newInvestigation.DateOfAction,
                    Description = newInvestigation.Description,
                    ReportId = report.Id,
                    UserId = _userManager.GetUserId(User)
                };
                _nemesysRepository.CreateInvestigation(investigate);
                _emailSender.SendEmailAsync(report.User.Email, "Your Report is being Investigated!", $"Dear {report.User.UserName},<br>Please refer to <a href='https://universitynemesys.azurewebsites.net/Reports/Details/" +report.Id+ "'>this link</a> to view the initial investigation response!. <br>Sincerely,<br>The Investigation Team");
                if(report.StatusId != newInvestigation.StatusId)
                {
                    report.StatusId = newInvestigation.StatusId;
                    _nemesysRepository.UpdateReport(report);
                    _emailSender.SendEmailAsync(report.User.Email, "Your Report's Status has changed!", $"Dear {report.User.UserName},<br>Please refer to <a href='https://universitynemesys.azurewebsites.net/Reports/Details/" + report.Id + "'>this link</a> to view the current status of your report.<br>Sincerely,<br>The Investigation Team");
                }
                return RedirectToAction("Details", "Reports", new { id = report.Id});
            }
            else
            {
                Status currStatus = _nemesysRepository.GetStatusById(report.StatusId);
                Models.Type currType = _nemesysRepository.GetTypeById(report.TypeId);
                var statusList = _nemesysRepository.GetAllStatuses().Select(s => new ListViewModel()
                {
                    Id = s.Id,
                    Name = s.Name
                }).ToList();
                newInvestigation.Report = new ReportViewModel()
                {
                    Id = report.Id,
                    ReportDate = report.ReportDate,
                    HazardDate = report.HazardDate,
                    Location = report.Location,
                    Type = new ListViewModel()
                    {
                        Name = currType.Name
                    },
                    Description = report.Description,
                    Status = new ListViewModel()
                    {
                        Name = currStatus.Name
                    },
                    PhotoUrl = report.PhotoUrl,
                    Upvotes = report.Upvotes,
                    Longitude = report.Longitude,
                    Latitiude = report.Latitude,
                    User = new UserViewModel()
                    {
                        Id = report.UserId,
                        UserName = report.User.UserName
                    },
                };
                newInvestigation.StatusList = statusList;
                newInvestigation.StatusId = report.StatusId;
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
                var statusList = _nemesysRepository.GetAllStatuses().Select(s => new ListViewModel()
                {
                    Id = s.Id,
                    Name = s.Name
                }).ToList();
                Report report = _nemesysRepository.GetReportById(id);
                Investigation investigation = _nemesysRepository.GetInvestigationByReport(id);
                Status currStatus = _nemesysRepository.GetStatusById(report.StatusId);
                Models.Type currType = _nemesysRepository.GetTypeById(report.TypeId);
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
                            Type = new ListViewModel()
                            {
                                Name = currStatus.Name
                            },
                            Description = report.Description,
                            Status = new ListViewModel()
                            {
                                Name = currType.Name
                            },
                            PhotoUrl = report.PhotoUrl,
                            Upvotes = report.Upvotes,
                            Longitude = report.Longitude,
                            Latitiude = report.Latitude,
                            User = new UserViewModel()
                            {
                                Id = report.UserId,
                                UserName = report.User.UserName
                            }
                        },
                        Id = investigation.Id,
                        Description = investigation.Description,
                        DateOfAction = investigation.DateOfAction,
                        StatusList = statusList,
                        StatusId = report.StatusId

                    };

                    return View(model);

                }
                else
                    return RedirectToAction("Details", "Reports", new { Id=id });
            }
            catch (Exception ex)
            {

                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind("Description, DateOfAction, StatusId")] NewInvestigationViewModel editedInvestigation, [FromRoute] int id)
        {
            try
            {
                var report = _nemesysRepository.GetReportById(id);
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
                    if (report.StatusId != editedInvestigation.StatusId)
                    {
                        report.StatusId = editedInvestigation.StatusId;
                        _nemesysRepository.UpdateReport(report);
                        await _emailSender.SendEmailAsync(report.User.Email, "Your Report's Status has changed!", $"Dear {report.User.UserName},<br>Please refer to <a href='https://universitynemesys.azurewebsites.net/Reports/Details/" + report.Id + "'>this link</a> to view the current status of your report.<br>Sincerely,<br>The Investigation Team");
                    }
                    return RedirectToAction("Details", "Reports", new { id = modelToUpdate.Id });
                }
                else
                {
                    Status currStatus = _nemesysRepository.GetStatusById(report.StatusId);
                    Models.Type currType = _nemesysRepository.GetTypeById(report.TypeId);
                    var statusList = _nemesysRepository.GetAllStatuses().Select(s => new ListViewModel()
                    {
                        Id = s.Id,
                        Name = s.Name
                    }).ToList();
                    editedInvestigation.Report = new ReportViewModel()
                    {
                        Id = report.Id,
                        ReportDate = report.ReportDate,
                        HazardDate = report.HazardDate,
                        Location = report.Location,
                        Type = new ListViewModel()
                        {
                            Name = currStatus.Name
                        },
                        Description = report.Description,
                        Longitude = report.Longitude,
                        Latitiude = report.Latitude,
                        Status = new ListViewModel()
                        {
                            Name = currType.Name
                        },
                        PhotoUrl = report.PhotoUrl,
                        Upvotes = report.Upvotes,
                        User = new UserViewModel()
                        {
                            Id = report.UserId,
                            UserName = report.User.UserName
                        }
                    };
                    editedInvestigation.StatusId = report.StatusId;
                    editedInvestigation.StatusList = statusList;
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

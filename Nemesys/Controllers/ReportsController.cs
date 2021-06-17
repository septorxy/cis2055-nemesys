using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Nemesys.Models;
using Nemesys.Models.Interfaces;
using Nemesys.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nemesys.Controllers
{
    public class ReportsController : Controller
    {   
        private readonly INemesysRepository _nemesysRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ReportsController(INemesysRepository nemesysRepository, UserManager<AppUser> userManager, IEmailSender emailSender)
        {
            _nemesysRepository = nemesysRepository;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            var model = new ReportListViewModel() {
                TotalEntries = _nemesysRepository.GetAllReports().Count(),
                Reports = _nemesysRepository.GetAllReports().OrderByDescending(b => b.ReportDate)
            };
            return View(model);
        }

        public IActionResult Details(int id)
        {
            try
            {
                int currVote = 0;
                var report = _nemesysRepository.GetReportById(id);
                var investigation = _nemesysRepository.GetInvestigationByReport(id);
                Status currStatus = _nemesysRepository.GetStatusById(report.StatusId);
                Models.Type currType = _nemesysRepository.GetTypeById(report.TypeId);
                var getVote = _nemesysRepository.getVoted(report.Id, _userManager.GetUserId(User));
                if(getVote != null)
                {
                    currVote = getVote.vote;
                }
                if (report == null)
                    return NotFound();
                else if (investigation != null)
                {
                    var investigatorUser = _nemesysRepository.GetUserById(investigation.UserId);
                    var model = new ReportViewModel()
                    {
                        Id = report.Id,
                        ReportDate = report.ReportDate,
                        HazardDate = report.HazardDate,
                        Location = report.Location,
                        Type = new ListViewModel() {
                            Id = currType.Id,
                            Name = currType.Name
                        },
                        Description = report.Description,
                        Status = new ListViewModel() {
                            Id = currStatus.Id,
                            Name = currStatus.Name
                        },
                        PhotoUrl = report.PhotoUrl,
                        Upvotes = report.Upvotes,
                        vote = currVote,
                        User = new UserViewModel()
                        {
                            Id = report.UserId,
                            UserName = (_userManager.FindByIdAsync(report.UserId).Result != null) ? _userManager.FindByIdAsync(report.UserId).Result.UserName : "Anonymous"
                        },
                        Investigation = new ViewInvestigationViewModel
                        {
                            Id = investigation.Id,
                            DateOfAction = investigation.DateOfAction,
                            Description = investigation.Description,
                            Investigator = new UserViewModel()
                            {
                                UserName = investigatorUser.UserName,
                                Email = investigatorUser.Email
                            }
                        }
                    };
                    return View(model);
                }
                else
                {
                    var model = new ReportViewModel()
                    {
                        Id = report.Id,
                        ReportDate = report.ReportDate,
                        HazardDate = report.HazardDate,
                        Location = report.Location,
                        Type = new ListViewModel()
                        {
                            Id = currType.Id,
                            Name = currType.Name
                        },
                        Description = report.Description,
                        Status = new ListViewModel()
                        {
                            Id = currStatus.Id,
                            Name = currStatus.Name
                        },
                        PhotoUrl = report.PhotoUrl,
                        vote = currVote,
                        Upvotes = report.Upvotes,
                        User = new UserViewModel()
                        {
                            Id = report.UserId,
                            UserName = (_userManager.FindByIdAsync(report.UserId).Result != null) ? _userManager.FindByIdAsync(report.UserId).Result.UserName : "Anonymous"
                        },
                        Investigation = new ViewInvestigationViewModel
                        {
                            Id = -1
                        }
                    };
                    return View(model);
                }
            }
            catch (Exception ex)
            { 
                return View("Error");
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            var typeList = _nemesysRepository.GetAllTypes().Select(t => new ListViewModel()
            {
                Id = t.Id,
                Name = t.Name
            }).ToList();
            var model = new EditReportViewModel()
            {
                TypeList = typeList
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create([Bind("Description, TypeId, ImageToUpload, Location, HazardDate")] EditReportViewModel newReport)
        {
            if (ModelState.IsValid)
            {
                string fileName = "";
                if (newReport.ImageToUpload != null)
                {
                    //At this point you should check size, extension etc...
                    //Then persist using a new name for consistency (e.g. new Guid)
                    var extension = "." + newReport.ImageToUpload.FileName.Split('.')[newReport.ImageToUpload.FileName.Split('.').Length - 1];
                    fileName = Guid.NewGuid().ToString() + extension;
                    var path = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\reports\\" + fileName;
                    using (var bits = new FileStream(path, FileMode.Create))
                    {
                        newReport.ImageToUpload.CopyTo(bits);
                    }
                }

                Report report = new Report()
                {
                    TypeId = newReport.TypeId,
                    Description = newReport.Description,
                    Location = newReport.Location,
                    HazardDate = newReport.HazardDate,
                    ReportDate = DateTime.UtcNow,
                    StatusId = 1,
                    PhotoUrl = " /images/reports/" + fileName,
                    Upvotes = 0,
                    UserId = _userManager.GetUserId(User)
                };

                _nemesysRepository.CreateReport(report);
                var user = _nemesysRepository.GetUserByUsername(User.Identity.Name);
                _nemesysRepository.UpdateTotalReports(user, 1);
                var AllUsers = _nemesysRepository.GetAllUsers();
                foreach(var aUser in AllUsers)
                {
                    var role = _userManager.GetRolesAsync(aUser);
                    if (role.Equals("Investigator") || role.Equals("Admin"))
                    {
                        if(user != aUser)
                            _emailSender.SendEmailAsync(aUser.Email, "Attention! A Report has been created", $"Dear {report.User.UserName},<br>Please refer to <a href='https://universitynemesys.azurewebsites.net/Reports/Details/" + report.Id + "'>this link</a> to view the new report.<br>Sincerely,<br>The Admin Team");
                    }
                }
                return RedirectToAction("Index");
            }
            else
            {
                var typeList = _nemesysRepository.GetAllTypes().Select(t => new ListViewModel()
                {
                    Id = t.Id,
                    Name = t.Name
                }).ToList();
                newReport.TypeList = typeList;
                return View(newReport);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var report = _nemesysRepository.GetReportById(id);
                var typeList = _nemesysRepository.GetAllTypes().Select(t => new ListViewModel()
                {
                    Id = t.Id,
                    Name = t.Name
                }).ToList();
                if (report != null)
                {
                    //Check if the current user has access to this resource
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (report.User.Id == currentUser.Id)
                    {
                        EditReportViewModel model = new EditReportViewModel()
                        {
                            Id = report.Id,
                            TypeId = report.TypeId,
                            Description = report.Description,
                            PhotoUrl = report.PhotoUrl,
                            HazardDate = report.HazardDate,
                            Location = report.Location,
                            TypeList = typeList
                        };

                        return View(model);
                    }
                    else
                        return Unauthorized();
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
        [Authorize]
        public async Task<IActionResult> Edit([FromRoute] int id, [Bind("Id, TypeId, Description, ImageToUpload, HazardDate, Location")] EditReportViewModel editedReport)
        {
            try
            {
                var modelToUpdate = _nemesysRepository.GetReportById(id);
                if (modelToUpdate == null)
                {
                    return NotFound();
                }

                //Check if the current user has access to this resource
                var currentUser = await _userManager.GetUserAsync(User);
                if (modelToUpdate.User.Id == currentUser.Id)
                {
                    if (ModelState.IsValid)
                    {
                        string photoUrl = "";

                        if (editedReport.ImageToUpload != null)
                        {
                            string fileName = "";
                            //At this point you should check size, extension etc...
                            //Then persist using a new name for consistency (e.g. new Guid)
                            var extension = "." + editedReport.ImageToUpload.FileName.Split('.')[editedReport.ImageToUpload.FileName.Split('.').Length - 1];
                            fileName = Guid.NewGuid().ToString() + extension;
                            var path = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\reports\\" + fileName;
                            using (var bits = new FileStream(path, FileMode.Create))
                            {
                                editedReport.ImageToUpload.CopyTo(bits);
                            }
                            photoUrl = "/images/reports/" + fileName;
                        }
                        else
                           photoUrl = modelToUpdate.PhotoUrl;

                        modelToUpdate.TypeId = editedReport.TypeId;
                        modelToUpdate.Description = editedReport.Description;
                        modelToUpdate.PhotoUrl = photoUrl;
                        modelToUpdate.HazardDate = editedReport.HazardDate;
                        modelToUpdate.Location = editedReport.Location;
                        modelToUpdate.UserId = _userManager.GetUserId(User);

                        _nemesysRepository.UpdateReport(modelToUpdate);

                        return RedirectToAction("Index");
                    }
                    else
                        return Unauthorized(); //or redirect to error controller with 401/403 actions
                }
                else
                {
                    var typeList = _nemesysRepository.GetAllTypes().Select(t => new ListViewModel()
                    {
                        Id = t.Id,
                        Name = t.Name
                    }).ToList();
                    editedReport.TypeList = typeList;

                    return View(editedReport);
                }
            }
            catch (Exception ex)
            {

                return View("Error");
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Delete(int id, EditReportViewModel deletedReport)
        {
            try
            {
                var modelToDelete = _nemesysRepository.GetReportById(id);
                if (modelToDelete == null)
                {
                    return NotFound();
                }

                //Check if the current user has access to this resource
                var currentUser = await _userManager.GetUserAsync(User);
                if (modelToDelete.User.Id == currentUser.Id || _userManager.GetRolesAsync(currentUser).Equals("Admin"))
                {
                    if (modelToDelete.PhotoUrl.Length > 1)
                    {
                        String path = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\reports\\" + Path.GetFileName(modelToDelete.PhotoUrl);
                        System.IO.File.Delete(path);
                    }
                    _nemesysRepository.UpdateTotalReports(modelToDelete.User, -1);
                    modelToDelete.User = null;
                    modelToDelete.UserId = null;
                    _nemesysRepository.DeleteReport(modelToDelete);

                    return RedirectToAction("Index");

                   
                }
                else
                {

                    return RedirectToAction("Details", new { Id = id });
                }
            }
            catch (Exception ex)
            {

                return View("Error");
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Upvote(int id)
        {
            try
            {
                var modelToUpvote = _nemesysRepository.GetReportById(id);
                if (modelToUpvote == null)
                {
                    return NotFound();
                }

                //Check if the current user has access to this resource
                var currentUser = await _userManager.GetUserAsync(User);
                if(_nemesysRepository.getVoted(id, currentUser.Id) == null) 
                {
                    modelToUpvote.Upvotes++;
                }
                else
                {
                    modelToUpvote.Upvotes += 2;
                }

                Vote vote = new Vote()
                {
                   ReportId = id,
                   UserId = currentUser.Id,
                   vote = 1
                };
                _nemesysRepository.setVoted(vote);
                _nemesysRepository.UpdateReport(modelToUpvote);
                return RedirectToAction("Details", new { Id = id });

                
            }
            catch (Exception ex)
            {

                return View("Error");
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Downvote(int id)
        {
            try
            {
                var modelToDownvote = _nemesysRepository.GetReportById(id);
                if (modelToDownvote == null)
                {
                    return NotFound();
                }

                //Check if the current user has access to this resource
                var currentUser = await _userManager.GetUserAsync(User);

                if (_nemesysRepository.getVoted(id, currentUser.Id) == null)
                {
                    modelToDownvote.Upvotes--;
                }
                else
                {
                    modelToDownvote.Upvotes -= 2;
                }

                Vote vote = new Vote()
                {
                    ReportId = id,
                    UserId = currentUser.Id,
                    vote = -1
                };
                _nemesysRepository.setVoted(vote);
                _nemesysRepository.UpdateReport(modelToDownvote);
                return RedirectToAction("Details", new { Id = id });


            }
            catch (Exception ex)
            {

                return View("Error");
            }
        }

    }
}


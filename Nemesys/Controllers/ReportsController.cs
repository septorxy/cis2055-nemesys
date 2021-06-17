using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(INemesysRepository nemesysRepository, UserManager<AppUser> userManager, IEmailSender emailSender, ILogger<ReportsController> logger)
        {
            _nemesysRepository = nemesysRepository;
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                var model = new ReportListViewModel()
                {
                    TotalEntries = _nemesysRepository.GetAllReports().Count(),
                    Reports = _nemesysRepository.GetAllReports().OrderByDescending(b => b.ReportDate)
                };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                return View("Error");
            }
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
                if (getVote != null)
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
                        Upvotes = report.Upvotes,
                        vote = currVote,
                        Longitude = report.Longitude,
                        Latitiude = report.Latitude,
                        User = new UserViewModel()
                        {
                            Id = report.UserId,
                            UserName = _userManager.FindByIdAsync(report.UserId).Result.UserName
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
                        Longitude = report.Longitude,
                        Latitiude = report.Latitude,
                        vote = currVote,
                        Upvotes = report.Upvotes,
                        User = new UserViewModel()
                        {
                            Id = report.UserId,
                            UserName = _userManager.FindByIdAsync(report.UserId).Result.UserName
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
                _logger.LogError(ex, ex.Message, ex.Data);
                return View("Error");
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                return View("Error");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Description, TypeId, ImageToUpload, Location, HazardDate, Longitude, Latitude")] EditReportViewModel newReport)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string fileName = "";
                    if (newReport.ImageToUpload != null)
                    {
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
                        UserId = _userManager.GetUserId(User),
                        Longitude = newReport.Longitude,
                        Latitude = newReport.Latitude
                    };

                    _nemesysRepository.CreateReport(report);
                    var user = _nemesysRepository.GetUserByUsername(User.Identity.Name);
                    _nemesysRepository.UpdateTotalReports(user, 1);
                    var Investigators = await _userManager.GetUsersInRoleAsync("Investigator");
                    foreach (var investigator in Investigators)
                    {
                        if (user != investigator)
                            await _emailSender.SendEmailAsync(investigator.Email, "Attention! A Report has been created", $"Dear {investigator.UserName},<br>Please refer to <a href='https://universitynemesys.azurewebsites.net/Reports/Details/" + report.Id + "'>this link</a> to view the new report.<br>Sincerely,<br>The Admin Team");
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
                    newReport.Longitude = 0;
                    newReport.Latitude = 0;
                    return View(newReport);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                return View("Error");
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
                            TypeList = typeList,
                            Longitude = report.Longitude,
                            Latitude = report.Latitude
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
                _logger.LogError(ex, ex.Message, ex.Data);
                return View("Error");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit([FromRoute] int id, [Bind("Id, TypeId, Description, ImageToUpload, HazardDate, Location, Longitude, Latitude")] EditReportViewModel editedReport)
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
                        modelToUpdate.Longitude = editedReport.Longitude;
                        modelToUpdate.Latitude = editedReport.Latitude;

                        _nemesysRepository.UpdateReport(modelToUpdate);

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var typeList = _nemesysRepository.GetAllTypes().Select(t => new ListViewModel()
                        {
                            Id = t.Id,
                            Name = t.Name
                        }).ToList();
                        editedReport.TypeList = typeList;
                        editedReport.Longitude = 0;
                        editedReport.Latitude = 0;

                        return View(editedReport);
                    }

                }
                else
                {
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
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
                    if (!modelToDelete.PhotoUrl.Equals(" /images/reports/"))
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
                _logger.LogError(ex, ex.Message, ex.Data);
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
                if (_nemesysRepository.getVoted(id, currentUser.Id) == null)
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
                _logger.LogError(ex, ex.Message, ex.Data);
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
                _logger.LogError(ex, ex.Message, ex.Data);
                return View("Error");
            }
        }

    }
}


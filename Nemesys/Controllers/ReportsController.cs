using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

        public ReportsController(INemesysRepository nemesysRepository, UserManager<AppUser> userManager)
        {
            _nemesysRepository = nemesysRepository;
            _userManager = userManager;
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
                var post = _nemesysRepository.GetReportById(id);
                var investigation = _nemesysRepository.GetInvestigationByReport(id);
                Status currStatus = _nemesysRepository.GetStatusById(post.StatusId);
                Models.Type currType = _nemesysRepository.GetTypeById(post.TypeId);
                if (post == null)
                    return NotFound();
                else if (investigation != null)
                {
                    var investigatorUser = _nemesysRepository.GetUserById(investigation.UserId);
                    var model = new ReportViewModel()
                    {
                        Id = post.Id,
                        ReportDate = post.ReportDate,
                        HazardDate = post.HazardDate,
                        Location = post.Location,
                        Type = new ListViewModel() { 
                            Id = currType.Id,
                            Name = currType.Name
                        },
                        Description = post.Description,
                        Status = new ListViewModel() {
                            Id = currStatus.Id,
                            Name = currStatus.Name
                        },
                        PhotoUrl = post.PhotoUrl,
                        Upvotes = post.Upvotes,
                        User = new UserViewModel()
                        {
                            Id = post.UserId,
                            UserName = (_userManager.FindByIdAsync(post.UserId).Result != null) ? _userManager.FindByIdAsync(post.UserId).Result.UserName : "Anonymous"
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
                        Id = post.Id,
                        ReportDate = post.ReportDate,
                        HazardDate = post.HazardDate,
                        Location = post.Location,
                        Type = new ListViewModel()
                        {
                            Id = currType.Id,
                            Name = currType.Name
                        },
                        Description = post.Description,
                        Status = new ListViewModel()
                        {
                            Id = currStatus.Id,
                            Name = currStatus.Name
                        },
                        PhotoUrl = post.PhotoUrl,
                        Upvotes = post.Upvotes,
                        User = new UserViewModel()
                        {
                            Id = post.UserId,
                            UserName = (_userManager.FindByIdAsync(post.UserId).Result != null) ? _userManager.FindByIdAsync(post.UserId).Result.UserName : "Anonymous"
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
            return View();
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
                return RedirectToAction("Index");
            }
            else
                return View(newReport);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var report = _nemesysRepository.GetReportById(id);
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
                            Location = report.Location
                        };

                        //Load all categories and create a list of CategoryViewModel
                        //var categoryList = _nemesysRepository.GetAllCategories().Select(c => new CategoryViewModel()
                        //{
                        //    Id = c.Id,
                        //    Name = c.Name
                        //}).ToList();

                        //Attach to view model - view will pre-select according to the value in CategoryId
                        //model.CategoryList = categoryList;

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
                    //Load all categories and create a list of CategoryViewModel
                    //var categoryList = _nemesysRepository.GetAllCategories().Select(c => new CategoryViewModel()
                    //{
                       // Id = c.Id,
                       // Name = c.Name
                   // }).ToList();

                    //Re-attach to view model before sending back to the View (this is necessary so that the View can repopulate the drop down and pre-select according to the CategoryId
                    //editedReport.CategoryList = categoryList;

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
                if (modelToDelete.User.Id == currentUser.Id)
                {

                    System.IO.File.Delete(modelToDelete.PhotoUrl);
                        _nemesysRepository.UpdateTotalReports(modelToDelete.User, -1);
                        modelToDelete.User = null;
                        modelToDelete.UserId = null;
                        _nemesysRepository.DeleteReport(modelToDelete);

                        return RedirectToAction("Index");

                   
                }
                else
                {

                    return View(deletedReport);
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
            
                modelToUpvote.Upvotes++;
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

                modelToDownvote.Upvotes--;
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


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
        private readonly UserManager<IdentityUser> _userManager;

        public ReportsController(INemesysRepository nemesysRepository, UserManager<IdentityUser> userManager)
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
                if (post == null)
                    return NotFound();
                else
                {
                    var model = new ReportViewModel()
                    {
                        Id = post.Id,
                        ReportDate = post.ReportDate,
                        HazardDate = post.HazardDate,
                        Location = post.Location,
                        Type = post.Type,
                        Description = post.Description,
                        Status = post.Status,
                        PhotoUrl = post.PhotoUrl,
                        Upvotes = post.Upvotes,
                        User = new UserViewModel()
                        {
                            Id = post.ReporterId,
                            UserName = (_userManager.FindByIdAsync(post.ReporterId).Result != null) ? _userManager.FindByIdAsync(post.ReporterId).Result.UserName : "Anonymous"
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

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create([Bind("Description, Type, ImageToUpload")] EditReportViewModel newReport)
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
                    var path = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\blogposts\\" + fileName;
                    using (var bits = new FileStream(path, FileMode.Create))
                    {
                        newReport.ImageToUpload.CopyTo(bits);
                    }
                }

                Report report = new Report()
                {
                    Type = newReport.Type,
                    Description = newReport.Description,
                    ReportDate = DateTime.UtcNow,
                    PhotoUrl = "/images/blogposts/" + fileName,
                    Upvotes = 0,
                    ReporterId = _userManager.GetUserId(User)
                };

                _nemesysRepository.CreateBlogPost(report);
                return RedirectToAction("Index");
            }
            else
                return View(newReport);
        }

    }
}


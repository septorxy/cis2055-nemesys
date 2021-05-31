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

        public ReportsController(INemesysRepository nemesysRepository)
        {
            _nemesysRepository = nemesysRepository;
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
            var post = _nemesysRepository.GetReportById(id);
            if (post == null)
                return NotFound();
            else
                return View(post);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //[HttpPost]
        //public IActionResult Create([Bind("Title, Content, ImageToUpload")] EditReportViewModel newReport)
        //{
           
        //}

    }
}


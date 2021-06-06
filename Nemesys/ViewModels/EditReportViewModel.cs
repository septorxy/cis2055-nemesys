using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nemesys.ViewModels
{
    public class EditReportViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [StringLength(1500, ErrorMessage = "Report must not be longer than 1500 characters")]
        public string Description { get; set; }

        [StringLength(1000, ErrorMessage = "Type needs to be short")]
        public string Type { get; set; }

        public string PhotoUrl { get; set; }
        [Display(Name = "Image")]
        public IFormFile ImageToUpload { get; set; } //used only when submitting form

    }
}
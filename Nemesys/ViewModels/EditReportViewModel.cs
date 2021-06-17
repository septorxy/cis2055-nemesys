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

        [Required(ErrorMessage = "This field is required")]
        [StringLength(1000, ErrorMessage = "Too many characters entered for field: Location")]
        public string Location { get; set; }

        [Display(Name = "Hazard Date")]
        [Required(ErrorMessage = "This field is required")]
        public DateTime HazardDate { get; set; }

        [Required(ErrorMessage = "Field Required")]
        [Range(34, 37)]
        public double Latitude { get; set; }

        [Required(ErrorMessage = "Field Required")]
        [Range(13, 15)]
        public double Longitude { get; set; }

        public string PhotoUrl { get; set; }
        [Display(Name = "Image")]
        public IFormFile ImageToUpload { get; set; } //used only when submitting form

        [Display(Name = "Hazard Type")]
        [Required(ErrorMessage = "This field is required")]
        public int TypeId { get; set; }
        public List<ListViewModel> TypeList { get; set; }

    }
}
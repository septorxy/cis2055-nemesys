using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nemesys.Models
{
    public class Report
    {
        public int Id { get; set; }
        public DateTime ReportDate { get; set; }
        public DateTime HazardDate { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string PhotoUrl { get; set; }
        public int Upvotes { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public int TypeId { get; set; }
        public Type Type { get; set; }
        public int StatusId {get;set;}
        public Status Status { get; set; }


    }
}

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Nemesys.Models;
using Microsoft.AspNetCore.Identity;
using Nemesys.ViewModels;

namespace Nemesys.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            //:)
        }
        public DbSet<Report> Reports { get; set; }

        public DbSet<Investigation> Investigations { get; set; }

        public DbSet<Status> Status { get; set; }

        public DbSet<Models.Type> Type {get;set;}

        public DbSet<Vote> Vote { get; set; }
    }
}

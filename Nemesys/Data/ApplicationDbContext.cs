﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Nemesys.Models;
using Microsoft.AspNetCore.Identity;

namespace Nemesys.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
           //:)
        }
        public DbSet<ProjectRole> ProjectRole { get; set; }
        public DbSet<Report> Reports { get; set; }

    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nemesys.Data;
using Nemesys.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nemesys.Models.Repository
{
    public class NemesysRepository : INemesysRepository
    {
        private readonly ApplicationDbContext _appDbContext;

        public NemesysRepository(ApplicationDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public void CreateBlogPost(Report newReport)
        {
            _appDbContext.Reports.Add(newReport);
            _appDbContext.SaveChanges();
        }

        public IEnumerable<Report> GetAllReports()
        {
            return _appDbContext.Reports.Include(b => b.User).OrderBy(b => b.ReportDate);
        }

        public IEnumerable<IdentityUser> GetAllUsers()
        {
            return _appDbContext.Users;
        }

        public Report GetReportById(int reportId)
        {
            return _appDbContext.Reports.Include(b => b.User).FirstOrDefault(p => p.Id == reportId);
        }

        public IdentityUser GetUserById(string userId)
        {
            return _appDbContext.Users.FirstOrDefault(a => a.Id == userId);
        }

        public void UpdateBlogPost(Report updatedReport)
        {
            throw new NotImplementedException();
        }
    }
}

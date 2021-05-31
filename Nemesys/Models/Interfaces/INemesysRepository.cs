using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nemesys.Models.Interfaces
{
    public interface INemesysRepository
    {
        IEnumerable<Report> GetAllReports();
        Report GetReportById(int ReportId);

        IEnumerable<IdentityUser> GetAllUsers();
        IdentityUser GetUserById(string userId);
        void CreateBlogPost(Report newReport);
        void UpdateBlogPost(Report updatedReport);
    }
}

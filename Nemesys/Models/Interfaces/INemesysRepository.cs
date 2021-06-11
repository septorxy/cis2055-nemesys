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
        IdentityUser GetUserByUsername(string user);
        void CreateReport(Report newReport);
        void UpdateReport(Report updatedReport);
        void CreateInvestigation(Investigation newInvestigation);
    }
}

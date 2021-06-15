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

        IEnumerable<AppUser> GetAllUsers();
        AppUser GetUserById(string userId);
        AppUser GetUserByUsername(string user);
        void CreateReport(Report newReport);
        void UpdateReport(Report updatedReport);
        void CreateInvestigation(Investigation newInvestigation);
        IEnumerable<AppUser> GetTopThree();
        void UpdateTotalReports(AppUser User, int amount);

        void DeleteReport(Report deletedReport);

    }
}

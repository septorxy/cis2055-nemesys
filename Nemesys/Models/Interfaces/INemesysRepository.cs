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
        Investigation GetInvestigationByReport(int reportId);
        void UpdateInvestigation(Investigation updatedInvestigation);
        void CreateStatus(Status status);
        void CreateType(Type type);
        IEnumerable<Status> GetAllStatuses();
        IEnumerable<Type> GetAllTypes();
        Type GetTypeById(int Id);
        Status GetStatusById(int Id);
        void DeleteReport(Report deletedReport);
        Vote getVoted(int reportId, string userId);
        void setVoted(Vote vote);

    }
}

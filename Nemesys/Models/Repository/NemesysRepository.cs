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

        public void CreateInvestigation(Investigation newInvestigation)
        {
            _appDbContext.Investigations.Add(newInvestigation);
            _appDbContext.SaveChanges();
        }

        public void CreateReport(Report newReport)
        {
            _appDbContext.Reports.Add(newReport);
            _appDbContext.SaveChanges();
        }

        public void CreateStatus(Status status)
        {
            _appDbContext.Status.Add(status);
            _appDbContext.SaveChanges();
        }

        public void CreateType(Type type)
        {
            _appDbContext.Type.Add(type);
            _appDbContext.SaveChanges();
        }

        public IEnumerable<Report> GetAllReports()
        {
            return _appDbContext.Reports.Include(b => b.User).OrderBy(b => b.ReportDate);
        }

        public IEnumerable<Status> GetAllStatuses()
        {
            return _appDbContext.Status.Include(b => b.Name).OrderBy(b => b.Name);
        }

        public IEnumerable<Type> GetAllTypes()
        {
            return _appDbContext.Type.Include(b => b.Name).OrderBy(b => b.Name);
        }

        public IEnumerable<AppUser> GetAllUsers()
        {
            return _appDbContext.Users;
        }

        public Investigation GetInvestigationByReport(int reportId)
        {
            return _appDbContext.Investigations.Include(b => b.User).FirstOrDefault(p => p.ReportId == reportId);
        }

        public Report GetReportById(int reportId)
        {
            return _appDbContext.Reports.Include(b => b.User).FirstOrDefault(p => p.Id == reportId);
        }

        public AppUser GetUserById(string userId)
        {
            return _appDbContext.Users.FirstOrDefault(a => a.Id == userId);
        }

        public AppUser GetUserByUsername(string user)
        {
            return _appDbContext.Users.FirstOrDefault(a => a.UserName == user);
        }

        public void UpdateInvestigation(Investigation updatedInvestigation)
        {
            try
            {
                var currInvestigation = _appDbContext.Investigations.SingleOrDefault(rp => rp.Id == updatedInvestigation.Id);
                if (currInvestigation != null)
                {
                    currInvestigation.Description = updatedInvestigation.Description;
                    currInvestigation.DateOfAction = updatedInvestigation.DateOfAction;
                    currInvestigation.UserId = updatedInvestigation.UserId;

                    _appDbContext.Entry(currInvestigation).State = EntityState.Modified;
                    _appDbContext.SaveChanges();
                }
            }
            catch (Exception)
            { 
                throw;
            }
        }

        public void UpdateReport(Report updatedReport)
        {
            try
            {
                var currReport = _appDbContext.Reports.SingleOrDefault(rp => rp.Id == updatedReport.Id);
                if (currReport != null)
                {
                    currReport.Type = updatedReport.Type;
                    currReport.Description = updatedReport.Description;
                    currReport.HazardDate = updatedReport.HazardDate;
                    currReport.PhotoUrl = updatedReport.PhotoUrl;
                    currReport.Location = updatedReport.Location;
                    currReport.UserId = updatedReport.UserId;

                    _appDbContext.Entry(currReport).State = EntityState.Modified;
                    _appDbContext.SaveChanges();
                }
            }
            catch (Exception)
            {
            
                throw;
            }
        }

        public void UpdateTotalReports(AppUser User, int amount)
        {
            var result = _appDbContext.Users.SingleOrDefault(b => b.Id == User.Id);
            if (result != null)
            {
                result.TotalReports = result.TotalReports + amount;
                _appDbContext.SaveChanges();
            }
        }

        IEnumerable<AppUser> INemesysRepository.GetTopThree()
        {
            return _appDbContext.Users.OrderByDescending(u => u.TotalReports).Take(3);
        }
    }
}

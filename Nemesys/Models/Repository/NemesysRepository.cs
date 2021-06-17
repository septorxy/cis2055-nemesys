using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<NemesysRepository> _logger;

        public NemesysRepository(ApplicationDbContext appDbContext, ILogger<NemesysRepository> logger)
        {
            try
            {
                _appDbContext = appDbContext;
                _logger = logger;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                throw;
            }
        }

        public void CreateInvestigation(Investigation newInvestigation)
        {
            try
            {
                _appDbContext.Investigations.Add(newInvestigation);
                _appDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                throw;
            }
        }

        public void CreateReport(Report newReport)
        {
            try
            {
                _appDbContext.Reports.Add(newReport);
                _appDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                throw;
            }
        }

        public void CreateStatus(Status status)
        {
            try
            {
                _appDbContext.Status.Add(status);
                _appDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                throw;
            }
        }

        public void CreateType(Type type)
        {
            try
            {
                _appDbContext.Type.Add(type);
                _appDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                throw;
            }
        }

        public IEnumerable<Report> GetAllReports()
        {
            try
            {
                return _appDbContext.Reports.Include(b => b.User).OrderBy(b => b.ReportDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                throw;
            }
        }

        public IEnumerable<Status> GetAllStatuses()
        {
            try
            {
                return _appDbContext.Status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                throw;
            }
        }

        public IEnumerable<Type> GetAllTypes()
        {
            try
            {
                return _appDbContext.Type;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                throw;
            }
        }

        public IEnumerable<AppUser> GetAllUsers()
        {
            try
            {
                return _appDbContext.Users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                throw;
            }
        }

        public Investigation GetInvestigationByReport(int reportId)
        {
            try
            {
                return _appDbContext.Investigations.Include(b => b.User).FirstOrDefault(p => p.ReportId == reportId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                throw;
            }
        }

        public Report GetReportById(int reportId)
        {
            try
            {
                return _appDbContext.Reports.Include(b => b.User).FirstOrDefault(p => p.Id == reportId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                throw;
            }
        }

        public Status GetStatusById(int Id)
        {
            try
            {
                return _appDbContext.Status.FirstOrDefault(a => a.Id == Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                throw;
            }
        }

        public Type GetTypeById(int Id)
        {
            try
            {
                return _appDbContext.Type.FirstOrDefault(a => a.Id == Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                throw;
            }
        }

        public AppUser GetUserById(string userId)
        {
            try
            {
                return _appDbContext.Users.FirstOrDefault(a => a.Id == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                throw;
            }
        }

        public AppUser GetUserByUsername(string user)
        {
            try
            {
                return _appDbContext.Users.FirstOrDefault(a => a.UserName == user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                throw;
            }
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
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
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
                    currReport.TypeId = updatedReport.TypeId;
                    currReport.StatusId = updatedReport.StatusId;
                    currReport.Description = updatedReport.Description;
                    currReport.HazardDate = updatedReport.HazardDate;
                    currReport.PhotoUrl = updatedReport.PhotoUrl;
                    currReport.Location = updatedReport.Location;
                    currReport.UserId = updatedReport.UserId;

                    _appDbContext.Entry(currReport).State = EntityState.Modified;
                    _appDbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                throw;
            }
        }

        public void UpdateTotalReports(AppUser User, int amount)
        {
            try
            {
                var result = _appDbContext.Users.SingleOrDefault(b => b.Id == User.Id);
                if (result != null)
                {
                    result.TotalReports = result.TotalReports + amount;
                    _appDbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                throw;
            }
        }

        IEnumerable<AppUser> INemesysRepository.GetTopThree()
        {
            try
            {
                return _appDbContext.Users.OrderByDescending(u => u.TotalReports).Take(3);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                throw;
            }
        }

        public void DeleteReport(Report deletedReport)
        {
            try
            {
                _appDbContext.Remove(deletedReport);
                _appDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                throw;
            }
        }

        public Vote getVoted(int reportId, string userId)
        {
            try
            {
                return _appDbContext.Vote.SingleOrDefault(v => v.UserId == userId && v.ReportId == reportId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                throw;
            }
        }

        public void setVoted(Vote vote)
        {
            try
            {
                var result = _appDbContext.Vote.SingleOrDefault(v => v.UserId == vote.UserId && v.ReportId == vote.ReportId);
                if (result != null)
                {
                    result.vote = vote.vote;
                    _appDbContext.Entry(result).State = EntityState.Modified;
                }
                else
                {
                    _appDbContext.Vote.Add(vote);
                }
                _appDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                throw;
            }
        }
    }
}

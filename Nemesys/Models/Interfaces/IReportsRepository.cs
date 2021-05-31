using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nemesys.Models.Interfaces
{
    public interface IReportsRepository
    {
        IEnumerable<Report> GetAllReports();
        Report GetReportById(int reportId);
    }
}

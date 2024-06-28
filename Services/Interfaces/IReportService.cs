using Models.DTOs.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IReportService
    {
        Task<SalesOrderSummary> SalesReportSummary(DateTime startDate, DateTime endDate);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tourism_firm.Interfaces
{
    public interface IReportAdapter
    {
        string GenerateSalesReport(DateTime fromDate, DateTime toDate);
        string GenerateEmployeeStatsReport(DateTime fromDate, DateTime toDate);
    }
}

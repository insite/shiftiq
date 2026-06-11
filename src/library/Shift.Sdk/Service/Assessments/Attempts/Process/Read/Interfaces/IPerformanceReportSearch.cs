using System.Collections.Generic;

namespace InSite.Application.Attempts.Read
{
    public interface IPerformanceReportSearch
    {
        List<VPerformanceReport> GetReport(VPerformanceReportFilter filter);
    }
}

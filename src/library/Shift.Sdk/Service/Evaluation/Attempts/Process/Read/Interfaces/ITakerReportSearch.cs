using System.Collections.Generic;

namespace InSite.Application.Attempts.Read
{
    public interface ITakerReportSearch
    {
        List<TakerReportItem> GetTakerReport(QAttemptFilter filter);
    }
}

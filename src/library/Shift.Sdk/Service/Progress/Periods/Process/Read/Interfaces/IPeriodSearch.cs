using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace InSite.Application.Records.Read
{
    public interface IPeriodSearch
    {
        QPeriod GetPeriod(Guid periodIdentifier, params Expression<Func<QPeriod, object>>[] includes);
        QPeriod[] GetPeriods(IEnumerable<Guid> periodIdentifiers, params Expression<Func<QPeriod, object>>[] includes);
        bool PeriodExists(QPeriodFilter filter);
        int CountPeriods(QPeriodFilter filter);
        List<QPeriod> GetPeriods(QPeriodFilter filter, params Expression<Func<QPeriod, object>>[] includes);
    }
}

using System.Linq;

using Shift.Common;

namespace InSite.Persistence.Plugin.NCSHA
{
    public static class HistoryFilterHelper
    {
        public static IQueryable<History> ApplyFilter(this IQueryable<History> query, HistoryFilter filter)
        {
            if (filter.EventTypeInclude.IsNotEmpty())
                query = query.Where(x => filter.EventTypeInclude.Contains(x.EventType));

            if (filter.EventTypeExclude.IsNotEmpty())
                query = query.Where(x => !filter.EventTypeExclude.Contains(x.EventType));

            if (filter.RecordTime != null)
            {
                if (filter.RecordTime.Since.HasValue)
                    query = query.Where(x => x.RecordTime >= filter.RecordTime.Since.Value);

                if (filter.RecordTime.Before.HasValue)
                    query = query.Where(x => x.RecordTime < filter.RecordTime.Before.Value);
            }

            if (filter.UserId.HasValue)
                query = query.Where(x => x.UserId == filter.UserId.Value);

            if (filter.UserEmail.IsNotEmpty())
                query = query.Where(x => x.UserEmail.Contains(filter.UserEmail));

            return query;
        }
    }
}

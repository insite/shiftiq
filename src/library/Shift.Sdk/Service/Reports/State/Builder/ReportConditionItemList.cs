using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common;

namespace InSite.Domain.Reports
{
    [Serializable]
    public class ReportConditionItemList : List<ReportConditionItem>
    {
        public ReportConditionClauseType ConditionClause { get; set; } = ReportConditionClauseType.And;

        public string GetSql(bool? isStatistic = false)
        {
            var sqlClause = ConditionClause == ReportConditionClauseType.And
                ? " AND "
                : " OR ";

            var query = this.AsQueryable();

            if (isStatistic.HasValue)
                query = query.Where(x => x.Column.IsStatistic == isStatistic.Value);

            return string.Join(
                sqlClause,
                query.Select(x => x.GetSql(isStatistic)).Where(x => x.IsNotEmpty()));
        }
    }
}

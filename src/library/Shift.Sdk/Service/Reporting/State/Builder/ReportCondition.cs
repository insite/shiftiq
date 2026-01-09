using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Reports
{
    [Serializable]
    public class ReportCondition
    {
        public string Name { get; set; }

        public ReportConditionItemList Where { get; } = new ReportConditionItemList();
        public ReportConditionItemList And { get; } = new ReportConditionItemList();
        public ReportConditionItemList Or { get; } = new ReportConditionItemList();

        [JsonIgnore]
        public bool IsEmpty => Where.Count == 0 && Or.Count == 0 && And.Count == 0;

        public string GetSql(bool? isStatistic = null)
        {
            var result = string.Empty;

            AppendCondition(Where, null);
            AppendCondition(And, " AND ");
            AppendCondition(Or, " OR ");

            return result;

            void AppendCondition(ReportConditionItemList items, string clause)
            {
                var sql = items.GetSql(isStatistic);
                if (sql.IsEmpty())
                    return;

                if (clause != null && result.Length > 0)
                    result += clause;

                result += "(" + sql + ")";
            }
        }

        public bool ShouldSerializeWhere() => Where.Count > 0;
        public bool ShouldSerializeOr() => Or.Count > 0;
        public bool ShouldSerializeAnd() => And.Count > 0;

        public void RestoreColumns(List<ReportColumn> columns)
        {
            Restore(Where);
            Restore(And);
            Restore(Or);

            void Restore(ReportConditionItemList list)
            {
                foreach (var item in list)
                {
                    var column = columns.Find(x => x.Name.Equals(item.ColumnName, StringComparison.OrdinalIgnoreCase));
                    if (column == null)
                        throw new ArgumentException($"Column '{item.ColumnName}' is not found");

                    item.Column = column;
                }
            }
        }

        public void RemoveColumns(Predicate<ReportConditionItem> match)
        {
            Where.RemoveAll(match);
            And.RemoveAll(match);
            Or.RemoveAll(match);
        }
    }
}

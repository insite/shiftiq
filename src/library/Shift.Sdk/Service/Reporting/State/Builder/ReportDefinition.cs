using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Newtonsoft.Json;

using Shift.Common;

using FunctionType = InSite.Domain.Reports.ReportAggregate.FunctionType;

namespace InSite.Domain.Reports
{
    [Serializable]
    public class ReportDefinition
    {
        #region Classes

        [Flags]
        private enum ColumnType
        {
            None = 0,
            Select = 1,
            Output = 2
        }

        private enum QueryType
        {
            Select = 0,
            Count = 1
        }

        private class QueryColumn
        {
            public string Body { get; set; }
            public string Alias { get; set; }
            public ColumnType Type { get; set; }
        }

        private class QueryColumns
        {
            public List<QueryColumn> Columns { get; } = new List<QueryColumn>();

            public List<QueryColumn> StatisticAggregateColumns { get; } = new List<QueryColumn>();

            public List<QueryColumn> StatisticCustomColumns { get; } = new List<QueryColumn>();
        }

        #endregion

        public string DataSource { get; set; }

        public List<ReportAggregate> Aggregates { get; } = new List<ReportAggregate>();
        public List<ReportColumn> Columns { get; } = new List<ReportColumn>();
        public List<ReportCondition> Conditions { get; } = new List<ReportCondition>();

        private static readonly JsonSerializerSettings _jsonSerializationSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };

        public ReportDefinition(string dataSourceName)
        {
            DataSource = dataSourceName;
        }

        #region Serialization

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, _jsonSerializationSettings);
        }

        public static ReportDefinition Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<ReportDefinition>(json);
        }

        public bool ShouldSerializeColumns() => Columns.IsNotEmpty();

        public bool ShouldSerializeAggregates() => Aggregates.IsNotEmpty();

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            foreach (var c in Conditions)
                c.RestoreColumns(Columns);
        }

        #endregion

        #region SQL generation

        public string GetSelectSql(Guid organizationId, int? conditionIndex)
        {
            return BuildSqlQuery(QueryType.Select, organizationId, conditionIndex);
        }

        public string GetSelectSql(Guid organizationId, int? conditionIndex, int skip, int take)
        {
            if (take == 0)
                take = 1;

            return BuildSqlQuery(QueryType.Select, organizationId, conditionIndex) + $" OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";
        }

        public string GetCountSql(Guid organizationId, int? conditionIndex)
        {
            return BuildSqlQuery(QueryType.Count, organizationId, conditionIndex);
        }

        private string BuildSqlQuery(QueryType type, Guid organizationId, int? conditionIndex)
        {
            const string dataView = "QueryData";
            var dataSource = ReportDataSourceReader.ReadDataSource(DataSource);
            var cols = GetQueryColumns(Columns, Aggregates, dataView);

            var result = new StringBuilder();

            if (cols.StatisticAggregateColumns.Count == 0 && cols.StatisticCustomColumns.Count == 0)
            {
                if (type == QueryType.Count)
                    result.AppendLine("SELECT COUNT(*)").AppendLine()
                        .AppendLine("FROM").AppendLine()
                        .AppendLine("(").AppendLine();

                result.Append("SELECT DISTINCT ");

                AppendQueryColumns(result, cols.Columns, ColumnType.Select, 0);

                result
                    .AppendLine()
                    .AppendLine()
                    .Append("FROM ");

                AppendQuerySource(result, dataSource, cols.Columns.Select(x => x.Body));

                result
                    .AppendLine()
                    .AppendLine()
                    .Append("WHERE ");

                AppendQuerySelectCondition(
                    result,
                    dataSource,
                    organizationId,
                    Conditions.TryGetItem(conditionIndex ?? 0));

                if (type == QueryType.Count)
                    result.AppendLine().AppendLine().Append(") AS N");
                else
                    result
                        .AppendLine()
                        .AppendLine()
                        .Append("ORDER BY ")
                        .Append(string.Join(", ", cols.Columns.Select(x => "[" + x.Alias + "]").Take(3)));
            }
            else
            {
                result
                    .Append("WITH ").Append(dataView).Append(" AS (").AppendLine()
                    .Append("    SELECT").AppendLine();

                AppendQueryColumns(result, cols.Columns, ColumnType.Select, 2);

                var groupByColumns = cols.Columns
                    .Where(x => x.Type.HasFlag(ColumnType.Select) && x.Type.HasFlag(ColumnType.Output))
                    .Select(x => x.Body)
                    .Distinct()
                    .ToArray();

                result.AppendLine()
                    .Append("       ,")
                    .Append("DENSE_RANK() OVER (ORDER BY ")
                    .Append(string.Join(", ", groupByColumns))
                    .Append(") AS GroupNumber")
                    .AppendLine();

                result.Append("       ,")
                    .Append("ROW_NUMBER() OVER (PARTITION BY ")
                    .Append(string.Join(", ", groupByColumns))
                    .Append(" ORDER BY ")
                    .Append(string.Join(", ", groupByColumns.Take(1)))
                    .Append(") AS RowNumber")
                    .AppendLine();

                result
                    .Append("    FROM").AppendLine().Append("        ");

                AppendQuerySource(result, dataSource, groupByColumns);

                result
                    .AppendLine()
                    .Append("    WHERE").AppendLine().Append("        ");

                AppendQuerySelectCondition(
                    result,
                    dataSource,
                    organizationId,
                    Conditions.TryGetItem(conditionIndex ?? 0));

                result
                    .AppendLine()
                    .AppendLine("), DataGroups AS (")
                    .AppendLine("    SELECT DISTINCT GroupNumber FROM QueryData")
                    .Append(")");

                if (cols.StatisticAggregateColumns.Count > 0)
                {
                    result
                        .AppendLine(", StatisticAggregateData AS (")
                        .AppendLine("    SELECT")
                        .AppendLine("        GroupNumber");

                    foreach (var col in cols.StatisticAggregateColumns)
                        result.Append("       ,").Append(col.Body).Append(" AS [").Append(col.Alias).Append("]").AppendLine();

                    result
                        .AppendLine("    FROM")
                        .AppendLine("        QueryData")
                        .AppendLine("    GROUP BY")
                        .AppendLine("        GroupNumber")
                        .Append(")");
                }

                if (cols.StatisticCustomColumns.Count > 0)
                {
                    result
                        .AppendLine(", StatisticCustomData AS (")
                        .AppendLine("    SELECT")
                        .AppendLine("        DataGroups.GroupNumber");


                    foreach (var col in cols.StatisticCustomColumns)
                        result.Append("       ,").Append(col.Body).Append(" AS [").Append(col.Alias).Append("]").AppendLine();

                    result
                        .AppendLine("    FROM")
                        .AppendLine("        DataGroups")
                        .Append(")");
                }

                result
                    .AppendLine()
                    .AppendLine("SELECT");

                if (type == QueryType.Count)
                    result.Append("COUNT(*)");
                else
                    AppendQueryColumns(result, cols.Columns, ColumnType.Output, 1);

                result
                    .AppendLine()
                    .AppendLine("FROM")
                    .Append("    ").AppendLine(dataView);

                if (cols.StatisticAggregateColumns.Count > 0)
                    result.Append("    INNER JOIN StatisticAggregateData ON ").Append(dataView).AppendLine(".GroupNumber = StatisticAggregateData.GroupNumber");

                if (cols.StatisticCustomColumns.Count > 0)
                    result.Append("    INNER JOIN StatisticCustomData ON ").Append(dataView).AppendLine(".GroupNumber = StatisticCustomData.GroupNumber");

                result
                    .AppendLine("WHERE")
                    .Append("    ").Append(dataView).Append(".RowNumber = 1");

                AppendQueryStatisticCondition(
                    result,
                    Conditions.TryGetItem(conditionIndex ?? 0));

                var orderByColumns = cols.Columns
                    .Where(x => x.Type.HasFlag(ColumnType.Output))
                    .Select(x => "[" + x.Alias + "]")
                    .ToArray();

                if (type == QueryType.Select)
                    result
                        .AppendLine()
                        .AppendLine("ORDER BY")
                        .Append("    ").Append(string.Join(", ", orderByColumns.Take(3)));
            }

            return result.ToString();
        }

        private static QueryColumns GetQueryColumns(IEnumerable<ReportColumn> columns, IEnumerable<ReportAggregate> aggregates, string dataView)
        {
            var result = new QueryColumns();

            var validCols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var col in columns)
            {
                if (validCols.Add(col.Name))
                    result.Columns.Add(new QueryColumn
                    {
                        Alias = col.Name,
                        Body = col.Name,
                        Type = col.IsStatistic ? ColumnType.Output : ColumnType.Select | ColumnType.Output
                    });
            }

            foreach (var agg in aggregates)
            {
                var colName = ReportAggregate.ViewAlias + "." + agg.Alias;

                if (!validCols.Contains(colName))
                    continue;

                if (agg.Function == FunctionType.Count)
                {
                    result.StatisticAggregateColumns.Add(new QueryColumn
                    {
                        Body = "COUNT(*)",
                        Alias = colName
                    });
                }
                else if (agg.Function == FunctionType.Sum)
                {
                    result.Columns.Add(new QueryColumn
                    {
                        Body = agg.Column.Name,
                        Alias = agg.Alias + "_Value",
                        Type = ColumnType.Select
                    });

                    result.StatisticAggregateColumns.Add(new QueryColumn
                    {
                        Body = $"SUM([{agg.Alias}_Value])",
                        Alias = colName
                    });
                }
                else if (agg.Function == FunctionType.Min)
                {
                    result.Columns.Add(new QueryColumn
                    {
                        Body = agg.Column.Name,
                        Alias = agg.Alias + "_Value",
                        Type = ColumnType.Select
                    });

                    result.StatisticAggregateColumns.Add(new QueryColumn
                    {
                        Body = $"MIN([{agg.Alias}_Value])",
                        Alias = colName
                    });
                }
                else if (agg.Function == FunctionType.Max)
                {
                    result.Columns.Add(new QueryColumn
                    {
                        Body = agg.Column.Name,
                        Alias = agg.Alias + "_Value",
                        Type = ColumnType.Select
                    });

                    result.StatisticAggregateColumns.Add(new QueryColumn
                    {
                        Body = $"MAX([{agg.Alias}_Value])",
                        Alias = colName
                    });
                }
                else if (agg.Function == FunctionType.Mean)
                {
                    result.Columns.Add(new QueryColumn
                    {
                        Body = agg.Column.Name,
                        Alias = agg.Alias + "_Value",
                        Type = ColumnType.Select
                    });

                    result.StatisticAggregateColumns.Add(new QueryColumn
                    {
                        Body = $"AVG([{agg.Alias}_Value] * 1.0)",
                        Alias = colName
                    });
                }
                else if (agg.Function == FunctionType.Median)
                {
                    result.Columns.Add(new QueryColumn
                    {
                        Body = agg.Column.Name,
                        Alias = agg.Alias + "_Value",
                        Type = ColumnType.Select
                    });

                    result.StatisticCustomColumns.Add(new QueryColumn
                    {
                        Body = $@"(
            SELECT
                AVG(1.0 * [Value])
            FROM
                (
                    SELECT
                        [{agg.Alias}_Value] AS [Value]
                       ,COUNT(*) OVER () AS [RowCount]
                       ,ROW_NUMBER() OVER (ORDER BY [{agg.Alias}_Value] ASC) AS [RowNumber]
                    FROM
                        {dataView}
                    WHERE
                        {dataView}.GroupNumber = DataGroups.GroupNumber
                        AND [{agg.Alias}_Value] IS NOT NULL
                ) AS SubQuery
            WHERE
                [RowNumber] IN (([RowCount] + 1)/2, ([RowCount] + 2)/2)
        )",
                        Alias = colName
                    });
                }
                else if (agg.Function == FunctionType.Mode)
                {
                    result.Columns.Add(new QueryColumn
                    {
                        Body = agg.Column.Name,
                        Alias = agg.Alias + "_Value",
                        Type = ColumnType.Select
                    });

                    result.StatisticCustomColumns.Add(new QueryColumn
                    {
                        Body = $@"(
            SELECT
                STRING_AGG([Value],', ') WITHIN GROUP (ORDER BY [Value])
            FROM
                (
                    SELECT TOP 1 WITH TIES
                        [{agg.Alias}_Value] AS [Value]
                    FROM
                        {dataView}
                    WHERE
                        {dataView}.GroupNumber = DataGroups.GroupNumber
                        AND [{agg.Alias}_Value] IS NOT NULL
                    GROUP BY
                        [{agg.Alias}_Value]
                    ORDER BY
                        COUNT(1) DESC
                ) AS SubQuery
        )",
                        Alias = colName
                    });
                }
            }

            return result;
        }

        private static void AppendQueryColumns(StringBuilder builder, IList<QueryColumn> columns, ColumnType type, int indent)
        {
            var cIndex = 0;
            var sIndent = indent > 0
                ? new string(' ', indent * 4)
                : " ";

            for (; cIndex < columns.Count; cIndex++)
            {
                if (AppendColumn(columns[cIndex]))
                {
                    cIndex++;
                    break;
                }
            }

            if (indent > 0)
                sIndent = Environment.NewLine + new string(' ', indent * 4 - 1) + ',';
            else
                sIndent = " ,";

            for (; cIndex < columns.Count; cIndex++)
                AppendColumn(columns[cIndex]);

            bool AppendColumn(QueryColumn c)
            {
                if (type == ColumnType.Select)
                {
                    if (!c.Type.HasFlag(type) || c.Body.IsEmpty())
                        return false;

                    builder.Append(sIndent).Append(c.Body);

                    if (c.Alias.IsNotEmpty())
                        builder.Append(" AS [").Append(c.Alias).Append("]");
                }
                else if (type == ColumnType.Output)
                {
                    if (!c.Type.HasFlag(type) || c.Alias.IsEmpty())
                        return false;

                    builder.Append(sIndent).Append("[").Append(c.Alias).Append("]");
                }
                else
                    throw new NotImplementedException();

                return true;
            }
        }

        private static void AppendQuerySource(StringBuilder sql, ReportDataSource dataSource, IEnumerable<string> columns)
        {
            sql.Append(dataSource.View.Name).Append(" AS ").Append(dataSource.View.Alias);

            var viewAliases = columns
                .Select(x => x.Substring(0, x.IndexOf('.')))
                .Distinct()
                .ToArray();

            if (viewAliases.Length == 0)
                return;

            foreach (var viewAlias in viewAliases)
            {
                var join = dataSource.Joins.FirstOrDefault(x => x.Alias == viewAlias);
                if (join != null)
                    sql.Append(" ").Append(join.Expression);
            }
        }

        private static void AppendQuerySelectCondition(StringBuilder sql, ReportDataSource dataSource, Guid organizationId, ReportCondition condition)
        {
            sql.Append(dataSource.View.Alias).Append(".OrganizationIdentifier = '").Append(organizationId).Append("'");

            var sqlCondition = condition?.GetSql(false);
            if (sqlCondition.IsNotEmpty())
                sql.Append(" AND ").Append(sqlCondition);
            else
                sql.Append(" AND 1 = 1");
        }

        private static void AppendQueryStatisticCondition(StringBuilder sql, ReportCondition condition)
        {
            var sqlCondition = condition?.GetSql(true);
            if (sqlCondition.IsNotEmpty())
                sql.Append(" AND ").Append(sqlCondition);
            else
                sql.Append(" AND 1 = 1");
        }

        #endregion
    }
}

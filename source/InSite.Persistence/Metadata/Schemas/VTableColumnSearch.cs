using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;

using Shift.Common.Linq;

namespace InSite.Persistence
{
    public static class VTableColumnSearch
    {
        public class VTableColumnInfo
        {
            public string ColumnName { get; set; }
            public string DataType { get; set; }
            public bool? IsIdentity { get; set; }
            public bool? IsRequired { get; set; }
            public int? MaximumLength { get; set; }
            public int? OrdinalPosition { get; set; }
            public int SchemaId { get; set; }
            public string SchemaName { get; set; }
            public int TableId { get; set; }
            public string TableName { get; set; }

            public int NonNullCount { get; set; }
            public decimal NonNullPercent { get; set; }
            public int DistinctCount { get; set; }
            public decimal DistinctPercent { get; set; }
        }

        private static IQueryable<VTableColumn> CreateQuery(ReportDbContext db, VTableColumnFilter filter)
        {
            var query = db.VTableColumns.AsNoTracking().AsQueryable();

            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.SchemaName))
                    query = filter.IsExactComparison
                        ? query.Where(x => x.SchemaName == filter.SchemaName)
                        : query.Where(x => x.SchemaName.Contains(filter.SchemaName));

                if (!string.IsNullOrEmpty(filter.TableName))
                    query = filter.IsExactComparison
                        ? query.Where(x => x.TableName == filter.TableName)
                        : query.Where(x => x.TableName.Contains(filter.TableName));

                if (!string.IsNullOrEmpty(filter.ColumnName))
                    query = filter.IsExactComparison
                        ? query.Where(x => x.ColumnName == filter.ColumnName)
                        : query.Where(x => x.ColumnName.Contains(filter.ColumnName));

                if (filter.IsRequired.HasValue)
                    query = query.Where(x => x.IsRequired == filter.IsRequired.Value);
            }

            return query.AsNoTracking();
        }

        public static int Count(VTableColumnFilter filter)
        {
            using (var db = new ReportDbContext())
            {
                return CreateQuery(db, filter).Count();
            }
        }

        public static VTableColumnInfo Select(string schema, string table, string column, string where = null)
        {
            using (var db = new ReportDbContext())
            {
                var info = db.VTableColumns.AsNoTracking()
                    .Single(x => x.SchemaName == schema && x.TableName == table && x.ColumnName == column);

                var result = new VTableColumnInfo { 
                    ColumnName = info.ColumnName,
                    DataType = info.DataType,
                    IsIdentity = info.IsIdentity,
                    IsRequired = info.IsRequired,
                    MaximumLength = info.MaximumLength,
                    OrdinalPosition = info.OrdinalPosition,
                    SchemaId = info.SchemaId,
                    SchemaName = info.SchemaName,
                    TableId = info.TableId,
                    TableName = info.TableName
                };

                var extraData = SelectExtraData(schema, table, column, where);
                if (extraData != null)
                {
                    var rowCount = (decimal)(int)extraData["RowCount"];

                    var nonNullCount = (int)extraData["NonNullCount"];
                    var distinctCount = (int)extraData["DistinctCount"];

                    result.NonNullCount = nonNullCount;
                    result.NonNullPercent = rowCount == 0 ? 0m : Math.Round(100m * nonNullCount / rowCount, 2);
                    result.DistinctCount = distinctCount;
                    result.DistinctPercent = nonNullCount == 0 ? 0m : Math.Round(100m * distinctCount / nonNullCount, 2);
                }

                return result;
            }
        }

        public static DataTable Select(VTableColumnFilter filter)
        {
            using (var db = new ReportDbContext())
            {
                var query = CreateQuery(db, filter)
                    .OrderBy(x => x.SchemaName)
                    .ThenBy(x => x.TableName)
                    .ThenBy(x => x.ColumnName)
                    .ApplyPaging(filter);
                    
                return CreateTable(query);
            }
        }

        #region Methods (private)

        private static DataTable CreateTable(IEnumerable<VTableColumn> list)
        {
            var table = new DataTable();

            table.Columns.Add("SchemaID", typeof(int));
            table.Columns.Add("SchemaName");
            table.Columns.Add("SchemaColor");

            table.Columns.Add("TableID", typeof(int));
            table.Columns.Add("TableName");

            table.Columns.Add("ColumnName");
            table.Columns.Add("DataType");
            table.Columns.Add("MaximumLength", typeof(int));
            table.Columns.Add("IsRequired", typeof(bool));
            table.Columns.Add("IsIdentity", typeof(bool));
            table.Columns.Add("OrdinalPosition", typeof(int));

            foreach (var item in list)
            {
                var row = table.NewRow();

                row["SchemaID"] = item.SchemaId;
                row["SchemaName"] = item.SchemaName;

                row["TableID"] = item.TableId;
                row["TableName"] = item.TableName;

                row["ColumnName"] = item.ColumnName;
                row["DataType"] = item.DataType;

                if (item.MaximumLength.HasValue)
                    row["MaximumLength"] = item.MaximumLength.Value;

                row["IsRequired"] = item.IsRequired;
                row["IsIdentity"] = item.IsIdentity;
                row["OrdinalPosition"] = item.OrdinalPosition;

                table.Rows.Add(row);
            }

            return table;
        }

        private static DataRow SelectExtraData(string schemaName, string tableName, string columnName, string where = null)
        {
            var table = new DataTable();

            var query = new StringBuilder();
            query.Append("SELECT COUNT(*) AS [RowCount]");

            query.AppendFormat(",COUNT([{0}]) AS [NonNullCount],COUNT(DISTINCT [{0}]) AS [DistinctCount]",
                columnName);

            query.AppendFormat(" FROM [{0}].[{1}] WITH(NOLOCK)", schemaName, tableName);

            if (where != null)
                query.AppendFormat(" WHERE {0}", where);

            using (var db = new ReportDbContext())
            {
                var connection = db.Database.Connection;
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query.ToString();
                    command.CommandType = CommandType.Text;
                    using (var reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }

                connection.Close();
            }

            return (table.Rows.Count > 0) ? table.Rows[0] : null;
        }

        #endregion
    }
}
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

using Shift.Common.Linq;

namespace InSite.Persistence
{
    public static class VTableSearch
    {
        public static string ConnectionString => new ReportDbContext().Database.Connection.ConnectionString;

        public static int Count(VTableFilter filter)
        {
            using (var context = new ReportDbContext())
            {
                return CreateQuery(context, filter).Count();
            }
        }

        public static int CountSchemas(VTableFilter filter)
        {
            using (var context = new ReportDbContext())
            {
                return CreateQuery(context, filter).Select(x => x.SchemaName).Distinct().Count();
            }
        }

        public static DataTable Select(string schemaName, string tableName)
        {
            var query = $"SELECT * FROM databases.VTable WHERE SchemaName = @SchemaName AND TableName = @TableName";

            var table = new DataTable();

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("SchemaName", schemaName);
                    command.Parameters.AddWithValue("TableName", tableName);

                    using (var reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
                connection.Close();
            }

            return table;
        }

        public static List<VTable> Select(VTableFilter filter)
        {
            using (var context = new ReportDbContext())
            {
                return CreateQuery(context, filter)
                    .OrderBy(x => x.SchemaName)
                    .ThenBy(x => x.TableName)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        #region Methods (helper)

        private static IQueryable<VTable> CreateQuery(ReportDbContext db, VTableFilter filter)
        {
            var query = db.VTables.AsNoTracking().AsQueryable();

            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.SchemaName))
                    query = query.Where(x => x.SchemaName.Contains(filter.SchemaName));

                if (!string.IsNullOrEmpty(filter.TableName))
                    query = query.Where(x => x.TableName.Contains(filter.TableName));
            }

            return query.AsNoTracking();
        }

        public static List<T> Select<T>(string query)
        {
            using (var db = new ReportDbContext())
            {
                return db.Database.SqlQuery<T>(query).ToList();
            }
        }

        #endregion
    }
}
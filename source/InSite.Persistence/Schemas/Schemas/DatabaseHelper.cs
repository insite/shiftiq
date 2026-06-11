using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace InSite.Persistence
{
    public class DatabaseHelper
    {
        public static DataTable CreateDataTable(string query, params SqlParameter[] parameters)
        {
            return CreateDataTable(query, null, parameters);
        }

        public static DataTable CreateDataTable(string query, int? commandTimeout, params SqlParameter[] parameters)
        {
            var table = new DataTable();

            using (var db = new InternalDbContext())
            {
                var conn = db.Database.Connection;

                if (conn.State != ConnectionState.Open)
                    conn.Open();

                try
                {
                    using (var command = CreateCommand(conn, query, commandTimeout, parameters))
                    {
                        using (var reader = command.ExecuteReader())
                            table.Load(reader);
                    }
                }
                finally
                {
                    conn.Close();
                }
            }

            AllowDbNull(table.Columns);

            return table;
        }

        public static int ExecuteCount(string query, params SqlParameter[] parameters)
        {
            using (var db = new InternalDbContext())
            {
                return db.Database.SqlQuery<int>(query, parameters).Single();
            }
        }

        private static DbCommand CreateCommand(DbConnection connection, string query, int? commandTimeout, SqlParameter[] parameters)
        {
            var command = connection.CreateCommand();

            command.CommandText = query;

            if (commandTimeout.HasValue)
                command.CommandTimeout = commandTimeout.Value;

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    if (!parameter.ParameterName.StartsWith("@"))
                        parameter.ParameterName = "@" + parameter.ParameterName;

                    if (parameter.Value == null)
                        parameter.Value = DBNull.Value;

                    command.Parameters.Add(parameter);
                }
            }

            return command;
        }

        private static void AllowDbNull(DataColumnCollection columns)
        {
            foreach (DataColumn column in columns)
                column.AllowDBNull = true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using Shift.Common;

namespace InSite.Common.Data
{
    public class Database
    {
        private readonly string _connectionString;
        private readonly TimeZoneInfo _timezone;
        private readonly bool _isHtml;

        public Database(string connectionString, TimeZoneInfo timezone, bool isHtml = true)
        {
            _connectionString = connectionString;
            _timezone = timezone;
            _isHtml = isHtml;
        }

        public DataTable SqlQuery(string query)
        {
            var data = new DataTable();

            using (var sql = new SqlConnection(_connectionString))
            {
                sql.Open();

                using (var select = new SqlCommand(query, sql))
                {
                    using (var reader = select.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        var valueGetters = new List<Func<object, object>>();

                        var schemaTable = reader.GetSchemaTable();
                        foreach (DataRow sRow in schemaTable.Rows)
                        {
                            var name = (string)sRow["ColumnName"];
                            var type = (Type)sRow["DataType"];

                            if (type == typeof(DateTimeOffset))
                            {
                                type = typeof(string);
                                valueGetters.Add(ConvertDateTimeOffset);
                            }
                            else if (type == typeof(DateTime))
                            {
                                type = typeof(string);
                                valueGetters.Add(ConvertDateTime);
                            }
                            else
                            {
                                valueGetters.Add(GetDefaultValue);
                            }

                            var column = new DataColumn(name, type);
                            column.AllowDBNull = (bool)sRow["AllowDBNull"];

                            data.Columns.Add(column);
                        }

                        while (reader.Read())
                        {
                            var row = data.NewRow();

                            for (var i = 0; i < valueGetters.Count; i++)
                                row[i] = valueGetters[i](reader[i]);

                            data.Rows.Add(row);
                        }
                    }
                }
            }

            return data;
        }

        public int SqlQueryCount(string query)
        {
            using (var sql = new SqlConnection(_connectionString))
            {
                sql.Open();

                using (var select = new SqlCommand(query, sql))
                {
                    return (int)select.ExecuteScalar();
                }
            }
        }

        private object GetDefaultValue(object value) => value;

        private object ConvertDateTimeOffset(object value) => value == DBNull.Value
            ? string.Empty
            : ((DateTimeOffset)value).Format(_timezone, _isHtml, _isHtml);

        private object ConvertDateTime(object value) => value == DBNull.Value
            ? string.Empty
            : ((DateTime)value).Format();
    }
}
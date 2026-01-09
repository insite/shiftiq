using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace InSite.Persistence
{
    public static class SqlParameterHelper
    {
        public static SqlParameter IdentifierList(string parameterName, IEnumerable<Guid> values)
        {
            var table = new DataTable();
            table.Columns.Add("IdentifierItem", typeof(Guid));

            foreach (var value in values)
            {
                var row = table.NewRow();
                row["IdentifierItem"] = value;

                table.Rows.Add(row);
            }

            var param = new SqlParameter(parameterName, SqlDbType.Structured);
            param.TypeName = "dbo.IdentifierList";
            param.Value = table;

            return param;
        }

        public static SqlParameter IntegerList(string parameterName, IEnumerable<int> values)
        {
            var table = new DataTable();
            table.Columns.Add("IntegerItem", typeof(int));

            foreach (var value in values)
            {
                var row = table.NewRow();
                row["IntegerItem"] = value;

                table.Rows.Add(row);
            }

            var param = new SqlParameter(parameterName, SqlDbType.Structured);
            param.TypeName = "dbo.IntegerList";
            param.Value = table;

            return param;
        }

        public static SqlParameter PrimaryKey(string parameterName, IEnumerable<int> values)
        {
            var table = new DataTable();
            table.Columns.Add("KeyValue", typeof(int));

            foreach (var value in values)
            {
                var row = table.NewRow();
                row["KeyValue"] = value;

                table.Rows.Add(row);
            }

            var param = new SqlParameter(parameterName, SqlDbType.Structured);
            param.TypeName = "dbo.IntegerList";
            param.Value = table;

            return param;
        }

        public static SqlParameter PrimaryKey(string parameterName, IEnumerable<Guid> values)
        {
            var table = new DataTable();
            table.Columns.Add("KeyValue", typeof(Guid));

            foreach (var value in values)
            {
                var row = table.NewRow();
                row["KeyValue"] = value;

                table.Rows.Add(row);
            }

            var param = new SqlParameter(parameterName, SqlDbType.Structured);
            param.TypeName = "dbo.IdentifierList";
            param.Value = table;

            return param;
        }
    }
}

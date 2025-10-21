using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

using InSite.Admin.Reports.Queries.Models;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Common.Events;

namespace InSite.Admin.Reports.Queries.Controls
{
    public partial class SqlSearchResults : SearchResultsGridViewController<SqlFilter>
    {
        #region Events

        public event AlertHandler Alert;

        private void OnAlert(AlertType type, string message) => Alert?.Invoke(this, new AlertArgs(type, message));

        #endregion

        #region Properties

        private Guid? QueryID
        {
            get => (Guid?)ViewState[nameof(QueryID)];
            set => ViewState[nameof(QueryID)] = value;
        }

        #endregion

        #region Fields

        private QueryData _dataSource;

        #endregion

        #region Methods (data bind)

        protected override int SelectCount(SqlFilter filter)
        {
            var data = GetQueryData(filter);

            return data == null ? 0 : data.RowsCount;
        }

        protected override IListSource SelectData(SqlFilter filter)
        {
            var data = GetQueryData(filter);
            DownloadDropDown.Visible = data.RowsCount > 0;

            return data.ToDataTable(filter.Paging);
        }

        public override void Search(SqlFilter filter, bool refreshLastSearched = false)
        {
            if (refreshLastSearched)
                QueryID = null;

            base.Search(filter, refreshLastSearched);

            DownloadDropDown.Visible = SelectCount(filter) > 0;
        }

        #endregion

        private QueryData GetQueryData(SqlFilter filter)
        {
            if (_dataSource == null || !QueryID.HasValue || _dataSource.QueryID != QueryID.Value)
            {
                _dataSource = (QueryID.HasValue ? QueryData.Load(QueryID.Value) : null);

                if (_dataSource == null)
                {
                    _dataSource = CreateQueryData(filter);

                    QueryID = _dataSource == null ? Guid.Empty : _dataSource.QueryID;
                }
            }

            return _dataSource;
        }

        private QueryData CreateQueryData(SqlFilter filter)
        {
            var query = filter?.Query;
            if (string.IsNullOrEmpty(query))
                return null;

            // TODO: Use the connection string builder to create a connection string dynamically for a read-only login to SQL Server.
            var connectionString = ServiceLocator.AppSettings.Database.ConnectionStrings.Shift;

            try
            {
                using (var sql = new SqlConnection(connectionString))
                {
                    sql.Open();

                    using (var select = new SqlCommand(query, sql))
                    {
                        using (var reader = select.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            using (var writer = QueryData.Create())
                            {
                                var valueGetters = new List<Func<object, object>>();

                                var schemaTable = reader.GetSchemaTable();
                                if (schemaTable != null)
                                {
                                    foreach (DataRow sRow in schemaTable.Rows)
                                    {
                                        var name = (string)sRow["ColumnName"];
                                        var type = (Type)sRow["DataType"];

                                        if (type == typeof(DateTimeOffset))
                                        {
                                            type = typeof(string);
                                            valueGetters.Add(ConvertDateTimeOffset);
                                        }
                                        else
                                        {
                                            valueGetters.Add(GetDefaultValue);
                                        }

                                        writer.AddColumn(name, type, (bool)sRow["AllowDBNull"]);
                                    }

                                    while (reader.Read())
                                    {
                                        var row = writer.AddRow();
                                        for (var i = 0; i < valueGetters.Count; i++)
                                            row[i] = valueGetters[i](reader[i]);
                                    }
                                }

                                return writer.QueryData;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnAlert(AlertType.Error, ex.Message.Replace("\r", string.Empty).Replace("\n", "<br/>"));
                return null;
            }
        }

        #region Methods (helpers)

        private static object GetDefaultValue(object value) => value == DBNull.Value ? null : value;

        private static object ConvertDateTimeOffset(object value) => value == DBNull.Value
            ? null
            : ((DateTimeOffset)value).Format(User.TimeZone);

        #endregion
    }
}
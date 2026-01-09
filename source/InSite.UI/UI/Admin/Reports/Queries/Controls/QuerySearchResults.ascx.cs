using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;

using Shift.Common.Events;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Reports.Queries.Controls
{
    public partial class QuerySearchResults : SearchResultsGridViewController<SqlFilter>
    {
        public event AlertHandler Alert;
        private void OnStatusUpdated(AlertType type, string message) =>
            Alert?.Invoke(this, new AlertArgs(type, message));

        public event BooleanValueHandler DataUpdated;
        private void OnDataUpdated(bool hasData) =>
            DataUpdated?.Invoke(this, new BooleanValueArgs(hasData));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            TempGrid.RowDataBound += TempGrid_RowDataBound;
        }

        private void TempGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var table = (DataTable)TempGrid.DataSource;

            if (e.Row.RowType == DataControlRowType.Header)
                FormatHeaderCells(e.Row, table);

            if (e.Row.RowType == DataControlRowType.DataRow)
                FormatValueCells(e.Row, table);
        }

        private static void FormatHeaderCells(GridViewRow view, DataTable table)
        {
            try
            {
                for (var i = 0; i < table.Columns.Count; i++)
                {
                    var column = table.Columns[i];
                    view.Cells[i].CssClass = $"table-header-{column.DataType.Name.ToLower()}";
                }
            }
            catch { }
        }

        private static void FormatValueCells(GridViewRow row, DataTable table)
        {
            try
            {
                for (var i = 0; i < table.Columns.Count; i++)
                {
                    var column = table.Columns[i];
                    var value = ((DataRowView)row.DataItem)[i];

                    var cell = row.Cells[i];
                    cell.CssClass = $"table-value-{column.DataType.Name.ToLower()}";

                    if (column.DataType == typeof(string) && value != DBNull.Value)
                        cell.Text = System.Web.HttpUtility.HtmlDecode(value as string);

                    else if (column.DataType == typeof(DateTime))
                        cell.Text = string.Format("{0:MMM d, yyyy}", value);

                    else if (column.DataType == typeof(int))
                        cell.Text = string.Format("{0:n0}", value);
                }
            }
            catch { }
        }

        protected override int SelectCount(SqlFilter filter) => throw new NotImplementedException();

        protected override IListSource SelectData(SqlFilter filter) => throw new NotImplementedException();

        protected override void Search(SqlFilter filter, int pageIndex)
        {
            Filter = filter;

            var data = ExecuteQuery(20);
            var hasData = data != null && data.Rows.Count > 0;

            OnDataUpdated(hasData);

            SetGridVisibility(hasData, false);

            if (Grid.Visible && hasData)
            {
                Grid.DataSource = data;
                Grid.DataBind();

                TempGrid.Visible = true;
                TempGrid.DataSource = data;
                TempGrid.DataBind();
            }
            else
            {
                TempGrid.Visible = false;
            }

            OnSearched();
        }

        protected override int GetRowCount()
        {
            return TempGrid.Rows.Count;
        }

        public DataTable ExecuteQuery(int? take)
        {
            var query = Filter?.Query;
            DataTable data = null;

            if (!string.IsNullOrEmpty(query))
            {
                try
                {
                    var db = new Common.Data.Database(ServiceLocator.AppSettings.Database.ConnectionStrings.Shift, User.TimeZone, false);
                    if (take.HasValue)
                        data = db.SqlQuery(query).AsEnumerable().Take(take.Value).CopyToDataTable();
                    else
                        data = db.SqlQuery(query);
                }
                catch (Exception ex)
                {
                    OnStatusUpdated(AlertType.Error, ex.Message.Replace("\r", string.Empty).Replace("\n", "<br/>"));
                }
            }

            return data;
        }
    }
}
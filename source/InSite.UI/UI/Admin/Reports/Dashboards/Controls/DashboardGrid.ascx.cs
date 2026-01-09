using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Toolbox;

namespace InSite.UI.Admin.Reports.Dashboards
{
    public partial class DashboardGrid : BaseUserControl
    {
        private const int DefaultRowsInWidgetTable = 7;

        public bool HasData
        {
            get
            {
                if (ViewState[nameof(HasData)] == null)
                {
                    return false;
                }

                return (bool)ViewState[nameof(HasData)];
            }
            set
            {
                ViewState[nameof(HasData)] = value;
            }
        }

        private DashboardQuery Query
        {
            get
            {
                return (DashboardQuery)ViewState[nameof(Query)];
            }
            set
            {
                ViewState[nameof(Query)] = value;
            }
        }

        #region Criteria

        [Serializable]
        public class Criterion
        {
            public Criterion(string type, string name, string value)
            {
                Type = type;
                Name = name;
                Value = value;
            }

            public string Type { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
        }

        private Dictionary<string, Criterion> _criteria;

        public Dictionary<string, Criterion> Criteria
        {
            get
            {
                _criteria = (Dictionary<string, Criterion>)ViewState[nameof(Criteria)];

                if (_criteria == null)
                    ViewState[nameof(Criteria)] = _criteria = new Dictionary<string, Criterion>();

                return _criteria;
            }
        }

        private void AddCriterion(string type, string name, string value)
        {
            if (_criteria.ContainsKey(name))
                _criteria.Remove(name);

            if (!string.IsNullOrWhiteSpace(value))
                _criteria.Add(name, new Criterion(type, name, value));

            ViewState[nameof(Criteria)] = _criteria;
        }

        private void ResetCriteria()
        {
            _criteria = null;
            ViewState[nameof(Criteria)] = null;
            BindModelToControls();
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            MyDownload.Click += (x, y) => DownloadCsvFile(Query.Sql, true, "Download");
            MyDownloadRaw.Click += (x, y) => DownloadCsvFile(Query.SqlRaw, false, "Download-Raw");
            MyClear.Click += (x, y) => ResetCriteria();

            MyGrid.AutoGenerateColumns = false;
            MyGrid.AllowPaging = true;
            MyGrid.AllowSorting = true;
            MyGrid.CssClass = "table table-striped";
            MyGrid.PageSize = DefaultRowsInWidgetTable;
            MyGrid.PageIndexChanging += MyGrid_PageIndexChanging;
            MyGrid.Sorting += MyGrid_Sorting;
            MyGrid.RowCreated += MyGrid_RowCreated;
            MyGrid.ShowHeaderWhenEmpty = true;
        }

        public void BindModel(DashboardQuery query)
        {
            Query = query;

            if (query.Columns == null || query.Columns.Length == 0)
            {
                MyGrid.AutoGenerateColumns = true;
            }
            else
            {
                foreach (var column in query.Columns)
                {
                    if (column.Link != null)
                    {
                        var field = new HyperLinkField
                        {
                            DataTextField = column.Name,
                            HeaderText = column.Label ?? column.Name,
                            DataNavigateUrlFields = new[] { column.Link.Value },
                            DataNavigateUrlFormatString = column.Link.Url
                        };

                        if (column.Sort != null)
                            field.SortExpression = column.Sort;

                        if (column.Type == "Integer")
                            field.ItemStyle.HorizontalAlign = HorizontalAlign.Right;

                        MyGrid.Columns.Add(field);
                    }
                    else
                    {
                        var field = new System.Web.UI.WebControls.BoundField
                        {
                            DataField = column.Name,
                            HeaderText = column.Label ?? column.Name,
                            HtmlEncode = false
                        };

                        if (column.Type == "Date")
                        {
                            field.DataFormatString = "{0:MMM d, yyyy}";
                            field.ItemStyle.Wrap = false;
                        }

                        if (column.Type == "Integer")
                        {
                            field.DataFormatString = "{0:n0}";
                            field.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                        }

                        if (column.Type == "Decimal")
                        {
                            field.DataFormatString = "{0:n2}";
                            field.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                        }

                        if (column.Type == "Percent")
                        {
                            field.DataFormatString = "{0:p0}";
                            field.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                        }

                        if (column.Type == "Email")
                        {
                            field.DataFormatString = "<a href='mailto:{0}'>{0}</a>";
                            field.ItemStyle.Wrap = false;
                        }

                        if (column.Sort != null)
                            field.SortExpression = column.Sort;

                        MyGrid.Columns.Add(field);
                    }
                }
            }

            BindModelToControls();
        }

        private void BindModelToControls()
        {
            try
            {
                MyClear.Visible = Criteria.Count > 0;
                var table = CreateDataSource(Query.Sql);

                HasData = table.Rows.Count > 0;
                MyDownload.Visible = HasData;
                MyDownloadRaw.Visible = MyDownload.Visible && Query.SqlRaw != null;

                MyCount.InnerText = "result".ToQuantity(table.Rows.Count, "N0");

                MyGrid.DataSource = table;
                MyGrid.DataBind();
                MyGrid.Visible = HasData || MyClear.Visible;
            }
            catch (Exception ex)
            {
                MyStatus.AddMessage(AlertType.Error, ex.Message);
            }
        }

        private DataTable CreateDataSource(string sql)
        {
            var sqlParameters = new List<SqlParameter>();
            foreach (var parameter in Query.Parameters)
                sqlParameters.Add(new SqlParameter(parameter.Key, parameter.Value));

            var table = DatabaseHelper.CreateDataTable(sql, sqlParameters.ToArray());

            var sort = (string)ViewState[$"{ID}Sort"];

            string where = "1=1";
            foreach (var key in Criteria.Keys)
            {
                var criterion = Criteria[key];
                if (criterion.Type == "Integer")
                    where += $" AND {criterion.Name} = {criterion.Value}";
                else
                    where += $" AND {criterion.Name} LIKE '%{criterion.Value}%'";
            }

            if (!string.IsNullOrEmpty(sort))
                table.DefaultView.Sort = sort;

            var view = table.DefaultView.ToTable();
            var selection = view.Select(where);

            DataTable finalTable;

            if (selection.Length > 0)
                finalTable = selection.CopyToDataTable();
            else
            {
                table.Clear();
                finalTable = table;
            }

            LocalizeDates(finalTable);

            return finalTable;
        }

        private static void LocalizeDates(DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn column in table.Columns)
                {
                    var value = row[column];
                    if (value is DateTimeOffset d)
                        row[column] = TimeZones.ConvertFromUtc(d, User.TimeZone);
                }
            }
        }

        private void DownloadCsvFile(string sql, bool mapColumns, string filename)
        {
            try
            {
                var table = CreateDataSource(sql);

                var helper = new XlsxExportHelper();

                if (mapColumns && Query.Columns != null)
                {
                    foreach (var column in Query.Columns)
                        helper.Map(column.Name, column.Label ?? column.Name);
                }
                else
                {
                    foreach (DataColumn column in table.Columns)
                        helper.Map(column.ColumnName, column.ColumnName);
                }

                var bytes = helper.GetXlsxBytes(table, filename);

                filename = StringHelper.Sanitize(filename, '-', false);

                Page.Response.SendFile(filename, "xlsx", bytes, null, false);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("Thread was being aborted"))
                {

                }
                else
                {
                    MyCount.InnerText = ex.Message;
                    MyCount.Attributes["class"] = "text-danger me-1";
                }
            }
        }

        private void MyGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            MyGrid.PageIndex = e.NewPageIndex;
            BindModelToControls();
        }

        private void MyGrid_Sorting(object sender, GridViewSortEventArgs e)
        {
            ViewState[$"{ID}Sort"] = e.SortExpression;
            BindModelToControls();
        }

        protected void MyGrid_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.Header)
                return;

            foreach (TableCell cell in e.Row.Cells)
            {
                if (!cell.HasControls())
                    continue;

                cell.Wrap = false;

                var link = cell.Controls[0] as LinkButton;
                if (link != null && (string)ViewState[$"{ID}Sort"] == link.CommandArgument)
                    cell.Controls.Add(new LiteralControl("<i class='fas fa-caret-up ms-2'></i>"));

                DataControlField field = ((DataControlFieldCell)cell).ContainingField;

                var bound = field as System.Web.UI.WebControls.BoundField;
                var hyperlink = field as HyperLinkField;

                if (bound == null && hyperlink == null)
                    continue;

                string name = null;
                if (bound != null)
                    name = bound.DataField;
                else if (hyperlink != null)
                    name = hyperlink.DataTextField;

                if (string.IsNullOrEmpty(name))
                    continue;

                var column = Query.FindColumn(name);

                if (column == null)
                    continue;

                var panel = new Panel { CssClass = "mt-2" };
                var input = new System.Web.UI.WebControls.TextBox { AutoPostBack = true, CssClass = "form-control" };
                input.TextChanged += Input_TextChanged;

                if (column.Type == "Date")
                    continue;

                if (column.Type == "Integer")
                {
                    field.HeaderStyle.CssClass = "text-end";
                    input.Width = Unit.Pixel(80);
                }

                if (column.Type == "Decimal")
                {
                    field.HeaderStyle.CssClass = "text-end";
                    continue;
                }

                if (column.Type == "Percent")
                {
                    field.HeaderStyle.CssClass = "text-end";
                    continue;
                }

                if (bound != null)
                {
                    input.ID = bound.DataField;
                    if (Criteria.ContainsKey(input.ID))
                        input.Text = Criteria[input.ID].Value;
                    panel.Controls.Add(input);
                }
                else if (hyperlink != null)
                {
                    input.ID = hyperlink.DataTextField;
                    if (Criteria.ContainsKey(input.ID))
                        input.Text = Criteria[input.ID].Value;
                    panel.Controls.Add(input);
                }

                cell.Controls.Add(panel);
            }
        }

        private void Input_TextChanged(object sender, EventArgs e)
        {
            var input = (System.Web.UI.WebControls.TextBox)sender;
            var column = Query.Columns.Single(x => x.Name == input.ID);
            AddCriterion(column.Type, input.ID, input.Text);
            BindModelToControls();
        }
    }
}
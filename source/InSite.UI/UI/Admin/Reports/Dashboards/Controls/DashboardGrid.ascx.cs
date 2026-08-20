using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Admin.Reports.Dashboards.Utilities;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Toolbox;

using AspLiteral = System.Web.UI.WebControls.Literal;
using AspTemplateField = System.Web.UI.WebControls.TemplateField;

namespace InSite.UI.Admin.Reports.Dashboards
{
    public partial class DashboardGrid : BaseUserControl
    {
        private const int DefaultRowsInWidgetTable = 7;

        public bool HasData
        {
            get => (bool?)ViewState[nameof(HasData)] == true;
            set => ViewState[nameof(HasData)] = value;
        }

        private DashboardTableQuery Query
        {
            get => (DashboardTableQuery)ViewState[nameof(Query)];
            set => ViewState[nameof(Query)] = value;
        }

        private Dictionary<string, string> WidgetQueryParameters
        {
            get => (Dictionary<string, string>)ViewState[nameof(WidgetQueryParameters)];
            set => ViewState[nameof(WidgetQueryParameters)] = value;
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

        #region Columns

        private class CheckBoxColumnTemplate : ITemplate
        {
            private readonly string _columnName;

            public CheckBoxColumnTemplate(string columnName) => _columnName = columnName;

            public void InstantiateIn(Control container)
            {
                var checkbox = new InSite.Common.Web.UI.CheckBox
                {
                    RenderMode = CheckBoxRenderMode.Input,
                    CssClass = "pe-none"
                };

                checkbox.DataBinding += (sender, e) =>
                {
                    var box = (InSite.Common.Web.UI.CheckBox)sender;
                    var row = (GridViewRow)box.NamingContainer;
                    box.Checked = DataBinder.Eval(row.DataItem, _columnName) as bool? == true;
                };

                container.Controls.Add(checkbox);
            }
        }

        private class EmailColumnTemplate : ITemplate
        {
            private readonly string _columnName;

            public EmailColumnTemplate(string columnName) => _columnName = columnName;

            public void InstantiateIn(Control container)
            {
                var literal = new LiteralControl();

                literal.DataBinding += Literal_DataBinding;

                container.Controls.Add(literal);
            }

            private void Literal_DataBinding(object sender, EventArgs e)
            {
                var lit = (LiteralControl)sender;
                var row = (GridViewRow)lit.NamingContainer;
                var email = DataBinder.Eval(row.DataItem, _columnName) as string;
                var encoded = System.Web.HttpUtility.HtmlEncode(email);
                lit.Text = email.IsEmpty() ? string.Empty : $"<a href='mailto:{encoded}'>{encoded}</a>";
            }
        }

        private class MarkdownColumnTemplate : ITemplate
        {
            private readonly string _columnName;

            public MarkdownColumnTemplate(string columnName) => _columnName = columnName;

            public void InstantiateIn(Control container)
            {
                var literal = new LiteralControl();

                literal.DataBinding += Literal_DataBinding;

                container.Controls.Add(literal);
            }

            private void Literal_DataBinding(object sender, EventArgs e)
            {
                var lit = (LiteralControl)sender;
                var row = (GridViewRow)lit.NamingContainer;
                var value = DataBinder.Eval(row.DataItem, _columnName) as string;

                lit.Text = Markdown.ToHtml(value);
            }
        }

        private static class ColumnType
        {
            public static readonly HashSet<string> RightAligned =
                new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Integer", "Decimal", "Percent" };

            public static readonly HashSet<string> NoFilter =
                new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Date", "Decimal", "Percent" };

            public static readonly Dictionary<string, ColumnFormatInfo> Format =
                new Dictionary<string, ColumnFormatInfo>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Date"] = new ColumnFormatInfo("{0:MMM d, yyyy}", wrap: false),
                    ["Integer"] = new ColumnFormatInfo("{0:n0}"),
                    ["Decimal"] = new ColumnFormatInfo("{0:n2}"),
                    ["Percent"] = new ColumnFormatInfo("{0:p0}"),
                };
        }

        private class ColumnFormatInfo
        {
            public string FormatString { get; }
            public bool Wrap { get; }

            public ColumnFormatInfo(string formatString, bool wrap = true)
            {
                FormatString = formatString;
                Wrap = wrap;
            }
        }

        private void CreateColumns()
        {
            MyGrid.Columns.Clear();

            var query = Query;
            if (query.Columns == null || query.Columns.Length == 0)
            {
                MyGrid.AutoGenerateColumns = true;
                return;
            }

            MyGrid.AutoGenerateColumns = false;

            foreach (var column in query.Columns)
            {
                var field = CreateField(column);
                ApplySortExpression(field, column);
                ApplyAlignment(field, column);
                MyGrid.Columns.Add(field);
            }
        }

        private static DataControlField CreateField(DashboardTableQueryColumn column)
        {
            if (string.Equals(column.Type, "Checkbox", StringComparison.OrdinalIgnoreCase))
                return CreateCheckboxField(column);

            if (string.Equals(column.Type, "Email", StringComparison.OrdinalIgnoreCase))
                return CreateEmailField(column);

            if (column.Link != null)
                return CreateHyperLinkField(column);

            if (string.Equals(column.Type, "Markdown", StringComparison.OrdinalIgnoreCase))
                return CreateMarkdownField(column);

            return CreateBoundField(column);
        }

        private static AspTemplateField CreateCheckboxField(DashboardTableQueryColumn column)
        {
            var field = new AspTemplateField();
            field.HeaderText = column.Label ?? column.Name;
            field.ItemTemplate = new CheckBoxColumnTemplate(column.Name);
            field.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
            return field;
        }

        private static AspTemplateField CreateEmailField(DashboardTableQueryColumn column)
        {
            var field = new AspTemplateField();
            field.HeaderText = column.Label ?? column.Name;
            field.ItemTemplate = new EmailColumnTemplate(column.Name);
            field.ItemStyle.Wrap = false;
            return field;
        }

        private static AspTemplateField CreateMarkdownField(DashboardTableQueryColumn column)
        {
            var field = new AspTemplateField();
            field.HeaderText = column.Label ?? column.Name;
            field.ItemTemplate = new MarkdownColumnTemplate(column.Name);
            return field;
        }

        private static HyperLinkField CreateHyperLinkField(DashboardTableQueryColumn column)
        {
            var field = new HyperLinkField();
            field.DataTextField = column.Name;
            field.HeaderText = column.Label ?? column.Name;
            field.DataNavigateUrlFields = new[] { column.Link.Value };
            field.DataNavigateUrlFormatString = column.Link.Url;

            if (ColumnType.Format.TryGetValue(column.Type.EmptyIfNull(), out var format))
                field.ItemStyle.Wrap = format.Wrap;

            return field;
        }

        private static System.Web.UI.WebControls.BoundField CreateBoundField(DashboardTableQueryColumn column)
        {
            var field = new System.Web.UI.WebControls.BoundField();
            field.DataField = column.Name;
            field.HeaderText = column.Label ?? column.Name;
            field.HtmlEncode = !string.Equals(column.Type, "Html", StringComparison.OrdinalIgnoreCase);

            if (ColumnType.Format.TryGetValue(column.Type.EmptyIfNull(), out var format))
            {
                field.DataFormatString = format.FormatString;
                field.ItemStyle.Wrap = format.Wrap;
            }

            return field;
        }

        private static void ApplySortExpression(DataControlField field, DashboardTableQueryColumn column)
        {
            if (column.Sort != null)
                field.SortExpression = column.Sort;
        }

        private static void ApplyAlignment(DataControlField field, DashboardTableQueryColumn column)
        {
            if (ColumnType.RightAligned.Contains(column.Type.EmptyIfNull()))
                field.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
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

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack && Query != null)
                CreateColumns();
        }

        public void BindModel(DashboardWidget widget)
        {
            Query = widget.Query;
            WidgetQueryParameters = widget.QueryParameters;

            CreateColumns();
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
            var sqlParameters = DashboardBuilder.BuildQueryParameters(WidgetQueryParameters, Query.Parameters);
            var table = DatabaseHelper.CreateDataTable(sql, sqlParameters);
            var sort = (string)ViewState[$"{ID}Sort"];

            var where = "1=1";
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

                var name = GetFieldDataName(field);
                if (name.IsEmpty())
                    continue;

                var column = Query.FindColumn(name);
                if (column == null)
                    continue;

                if (ColumnType.RightAligned.Contains(column.Type.EmptyIfNull()))
                    field.HeaderStyle.CssClass = "text-end";

                if (ColumnType.NoFilter.Contains(column.Type.EmptyIfNull()))
                    continue;

                var panel = new Panel { CssClass = "mt-2" };
                var input = new System.Web.UI.WebControls.TextBox
                {
                    ID = name,
                    AutoPostBack = true,
                    CssClass = "form-control"
                };
                input.TextChanged += Input_TextChanged;

                if (string.Equals(column.Type, "Integer", StringComparison.OrdinalIgnoreCase))
                    input.Width = Unit.Pixel(80);

                if (Criteria.ContainsKey(name))
                    input.Text = Criteria[name].Value;

                panel.Controls.Add(input);
                cell.Controls.Add(panel);
            }
        }

        private static string GetFieldDataName(DataControlField field)
        {
            if (field is System.Web.UI.WebControls.BoundField bound)
                return bound.DataField;

            if (field is HyperLinkField hyperlink)
                return hyperlink.DataTextField;

            return null;
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

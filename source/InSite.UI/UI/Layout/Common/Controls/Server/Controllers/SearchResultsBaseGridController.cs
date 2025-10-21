using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Domain.Reports;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Toolbox;

using TReportSearch = InSite.Persistence.TReportSearch;
using TReportStore = InSite.Persistence.TReportStore;

namespace InSite.Common.Web.UI
{
    public abstract class SearchResultsBaseGridController<TFilter> : BaseSearchResultsController<TFilter> where TFilter : Filter
    {
        #region Classes

        protected interface IColumn
        {
            string Title { get; }
            string SortExpression { get; set; }
            bool Visible { get; set; }
        }

        protected enum UpdateType
        {
            None,
            UpdateGrid,
            UpdatePage
        }

        protected interface IControlLinker
        {
            ITextControl Instructions { get; }
            DownloadButton DownloadButton { get; }
            DropDownButton DownloadDropDown { get; }
        }

        #endregion

        #region Fields

        private DateTimeOffset? _lastSearched;

        #endregion

        #region Properties

        protected virtual bool IsFinder => true;

        protected virtual int DefaultPageSize => 20;

        public bool HasRows => RowCount > 0;

        public override int RowCount => GetRowCount();

        public int PageSize
        {
            get => (int)(ViewState[nameof(PageSize)] ?? DefaultPageSize);
            set => ViewState[nameof(PageSize)] = Number.CheckRange(value, DefaultPageSize);
        }

        protected string[] ColumnNames => (string[])(ViewState[nameof(ColumnNames)]
            ?? (ViewState[nameof(ColumnNames)] = GetColumnNames()));

        protected bool RefreshLastSearched { get; set; }

        public override DateTimeOffset? LastSearched { get => _lastSearched; }

        protected SearchSort SortExpression
        {
            get { return (SearchSort)ViewState[nameof(SortExpression)]; }
            set { ViewState[nameof(SortExpression)] = value; }
        }

        protected virtual string ExportFileName { get => null; }

        protected UpdateType Update { get; set; }

        protected abstract int GridPageIndex { get; set; }

        protected abstract int GridTotalItemCount { get; set; }

        protected abstract bool GridPagerVisible { get; set; }

        public abstract object GridDataSource { get; set; }

        protected abstract bool GridVisible { get; set; }

        protected abstract IControlLinker Linker { get; }

        #endregion

        #region PreRender and Loading

        protected override void OnInit(EventArgs e)
        {
            if (Linker.DownloadDropDown != null)
                Linker.DownloadDropDown.Click += DownloadButton_Click;

            if (!IsPostBack)
                SetGridVisibility(false, true);

            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Linker.DownloadDropDown != null)
                    SetupExportButton(Linker.DownloadDropDown);
            }

            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (Update != UpdateType.None)
            {
                if (Update == UpdateType.UpdatePage)
                    SearchWithCurrentPageIndex(Filter);
                else if (Update == UpdateType.UpdateGrid)
                    Search(Filter);

                SaveSearch();
            }
        }

        protected abstract int GetRowCount();

        private string[] GetColumnNames() =>
            GetColumns().Where(x => x.Title.IsNotEmpty()).Select(x => x.Title).ToArray();

        protected abstract IEnumerable<IColumn> GetColumns();

        #endregion

        #region Event handlers

        private void DownloadButton_Click(object sender, CommandEventArgs e) =>
            OnExportButtonClick(e.CommandName);

        protected virtual void OnExportButtonClick(string commandName)
        {
            if (commandName == "default" || commandName == "XLSX")
                ExportXlsx();
            else if (commandName == "CSV")
                ExportCsv();
        }

        #endregion

        #region Searching

        protected abstract int SelectCount(TFilter filter);

        protected abstract IListSource SelectData(TFilter filter);

        public override void Search(TFilter filter, bool refreshLastSearched = false)
        {
            RefreshLastSearched = refreshLastSearched;
            _lastSearched = DateTimeOffset.Now;

            Search(filter, 0);
        }

        public virtual void SearchWithCurrentPageIndex(TFilter filter)
        {
            Search(filter, GridPageIndex);
        }

        protected virtual void Search(TFilter filter, int pageIndex)
        {
            var count = SelectCount(filter);

            var maxPageIndex = (int)Math.Ceiling((double)count / PageSize) - 1;
            if (maxPageIndex < 0)
                maxPageIndex = 0;

            GridTotalItemCount = count;
            GridPageIndex = pageIndex > maxPageIndex ? maxPageIndex : pageIndex;

            var hasData = count > 0;

            Filter = filter;

            SetGridVisibility(hasData, false);

            GridPagerVisible = count > 2; // count > PageSize;

            if (!hasData)
            {
                GridDataSource = new DataTable();
            }
            else
            {
                BindGrid();
            }

            if ((!IsFinder || Visible) && hasData)
                GridDataBind();

            OnSearched();
        }

        protected void BindGrid()
        {
            if (Filter == null && IsFinder)
                RaiseBubbleEvent("bind_result_filter", EventArgs.Empty);

            ShowColumns();

            var page = GridPageIndex + 1;
            var prevPaging = Filter?.Paging;

            if (Filter != null)
                Filter.Paging = Paging.SetPage(page, PageSize);

            var dataSource = SelectData(Filter);

            if (Filter != null)
                Filter.Paging = prevPaging;

            GridDataSource = dataSource;

            Update = UpdateType.None;
        }

        public override void Clear(TFilter filter)
        {
            Filter = filter;

            SetGridVisibility(false, true);

            GridDataSource = null;
            GridDataBind();
        }

        public virtual void RefreshGrid()
        {
            if (Filter != null)
                Search(Filter);
        }

        private void ShowColumns()
        {
            if (!IsFinder || Filter == null)
                return;

            var columns = Filter.ShowColumns;
            var columnIndex = 0;

            foreach (var column in GetColumns())
            {
                if (string.IsNullOrEmpty(column.Title))
                    continue;

                var columnName = ColumnNames[columnIndex++];

                column.Visible = columns.Count == 0
                    || columns.FirstOrDefault(c => StringHelper.Equals(columnName, c)) != null;
            }
        }

        protected abstract void GridDataBind();

        #endregion

        #region Save & Load searches

        public virtual void DeleteSearch()
        {
            if (Route == null)
                return;

            var reports = TReportSearch.Select(
                x => x.OrganizationIdentifier == Organization.OrganizationIdentifier
                  && x.UserIdentifier == User.UserIdentifier
                  && x.ReportType == ReportType.Search
                  && x.ReportDescription == Route.Name);

            foreach (var r in reports)
            {
                var request = ReportRequest.Deserialize(r.ReportData);
                if (request != null)
                {
                    request.RemoveSearch();

                    r.ReportData = request.Serialize();
                    r.Modified = DateTimeOffset.Now;
                    r.ModifiedBy = User.UserIdentifier;

                    TReportStore.Update(r);
                }
                else
                    TReportStore.Delete(r.ReportIdentifier);
            }
        }

        public override void SaveSearch(TFilter filter, bool refreshLastSearched)
        {
            if (!IsFinder)
                return;

            OnSavingSearch();

            SaveSearchSettings(filter, GridPageIndex, SortExpression, refreshLastSearched);
        }

        protected virtual void SaveSearchSettings(TFilter filter, int pageIndex, SearchSort sortExpression, bool refreshLastSearched)
        {
            if (Route == null)
                return;

            var isNew = false;
            var lastSearched = _lastSearched;

            var report = TReportSearch.SelectFirst(
                x => x.OrganizationIdentifier == Organization.OrganizationIdentifier
                  && x.UserIdentifier == User.UserIdentifier
                  && x.ReportType == ReportType.Search
                  && x.ReportDescription == Route.Name);

            if (report == null)
            {
                isNew = true;
                report = new Persistence.TReport
                {
                    ReportIdentifier = UniqueIdentifier.Create(),
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    UserIdentifier = User.UserIdentifier,
                    ReportType = ReportType.Search,
                    ReportTitle = StringHelper.FirstValue(Route.Title, Route.Name),
                    ReportDescription = Route.Name,
                    Created = DateTimeOffset.Now,
                    CreatedBy = User.UserIdentifier,
                };
            }

            var request = ReportRequest.Deserialize(report.ReportData) ?? new ReportRequest();

            if (!refreshLastSearched)
            {
                var search = request.GetSearch<TFilter>();
                if (search != null && search.LastSearched.HasValue)
                    lastSearched = search.LastSearched.Value;
            }

            request.SetSearch(new Search<TFilter>
            {
                Filter = filter,
                PageIndex = pageIndex,
                Sort = sortExpression,
                LastSearched = lastSearched
            });

            report.ReportData = request.Serialize();
            report.Modified = DateTimeOffset.Now;
            report.ModifiedBy = User.UserIdentifier;

            if (isNew)
                TReportStore.Insert(report);
            else
                TReportStore.Update(report);
        }

        public override bool LoadSearch()
        {
            if (!IsFinder)
                return false;

            var settings = LoadSearchSettings();
            if (settings == null)
                return false;

            //Make sure there are no spaces between fields in SortExpression for columns
            foreach (var column in GetColumns())
            {
                if (string.IsNullOrEmpty(column.SortExpression))
                    continue;

                var parts = column.SortExpression.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var sortExpression = new StringBuilder();

                foreach (var part in parts)
                {
                    if (sortExpression.Length > 0)
                        sortExpression.Append(",");

                    sortExpression.Append(part.Trim());
                }

                column.SortExpression = sortExpression.ToString();
            }

            GridPageIndex = settings.PageIndex;
            SortExpression = settings.Sort;
            Filter = settings.Filter;
            _lastSearched = settings.LastSearched;

            return true;
        }

        protected virtual Search<TFilter> LoadSearchSettings()
        {
            if (Route == null)
                return null;

            if (Guid.TryParse(Request.QueryString["filter"], out var reportId))
            {
                var filterReport = TReportSearch.SelectFirst(
                    x => x.OrganizationIdentifier == Organization.OrganizationIdentifier
                      && x.UserIdentifier == User.UserIdentifier
                      && x.ReportIdentifier == reportId
                      && x.ReportType == ReportType.Filter
                      && x.ReportDescription == Route.Name);

                if (filterReport != null)
                {
                    try
                    {
                        var filter = JsonConvert.DeserializeObject<Filter>(filterReport.ReportData, new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Auto
                        }) as TFilter;

                        if (filter != null)
                            return new Search<TFilter> { Filter = filter };
                    }
                    catch
                    {

                    }
                }
            }

            var report = TReportSearch.SelectFirst(
                x => x.OrganizationIdentifier == Organization.OrganizationIdentifier
                  && x.UserIdentifier == User.UserIdentifier
                  && x.ReportType == ReportType.Search
                  && x.ReportDescription == Route.Name);

            return ReportRequest.Deserialize(report?.ReportData)?.GetSearch<TFilter>();
        }

        #endregion

        #region Helpers

        public IEnumerable<string> GetColumnsSelectorDataSource()
        {
            var columnIndex = 0;

            var columnNames = new List<string>();

            foreach (var column in GetColumns())
            {
                if (string.IsNullOrEmpty(column.Title))
                    continue;

                columnNames.Add(ColumnNames[columnIndex]);

                columnIndex++;
            }

            return columnNames.ToArray();
        }

        public void CallUpdateDateTimeOffsets(IList list, string timeZoneId)
        {
            Type itemType = list.GetType().GetGenericArguments().FirstOrDefault();
            if (itemType == null)
                return;

            MethodInfo methodInfo = typeof(SearchResultsBaseGridController<TFilter>).GetMethod(
                nameof(UpdateDateTimeOffsets),
                BindingFlags.NonPublic | BindingFlags.Instance
            );

            MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(itemType);

            genericMethodInfo.Invoke(this, new object[] { list, timeZoneId });
        }

        protected virtual void UpdateDateTimeOffsets<T>(
            IEnumerable<T> objects,
            string timeZoneId)
            where T : class
        {
            TimeZoneInfo userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

            foreach (var obj in objects)
            {
                var properties = obj.GetType().GetProperties()
                    .Where(p => p.PropertyType == typeof(DateTimeOffset?) || p.PropertyType == typeof(DateTimeOffset));

                foreach (var property in properties)
                {
                    if (property.PropertyType == typeof(DateTimeOffset?))
                    {
                        var value = (DateTimeOffset?)property.GetValue(obj);
                        if (value.HasValue)
                            UpdateDateTimeOffsetProperty(userTimeZone, obj, property, value);
                    }
                    else if (property.PropertyType == typeof(DateTimeOffset))
                    {
                        var value = (DateTimeOffset)property.GetValue(obj);
                        UpdateDateTimeOffsetProperty(userTimeZone, obj, property, value);
                    }
                }
            }
        }

        private static void UpdateDateTimeOffsetProperty<T>(TimeZoneInfo userTimeZone, T obj, System.Reflection.PropertyInfo property, DateTimeOffset? value) where T : class
        {
            var utcDateTime = value.Value.UtcDateTime;
            var newDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, userTimeZone);
            property.SetValue(obj, new DateTimeOffset(newDateTime, userTimeZone.BaseUtcOffset));
        }

        protected virtual void SetupExportButton(DropDownButton button)
        {
            if (button.ButtonStyle == ButtonStyle.Default)
                button.ButtonStyle = ButtonStyle.Default;

            button.DefaultAction = DefaultButtonAction.PostBack;
            button.IconName = "download";
            button.Text = "Download";

            button.Items.Clear();

            button.Items.Add(new DropDownButtonItem
            {
                Name = "XLSX",
                IconName = "file-excel",
                Text = "Excel (*.xlsx)"
            });

            button.Items.Add(new DropDownButtonItem
            {
                Name = "CSV",
                IconName = "file-csv",
                Text = "Text (*.csv)"
            });
        }

        protected virtual void SetGridVisibility(bool isVisible, bool showInstructions)
        {
            GridVisible = isVisible;

            if (Linker.DownloadButton != null)
                Linker.DownloadButton.Visible = isVisible;

            OnDataStateChanged(isVisible);

            if (isVisible)
            {
                SetInstructionText(string.Empty);
                return;
            }

            if (showInstructions)
                SetInstructionText(InstructionText);
            else
                SetInstructionText(Translate("There are no records matching your search criteria."));
        }

        protected virtual void SetInstructionText(string text)
        {
            if (Linker.Instructions == null)
                return;

            var control = (Control)Linker.Instructions;
            control.Visible = !string.IsNullOrEmpty(text);
            control.EnableViewState = false;

            if (!control.Visible)
                return;

            Linker.Instructions.Text = Global.Translate(text);
        }

        #endregion

        #region Methods (export)

        public override IListSource GetExportData(TFilter filter, bool empty)
        {
            filter.IsSchemaOnly = empty;

            return SelectData(filter);
        }

        private void ExportXlsx()
        {
            var bytes = GetXlsxBytes();
            if (bytes == null)
                return;

            var filename = GetExportFileName("result");
            filename = string.Format("{0}-{1:yyyyMMdd}-{1:HHmmss}", StringHelper.Sanitize(filename, '-'), DateTime.UtcNow);

            Page.Response.SendFile(filename, "xlsx", bytes);
        }

        protected virtual byte[] GetXlsxBytes()
        {
            var list = GetExportData().GetList();
            if (list.Count == 0)
                return null;

            CallUpdateDateTimeOffsets(list, User.TimeZoneId);

            var columns = GetDownloadColumns(list);
            if (columns == null)
                return null;

            var sheetName = GetExportFileName("Workflow");
            var helper = new XlsxExportHelper();

            foreach (var column in columns)
                helper.Map(column.Name, Translate(column.Title), column.Format, column.Width, column.Align);

            return helper.GetXlsxBytes(list, sheetName);
        }

        private void ExportCsv()
        {
            var bytes = GetCsvBytes();
            if (bytes.IsEmpty())
                return;

            var filename = GetExportFileName("result");
            filename = string.Format("{0}-{1:yyyyMMdd}-{1:HHmmss}", StringHelper.Sanitize(filename, '-'), DateTime.UtcNow);

            Page.Response.SendFile(filename, "csv", bytes);
        }


        protected virtual byte[] GetCsvBytes()
        {
            const string separator = ",";

            var list = GetExportData().GetList();
            if (list.Count == 0)
                return null;

            CallUpdateDateTimeOffsets(list, User.TimeZoneId);

            var columns = GetDownloadColumns(list);
            if (columns == null)
                return null;

            var properties = TypeDescriptor.GetProperties(list[0]);
            var csv = new StringBuilder();

            foreach (var column in columns)
            {
                if (csv.Length > 0)
                    csv.Append(separator);

                csv.AppendFormat("\"{0}\"", Translate(column.Title));
            }

            foreach (var item in list)
            {
                csv.Append("\r\n");

                var isFirstColumn = true;

                foreach (var column in columns)
                {
                    if (isFirstColumn)
                        isFirstColumn = false;
                    else
                        csv.Append(separator);

                    var property = properties[column.Name];
                    var value = GetStringCsvValue(property.GetValue(item));
                    csv.AppendFormat("\"{0}\"", value);
                }
            }

            return Encoding.UTF8.GetBytes(csv.ToString());

            string GetStringCsvValue(object value)
            {
                return ValueConverter.IsNotNull(value)
                    ? value.ToString().Replace("\"", "\"\"").Replace("\r\n", " ")
                    : null;
            }
        }

        protected virtual IEnumerable<DownloadColumn> GetDownloadColumns(IList dataList)
        {
            var properties = TypeDescriptor.GetProperties(dataList[0]);
            if (properties.Count == 0)
                return null;

            var result = new List<DownloadColumn>();

            foreach (PropertyDescriptor p in properties)
                result.Add(new DownloadColumn(p.Name));

            return result;
        }

        protected virtual string GetExportFileName(string defaultName)
        {
            string name = null;

            if (string.IsNullOrEmpty(ExportFileName))
            {
                var parent = NamingContainer;

                while (parent != null)
                {
                    parent = parent.NamingContainer;
                }
            }
            else
            {
                name = ExportFileName;
            }

            if (string.IsNullOrEmpty(name))
                name = defaultName;

            return name.Pluralize();
        }

        #endregion
    }
}
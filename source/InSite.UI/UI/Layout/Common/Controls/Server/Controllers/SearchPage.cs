using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Domain.Reports;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Common.Controls;
using InSite.UI.Layout.Portal;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    public abstract class SearchPage<TFilter> : PortalBasePage, IFinder where TFilter : Filter
    {
        #region Classes

        protected sealed class ControlLinker
        {
            #region Classes

            private class Accessor : Shift.Sdk.UI.ControlLinkerCache.Accessor
            {
                #region Constants

                public static readonly Accessor Empty = new Accessor();

                #endregion

                #region Fields

                public Func<object, InSite.Common.Web.UI.Alert> GetScreenStatus { get; }
                public Func<object, NavItem> GetCriteriaTab { get; }
                public Func<object, NavItem> GetResultsTab { get; }
                public Func<object, NavItem> GetDownloadTab { get; }
                public Func<object, SearchCriteriaController<TFilter>> GetCriteria { get; }
                public Func<object, BaseSearchResultsController<TFilter>> GetResults { get; }
                public Func<object, BaseSearchDownload> GetDownload { get; }

                #endregion

                #region Construction

                private Accessor() : base(null)
                {
                    GetScreenStatus = Default<InSite.Common.Web.UI.Alert>();
                    GetCriteriaTab = Default<NavItem>();
                    GetResultsTab = Default<NavItem>();
                    GetDownloadTab = Default<NavItem>();
                    GetCriteria = Default<SearchCriteriaController<TFilter>>();
                    GetResults = Default<BaseSearchResultsController<TFilter>>();
                    GetDownload = Default<BaseSearchDownload>();
                }

                public Accessor(Type t) : base(t)
                {
                    GetScreenStatus = Find<InSite.Common.Web.UI.Alert>(t, "ScreenStatus");
                    GetCriteriaTab = Find<NavItem>(t, "SearchCriteriaTab");
                    GetResultsTab = Find<NavItem>(t, "SearchResultsTab");
                    GetDownloadTab = Find<NavItem>(t, "DownloadsTab");
                    GetCriteria = Find<SearchCriteriaController<TFilter>>(t, "SearchCriteria");
                    GetResults = Find<BaseSearchResultsController<TFilter>>(t, "SearchResults");
                    GetDownload = Find<BaseSearchDownload>(t, "SearchDownload");
                }

                #endregion
            }

            #endregion

            #region Properties

            public InSite.Common.Web.UI.Alert ScreenStatus => _data.GetScreenStatus(_container);
            public NavItem CriteriaTab => _data.GetCriteriaTab(_container);
            public NavItem ResultsTab => _data.GetResultsTab(_container);
            public NavItem DownloadTab => _data.GetDownloadTab(_container);
            public SearchCriteriaController<TFilter> Criteria => _data.GetCriteria(_container);
            public BaseSearchResultsController<TFilter> Results => _data.GetResults(_container);
            public BaseSearchDownload Download => _data.GetDownload(_container);

            #endregion

            #region Fields

            private Accessor _data;
            private object _container;

            #endregion

            #region Construction

            public ControlLinker(object container)
            {
                _data = (Accessor)ControlLinkerCache.Get(container) ?? Accessor.Empty;
                _container = container;
            }

            #endregion
        }

        #endregion

        #region Properties

        protected const string CsvSeparator = ",";

        ISearchResults IFinder.Results => _linker.Results;

        public virtual string EntityName => null;

        protected virtual string SearchResultTitle => "Results";

        protected ControlLinker Linker => _linker;

        protected virtual bool IsClearCriteria => Request.QueryString["clearcriteria"] == "1";

        #endregion

        #region Fields

        private ControlLinker _linker;

        private BaseReportManager _filterManager;

        #endregion

        #region Construction

        public SearchPage()
        {
            _linker = new ControlLinker(this);
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (_linker.Criteria != null)
            {
                _linker.Criteria.Searching += Criteria_Searching;
                _linker.Criteria.Clearing += Criteria_Clearing;

                _filterManager = _linker.Criteria.GetFilterManager();
                if (_filterManager != null)
                {
                    _filterManager.NeedReport += FilterManager_NeedReport;
                    _filterManager.ReportSelected += FilterManager_ReportSelected;
                }
            }

            if (_linker.Results != null)
            {
                _linker.Results.RowCommand += GridControl_RowCommand;
                _linker.Results.DataStateChanged += Results_DataStateChanged;
                _linker.Results.Searched += Results_Searched;
            }

            if (_linker.Download != null)
            {
                _linker.Download.Alert += Download_Alert;
                _linker.Download.NeedData += Download_NeedData;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                SetDownloadsVisiblity(false);

                if (_linker.Results != null && _linker.Criteria != null)
                {
                    _linker.Criteria.LoadShowColumns(_linker.Results);

                    if (_linker.Download != null)
                        SetupExportColumns(null);
                }

                if (Page.Master is AdminHome a)
                    a.RenderHelpContent(ActionModel);
            }

            if (_linker.Download != null)
                _linker.Download.Finder = this;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            var body = (HtmlGenericControl)ControlHelper.GetControl(Page.Master, "HtmlBody", this);
            body.Attributes["class"] += " finder";

            if (IsPostBack)
                return;

            var filter = _linker.Results.Filter;
            var isLoaded = !IsClearCriteria && LoadSavedFilter();

            if (_linker.Criteria != null)
            {
                var isFilterClean = isLoaded && filter != null && filter.IsFilterClean;

                if (!isLoaded || isFilterClean || _linker.Results.Filter == null)
                {
                    if (isLoaded && filter != null)
                        _linker.Criteria.SetCheckedShowColumns(filter);
                    else
                        _linker.Criteria.SetDefaultShowColumns();

                    _linker.Criteria.Clear();

                    if (!_linker.Results.HasData)
                        LoadSearchedResults();
                }
                else
                {
                    LoadSearchedResults();
                }
            }

            if (_filterManager != null && isLoaded && _linker.Results.Filter.FilterIdentifier.HasValue)
                _filterManager.SetSelectedReport(_linker.Results.Filter.FilterIdentifier.Value);
        }

        #endregion

        #region Binding Models to Controls (control setters)

        protected virtual bool LoadSavedFilter()
        {
            return _linker.Results.LoadSearch();
        }

        protected virtual void LoadSearchedResults()
        {
            var resultFilter = _linker.Results.Filter;

            _linker.Criteria.SetCheckedShowColumns(resultFilter);

            if (resultFilter != null)
                _linker.Criteria.Filter = resultFilter;
            else
                _linker.Criteria.Clear();

            var criteriaFilter = _linker.Criteria.Filter;

            if (resultFilter != null && resultFilter.FilterIdentifier.HasValue)
            {
                criteriaFilter.FilterIdentifier = resultFilter.FilterIdentifier;
                criteriaFilter.FilterName = resultFilter.FilterName;
            }

            if (_linker.Results is SearchResultsBaseGridController<TFilter> gridController)
                gridController.SearchWithCurrentPageIndex(criteriaFilter);
            else
                _linker.Results.Search(criteriaFilter);
        }

        protected void SetupMainAccordionIndex(bool hasResults)
        {
            if (_linker.CriteriaTab != null && _linker.ResultsTab != null)
            {
                if (hasResults)
                    _linker.ResultsTab.IsSelected = true;
                else
                    _linker.CriteriaTab.IsSelected = true;
            }
        }

        private void SetTitle()
        {
            if (_linker.ResultsTab != null)
            {
                var title = Translate(SearchResultTitle);

                if (_linker.Results.HasData)
                    title += $" <span class='badge rounded-pill bg-info ms-1'>{_linker.Results.RowCount:n0}</span>";

                _linker.ResultsTab.IsTitleLocalizable = false;
                _linker.ResultsTab.Title = title;
            }
        }

        protected virtual void SetDownloadsVisiblity(bool visible)
        {
            if (_linker.DownloadTab != null)
                _linker.DownloadTab.Visible = visible;
        }

        #endregion

        #region UI Event Handling

        private void Results_Searched(object sender, EventArgs e)
        {
            if (!IsPostBack)
                SetupMainAccordionIndex(Request["panel"] == "results" || _linker.Results.HasData);

            SetTitle();

            if (_linker.Download != null)
            {
                var dataList = _linker.Results.Visible && _linker.Results is SearchResultsBaseGridController<TFilter> gridController
                    ? (gridController.GridDataSource as IListSource)?.GetList()
                    : null;

                var hasData = dataList.IsNotEmpty();

                SetDownloadsVisiblity(hasData);

                if (hasData)
                    _linker.Download.RowCount = _linker.Results.HasData ? _linker.Results.RowCount : (int?)null;
            }
        }

        private void Results_DataStateChanged(object sender, BooleanValueArgs args)
        {
            SetDownloadsVisiblity(false);
        }

        private void GridControl_RowCommand(object sender, SearchResultsRowCommandArgs e)
        {
            if (e.Canceled)
                return;

            var isHandled = false;

            if (e.CommandName == "DeleteRecord")
            {
                if (_linker.Results is SearchResultsGridViewController<TFilter> gridViewResults)
                {
                    var dataKeys = gridViewResults.GetDataKeys(e.Row);
                    if (dataKeys != null && DeleteRecords(new[] { dataKeys }))
                        gridViewResults.SearchWithCurrentPageIndex(gridViewResults.Filter);
                }
            }

            if (!isHandled)
                OnGridRowCommand(e);
        }

        protected virtual void OnGridRowCommand(SearchResultsRowCommandArgs e)
        {

        }

        private void Criteria_Searching(object sender, EventArgs e)
        {
            OnSearching(_linker.Criteria.Filter);
        }

        protected virtual void OnSearching(TFilter filter)
        {
            if (filter != null)
                filter.IsFilterClean = false;

            _linker.Results.Search(filter, true);

            SetupMainAccordionIndex(IsPostBack || _linker.Results.HasData);
        }

        private void Criteria_Clearing(object sender, EventArgs e)
        {
            OnClearing(_linker.Criteria);
        }

        protected virtual void OnClearing(SearchCriteriaController<TFilter> criteria)
        {
            var filter = criteria.Filter;
            if (filter != null)
                filter.IsFilterClean = true;

            //_linker.Results.Clear(filter);

            if (_filterManager != null)
                _filterManager.SetSelectedReport(null);

            OnSearching(filter);

            SetupMainAccordionIndex(false);
        }

        protected override bool OnBubbleEvent(object source, EventArgs args)
        {
            var result = true;

            if (source as string == "bind_result_filter")
                _linker.Results.Filter = _linker.Criteria.Filter;
            else
                result = base.OnBubbleEvent(source, args);

            return result;
        }

        private void FilterManager_NeedReport(object sender, SearchCriteriaFilterManager.ReportArgs args)
        {
            args.Report = _linker.Criteria.Filter;
        }

        private void FilterManager_ReportSelected(object sender, SearchCriteriaFilterManager.ReportArgs args)
        {
            if (args.Report == null)
            {
                _linker.Criteria.Clear();
                OnClearing(_linker.Criteria);
            }
            else if (args.Report is TFilter filter)
            {
                _linker.Results.Filter = filter;
                LoadSearchedResults();
                SetupMainAccordionIndex(_linker.Results.HasData);
            }
        }

        private void Download_Alert(object sender, AlertArgs args)
        {
            if (_linker.ScreenStatus != null)
                _linker.ScreenStatus.AddMessage(AlertType.Information, "fas fa-info-circle", args.Text);
            else
                ScriptManager.RegisterStartupScript(Page, GetType(), "", $"alert({HttpUtility.JavaScriptStringEncode(args.Text, true)});", true);
        }

        private void Download_NeedData(object sender, BaseSearchDownload.NeedDataArgs args)
        {
            args.DataName = EntityName;
            args.DataSource = GetExportData(args.MaximumRows);
        }

        #endregion

        #region Validation and Integrity

        /// <summary>
        /// Returns true if the URL has no unexpected parameters. Otherwise, adds an alert message and returns false.
        /// </summary>
        protected bool IsQueryStringValid(NameValueCollection queryString, string[] requiredParameters,
            string[] optionalParameters, Alert alert)
        {
            var isValid = true;
            var hasRequiredParameters = requiredParameters.IsNotEmpty();
            var hasOptionalParameters = optionalParameters.IsNotEmpty();

            var parameters = queryString.AllKeys.Where(x => !x.ToLower().Equals("actionurl") && !x.ToLower().Equals("action-url")).ToList();

            // If no parmeters are required and no parameters are optional, then the parameter list should be empty.

            if (parameters.Count > 0 && !hasRequiredParameters && !hasOptionalParameters)
            {
                isValid = false;
                var names = string.Join(", ", parameters.Select(o => o.ToString()));
                alert.AddMessage(AlertType.Error, $"This page does not expect any parameters in the URL, " +
                    $"therefore the following parameters are unexpected: <br><strong>{names}</strong>");
            }
            else if (parameters.Count > 0 && hasRequiredParameters)
            {
                var urlParamsExceptReq = parameters.Except(requiredParameters).ToList();
                var reqParamsExceptUrl = requiredParameters.Except(parameters).ToList();

                if ((urlParamsExceptReq.Count + reqParamsExceptUrl.Count()) > 0)
                {
                    if (!hasOptionalParameters)
                    {
                        isValid = false;
                        alert.AddMessage(AlertType.Error, $"This page requires specific parameters in the URL.");
                    }
                    else
                    {
                        if (urlParamsExceptReq.Except(optionalParameters).Count() > 0)
                        {
                            isValid = false;
                            alert.AddMessage(AlertType.Error, $"This page requires specific parameters in the URL.");
                        }
                    }
                }
            }
            else if (parameters.Count > 0 && hasOptionalParameters)
            {
                var urlParamsExceptOpt = parameters.Except(optionalParameters).ToList();
                if (urlParamsExceptOpt.Count > 0)
                {
                    isValid = false;
                    var names = string.Join(", ", urlParamsExceptOpt.Select(o => o.ToString()));
                    alert.AddMessage(AlertType.Error, $"This page did not expected " +
                            $"the following parameters: <br><strong>{names}</strong>");
                }
            }

            return isValid;
        }

        /// <summary>
        /// Hides the search Criteria and search Results panels.
        /// </summary>
        protected void DisableForm()
        {
            if (_linker.CriteriaTab != null)
                _linker.CriteriaTab.Visible = false;

            if (_linker.ResultsTab != null)
                _linker.ResultsTab.Visible = false;

            SetDownloadsVisiblity(false);
        }

        #endregion

        #region Database Operations

        protected virtual bool DeleteRecords(int[][] keys)
        {
            return false;
        }

        protected virtual bool DeleteRecords(Guid[][] keys)
        {
            return false;
        }

        #endregion

        #region Downloading Search Results

        protected virtual IEnumerable<DownloadColumn> GetExportColumns() => null;

        protected virtual IList GetExportData(int? take) => _linker.Results.GetExportData(take).GetList();

        private void SetupExportColumns(object dataItem)
        {
            var downloadColumns = GetExportColumns();

            if (downloadColumns != null)
            {
                _linker.Download.LoadColumns(downloadColumns);
            }
            else if (dataItem != null)
            {
                _linker.Download.LoadColumns(dataItem);
            }
            else
            {
                var columns = GetDefaultDownloadColumns();

                _linker.Download.LoadColumns(columns);
            }
        }

        protected IEnumerable<DownloadColumn> GetDefaultDownloadColumns() =>
            GetDefaultDownloadColumns(_linker.Criteria, _linker.Results) ?? Enumerable.Empty<DownloadColumn>();

        private static IEnumerable<DownloadColumn> GetDefaultDownloadColumns(SearchCriteriaController<TFilter> criteria, BaseSearchResultsController<TFilter> results)
        {
            if (criteria == null || results == null)
                return null;

            var filter = criteria.Filter;

            if (filter != null)
                filter.Paging = Paging.SetSkipTake(0, 0);

            var list = results.GetExportData(filter, true)?.GetList();
            if (list == null)
                return null;

            if (list is ITypedList tList)
                return BaseSearchDownload.GetColumns(tList);

            var type = list.GetType();
            if (type.IsGenericType && type.GenericTypeArguments.Length == 1)
                return BaseSearchDownload.GetColumns(type.GenericTypeArguments[0]);
            else if (type.IsArray && type.HasElementType)
                return BaseSearchDownload.GetColumns(type.GetElementType());

            return null;
        }

        #endregion
    }
}
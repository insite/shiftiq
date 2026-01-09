using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.UI.Admin.Reports.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<VReportFilter>
    {
        #region Classes

        public class ExportDataItem
        {
            public string ReportTitle { get; set; }
            public string ReportDescription { get; set; }
            public DateTimeOffset? Modified { get; set; }
            public DateTimeOffset? Created { get; set; }
        }

        private class SearchDataItem
        {
            public Guid ReportIdentifier { get; set; }
            public Guid OrganizationIdentifier { get; set; }
            public Guid UserIdentifier { get; set; }

            public string ReportType { get; set; }
            public string ReportTitle { get; set; }
            public string ReportData { get; set; }
            public string ReportDescription { get; set; }

            public DateTimeOffset? Created { get; set; }
            public Guid? CreatedBy { get; set; }
            public DateTimeOffset? Modified { get; set; }
            public Guid? ModifiedBy { get; set; }

            public string CreatedByFullName { get; set; }
            public string ModifiedByFullName { get; set; }

            public bool CanExecute { get; set; }
            public bool CanConfigure { get; set; }
            public bool CanDelete { get; set; }
        }

        #endregion

        #region Initialization and Loading

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //if (!IsPostBack)
            //    AllowImpersonation = TGroupActionSearch.AllowImpersonation(Identity.Groups);
        }

        #endregion

        #region Methods (data binding)

        protected override int SelectCount(VReportFilter filter)
            => VReportSearch.Count(filter);

        protected override IListSource SelectData(VReportFilter filter)
        {
            return VReportSearch.Select(filter)
                .Select(x => new SearchDataItem
                {
                    ReportIdentifier = x.ReportIdentifier,
                    OrganizationIdentifier = x.OrganizationIdentifier,
                    UserIdentifier = x.UserIdentifier,

                    ReportType = x.ReportType,
                    ReportTitle = x.ReportTitle,
                    ReportData = x.ReportData,
                    ReportDescription = x.ReportDescription,

                    Created = x.Created,
                    CreatedBy = x.CreatedBy,
                    Modified = x.Modified,
                    ModifiedBy = x.ModifiedBy,

                    CreatedByFullName = x.CreatedByFullName,
                    ModifiedByFullName = x.ModifiedByFullName,

                    CanExecute = x.ReportType == ReportType.Custom || x.ReportType == ReportType.Shared,
                    CanConfigure = (x.ReportType == ReportType.Custom || x.ReportType == ReportType.Shared)
                                    && x.OrganizationIdentifier == Organization.Identifier,
                    CanDelete = x.OrganizationIdentifier == Organization.Identifier,
                })
                .ToList()
                .ToSearchResult();
        }

        #endregion

        #region Methods (export)

        public override IListSource GetExportData(VReportFilter filter, bool empty)
        {
            return SelectData(filter).GetList().Cast<SearchDataItem>().Select(x => new ExportDataItem
            {
                Created = x.Created,
                Modified = x.Modified,
                ReportDescription = x.ReportDescription,
                ReportTitle = x.ReportTitle
            }).ToList().ToSearchResult();
        }

        #endregion

        #region Methods (render)

        protected string GetDataTimeHtml(DateTimeOffset? date, string userFullName)
        {
            var builder = new StringBuilder();

            if (date.HasValue)
                builder.Append(TimeZones.Format(date.Value, User.TimeZone, true));

            if (userFullName.HasValue())
            {
                if (builder.Length > 0) builder.Append("<br/>");

                builder.Append($"<small class=\"text-body-secondary\">by {userFullName}</small>");
            }

            return builder.ToString();
        }

        protected string GetHtml(object obj)
        {
            var str = obj as string;

            if (str.HasValue())
                return Markdown.ToHtml(obj as string);

            return null;
        }

        protected string GetTitleUrl()
        {
            var url = string.Empty;

            var reportType = (string)Eval("ReportType");
            if (reportType == "Search")
                url = GetActionUrl(null);
            else if (reportType == "Custom")
                url = GetActionUrl(Eval("ReportIdentifier", "custom={0}"));

            if (!url.IsEmpty())
                return url;

            var canConfigure = (bool)Eval("CanConfigure");

            return canConfigure
                ? Eval("ReportIdentifier", "/ui/admin/reports/build?id={0}")
                : Eval("ReportIdentifier", "/ui/admin/reports/edit?id={0}");

            string GetActionUrl(string query)
            {
                var name = (string)Eval("ReportDescription");

                return string.IsNullOrEmpty(name) || WebRoute.GetWebRoute(name) == null
                    ? null
                    : query.IsEmpty()
                        ? "/" + name
                        : $"/{name}?{query}";
            }
        }
        #endregion
    }
}
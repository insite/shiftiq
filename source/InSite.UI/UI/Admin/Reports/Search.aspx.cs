using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Contract;

namespace InSite.UI.Admin.Reports
{
    public partial class Search : SearchPage<VReportFilter>
    {
        private VReportFilter Filter
        {
            get => ViewState[nameof(Filter)] as VReportFilter;
            set => ViewState[nameof(Filter)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Report", "/ui/admin/reports/create", null, null));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CurrentSessionState.Identity.IsAuthenticated)
                HttpResponseHelper.SendHttp403();

            if (IsPostBack)
                return;
        }

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return BaseSearchDownload.GetColumns(typeof(Controls.SearchResults.ExportDataItem)).OrderBy(x => x.Name);
        }

        protected string GetPrivacyScope(string privacy)
        {
            if (privacy.HasValue())
            {
                if (privacy == "User") return Translate("Only me");
                if (privacy == "Tenant") return Translate("Everybody in the organization");

                return privacy;
            }

            return null;
        }
    }
}
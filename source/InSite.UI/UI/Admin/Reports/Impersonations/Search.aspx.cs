using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Reports.Impersonations
{
    public partial class Search : SearchPage<ImpersonationFilter>
    {
        public override string EntityName => "Impersonation";

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return BaseSearchDownload.GetColumns(typeof(Controls.SearchResults.ExportDataItem)).OrderBy(x => x.Name);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }
    }
}
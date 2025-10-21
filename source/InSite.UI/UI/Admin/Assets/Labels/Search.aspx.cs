using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Assets.Labels
{
    public partial class Search : SearchPage<LabelFilter>
    {
        public override string EntityName => "Label";

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return BaseSearchDownload.GetColumns(typeof(Controls.SearchResults.ExportDataItem)).OrderBy(x => x.Name);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Label", "/ui/admin/assets/labels/create", null, null));
        }
    }
}
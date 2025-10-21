using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Glossaries.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Assets.Glossaries
{
    public partial class Search : SearchPage<GlossaryTermFilter>
    {
        public override string EntityName => "Term";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchResults.Alert += (s, a) => ScreenStatus.AddMessage(a);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Glossary Term", "/ui/admin/assets/glossaries/terms/propose", null, null));
        }

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return BaseSearchDownload
                .GetColumns(typeof(InSite.Admin.Assets.Glossaries.Terms.Controls.SearchResults.ExportDataItem))
                .OrderBy(x => x.Name);
        }
    }
}
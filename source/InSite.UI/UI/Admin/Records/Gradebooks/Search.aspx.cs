using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Records.Gradebooks
{
    public partial class Search : SearchPage<QGradebookFilter>
    {
        public override string EntityName => "Gradebook";

        private static readonly Dictionary<string, Func<DownloadColumn, bool>> ExportColumnHandlers = new Dictionary<string, Func<DownloadColumn, bool>>
        {
            { "OrganizationIdentifier", x => false }
        };

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return BaseSearchDownload
                .GetColumns(typeof(InSite.Admin.Records.Gradebooks.Controls.SearchResults.ExportDataItem))
                .Where(x => !ExportColumnHandlers.TryGetValue(x.Name, out var handler) || handler(x))
                .OrderBy(x => x.Name);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Gradebook", "/ui/admin/records/gradebooks/open", null, null));
        }
    }
}
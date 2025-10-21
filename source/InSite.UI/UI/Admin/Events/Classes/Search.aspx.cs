using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Events.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Events.Classes
{
    public partial class Search : SearchPage<QEventFilter>
    {
        public override string EntityName => "Class";

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return BaseSearchDownload
                .GetColumns(typeof(InSite.Admin.Events.Classes.Controls.SearchResults.ExportDataItem))
                .OrderBy(x => x.Name);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Class", "/ui/admin/events/classes/create", null, null));
        }
    }
}
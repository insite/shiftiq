using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Events.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Events.Exams
{
    public partial class Search : SearchPage<QEventFilter>
    {
        public override string EntityName => "Exam";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Exam Event", "/ui/admin/events/exams/create", null, null));
        }

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return BaseSearchDownload
                .GetColumns(typeof(InSite.Admin.Events.Exams.Controls.SearchResults.ExportDataItem))
                .OrderBy(x => x.Name);
        }
    }
}
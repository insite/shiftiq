using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Events.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Events.Appointments
{
    public partial class Search : SearchPage<QEventFilter>
    {
        public override string EntityName => "Appointment";

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return BaseSearchDownload
                .GetColumns(typeof(InSite.Admin.Events.Appointments.Controls.SearchResults.ExportDataItem))
                .OrderBy(x => x.Name);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Appointment", "/ui/admin/events/appointments/create", null, null));
        }
    }
}
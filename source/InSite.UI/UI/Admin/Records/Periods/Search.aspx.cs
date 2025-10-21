using System;
using System.Collections.Generic;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.UI.Layout.Admin;

using Shift.Constant;
using Shift.Contract;

namespace InSite.Admin.Records.Periods
{
    public partial class Search : SearchPage<QPeriodFilter>
    {
        public override string EntityName => "Period";

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return new[]
            {
                new DownloadColumn("PeriodIdentifier", "Identifier", null, 15, HorizontalAlignment.Right),
                new DownloadColumn("PeriodName", "Period Name", null, 60),
                new DownloadColumn("PeriodStart", "Period Start","MMM dd, yyyy", 15),
                new DownloadColumn("PeriodEnd", "Period End","MMM dd, yyyy", 15)
            };
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Period", "/ui/admin/records/periods/create", null, null));
        }
    }
}
using System;
using System.Collections.Generic;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.UI.Layout.Admin;

using Shift.Constant;
using Shift.Contract;

namespace InSite.Admin.Records.Logbooks
{
    public partial class Search : SearchPage<QJournalSetupFilter>
    {
        public override string EntityName => "Logbook Template";

        


        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return new[]
            {
                new DownloadColumn("Identifier", "Identifier", null, 15, HorizontalAlignment.Right),
                new DownloadColumn("LogbookName", "Logbook Name", null, 60),
                new DownloadColumn("AchievementTitle", "Achievement", null, 60),
                new DownloadColumn("ClassTitle", "Class", null, 60),
                new DownloadColumn("ClassScheduledStartDate", "Class Start","MMM dd, yyyy", 15),
                new DownloadColumn("ClassScheduledEndDate", "Class End","MMM dd, yyyy", 15),
                new DownloadColumn("IsLocked", "Is Locked")
            };
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Logbook", "/ui/admin/records/logbooks/open", null, null));
        }
    }
}
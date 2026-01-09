using System;
using System.Collections.Generic;

using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Constant;
using Shift.Contract;
using Shift.Sdk.UI;

namespace InSite.Cmds.Admin.Organizations.Forms
{
    public partial class Search : SearchPage<CompanyFilter>, ICmdsUserControl
    {
        public override string EntityName => "Organization";

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            PageHelper.AutoBindHeader(Page, new BreadcrumbItem("Add New Organization", "/ui/cmds/admin/organizations/create"));
        }

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return new DownloadColumn[]
            {
                new DownloadColumn("CompanyTitle", "Name", null, 40, HorizontalAlignment.Left),
                new DownloadColumn("DepartmentCount", "Departments", null, 25, HorizontalAlignment.Center),
                new DownloadColumn("PersonCount", "People", null, 25, HorizontalAlignment.Center),
                new DownloadColumn("ProfileCount", "Profiles", null, 25, HorizontalAlignment.Center),
                new DownloadColumn("CompetencyCount", "Competencies", null, 25, HorizontalAlignment.Center),
                new DownloadColumn("AchievementCount", "Achievements", null, 25, HorizontalAlignment.Center)
            };
        }
    }
}
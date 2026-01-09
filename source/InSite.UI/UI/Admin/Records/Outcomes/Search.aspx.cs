using System;
using System.Collections.Generic;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Records.Outcomes
{
    public partial class Search : SearchPage<QGradebookCompetencyValidationFilter>
    {
        public override string EntityName => "Outcomes";

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return new[]
            {
                new DownloadColumn("Gradebook.GradebookTitle", "Gradebook", null, 45),
                new DownloadColumn("Student.UserFullName", "Student", null, 45),
                new DownloadColumn("Standard.StandardTitle", "Competency", null, 45),
                new DownloadColumn("ValidationPoints", "Points", "n2", 15),
                new DownloadColumn("Gradebook.Event.EventTitle", "Class", null, 45),
                new DownloadColumn("Gradebook.Achievement.AchievementTitle", "Achievment", null, 25),
            };
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }
    }
}
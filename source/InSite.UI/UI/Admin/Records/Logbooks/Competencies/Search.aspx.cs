using System;
using System.Collections.Generic;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.UI.Admin.Records.Logbooks.Competencies
{
    public partial class Search1 : SearchPage<QExperienceCompetencyFilter>
    {
        public override string EntityName => "Logged Competency";

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return new[]
            {
                new DownloadColumn("JournalIdentifier", "LogbookIdentifier", null, 15, HorizontalAlignment.Right),
                new DownloadColumn("JournalSetupName", "Logbook Name", null, 60),
                new DownloadColumn("UserFullName", "User", null, 60),
                new DownloadColumn("UserEmail", "Email", null, 60),
                new DownloadColumn("Sequence", "Sequence", null, 60),
                new DownloadColumn("Created", "Created", null, 60),
                new DownloadColumn("Status", "Status", null, 60),

                new DownloadColumn("FrameworkIdentifier", "Framework Identifier", null, 15, HorizontalAlignment.Right),
                new DownloadColumn("FrameworkName", "Framework", null, 60),
                new DownloadColumn("CompetencyIdentifier", "Competency Identifier", null, 15, HorizontalAlignment.Right),
                new DownloadColumn("CompetencyName", "Competency", null, 60),
                new DownloadColumn("Hours", "Hours", null, 60),
                new DownloadColumn("SatisfactionLevel", "Satisfaction Level", null, 60),
                new DownloadColumn("SkillRating", "Skill Rating", null, 60),
            };
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }
    }
}
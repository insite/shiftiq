using System;
using System.Collections.Generic;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.UI.Admin.Records.Validators.Entries
{
    public partial class Search1 : SearchPage<QExperienceFilter>
    {
        public override string EntityName => "Logged Entry";

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return new[]
            {
                new DownloadColumn("JournalIdentifier", "Identifier", null, 15, HorizontalAlignment.Right),
                new DownloadColumn("JournalSetupName", "Logbook Name", null, 60),
                new DownloadColumn("UserFullName", "Learner", null, 60),
                new DownloadColumn("UserEmail", "Email", null, 60),
                new DownloadColumn("Sequence", "Sequence", null, 60),
                new DownloadColumn("Created", "Created", null, 60),
                new DownloadColumn("Status", "Status", null, 60),
            };
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }
    }
}
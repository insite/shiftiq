using System;
using System.Collections.Generic;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.UI.Admin.Records.Logbooks.SearchJournal
{
    public partial class Search : SearchPage<VJournalSetupUserFilter>
    {
        public override string EntityName => "Logbook";

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return new[]
            {
                new DownloadColumn("JournalSetupIdentifier", "Identifier", null, 15, HorizontalAlignment.Right),
                new DownloadColumn("JournalSetupName", "Logbook Name", null, 60),
                new DownloadColumn("UserFullName", "Learner", null, 60),
                new DownloadColumn("UserEmail", "Email", null, 60),
            };
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }
    }
}
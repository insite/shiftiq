using System;
using System.Collections.Generic;

using InSite.Application.Registrations.Read;
using InSite.Common;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Events.Registrations.Exams
{
    public partial class Search : SearchPage<QRegistrationFilter>
    {
        public override string EntityName => "Registration";

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!IsPostBack)
                LoadSearchedResults();
        }

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return new[]
            {
                new DownloadColumn("EventType", null, null, 20),
                new DownloadColumn("EventScheduledStart", null, "MMM dd, yyyy", 20),
                new DownloadColumn("EventNumber", null, null, 20),
                new DownloadColumn("EventTitle", null, null, 40),
                new DownloadColumn("EventVenueOffice", null, null, 40),
                new DownloadColumn("EventVenueLocation", null, null, 40),
                new DownloadColumn("EventVenueRoom", null, null, 20),

                new DownloadColumn("AssessmentFormCode", null, null, 20),
                new DownloadColumn("AssessmentFormName", null, null, 20),

                new DownloadColumn("LearnerIdentifier"),
                new DownloadColumn("LearnerName"),
                new DownloadColumn("LearnerCode", LabelHelper.GetLabelContentText("Person Code") ),
                new DownloadColumn("LearnerEmail"),

                new DownloadColumn("RegistrationSequence"),
                new DownloadColumn("RegistrationRequestedOn", null, "MMM dd, yyyy", 20),
                new DownloadColumn("RegistrationType"),
                new DownloadColumn("ApprovalStatus"),
                new DownloadColumn("AttendanceStatus"),
                new DownloadColumn("RegistrationComment"),
            };
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }
    }
}
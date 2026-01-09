using System;
using System.Collections.Generic;

using InSite.Application.Registrations.Read;
using InSite.Common;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.UI.Admin.Events.Registrations
{
    public partial class Search : SearchPage<QRegistrationFilter>
    {
        public override string EntityName => "Registration";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);

            SearchResults.Searched += (source, args) =>
            {
                var filter = SearchCriteria.Filter;
                var registrants = ServiceLocator.RegistrationSearch.GetRegistrationCandidateIdentifiers(filter);

                SendEmail.BindModelToControls("Event Registrant", registrants);
            };
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (IsPostBack)
                return;

            LoadSearchedResults();
        }

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return new[]
            {
                new DownloadColumn("EventScheduledStart", "Event Start", "MMM dd, yyyy", 15),
                new DownloadColumn("EventScheduledEnd", "Event End", "MMM dd, yyyy", 15),
                new DownloadColumn("EventType", "Event Type", null, 15),
                new DownloadColumn("EventTitle", "Event Title", null, 55),
                new DownloadColumn("EventAchievementTitle", "Achievement Title", null, 55),
                new DownloadColumn("EventAchievementDescription", "Achievement Description", null, 55),
                new DownloadColumn("RegistrationRequestedOn", "Registration Date", "MMM dd, yyyy", 15),
                new DownloadColumn("UserFullName", "Registrant Name"),
                new DownloadColumn("Email", "Registrant Email"),
                new DownloadColumn("Phone", "Registrant Phone", null, 15),
                new DownloadColumn("ApprovalStatus", "Approval", null, 15),
                new DownloadColumn("AttendanceStatus", "AttendanceStatus", null, 15),
                new DownloadColumn("BillingCode", "BillingCode", null, 15),
                new DownloadColumn("RegistrationFee", "Fee", "$ ###,##0.00", 15, HorizontalAlignment.Right),
                new DownloadColumn("IncludeInT2202", "Include in T2202"),
                new DownloadColumn("EmployerGroupIdentifier", "Employer Identifier"),
                new DownloadColumn("EmployerGroupName", "Employer at Time of Registration"),
                new DownloadColumn("EmployerGroupRegion", "Employer Region"),
                new DownloadColumn("EmployerGroupStatus", "Employer Status"),
                new DownloadColumn("LearnerId", LabelHelper.GetLabelContentText("Person Code")),
                new DownloadColumn("RegistrationSequence", "Registration #"),
                new DownloadColumn("WorkBasedHoursToDate", "Hours Worked to Date"),
                new DownloadColumn("RegistrationComment", "Comment"),
                new DownloadColumn("RegistrationRequestedByName", "Registered By Name"),
                new DownloadColumn("RegistrationRequestedByEmail", "Registered By Email"),
                new DownloadColumn("RegistrationRequestedByIdentifier", "Registered By Identifier"),
                new DownloadColumn("RegistrantIsELL", "Registrant is ELL"),
                new DownloadColumn("RegistrantPostalCode", "Registrant Postal Code"),
                new DownloadColumn("Department", "Department"),
            };
        }
    }
}
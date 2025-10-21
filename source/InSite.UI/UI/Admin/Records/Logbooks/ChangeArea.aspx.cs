using System;

using InSite.Admin.Records.Logbooks;
using InSite.Application.JournalSetups.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Records.Logbooks
{
    public partial class ChangeArea : AdminBasePage, IHasParentLinkParameters
    {
        private Guid JournalSetupId => Guid.TryParse(Request["journalsetup"], out var value) ? value : Guid.Empty;
        private Guid AreaId => Guid.TryParse(Request["area"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var journalSetup = ServiceLocator.JournalSearch.GetJournalSetup(JournalSetupId, x => x.Event);
            if (journalSetup == null || journalSetup.OrganizationIdentifier != Organization.Identifier)
                RedirectToSearch();

            var standard = StandardSearch.Select(AreaId);
            if (standard == null || standard.StandardType != StandardType.Area)
                RedirectToParent();

            var header = LogbookHeaderHelper.GetLogbookHeader(journalSetup, User.TimeZone);
            PageHelper.AutoBindHeader(this, null, header);

            var requirement = ServiceLocator.JournalSearch.GetAreaRequirement(JournalSetupId, AreaId);

            LoadData(standard, requirement);

            CancelButton.NavigateUrl = GetParentUrl();
        }

        private void LoadData(Standard standard, QAreaRequirement requirement)
        {
            AreaName.Text = $"<a href=\"/ui/admin/standards/edit?id={standard.StandardIdentifier}\">{CompetencyHelper.GetStandardName(standard)}</a>";
            AreaHours.ValueAsDecimal = requirement?.AreaHours;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var hours = AreaHours.ValueAsDecimal;

            ServiceLocator.SendCommand(new ModifyJournalSetupAreaHours(JournalSetupId, AreaId, hours));

            RedirectToParent();
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? GetParentUrlParameters()
                : null;
        }

        public string GetParentUrl() => $"/ui/admin/records/logbooks/outline?" + GetParentUrlParameters();

        public string GetParentUrlParameters() => $"journalsetup={JournalSetupId}&panel=competencies";

        private void RedirectToParent() => HttpResponseHelper.Redirect(GetParentUrl());

        private void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/records/logbooks/search");
    }
}
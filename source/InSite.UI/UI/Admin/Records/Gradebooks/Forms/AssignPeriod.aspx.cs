using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Admin.Records.Gradebooks.Controls;
using InSite.Application.Gradebooks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Records.Gradebooks.Forms
{
    public partial class AssignPeriod : AdminBasePage, IHasParentLinkParameters
    {
        private Guid GradebookID => Guid.TryParse(Request["gradebook"], out var gradebookID) ? gradebookID : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchButton.Click += SearchButton_Click;
            ClearButton.Click += ClearButton_Click;

            UpdateButton.Click += UpdateButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            LoadGrid();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            FilterPeriodIdentifier.Value = null;
            FilterTextBox.Text = null;
            HideAssignedPeriod.Checked = false;
            GradedSince.Value = null;
            GradedBefore.Value = null;

            LoadGrid();
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            SaveChanges();
        }

        private void LoadData()
        {
            var gradebook = ServiceLocator.RecordSearch.GetGradebook(GradebookID, x => x.Event, x => x.Achievement, x => x.Period);
            if (gradebook == null || gradebook.OrganizationIdentifier != CurrentSessionState.Identity.Organization.OrganizationIdentifier)
            {
                HttpResponseHelper.Redirect("/ui/admin/records/gradebooks/search");
                return;
            }

            if (gradebook.IsLocked)
            {
                HttpResponseHelper.Redirect($"/ui/admin/records/gradebooks/outline?id={gradebook.GradebookIdentifier}");
                return;
            }

            var title = gradebook.GradebookTitle;
            if (gradebook.Event != null)
                title += $" <span class='form-text'> for {gradebook.Event.EventTitle} ({GetLocalTime(gradebook.Event.EventScheduledStart)} - {GetLocalTime(gradebook.Event.EventScheduledEnd)})</span>";

            PageHelper.AutoBindHeader(this, null, title);

            LoadGrid();
        }

        private static string GetLocalTime(DateTimeOffset? item)
            => item.FormatDateOnly(User.TimeZone, nullValue: string.Empty);

        private void LoadGrid()
        {
            var hasRows = AssignPeriodGrid.LoadData(
                GradebookID,
                FilterPeriodIdentifier.Value,
                FilterTextBox.Text,
                HideAssignedPeriod.Checked,
                GradedSince.Value,
                GradedBefore.Value
            ) > 0;

            UpdateButton.Enabled = hasRows;
        }

        private void SaveChanges()
        {
            var users = AssignPeriodGrid.GetUsers();
            var commands = new List<Command>();
            var periodIdentifier = UpdatePeriodIdentifier.Value;

            foreach (var userIdentifier in users)
            {
                var command = new ChangeGradebookUserPeriod(GradebookID, userIdentifier, periodIdentifier);
                commands.Add(command);
            }

            ServiceLocator.SendCommands(commands);

            AssignPeriodGrid.SearchWithCurrentPageIndex(AssignPeriodGrid.Filter);

            UpdateButton.Enabled = AssignPeriodGrid.HasRows;

            StatusAlert.AddMessage(AlertType.Success, $"{users.Count} users have been updated");
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={GradebookID}"
                : null;
        }
    }
}

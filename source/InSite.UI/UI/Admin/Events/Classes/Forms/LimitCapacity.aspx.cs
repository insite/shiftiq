using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

using Shift.Common.Timeline.Commands;

using InSite.Application.Events.Write;
using InSite.Common;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Events.Classes.Forms
{
    public partial class LimitCapacity : AdminBasePage, IHasParentLinkParameters
    {
        private const string SearchUrl = "/ui/admin/events/classes/search";

        private Guid? EventIdentifier =>
            Guid.TryParse(Request["event"], out var result) ? result : (Guid?)null;

        private string OutlineUrl
            => $"/ui/admin/events/classes/outline?event={EventIdentifier.Value}&panel=class";

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/outline") ? $"event={EventIdentifier}&panel=class" : null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += (s, a) => Save();

            CapacityValidator.ServerValidate += (s, a) =>
            {
                a.IsValid = MaximumCapacity.ValueAsInt == null
                    || MinimalCapacity.ValueAsInt == null
                    || MaximumCapacity.ValueAsInt >= MinimalCapacity.ValueAsInt;
            };

            WaitlistEnabled.AutoPostBack = true;
            WaitlistEnabled.SelectedIndexChanged += (s, a) => OnWaitlistEnabledChanged();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        private void OnWaitlistEnabledChanged()
        {
            MaximumCapacityRequired.Visible = WaitlistEnabled.SelectedValue == "Enabled";
        }

        private void Open()
        {
            if (!CanEdit)
                HttpResponseHelper.Redirect(SearchUrl);

            var @event = EventIdentifier.HasValue
                ? ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value, x => x.VenueLocation, x => x.Achievement)
                : null;

            if (@event == null || @event.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(
                Page,
                qualifier: $"{@event.EventTitle} <span class='form-text'>scheduled to start {@event.EventScheduledStart.FormatDateOnly(User.TimeZone)}</span>");

            SummaryInfo.Bind(@event);
            LocationInfo.Bind(@event);

            var approvedCount = @event.Registrations.Where(x => string.Equals(x.ApprovalStatus, "Registered", StringComparison.OrdinalIgnoreCase)).Count();
            var waitlistedCount = @event.Registrations.Where(x => string.Equals(x.ApprovalStatus, "Waitlisted", StringComparison.OrdinalIgnoreCase)).Count();

            MinimalCapacity.ValueAsInt = @event.CapacityMinimum;
            MaximumCapacity.ValueAsInt = @event.CapacityMaximum;
            WaitlistEnabled.SelectedValue = @event.WaitlistEnabled ? "Enabled" : "Disabled";
            BillingCodeEnabled.SelectedValue = @event.BillingCodeEnabled ? "Yes" : "No";

            var personCodeText = LabelHelper.GetLabelContentText("Person Code");

            PersonCodeIsRequiredLabel.Text = $"{personCodeText} Required";
            PersonCodeIsRequiredHint.Text = $"For organizations using the {personCodeText} unique identifier, indicate if registrants must enter the number to complete a registration.";
            PersonCodeIsRequired.SelectedValue = @event.PersonCodeIsRequired ? "Yes" : "No";

            AllowMultipleRegistrations.SelectedValue = @event.AllowMultipleRegistrations ? "Yes" : "No";

            OnWaitlistEnabledChanged();

            CancelButton.NavigateUrl = OutlineUrl;
        }

        private void Save()
        {
            if (!Page.IsValid)
                return;

            var min = MinimalCapacity.ValueAsInt;
            var max = MaximumCapacity.ValueAsInt;
            var waitlist = WaitlistEnabled.SelectedValue == "Enabled" ? ToggleType.Enabled : ToggleType.Disabled;

            ServiceLocator.SendCommands(new Command[] 
            {
                new AdjustCandidateCapacity(EventIdentifier.Value, min, max, waitlist),
                new ModifyAllowMultipleRegistrations(EventIdentifier.Value, AllowMultipleRegistrations.SelectedValue == "Yes"),
                new ModifyPersonCodeIsRequired(EventIdentifier.Value, PersonCodeIsRequired.SelectedValue == "Yes"),
                new EnableEventBillingCode(EventIdentifier.Value, BillingCodeEnabled.SelectedValue == "Yes"),
            });

            HttpResponseHelper.Redirect(OutlineUrl);
        }
    }
}
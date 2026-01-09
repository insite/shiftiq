using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Registrations.Write;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Sdk.UI;

using AlertType = Shift.Constant.AlertType;

namespace InSite.Custom.CMDS.User.Events.Forms
{
    public partial class Register : AdminBasePage, ICmdsUserControl
    {
        private Guid? EventID => Guid.TryParse(Request["id"], out var value) ? value : (Guid?)null;

        private bool IsAdministrator => Identity.Groups.Find(x => x.Name.EndsWith("Administrators")) != null;

        public List<InSite.Custom.CMDS.User.Programs.Controls.UpcomingSessionListItem> UpcomingEvents
        {
            get { return ViewState[nameof(UpcomingEvents)] as List<InSite.Custom.CMDS.User.Programs.Controls.UpcomingSessionListItem>; }
            set { ViewState[nameof(UpcomingEvents)] = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SelectedEventID.AutoPostBack = true;
            SelectedEventID.ValueChanged += (s, a) => ShowEvent();

            CandidateKey.AutoPostBack = true;
            CandidateKey.ValueChanged += (s, a) => ShowEvent();

            SubmitButton.Click += SubmitButton_Click;
            RegisterAnotherUserLink.Click += RegisterAnotherUserLink_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Request["status"] == "registered")
            {
                ScreenStatus.AddMessage(AlertType.Success, $"Validator training session registration completed successfully.");
                return;
            }

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            UpcomingEvents = InSite.Custom.CMDS.User.Programs.Controls.UpcomingSessionList.SearchUpcomingEvents();

            if (UpcomingEvents.Count == 0)
            {
                ScreenStatus.AddMessage(AlertType.Error, "There are no upcoming training sessions.");
                return;
            }

            RegistrationSection.Visible = true;

            SelectedEventID.Items.Clear();
            foreach (var item in UpcomingEvents)
                SelectedEventID.Items.Add(new ComboBoxOption
                {
                    Text = item.Title + (item.IsFull ? " (full)" : ""),
                    Value = item.Identifier.ToString(),
                });

            CandidateKey.Value = User.UserIdentifier;
            CandidateKey.Enabled = IsAdministrator;

            var @event = UpcomingEvents.Find(x => x.Identifier == EventID) ?? UpcomingEvents.Find(x => !x.IsFull) ?? UpcomingEvents[0];

            SelectedEventID.ValueAsGuid = @event.Identifier;

            if (UpcomingEvents.Find(x => !x.IsFull && !x.IsClosed) == null)
                ScreenStatus.AddMessage(AlertType.Error, "There is no upcoming training session that has a vacant seat.");

            ShowEvent();
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid || !RegistrationSection.Visible)
                return;

            var eventId = SelectedEventID.ValueAsGuid;

            var upcomingEvent = UpcomingEvents.FirstOrDefault(x => x.Identifier == eventId.Value);

            if (upcomingEvent == null)
            {
                ScreenStatus.AddMessage(AlertType.Error, $"<strong>Class</strong> is a required field.");
                return;
            }

            if (upcomingEvent.IsFull)
            {
                ScreenStatus.AddMessage(AlertType.Error, $"The class you selected has no vacant seats.");
                return;
            }

            if (!CandidateKey.HasValue)
            {
                ScreenStatus.AddMessage(AlertType.Error, $"<strong>Candidate</strong> is a required field.");
                return;
            }

            var candidate = UserSearch.Select(CandidateKey.Value.Value);

            var registrationIdentifier = UniqueIdentifier.Create();
            var requestCommand = new RequestRegistration(registrationIdentifier, Organization.OrganizationIdentifier, eventId.Value, candidate.UserIdentifier, null, null, null, null, null);

            ServiceLocator.SendCommand(requestCommand);

            var change = new CmdsTrainingRegistrationSubmitted
            {
                SessionIdentifier = upcomingEvent.Identifier,
                SessionTitle = upcomingEvent.Title,
                RegistrantName = candidate.FullName,
                RegistrantEmail = candidate.Email,
                RegistrantCompany = Organization.CompanyName,
                Comment = Comment.Text,
            };

            ServiceLocator.ChangeQueue.Publish(change);

            ScreenStatus.AddMessage(AlertType.Success, $"Validator training session registration completed successfully for {candidate.FullName} ({candidate.Email}).");

            RegisterAnotherUserLink.Visible = IsAdministrator;
            RegistrationSection.Visible = false;
        }

        private void RegisterAnotherUserLink_Click(object sender, EventArgs e)
        {
            RegisterAnotherUserLink.Visible = false;
            RegistrationSection.Visible = true;

            CandidateKey.Value = null;

            ShowEvent();
        }

        private void ShowEvent()
        {
            var eventId = SelectedEventID.ValueAsGuid;

            var ev = UpcomingEvents.FirstOrDefault(x => x.Identifier == eventId.Value);

            var candidate = CandidateKey.HasValue ? UserSearch.Select(CandidateKey.Value.Value) : null;

            var alreadyRegistered = candidate != null && ev.Registrants.Any(x => x == candidate.UserIdentifier);

            if (alreadyRegistered)
            {
                var message = candidate.UserIdentifier == User.UserIdentifier ? "You already registered in this class" : "This candidate already registered in this class";
                ScreenStatus.AddMessage(AlertType.Warning, message);
            }

            var isFull = ev.IsFull;
            var isClosed = ev.IsClosed;

            if (isClosed && !alreadyRegistered)
                ScreenStatus.AddMessage(AlertType.Error, "The registration to this class is now closed. Please register in another class, send us an email if you are interested in having another class added to our schedule.");
            else if (isFull && !alreadyRegistered)
                ScreenStatus.AddMessage(AlertType.Error, "This class is now full. Please register in another class, send us an email if you are interested in having another class added to our schedule.");

            IsClosed.Visible = isClosed;
            IsFull.Visible = !isClosed && isFull;
            SeatsAvailableField.Visible = !isClosed && !isFull && ev.CapacityMaximum.HasValue;

            CandidateKey.Enabled = !isClosed && !isFull;

            Format.Text = ev.Format ?? "N/A";
            Capacity.Text = $"{ev.CapacityMinimum} - {ev.CapacityMaximum}";
            RegistrationCount.Text = ev.RegistrationCount.ToString();

            if (!isClosed && !isFull && ev.CapacityMaximum.HasValue)
                SeatsAvailable.Text = (ev.CapacityMaximum.Value - ev.RegistrationCount).ToString();

            CandidatePanel.Visible = candidate != null;

            if (candidate != null)
            {
                CandidateName.Text = candidate.FullName;
                CandidateEmail.Text = candidate.Email;
                CandidateCompany.Text = Organization.CompanyName;
            }

            CommentField.Visible = !alreadyRegistered && !isClosed && !isFull;

            SubmitButton.Visible = !alreadyRegistered && !isClosed && !isFull;
        }
    }
}
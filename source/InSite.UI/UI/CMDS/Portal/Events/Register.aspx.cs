using System;
using System.Linq;

using InSite.Application.Events.Read;
using InSite.Application.Registrations.Write;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

using AlertType = Shift.Constant.AlertType;

namespace InSite.Custom.CMDS.User.Events.Forms
{
    public partial class Register : AdminBasePage, ICmdsUserControl
    {
        #region Properties

        private Guid? EventID => Guid.TryParse(Request["id"], out var value) ? value : (Guid?)null;

        private bool IsAdministrator => Identity.Groups.Find(x => x.Name.EndsWith("Administrators")) != null;

        #endregion

        #region Initialization

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

            var events = ServiceLocator.EventSearch.GetEvents(new QEventFilter
            {
                OrganizationIdentifier = OrganizationIdentifiers.Keyera,
                EventScheduledSince = DateTimeOffset.UtcNow
            }, null, null, x => x.Registrations);

            if (events.Count == 0)
            {
                ScreenStatus.AddMessage(AlertType.Error, "There are no upcoming training sessions.");
                return;
            }

            RegistrationSection.Visible = true;

            SelectedEventID.Items.Clear();
            foreach (var item in events)
                SelectedEventID.Items.Add(new ComboBoxOption
                {
                    Text = item.EventTitle + (IsEventFull(item) ? " (full)" : ""),
                    Value = item.EventIdentifier.ToString(),
                });

            CandidateKey.Value = User.UserIdentifier;
            CandidateKey.Enabled = IsAdministrator;

            var @event = events.Find(x => x.EventIdentifier == EventID) ?? events.Find(x => !IsEventFull(x)) ?? events[0];

            SelectedEventID.ValueAsGuid = @event.EventIdentifier;

            if (events.Find(x => !IsEventFull(x) && !IsEventClosed(x)) == null)
                ScreenStatus.AddMessage(AlertType.Error, "There is no upcoming training session that has a vacant seat.");

            ShowEvent();
        }

        #endregion

        #region Event handlers

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid || !RegistrationSection.Visible)
                return;

            var eventId = SelectedEventID.ValueAsGuid;
            var @event = eventId.HasValue
                ? ServiceLocator.EventSearch.GetEvent(eventId.Value)
                : null;

            if (@event == null)
            {
                ScreenStatus.AddMessage(AlertType.Error, $"<strong>Class</strong> is a required field.");
                return;
            }

            if (IsEventFull(@event))
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
                SessionIdentifier = @event.EventIdentifier,
                SessionTitle = @event.EventTitle,
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

        #endregion

        #region Helper methods

        private void ShowEvent()
        {
            var eventId = SelectedEventID.ValueAsGuid;
            var ev = ServiceLocator.EventSearch.GetEvent(eventId.Value, x => x.Registrations);

            var candidate = CandidateKey.HasValue ? UserSearch.Select(CandidateKey.Value.Value) : null;

            var alreadyRegistered = candidate != null && ev.Registrations.FirstOrDefault(x => x.CandidateIdentifier == candidate.UserIdentifier) != null;
            if (alreadyRegistered)
            {
                var message = candidate.UserIdentifier == User.UserIdentifier ? "You already registered in this class" : "This candidate already registered in this class";
                ScreenStatus.AddMessage(AlertType.Warning, message);
            }

            var isFull = IsEventFull(ev);
            var isClosed = IsEventClosed(ev);

            if (isClosed && !alreadyRegistered)
                ScreenStatus.AddMessage(AlertType.Error, "The registration to this class is now closed. Please register in another class, send us an email if you are interested in having another class added to our schedule.");
            else if (isFull && !alreadyRegistered)
                ScreenStatus.AddMessage(AlertType.Error, "This class is now full. Please register in another class, send us an email if you are interested in having another class added to our schedule.");

            IsClosed.Visible = isClosed;
            IsFull.Visible = !isClosed && isFull;
            SeatsAvailableField.Visible = !isClosed && !isFull && ev.CapacityMaximum.HasValue;

            CandidateKey.Enabled = !isClosed && !isFull;

            Format.Text = ev.EventFormat ?? "N/A";
            Capacity.Text = $"{ev.CapacityMinimum} - {ev.CapacityMaximum}";
            RegistrationCount.Text = ev.Registrations.Count.ToString();

            if (!isClosed && !isFull && ev.CapacityMaximum.HasValue)
                SeatsAvailable.Text = (ev.CapacityMaximum.Value - ev.Registrations.Count).ToString();

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

        private static bool IsEventFull(QEvent @event)
        {
            return @event.CapacityMaximum.HasValue && @event.CapacityMaximum.Value <= @event.Registrations.Count
                || (@event.CapacityMinimum ?? 0) == 0 && !@event.CapacityMaximum.HasValue;
        }

        private static bool IsEventClosed(QEvent @event)
        {
            return @event.RegistrationDeadline.HasValue && @event.RegistrationDeadline < DateTimeOffset.UtcNow;
        }

        #endregion
    }
}
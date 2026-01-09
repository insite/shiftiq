using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Application.Gradebooks.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Events;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Events.Classes.Forms
{
    public partial class Create : AdminBasePage
    {
        #region Constants

        private const string OutlineUrl = "/ui/admin/events/classes/outline";
        private const string SearchUrl = "/ui/admin/events/classes/search";

        #endregion

        #region Inialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CreationType.AutoPostBack = true;
            CreationType.ValueChanged += (s, a) => OnCreationTypeSelected();

            OneEventScheduledEndValidator.ServerValidate += (s, a) => OnEventScheduledEndValidatorServerValidate(
                OneEventScheduledEndValidator, a,
                OneEventScheduledStart.Value?.UtcDateTime, OneEventScheduledEnd.Value?.UtcDateTime);

            CopyEventSelector.AutoPostBack = true;
            CopyEventSelector.ValueChanged += (s, a) => OnCopyEventSelectorSelectedIndexChanged();

            CopyEventScheduledEndValidator.ServerValidate += (s, a) => OnEventScheduledEndValidatorServerValidate(
                CopyEventScheduledEndValidator, a,
                CopyEventScheduledStart.Value?.UtcDateTime, CopyEventScheduledEnd.Value?.UtcDateTime);

            UploadJsonFileUploaded.Click += UploadJsonFileUploaded_Click;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanCreate)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(Page);

            Open();
        }

        #endregion

        #region Event handlers

        private void OnCreationTypeSelected()
        {
            var value = CreationType.ValueAsEnum;

            if (value == CreationTypeEnum.One)
                MultiView.SetActiveView(OneView);
            if (value == CreationTypeEnum.Duplicate)
                MultiView.SetActiveView(CopyView);
            if (value == CreationTypeEnum.Upload)
                MultiView.SetActiveView(UploadView);
        }

        private void OnEventScheduledEndValidatorServerValidate(Common.Web.UI.CustomValidator validator, ServerValidateEventArgs args, DateTime? start, DateTime? end)
        {
            if (end <= start)
            {
                args.IsValid = false;
                validator.ErrorMessage = "End must be later than Start";
            }
        }

        private void OnCopyEventSelectorSelectedIndexChanged()
        {
            if (CopyEventSelector.HasValue)
            {
                var @event = ServiceLocator.EventSearch.GetEvent(CopyEventSelector.Value.Value);

                CopyEventTitle.Text = @event.EventTitle;
                CopyEventScheduledStart.Value = @event.EventScheduledStart;
                CopyEventScheduledEnd.Value = @event.EventScheduledEnd;

                var gradebookCount = ServiceLocator.RecordSearch.CountGradebooks(new QGradebookFilter { GradebookEventIdentifier = @event.EventIdentifier });
                CopyGradebooksField.Visible = gradebookCount > 0;

                DurationField.Visible = true;

                if (@event.DurationQuantity.HasValue && !string.IsNullOrEmpty(@event.DurationUnit))
                {
                    var ending = @event.DurationQuantity != 1 ? "s" : "";

                    CopyDurationCheckbox.Visible = true;
                    CopyDurationCheckbox.Checked = true;
                    CopyDurationCheckbox.Text = $"Copy Duration ({@event.DurationQuantity} {@event.DurationUnit}{ending})";
                }
                else
                {
                    CopyDurationCheckbox.Visible = false;
                    CopyDurationCheckbox.Checked = false;
                }

                CopyEventDuration.ValueAsInt = null;
            }
            else
            {
                CopyEventTitle.Text = null;
                CopyEventScheduledStart.Value = null;
                CopyEventScheduledEnd.Value = null;

                CopyGradebooksField.Visible = false;
                
                DurationField.Visible = false;
            }
        }

        private void UploadJsonFileUploaded_Click(object sender, EventArgs e)
        {
            if (!UploadJsonFile.HasFile)
            {
                ScreenStatus.AddMessage(AlertType.Error, "Uploaded file is empty");
                return;
            }

            using (var stream = UploadJsonFile.OpenFile())
            {
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                    UploadJsonInput.Text = reader.ReadToEnd();
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var value = CreationType.ValueAsEnum;

            if (value == CreationTypeEnum.One)
                SaveOne();
            if (value == CreationTypeEnum.Duplicate)
                SaveCopy();
            if (value == CreationTypeEnum.Upload)
                SaveUpload();
        }

        #endregion

        #region Methods (open)

        private void Open()
        {
            CreationType.EnsureDataBound();
            CreationType.SetVisibleOptions(CreationTypeEnum.One, CreationTypeEnum.Duplicate, CreationTypeEnum.Upload);

            OneVenueLocationGroup.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
            OneVenueLocationGroup.Filter.GroupType = GroupTypes.Venue;

            CopyEventSelector.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
            CopyEventSelector.Filter.EventType = "Class";

            var returnUrl = SearchUrl;

            if (Request.QueryString["action"] == "duplicate")
            {
                CreationType.ValueAsEnum = CreationTypeEnum.Duplicate;

                if (Guid.TryParse(Request.QueryString["event"], out var eventId))
                {
                    CopyEventSelector.Value = eventId;
                    returnUrl = OutlineUrl + $"?event={eventId}&panel=class";
                }
            }

            LoadDurationUnit(OneEventDurationUnit);
            LoadDurationUnit(CopyEventDurationUnit);

            OnCreationTypeSelected();
            OnCopyEventSelectorSelectedIndexChanged();

            CancelButton.NavigateUrl = returnUrl;
        }

        private static void LoadDurationUnit(ComboBox cmb)
        {
            cmb.Items.Add(new ComboBoxOption("Minute", "Minute"));
            cmb.Items.Add(new ComboBoxOption("Hour", "Hour"));
            cmb.Items.Add(new ComboBoxOption("Day", "Day"));
            cmb.Items.Add(new ComboBoxOption("Week", "Week"));
            cmb.Items.Add(new ComboBoxOption("Month", "Month"));
            cmb.Items.Add(new ComboBoxOption("Year", "Year"));
        }

        #endregion

        #region Methods (save one)

        private void SaveOne()
        {
            var aggregate = new EventAggregate { AggregateIdentifier = UniqueIdentifier.Create() };
            var changes = CreateOneChanges();

            for (var i = 0; i < changes.Count; i++)
            {
                var change = changes[i];
                change.AggregateIdentifier = aggregate.AggregateIdentifier;
                change.AggregateVersion = i + 1;
                change.OriginOrganization = Organization.Identifier;
                change.OriginUser = User.UserIdentifier;
                change.ChangeTime = DateTimeOffset.Now;
            }

            ServiceLocator.ChangeStore.Save(aggregate, changes);

            for (int i = 0; i < changes.Count; i++)
                ServiceLocator.ChangeQueue.Publish(changes[i]);

            HttpResponseHelper.Redirect(OutlineUrl + $"?event={aggregate.AggregateIdentifier}");
        }

        private List<IChange> CreateOneChanges()
        {
            var number = Sequence.Increment(Organization.OrganizationIdentifier, SequenceType.Event);
            var start = OneEventScheduledStart.Value.Value;
            var end = OneEventScheduledEnd.Value.Value;
            var (duration, durationUnit) = CalcDuration(start, end, OneEventDuration.ValueAsInt, OneEventDurationUnit.Value);
            var changes = new List<IChange>();

            changes.Add(new ClassScheduled2(Organization.OrganizationIdentifier, OneEventTitle.Text, "Active", number, start, end, duration, durationUnit, null));

            if (OneAchievementIdentifier.HasValue)
                changes.Add(new EventAchievementAdded(OneAchievementIdentifier.Value.Value));

            if (OneEventSummary.Text.HasValue())
                changes.Add(new EventDescribed(
                    new MultilingualString { Default = OneEventTitle.Text },
                    new MultilingualString { Default = OneEventSummary.Text },
                    null, null, null, null));

            changes.Add(new EventCalendarColorModified("primary"));

            if (OneVenueLocationGroup.HasValue)
            {
                var venue = ServiceLocator.GroupSearch.GetGroup(OneVenueLocationGroup.Value.Value);
                changes.Add(new EventVenueChanged2(venue.GroupIdentifier, venue.GroupIdentifier, OneVenueRoom.Text));
            }

            return changes;
        }

        #endregion

        #region Methods (save copy)

        private void SaveCopy()
        {
            var id = UniqueIdentifier.Create();
            var commands = CreateCopyCommands(id);

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            CopyGroups(id);

            HttpResponseHelper.Redirect(OutlineUrl + $"?event={id}");
        }

        private void CopyGroups(Guid id)
        {
            var groupPermissions = TGroupPermissionSearch
            .Bind(
                x => new
                {
                    x.Group.GroupIdentifier
                },
                x => x.ObjectIdentifier == CopyEventSelector.Value.Value
            )
            .Select(x => x.GroupIdentifier)
            .ToList();

            TGroupPermissionStore.Update(DateTimeOffset.UtcNow, User.UserIdentifier, id, "Event", groupPermissions, null);
        }

        private List<Command> CreateCopyCommands(Guid id)
        {
            var @event = ServiceLocator.EventSearch.GetEvent(CopyEventSelector.Value.Value);
            var number = Sequence.Increment(Organization.OrganizationIdentifier, SequenceType.Event);
            var start = CopyEventScheduledStart.Value.Value;
            var end = CopyEventScheduledEnd.Value.Value;

            var (duration, durationUnit) = CopyDurationCheckbox.Checked && @event.DurationQuantity.HasValue
                ? (@event.DurationQuantity.Value, @event.DurationUnit)
                : CalcDuration(start, end, CopyEventDuration.ValueAsInt, CopyEventDurationUnit.Value);

            var commands = new List<Command>
            {
                new ScheduleClass(id, Organization.OrganizationIdentifier, CopyEventTitle.Text, "Active", number, start, end, duration, durationUnit, @event.CreditHours)
            };

            if (@event.AchievementIdentifier.HasValue)
                commands.Add(new AddEventAchievement(id, @event.AchievementIdentifier.Value));

            if (@event.VenueLocationIdentifier.HasValue)
                commands.Add(new ChangeEventVenue(id, @event.VenueLocationIdentifier.Value, @event.VenueLocationIdentifier.Value, @event.VenueRoom));

            commands.Add(new ModifyEventCalendarColor(id, @event.EventCalendarColor ?? "Primary"));

            var waitlist = @event.WaitlistEnabled ? ToggleType.Enabled : ToggleType.Disabled;
            commands.Add(new AdjustCandidateCapacity(id, @event.CapacityMinimum, @event.CapacityMaximum, waitlist));

            if (@event.AllowMultipleRegistrations)
                commands.Add(new ModifyAllowMultipleRegistrations(id, @event.AllowMultipleRegistrations));

            if (@event.PersonCodeIsRequired)
                commands.Add(new ModifyPersonCodeIsRequired(id, @event.PersonCodeIsRequired));

            AddDescription(@event, id, CopyEventTitle.Text, commands);
            AddInstructors(@event, id, commands);
            AddSeats(@event, id, commands);
            AddGradebooks(@event, id, commands);
            AddNotifications(@event, id, commands);

            var registrationFields = @event.GetRegistrationFields();
            foreach(var field in registrationFields)
                commands.Add(new ModifyRegistrationField(id, field.Clone()));

            return commands;
        }

        #endregion

        #region Methods (save upload)

        private void SaveUpload()
        {
            try
            {
                var json = UploadJsonInput.Text;
                var qClassEventId = UniqueIdentifier.Create();
                var commands = CreateUploadCommands(qClassEventId, json);

                foreach (var command in commands)
                    ServiceLocator.SendCommand(command);

                HttpResponseHelper.Redirect(OutlineUrl + $"?event={qClassEventId}");
            }
            catch (JsonReaderException ex)
            {
                ScreenStatus.AddMessage(AlertType.Error, $"Your uploaded file has an unexpected format. {ex.Message}");
            }
            catch (Exception ex)
            {
                ScreenStatus.AddMessage(AlertType.Error, "Error during import: " + ex.Message);
            }
        }

        private static bool MessageExists(Guid messageId)
        {
            var message = ServiceLocator.MessageSearch.GetMessage(messageId);
            return
                message != null
                && message.OrganizationIdentifier == Organization.OrganizationIdentifier
                && string.Equals(message.MessageType, MessageTypeName.Notification, StringComparison.OrdinalIgnoreCase);
        }

        private List<Command> CreateUploadCommands(Guid newId, string json)
        {
            var result = ClassEventHelper.Deserialize(json, MessageExists, out var privacyGroups);

            if (!string.IsNullOrEmpty(json) && result == null)
            {
                ScreenStatus.AddMessage(AlertType.Error, "Wrong JSON file uploaded");
                return null;
            }

            var number = Sequence.Increment(Organization.OrganizationIdentifier, SequenceType.Event);
            var start = result.EventScheduledStart;
            var end = result.EventScheduledEnd.Value;

            var duration = (new DateTime(end.UtcDateTime.Year, end.UtcDateTime.Month, end.UtcDateTime.Day) - new DateTime(start.UtcDateTime.Year, start.UtcDateTime.Month, start.UtcDateTime.Day)).TotalDays + 1;

            var commands = new List<Command>();
            commands.Add(new ScheduleClass(newId, Organization.OrganizationIdentifier, result.EventTitle, "Active", number, start, end, (int)duration, "Day", result.CreditHours));

            if (result.AchievementIdentifier.HasValue)
                commands.Add(new AddEventAchievement(newId, result.AchievementIdentifier.Value));

            if (result.VenueLocationIdentifier.HasValue)
                commands.Add(new ChangeEventVenue(newId, result.VenueLocationIdentifier.Value, result.VenueLocationIdentifier.Value, result.VenueRoom));

            var waitlist = result.WaitlistEnabled ? ToggleType.Enabled : ToggleType.Disabled;
            commands.Add(new AdjustCandidateCapacity(newId, result.CapacityMinimum, result.CapacityMaximum, waitlist));

            AddDescription(result, newId, null, commands);
            CreateSeats(result, newId, commands);
            AddNotifications(result, newId, commands);

            var registrationFields = result.GetRegistrationFields();
            foreach (var field in registrationFields)
                commands.Add(new ModifyRegistrationField(newId, field.Clone()));

            TGroupPermissionStore.Update(DateTimeOffset.UtcNow, User.UserIdentifier, newId, "Event", privacyGroups, null);

            return commands;
        }

        #endregion

        #region Methods (save common)

        private static (int, string) CalcDuration(DateTimeOffset start, DateTimeOffset end, int? duration, string durationUnit)
        {
            if (duration.HasValue)
                return (duration.Value, durationUnit);

            var durationFromDates = (int)(
                    new DateTime(end.UtcDateTime.Year, end.UtcDateTime.Month, end.UtcDateTime.Day)
                    -
                    new DateTime(start.UtcDateTime.Year, start.UtcDateTime.Month, start.UtcDateTime.Day)
                ).TotalDays + 1;

            return (durationFromDates, "Day");
        }

        private void AddDescription(QEvent @event, Guid id, string title, List<Command> commands)
        {
            if (!@event.Content.HasValue())
                return;

            var content = ContentEventClass.Deserialize(@event.Content);
            var instructions = new[]
            {
                new EventInstruction
                {
                    Type = EventInstructionType.Contact,
                    Text = content.Get(EventInstructionType.Contact.GetName())
                },
                new EventInstruction
                {
                    Type = EventInstructionType.Accommodation,
                    Text = content.Get(EventInstructionType.Accommodation.GetName())
                },
                new EventInstruction
                {
                    Type = EventInstructionType.Additional,
                    Text = content.Get(EventInstructionType.Additional.GetName())
                },
                new EventInstruction
                {
                    Type = EventInstructionType.Cancellation,
                    Text = content.Get(EventInstructionType.Cancellation.GetName())
                },
                new EventInstruction
                {
                    Type = EventInstructionType.Completion,
                    Text = content.Get(EventInstructionType.Completion.GetName())
                }
            };

            var titleMultiString = !string.IsNullOrEmpty(title)
                ? new MultilingualString { Default = title }
                : content.Title;

            commands.Add(new DescribeEvent(
                id,
                titleMultiString,
                content.Summary,
                content.Description,
                content.MaterialsForParticipation,
                instructions,
                content.ClassLink)
            );
        }

        private void AddInstructors(QEvent @event, Guid id, List<Command> commands)
        {
            var instructors = ServiceLocator.EventSearch.GetAttendees(@event.EventIdentifier, x => x.Person).Where(x => x.AttendeeRole == "Instructor").ToList();
            foreach (var instructor in instructors)
                commands.Add(new AddEventAttendee(id, instructor.UserIdentifier, "Instructor", false));
        }

        private void AddSeats(QEvent @event, Guid id, List<Command> commands)
        {
            var seats = ServiceLocator.EventSearch.GetSeats(@event.EventIdentifier);

            if (seats != null && seats.Count > 0)
            {
                foreach (var seat in seats)
                    commands.Add(new AddSeat(id, UniqueIdentifier.Create(), seat.Configuration, seat.Content, seat.IsAvailable, seat.IsTaxable, seat.OrderSequence, seat.SeatTitle));
            }
            else if (@event.Seats != null && @event.Seats.Count > 0)
            {
                foreach (var seat in @event.Seats)
                {
                    commands.Add(new AddSeat(id, UniqueIdentifier.Create(), seat.Configuration, seat.Content, seat.IsAvailable, seat.IsTaxable, seat.OrderSequence, seat.SeatTitle));
                }
            }
        }

        private void CreateSeats(QEvent @event, Guid id, List<Command> commands)
        {
            if (@event.Seats != null && @event.Seats.Count > 0)
            {
                foreach (var seat in @event.Seats)
                {
                    commands.Add(new AddSeat(id, UniqueIdentifier.Create(), seat.Configuration, seat.Content, seat.IsAvailable, seat.IsTaxable, seat.OrderSequence, seat.SeatTitle));
                }
            }
        }

        private void AddGradebooks(QEvent @event, Guid id, List<Command> commands)
        {
            if (CopyGradebooks.SelectedValue != "Yes")
                return;

            var gradebooks = ServiceLocator.RecordSearch.GetGradebooks(new QGradebookFilter { GradebookEventIdentifier = @event.EventIdentifier });

            if (gradebooks.Count == 0)
                return;

            var achievementIdentifier = @event.AchievementIdentifier;
            var index = 1;

            foreach (var gradebook in gradebooks)
            {
                var newGradebook = UniqueIdentifier.Create();
                var title = gradebooks.Count > 1 ? CopyEventTitle.Text + "-" + index : CopyEventTitle.Text;
                var type = gradebook.GradebookType.ToEnum<GradebookType>();

                commands.Add(new DuplicateGradebook(
                    newGradebook,
                    Organization.OrganizationIdentifier,
                    gradebook.GradebookIdentifier,
                    title,
                    type,
                    id,
                    achievementIdentifier,
                    gradebook.FrameworkIdentifier
                ));

                index++;
            }
        }

        private void AddNotifications(QEvent @event, Guid id, List<Command> commands)
        {
            if (@event.WhenEventReminderRequestedNotifyLearnerMessageIdentifier.HasValue)
                commands.Add(new ConnectEventMessage(id, EventMessageType.ReminderLearner, @event.WhenEventReminderRequestedNotifyLearnerMessageIdentifier));

            if (@event.WhenEventReminderRequestedNotifyInstructorMessageIdentifier.HasValue)
                commands.Add(new ConnectEventMessage(id, EventMessageType.ReminderInstructor, @event.WhenEventReminderRequestedNotifyInstructorMessageIdentifier));

            if (@event.SendReminderBeforeDays.HasValue)
                commands.Add(new ModifyEventMessagePeriod(id, @event.SendReminderBeforeDays));
        }

        #endregion
    }
}
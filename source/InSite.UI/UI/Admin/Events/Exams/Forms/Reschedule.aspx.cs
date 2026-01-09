using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Events.Exams.Forms
{
    public partial class Reschedule : AdminBasePage, IHasParentLinkParameters
    {
        private readonly RescheduleProperties Properties;

        public Reschedule()
        {
            Properties = new RescheduleProperties(ViewState);
        }

        #region UI Event Handling

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EventScheduledStartValidator.ServerValidate += EventScheduledStartValidator_ServerValidate;
            SaveButton.Click += SaveButton_Click;

            EventSchedulingStatus.Settings.CollectionName = CollectionName.Activities_Exams_Scheduling_Status;
            EventSchedulingStatus.Settings.OrganizationIdentifier = Organization.Key;
            EventSchedulingStatus.AutoPostBack = true;
            EventSchedulingStatus.ValueChanged += EventSchedulingStatus_ValueChanged;

            EventRequisitionStatus.Settings.CollectionName = CollectionName.Activities_Exams_Requisition_Status;
            EventRequisitionStatus.Settings.OrganizationIdentifier = Organization.Key;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write))
                NavigateToSearch();

            if (Properties.Event == null || Properties.Event.OrganizationIdentifier != Organization.OrganizationIdentifier)
                NavigateToSearch();

            if (!IsPostBack)
                Open();
        }

        private void EventScheduledStartValidator_ServerValidate(object source, ServerValidateEventArgs args)
            => args.IsValid = EventScheduledStart.Value > DateTimeOffset.UtcNow;

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Save();
        }

        private void EventSchedulingStatus_ValueChanged(object sender, ComboBoxValueChangedEventArgs e)
        {
            SaveButton.ConfirmText = e.NewValue == "Cancelled" ? "After an exam event is cancelled you cannot modify its scheduling status. Are you sure you want to cancel this exam event?" : null;
        }

        #endregion

        #region UI Navigation

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/outline") ? $"event={Properties.EventIdentifier}" : null;

        private void NavigateToSearch()
            => HttpResponseHelper.Redirect(Properties.SearchUrl, true);

        #endregion

        #region Data Binding

        private void Open()
        {
            BackButton.NavigateUrl = Properties.OutlineUrl;

            PageHelper.AutoBindHeader(this);

            ExamInfoSummary.LoadData(Properties.Event, Properties.Event.VenueLocation, showAchievement: false, showSchedule: false);

            EventScheduledStart.Value = Properties.Event.EventScheduledStart;
            Duration.ValueAsInt = Properties.Event.DurationQuantity;
            DurationUnit.Value = Properties.Event.DurationUnit;
            EventFormat.Value = Properties.Event.EventFormat;
            EventSchedulingStatus.Value = Properties.Event.EventSchedulingStatus;
            EventRequisitionStatus.Value = Properties.Event.EventRequisitionStatus;
            CapacityMaximum.ValueAsInt = Properties.Event.CapacityMaximum;
            InvigilatorMinimum.ValueAsInt = Properties.Event.InvigilatorMinimum;

            CheckStatus();
            CheckCapacity();
        }

        private void CheckCapacity()
        {
            var filter = new QEventAttendeeFilter
            {
                EventIdentifier = Properties.Event.EventIdentifier
            };
            var personCount = ServiceLocator.EventSearch.CountAttendees(filter);

            var isFull = personCount > 0
                         && CapacityMaximum.ValueAsInt.HasValue
                         && CapacityMaximum.ValueAsInt.Value <= personCount;

            FullLabel.Visible = isFull;
        }

        private void CheckStatus()
        {
            var isCancelled = Properties.Event.EventSchedulingStatus == "Cancelled";
            EventSchedulingStatus.Enabled = !isCancelled;
            SaveButton.Visible = !isCancelled;
        }

        #endregion

        #region Command Sending

        private void Save()
        {
            if (!Page.IsValid)
                return;

            if (Properties.Event.ExamType == EventExamType.Class.Value)
            {
                var status = EventSchedulingStatus.Value;
                if (status.IsNotEmpty() && Properties.Event.EventSchedulingStatus != status && status.StartsWith("Approved"))
                {
                    var tpCount = ServiceLocator.EventSearch.CountAttendees(new QEventAttendeeFilter
                    {
                        EventIdentifier = Properties.EventIdentifier.Value,
                        ContactRole = "Training Provider"
                    });

                    if (tpCount == 0)
                    {
                        EditorStatus.AddMessage(AlertType.Error, "A training provider contact has not been added to this event.");
                        return;
                    }
                }
            }

            try
            {
                var changed = DetectTimeChange();
                changed = DetectFormatChange() || changed;
                changed = DetectRequestStatusChange() || changed;
                changed = DetectScheduleStatusChange() || changed;
                changed = DetectCapacityChange() || changed;
                changed = DetectInvigilatorChange() || changed;

                HttpResponseHelper.Redirect(BackButton.NavigateUrl);
            }
            catch (WebServiceUnavailableException)
            {
                EditorStatus.AddMessage(AlertType.Error, "The Direct Access web service is unavailable. Your latest change to this exam event is <b>not</b> published to Direct Access.");
            }
        }

        private bool DetectTimeChange()
        {
            var before = Properties.Event;
            var id = before.EventIdentifier;
            var start = EventScheduledStart.Value.Value;
            var duration = Duration.ValueAsInt.Value;
            var durationUnit = DurationUnit.Value;
            var end = Properties.CalculateEndTime(start, duration, durationUnit);
            var changed = false;

            if (before.EventScheduledStart != start || before.EventScheduledEnd != end)
            {
                ServiceLocator.SendCommand(new RescheduleEvent(id, start, end));
                changed = true;
            }

            if (before.DurationQuantity != duration || before.DurationUnit != durationUnit)
            {
                ServiceLocator.SendCommand(new ChangeEventDuration(id, duration, durationUnit));
                changed = true;
            }

            return changed;
        }

        private bool DetectFormatChange()
        {
            var before = Properties.Event;
            var id = before.EventIdentifier;
            var format = EventFormat.Value;

            var formatChanged = before.EventFormat != format;
            if (formatChanged)
                ServiceLocator.SendCommand(new ChangeEventFormat(id, format));

            return formatChanged;
        }

        private bool DetectRequestStatusChange()
        {
            var before = Properties.Event;
            var id = before.EventIdentifier;
            var status = EventRequisitionStatus.Value;

            var statusChanged = before.EventRequisitionStatus != status;

            if (statusChanged)
                ServiceLocator.SendCommand(new ChangeEventStatus(id, status, null));

            return statusChanged;
        }

        private bool DetectScheduleStatusChange()
        {
            var before = Properties.Event;
            var id = before.EventIdentifier;
            var status = EventSchedulingStatus.Value;

            var statusChanged = before.EventSchedulingStatus != status;

            if (statusChanged)
            {
                ServiceLocator.SendCommand(new ChangeEventStatus(id, null, status));

                if (status == "Ready to Schedule" || status.StartsWith("Approved"))
                    ServiceLocator.SendCommand(new StartEventPublication(id));

                else if (status == "Completed")
                    ServiceLocator.SendCommand(new CompleteEvent(id));

                else if (status == "Cancelled")
                    ServiceLocator.SendCommand(new Application.Events.Write.CancelEvent(id, $"Cancelled by {User.FullName}", true));
            }

            return statusChanged;
        }

        private bool DetectCapacityChange()
        {
            var before = Properties.Event;
            var id = before.EventIdentifier;
            var newCapacityMaximum = !before.ExamType.StartsWith("Individual") && CapacityMaximum.ValueAsInt.HasValue ? CapacityMaximum.ValueAsInt.Value : 1;

            var capacityMaximumChanged = before.CapacityMaximum != newCapacityMaximum;
            if (capacityMaximumChanged)
                ServiceLocator.SendCommand(new AdjustCandidateCapacity(id, null, newCapacityMaximum, ToggleType.Disabled));
            return capacityMaximumChanged;
        }

        private bool DetectInvigilatorChange()
        {
            var before = Properties.Event;
            var id = before.EventIdentifier;
            var newInvigilatorMinimum = InvigilatorMinimum.ValueAsInt ?? 0;

            var invigilatorMinimumChanged = before.InvigilatorMinimum != newInvigilatorMinimum;
            if (invigilatorMinimumChanged)
                ServiceLocator.SendCommand(new AdjustInvigilatorCapacity(id, newInvigilatorMinimum, null));
            return invigilatorMinimumChanged;
        }

        #endregion
    }
}

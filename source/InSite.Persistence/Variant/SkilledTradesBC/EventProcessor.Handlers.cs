using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Domain.Events;
using InSite.Domain.Messages;
using InSite.Persistence.Integration.BCMail;

using Shift.Common;

namespace InSite.Persistence.Plugin.SkilledTradesBC
{
    /// <summary>
    /// A processor (or process manager) has side effects.
    /// </summary>
    public partial class EventProcessor
    {
        public void Handle(ExamScheduled2 e)
        {
            if (e.Format == EventExamFormat.Paper.Value)
            {
                var when = QEvent.GetDefaultDistributionExpected(e.StartTime);
                var change = new ChangeDistribution(e.AggregateIdentifier, "Standard", null, when, null, null);
                _broker.Send(e, change);
            }

            ScheduleNoReturnShipmentReceived(e, e.AggregateIdentifier);
        }

        public void Handle(EventFormatChanged e)
        {
            var @event = _events.GetEvent(e.AggregateIdentifier);
            var command = e.Format == EventExamFormat.Paper.Value
                ? new ChangeDistribution(e.AggregateIdentifier, "Standard", null, QEvent.GetDefaultDistributionExpected(@event.EventScheduledStart), null, @event.ExamStarted)
                : new ChangeDistribution(e.AggregateIdentifier, null, null, null, null, @event.ExamStarted);

            _broker.Send(e, command);

            ReScheduleNoReturnShipmentReceived(e, @event);
        }

        public void Handle(EventRescheduled e)
        {
            ReScheduleNoReturnShipmentReceived(e, e.AggregateIdentifier);
        }

        public void Handle(EventScheduleStatusChanged e)
        {
            QEvent @event = _events.GetEvent(e.AggregateIdentifier);

            if (@event.EventSchedulingStatus == "Cancelled")
                Cancelled(e, @event.EventIdentifier);

            else if (EnableIntegrationWithDirectAccess(e.AggregateIdentifier, false))
            {
                if (@event.EventSchedulingStatus.StartsWith("Approved"))
                    Approved(e, @event);

                else if (@event.EventSchedulingStatus == "Completed")
                    Completed(e, @event.EventIdentifier);
            }

            ReScheduleNoReturnShipmentReceived(e, @event);
        }

        public void Handle(EventVenueChanged2 e)
        {
            ReScheduleNoReturnShipmentReceived(e, e.AggregateIdentifier);
        }

        public void Handle(EventCancelled e)
        {
            ReScheduleNoReturnShipmentReceived(e, e.AggregateIdentifier);
        }

        public void Handle(EventNotificationTriggered e)
        {
            if (AbortNotification(e))
                return;

            var type = Notifications.Select(e.Name);

            ExecuteMailout(e, e.AggregateIdentifier, type);
        }

        private bool AbortNotification(EventNotificationTriggered e)
        {
            if (!EnableIntegrationWithDirectAccess(e.AggregateIdentifier, false))
            {
                Comment(e, $"Notification {e.Name} will not be sent because integration with Direct Access is disabled for this event.");
                return true;
            }

            return false;
        }

        public void Handle(EventPublicationStarted e)
        {
            string status = null;
            string errors = null;

            if (EnableIntegrationWithDirectAccess(e.AggregateIdentifier, false))
                UpdateEventInDirectAccess(e, out status, out errors);

            _broker.Send(e, new CompleteEventPublication(e.AggregateIdentifier, status ?? "Cancelled", errors));
        }

        public void Handle(EventPublicationCompleted e) { }

        public void Handle(EventScoresValidated e)
        {
            ValidateExamResults(e);
        }

        public void Handle(EventScoresPublished e)
        {
            if (EnableIntegrationWithDirectAccess(e.AggregateIdentifier, true))
                SubmitExamResultsToDirectAccess(e);
        }

        public void Handle(DistributionTracked e)
        {
            var request = new ExamDistributionRequest
            {
                RequestIdentifier = UniqueIdentifier.Create(),
                Requested = e.ChangeTime,
                RequestedBy = e.OriginUser,
                JobCode = e.Job,
                JobStatus = e.Status,
                JobErrors = e.Errors
            };
            ExamDistributionRequestStore.Insert(request);
        }

        public void Handle(ExamMaterialReturned e)
        {
            // If a date is now selected for the Return Shipment Received (and there was no input here before) and if 
            // the event format is "Paper" and if the shipment condition is not "Full" then start two new Event 
            // Notification timers: one that elapses after 5 business days, and one that elapses after 10 business days.

            var isShipmentReceived = e.ReturnShipmentDate.HasValue;
            if (!isShipmentReceived)
                return;

            var isShipmentComplete = e.ReturnShipmentCondition == "Full";
            if (!isShipmentComplete)
                ScheduleShipmentReminders(e, e.AggregateIdentifier);
            else
                CancelEventAlertTimer(e, e.AggregateIdentifier, NotificationType.ITA017);

            CancelEventAlertTimer(e, e.AggregateIdentifier, NotificationType.ITA026);
        }
    }
}

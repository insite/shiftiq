using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Events
{
    public class EventAggregate : AggregateRoot
    {
        public override AggregateState CreateState() => new EventState();

        public EventState Data => (EventState)State;

        public void AddEventAssessment(Guid form)
        {
            var e = new ExamFormAttached(form);
            Apply(e);
        }

        public void AddEventAttendee(Guid contact, string role, bool validate)
        {
            if (Data.Contacts.Contains(contact))
                return;

            if (validate && role == "Exam Candidate")
            {
                var cutoff = Data.EventTime.AddHours(1);
                if (cutoff < DateTimeOffset.UtcNow)
                {
                    throw new ExamCandidateNotAllowedException(cutoff);
                }
            }

            var e = new EventAttendeeAdded(contact, role);
            Apply(e);
        }

        public void AddEventAchievement(Guid achievement)
        {
            var e = new EventAchievementAdded(achievement);
            Apply(e);
        }

        public void AdjustCandidateCapacity(int? min, int? max, ToggleType waitlist)
        {
            var e = new CapacityAdjusted(min, max, waitlist);
            Apply(e);
        }

        public void AdjustInvigilatorCapacity(int? min, int? max)
            => Apply(new InvigilatorCapacityAdjusted(min, max));

        public void CancelEvent(string reason, bool cancelRegistration)
        {
            var e = new EventCancelled(reason, cancelRegistration);
            Apply(e);
        }

        public void CancelEventTimer(Guid timer)
        {
            var e = new EventTimerCancelled(timer);
            Apply(e);
        }

        public void ChangeEventFormat(string format)
        {
            if (Data.EventFormat == format)
                return;

            Apply(new EventFormatChanged(format));
        }

        public void ChangeEventCalendarColor(string calendarColor)
            => Apply(new EventCalendarColorModified(calendarColor));

        public void ChangeRequestStatus(string status)
            => Apply(new EventRequestStatusChanged(status));

        public void ChangeScheduleStatus(string status)
            => Apply(new EventScheduleStatusChanged(status));

        public void ChangeExamType(string type)
        {
            var e = new ExamTypeChanged(type);
            Apply(e);
        }

        public void ChangeEventAchievement(Guid? achievement)
        {
            Apply(new EventAchievementChanged(achievement));
        }

        public void ChangeEventVenue(Guid? office, Guid? location, string room)
            => Apply(new EventVenueChanged2(office, location, room));

        public void ChangeDistribution(string process, DateTimeOffset? ordered, DateTimeOffset? expected, DateTimeOffset? shipped, DateTimeOffset? started)
        {
            var e = new DistributionChanged(process, ordered, expected, shipped, started);
            Apply(e);
        }

        public void CompleteEvent()
        {
            var e = new EventCompleted();
            Apply(e);
        }

        public void CompleteEventPublication(string status, string errors)
        {
            var e = new EventPublicationCompleted(status, errors);
            Apply(e);
        }

        public void ConfigureIntegration(bool withholdGrades, bool withholdDistribution)
            => Apply(new IntegrationConfigured(withholdGrades, withholdDistribution));

        public void DescribeEvent(MultilingualString title, MultilingualString summary, MultilingualString description, MultilingualString materialsForParticipation, EventInstruction[] instructions, MultilingualString classLink)
        {
            var e = new EventDescribed(title, summary, description, materialsForParticipation, instructions, classLink);
            Apply(e);
        }

        public void DescribeAppointment(MultilingualString title, MultilingualString description)
        {
            var e = new AppointmentDescribed(title, description);
            Apply(e);
        }

        public void ElapseEventTimer(Guid timer)
        {
            var e = new EventTimerElapsed(timer);
            Apply(e);
        }

        public void EnableEventBillingCode(bool enabled)
        {
            if (Data.BillingCodeEnabled == enabled)
                return;

            var e = new EventBillingCodeEnabled(enabled);
            Apply(e);
        }

        public void ImportExamAttempts(bool allowDuplicates)
        {
            var e = new ExamAttemptsImported(allowDuplicates);
            Apply(e);
        }

        public void OrderDistribution(string code, string status, string errors)
        {
            var e = new DistributionOrdered(code, status, errors);
            Apply(e);
        }

        public void PostComment(Guid comment, Guid author, string text)
        {
            var e = new EventCommentPosted(comment, author, text);
            Apply(e);
        }

        public void PublishEventScores(Guid[] registrations, bool alertMessageEnabled)
        {
            Apply(new EventScoresPublished(registrations, alertMessageEnabled));
        }

        public void ValidateEventScores(Guid[] registrations)
        {
            Apply(new EventScoresValidated(registrations));
        }

        public void RecodeEvent(string classCode, string billingCode)
        {
            var e = new EventRecoded(classCode, billingCode);
            Apply(e);
        }

        public void DeleteEvent()
        {
            var e = new EventDeleted();
            Apply(e);
        }

        public void AllowEventRegistrationWithLink()
        {
            var e = new EventRegistrationWithLinkAllowed();
            Apply(e);
        }

        public void RemoveEventAssessment(Guid form)
        {
            var e = new ExamFormDetached(form);
            Apply(e);
        }

        public void RemoveEventAttendee(Guid contact)
        {
            bool isCandidate = Data.Candidates.Contains(contact);

            var e = new EventAttendeeRemoved(contact, isCandidate ? "Exam Candidate" : null);
            Apply(e);
        }

        public void RemoveComment(Guid comment)
        {
            var e = new EventCommentDeleted(comment);
            Apply(e);
        }

        public void RescheduleEvent(DateTimeOffset startTime, DateTimeOffset endTime)
        {
            Apply(new EventRescheduled(startTime, endTime));
        }

        public void ChangeEventDuration(int? duration, string unit)
        {
            var e = new EventDurationChanged(duration, unit);
            Apply(e);
        }

        public void ChangeEventCreditHours(decimal? credit)
        {
            var e = new EventCreditHoursChanged(credit);
            Apply(e);
        }

        public void RenumberEvent(int number)
        {
            var e = new EventRenumbered(number);
            Apply(e);
        }

        public void RetitleEvent(string title)
        {
            var e = new EventRetitled(title);
            Apply(e);
        }

        public void ReturnDistribution()
        {
            var e = new DistributionReturned();
            Apply(e);
        }

        public void ReviseComment(Guid comment, Guid author, string text)
        {
            var e = new EventCommentModified(comment, author, text);
            Apply(e);
        }

        public void ScheduleClass(Guid organization, string title, string status, int number, DateTimeOffset start, DateTimeOffset end, int duration, string durationUnit, decimal? credit)
        {
            var e = new ClassScheduled2(organization, title, status, number, start, end, duration, durationUnit, credit);
            Apply(e);
        }

        public void ScheduleAppointment(Guid organization, string title, string appointmentType, string description, DateTimeOffset start, DateTimeOffset end)
        {
            var e = new AppointmentScheduled(organization, title, appointmentType, description, start, end);
            Apply(e);
        }

        public void ScheduleExam(Guid organization, string type, string format, string title, string status, string billingCode, string classCode, string source, int duration, int number, DateTimeOffset start, Guid venueIdentifier, string venueRoom, int? capacityMaximum)
        {
            Apply(new ExamScheduled2(organization, type, format, title, status, billingCode, classCode, source, duration, number, start));

            if (venueIdentifier != Guid.Empty || venueRoom != null)
                Apply(new EventVenueChanged2(venueIdentifier, venueIdentifier, venueRoom));

            if ((capacityMaximum ?? 0) > 0)
                Apply(new CapacityAdjusted(null, capacityMaximum, ToggleType.Disabled));
        }

        public void ShipDistribution()
        {
            var e = new DistributionShipped();
            Apply(e);
        }

        public void StartEventPublication()
        {
            var e = new EventPublicationStarted();
            Apply(e);
        }

        public void StartEventTimer(Guid timer, DateTimeOffset at, string description)
        {
            var e = new EventTimerStarted(timer, at, description);
            Apply(e);
        }

        public void TrackDistribution(string job, string status, string errors)
        {
            var e = new DistributionTracked(job, status, errors);
            Apply(e);
        }

        public void TriggerEventNotification(string name)
        {
            var e = new EventNotificationTriggered(name);
            Apply(e);
        }

        public void PublishEvent(DateTimeOffset? registrationStart, DateTimeOffset? registrationDeadline)
        {
            var e = new EventPublished(registrationStart, registrationDeadline);
            Apply(e);
        }

        public void UnpublishEvent()
        {
            var e = new EventUnpublished();
            Apply(e);
        }

        public void AddSeat(Guid seat, string configuration, string content, bool isAvailable, bool isTaxable, int? orderSequence, string title)
        {
            var e = new SeatAdded(seat, configuration, content, isAvailable, isTaxable, orderSequence, title);
            Apply(e);
        }

        public void ReviseSeat(Guid seat, string configuration, string content, bool isAvailable, bool isTaxable, int? orderSequence, string title)
        {
            var e = new SeatRevised(seat, configuration, content, isAvailable, isTaxable, orderSequence, title);
            Apply(e);
        }

        public void DeleteSeat(Guid seat)
        {
            var e = new SeatDeleted(seat);
            Apply(e);
        }

        public void AddLearnerRegistrationGroup(Guid? learnerRegistrationGroup)
        {
            var e = new LearnerRegistrationGroupModified(learnerRegistrationGroup);
            Apply(e);
        }

        public void ReturnExamMaterial(string code, DateTimeOffset? received, string condition)
        {
            if (Data.Exam.MaterialReturnShipmentCode == code
             && Data.Exam.MaterialReturnShipmentReceived == received
             && Data.Exam.MaterialReturnShipmentCondition == condition)
                return;

            Apply(new ExamMaterialReturned(code, received, condition));
        }

        public void ChangeAppointmentType(string appointmentType)
        {
            if (Data.AppointmentType == appointmentType)
                return;

            Apply(new AppointmentTypeChanged(appointmentType));
        }

        public void LockEventRegistration()
        {
            if (Data.RegistrationLocked.HasValue)
                return;

            Apply(new EventRegistrationLocked(DateTimeOffset.UtcNow));
        }

        public void UnlockEventRegistration()
        {
            if (Data.RegistrationLocked == null)
                return;

            Apply(new EventRegistrationUnlocked());
        }

        public void ModifyAllowMultipleRegistrations(bool value)
        {
            if (Data.AllowMultipleRegistrations == value)
                return;

            Apply(new EventAllowMultipleRegistrationsModified(value));
        }

        public void ModifyPersonCodeIsRequired(bool value)
        {
            if (Data.PersonCodeIsRequired == value)
                return;

            Apply(new EventPersonCodeIsRequiredModified(value));
        }

        public void ModifyRegistrationField(RegistrationField field)
        {
            var f = Data.RegistrationFields.GetOrDefault(field.FieldName);
            if (f != null && f.IsRequired == field.IsRequired && f.IsVisible == field.IsVisible && f.IsEditable == field.IsEditable)
                return;

            var e = new RegistrationFieldModified(field);
            Apply(e);
        }

        public void ModifyMandatorySurvey(Guid? surveyForm)
        {
            if (Data.MandatorySurveyFormIdentifier == surveyForm)
                return;

            var e = new MandatorySurveyModified(surveyForm);
            Apply(e);
        }

        public void ConnectEventMessage(EventMessageType messageType, Guid? messageId)
        {
            switch (messageType)
            {
                case EventMessageType.ReminderLearner:
                    if (messageId == Data.WhenEventReminderRequestedNotifyLearnerMessageIdentifier)
                        return;
                    break;
                case EventMessageType.ReminderInstructor:
                    if (messageId == Data.WhenEventReminderRequestedNotifyInstructorMessageIdentifier)
                        return;
                    break;
                default:
                    throw new ArgumentException($"Unsupported message: {messageType}");
            }

            Apply(new EventMessageConnected(messageType, messageId));
        }

        public void ModifyEventMessagePeriod(int? sendReminderBeforeDays)
        {
            if (Data.SendReminderBeforeDays == sendReminderBeforeDays)
                return;

            Apply(new EventMessagePeriodModified(sendReminderBeforeDays));
        }

        public void SendEventMessage(EventMessageType messageType, Guid messageId, Guid[] recipients)
        {
            Apply(new EventMessageSent(messageType, messageId, recipients));
        }
    }
}
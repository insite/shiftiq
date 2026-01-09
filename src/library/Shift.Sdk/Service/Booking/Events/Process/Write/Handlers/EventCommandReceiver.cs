using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Domain.Events;

namespace InSite.Application.Events.Write
{
    public class EventCommandReceiver
    {
        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;

        public EventCommandReceiver(ICommandQueue commander, IChangeQueue publisher, IChangeRepository repository)
        {
            _publisher = publisher;
            _repository = repository;

            commander.Subscribe<AddEventAssessment>(Handle);
            commander.Subscribe<AddEventAttendee>(Handle);
            commander.Subscribe<AddEventAchievement>(Handle);
            commander.Subscribe<AdjustCandidateCapacity>(Handle);
            commander.Subscribe<AdjustInvigilatorCapacity>(Handle);
            commander.Subscribe<CancelEvent>(Handle);
            commander.Subscribe<CancelEventTimer>(Handle);
            commander.Subscribe<ChangeEventFormat>(Handle);
            commander.Subscribe<ChangeEventStatus>(Handle);
            commander.Subscribe<ChangeEventAchievement>(Handle);
            commander.Subscribe<ChangeEventVenue>(Handle);
            commander.Subscribe<ChangeDistribution>(Handle);
            commander.Subscribe<ChangeExamType>(Handle);
            commander.Subscribe<CompleteEvent>(Handle);
            commander.Subscribe<CompleteEventPublication>(Handle);
            commander.Subscribe<ConfigureIntegration>(Handle);
            commander.Subscribe<DescribeAppointment>(Handle);
            commander.Subscribe<DescribeEvent>(Handle);
            commander.Subscribe<ElapseEventTimer>(Handle);
            commander.Subscribe<EnableEventBillingCode>(Handle);
            commander.Subscribe<ImportExamAttempts>(Handle);
            commander.Subscribe<PostComment>(Handle);
            commander.Subscribe<PublishEventScores>(Handle);
            commander.Subscribe<ValidateEventScores>(Handle);
            commander.Subscribe<RecodeEvent>(Handle);
            commander.Subscribe<DeleteEvent>(Handle);
            commander.Subscribe<AllowEventRegistrationWithLink>(Handle);
            commander.Subscribe<RemoveEventAssessment>(Handle);
            commander.Subscribe<RemoveEventAttendee>(Handle);
            commander.Subscribe<RemoveComment>(Handle);
            commander.Subscribe<RescheduleEvent>(Handle);
            commander.Subscribe<ChangeEventDuration>(Handle);
            commander.Subscribe<ChangeEventCreditHours>(Handle);
            commander.Subscribe<RenumberEvent>(Handle);
            commander.Subscribe<RetitleEvent>(Handle);
            commander.Subscribe<ReviseComment>(Handle);
            commander.Subscribe<ScheduleAppointment>(Handle);
            commander.Subscribe<ScheduleClass>(Handle);
            commander.Subscribe<ScheduleExam>(Handle);
            commander.Subscribe<StartEventPublication>(Handle);
            commander.Subscribe<StartEventTimer>(Handle);
            commander.Subscribe<TriggerEventNotification>(Handle);
            commander.Subscribe<PublishEvent>(Handle);
            commander.Subscribe<UnpublishEvent>(Handle);
            commander.Subscribe<AddSeat>(Handle);
            commander.Subscribe<ReviseSeat>(Handle);
            commander.Subscribe<DeleteSeat>(Handle);
            commander.Subscribe<ModifyLearnerRegistrationGroup>(Handle);
            commander.Subscribe<ReturnExamMaterial>(Handle);
            commander.Subscribe<ChangeAppointmentType>(Handle);
            commander.Subscribe<ModifyEventCalendarColor>(Handle);
            commander.Subscribe<LockEventRegistration>(Handle);
            commander.Subscribe<UnlockEventRegistration>(Handle);
            commander.Subscribe<ModifyAllowMultipleRegistrations>(Handle);
            commander.Subscribe<ModifyMandatorySurvey>(Handle);
            commander.Subscribe<ModifyPersonCodeIsRequired>(Handle);
            commander.Subscribe<ModifyRegistrationField>(Handle);
            commander.Subscribe<ConnectEventMessage>(Handle);
            commander.Subscribe<ModifyEventMessagePeriod>(Handle);
            commander.Subscribe<SendEventMessage>(Handle);
        }

        private void Commit(EventAggregate aggregate, ICommand c)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
            {
                change.AggregateState = aggregate.State;
                _publisher.Publish(change);
            }
        }

        public void Handle(AddEventAssessment c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AddEventAssessment(c.Form);
                Commit(aggregate, c);
            });
        }

        public void Handle(AddEventAttendee c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AddEventAttendee(c.ContactIdentifier, c.ContactRole, c.Validate);
                Commit(aggregate, c);
            });
        }

        public void Handle(AddEventAchievement c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AddEventAchievement(c.Achievement);
                Commit(aggregate, c);
            });
        }

        public void Handle(AdjustCandidateCapacity c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AdjustCandidateCapacity(c.Minimum, c.Maximum, c.Waitlist);
                Commit(aggregate, c);
            });
        }

        public void Handle(AdjustInvigilatorCapacity c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AdjustInvigilatorCapacity(c.Minimum, c.Maximum);
                Commit(aggregate, c);
            });
        }

        public void Handle(CancelEvent c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.CancelEvent(c.Reason, c.CancelRegistrations);
                Commit(aggregate, c);
            });
        }

        public void Handle(CancelEventTimer c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.CancelEventTimer(c.Timer);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeEventFormat c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeEventFormat(c.EventFormat);
                Commit(aggregate, c);
            });
        }

        public void Handle(ModifyEventCalendarColor c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeEventCalendarColor(c.CalendarColor);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeEventStatus c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                if (c.RequestStatus != null)
                    aggregate.ChangeRequestStatus(c.RequestStatus);

                if (c.ScheduleStatus != null)
                    aggregate.ChangeScheduleStatus(c.ScheduleStatus);

                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeEventVenue c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeEventVenue(c.Office, c.Location, c.Room);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeEventAchievement c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeEventAchievement(c.Achievement);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeDistribution c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeDistribution(c.DistributionProcess, c.DistributionOrdered, c.DistributionExpected, c.DistributionShipped, c.AttemptStarted);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeExamType c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeExamType(c.Type);
                Commit(aggregate, c);
            });
        }

        public void Handle(CompleteEvent c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.CompleteEvent();
                Commit(aggregate, c);
            });
        }

        public void Handle(CompleteEventPublication c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.CompleteEventPublication(c.Status, c.Errors);
                Commit(aggregate, c);
            });
        }

        public void Handle(ConfigureIntegration c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ConfigureIntegration(c.WithholdGrades, c.WithholdDistribution);
                Commit(aggregate, c);
            });
        }

        public void Handle(DescribeEvent c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.DescribeEvent(c.Title, c.Summary, c.Description, c.MaterialsForParticipation, c.Instructions, c.ClassLink);
                Commit(aggregate, c);
            });
        }

        public void Handle(DescribeAppointment c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.DescribeAppointment(c.Title, c.Description);
                Commit(aggregate, c);
            });
        }

        public void Handle(ElapseEventTimer c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ElapseEventTimer(c.Timer);
                Commit(aggregate, c);
            });
        }

        public void Handle(EnableEventBillingCode c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.EnableEventBillingCode(c.Enabled);
                Commit(aggregate, c);
            });
        }

        public void Handle(ImportExamAttempts c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ImportExamAttempts(c.AllowDuplicates);
                Commit(aggregate, c);
            });
        }

        public void Handle(PostComment c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.PostComment(c.CommentIdentifier, c.AuthorIdentifier, c.CommentText);
                Commit(aggregate, c);
            });
        }

        public void Handle(PublishEventScores c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.PublishEventScores(c.Registrations, c.AlertMessageEnabled);
                Commit(aggregate, c);
            });
        }

        public void Handle(ValidateEventScores c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ValidateEventScores(c.Registrations);
                Commit(aggregate, c);
            });
        }

        public void Handle(RecodeEvent c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.RecodeEvent(c.EventClassCode, c.EventBillingCode);
                Commit(aggregate, c);
            });
        }

        public void Handle(DeleteEvent c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.DeleteEvent();
                Commit(aggregate, c);
            });
        }

        public void Handle(AllowEventRegistrationWithLink c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AllowEventRegistrationWithLink();
                Commit(aggregate, c);
            });
        }

        public void Handle(RemoveEventAssessment c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.RemoveEventAssessment(c.FormIdentifier);
                Commit(aggregate, c);
            });
        }

        public void Handle(RemoveEventAttendee c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.RemoveEventAttendee(c.ContactIdentifier);
                Commit(aggregate, c);
            });
        }

        public void Handle(RemoveComment c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.RemoveComment(c.CommentIdentifier);
                Commit(aggregate, c);
            });
        }

        public void Handle(RescheduleEvent c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.RescheduleEvent(c.StartTime, c.EndTime);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeEventDuration c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeEventDuration(c.Duration, c.Unit);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeEventCreditHours c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeEventCreditHours(c.CreditHours);
                Commit(aggregate, c);
            });
        }

        public void Handle(RenumberEvent c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.RenumberEvent(c.Number);
                Commit(aggregate, c);
            });
        }

        public void Handle(RetitleEvent c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.RetitleEvent(c.EventTitle);
                Commit(aggregate, c);
            });
        }

        public void Handle(ReviseComment c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ReviseComment(c.CommentIdentifier, c.AuthorIdentifier, c.CommentText);
                Commit(aggregate, c);
            });
        }

        public void Handle(ScheduleAppointment c)
        {
            var aggregate = new EventAggregate { AggregateIdentifier = c.AggregateIdentifier };
            aggregate.ScheduleAppointment(c.Tenant, c.Title, c.AppointmentType, c.Description, c.StartTime, c.EndTime);
            Commit(aggregate, c);
        }

        public void Handle(ScheduleClass c)
        {
            var aggregate = new EventAggregate { AggregateIdentifier = c.AggregateIdentifier };
            aggregate.ScheduleClass(c.Tenant, c.Title, c.Status, c.Number, c.StartTime, c.EndTime, c.Duration, c.DurationUnit, c.Credit);
            Commit(aggregate, c);
        }

        public void Handle(ScheduleExam c)
        {
            var aggregate = new EventAggregate { AggregateIdentifier = c.AggregateIdentifier };
            aggregate.ScheduleExam(c.Tenant, c.ExamType, c.EventFormat, c.EventTitle, c.EventStatus, c.EventBillingCode, c.EventClassCode, c.EventSource, c.ExamDuration, c.EventNumber, c.EventStartTime, c.VenueIdentifier, c.VenueRoom, c.CapacityMaximum);
            Commit(aggregate, c);
        }

        public void Handle(StartEventPublication c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.StartEventPublication();
                Commit(aggregate, c);
            });
        }

        public void Handle(StartEventTimer c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.StartEventTimer(c.Timer, c.At, c.Description);
                Commit(aggregate, c);
            });
        }

        public void Handle(TriggerEventNotification c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.TriggerEventNotification(c.Name);
                Commit(aggregate, c);
            });
        }

        public void Handle(PublishEvent c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.PublishEvent(c.RegistrationStart, c.RegistrationDeadline);
                Commit(aggregate, c);
            });
        }

        public void Handle(UnpublishEvent c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.UnpublishEvent();
                Commit(aggregate, c);
            });
        }

        public void Handle(AddSeat c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AddSeat(c.Seat, c.Configuration, c.Content, c.IsAvailable, c.IsTaxable, c.OrderSequence, c.Title);
                Commit(aggregate, c);
            });
        }

        public void Handle(ReviseSeat c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ReviseSeat(c.Seat, c.Configuration, c.Content, c.IsAvailable, c.IsTaxable, c.OrderSequence, c.Title);
                Commit(aggregate, c);
            });
        }

        public void Handle(DeleteSeat c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.DeleteSeat(c.Seat);
                Commit(aggregate, c);
            });
        }

        public void Handle(ModifyLearnerRegistrationGroup c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AddLearnerRegistrationGroup(c.LearnerRegistrationGroup);
                Commit(aggregate, c);
            });
        }

        public void Handle(ReturnExamMaterial c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ReturnExamMaterial(c.ShipmentCode, c.ShipmentReceived, c.ShipmentCondition);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeAppointmentType c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeAppointmentType(c.AppointmentType);
                Commit(aggregate, c);
            });
        }

        public void Handle(LockEventRegistration c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.LockEventRegistration();
                Commit(aggregate, c);
            });
        }

        public void Handle(UnlockEventRegistration c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.UnlockEventRegistration();
                Commit(aggregate, c);
            });
        }

        public void Handle(ModifyAllowMultipleRegistrations c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ModifyAllowMultipleRegistrations(c.Value);
                Commit(aggregate, c);
            });
        }

        public void Handle(ModifyMandatorySurvey c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ModifyMandatorySurvey(c.SurveyForm);
                Commit(aggregate, c);
            });
        }

        public void Handle(ModifyPersonCodeIsRequired c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ModifyPersonCodeIsRequired(c.Value);
                Commit(aggregate, c);
            });
        }

        public void Handle(ModifyRegistrationField c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ModifyRegistrationField(c.Field);
                Commit(aggregate, c);
            });
        }

        public void Handle(ConnectEventMessage c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ConnectEventMessage(c.MessageType, c.MessageId);
                Commit(aggregate, c);
            });
        }

        public void Handle(ModifyEventMessagePeriod c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ModifyEventMessagePeriod(c.SendReminderBeforeDays);
                Commit(aggregate, c);
            });
        }

        public void Handle(SendEventMessage c)
        {
            _repository.LockAndRun<EventAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.SendEventMessage(c.MessageType, c.MessageId, c.Recipients);
                Commit(aggregate, c);
            });
        }
    }
}

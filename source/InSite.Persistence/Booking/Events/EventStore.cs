using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Application.Banks.Read;
using InSite.Application.Contents.Read;
using InSite.Application.Events.Read;
using InSite.Domain.Events;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    public class EventStore : IEventStore
    {
        private readonly IBankSearch _bankSearch;
        private readonly IJsonSerializer _serializer;

        public EventStore(IJsonSerializer serializer, IBankSearch bankSearch)
        {
            _serializer = serializer;
            _bankSearch = bankSearch;
        }

        internal InternalDbContext CreateContext() => new InternalDbContext(true);

        public void DeleteAll()
        {
            using (var db = CreateContext())
            {
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE events.QEvent");
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE events.QEventAttendee");
                db.Database.ExecuteSqlCommand("delete from assets.QComment where ContainerType = 'Event' and EventIdentifier is not null");
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE events.QEventAssessmentForm");
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE events.QEventTimer");
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE events.QSeat");
            }
        }

        public void DeleteEvent(Guid id)
        {
            using (var db = CreateContext())
            {
                var entity = db.Events.FirstOrDefault(x => x.EventIdentifier == id);
                if (entity != null)
                {
                    var sql = @"
DELETE FROM events.QEvent         WHERE EventIdentifier = @Aggregate;
DELETE FROM events.QEventAttendee WHERE EventIdentifier = @Aggregate;
DELETE FROM assets.QComment WHERE ContainerType = 'Event' AND EventIdentifier = @Aggregate;
DELETE FROM events.QEventAssessmentForm WHERE EventIdentifier = @Aggregate;
DELETE FROM events.QEventTimer    WHERE EventIdentifier = @Aggregate;
DELETE FROM events.QSeat          WHERE EventIdentifier = @Aggregate;
";

                    db.Database.ExecuteSqlCommand(sql, new SqlParameter("@Aggregate", id));
                }
            }
        }

        #region Activities

        public void InsertEvent(ClassImported e)
        {
            using (var db = CreateContext())
            {
                var @event = new QEvent
                {
                    DurationQuantity = e.DurationQuantity,
                    DurationUnit = e.DurationUnit,
                    EventIdentifier = e.AggregateIdentifier,
                    EventScheduledEnd = e.End,
                    EventScheduledStart = e.Start,
                    EventSource = e.Source,
                    EventTitle = e.Title,
                    EventType = "Class",
                    OrganizationIdentifier = e.Tenant,
                };

                SetLastChange(@event, e);

                db.Events.Add(@event);
                db.SaveChanges();
            }
        }

        public void InsertEvent(ClassScheduled2 e)
        {
            using (var db = CreateContext())
            {
                var @event = new QEvent
                {
                    CreditHours = e.Credit,
                    DurationQuantity = e.Duration,
                    DurationUnit = e.DurationUnit,
                    EventIdentifier = e.AggregateIdentifier,
                    EventNumber = e.Number,
                    EventScheduledEnd = e.EndTime,
                    EventScheduledStart = e.StartTime,
                    EventSchedulingStatus = e.Status,
                    EventTitle = e.Title,
                    EventType = "Class",
                    OrganizationIdentifier = e.Tenant
                };

                SetLastChange(@event, e);

                db.Events.Add(@event);
                db.SaveChanges();
            }
        }

        public void InsertEvent(AppointmentScheduled e)
        {
            using (var db = CreateContext())
            {
                var @event = new QEvent
                {
                    EventIdentifier = e.AggregateIdentifier,
                    EventScheduledEnd = e.EndTime,
                    EventScheduledStart = e.StartTime,
                    EventTitle = e.Title,
                    EventType = EventType.Appointment.ToString(),
                    AppointmentType = e.AppointmentType,
                    EventDescription = e.Description,
                    OrganizationIdentifier = e.Tenant,
                };

                SetLastChange(@event, e);

                db.Events.Add(@event);
                db.SaveChanges();
            }
        }

        public void InsertEvent(ExamScheduled2 e)
        {
            using (var db = CreateContext())
            {
                var @event = new QEvent
                {
                    DurationQuantity = e.Duration,
                    DurationUnit = "Minute",
                    EventBillingType = e.BillingCode,
                    EventClassCode = e.ClassCode,
                    EventFormat = e.Format,
                    EventIdentifier = e.AggregateIdentifier,
                    EventNumber = e.Number,
                    EventScheduledEnd = e.StartTime.AddMinutes(e.Duration),
                    EventScheduledStart = e.StartTime,
                    EventSchedulingStatus = e.Status,
                    EventTitle = e.Title,
                    EventType = "Exam",
                    ExamDurationInMinutes = e.Duration,
                    ExamType = e.Type,
                    OrganizationIdentifier = e.Tenant,
                    EventSource = e.Source,
                };

                SetLastChange(@event, e);

                db.Events.Add(@event);
                db.SaveChanges();
            }
        }

        public void InsertEvent(MeetingScheduled2 e)
        {
            using (var db = CreateContext())
            {
                var @event = new QEvent
                {
                    DurationQuantity = e.Duration,
                    DurationUnit = "Minute",
                    EventDescription = e.Description,
                    EventIdentifier = e.AggregateIdentifier,
                    EventNumber = e.Number,
                    EventScheduledEnd = e.StartTime.AddMinutes(e.Duration),
                    EventScheduledStart = e.StartTime,
                    EventSchedulingStatus = e.Status,
                    EventTitle = e.Title,
                    EventType = "Meeting",
                    OrganizationIdentifier = e.Tenant,
                };

                SetLastChange(@event, e);

                db.Events.Add(@event);
                db.SaveChanges();
            }
        }

        public void UpdateEvent(IChange e, Action<QEvent> change)
        {
            using (var db = CreateContext())
            {
                var @event = db.Events.Single(x => x.EventIdentifier == e.AggregateIdentifier);

                change(@event);

                SetLastChange(@event, e);

                db.SaveChanges();
            }
        }

        private void UpdateOrIgnore(IChange e, Action<QEvent> change)
        {
            using (var db = CreateContext())
            {
                var @event = db.Events.FirstOrDefault(x => x.EventIdentifier == e.AggregateIdentifier);
                if (@event == null)
                    return;

                change(@event);

                SetLastChange(@event, e);

                db.SaveChanges();
            }
        }

        private void SetLastChange(QEvent @event, IChange e)
        {
            @event.LastChangeTime = e.ChangeTime;
            @event.LastChangeType = e.GetType().Name;
            @event.LastChangeUser = UserSearch.GetFullName(e.OriginUser);
        }

        public void UpdateEvent(CapacityAdjusted e)
        {
            UpdateEvent(e, @event =>
            {
                @event.CapacityMinimum = e.Minimum;
                @event.CapacityMaximum = e.Maximum;
                @event.WaitlistEnabled = e.Waitlist == ToggleType.Enabled;
            });
        }

        public void UpdateEvent(EventRegistrationWithLinkAllowed e)
        {
            UpdateEvent(e, @event =>
            {
                @event.AllowRegistrationWithLink = true;
            });
        }

        public void UpdateEvent(CapacityIncreased e)
        {
            UpdateEvent(e, @event =>
            {
                @event.CapacityMaximum = (@event.CapacityMaximum ?? 0) + e.Increment;
            });
        }

        public void UpdateEvent(CapacityDecreased e)
        {
            UpdateEvent(e, @event =>
            {
                @event.CapacityMaximum = (@event.CapacityMaximum ?? 0) - e.Decrement;
            });
        }

        public void UpdateEvent(InvigilatorCapacityAdjusted e)
        {
            UpdateEvent(e, @event =>
            {
                @event.InvigilatorMinimum = e.Minimum;
            });
        }

        public void UpdateEvent(IntegrationConfigured e)
        {
            UpdateEvent(e, @event =>
            {
                @event.IntegrationWithholdGrades = e.WithholdGrades;
                @event.IntegrationWithholdDistribution = e.WithholdDistribution;
            });
        }

        public void UpdateEvent(EventCompleted e)
        {

        }

        public void UpdateEvent(EventCreditAssigned e)
        {
            UpdateEvent(e, @event =>
            {
                @event.CreditHours = e.Hours;
            });
        }

        public void UpdateEvent(EventPublicationCompleted e)
        {
            UpdateEvent(e, @event =>
            {
                @event.EventPublicationStatus = e.Status;
                @event.PublicationErrors = e.Errors;
            });
        }

        public void UpdateEvent(EventPublicationStarted e)
        {
            UpdateEvent(e, @event =>
            {
                @event.EventPublicationStatus = PublicationStatus.Drafted.ToString();
                @event.PublicationErrors = null;
            });
        }

        public void UpdateEvent(EventFormatChanged e)
        {
            UpdateEvent(e, @event =>
            {
                @event.EventFormat = e.Format;
            });
        }

        public void UpdateEvent(EventCalendarColorModified e)
        {
            UpdateEvent(e, @event =>
            {
                @event.EventCalendarColor = e.CalendarColor;
            });
        }

        public void UpdateEvent(EventNotificationTriggered e)
        {

        }

        public void UpdateEvent(EventRecoded e)
        {
            UpdateEvent(e, @event =>
            {
                @event.EventClassCode = e.ClassCode;
                @event.EventBillingType = e.BillingCode;
            });
        }

        public void UpdateEvent(EventRequestStatusChanged e)
        {
            UpdateEvent(e, @event =>
            {
                @event.EventRequisitionStatus = e.Status;
            });
        }

        public void UpdateEvent(EventRescheduled e)
        {
            UpdateEvent(e, @event =>
            {
                @event.EventScheduledStart = e.StartTime;
                @event.EventScheduledEnd = e.EndTime;
            });
        }

        public void UpdateEvent(EventDurationChanged e)
        {
            UpdateEvent(e, @event =>
            {
                @event.DurationQuantity = e.Duration;
                @event.DurationUnit = e.Unit;
            });
        }

        public void UpdateEvent(EventCreditHoursChanged e)
        {
            UpdateEvent(e, @event =>
            {
                @event.CreditHours = e.CreditHours;
            });
        }

        public void UpdateEvent(EventAchievementAdded e)
        {
            UpdateEvent(e, @event =>
            {
                @event.AchievementIdentifier = e.Achievement;
            });
        }

        public void UpdateEvent(EventAchievementChanged e)
        {
            UpdateEvent(e, @event =>
            {
                @event.AchievementIdentifier = e.Achievement;
            });
        }

        public void UpdateEvent(EventRenumbered e)
        {
            UpdateEvent(e, @event =>
            {
                @event.EventNumber = e.Number;
            });
        }

        public void UpdateEvent(EventRetitled e)
        {
            UpdateEvent(e, @event =>
            {
                @event.EventTitle = e.Title;
            });
        }

        public void UpdateEvent(EventScoresPublished e)
        {
            UpdateEvent(e, @event => { });
        }

        public void UpdateEvent(EventScoresValidated e)
        {
            UpdateEvent(e, @event => { });
        }

        public void UpdateEvent(EventScheduleStatusChanged e)
        {
            UpdateEvent(e, @event =>
            {
                @event.EventSchedulingStatus = e.Status;
            });
        }

        public void UpdateEvent(EventVenueChanged2 e)
        {
            UpdateEvent(e, @event =>
            {
                @event.VenueOfficeIdentifier = e.Office;
                @event.VenueLocationIdentifier = e.Location;
                @event.VenueRoom = e.Room;
            });
        }

        public void UpdateEvent(DistributionChanged e)
        {
            UpdateEvent(e, @event =>
            {
                @event.DistributionProcess = e.Process;

                @event.DistributionOrdered = e.Ordered;
                @event.DistributionExpected = e.Expected;
                @event.DistributionShipped = e.Shipped;
                @event.ExamStarted = e.Used;
            });
        }

        public void UpdateEvent(DistributionOrdered e)
        {
            UpdateEvent(e, @event =>
            {
                @event.DistributionOrdered = e.ChangeTime;
                @event.DistributionCode = e.Code;
                @event.DistributionStatus = e.Status;
                @event.DistributionErrors = e.Errors;
            });
        }

        public void UpdateEvent(DistributionTracked e)
        {
            UpdateEvent(e, @event =>
            {
                @event.DistributionTracked = e.ChangeTime;
                @event.DistributionStatus = e.Status;
                @event.DistributionErrors = e.Errors;
            });
        }

        public void UpdateEvent(ExamTypeChanged e)
        {
            UpdateEvent(e, @event =>
            {
                @event.ExamType = e.Type;
            });
        }

        public void UpdateEvent(EventDescribed e)
        {
            UpdateEvent(e, @event =>
            {
                var content = ContentEventClass.Deserialize(@event.Content);
                content.Title = e.Title;
                content.Description = e.Description;
                content.Summary = e.Summary;
                content.MaterialsForParticipation = e.MaterialsForParticipation;

                content.ClearInstructions();

                if (e.Instructions != null)
                {
                    foreach (var instruction in e.Instructions)
                        content[instruction.Type.GetName()] = instruction.Text;
                }

                content.ClassLink = e.ClassLink;

                var title = e.Title.Default;

                @event.EventTitle = title.HasValue() && title.Length > 400 ? title.Substring(0, 400) : title;
                @event.Content = JsonConvert.SerializeObject(content);
            });
        }

        public void UpdateEvent(AppointmentDescribed e)
        {
            UpdateEvent(e, @event =>
            {
                var content = ContentEventClass.Deserialize(@event.Content);
                content.Title = e.Title;
                content.Description = e.Description;

                var title = e.Title.Default;

                @event.EventTitle = title.HasValue() && title.Length > 400 ? title.Substring(0, 400) : title;
                @event.Content = JsonConvert.SerializeObject(content);
            });
        }

        public void UpdateEvent(EventPublished e)
        {
            UpdateOrIgnore(e, @event =>
            {
                @event.EventPublicationStatus = PublicationStatus.Published.ToString();
                @event.RegistrationStart = e.RegistrationStart;
                @event.RegistrationDeadline = e.RegistrationDeadline;
            });
        }

        public void UpdateEvent(EventUnpublished e)
        {
            UpdateEvent(e, @event =>
            {
                @event.EventPublicationStatus = PublicationStatus.Unpublished.ToString();
                @event.RegistrationStart = null;
                @event.RegistrationDeadline = null;
            });
        }

        public void UpdateEvent(EventCancelled e)
        {
            UpdateEvent(e, @event =>
            {
                @event.EventPublicationStatus = PublicationStatus.Archived.ToString();
                @event.EventSchedulingStatus = "Cancelled";
            });
        }

        public void UpdateEvent(ExamMaterialReturned e)
        {
            UpdateEvent(e, @event =>
            {
                @event.ExamMaterialReturnShipmentCode = e.ReturnShipmentCode;
                @event.ExamMaterialReturnShipmentReceived = e.ReturnShipmentDate;
                @event.ExamMaterialReturnShipmentCondition = e.ReturnShipmentCondition;
            });
        }

        public void UpdateEvent(AppointmentTypeChanged e)
        {
            UpdateEvent(e, @event =>
            {
                @event.AppointmentType = e.AppointmentType;
            });
        }

        public void UpdateEvent(LearnerRegistrationGroupModified e)
        {
            UpdateEvent(e, @event =>
            {
                @event.LearnerRegistrationGroupIdentifier = e.LearnerRegistrationGroup;
            });
        }

        public void UpdateEvent(RegistrationFieldModified e)
        {
            var state = (EventState)e.AggregateState;

            UpdateOrIgnore(e, @event =>
            {
                var fields = state.RegistrationFields.IsNotEmpty() ? state.RegistrationFields.Values : null;
                @event.SetRegistrationFields(fields);
            });
        }

        public void UpdateEvent(MandatorySurveyModified e)
        {
            UpdateOrIgnore(e, @event =>
            {
                @event.MandatorySurveyFormIdentifier = e.SurveyForm;
            });
        }

        #endregion

        #region Assessments

        public void InsertExamForm(Guid aggregateIdentifier, Guid formIdentifier)
        {
            var form = _bankSearch.GetFormData(formIdentifier);
            var bank = form.Specification.Bank;
            using (var db = CreateContext())
            {
                var attachment = db.EventAssessmentForms.FirstOrDefault(x => x.EventIdentifier == aggregateIdentifier && x.FormIdentifier == form.Identifier);
                if (attachment == null)
                {
                    attachment = new QEventAssessmentForm
                    {
                        EventIdentifier = aggregateIdentifier,
                        FormIdentifier = form.Identifier,
                        BankIdentifier = bank.Identifier
                    };
                    db.EventAssessmentForms.Add(attachment);
                }

                db.SaveChanges();
            }
        }

        public void DeleteExamForm(Guid aggregate, Guid form)
        {
            using (var db = CreateContext())
            {
                var attachment = db.EventAssessmentForms.FirstOrDefault(x => x.EventIdentifier == aggregate && x.FormIdentifier == form);
                if (attachment != null)
                {
                    db.EventAssessmentForms.Remove(attachment);
                    db.SaveChanges();
                }
            }
        }

        #endregion

        #region Comments

        public void InsertComment(Guid aggregate, Guid comment, Guid organization, Guid author, DateTimeOffset time, string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            using (var db = CreateContext())
            {
                var attachment = new QComment
                {
                    ContainerIdentifier = aggregate,
                    ContainerType = "Event",

                    AuthorUserIdentifier = author,

                    CommentIdentifier = comment,
                    CommentPosted = time,
                    CommentText = text,

                    EventIdentifier = aggregate,

                    OrganizationIdentifier = organization
                };
                db.QComments.Add(attachment);
                db.SaveChanges();
            }
        }

        public void UpdateComment(Guid comment, Guid author, DateTimeOffset time, string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            using (var db = CreateContext())
            {
                var attachment = db.QComments.FirstOrDefault(x => x.CommentIdentifier == comment);
                if (attachment != null)
                {
                    attachment.CommentText = text;
                    attachment.CommentPosted = time;
                    attachment.AuthorUserIdentifier = author;
                    db.SaveChanges();
                }
            }
        }

        public void DeleteComment(Guid comment)
        {
            using (var db = CreateContext())
            {
                var attachment = db.QComments.FirstOrDefault(x => x.CommentIdentifier == comment);
                if (attachment != null)
                {
                    db.QComments.Remove(attachment);
                    db.SaveChanges();
                }
            }
        }

        #endregion

        #region Timers

        public void DeleteTimer(Guid id)
        {
            using (var db = CreateContext())
            {
                var timer = db.EventTimers.FirstOrDefault(x => x.TriggerCommand == id);
                if (timer != null)
                {
                    db.EventTimers.Remove(timer);
                    db.SaveChanges();
                }
            }
        }

        public void InsertTimer(QEventTimer timer)
        {
            using (var db = CreateContext())
            {
                db.EventTimers.Add(timer);
                db.SaveChanges();
            }
        }

        public void UpdateTimer(Guid id, string status)
        {
            using (var db = CreateContext())
            {
                var timer = db.EventTimers.FirstOrDefault(x => x.TriggerCommand == id);
                if (timer != null)
                {
                    timer.TimerStatus = status;
                    db.SaveChanges();
                }
            }
        }

        #endregion

        #region Contacts

        public void InsertContact(Guid aggregate, Guid contact, string role, DateTimeOffset time)
        {
            if (string.IsNullOrWhiteSpace(role))
                role = "Role Not Defined";

            using (var db = CreateContext())
            {
                var attachment = db.EventAttendees.FirstOrDefault(x => x.EventIdentifier == aggregate && x.UserIdentifier == contact);

                if (attachment == null)
                {
                    var @event = db.Events.FirstOrDefault(x => x.EventIdentifier == aggregate);
                    if (@event != null)
                    {
                        attachment = new QEventAttendee
                        {
                            Assigned = time,
                            AttendeeRole = role,
                            EventIdentifier = aggregate,
                            OrganizationIdentifier = @event.OrganizationIdentifier,
                            UserIdentifier = contact
                        };
                        db.EventAttendees.Add(attachment);
                    }
                }

                db.SaveChanges();
            }
        }

        public void DeleteContact(Guid aggregate, Guid contact)
        {
            using (var db = CreateContext())
            {
                var attachment = db.EventAttendees.FirstOrDefault(x => x.EventIdentifier == aggregate && x.UserIdentifier == contact);
                if (attachment != null)
                {
                    db.EventAttendees.Remove(attachment);
                    db.SaveChanges();
                }
            }
        }

        #endregion

        #region Seats

        public void InsertSeat(SeatAdded e)
        {
            using (var db = CreateContext())
            {
                var seat = new QSeat
                {
                    SeatIdentifier = e.Seat,
                    EventIdentifier = e.AggregateIdentifier,
                    Configuration = e.Configuration,
                    Content = e.Content,
                    IsAvailable = e.IsAvailable,
                    IsTaxable = e.IsTaxable,
                    OrderSequence = e.OrderSequence,
                    SeatTitle = e.Title.MaxLength(100)
                };

                db.Seats.Add(seat);
                db.SaveChanges();
            }
        }

        public void Update(SeatRevised e)
        {
            using (var db = CreateContext())
            {
                var seat = db.Seats.Single(x => x.SeatIdentifier == e.Seat);
                seat.Configuration = e.Configuration;
                seat.Content = e.Content;
                seat.IsAvailable = e.IsAvailable;
                seat.IsTaxable = e.IsTaxable;
                seat.OrderSequence = e.OrderSequence;
                seat.SeatTitle = e.Title.MaxLength(100);

                db.SaveChanges();
            }
        }

        public void RemoveSeat(SeatDeleted e)
        {
            using (var db = CreateContext())
            {
                var seat = db.Seats.FirstOrDefault(x => x.SeatIdentifier == e.Seat);

                db.Entry(seat).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        #endregion
    }
}

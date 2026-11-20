using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

using InSite.Application.Registrations.Read;
using InSite.Domain.Registrations;

using Shift.Common;
using Shift.Common.Timeline.Changes;

namespace InSite.Persistence
{
    public class RegistrationStore : IRegistrationStore
    {
        private readonly IJsonSerializer _serializer;

        public RegistrationStore(IJsonSerializer serializer)
        {
            _serializer = serializer;
        }

        internal InternalDbContext CreateContext() => new InternalDbContext(true);

        public void InsertRegistration(RegistrationRequested e)
        {
            using (var db = CreateContext())
            {
                var registration = new QRegistration
                {
                    ApprovalStatus = e.ApprovalStatus,
                    AttendanceStatus = e.AttendanceStatus,
                    CandidateIdentifier = e.Candidate,
                    EventIdentifier = e.Event,
                    RegistrationComment = e.Comment,
                    RegistrationFee = e.Fee,
                    RegistrationIdentifier = e.AggregateIdentifier,
                    RegistrationPassword = e.Password,
                    RegistrationRequestedOn = e.ChangeTime,
                    RegistrationRequestedBy = e.OriginUser,
                    RegistrationSequence = e.Sequence,
                    RegistrationSource = e.Source,
                    OrganizationIdentifier = e.Tenant
                };

                SetLastChange(registration, e);

                db.Registrations.Add(registration);
                db.SaveChanges();
            }
        }

        private void Update(IChange e, Action<QRegistration> change) => Update(e, (db, reg) => change(reg));

        private void Update(IChange e, Action<InternalDbContext, QRegistration> change)
        {
            using (var db = CreateContext())
            {
                var registration = db.Registrations.FirstOrDefault(x => x.RegistrationIdentifier == e.AggregateIdentifier);
                if (registration == null)
                    return;

                change(db, registration);

                SetLastChange(registration, e);

                db.SaveChanges();
            }
        }

        public void UpdateRegistration(AccommodationGranted e)
        {
            using (var db = CreateContext())
            {
                var entity = db.Accommodations.FirstOrDefault(x => x.RegistrationIdentifier == e.AggregateIdentifier && x.AccommodationType == e.Type);
                if (entity == null)
                    db.Accommodations.Add(entity = new QAccommodation { AccommodationIdentifier = UniqueIdentifier.Create(), RegistrationIdentifier = e.AggregateIdentifier });
                else
                    db.Entry(entity).State = EntityState.Modified;

                entity.AccommodationType = e.Type;
                entity.AccommodationName = e.Name;
                entity.TimeExtension = e.TimeExtension;

                db.SaveChanges();
            }
        }

        public void UpdateRegistration(AccommodationRevoked e)
        {
            using (var db = CreateContext())
            {
                var entity = db.Accommodations.FirstOrDefault(x => x.RegistrationIdentifier == e.AggregateIdentifier && x.AccommodationType == e.Type);
                if (entity != null)
                {
                    db.Accommodations.Remove(entity);
                    db.SaveChanges();
                }
            }
        }

        public void UpdateRegistration(ApprovalChanged e)
        {
            Update(e, registration =>
            {
                registration.ApprovalStatus = e.Status;
                registration.ApprovalReason = e.Reason;
                registration.ApprovalProcess = _serializer.Serialize(e.Process);
            });
        }

        public void UpdateRegistration(AttemptAssigned e)
        {
            Update(e, (db, registration) =>
            {
                if (e.Form.HasValue && registration.ExamFormIdentifier != e.Form.Value)
                {
                    var forms = GetTargetForms(db, e.AggregateIdentifier, e.Form);
                    foreach (var form in forms)
                        form.AttemptIdentifier = e.Attempt;
                }
                else
                {
                    registration.AttemptIdentifier = e.Attempt;
                }
            });
        }

        public void UpdateRegistration(AttendanceTaken e)
        {
            Update(e, registration =>
            {
                registration.AttendanceStatus = e.Status;
                registration.AttendanceTaken = !string.IsNullOrEmpty(e.Status)
                                                ? DateTimeOffset.UtcNow         // Oleg: Possiible bug, maybe we should use ChangeTime instead of UtcNow here
                                                : (DateTimeOffset?)null;
            });
        }

        public void UpdateRegistration(ExamTimeLimited e)
        {
            Update(e, (db, registration) =>
            {
                if (e.Form.HasValue && registration.ExamFormIdentifier != e.Form.Value)
                {
                    var forms = GetTargetForms(db, e.AggregateIdentifier, e.Form);
                    foreach (var form in forms)
                        form.ExamTimeLimit = e.Minutes;
                }
                else
                {
                    registration.ExamTimeLimit = e.Minutes;
                }
            });
        }

        public void UpdateRegistration(CustomerAssigned e)
        {
            Update(e, registration =>
            {
                registration.CustomerIdentifier = e.Customer;
            });
        }

        public void UpdateRegistration(EligibilityChanged e)
        {
            Update(e, (db, registration) =>
            {
                var process = _serializer.Serialize(e.Process);

                if (e.Form.HasValue && registration.ExamFormIdentifier != e.Form.Value)
                {
                    var forms = GetTargetForms(db, e.AggregateIdentifier, e.Form);
                    foreach (var form in forms)
                    {
                        form.EligibilityStatus = e.Status;
                        form.EligibilityProcess = process;
                        form.EligibilityUpdated = e.ChangeTime;
                    }
                }
                else
                {
                    registration.EligibilityStatus = e.Status;
                    registration.EligibilityProcess = process;
                    registration.EligibilityUpdated = e.ChangeTime;
                }
            });
        }

        public void UpdateRegistration(EmployerAssigned e)
        {
            Update(e, registration =>
            {
                registration.EmployerIdentifier = e.Employer;
            });
        }

        public void UpdateRegistration(ExamFormAdded e)
        {
            var state = (Registration)e.AggregateState;

            Update(e, (db, registration) =>
            {
                var index = state.Forms.FindIndex(x => x.FormIdentifier == e.Form);

                if (index > 0)
                {
                    db.RegistrationExamForms.Add(new QRegistrationExamForm
                    {
                        RegistrationExamFormIdentifier = e.ExamForm,
                        RegistrationIdentifier = e.AggregateIdentifier,
                        ExamFormIdentifier = e.Form,
                        OrganizationIdentifier = registration.OrganizationIdentifier,
                        Password = e.Password,
                        Sequence = index
                    });
                }
                else
                {
                    registration.ExamFormIdentifier = e.Form;
                }
            });
        }

        public void UpdateRegistration(ExamFormAssigned e)
        {
            Update(e, registration =>
            {
                registration.ExamFormIdentifier = e.Form;
            });
        }

        public void UpdateRegistration(ExamFormChanged e)
        {
            Update(e, (db, registration) =>
            {
                if (registration.ExamFormIdentifier == e.FromForm)
                {
                    registration.ExamFormIdentifier = e.ToForm;
                    return;
                }

                var examForm = registration.ExamForms.FirstOrDefault(x => x.ExamFormIdentifier == e.FromForm);
                if (examForm != null)
                    examForm.ExamFormIdentifier = e.ToForm;
            });
        }

        public void UpdateRegistration(ExamFormPasswordChanged e)
        {
            Update(e, (db, registration) =>
            {
                if (registration.ExamFormIdentifier == e.Form)
                {
                    registration.RegistrationPassword = e.Password;
                    return;
                }

                var examForm = registration.ExamForms.FirstOrDefault(x => x.ExamFormIdentifier == e.Form);
                if (examForm != null)
                    examForm.Password = e.Password;
            });
        }

        public void UpdateRegistration(ExamFormRemoved e)
        {
            Update(e, (db, registration) =>
            {
                var examForms = registration.ExamForms.OrderBy(x => x.Sequence).ToArray();

                if (registration.ExamFormIdentifier == e.Form)
                {
                    var promoted = examForms.FirstOrDefault();
                    if (promoted == null)
                    {
                        registration.ExamFormIdentifier = null;
                        return;
                    }

                    registration.ExamFormIdentifier = promoted.ExamFormIdentifier;
                    registration.RegistrationPassword = promoted.Password;
                    registration.AttemptIdentifier = promoted.AttemptIdentifier;
                    registration.ExamTimeLimit = promoted.ExamTimeLimit;
                    registration.EligibilityProcess = promoted.EligibilityProcess;
                    registration.EligibilityStatus = promoted.EligibilityStatus;
                    registration.EligibilityUpdated = promoted.EligibilityUpdated;

                    db.RegistrationExamForms.Remove(promoted);
                    examForms = examForms.Skip(1).ToArray();
                }
                else
                {
                    var removed = examForms.FirstOrDefault(x => x.ExamFormIdentifier == e.Form);
                    if (removed == null)
                        return;

                    db.RegistrationExamForms.Remove(removed);
                    examForms = examForms.Where(x => x != removed).ToArray();
                }

                var sequence = 0;

                foreach (var form in examForms)
                {
                    sequence++;

                    if (form.Sequence != sequence)
                        form.Sequence = sequence;
                }
            });
        }

        public void UpdateRegistration(ExamFormUnassigned e)
        {
            Update(e, registration =>
            {
                registration.ExamFormIdentifier = null;
            });
        }

        public void UpdateRegistration(GradeChanged e)
        {
            Update(e, registration =>
            {
                registration.Grade = e.Grade;
                registration.GradeAssigned = e.ChangeTime;
                registration.Score = e.Score.HasValue
                    ? Math.Round(e.Score.Value, 4, MidpointRounding.AwayFromZero)
                    : (decimal?)null;
            });
        }

        public void UpdateRegistration(GradingChanged e)
        {
            Update(e, registration =>
            {
                registration.GradingStatus = e.Status;
                if (e.Process != null)
                    registration.GradingProcess = _serializer.Serialize(e.Process);

                if (e.Status == "Published")
                {
                    registration.GradePublished = e.ChangeTime;
                }
                else if (e.Status == "Released")
                {
                    registration.GradeReleased = e.ChangeTime;

                    registration.GradeWithheld = null;
                    registration.GradeWithheldReason = null;
                }
                else if (e.Status == "Withheld")
                {
                    registration.GradeReleased = null;

                    registration.GradeWithheld = e.ChangeTime;
                    registration.GradeWithheldReason = e.Process?.Description;
                }
                else if (e.Status == "Unpublished")
                {
                    registration.GradingStatus = null;
                    registration.GradePublished = null;
                }
            });
        }

        public void UpdateRegistration(InstructorAdded e)
        {
            using (var db = CreateContext())
            {
                var instructor = db.RegistrationInstructors
                    .FirstOrDefault(x => x.RegistrationIdentifier == e.AggregateIdentifier && x.InstructorIdentifier == e.Instructor);

                if (instructor == null)
                {
                    var registration = db.Registrations.FirstOrDefault(x => x.RegistrationIdentifier == e.AggregateIdentifier);
                    if (registration != null)
                    {
                        instructor = new QRegistrationInstructor
                        {
                            InstructorIdentifier = e.Instructor,
                            RegistrationIdentifier = e.AggregateIdentifier,
                            OrganizationIdentifier = registration.OrganizationIdentifier
                        };
                        db.RegistrationInstructors.Add(instructor);
                        db.SaveChanges();
                    }
                }
            }
        }

        public void UpdateRegistration(InstructorRemoved e)
        {
            using (var db = CreateContext())
            {
                var entity = db.RegistrationInstructors.FirstOrDefault(x => x.RegistrationIdentifier == e.AggregateIdentifier && x.InstructorIdentifier == e.Instructor);
                if (entity != null)
                {
                    db.RegistrationInstructors.Remove(entity);
                    db.SaveChanges();
                }
            }
        }

        public void UpdateRegistration(NotificationTriggered e)
        {

        }

        public void UpdateRegistration(RegistrationCancelled e)
        {
            Update(e, registration =>
            {
                registration.GradingStatus = "Cancelled";
            });
        }

        public void UpdateRegistration(RegistrationCommented e)
        {
            Update(e, registration =>
            {
                registration.RegistrationComment = e.Comment;
            });
        }

        public void UpdateRegistration(RegistrationFeeAssigned e)
        {
            Update(e, registration =>
            {
                registration.RegistrationFee = e.Fee;
            });
        }

        public void UpdateRegistration(RegistrationHoursWorkedAssigned e)
        {
            Update(e, registration =>
            {
                registration.WorkBasedHoursToDate = e.HoursWorked;
            });
        }

        public void UpdateRegistration(RegistrationPaymentAssigned e)
        {
            Update(e, registration =>
            {
                registration.PaymentIdentifier = e.Payment;
            });
        }

        public void UpdateRegistration(RegistrationPasswordChanged e)
        {
            Update(e, registration =>
            {
                registration.RegistrationPassword = e.Password;
            });
        }

        public void UpdateRegistration(SchoolAssigned e)
        {
            Update(e, registration =>
            {
                registration.SchoolIdentifier = e.School;
            });
        }

        public void UpdateRegistration(SchoolUnassigned e)
        {
            Update(e, registration =>
            {
                registration.SchoolIdentifier = null;
            });
        }

        public void UpdateRegistration(SeatAssigned e)
        {
            Update(e, registration =>
            {
                registration.SeatIdentifier = e.Seat;
                registration.RegistrationFee = e.Price;
                registration.BillingCustomer = e.BillingCustomer;
            });
        }

        public void UpdateRegistration(SynchronizationChanged e)
        {
            Update(e, registration =>
            {
                registration.SynchronizationStatus = e.Status;
                registration.SynchronizationProcess = _serializer.Serialize(e.Process);
            });
        }

        public void UpdateRegistration(CandidateChanged e)
        {
            Update(e, registration =>
            {
                registration.CandidateIdentifier = e.Candidate;
            });
        }

        public void UpdateRegistration(CandidateTypeChanged e)
        {
            Update(e, registration =>
            {
                registration.CandidateType = e.Type;
            });
        }

        public void UpdateRegistration(RegistrationBillingCodeModified e)
        {
            Update(e, registration =>
            {
                registration.BillingCode = e.BillingCode;
            });
        }

        public void UpdateRegistration(RegistrationIncludedToT2202 e)
        {
            Update(e, registration =>
            {
                registration.IncludeInT2202 = true;
            });
        }

        public void UpdateRegistration(RegistrationExcludedFromT2202 e)
        {
            Update(e, registration =>
            {
                registration.IncludeInT2202 = false;
            });
        }

        public void UpdateRegistration(EventChanged e)
        {
            Update(e, registration =>
            {
                registration.EventIdentifier = e.Event;
            });
        }

        public void UpdateRegistration(RegistrationRequestedByModified e)
        {
            Update(e, registration =>
            {
                registration.RegistrationRequestedBy = e.RegistrationRequestedBy;
            });
        }

        public void DeleteAll()
        {
            using (var db = CreateContext())
            {
                var sql = @"
TRUNCATE TABLE registrations.QRegistrationInstructor;
TRUNCATE TABLE registrations.QRegistrationExamForm;
TRUNCATE TABLE registrations.QRegistrationTimer;
TRUNCATE TABLE registrations.QAccommodation;
TRUNCATE TABLE registrations.QRegistration;
";
                db.Database.ExecuteSqlCommand(sql);
            }
        }

        public void Delete(Guid registration)
        {
            using (var db = CreateContext())
            {
                var sql = @"
DELETE FROM registrations.QRegistrationExamForm WHERE RegistrationIdentifier = @Aggregate;
DELETE FROM registrations.QRegistrationTimer WHERE RegistrationIdentifier = @Aggregate;
DELETE FROM registrations.QRegistration WHERE RegistrationIdentifier = @Aggregate;
";

                db.Database.ExecuteSqlCommand(sql, new SqlParameter("@Aggregate", registration));
            }
        }

        public void DeleteRegistration(RegistrationDeleted e)
        {
            Delete(e.AggregateIdentifier);
        }

        #region Timers

        public void InsertTimer(QRegistrationTimer timer)
        {
            using (var db = CreateContext())
            {
                db.Timers.Add(timer);
                db.SaveChanges();
            }
        }

        public void UpdateTimer(Guid id, string status)
        {
            using (var db = CreateContext())
            {
                var timer = db.Timers.FirstOrDefault(x => x.TriggerCommand == id);
                if (timer != null)
                {
                    timer.TimerStatus = status;
                    db.SaveChanges();
                }
            }
        }

        public void DeleteTimer(Guid id)
        {
            using (var db = CreateContext())
            {
                var timer = db.Timers.FirstOrDefault(x => x.TriggerCommand == id);
                if (timer != null)
                {
                    db.Timers.Remove(timer);
                    db.SaveChanges();
                }
            }
        }

        #endregion

        #region Helpers

        private void SetLastChange(QRegistration registration, IChange e)
        {
            registration.LastChangeTime = e.ChangeTime;
            registration.LastChangeType = e.GetType().Name;
            registration.LastChangeUser = UserSearch.GetFullName(e.OriginUser);
        }

        private static QRegistrationExamForm[] GetTargetForms(InternalDbContext db, Guid registrationId, Guid? formId)
        {
            var query = db.RegistrationExamForms.Where(x => x.RegistrationIdentifier == registrationId);

            if (formId.HasValue)
                query = query.Where(x => x.ExamFormIdentifier == formId.Value);

            return query.OrderBy(x => x.Sequence).ToArray();
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Contacts.Read;
using InSite.Application.Registrations.Read;
using InSite.Domain.Registrations;
using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public class RegistrationSearch : IRegistrationSearch
    {
        #region VAttendance

        public int CountAttendances(VAttendanceFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateAttendanceQuery(filter, db)
                    .Count();
            }
        }

        public List<VAttendance> GetAttendances(VAttendanceFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateAttendanceQuery(filter, db)
                    .OrderByDescending(x => x.LastChangeTime)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        private static IQueryable<VAttendance> CreateAttendanceQuery(VAttendanceFilter filter, InternalDbContext db)
        {
            var query = db.Attendances.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.LearnerName.HasValue())
                query = query.Where(x => x.LearnerName.Contains(filter.LearnerName));

            if (filter.LearnerEmail.HasValue())
                query = query.Where(x => x.LearnerEmail.Contains(filter.LearnerEmail));

            if (filter.LearnerCode.HasValue())
                query = query.Where(x => x.LearnerCode.Contains(filter.LearnerCode));

            if (filter.LastChangeTimeSince.HasValue)
                query = query.Where(x => x.LastChangeTime >= filter.LastChangeTimeSince.Value);

            if (filter.LastChangeTimeBefore.HasValue)
                query = query.Where(x => x.LastChangeTime < filter.LastChangeTimeBefore.Value);

            return query;
        }

        #endregion

        #region Public (Registrations)

        public int? GetMaxSequence(Guid eventIdentifier)
        {
            using (var db = CreateContext())
            {
                return db.Registrations.AsNoTracking()
                    .Where(x => x.EventIdentifier == eventIdentifier)
                    .Max(x => x.RegistrationSequence);
            }
        }

        public List<ApprenticeCompletionRateReportItem> GetApprenticeCompletionRateReport(QRegistrationFilter filter)
        {
            using (var db = CreateContext())
            {
                var registrations = CreateQuery(filter, db)
                    .Where(x => x.Event.Achievement.AchievementLabel == "Level")
                    .Select(x => new
                    {
                        UserIdentifier = x.CandidateIdentifier,
                        UserFullName = x.Candidate.UserFullName,
                        AchievementDescription = x.Event.Achievement.AchievementDescription,
                        CredentialCount = db.QCredentials
                            .Where(y =>
                                y.UserIdentifier == x.CandidateIdentifier
                                && y.Achievement.OrganizationIdentifier == x.Event.OrganizationIdentifier
                                && y.Achievement.AchievementDescription == x.Event.Achievement.AchievementDescription
                            )
                            .Count(),
                        Classes = db.QProgresses
                            .Where(y =>
                                y.UserIdentifier == x.CandidateIdentifier
                                && y.GradeItem.Achievement.OrganizationIdentifier == x.Event.OrganizationIdentifier
                                && y.GradeItem.Achievement.AchievementDescription == x.Event.Achievement.AchievementDescription
                                && y.ProgressPercent != null
                                && y.Gradebook.Event != null
                            )
                            .Select(y => new ApprenticeCompletionRateReportItem.ClassItem
                            {
                                GradebookIdentifier = y.GradebookIdentifier,
                                EventTitle = y.Gradebook.Event.EventTitle,
                                EventScheduledStart = y.Gradebook.Event.EventScheduledStart,
                                EventScheduledEnd = y.Gradebook.Event.EventScheduledEnd,
                                Percent = (decimal)y.ProgressPercent,
                            })
                            .ToList()
                    })
                    .ToList();

                var achievementDescriptions = registrations
                    .Select(x => x.AchievementDescription)
                    .Distinct()
                    .ToList();

                var filteredAchievementDescriptions = db.QAchievements
                    .Where(x => x.AchievementLabel == "Level" && achievementDescriptions.Contains(x.AchievementDescription))
                    .ToList()
                    .GroupBy(x => x.AchievementDescription)
                    .Where(x => x.Count() > 1)
                    .Select(x => new { AchievementDescription = x.Key, Count = x.Count() })
                    .ToList();

                return registrations
                    .Where(x => filteredAchievementDescriptions.Any(y => x.AchievementDescription == y.AchievementDescription))
                    .GroupBy(x => new { x.UserIdentifier, x.AchievementDescription })
                    .Select(x => x.FirstOrDefault())
                    .Select(x => new ApprenticeCompletionRateReportItem
                    {
                        UserFullName = x.UserFullName,
                        AchievementDescription = x.AchievementDescription,
                        CredentialCount = x.CredentialCount,
                        IsCompleted = x.CredentialCount == filteredAchievementDescriptions.Find(y => y.AchievementDescription == x.AchievementDescription).Count,
                        Classes = x.Classes
                    })
                    .ToList();
            }
        }

        public List<ApprenticeScoresReportItem> GetApprenticeScoresReport(QRegistrationFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .GroupJoin(db.QEnrollments,
                        a => a.CandidateIdentifier,
                        b => b.LearnerIdentifier,
                        (a, b) => new { Registration = a, Students = b.DefaultIfEmpty() }
                    )
                    .SelectMany(x => x.Students.Select(y => new
                    {
                        Registration = x.Registration,
                        Student = y
                    }))
                    .GroupJoin(db.QProgresses.Where(x => x.GradeItem.Achievement.AchievementLabel == "Level"),
                        a => new { a.Student.GradebookIdentifier, UserIdentifier = a.Student.LearnerIdentifier },
                        b => new { b.GradebookIdentifier, b.UserIdentifier },
                        (a, b) => new { Registration = a.Registration, Student = a.Student, Scores = b.DefaultIfEmpty() }
                    )
                    .SelectMany(x => x.Scores.Select(y => new ApprenticeScoresReportItem
                    {
                        GradeItemIdentifier = y.GradeItemIdentifier,
                        GradeItemSequence = y.GradeItem.GradeItemSequence,
                        RegistrationIdentifier = x.Registration.RegistrationIdentifier,
                        EventIdentifier = x.Registration.EventIdentifier,
                        AchievementDescription = x.Registration.Event.Achievement.AchievementDescription,
                        EventTitle = x.Registration.Event.EventTitle,
                        EventScheduledStart = x.Registration.Event.EventScheduledStart,
                        CandidateIdentifier = x.Registration.CandidateIdentifier,
                        UserFullName = x.Registration.Candidate.UserFullName,
                        UserEmail = x.Registration.Candidate.UserEmail,
                        EmployerGroupName = x.Registration.Employer.GroupName,
                        Region = x.Registration.Employer.GroupRegion,
                        Street1 = x.Registration.Candidate.HomeAddress.Street1,
                        Street2 = x.Registration.Candidate.HomeAddress.Street2,
                        City = x.Registration.Candidate.HomeAddress.City,
                        Province = x.Registration.Candidate.HomeAddress.Province,
                        PostalCode = x.Registration.Candidate.HomeAddress.PostalCode,
                        UserPhone = x.Registration.Candidate.UserPhone,
                        ScoreAchievementIdentifier = y.GradeItem.AchievementIdentifier,
                        ScorePercent = y.ProgressPercent,
                        ScoreText = y.ProgressText,
                        GradebookEventStartDate = (DateTimeOffset?)y.Gradebook.Event.EventScheduledStart
                    }))
                    .ToList()
                    .OrderBy(x => x.GradeItemSequence)
                    .ToList();
            }
        }

        public List<QAccommodation> GetAccommodations(Guid registrationIdentifier)
        {
            using (var db = CreateContext())
            {
                return db.Accommodations
                    .Where(x => x.RegistrationIdentifier == registrationIdentifier)
                    .OrderBy(x => x.AccommodationType)
                    .ToList();
            }
        }

        public List<QAccommodation> GetAccommodations(IEnumerable<Guid> registrationIdentifiers)
        {
            using (var db = CreateContext())
            {
                return db.Accommodations
                    .Where(x => registrationIdentifiers.Contains(x.RegistrationIdentifier))
                    .OrderBy(x => x.RegistrationIdentifier).ThenBy(x => x.AccommodationType)
                    .ToList();
            }
        }

        public List<string> GetAccommodationTypes(Guid organizationIdentifier)
        {
            using (var db = CreateContext())
            {
                return db.Accommodations
                    .Where(x => x.Registration.OrganizationIdentifier == organizationIdentifier && !string.IsNullOrEmpty(x.AccommodationType))
                    .Select(x => x.AccommodationType)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
            }
        }

        public List<VPerson> GetInstructors(Guid registrationIdentifier)
        {
            using (var db = CreateContext())
            {
                return db.RegistrationInstructors
                    .Where(x => x.RegistrationIdentifier == registrationIdentifier)
                    .Select(x => x.Instructor)
                    .OrderBy(x => x.UserFullName)
                    .ToList();
            }
        }

        public int CountRegistrations(QRegistrationFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db);

                return query.Count();
            }
        }

        public QRegistration GetRegistration(Guid registration, params Expression<Func<QRegistration, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.Registrations.AsNoTracking().ApplyIncludes(includes);

                return query.FirstOrDefault(x => x.RegistrationIdentifier == registration);
            }
        }

        public QRegistration GetRegistration(
            QRegistrationFilter filter,
            params Expression<Func<QRegistration, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db).ApplyIncludes(includes);

                query = string.IsNullOrEmpty(filter.OrderBy)
                    ? query.OrderBy(x => x.Attempt.AttemptStarted)
                    : query.OrderBy(filter.OrderBy);

                return query.FirstOrDefault();
            }
        }

        public List<QRegistration> GetRegistrations(
            QRegistrationFilter filter,
            params Expression<Func<QRegistration, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db).ApplyIncludes(includes);

                query = string.IsNullOrEmpty(filter.OrderBy)
                    ? query.OrderBy(x => x.Attempt.AttemptStarted)
                    : query.OrderBy(filter.OrderBy);

                return query.ApplyPaging(filter).ToList();
            }
        }

        public Guid? GetRegistrationIdentifier(QRegistrationFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db);

                query = string.IsNullOrEmpty(filter.OrderBy)
                    ? query.OrderBy(x => x.Attempt.AttemptStarted)
                    : query.OrderBy(filter.OrderBy);

                return query.Select(x => (Guid?)x.RegistrationIdentifier).FirstOrDefault();
            }
        }

        public List<Guid> GetRegistrationIdentifiers(QRegistrationFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db);

                query = string.IsNullOrEmpty(filter.OrderBy)
                    ? query.OrderBy(x => x.Attempt.AttemptStarted)
                    : query.OrderBy(filter.OrderBy);

                return query.ApplyPaging(filter).Select(x => x.RegistrationIdentifier).ToList();
            }
        }

        public List<Guid> GetRegistrationCandidateIdentifiers(QRegistrationFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db);

                return query.ApplyPaging(filter).Select(x => x.CandidateIdentifier).Distinct().ToList();
            }
        }

        public List<AttendeeListReportDataItem> GetRegistrationsForAttendeeListReport(QRegistrationFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db);

                query = string.IsNullOrEmpty(filter.OrderBy)
                    ? query.OrderBy(x => x.Attempt.AttemptStarted)
                    : query.OrderBy(filter.OrderBy);

                return query.ApplyPaging(filter)
                    .Where(x =>
                        (string.IsNullOrEmpty(x.ApprovalStatus) || x.ApprovalStatus == "Registered")
                        && x.AttendanceStatus != "Withdrawn/Cancelled"
                    )
                    .Select(x => new AttendeeListReportDataItem
                    {
                        UserFullName = x.Candidate.UserFullName,
                        Email = x.Candidate.UserEmail,
                        EmployerName = x.Employer.GroupName,
                        Phone = x.Candidate.UserPhone,
                        PersonCode = x.Candidate.PersonCode,
                        CandidateBirthdate = x.Candidate.Birthdate,
                        EventScheduledStart = x.Event.EventScheduledStart
                    }).ToList();
            }
        }

        public Dictionary<Guid, List<QRegistration>> GetRegistrationsByEvents(List<Guid> events)
        {
            using (var db = CreateContext())
            {
                db.Database.CommandTimeout = 120;

                var list = db.Registrations
                    .AsNoTracking()
                    .Include(x => x.Accommodations)
                    .Where(x => events.Contains(x.EventIdentifier))
                    .ToList();

                return list
                    .GroupBy(x => x.EventIdentifier)
                    .ToDictionary(x => x.Key, y => y.ToList());

            }
        }

        public List<QRegistration> GetRegistrationsByEvent(Guid @event,
            string filterText = null, Paging paging = null, string orderBy = null,
            bool includeAccommodations = false, bool includeAttempt = false, bool includeCandidate = false, bool includeForm = false, bool includeInstructors = false)
        {
            using (var db = CreateContext())
            {
                db.Database.CommandTimeout = 120;

                var query = db.Registrations.AsNoTracking().AsQueryable();

                if (includeAccommodations)
                    query = query.Include(x => x.Accommodations);

                if (includeAttempt)
                    query = query.Include(x => x.Attempt);

                if (includeCandidate)
                    query = query.Include(x => x.Candidate);

                if (includeForm)
                    query = query.Include(x => x.Form);

                if (includeInstructors)
                    query = query.Include(x => x.RegistrationInstructors);

                query = query.Where(x => x.EventIdentifier == @event);

                if (filterText.IsNotEmpty())
                    query = query.Where(x => x.Candidate.UserFullName.Contains(filterText)
                                          || x.Candidate.UserEmail.Contains(filterText)
                                          || x.Candidate.PersonCode.Contains(filterText));

                if (string.IsNullOrEmpty(orderBy))
                    query = query.OrderBy(x => x.Candidate.UserFullName);
                else
                    query = query.OrderBy(orderBy);

                return query.ApplyPaging(paging).ToList();
            }
        }

        public List<QRegistration> GetRegistrationsByCandidate(Guid candidate, params Expression<Func<QRegistration, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.Registrations.AsQueryable();

                if (includes == null || includes.Length == 0)
                {
                    query = db.Registrations
                        .Include(x => x.Event)
                        .Include(x => x.Form)
                        .Include(x => x.Accommodations);
                }
                else
                {
                    query = query.ApplyIncludes(includes);
                }

                query = query
                    .AsNoTracking()
                    .Where(x => x.CandidateIdentifier == candidate);

                return query.ToList();
            }
        }

        public void Refresh(List<QRegistration> registrations)
        {
            using (var db = CreateContext())
            {
                var data = registrations.ToDictionary(x => x.RegistrationIdentifier);
                var query = db.Registrations.AsNoTracking().Where(x => data.Keys.Contains(x.RegistrationIdentifier)).ToArray();

                foreach (var entity in query)
                    query.ShallowCopyTo(data[entity.RegistrationIdentifier]);
            }
        }

        #endregion

        #region Public (Timers)

        public int CountTimers(QRegistrationTimerFilter filter)
        {
            using (var db = CreateContext())
                return CreateQuery(filter, db).Count();
        }

        public XRegistrationTimer GetTimer(Guid id)
        {
            using (var db = CreateContext())
                return db.XTimers.FirstOrDefault(x => x.TriggerCommand == id);
        }

        public List<XRegistrationTimer> GetTimers(QRegistrationTimerFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .OrderBy(x => x.TriggerTime)
                    .ThenBy(x => x.TriggerCommand)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        public List<XRegistrationTimer> GetTimersThatShouldBeElapsed()
        {
            using (var db = CreateContext())
            {
                return db.XTimers
                    .Where(x => x.TimerStatus == "Started" && x.TriggerTime <= DateTimeOffset.UtcNow)
                    .ToList();
            }
        }

        private IQueryable<XRegistrationTimer> CreateQuery(QRegistrationTimerFilter filter, InternalDbContext db)
        {
            var query = db.XTimers
                .AsNoTracking()
                .AsQueryable();

            if (filter == null)
                return query;

            if (filter.EventIdentifier.HasValue)
                query = query.Where(x => x.EventIdentifier == filter.EventIdentifier);

            if (filter.RegistrationIdentifier.HasValue)
                query = query.Where(x => x.RegistrationIdentifier == filter.RegistrationIdentifier);

            if (!string.IsNullOrWhiteSpace(filter.TimerStatus))
                query = query.Where(x => x.TimerStatus == filter.TimerStatus);

            if (filter.TriggerTimeSince.HasValue)
                query = query.Where(x => x.TriggerTime >= filter.TriggerTimeSince.Value);

            return query;
        }

        #endregion

        #region Private

        public List<string> GetApprovalStatuses(Guid organizationIdentifier)
        {
            using (var db = CreateContext())
            {
                return db.Registrations.AsNoTracking()
                    .Where(x => x.Event.OrganizationIdentifier == organizationIdentifier && x.ApprovalStatus != null)
                    .Select(x => x.ApprovalStatus)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
            }
        }

        public List<QRegistration> GetRegistrationsWithoutEvent()
        {
            using (var db = CreateContext())
            {
                return db.Registrations.AsNoTracking()
                    .Where(x => db.Events.FirstOrDefault(y => y.EventIdentifier == x.EventIdentifier) == null)
                    .ToList();
            }
        }

        private InternalDbContext CreateContext() => new InternalDbContext(false);

        private IQueryable<QRegistration> CreateQuery(QRegistrationFilter inputFilter, InternalDbContext db)
        {
            IQueryable<QRegistration> query;

            if (inputFilter.OrganizationIdentifier.HasValue)
            {
                query = db.Registrations.AsNoTracking()
                    .Where(x => x.Event.OrganizationIdentifier == inputFilter.OrganizationIdentifier);
            }
            else
            {
                query = db.Registrations.AsNoTracking().AsQueryable();
            }

            if (inputFilter.RegistrationIdentifiers.IsNotEmpty())
            {
                if (inputFilter.RegistrationIdentifiers.Length == 1)
                {
                    var value = inputFilter.RegistrationIdentifiers[0];
                    query = query.Where(x => x.RegistrationIdentifier == value);
                }
                else
                    query = query.Where(x => inputFilter.RegistrationIdentifiers.Contains(x.RegistrationIdentifier));
            }

            if (inputFilter.CandidateIdentifiers.IsNotEmpty())
            {
                if (inputFilter.CandidateIdentifiers.Length == 1)
                {
                    var value = inputFilter.CandidateIdentifiers[0];
                    query = query.Where(x => x.CandidateIdentifier == value);
                }
                else
                    query = query.Where(x => inputFilter.CandidateIdentifiers.Contains(x.CandidateIdentifier));
            }

            if (inputFilter.EventIdentifier.HasValue)
                query = query.Where(x => x.EventIdentifier == inputFilter.EventIdentifier);

            if (inputFilter.HasEvent.HasValue)
            {
                if (inputFilter.HasEvent.Value)
                    query = query.Where(x => x.Event != null);
                else
                    query = query.Where(x => x.Event == null);
            }

            if (inputFilter.HasExamForm.HasValue)
            {
                if (inputFilter.HasExamForm.Value)
                    query = query.Where(x => x.ExamFormIdentifier.HasValue);
                else
                    query = query.Where(x => !x.ExamFormIdentifier.HasValue);
            }

            if (inputFilter.EventFormat.IsNotEmpty())
                query = query.Where(x => x.Event.EventFormat == inputFilter.EventFormat);

            if (inputFilter.EventTitle.IsNotEmpty())
                query = query.Where(x => x.Event.EventTitle.Contains(inputFilter.EventTitle));

            if (inputFilter.EventType.IsNotEmpty())
                query = query.Where(x => x.Event.EventType == inputFilter.EventType);

            if (inputFilter.EventScheduledSince.HasValue)
                query = query.Where(x => x.Event.EventScheduledStart >= inputFilter.EventScheduledSince.Value);

            if (inputFilter.EventScheduledBefore.HasValue)
                query = query.Where(x => x.Event.EventScheduledStart < inputFilter.EventScheduledBefore.Value);

            if (inputFilter.BankIdentifier.HasValue)
                query = query.Where(x => x.Form.BankIdentifier == inputFilter.BankIdentifier);

            if (inputFilter.FormIdentifiers.IsNotEmpty())
                query = query.Where(x => inputFilter.FormIdentifiers.Contains(x.Attempt.FormIdentifier));

            if (inputFilter.ExamFormName.IsNotEmpty())
                query = query.Where(x => x.Form.FormName.Contains(inputFilter.ExamFormName));

            if (inputFilter.QuestionIdentifier.HasValue)
                query = query.Where(x => x.Attempt.Questions.Any(y => y.QuestionIdentifier == inputFilter.QuestionIdentifier));

            if (inputFilter.OccupationIdentifier.HasValue)
                query = query.Where(x => x.Form.VBank.OccupationIdentifier == inputFilter.OccupationIdentifier.Value);

            if (inputFilter.FrameworkIdentifier.HasValue)
                query = query.Where(x => x.Form.Bank.FrameworkIdentifier == inputFilter.FrameworkIdentifier.Value);

            if (inputFilter.Candidate.IsNotEmpty())
            {
                var candidates = StringHelper.Split(inputFilter.Candidate);
                query = query.Where(x => x.Candidate.UserFullName.Contains(inputFilter.Candidate)
                                 || x.Candidate.UserEmail.Contains(inputFilter.Candidate)
                                 || candidates.Contains(x.Candidate.PersonCode)
                                 );
            }

            if (inputFilter.CandidateIdentifier.HasValue)
                query = query.Where(x => x.CandidateIdentifier == inputFilter.CandidateIdentifier.Value);

            if (inputFilter.HasCandidate.HasValue)
            {
                if (inputFilter.HasCandidate.Value)
                    query = query.Where(x => x.Candidate != null);
                else
                    query = query.Where(x => x.Candidate == null);
            }

            if (inputFilter.CandidateEmailEnabled.HasValue)
                query = query.Where(x => x.Candidate.UserEmailEnabled == inputFilter.CandidateEmailEnabled);

            if (inputFilter.CandidateCode.IsNotEmpty())
                query = query.Where(x => x.Candidate.PersonCode.Contains(inputFilter.CandidateCode));

            if (inputFilter.CandidateName.IsNotEmpty())
                query = query.Where(x => x.Candidate.UserFullName.Contains(inputFilter.CandidateName));

            if (inputFilter.CandidateEmail.IsNotEmpty())
                query = query.Where(x => x.Candidate.UserEmail.Contains(inputFilter.CandidateEmail));

            if (inputFilter.CandidateCompany.IsNotEmpty())
                query = query.Where(x => x.Candidate.EmployerGroupName.Contains(inputFilter.CandidateCompany));

            if (inputFilter.CandidateType.IsNotEmpty())
                query = query.Where(x => x.CandidateType.Contains(inputFilter.CandidateType));

            if (inputFilter.Form.IsNotEmpty())
            {
                query = query.Where(x => x.Form.FormTitle.Contains(inputFilter.Form)
                             || x.Form.FormName.Contains(inputFilter.Form)
                             || x.Form.FormAsset.ToString() == inputFilter.Form
                             );
            }

            if (inputFilter.FormName.IsNotEmpty())
                query = query.Where(x => x.Form.FormName.Contains(inputFilter.FormName));

            if (inputFilter.Event.IsNotEmpty())
                query = query.Where(x => inputFilter.Event == x.Event.EventNumber.ToString());

            if (inputFilter.IsStarted.HasValue)
            {
                if (inputFilter.IsStarted.Value)
                    query = query.Where(x => x.Attempt.AttemptStarted.HasValue);
                else
                    query = query.Where(x => !x.Attempt.AttemptStarted.HasValue);
            }

            if (inputFilter.IsCompleted.HasValue)
            {
                if (inputFilter.IsCompleted.Value)
                    query = query.Where(x => x.Attempt.AttemptGraded.HasValue);
                else
                    query = query.Where(x => !x.Attempt.AttemptGraded.HasValue);
            }

            if (inputFilter.IsPublished.HasValue)
            {
                if (inputFilter.IsPublished.Value)
                    query = query.Where(x => x.GradePublished.HasValue);
                else
                    query = query.Where(x => !x.GradePublished.HasValue);
            }

            if (inputFilter.IsWithheld.HasValue)
            {
                if (inputFilter.IsWithheld.Value)
                    query = query.Where(x => x.GradeWithheld.HasValue);
                else
                    query = query.Where(x => !x.GradeWithheld.HasValue);
            }

            if (inputFilter.SubmissionGrade.IsNotEmpty())
                query = query.Where(x => x.Attempt.AttemptGrade == inputFilter.SubmissionGrade);

            if (inputFilter.SubmissionTag.IsNotEmpty())
                query = query.Where(x => inputFilter.SubmissionTag.Contains(x.Attempt.AttemptTag));

            if (inputFilter.SubmissionScoreFrom.HasValue)
                query = query.Where(x => Math.Round(x.Attempt.AttemptScore.Value * 100) >= inputFilter.SubmissionScoreFrom.Value);

            if (inputFilter.SubmissionScoreThru.HasValue)
                query = query.Where(x => Math.Round(x.Attempt.AttemptScore.Value * 100) <= inputFilter.SubmissionScoreThru.Value);

            if (inputFilter.AttemptCompletedSince.HasValue)
                query = query.Where(x => x.Attempt.AttemptGraded >= inputFilter.AttemptCompletedSince.Value);

            if (inputFilter.AttemptCompletedBefore.HasValue)
                query = query.Where(x => x.Attempt.AttemptGraded < inputFilter.AttemptCompletedBefore.Value);

            if (inputFilter.IsRegistered)
                query = query.Where(x => x.ApprovalStatus == "Registered" && x.AttendanceStatus != "Withdrawn/Cancelled" && x.AttendanceStatus != "Incomplete");

            if (inputFilter.ApprovalStatus.HasValue())
                query = query.Where(x => x.ApprovalStatus == inputFilter.ApprovalStatus);

            if (inputFilter.ApprovalStatuses != null && inputFilter.ApprovalStatuses.Length > 0)
                query = query.Where(x => inputFilter.ApprovalStatuses.Contains(x.ApprovalStatus));

            if (inputFilter.GradingStatus.HasValue())
                query = query.Where(x => x.GradingStatus == inputFilter.GradingStatus);

            if (inputFilter.RegistrationStartedSince.HasValue)
                query = query.Where(x => x.Attempt.AttemptStarted >= inputFilter.RegistrationStartedSince.Value);

            if (inputFilter.RegistrationStartedBefore.HasValue)
                query = query.Where(x => x.Attempt.AttemptStarted < inputFilter.RegistrationStartedBefore.Value);

            if (inputFilter.RegistrationCompletedSince.HasValue)
                query = query.Where(x => x.Attempt.AttemptGraded >= inputFilter.RegistrationCompletedSince.Value);

            if (inputFilter.RegistrationCompletedBefore.HasValue)
                query = query.Where(x => x.Attempt.AttemptGraded < inputFilter.RegistrationCompletedBefore.Value);

            if (inputFilter.RegistrationRequestedSince.HasValue)
                query = query.Where(x => x.RegistrationRequestedOn >= inputFilter.RegistrationRequestedSince.Value);

            if (inputFilter.RegistrationRequestedBefore.HasValue)
                query = query.Where(x => x.RegistrationRequestedOn < inputFilter.RegistrationRequestedBefore.Value);

            if (inputFilter.IsAttendanceOverdue.HasValue && inputFilter.IsAttendanceOverdue.Value)
            {
                var cutoff = DateTimeOffset.UtcNow.AddHours(-1);
                query = query.Where(x => x.Event.EventScheduledStart < cutoff && (x.AttendanceStatus == null || x.AttendanceStatus == "Pending"));
            }

            if (inputFilter.CandidateEmployerGroupIdentifier.HasValue)
                query = query.Where(x => x.Candidate.EmployerGroupIdentifier == inputFilter.CandidateEmployerGroupIdentifier);

            if (inputFilter.SeatAvailable.HasValue)
                query = query.Where(x => x.Seat.IsAvailable == inputFilter.SeatAvailable.Value);

            if (inputFilter.SeatIdentifier.HasValue)
                query = query.Where(x => x.SeatIdentifier == inputFilter.SeatIdentifier);

            if (inputFilter.RegistrationEmployerName.HasValue())
                query = query.Where(x => x.Employer.GroupName.Contains(inputFilter.RegistrationEmployerName));

            if (inputFilter.RegistrationEmployerStatus.HasValue())
                query = query.Where(x => x.Employer.GroupStatus.Contains(inputFilter.RegistrationEmployerStatus));

            if (inputFilter.RegistrationEmployerRegion.HasValue())
                query = query.Where(x => x.Employer.GroupRegion == inputFilter.RegistrationEmployerRegion);

            if (inputFilter.RegistrationCustomerName.HasValue())
                query = query.Where(x => x.Customer.GroupName.Contains(inputFilter.RegistrationCustomerName));

            if (inputFilter.RegistrationEmployerIdentifier.HasValue)
                query = query.Where(x => x.EmployerIdentifier == inputFilter.RegistrationEmployerIdentifier);

            if (inputFilter.RegistrationCustomerIdentifier.HasValue)
                query = query.Where(x => x.CustomerIdentifier == inputFilter.RegistrationCustomerIdentifier);

            if (inputFilter.RegistrationComment.HasValue())
                query = query.Where(x => x.RegistrationComment.Contains(inputFilter.RegistrationComment));

            if (inputFilter.VenueName.HasValue())
                query = query.Where(x => x.Event.VenueLocation.GroupName.Contains(inputFilter.VenueName));

            if (inputFilter.VenueLocationIdentifier.IsNotEmpty())
                query = query.Where(x => inputFilter.VenueLocationIdentifier.Contains(x.Event.VenueLocationIdentifier.Value));

            if (!string.IsNullOrWhiteSpace(inputFilter.AttendanceStatus))
            {
                if (inputFilter.AttendanceStatus == "null")
                    query = query.Where(x => x.AttendanceStatus == null);
                else
                    query = query.Where(x => x.AttendanceStatus == inputFilter.AttendanceStatus);
            }

            if (inputFilter.AttendanceStatuses != null && inputFilter.AttendanceStatuses.Length > 0)
                query = query.Where(x => inputFilter.AttendanceStatuses.Contains(x.AttendanceStatus));

            if (inputFilter.IncludeInT2202.HasValue)
                query = query.Where(x => x.IncludeInT2202 == inputFilter.IncludeInT2202.Value);

            if (string.Equals(inputFilter.PaymentStatus, "N/A", StringComparison.OrdinalIgnoreCase))
                query = query.Where(x => x.PaymentIdentifier == null);
            else if (inputFilter.PaymentStatus.IsNotEmpty())
                query = query.Where(x => x.Payment.PaymentStatus == inputFilter.PaymentStatus);

            if (inputFilter.PaymentIdentifier.HasValue)
                query = query.Where(x => x.PaymentIdentifier == inputFilter.PaymentIdentifier);

            if (inputFilter.RegistrationRequestedBy.HasValue)
                query = query.Where(x => x.RegistrationRequestedBy == inputFilter.RegistrationRequestedBy.Value);

            if (!string.IsNullOrEmpty(inputFilter.RegistrationRequestedByName))
            {
                query = query.Where(x =>
                    (x.RegistrationRequestedByPerson.UserFirstName + " " + x.RegistrationRequestedByPerson.UserLastName).Contains(inputFilter.RegistrationRequestedByName)
                    || (x.RegistrationRequestedByPerson.UserLastName + ", " + x.RegistrationRequestedByPerson.UserFirstName).Contains(inputFilter.RegistrationRequestedByName)
                    || x.RegistrationRequestedByPerson.UserFullName.Contains(inputFilter.RegistrationRequestedByName)
                );
            }

            if (inputFilter.ExcludeCandidateIdentifier.IsNotEmpty())
                query = query.Where(x => !inputFilter.ExcludeCandidateIdentifier.Contains(x.CandidateIdentifier));

            if (inputFilter.BillingCode.IsNotEmpty())
                query = query.Where(x => x.BillingCode.Contains(inputFilter.BillingCode));

            if (inputFilter.CandidateMembershipGroupIdentifier.HasValue)
                query = query.Where(x => db.QMemberships.Any(y => y.UserIdentifier == x.CandidateIdentifier && y.GroupIdentifier == inputFilter.CandidateMembershipGroupIdentifier));

            return query;
        }

        #endregion

        public List<RegistrationLearnerTypeModel> GetLearnerTypes(List<Guid> registrations)
        {
            using (var db = CreateContext())
            {
                return db.Registrations.AsNoTracking().AsQueryable()
                    .Where(x => registrations.Any(r => r == x.RegistrationIdentifier))
                    .Select(x => new RegistrationLearnerTypeModel
                    {
                        RegistrationIdentifier = x.RegistrationIdentifier,
                        LearnerType = x.CandidateType
                    })
                    .ToList();
            }
        }
    }
}
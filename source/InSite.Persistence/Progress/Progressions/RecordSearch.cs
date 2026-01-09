using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Gradebooks.Write;
using InSite.Application.Logs.Read;
using InSite.Application.Progresses.Write;
using InSite.Application.Records.Read;
using InSite.Application.Standards.Read;
using InSite.Domain.Records;
using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Common.Linq;

using QGradebookCompetencyValidation = InSite.Application.Records.Read.QGradebookCompetencyValidation;

namespace InSite.Persistence
{
    public class RecordSearch : IRecordSearch
    {
        private readonly IAggregateSearch _aggregateSearch;

        public RecordSearch(IAggregateSearch aggregateSearch)
        {
            _aggregateSearch = aggregateSearch;
        }

        private static InternalDbContext CreateContext()
        {
            return new InternalDbContext(false);
        }

        public List<AcademicYearOutcome> GetAcademicYearOutcome(Guid organizationIdentifier, IEnumerable<Guid> gradebookPeriods)
        {
            using (var db = CreateContext())
            {
                return db.QGradebooks
                    .Where(x => x.OrganizationIdentifier == organizationIdentifier && x.PeriodIdentifier != null && gradebookPeriods.Contains(x.PeriodIdentifier.Value))
                    .Join(db.QGradebookCompetencyValidations,
                        a => a.GradebookIdentifier,
                        b => b.GradebookIdentifier,
                        (a, b) => new
                        {
                            StandardIdentifier = b.CompetencyIdentifier,
                            StandardTitle = b.Standard.StandardTitle,
                            ParentStandardTitle = b.Standard.Parent.StandardTitle,
                            Score = b.ValidationPoints ?? 0
                        }
                    )
                    .GroupBy(x => new
                    {
                        x.StandardIdentifier,
                        x.StandardTitle,
                        x.ParentStandardTitle
                    })
                    .Select(x => new AcademicYearOutcome
                    {
                        StandardIdentifier = x.Key.StandardIdentifier,
                        StandardTitle = x.Key.StandardTitle,
                        ParentStandardTitle = x.Key.ParentStandardTitle,
                        AvgScore = x.Average(y => y.Score),
                        StdScore = DbFunctions.StandardDeviation(x.Select(y => y.Score)),
                        StudentCount = x.Count()
                    })
                    .ToList();
            }
        }

        public List<OutcomeSummary> GetOutcomeSummary(QGradebookFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .Join(db.QGradebookCompetencyValidations,
                        a => a.GradebookIdentifier,
                        b => b.GradebookIdentifier,
                        (a, b) => new
                        {
                            Term = a.Period.PeriodName,
                            StandardIdentifier = b.CompetencyIdentifier,
                            StandardTitle = b.Standard.StandardTitle,
                            ParentStandardTitle = b.Standard.Parent.StandardTitle,
                            Score = b.ValidationPoints ?? 0
                        }
                    )
                    .GroupBy(x => new
                    {
                        x.Term,
                        x.StandardIdentifier,
                        x.StandardTitle,
                        x.ParentStandardTitle
                    })
                    .Select(x => new OutcomeSummary
                    {
                        Term = x.Key.Term,
                        StandardIdentifier = x.Key.StandardIdentifier,
                        StandardTitle = x.Key.StandardTitle,
                        ParentStandardTitle = x.Key.ParentStandardTitle,
                        AvgScore = x.Average(y => y.Score),
                        StdScore = DbFunctions.StandardDeviation(x.Select(y => y.Score)),
                        StudentCount = x.Count()
                    })
                    .ToList();
            }
        }

        public List<CourseOutcomeSummary> GetCourseOutcomeSummary(QGradebookFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .Join(db.QGradebookCompetencyValidations,
                        a => a.GradebookIdentifier,
                        b => b.GradebookIdentifier,
                        (a, b) => new
                        {
                            GradebookIdentifier = a.GradebookIdentifier,
                            GradebookTitle = a.GradebookTitle,
                            Reference = a.Reference,
                            Term = a.Period.PeriodName,
                            StandardIdentifier = b.CompetencyIdentifier,
                            StandardTitle = b.Standard.StandardTitle,
                            ParentStandardTitle = b.Standard.Parent.StandardTitle,
                            Score = b.ValidationPoints ?? 0
                        }
                    )
                    .GroupBy(x => new
                    {
                        x.GradebookIdentifier,
                        x.GradebookTitle,
                        x.Reference,
                        x.Term,
                        x.StandardIdentifier,
                        x.StandardTitle,
                        x.ParentStandardTitle
                    })
                    .Select(x => new CourseOutcomeSummary
                    {
                        GradebookIdentifier = x.Key.GradebookIdentifier,
                        GradebookTitle = x.Key.GradebookTitle,
                        Reference = x.Key.Reference,
                        Term = x.Key.Term,
                        StandardIdentifier = x.Key.StandardIdentifier,
                        StandardTitle = x.Key.StandardTitle,
                        ParentStandardTitle = x.Key.ParentStandardTitle,
                        AvgScore = x.Average(y => y.Score),
                        StdScore = DbFunctions.StandardDeviation(x.Select(y => y.Score)),
                        StudentCount = x.Count()
                    })
                    .ToList();
            }
        }

        public List<MostImprovedStudent> GetMostImprovedStudents(QProgressFilter filter)
        {
            using (var db = CreateContext())
            {
                db.Database.CommandTimeout = 5 * 60;

                var descriptions = db.QAchievements
                    .Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier && x.AchievementLabel == "Level")
                    .OrderBy(x => x.AchievementTitle)
                    .ToList()
                    .GroupBy(x => x.AchievementDescription)
                    .Where(x => x.Count() > 1)
                    .Select(x => new
                    {
                        Description = x.Key,
                        Level1 = x.First().AchievementTitle,
                        Level3 = x.Last().AchievementTitle
                    })
                    .ToDictionary(x => x.Description);

                var level3List = descriptions.Values.Select(x => x.Level3).ToList();

                var scores = CreateScoreQuery(filter, db)
                    .Where(x =>
                        x.ProgressPercent != null
                        && level3List.Contains(x.GradeItem.Achievement.AchievementTitle)
                        && x.GradeItem.Achievement.AchievementLabel == "Level"
                        && x.Gradebook.Event != null
                    )
                    .Join(db.QProgresses.Where(x =>
                            x.Gradebook.OrganizationIdentifier == filter.OrganizationIdentifier
                            && x.ProgressPercent != null
                            && x.GradeItem.Achievement.AchievementLabel == "Level"
                            && x.Gradebook.Event != null
                        ),
                        a => new { a.UserIdentifier, a.GradeItem.Achievement.AchievementDescription },
                        b => new { b.UserIdentifier, b.GradeItem.Achievement.AchievementDescription },
                        (a, b) => b
                    )
                    .Join(db.Persons.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier),
                        a => a.UserIdentifier,
                        b => b.UserIdentifier,
                        (a, b) => new { Progress = a, EmployerGroupIdentifier = b.EmployerGroupIdentifier }
                    )
                    .GroupJoin(db.Groups,
                        a => a.EmployerGroupIdentifier,
                        b => b.GroupIdentifier,
                        (a, b) => new { Score = a.Progress, Employers = b.DefaultIfEmpty() }
                    )
                    .SelectMany(x => x.Employers.Select(y => new
                    {
                        EmployerName = y.GroupName,
                        EmployerRegion = y.GroupRegion,
                        UserIdentifier = x.Score.UserIdentifier,
                        UserFullName = x.Score.Learner.UserFullName,
                        UserEmail = x.Score.Learner.UserEmail,
                        AchievementTitle = x.Score.GradeItem.Achievement.AchievementTitle,
                        AchievementDescription = x.Score.GradeItem.Achievement.AchievementDescription,
                        EventScheduledStart = x.Score.Gradebook.Event.EventScheduledStart,
                        EventScheduledEnd = x.Score.Gradebook.Event.EventScheduledEnd,
                        Percent = (decimal)x.Score.ProgressPercent
                    }))
                    .ToList()
                    .GroupBy(x => new
                    {
                        x.EmployerName,
                        x.EmployerRegion,
                        x.UserIdentifier,
                        x.UserFullName,
                        x.UserEmail,
                        x.AchievementTitle,
                        x.AchievementDescription
                    })
                    .Select(x =>
                    {
                        var latestScore = x.OrderByDescending(y => y.EventScheduledStart).First();
                        return new
                        {
                            x.Key.EmployerName,
                            x.Key.EmployerRegion,
                            x.Key.UserIdentifier,
                            x.Key.UserFullName,
                            x.Key.UserEmail,
                            x.Key.AchievementTitle,
                            x.Key.AchievementDescription,
                            latestScore.EventScheduledStart,
                            latestScore.EventScheduledEnd,
                            latestScore.Percent
                        };
                    })
                    .ToList();

                return scores
                    .Where(x => descriptions.ContainsKey(x.AchievementDescription))
                    .GroupBy(x => new
                    {
                        x.EmployerName,
                        x.EmployerRegion,
                        x.UserIdentifier,
                        x.UserFullName,
                        x.UserEmail,
                        x.AchievementDescription,
                    })
                    .Select(x =>
                    {
                        var description = descriptions[x.Key.AchievementDescription];
                        var level1 = x.FirstOrDefault(y => y.AchievementTitle == description.Level1);
                        var level3 = x.FirstOrDefault(y => y.AchievementTitle == description.Level3);
                        return level1 != null && level3 != null
                            ? new
                            {
                                EmployerName = x.Key.EmployerName,
                                EmployerRegion = x.Key.EmployerRegion,
                                UserIdentifier = x.Key.UserIdentifier,
                                UserFullName = x.Key.UserFullName,
                                UserEmail = x.Key.UserEmail,
                                AchievementDescription = x.Key.AchievementDescription,
                                EventScheduledStart = level1.EventScheduledStart,
                                EventScheduledEnd = level3.EventScheduledEnd,
                                Difference = level3.Percent - level1.Percent,
                                Levels = x
                                    .OrderBy(y => y.EventScheduledStart)
                                    .Select(y => new MostImprovedStudent.Level
                                    {
                                        AchievementTitle = y.AchievementTitle,
                                        EventScheduledStart = y.EventScheduledStart,
                                        Percent = y.Percent
                                    })
                                    .ToList()
                            }
                            : null;
                    })
                    .Where(x => x != null)
                    .GroupBy(x => x.AchievementDescription)
                    .Select(x => new MostImprovedStudent
                    {
                        AchievementDescription = x.Key,
                        TotalStudentCount = x.Count(),
                        Students = x.OrderByDescending(y => y.Difference)
                            .Select(y => new MostImprovedStudent.Student
                            {
                                EmployerName = y.EmployerName,
                                EmployerRegion = y.EmployerRegion,
                                UserIdentifier = y.UserIdentifier,
                                UserFullName = y.UserFullName,
                                UserEmail = y.UserEmail,
                                AchievementDescription = y.AchievementDescription,
                                EventScheduledStart = y.EventScheduledStart,
                                EventScheduledEnd = y.EventScheduledEnd,
                                Difference = y.Difference,
                                Levels = y.Levels
                            })
                            .Take(10)
                            .ToList()
                    })
                    .OrderBy(x => x.AchievementDescription)
                    .ToList();
            }
        }

        public List<PassingRateSummary> GetPassingRateSummaries(QProgressFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateScoreQuery(filter, db)
                    .Where(x => x.ProgressPercent != null && x.Gradebook.Achievement != null)
                    .GroupBy(x => new { x.Gradebook.Achievement.AchievementTitle, x.GradeItem.GradeItemName })
                    .Select(x => new PassingRateSummary
                    {
                        AchievementTitle = x.Key.AchievementTitle,
                        ScoreItemName = x.Key.GradeItemName,
                        StudentCount = x.Count(),
                        StudentAbove70Count = x.Count(y => y.ProgressPercent >= 0.695m)
                    });

                return query.ToList();
            }
        }

        public List<TopStudentSummary> GetTopStudentSummaries(QProgressFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateScoreQuery(filter, db)
                    .Join(db.VTopStudents,
                        a => a.ProgressIdentifier,
                        b => b.ProgressIdentifier,
                        (a, b) => new
                        {
                            b.EmployerRegion,
                            b.AchievementTitle,
                            b.GradeItemName,
                            b.UserFullName,
                            b.EmployerName,
                            b.ProgressPercent
                        }
                    )
                    .GroupBy(x => new
                    {
                        x.EmployerRegion,
                        x.AchievementTitle,
                        x.GradeItemName
                    })
                    .Select(x => new TopStudentSummary
                    {
                        EmployerRegion = x.Key.EmployerRegion ?? "(Unknown)",
                        AchievementTitle = x.Key.AchievementTitle,
                        ScoreItemName = x.Key.GradeItemName,
                        Students = x.Select(y => new TopStudentSummary.Student
                        {
                            FullName = y.UserFullName,
                            EmployerName = y.EmployerName,
                            Percent = y.ProgressPercent
                        })
                        .OrderByDescending(y => y.Percent)
                        .Take(10)
                    });

                return query.ToList();
            }
        }

        public List<LowestScoreStudent> GetLowestScoreStudents(QProgressFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateScoreQuery(filter, db)
                    .Where(x => x.ProgressPoints != null)
                    .Select(x => new LowestScoreStudent
                    {
                        GradebookTitle = x.Gradebook.GradebookTitle,
                        ScoreItemName = x.GradeItem.GradeItemName,
                        UserFullName = x.Learner.UserFullName,
                        Points = x.ProgressPoints.Value
                    })
                    .OrderBy(x => x.Points)
                    .Take(10);

                return query.ToList();
            }
        }

        public List<VStandard> GetGradebookStandards(Guid id)
        {
            using (var db = CreateContext())
            {
                return db.QGradeItemCompetencies
                    .Where(x => x.GradebookIdentifier == id)
                    .Select(x => x.Standard)
                    .Distinct()
                    .OrderBy(x => x.StandardTitle)
                    .ToList();
            }
        }

        public int CountEnrollments(QEnrollmentFilter filter)
        {
            using (var db = CreateContext())
                return CreateGradebookUserQuery(filter, db).Count();
        }

        public List<QEnrollment> GetEnrollments(QEnrollmentFilter filter, params Expression<Func<QEnrollment, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = CreateGradebookUserQuery(filter, db);

                query = query.Include(x => x.Learner).ApplyIncludes(includes);

                return query
                    .OrderBy(x => x.Learner.UserFullName)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        public List<EnrollmentForPeriodGrid> GetEnrollmentsForPeriodGrid(QEnrollmentFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateGradebookUserQuery(filter, db)
                    .Select(x => new EnrollmentForPeriodGrid
                    {
                        LearnerIdentifier = x.LearnerIdentifier,
                        PeriodIdentifier = x.PeriodIdentifier,
                        UserFullName = x.Learner.UserFullName,
                        UserEmail = x.Learner.UserEmail,
                        UserEmailAlternate = x.Learner.UserEmailAlternate,
                        PeriodName = x.Period.PeriodName,
                        PeriodStart = x.Period.PeriodStart,
                        PeriodEnd = x.Period.PeriodEnd,
                        Graded = db.QProgresses
                                    .Where(y => y.GradebookIdentifier == x.GradebookIdentifier && y.UserIdentifier == x.LearnerIdentifier && y.ProgressGraded != null)
                                    .Min(y => y.ProgressGraded)
                    });

                query = query.OrderBy(x => x.UserFullName);

                return query.ApplyPaging(filter).ToList();
            }
        }

        private static IQueryable<QEnrollment> CreateGradebookUserQuery(QEnrollmentFilter filter, InternalDbContext db)
        {
            var query = db.QEnrollments.Where(x => x.GradebookIdentifier == filter.GradebookIdentifier);

            if (filter.PeriodIdentifier.HasValue)
                query = query.Where(x => x.PeriodIdentifier == filter.PeriodIdentifier);

            if (filter.SearchKeyword.IsNotEmpty())
            {
                query = query.Where(x =>
                    x.Learner.UserFirstName.Contains(filter.SearchKeyword)
                    || x.Learner.UserEmail.Contains(filter.SearchKeyword)
                    || x.Learner.UserEmailAlternate.Contains(filter.SearchKeyword)
                );
            }

            if (filter.IsPeriodAssigned.HasValue)
            {
                query = filter.IsPeriodAssigned.Value
                    ? query.Where(x => x.PeriodIdentifier != null)
                    : query.Where(x => x.PeriodIdentifier == null);
            }

            if (filter.LearnerFullName.IsNotEmpty())
                query = query.Where(x => x.Learner.UserFullName.Contains(filter.LearnerFullName));

            if (filter.GradedSince.HasValue || filter.GradedBefore.HasValue)
            {
                var queryWithGraded = query.Select(x => new
                {
                    Enrolment = x,
                    Graded = db.QProgresses
                        .Where(y => y.GradebookIdentifier == x.GradebookIdentifier && y.UserIdentifier == x.LearnerIdentifier && y.ProgressGraded != null)
                        .Min(y => y.ProgressGraded)
                });

                if (filter.GradedSince.HasValue)
                    queryWithGraded = queryWithGraded.Where(x => x.Graded >= filter.GradedSince.Value);

                if (filter.GradedBefore.HasValue)
                    queryWithGraded = queryWithGraded.Where(x => x.Graded < filter.GradedBefore.Value);

                query = queryWithGraded.Select(x => x.Enrolment);
            }

            return query;
        }

        public bool EnrollmentExists(Guid gradebook)
        {
            using (var db = CreateContext())
                return db.QEnrollments.Any(x => x.GradebookIdentifier == gradebook);
        }

        public bool EnrollmentExists(Guid gradebook, Guid user)
        {
            using (var db = CreateContext())
                return db.QEnrollments.Any(x => x.GradebookIdentifier == gradebook && x.LearnerIdentifier == user);
        }

        public QEnrollment GetEnrollment(Guid gradebook, Guid user)
        {
            using (var db = CreateContext())
                return db.QEnrollments.FirstOrDefault(x => x.GradebookIdentifier == gradebook && x.LearnerIdentifier == user);
        }

        public GradebookState GetGradebookState(Guid gradebook)
        {
            return _aggregateSearch.GetState<GradebookAggregate>(gradebook) as GradebookState;
        }

        public int CountGradebooks(QGradebookFilter filter)
        {
            using (var db = CreateContext())
                return CreateQuery(filter, db).Count();
        }

        public bool GradebookExists(Guid gradebook)
        {
            using (var db = CreateContext())
                return db.QGradebooks.Where(x => x.GradebookIdentifier == gradebook).Any();
        }

        public QGradebook GetGradebook(Guid id, params Expression<Func<QGradebook, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.QGradebooks
                    .AsNoTracking()
                    .Where(x => x.GradebookIdentifier == id)
                    .ApplyIncludes(includes);

                return query.FirstOrDefault();
            }
        }

        public QGradebook GetGradebookByReference(string reference, Guid organization, params Expression<Func<QGradebook, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.QGradebooks
                    .AsNoTracking()
                    .Where(x => x.OrganizationIdentifier == organization && x.Reference == reference)
                    .ApplyIncludes(includes);

                return query.FirstOrDefault();
            }
        }

        public List<QGradebook> GetGradebooks(QGradebookFilter filter, params Expression<Func<QGradebook, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db).ApplyIncludes(includes);

                return query
                    .OrderByDescending(x => DbFunctions.TruncateTime(x.Event.EventScheduledStart))
                    .ThenBy(x => x.GradebookTitle)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        public List<QGradebook> GetEventGradebooks(Guid eventId)
        {
            using (var db = CreateContext())
            {
                return db.QGradebookEvents
                    .Where(x => x.EventIdentifier == eventId)
                    .Select(x => x.Gradebook)
                    .ToList();
            }
        }

        public List<QGradebook> GetRecentGradebooks(QGradebookFilter filter, int? take = null)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db)
                    .OrderByDescending(x => x.LastChangeTime)
                    .AsQueryable();

                if (take.HasValue)
                    query = query.Take(take.Value);

                return query.ToList();
            }
        }

        private static IQueryable<QGradebook> CreateQuery(QGradebookFilter filter, InternalDbContext db)
        {
            var query = db.QGradebooks.AsQueryable();

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.AchievementIdentifier.HasValue)
                query = query.Where(x => x.AchievementIdentifier == filter.AchievementIdentifier);

            if (filter.StandardIdentifier.HasValue)
                query = query.Where(x => x.FrameworkIdentifier == filter.StandardIdentifier);

            if (filter.PrimaryEventIdentifier.HasValue)
                query = query.Where(x => x.EventIdentifier == filter.PrimaryEventIdentifier);

            if (filter.GradebookEventIdentifier.HasValue)
                query = query.Where(x => x.GradebookEvents.Any(y => y.EventIdentifier == filter.GradebookEventIdentifier));

            if (filter.PeriodIdentifier.HasValue)
                query = query.Where(x => x.PeriodIdentifier == filter.PeriodIdentifier);

            if (filter.EventTitle.HasValue())
                query = query.Where(x => x.Event.EventTitle.Contains(filter.EventTitle));

            if (filter.EventScheduledSince.HasValue)
                query = query.Where(x => x.Event.EventScheduledStart >= filter.EventScheduledSince.Value);

            if (filter.EventScheduledBefore.HasValue)
                query = query.Where(x => x.Event.EventScheduledStart < filter.EventScheduledBefore.Value);

            if (filter.EventInstructorIdentifier.HasValue)
                query = query.Where(g => g.GradebookEvents.Any(x => x.Event.Attendees.Any(y => y.UserIdentifier == filter.EventInstructorIdentifier && y.AttendeeRole == "Instructor")));

            if (filter.GradebookTitle.HasValue())
                query = query.Where(x => x.GradebookTitle.Contains(filter.GradebookTitle));

            if (filter.GradebookTypes.IsNotEmpty())
                query = query.Where(x => filter.GradebookTypes.Contains(x.GradebookType));

            if (filter.GradebookCreatedSince.HasValue)
                query = query.Where(x => x.GradebookCreated >= filter.GradebookCreatedSince.Value);

            if (filter.GradebookCreatedBefore.HasValue)
                query = query.Where(x => x.GradebookCreated < filter.GradebookCreatedBefore.Value);

            if (filter.IsLocked.HasValue)
                query = query.Where(x => x.IsLocked == filter.IsLocked);

            if (filter.IsEventCancelled.HasValue)
            {
                query = filter.IsEventCancelled.Value
                    ? query.Where(x => x.Event.EventSchedulingStatus == "Cancelled")
                    : query.Where(x => x.Event.EventSchedulingStatus != "Cancelled");
            }

            if (filter.StudentIdentifier.HasValue)
                query = query.Where(x => x.Enrollments.Any(y => y.LearnerIdentifier == filter.StudentIdentifier));

            if (filter.GradebookPeriodIdentifier.HasValue)
                query = query.Where(x => x.PeriodIdentifier == filter.GradebookPeriodIdentifier);

            if (filter.GradebookIdentifier.HasValue)
                query = query.Where(x => x.GradebookIdentifier == filter.GradebookIdentifier);

            if (filter.CourseName.IsNotEmpty())
                query = query.Where(x => x.Courses.Any(y => y.CourseName.Contains(filter.CourseName)));

            return query;
        }

        public int CountGradebookScores(QProgressFilter filter)
        {
            using (var db = CreateContext())
                return CreateScoreQuery(filter, db).Count();
        }

        public List<QProgress> GetGradebookScores(QProgressFilter filter, params Expression<Func<QProgress, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = CreateScoreQuery(filter, db).ApplyIncludes(includes);

                return query
                    .OrderByDescending(x => x.ProgressAdded)
                    .ThenBy(x => x.ProgressIdentifier)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        public AddEnrollment CreateCommandToAddEnrollment(Guid? enrollment, Guid gradebook, Guid learner, Guid? period, DateTimeOffset? time, string comment)
        {
            Guid id;

            using (var db = new InternalDbContext())
            {
                var historyId = db.QEnrollmentHistories
                    .Where(x => x.GradebookIdentifier == gradebook && x.LearnerIdentifier == learner)
                    .Select(x => (Guid?)x.EnrollmentIdentifier)
                    .FirstOrDefault();

                if (historyId.HasValue)
                    id = historyId.Value;

                else if (enrollment.HasValue)
                    id = enrollment.Value;

                else
                    id = UniqueIdentifier.Create();

                return new AddEnrollment(gradebook, id, learner, period, time, comment);
            }
        }

        public AddProgress CreateCommandToAddProgress(Guid? progress, Guid gradebook, Guid gradeitem, Guid user)
        {
            Guid id;

            using (var db = new InternalDbContext())
            {
                var historyId = db.QProgressHistories
                    .Where(x => x.GradeItemIdentifier == gradeitem && x.UserIdentifier == user)
                    .Select(x => (Guid?)x.AggregateIdentifier)
                    .FirstOrDefault();

                if (historyId.HasValue)
                    id = historyId.Value;

                else if (progress.HasValue)
                    id = progress.Value;

                else
                    id = UniqueIdentifier.Create();

                return new AddProgress(id, gradebook, gradeitem, user);
            }
        }

        public List<AddProgress> CreateCommandsToAddProgresses(Guid? progress, Guid gradebook, Guid user, IEnumerable<Guid> gradeitems)
        {
            var result = new List<AddProgress>();
            if (gradeitems.Any())
                return result;

            using (var db = new InternalDbContext())
            {
                var historyItems = db.QProgressHistories
                    .Where(x => x.UserIdentifier == user && gradeitems.Contains(x.GradeItemIdentifier))
                    .Select(x => new
                    {
                        ProgressIdentifier = x.AggregateIdentifier,
                        x.GradeItemIdentifier
                    })
                    .ToArray();

                foreach (var itemId in gradeitems)
                {
                    var progressId = historyItems
                        .Where(x => x.GradeItemIdentifier == itemId)
                        .Select(x => (Guid?)x.ProgressIdentifier)
                        .FirstOrDefault();

                    if (!progressId.HasValue)
                        progressId = progress ?? UniqueIdentifier.Create();

                    result.Add(new AddProgress(progressId.Value, gradebook, itemId, user));
                }
            }

            return result;
        }

        public QProgress GetProgress(Guid gradebook, Guid gradeitem, Guid learner)
        {
            using (var db = CreateContext())
            {
                return db.QProgresses
                    .FirstOrDefault(x => x.GradebookIdentifier == gradebook
                        && x.GradeItemIdentifier == gradeitem
                        && x.UserIdentifier == learner
                    );
            }
        }

        public Guid? GetProgressIdentifier(Guid gradebook, Guid gradeitem, Guid learner)
        {
            using (var db = CreateContext())
            {
                return db.QProgresses
                    .Where(x => x.GradebookIdentifier == gradebook
                        && x.GradeItemIdentifier == gradeitem
                        && x.UserIdentifier == learner
                    )
                    .Select(x => (Guid?)x.ProgressIdentifier)
                    .FirstOrDefault();
            }
        }

        public QProgress GetProgress(Guid progress, params Expression<Func<QProgress, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.QProgresses
                    .Where(x => x.ProgressIdentifier == progress)
                    .ApplyIncludes(includes);

                return query.FirstOrDefault();
            }
        }

        public bool RecordHasProgress(Guid record)
        {
            using (var db = CreateContext())
                return db.QProgresses.Any(x => x.GradebookIdentifier == record);
        }

        public bool ItemHasProgress(Guid gradebook, Guid gradeItem)
        {
            using (var db = CreateContext())
                return db.QProgresses.Any(x => x.GradebookIdentifier == gradebook && x.GradeItemIdentifier == gradeItem);
        }

        public bool ProgressExists(Guid gradebook, Guid gradeitem, Guid learner)
        {
            using (var db = CreateContext())
                return db.QProgresses.Any(x => x.GradebookIdentifier == gradebook && x.GradeItemIdentifier == gradeitem && x.UserIdentifier == learner);
        }

        public T BindProgress<T>(
            Expression<Func<QProgress, T>> binder,
            Expression<Func<QProgress, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return ProgressReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);
        }

        public T[] BindProgresses<T>(
            Expression<Func<QProgress, T>> binder,
            Expression<Func<QProgress, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return ProgressReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);
        }

        private static IQueryable<QProgress> CreateScoreQuery(QProgressFilter filter, InternalDbContext db)
        {
            var query = db.QProgresses.AsQueryable();

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.Gradebook.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.GradebookIdentifier.HasValue)
                query = query.Where(x => x.GradebookIdentifier == filter.GradebookIdentifier);

            if (filter.AchievementTitle.HasValue())
                query = query.Where(x => x.Gradebook.Achievement.AchievementTitle.Contains(filter.AchievementTitle));

            if (filter.EventTitle.HasValue())
                query = query.Where(x => x.Gradebook.Event.EventTitle.Contains(filter.EventTitle));

            if (filter.EventScheduledSince.HasValue)
                query = query.Where(x => x.Gradebook.Event.EventScheduledStart >= filter.EventScheduledSince.Value);

            if (filter.EventScheduledBefore.HasValue)
                query = query.Where(x => x.Gradebook.Event.EventScheduledStart < filter.EventScheduledBefore.Value);

            if (filter.EventInstructorIdentifier.HasValue)
                query = query.Where(x => x.Gradebook.Event.Attendees.Any(y => y.UserIdentifier == filter.EventInstructorIdentifier && y.AttendeeRole == "Instructor"));

            query = AddEmployerFilter(db, filter, query);

            if (filter.GradebookTitle.HasValue())
                query = query.Where(x => x.Gradebook.GradebookTitle.Contains(filter.GradebookTitle));

            if (filter.GradebookCreatedSince.HasValue)
                query = query.Where(x => x.Gradebook.GradebookCreated >= filter.GradebookCreatedSince.Value);

            if (filter.GradebookCreatedBefore.HasValue)
                query = query.Where(x => x.Gradebook.GradebookCreated < filter.GradebookCreatedBefore.Value);

            if (filter.ItemName.HasValue())
                query = query.Where(x => x.GradeItem.GradeItemName.Contains(filter.ItemName));

            if (filter.ItemFormat.HasValue())
                query = query.Where(x => x.GradeItem.GradeItemFormat == filter.ItemFormat);

            if (filter.ItemTypes.IsNotEmpty())
                query = query.Where(x => filter.ItemTypes.Contains(x.GradeItem.GradeItemType));

            if (filter.ScoreText.HasValue())
                query = query.Where(x => x.ProgressText == filter.ScoreText);

            if (filter.ProgressStatus.HasValue())
                query = query.Where(x => x.ProgressStatus == filter.ProgressStatus);

            if (filter.IsScoreIgnored.HasValue)
                query = query.Where(x => x.ProgressIsIgnored == filter.IsScoreIgnored.Value);

            if (filter.ScoreComment.HasValue())
                query = query.Where(x => x.ProgressComment.Contains(filter.ScoreComment));

            if (filter.ScorePercentFrom.HasValue)
                query = query.Where(x => x.ProgressPercent >= filter.ScorePercentFrom);

            if (filter.ScorePercentThru.HasValue)
                query = query.Where(x => x.ProgressPercent <= filter.ScorePercentThru);

            if (filter.StudentName.HasValue())
                query = query.Where(x => x.Learner.UserFullName.Contains(filter.StudentName));

            if (filter.StudentUserIdentifier.HasValue)
                query = query.Where(x => x.UserIdentifier == filter.StudentUserIdentifier);

            if (filter.GradeItemIdentifier.HasValue)
                query = query.Where(x => x.GradeItemIdentifier == filter.GradeItemIdentifier);

            if (filter.UserPeriodIdentifier.HasValue)
            {
                query = query.Join(db.QEnrollments.Where(x => x.PeriodIdentifier == filter.UserPeriodIdentifier),
                    a => new { a.GradebookIdentifier, a.UserIdentifier },
                    b => new { b.GradebookIdentifier, UserIdentifier = b.LearnerIdentifier },
                    (a, b) => a
                );
            }

            if (filter.GradedSince.HasValue)
                query = query.Where(x => x.ProgressGraded >= filter.GradedSince.Value);

            if (filter.GradedBefore.HasValue)
                query = query.Where(x => x.ProgressGraded < filter.GradedBefore.Value);

            return query;
        }

        private static IQueryable<QProgress> AddEmployerFilter(InternalDbContext db, QProgressFilter filter, IQueryable<QProgress> query)
        {
            if (filter.StudentEmployerGroupIdentifier.HasValue)
            {
                if (filter.StudentEmployerGroupStatusIdentifier.HasValue)
                {
                    var employer = db.QGroups.FirstOrDefault(x => x.GroupIdentifier == filter.StudentEmployerGroupIdentifier);
                    if (employer?.GroupStatusItemIdentifier != filter.StudentEmployerGroupStatusIdentifier)
                        return query.Where(x => 1 == 0);
                }

                return db.QGroups
                    .Where(x => x.GroupIdentifier == filter.StudentEmployerGroupIdentifier)
                    .Join(db.Persons,
                        a => a.GroupIdentifier,
                        b => b.EmployerGroupIdentifier,
                        (a, b) => b
                    )
                    .Join(query,
                        a => a.UserIdentifier,
                        b => b.UserIdentifier,
                        (a, b) => b
                    );
            }

            if (filter.StudentEmployerGroupStatusIdentifier.HasValue)
            {
                return db.QGroups
                    .Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier && x.GroupStatusItemIdentifier == filter.StudentEmployerGroupStatusIdentifier)
                    .Join(db.Persons,
                        a => a.GroupIdentifier,
                        b => b.EmployerGroupIdentifier,
                        (a, b) => b
                    )
                    .Join(query,
                        a => a.UserIdentifier,
                        b => b.UserIdentifier,
                        (a, b) => b
                    );
            }

            return query;
        }

        public QGradeItem GetGradeItem(Guid item)
        {
            using (var db = CreateContext())
                return db.QGradeItems.FirstOrDefault(x => x.GradeItemIdentifier == item);
        }

        public T[] BindGradeItems<T>(
            Expression<Func<QGradeItem, T>> binder,
            Expression<Func<QGradeItem, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return GradeItemReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);
        }

        public List<QGradeItem> GetGradeItems(Guid gradebook)
        {
            using (var db = CreateContext())
            {
                return db.QGradeItems
                    .Where(x => x.GradebookIdentifier == gradebook)
                    .ToList();
            }
        }

        public bool IsGradeItemCodeUniqe(Guid gradebook, Guid excludeGradeItem, string code)
        {
            using (var db = CreateContext())
            {
                return !db.QGradeItems
                    .Where(x => x.GradebookIdentifier == gradebook
                                && x.GradeItemIdentifier != excludeGradeItem
                                && x.GradeItemCode == code)
                    .Any();
            }
        }

        public List<VGradeItemHierarchy> GetGradeItemHierarchies(Guid gradebook)
        {
            using (var db = CreateContext())
            {
                return db.VGradeItemHierarchies
                    .Where(x => x.GradebookIdentifier == gradebook)
                    .OrderBy(x => x.PathSequence)
                    .ToList();
            }
        }

        public string BuildGradebookReport(Guid gradebook)
        {
            var report = new GradebookReport();
            return report.Build(this, gradebook);
        }

        public int CountGradeItems(QGradeItemFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateItemQuery(filter, db).Count();
            }
        }

        public bool GradeItemExists(Guid item)
        {
            using (var db = CreateContext())
            {
                return db.QGradeItems.Where(x => x.GradeItemIdentifier == item).Any();
            }
        }

        public QGradeItem GetGradeItemByHook(string hook)
        {
            using (var db = CreateContext())
                return db.QGradeItems.FirstOrDefault(x => x.GradeItemHook == hook);
        }

        public List<QGradeItem> GetGradeItems(QGradeItemFilter filter, params Expression<Func<QGradeItem, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = CreateItemQuery(filter, db).ApplyIncludes(includes);

                return query
                    .OrderBy(x => x.GradeItemName)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        private static IQueryable<QGradeItem> CreateItemQuery(QGradeItemFilter filter, InternalDbContext db)
        {
            var query = db.QGradeItems.AsQueryable();

            if (filter.AchievementIdentifier.HasValue)
                query = query.Where(x => x.AchievementIdentifier == filter.AchievementIdentifier);

            if (filter.AssessmentFormIdentifier.HasValue)
                query = query.Where(x => x.Activities.Any(a => a.AssessmentFormIdentifier == filter.AssessmentFormIdentifier.Value));

            if (filter.ItemFormat.HasValue())
                query = query.Where(x => x.GradeItemFormat == filter.ItemFormat);

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.Gradebook.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (!filter.GradeItemIdentifiers.IsEmpty())
                query = query.Where(x => filter.GradeItemIdentifiers.Contains(x.GradeItemIdentifier));

            if (filter.GradebookIdentifier.HasValue)
                query = query.Where(x => x.GradebookIdentifier == filter.GradebookIdentifier);

            if (filter.GradebookIdentifiers.IsNotEmpty())
                query = query.Where(x => filter.GradebookIdentifiers.Contains(x.GradebookIdentifier));

            if (filter.ParentGradeItemIdentifier.HasValue)
                query = query.Where(x => x.ParentGradeItemIdentifier == filter.ParentGradeItemIdentifier);

            return query;
        }

        #region Validations

        public QGradebookCompetencyValidation GetValidation(Guid gradebookIdentifier, Guid userIdentifier, Guid competencyIdentifier, params Expression<Func<QGradebookCompetencyValidation, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.QGradebookCompetencyValidations
                    .Where(x => x.GradebookIdentifier == gradebookIdentifier && x.UserIdentifier == userIdentifier && x.CompetencyIdentifier == competencyIdentifier)
                    .ApplyIncludes(includes);

                return query.FirstOrDefault();
            }
        }

        public int CountValidations(QGradebookCompetencyValidationFilter filter)
        {
            using (var db = CreateContext())
                return CreateValidationQuery(filter, db).Count();
        }

        public List<QGradebookCompetencyValidation> GetValidations(QGradebookCompetencyValidationFilter filter, params Expression<Func<QGradebookCompetencyValidation, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = CreateValidationQuery(filter, db).ApplyIncludes(includes);

                return query
                    .OrderBy(x => x.Gradebook.GradebookTitle)
                    .ThenBy(x => x.Student.UserFullName)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        private static IQueryable<QGradebookCompetencyValidation> CreateValidationQuery(QGradebookCompetencyValidationFilter filter, InternalDbContext db)
        {
            var query = db.QGradebookCompetencyValidations.AsQueryable();

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.Gradebook.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.AchievementTitle.HasValue())
                query = query.Where(x => x.Gradebook.Achievement.AchievementTitle.Contains(filter.AchievementTitle));

            if (filter.EventTitle.HasValue())
                query = query.Where(x => x.Gradebook.Event.EventTitle.Contains(filter.EventTitle));

            if (filter.EventScheduledSince.HasValue)
                query = query.Where(x => x.Gradebook.Event.EventScheduledStart >= filter.EventScheduledSince.Value);

            if (filter.EventScheduledBefore.HasValue)
                query = query.Where(x => x.Gradebook.Event.EventScheduledStart < filter.EventScheduledBefore.Value);

            if (filter.EventInstructorIdentifier.HasValue)
                query = query.Where(x => x.Gradebook.Event.Attendees.Any(y => y.UserIdentifier == filter.EventInstructorIdentifier && y.AttendeeRole == "Instructor"));

            if (filter.GradebookTitle.HasValue())
                query = query.Where(x => x.Gradebook.GradebookTitle.Contains(filter.GradebookTitle));

            if (filter.GradebookCreatedSince.HasValue)
                query = query.Where(x => x.Gradebook.GradebookCreated >= filter.GradebookCreatedSince.Value);

            if (filter.GradebookCreatedBefore.HasValue)
                query = query.Where(x => x.Gradebook.GradebookCreated < filter.GradebookCreatedBefore.Value);

            if (filter.PointsFrom.HasValue)
                query = query.Where(x => x.ValidationPoints >= filter.PointsFrom);

            if (filter.PointsThru.HasValue)
                query = query.Where(x => x.ValidationPoints <= filter.PointsThru);

            if (filter.StudentName.HasValue())
                query = query.Where(x => x.Student.UserFullName.Contains(filter.StudentName));

            if (filter.CompetencyIdentifier.HasValue)
                query = query.Where(x => x.CompetencyIdentifier == filter.CompetencyIdentifier);

            if (filter.NotAchievedMastery)
                query = query.Where(x => x.Standard.MasteryPoints != null && (x.ValidationPoints == null || x.ValidationPoints < x.Standard.MasteryPoints));

            if (filter.UserIdentifier.HasValue)
                query = query.Where(x => x.UserIdentifier == filter.UserIdentifier);

            if (filter.GradebookPeriodIdentifier.HasValue)
                query = query.Where(x => x.Gradebook.PeriodIdentifier == filter.GradebookPeriodIdentifier);

            if (filter.StudentEmployerGroupIdentifier.HasValue)
            {
                query = query.Join(db.Persons.Where(x => x.EmployerGroupIdentifier == filter.StudentEmployerGroupIdentifier),
                    a => new { a.UserIdentifier, a.Gradebook.OrganizationIdentifier },
                    b => new { b.UserIdentifier, b.OrganizationIdentifier },
                    (a, b) => a
                );
            }

            if (filter.UserPeriodIdentifier.HasValue)
            {
                query = query.Join(db.QEnrollments.Where(x => x.PeriodIdentifier == filter.UserPeriodIdentifier),
                    a => new { a.GradebookIdentifier, a.UserIdentifier },
                    b => new { b.GradebookIdentifier, UserIdentifier = b.LearnerIdentifier },
                    (a, b) => a
                );
            }

            return query;
        }

        #endregion

        public int CountStatements(VStatementFilter filter)
        {
            using (var db = CreateContext())
                return CreateQuery(filter, db).Count();
        }

        public List<VStatement> GetStatements(VStatementFilter filter, params Expression<Func<VStatement, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db).ApplyIncludes(includes);

                if (filter.OrderBy.IsNotEmpty())
                    query = query.OrderBy(filter.OrderBy);
                else
                    query = query.OrderByDescending(x => x.StatementTime);

                return query.ApplyPaging(filter).ToList();
            }
        }

        private static IQueryable<VStatement> CreateQuery(VStatementFilter filter, InternalDbContext db)
        {
            var query = db.VStatements.AsQueryable();

            if (filter.StatementTimeSince.HasValue)
                query = query.Where(x => filter.StatementTimeSince <= x.StatementTime);

            if (filter.StatementTimeBefore.HasValue)
                query = query.Where(x => x.StatementTime < filter.StatementTimeBefore);

            if (filter.LearnerName.IsNotEmpty())
                query = query.Where(x => x.LearnerName.Contains(filter.LearnerName));

            if (filter.ObjectUrl.IsNotEmpty())
                query = query.Where(x => x.ObjectId.Contains(filter.ObjectUrl));

            return query;
        }

        #region Helpers

        private class GradeItemReadHelper : ReadHelper<QGradeItem>
        {
            public static readonly GradeItemReadHelper Instance = new GradeItemReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<QGradeItem>, TResult> func)
            {
                using (var context = new InternalDbContext(false))
                {
                    var query = context.QGradeItems.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        private class ProgressReadHelper : ReadHelper<QProgress>
        {
            public static readonly ProgressReadHelper Instance = new ProgressReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<QProgress>, TResult> func)
            {
                using (var context = new InternalDbContext(false))
                {
                    var query = context.QProgresses.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        #endregion
    }
}

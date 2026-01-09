using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Contents.Read;
using InSite.Application.Records.Read;
using InSite.Domain.Records;
using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Persistence
{
    public class JournalSearch : IJournalSearch
    {
        public LogbookEnrollmentStatus GetEnrollmentStatus(Guid journalSetupIdentifier, Guid userIdentifier)
        {
            var role = JournalSetupUserRole.Learner.ToString();

            using (var db = CreateContext())
            {
                var userEnrolled = db.QJournalSetupUsers.AsNoTracking()
                    .Where(x => x.JournalSetupIdentifier == journalSetupIdentifier && x.UserIdentifier == userIdentifier && x.UserRole == role)
                    .Any();

                if (userEnrolled)
                    return LogbookEnrollmentStatus.UserEnrolled;

                var groupEnrolled = db.QJournalSetupGroups
                    .Where(x => x.JournalSetupIdentifier == journalSetupIdentifier)
                    .Join(db.QMemberships.Where(x => x.UserIdentifier == userIdentifier),
                        a => a.GroupIdentifier,
                        b => b.GroupIdentifier,
                        (a, b) => a
                    )
                    .Any();

                return groupEnrolled ? LogbookEnrollmentStatus.GroupEnrolled : LogbookEnrollmentStatus.NotEnrolled;
            }
        }

        public int CountLearnerJournals(Guid organizationIdentifier, Guid userIdentifier)
        {
            var role = JournalSetupUserRole.Learner.ToString();

            using (var db = CreateContext())
            {
                return db.QJournalSetupUsers.AsNoTracking()
                    .Where(x => x.JournalSetup.OrganizationIdentifier == organizationIdentifier && x.UserIdentifier == userIdentifier && x.UserRole == role)
                    .Count();
            }
        }

        public List<UserJournalDetail> GetEnrolledJournals(Guid organizationIdentifier, Guid userIdentifier, string language)
        {
            List<UserJournalDetail> groupJournals1, groupJournals2;

            using (var db = CreateContext())
            {
                groupJournals1 = GetJournalsByGroups(db, organizationIdentifier, userIdentifier, language);
                groupJournals2 = GetJournalsByProgramGroups(db, organizationIdentifier, userIdentifier, language);
            }

            var userJournals = GetLearnerJournals(organizationIdentifier, userIdentifier, language);

            var missingJournals1 = groupJournals1
                .Where(x => !userJournals.Any(y => y.JournalSetupIdentifier == x.JournalSetupIdentifier))
                .ToList();

            userJournals.AddRange(missingJournals1);

            var missingJournals2 = groupJournals2
                .Where(x => !userJournals.Any(y => y.JournalSetupIdentifier == x.JournalSetupIdentifier))
                .ToList();

            userJournals.AddRange(missingJournals2);

            userJournals.Sort((a, b) => a.JournalSetupCreated.CompareTo(b.JournalSetupCreated));

            return userJournals;
        }

        private List<UserJournalDetail> GetJournalsByGroups(InternalDbContext db, Guid organizationIdentifier, Guid userIdentifier, string language)
        {
            return db.QJournalSetupGroups
                .Where(x => x.JournalSetup.OrganizationIdentifier == organizationIdentifier)
                .Join(db.QMemberships.Where(x => x.UserIdentifier == userIdentifier),
                    a => a.GroupIdentifier,
                    b => b.GroupIdentifier,
                    (a, b) => a
                )
                .Select(x => new UserJournalDetail
                {
                    JournalSetupIdentifier = x.JournalSetupIdentifier,
                    UserIdentifier = userIdentifier,
                    JournalSetupLocked = x.JournalSetup.JournalSetupLocked,
                    Title = CoreFunctions.GetContentText(x.JournalSetupIdentifier, JournalSetupState.ContentLabels.Title, language),
                    JournalSetupCreated = x.JournalSetup.JournalSetupCreated,
                    ExperienceCount = 0
                })
                .Distinct()
                .ToList();
        }

        private List<UserJournalDetail> GetJournalsByProgramGroups(InternalDbContext db, Guid organizationIdentifier, Guid userIdentifier, string language)
        {
            return db.QMemberships
                .Where(x => x.UserIdentifier == userIdentifier && x.Group.OrganizationIdentifier == organizationIdentifier)
                .Join(db.TProgramGroupEnrollments,
                    m => m.GroupIdentifier,
                    e => e.GroupIdentifier,
                    (m, e) => e
                )
                .Join(db.TTasks.Where(x => x.OrganizationIdentifier == organizationIdentifier && x.ObjectType == "Logbook"),
                    e => e.ProgramIdentifier,
                    t => t.ProgramIdentifier,
                    (e, t) => t
                )
                .Join(db.QJournalSetups,
                    t => t.ObjectIdentifier,
                    l => l.JournalSetupIdentifier,
                    (t, l) => new UserJournalDetail
                    {
                        JournalSetupIdentifier = l.JournalSetupIdentifier,
                        UserIdentifier = userIdentifier,
                        JournalSetupLocked = l.JournalSetupLocked,
                        Title = CoreFunctions.GetContentText(l.JournalSetupIdentifier, JournalSetupState.ContentLabels.Title, language),
                        JournalSetupCreated = l.JournalSetupCreated,
                        ExperienceCount = 0
                    }
                )
                .Distinct()
                .ToList();
        }

        public List<UserJournalDetail> GetLearnerJournals(Guid organizationIdentifier, Guid userIdentifier, string language)
        {
            var role = JournalSetupUserRole.Learner.ToString();

            using (var db = CreateContext())
            {
                return db.QJournalSetupUsers.AsNoTracking()
                    .Where(x => x.JournalSetup.OrganizationIdentifier == organizationIdentifier && x.UserIdentifier == userIdentifier && x.UserRole == role)
                    .Select(x => new UserJournalDetail
                    {
                        JournalSetupIdentifier = x.JournalSetupIdentifier,
                        UserIdentifier = userIdentifier,
                        JournalSetupLocked = x.JournalSetup.JournalSetupLocked,
                        Title = CoreFunctions.GetContentText(x.JournalSetupIdentifier, JournalSetupState.ContentLabels.Title, language),
                        JournalSetupCreated = x.JournalSetup.JournalSetupCreated,
                        ExperienceCount = (int?)x.JournalSetup.Journals
                                            .Where(y => y.UserIdentifier == userIdentifier)
                                            .Sum(y => y.Experiences.Count()) ?? 0
                    })
                    .OrderByDescending(x => x.JournalSetupCreated)
                    .ToList();
            }
        }

        public QJournal GetJournal(Guid journalIdentifier, params Expression<Func<QJournal, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.QJournals
                    .AsNoTracking()
                    .Where(x => x.JournalIdentifier == journalIdentifier)
                    .ApplyIncludes(includes);

                return query.FirstOrDefault();
            }
        }

        public QJournal GetJournal(Guid journalSetupIdentifier, Guid userIdentifier, params Expression<Func<QJournal, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.QJournals
                    .AsNoTracking()
                    .Where(x => x.JournalSetupIdentifier == journalSetupIdentifier && x.UserIdentifier == userIdentifier)
                    .ApplyIncludes(includes);

                return query.FirstOrDefault();
            }
        }

        public bool JournalExists(QJournalFilter filter)
        {
            using (var db = CreateContext())
                return CreateJournalQuery(filter, db).Any();
        }

        public int CountJournals(QJournalFilter filter)
        {
            using (var db = CreateContext())
                return CreateJournalQuery(filter, db).Count();
        }

        public List<QJournal> GetJournals(QJournalFilter filter, params Expression<Func<QJournal, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = CreateJournalQuery(filter, db).ApplyIncludes(includes);

                return query
                    .OrderByDescending(x => x.JournalCreated)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        private static IQueryable<QJournal> CreateJournalQuery(QJournalFilter filter, InternalDbContext db)
        {
            var query = db.QJournals.AsNoTracking().AsQueryable();

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.JournalSetup.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.JournalSetupIdentifier.HasValue)
                query = query.Where(x => x.JournalSetupIdentifier == filter.JournalSetupIdentifier);

            if (filter.CompetencyStandardIdentifier.HasValue)
                query = query.Where(x => x.Experiences.Any(y => y.Competencies.Any(z => z.CompetencyStandardIdentifier == filter.CompetencyStandardIdentifier)));

            if (filter.UserIdentifier.HasValue)
                query = query.Where(x => x.UserIdentifier == filter.UserIdentifier);

            if (filter.ValidatorUserIdentifier.HasValue)
            {
                var validatorRole = JournalSetupUserRole.Validator.ToString();
                query = query.Where(x => x.JournalSetup.Users.Any(y => y.UserIdentifier == filter.ValidatorUserIdentifier && y.UserRole == validatorRole));
            }

            return query;
        }

        public List<EntrySummaryItem> GetEntrySummary(QExperienceFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateExperienceQuery(filter, db);

                return query
                    .GroupJoin(db.QExperienceCompetencies,
                        a => a.ExperienceIdentifier,
                        b => b.ExperienceIdentifier,
                        (a, b) => new { Experience = a, Competencies = b.DefaultIfEmpty() }
                    )
                    .SelectMany(x => x.Competencies.Select(y => new EntrySummaryItem
                    {
                        UserIdentifier = x.Experience.Journal.UserIdentifier,
                        JournalSetupIdentifier = x.Experience.Journal.JournalSetupIdentifier,
                        ExperienceIdentifier = x.Experience.ExperienceIdentifier,
                        CompetencyStandardIdentifier = y.CompetencyStandardIdentifier,
                        User = x.Experience.Journal.User.UserFullName,
                        Logbook = x.Experience.Journal.JournalSetup.JournalSetupName,
                        GAC = y.Competency.AreaTitle,
                        Competency = y.Competency.CompetencyTitle,
                        Created = x.Experience.ExperienceCreated,
                        Started = x.Experience.ExperienceStarted,
                        Stopped = x.Experience.ExperienceStopped,
                        Employer = x.Experience.Employer,
                        Supervisor = x.Experience.Supervisor,
                        TrainingLocation = x.Experience.TrainingLocation,
                        Class = x.Experience.Journal.JournalSetup.Event.EventTitle,
                        EntryNumber = x.Experience.Sequence,
                        Validated = x.Experience.ExperienceValidated.HasValue,
                        EntryHours = x.Experience.ExperienceHours,
                        CompetencyHours = y.CompetencyHours,
                        CompetencySatisfactionLevel = y.SatisfactionLevel,
                        CompetencySkillRating = y.SkillRating
                    }))
                    .ToList();
            }
        }

        public QExperienceCompetency GetExperienceCompetency(
            Guid experienceIdentifier,
            Guid competencyIdentifier,
            params Expression<Func<QExperienceCompetency, object>>[] includes
            )
        {
            using (var db = CreateContext())
            {
                var query = db.QExperienceCompetencies
                    .AsNoTracking()
                    .Where(x =>
                        x.ExperienceIdentifier == experienceIdentifier
                        && x.CompetencyStandardIdentifier == competencyIdentifier
                    )
                    .ApplyIncludes(includes);

                return query.FirstOrDefault();
            }
        }

        public int CountExperienceCompetencies(QExperienceCompetencyFilter filter)
        {
            using (var db = CreateContext())
                return CreateExperienceCompetencyQuery(filter, db).Count();
        }

        public int CountExperienceCompetenciesFrameworks(QExperienceCompetencyFilter filter)
        {
            using (var db = CreateContext())
                return CreateExperienceCompetencyQuery(filter, db)
                    .Select(x => x.Experience.Journal.JournalSetup.Framework)
                    .Distinct()
                    .Count();
        }

        public List<QExperienceCompetency> GetExperienceCompetencies(
            QExperienceCompetencyFilter filter,
            params Expression<Func<QExperienceCompetency, object>>[] includes
            )
        {
            using (var db = CreateContext())
            {
                var query = CreateExperienceCompetencyQuery(filter, db).ApplyIncludes(includes);

                return query
                    .OrderByDescending(x => x.Experience.Journal.JournalSetup.JournalSetupCreated)
                    .ThenByDescending(x => x.Experience.Journal.User.UserFullName)
                    .ThenByDescending(x => x.Experience.ExperienceCreated)
                    .ThenBy(x => x.Competency.CompetencyTitle)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        private static IQueryable<QExperienceCompetency> CreateExperienceCompetencyQuery(QExperienceCompetencyFilter filter, InternalDbContext db)
        {
            var query = db.QExperienceCompetencies.AsNoTracking().AsQueryable();

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.Experience.Journal.JournalSetup.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.JournalSetupIdentifier.HasValue)
                query = query.Where(x => x.Experience.Journal.JournalSetupIdentifier == filter.JournalSetupIdentifier);

            if (filter.UserIdentifier.HasValue)
                query = query.Where(x => x.Experience.Journal.UserIdentifier == filter.UserIdentifier);

            if (filter.ValidatorUserIdentifier.HasValue)
            {
                var validatorRole = JournalSetupUserRole.Validator.ToString();
                query = query.Where(x => x.Experience.Journal.JournalSetup.Users.Any(y => y.UserIdentifier == filter.ValidatorUserIdentifier && y.UserRole == validatorRole));
            }

            if (filter.ExperienceIdentifier.HasValue)
                query = query.Where(x => x.ExperienceIdentifier == filter.ExperienceIdentifier);

            if (filter.CompetencyStandardIdentifier.IsNotEmpty())
                query = query.Where(x => filter.CompetencyStandardIdentifier.Contains(x.CompetencyStandardIdentifier));

            if (filter.CreatedSince.HasValue)
                query = query.Where(x => x.Experience.ExperienceCreated >= filter.CreatedSince.Value);

            if (filter.CreatedBefore.HasValue)
                query = query.Where(x => x.Experience.ExperienceCreated < filter.CreatedBefore.Value);

            if (filter.IsValidated.HasValue)
            {
                query = filter.IsValidated.Value
                    ? query.Where(x => x.Experience.ExperienceValidated != null)
                    : query.Where(x => x.Experience.ExperienceValidated == null);
            }

            return query;
        }

        public List<QExperienceCompetency> GetExperienceCompetencies(Guid experienceIdentifier, params Expression<Func<QExperienceCompetency, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.QExperienceCompetencies
                    .AsNoTracking()
                    .Where(x => x.ExperienceIdentifier == experienceIdentifier)
                    .ApplyIncludes(includes);

                return query.ToList();
            }
        }

        public List<QExperienceCompetency> GetExperienceCompetencies(Expression<Func<QExperienceCompetency, bool>> filter, params Expression<Func<QExperienceCompetency, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.QExperienceCompetencies
                    .AsNoTracking()
                    .Where(filter)
                    .ApplyIncludes(includes);

                return query.ToList();
            }
        }

        public QExperience GetExperience(Guid experienceIdentifier, params Expression<Func<QExperience, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.QExperiences
                    .Where(x => x.ExperienceIdentifier == experienceIdentifier)
                    .ApplyIncludes(includes);

                return query.FirstOrDefault();
            }
        }

        public bool ExperienceExists(QExperienceFilter filter)
        {
            using (var db = CreateContext())
                return CreateExperienceQuery(filter, db).Any();
        }

        public int CountExperiences(QExperienceFilter filter)
        {
            using (var db = CreateContext())
                return CreateExperienceQuery(filter, db).Count();
        }

        public List<QExperience> GetExperiences(QExperienceFilter filter, params Expression<Func<QExperience, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = CreateExperienceQuery(filter, db).ApplyIncludes(includes);

                if (string.Equals(filter.OrderBy, "SearchResultsDefault", StringComparison.OrdinalIgnoreCase))
                {
                    query = query
                        .OrderByDescending(x => x.Journal.JournalSetup.JournalSetupCreated)
                        .ThenByDescending(x => x.Journal.User.UserFullName)
                        .ThenByDescending(x => x.ExperienceCreated);
                }
                else
                    query = query.OrderByDescending(x => x.ExperienceCreated);

                return query.ApplyPaging(filter).ToList();
            }
        }

        private static IQueryable<QExperience> CreateExperienceQuery(QExperienceFilter filter, InternalDbContext db)
        {
            var query = db.QExperiences.AsNoTracking().AsQueryable();

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.Journal.JournalSetup.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.JournalSetupIdentifier.HasValue)
                query = query.Where(x => x.Journal.JournalSetupIdentifier == filter.JournalSetupIdentifier);

            if (filter.JournalIdentifier.HasValue)
                query = query.Where(x => x.JournalIdentifier == filter.JournalIdentifier);

            if (filter.UserIdentifier.HasValue)
                query = query.Where(x => x.Journal.UserIdentifier == filter.UserIdentifier);

            if (filter.CompetencyStandardIdentifier.HasValue)
                query = query.Where(x => x.Competencies.Any(y => y.CompetencyStandardIdentifier == filter.CompetencyStandardIdentifier));

            if (filter.TrainingType.HasValue())
                query = query.Where(x => x.TrainingType == filter.TrainingType);

            if (filter.ValidatorUserIdentifier.HasValue)
            {
                var validatorRole = JournalSetupUserRole.Validator.ToString();
                query = query.Where(x => x.Journal.JournalSetup.Users.Any(y => y.UserIdentifier == filter.ValidatorUserIdentifier && y.UserRole == validatorRole));
            }

            if (filter.CreatedSince.HasValue)
                query = query.Where(x => x.ExperienceCreated >= filter.CreatedSince.Value);

            if (filter.CreatedBefore.HasValue)
                query = query.Where(x => x.ExperienceCreated < filter.CreatedBefore.Value);

            if (filter.IsValidated.HasValue)
            {
                query = filter.IsValidated.Value
                    ? query.Where(x => x.ExperienceValidated != null)
                    : query.Where(x => x.ExperienceValidated == null);
            }

            return query;
        }

        public QComment GetJournalComment(Guid commentIdentifier)
        {
            using (var db = CreateContext())
            {
                var query = db.QComments.AsNoTracking().Where(x => x.CommentIdentifier == commentIdentifier);
                return query.FirstOrDefault();
            }
        }

        public List<QComment> GetJournalComments(Guid journalIdentifier)
        {
            using (var db = CreateContext())
            {
                var query = db.QComments.AsNoTracking().Where(x => x.LogbookIdentifier == journalIdentifier);
                query = query.OrderByDescending(x => x.CommentPosted);
                return query.ToList();
            }
        }

        public int CountJournalSetups(QJournalSetupFilter filter)
        {
            using (var db = CreateContext())
                return CreateJournalSetupQuery(filter, db).Count();
        }

        public List<QJournalSetup> GetJournalSetups(IEnumerable<Guid> ids, params Expression<Func<QJournalSetup, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.QJournalSetups
                    .AsNoTracking()
                    .Where(x => ids.Contains(x.JournalSetupIdentifier))
                    .ApplyIncludes(includes);

                return query.ToList();
            }
        }

        public List<QJournalSetup> GetJournalSetups(QJournalSetupFilter filter, params Expression<Func<QJournalSetup, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = CreateJournalSetupQuery(filter, db).ApplyIncludes(includes);

                if (filter.OrderBy.IsNotEmpty())
                    query = query.OrderBy(filter.OrderBy);
                else
                    query = query.OrderByDescending(x => x.JournalSetupCreated);

                return query.ApplyPaging(filter).ToList();
            }
        }

        private static IQueryable<QJournalSetup> CreateJournalSetupQuery(QJournalSetupFilter filter, InternalDbContext db)
        {
            var query = db.QJournalSetups.AsNoTracking().AsQueryable();

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.AchievementIdentifier.HasValue)
                query = query.Where(x => x.AchievementIdentifier == filter.AchievementIdentifier);

            if (filter.JournalSetupName.IsNotEmpty())
                query = query.Where(x => x.JournalSetupName.Contains(filter.JournalSetupName));

            if (filter.JournalSetupCreatedSince.HasValue)
                query = query.Where(x => x.JournalSetupCreated >= filter.JournalSetupCreatedSince.Value);

            if (filter.JournalSetupCreatedBefore.HasValue)
                query = query.Where(x => x.JournalSetupCreated < filter.JournalSetupCreatedBefore.Value);

            if (filter.IsLocked.HasValue)
            {
                if (filter.IsLocked.Value)
                    query = query.Where(x => x.JournalSetupLocked != null);
                else
                    query = query.Where(x => x.JournalSetupLocked == null);
            }

            if (filter.EventTitle.IsNotEmpty())
                query = query.Where(x => x.Event.EventTitle.Contains(filter.EventTitle));

            if (filter.EventScheduledSince.HasValue)
                query = query.Where(x => x.Event.EventScheduledStart >= filter.EventScheduledSince.Value);

            if (filter.EventScheduledBefore.HasValue)
                query = query.Where(x => x.Event.EventScheduledStart < filter.EventScheduledBefore.Value);

            if (filter.ValidatorUserIdentifier.HasValue)
            {
                var validatorRole = JournalSetupUserRole.Validator.ToString();
                query = query.Where(x => x.Users.Any(y => y.UserIdentifier == filter.ValidatorUserIdentifier && y.UserRole == validatorRole));
            }

            return query;
        }

        public QJournalSetup GetJournalSetup(Guid journalSetupIdentifier, params Expression<Func<QJournalSetup, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.QJournalSetups.AsNoTracking()
                    .Where(x => x.JournalSetupIdentifier == journalSetupIdentifier)
                    .ApplyIncludes(includes);

                return query.FirstOrDefault();
            }
        }

        public int GetNextFieldSequence(Guid journalSetupIdentifier)
        {
            using (var db = CreateContext())
            {
                var maxSequence = db.QJournalSetupFields
                    .Where(x => x.JournalSetupIdentifier == journalSetupIdentifier)
                    .Max(x => (int?)x.Sequence);

                return maxSequence.HasValue ? maxSequence.Value + 1 : 1;
            }
        }

        public QJournalSetupField GetJournalSetupField(Guid journalSetupIdentifier, string fieldType, params Expression<Func<QJournalSetupField, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.QJournalSetupFields
                    .AsNoTracking()
                    .Where(x => x.JournalSetupIdentifier == journalSetupIdentifier && x.FieldType == fieldType)
                    .ApplyIncludes(includes);

                return query.FirstOrDefault();
            }
        }

        public QJournalSetupField GetJournalSetupField(Guid journalSetupFieldIdentifier, params Expression<Func<QJournalSetupField, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.QJournalSetupFields
                    .AsNoTracking()
                    .Where(x => x.JournalSetupFieldIdentifier == journalSetupFieldIdentifier)
                    .ApplyIncludes(includes);

                return query.FirstOrDefault();
            }
        }

        public List<QJournalSetupField> GetJournalSetupFields(Guid journalSetupIdentifier)
        {
            using (var db = CreateContext())
            {
                return db.QJournalSetupFields
                    .AsNoTracking()
                    .Where(x => x.JournalSetupIdentifier == journalSetupIdentifier)
                    .OrderBy(x => x.Sequence)
                    .ToList();
            }
        }

        public QCompetencyRequirement GetCompetencyRequirement(Guid journalSetupIdentifier, Guid standardIdentifier, params Expression<Func<QCompetencyRequirement, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.QCompetencyRequirements
                    .AsNoTracking()
                    .Where(x => x.JournalSetupIdentifier == journalSetupIdentifier && x.CompetencyStandardIdentifier == standardIdentifier)
                    .ApplyIncludes(includes);

                return query.FirstOrDefault();
            }
        }

        public List<QCompetencyRequirement> GetCompetencyRequirements(Guid journalSetupIdentifier, params Expression<Func<QCompetencyRequirement, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.QCompetencyRequirements
                    .AsNoTracking()
                    .Where(x => x.JournalSetupIdentifier == journalSetupIdentifier)
                    .ApplyIncludes(includes);

                return query.ToList();
            }
        }

        public List<QCompetencyRequirement> GetCompetencyRequirements(Expression<Func<QCompetencyRequirement, bool>> filter, params Expression<Func<QCompetencyRequirement, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.QCompetencyRequirements
                    .AsNoTracking()
                    .Where(filter)
                    .ApplyIncludes(includes);

                return query.ToList();
            }
        }

        public int CountCompetencyRequirements(Expression<Func<QCompetencyRequirement, bool>> filter)
        {
            using (var db = CreateContext())
                return db.QCompetencyRequirements.AsNoTracking().Where(filter).Count();
        }

        public bool ExistsJournalSetupUser(Guid journalSetupIdentifier, Guid userIdentifier, JournalSetupUserRole role)
        {
            var roleText = role.ToString();

            using (var db = CreateContext())
            {
                return db.QJournalSetupUsers
                    .AsNoTracking()
                    .Where(x =>
                        x.JournalSetupIdentifier == journalSetupIdentifier
                        && x.UserIdentifier == userIdentifier
                        && x.UserRole == roleText
                    )
                    .Any();
            }
        }

        public QJournalSetupUser GetJournalSetupUser(
            Guid journalSetupIdentifier,
            Guid userIdentifier,
            JournalSetupUserRole role,
            params Expression<Func<QJournalSetupUser, object>>[] includes
            )
        {
            var roleText = role.ToString();

            using (var db = CreateContext())
            {
                var query = db.QJournalSetupUsers
                    .AsNoTracking()
                    .Where(x =>
                        x.JournalSetupIdentifier == journalSetupIdentifier
                        && x.UserIdentifier == userIdentifier
                        && x.UserRole == roleText
                    )
                    .ApplyIncludes(includes);

                return query.FirstOrDefault();
            }
        }

        public int CountJournalSetupUsers(VJournalSetupUserFilter filter)
        {
            using (var db = CreateContext())
                return CreateJournalSetupUserQuery(filter, db).Count();
        }

        public List<VJournalSetupUser> GetJournalSetupUsers(VJournalSetupUserFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateJournalSetupUserQuery(filter, db);

                if (string.Equals(filter.OrderBy, "JournalCreated"))
                {
                    query = query
                        .OrderByDescending(x => x.JournalSetupCreated)
                        .ThenBy(x => x.UserFullName);
                }
                else
                    query = query.OrderBy(x => x.UserFullName);

                return query.ApplyPaging(filter).ToList();
            }
        }

        public List<JournalSetupUserExtended> GetJournalSetupUsersExtended(VJournalSetupUserFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateJournalSetupUserQuery(filter, db)
                    .Select(x => new JournalSetupUserExtended
                    {
                        JournalSetupIdentifier = x.JournalSetupIdentifier,
                        UserIdentifier = x.UserIdentifier,
                        OrganizationIdentifier = x.OrganizationIdentifier,
                        JournalIdentifier = x.JournalIdentifier,
                        EmployerGroupIdentifier = x.EmployerGroupIdentifier,
                        UserRole = x.UserRole,
                        JournalSetupName = x.JournalSetupName,
                        JournalSetupCreated = x.JournalSetupCreated,
                        UserFullName = x.UserFullName,
                        UserEmail = x.UserEmail,
                        UserEmailAlternate = x.UserEmailAlternate,
                        EmployerGroupName = x.EmployerGroupName,
                        PersonCode = x.PersonCode,
                        ExperienceCount = db.QExperiences.Count(y => y.JournalIdentifier == x.JournalIdentifier),
                        ValidatedExperienceCount = db.QExperiences.Count(y => y.JournalIdentifier == x.JournalIdentifier && y.ExperienceValidated.HasValue),
                        HasAchievement = db.QCredentials.Any(y => y.UserIdentifier == x.UserIdentifier && y.AchievementIdentifier == x.AchievementIdentifier)
                    });

                if (string.Equals(filter.OrderBy, "JournalCreated"))
                {
                    query = query
                        .OrderByDescending(x => x.JournalSetupCreated)
                        .ThenBy(x => x.UserFullName);
                }
                else
                    query = query.OrderBy(x => x.UserFullName);

                return query.ApplyPaging(filter).ToList();
            }
        }

        private static IQueryable<VJournalSetupUser> CreateJournalSetupUserQuery(VJournalSetupUserFilter filter, InternalDbContext db)
        {
            var query = db.VJournalSetupUsers.AsNoTracking().AsQueryable();

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.JournalSetupIdentifier.HasValue)
                query = query.Where(x => x.JournalSetupIdentifier == filter.JournalSetupIdentifier);

            if (filter.UserIdentifier.HasValue)
                query = query.Where(x => x.UserIdentifier == filter.UserIdentifier);

            if (filter.Role.HasValue)
            {
                var role = filter.Role.ToString();
                query = query.Where(x => x.UserRole == role);
            }

            if (filter.ValidatorUserIdentifier.HasValue)
            {
                var validatorRole = JournalSetupUserRole.Validator.ToString();
                query = query.Where(x =>
                    db.QJournalSetupUsers.Any(y =>
                        y.JournalSetupIdentifier == x.JournalSetupIdentifier
                        && y.UserIdentifier == filter.ValidatorUserIdentifier
                        && y.UserRole == validatorRole
                    )
                );
            }

            if (filter.ExcludeAchievementIdentifier.HasValue)
            {
                query = query.Where(x => !db.QCredentials.Any(y =>
                    y.UserIdentifier == x.UserIdentifier
                    && y.AchievementIdentifier == filter.ExcludeAchievementIdentifier
                ));
            }

            if (!string.IsNullOrEmpty(filter.UserKeyword))
                query = query.Where(x => x.UserFullName.Contains(filter.UserKeyword) || x.UserEmail.Contains(filter.UserKeyword));

            return query;
        }

        public bool ExistsJournalSetupGroup(Guid journalSetupId, Guid groupId)
        {
            using (var db = CreateContext())
            {
                return db.QJournalSetupGroups
                    .AsNoTracking()
                    .Where(x =>
                        x.JournalSetupIdentifier == journalSetupId
                        && x.GroupIdentifier == groupId
                    )
                    .Any();
            }
        }

        public int CountJournalSetupGroups(QJournalSetupGroupFilter filter)
        {
            using (var db = CreateContext())
                return CreateJournalSetupGroupQuery(filter, db).Count();
        }

        public List<JournalSetupGroupDetail> GetJournalSetupGroupDetails(QJournalSetupGroupFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateJournalSetupGroupQuery(filter, db);

                query = !string.IsNullOrEmpty(filter.OrderBy)
                    ? query.OrderBy(filter.OrderBy)
                    : query.OrderBy(x => x.Group.GroupName);

                return query
                    .ApplyPaging(filter)
                    .Select(x => new JournalSetupGroupDetail
                    {
                        JournalSetupIdentifier = x.JournalSetupIdentifier,
                        GroupIdentifier = x.GroupIdentifier,
                        Created = x.Created,
                        GroupName = x.Group.GroupName,
                        MembershipCount = x.Group.VMemberships.Count
                    })
                    .ToList();
            }
        }

        private static IQueryable<QJournalSetupGroup> CreateJournalSetupGroupQuery(QJournalSetupGroupFilter filter, InternalDbContext db)
        {
            var query = db.QJournalSetupGroups
                .AsNoTracking()
                .Where(x => x.JournalSetupIdentifier == filter.JournalSetupIdentifier);

            if (!string.IsNullOrEmpty(filter.GroupName))
                query = query.Where(x => x.Group.GroupName.Contains(filter.GroupName));

            return query;
        }

        public QAreaRequirement GetAreaRequirement(Guid journalSetupId, Guid standardId, params Expression<Func<QAreaRequirement, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                return db.QAreaRequirements
                    .AsNoTracking()
                    .Where(x => x.JournalSetupIdentifier == journalSetupId && x.AreaStandardIdentifier == standardId)
                    .ApplyIncludes(includes)
                    .FirstOrDefault();
            }
        }

        public List<QAreaRequirement> GetAreaRequirements(Guid journalSetupId, params Expression<Func<QAreaRequirement, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                return db.QAreaRequirements
                    .AsNoTracking()
                    .Where(x => x.JournalSetupIdentifier == journalSetupId)
                    .ApplyIncludes(includes)
                    .ToList();
            }
        }

        private InternalDbContext CreateContext()
            => new InternalDbContext(false);
    }
}

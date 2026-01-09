using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Records.Read;
using InSite.Domain.Records;
using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Persistence
{
    public class AchievementSearch : IAchievementSearch
    {
        internal InternalDbContext CreateContext() => new InternalDbContext(false);

        #region Achievements

        public List<ContactSummaryWithExpiryReportItem> GetContactSummaryWithExpiryReport(Guid userIdentifier, Guid organizationIdentifier)
        {
            var validStatus = CredentialStatus.Valid.ToString();

            using (var db = CreateContext())
            {
                return db.QCredentials
                    .Where(x =>
                        x.UserIdentifier == userIdentifier
                        && x.Achievement.OrganizationIdentifier == organizationIdentifier
                        && x.CredentialStatus == validStatus
                        && x.CredentialExpirationExpected != null
                    )
                    .Join(db.QProgresses.Where(x =>
                                x.UserIdentifier == userIdentifier
                                && x.GradeItem.ParentGradeItemIdentifier == null
                                && x.GradeItem.GradeItemIsReported
                            ),
                        a => a.AchievementIdentifier,
                        b => b.GradeItem.AchievementIdentifier,
                        (a, b) => new ContactSummaryWithExpiryReportItem
                        {
                            AchievementTitle = a.Achievement.AchievementTitle,
                            ScoreType = b.GradeItem.GradeItemType,
                            ScoreSequence = b.GradeItem.GradeItemSequence,
                            Granted = a.CredentialGranted.Value,
                            ExpirationExpected = a.CredentialExpirationExpected.Value,
                            ScorePercent = b.ProgressPercent,
                            ScoreText = b.ProgressText,
                            ScoreNumber = b.ProgressNumber,
                            ScorePoint = b.ProgressPoints,
                            EventScheduledStart = b.GradeItem.Gradebook.Event.EventScheduledStart
                        }
                    )
                    .ToList();
            }
        }

        public List<ContactSummaryNoExpiryReportItem> GetContactSummaryNoExpiryReport(Guid userIdentifier, Guid organizationIdentifier)
        {
            var validStatus = CredentialStatus.Valid.ToString();

            using (var db = CreateContext())
            {
                return db.QCredentials
                    .Where(x => x.UserIdentifier == userIdentifier && x.CredentialStatus == validStatus && x.CredentialExpirationExpected == null)
                    .Join(db.Registrations.Where(x => x.CandidateIdentifier == userIdentifier && x.Event.OrganizationIdentifier == organizationIdentifier),
                        a => a.AchievementIdentifier,
                        b => b.Event.AchievementIdentifier,
                        (a, b) => b
                    )
                    .Join(db.QGradebookEvents,
                        a => a.EventIdentifier,
                        b => b.EventIdentifier,
                        (a, b) => new { a.Event, b.GradebookIdentifier }
                    )
                    .Join(db.QProgresses.Where(x =>
                                x.UserIdentifier == userIdentifier
                                && x.GradeItem.ParentGradeItemIdentifier == null
                                && x.GradeItem.GradeItemIsReported
                            ),
                        a => a.GradebookIdentifier,
                        b => b.GradebookIdentifier,
                        (a, b) => new ContactSummaryNoExpiryReportItem
                        {
                            AchievementDescription = a.Event.Achievement.AchievementDescription,
                            AchievementTitle = a.Event.Achievement.AchievementTitle,
                            EventTitle = a.Event.EventTitle,
                            EventScheduledStart = a.Event.EventScheduledStart,
                            EventScheduledEnd = a.Event.EventScheduledEnd,
                            ScoreName = b.GradeItem.GradeItemName,
                            ScoreType = b.GradeItem.GradeItemType,
                            ScoreSequence = b.GradeItem.GradeItemSequence,
                            ScoreComment = b.ProgressComment,
                            ScorePercent = b.ProgressPercent,
                            ScoreText = b.ProgressText,
                            ScoreNumber = b.ProgressNumber,
                            ScorePoint = b.ProgressPoints
                        }
                    )
                    .ToList();
            }
        }

        public List<Tuple<QAchievement, int>> GetStudentAchievements(Guid gradebook, Guid organization)
        {
            using (var db = CreateContext())
            {
                return db.QEnrollments.Where(x => x.GradebookIdentifier == gradebook)
                    .Join(db.QCredentials.Where(x => x.Achievement.OrganizationIdentifier == organization),
                        a => a.LearnerIdentifier,
                        b => b.UserIdentifier,
                        (a, b) => new { b.AchievementIdentifier }
                    )
                    .Join(db.QAchievements,
                        a => a.AchievementIdentifier,
                        b => b.AchievementIdentifier,
                        (a, b) => b
                    )
                    .GroupBy(x => x)
                    .Select(x => new { Achievement = x.Key, Count = x.Count() })
                    .OrderBy(x => x.Achievement.AchievementTitle)
                    .ToList()
                    .Select(x => new Tuple<QAchievement, int>(x.Achievement, x.Count))
                    .ToList();
            }
        }

        public List<Tuple<QAchievement, int>> GetItemAndStudentAchievements(Guid gradebook, Guid organization)
        {
            using (var db = CreateContext())
            {
                var achievement = db.QGradebooks.FirstOrDefault(x => x.GradebookIdentifier == gradebook)?.AchievementIdentifier;

                return db.QEnrollments.Where(x => x.GradebookIdentifier == gradebook)
                    .Join(db.QCredentials.Where(x =>
                            x.Achievement.OrganizationIdentifier == organization
                            && (
                                x.AchievementIdentifier == achievement
                                || db.QGradeItems.Any(y => y.AchievementIdentifier == x.AchievementIdentifier && y.GradebookIdentifier == gradebook)
                            )
                        ),
                        a => a.LearnerIdentifier,
                        b => b.UserIdentifier,
                        (a, b) => new { b.AchievementIdentifier }
                    )
                    .Join(db.QAchievements,
                        a => a.AchievementIdentifier,
                        b => b.AchievementIdentifier,
                        (a, b) => b
                    )
                    .GroupBy(x => x)
                    .Select(x => new { Achievement = x.Key, Count = x.Count() })
                    .OrderBy(x => x.Achievement.AchievementTitle)
                    .ToList()
                    .Select(x => new Tuple<QAchievement, int>(x.Achievement, x.Count))
                    .ToList();
            }
        }

        public List<Tuple<QAchievement, int>> GetProgramAndStudentAchievements(Guid program, Guid organization)
        {
            using (var db = CreateContext())
            {
                var achievement = db.TPrograms.FirstOrDefault(x => x.ProgramIdentifier == program)?.AchievementIdentifier;

                return db.TProgramEnrollments.Where(x => x.ProgramIdentifier == program)
                    .Join(db.QCredentials.Where(x =>
                            x.Achievement.OrganizationIdentifier == organization
                            && (
                                x.AchievementIdentifier == achievement
                            )
                        ),
                        a => a.LearnerUserIdentifier,
                        b => b.UserIdentifier,
                        (a, b) => new { b.AchievementIdentifier }
                    )
                    .Join(db.QAchievements,
                        a => a.AchievementIdentifier,
                        b => b.AchievementIdentifier,
                        (a, b) => b
                    )
                    .GroupBy(x => x)
                    .Select(x => new { Achievement = x.Key, Count = x.Count() })
                    .OrderBy(x => x.Achievement.AchievementTitle)
                    .ToList()
                    .Select(x => new Tuple<QAchievement, int>(x.Achievement, x.Count))
                    .ToList();
            }
        }

        public List<Tuple<QAchievement, int>> GetProgramStudentAchievements(Guid program, Guid organization)
        {
            using (var db = CreateContext())
            {
                return db.TProgramEnrollments.Where(x => x.ProgramIdentifier == program)
                    .Join(db.QCredentials.Where(x => x.Achievement.OrganizationIdentifier == organization),
                        a => a.LearnerUserIdentifier,
                        b => b.UserIdentifier,
                        (a, b) => new { b.AchievementIdentifier }
                    )
                    .Join(db.QAchievements,
                        a => a.AchievementIdentifier,
                        b => b.AchievementIdentifier,
                        (a, b) => b
                    )
                    .GroupBy(x => x)
                    .Select(x => new { Achievement = x.Key, Count = x.Count() })
                    .OrderBy(x => x.Achievement.AchievementTitle)
                    .ToList()
                    .Select(x => new Tuple<QAchievement, int>(x.Achievement, x.Count))
                    .ToList();
            }
        }

        public QAchievement GetAchievement(Guid organization, string title)
        {
            using (var db = CreateContext())
                return db.QAchievements.FirstOrDefault(x => x.OrganizationIdentifier == organization && x.AchievementTitle == title);
        }

        public QAchievement GetAchievement(Guid achievementIdentifier, params Expression<Func<QAchievement, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                return db.QAchievements
                    .ApplyIncludes(includes)
                    .Where(x => x.AchievementIdentifier == achievementIdentifier)
                    .FirstOrDefault();
            }
        }

        public QAchievement[] GetAchievements(IEnumerable<Guid> achievementIdentifiers)
        {
            using (var db = CreateContext())
                return db.QAchievements.Where(x => achievementIdentifiers.Contains(x.AchievementIdentifier)).ToArray();
        }

        public int CountAchievements(QAchievementFilter filter)
        {
            using (var db = CreateContext())
                return CreateQuery(filter, db).Count();
        }

        public List<VAchievement> GetAchievements(QAchievementFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .OrderBy(x => x.AchievementTitle)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        public string[] GetAchievementLabels(Guid organization)
        {
            using (var db = CreateContext())
            {
                return db.QAchievements
                    .Where(x => x.OrganizationIdentifier == organization)
                    .Select(x => x.AchievementLabel)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToArray();
            }
        }

        private static IQueryable<VAchievement> CreateQuery(QAchievementFilter filter, InternalDbContext db)
        {
            var query = db.VAchievements
                .AsNoTracking()
                .AsQueryable();

            if (filter.OrganizationIdentifiers != null && filter.OrganizationIdentifiers.Any())
                query = query.Where(x => filter.OrganizationIdentifiers.Contains(x.OrganizationIdentifier));

            if (filter.AchievementTitle.HasValue())
                query = query.Where(x => x.AchievementTitle.Contains(filter.AchievementTitle));

            if (filter.AchievementLabels != null && filter.AchievementLabels.Any())
                query = query.Where(x => filter.AchievementLabels.Contains(x.AchievementLabel));

            if (filter.AchievementDescription.HasValue())
                query = query.Where(x => x.AchievementDescription.Contains(filter.AchievementDescription));

            if (filter.AchievementIsEnabled.HasValue)
                query = query.Where(x => x.AchievementIsEnabled == filter.AchievementIsEnabled);

            if (filter.ExpirationType.HasValue())
                query = query.Where(x => x.ExpirationType == filter.ExpirationType);

            if (filter.ExpirationLifetimeQuantity.HasValue)
                query = query.Where(x => x.ExpirationLifetimeQuantity == filter.ExpirationLifetimeQuantity);

            if (filter.ExpirationLifetimeUnit.HasValue())
                query = query.Where(x => x.ExpirationLifetimeUnit.Contains(filter.ExpirationLifetimeUnit));

            if (filter.ExpirationFixedDateSince.HasValue)
                query = query.Where(x => x.ExpirationFixedDate >= filter.ExpirationFixedDateSince.Value);

            if (filter.ExpirationFixedDateBefore.HasValue)
                query = query.Where(x => x.ExpirationFixedDate < filter.ExpirationFixedDateBefore.Value);

            return query;
        }

        #endregion

        #region Credentials

        public int CountCredentials(VCredentialFilter filter)
        {
            using (var db = CreateContext())
                return CreateQuery(filter, db).Count();
        }

        public int CountGradebookEnrollmentsForCredentials(Guid? learner, Guid achievement, bool pending, bool valid, bool expired)
        {
            return SelectGradebookEnrollmentsForCredentials(learner, achievement, pending, valid, expired).Length;
        }

        public GradebookEnrollmentsForCredentialItem[] SelectGradebookEnrollmentsForCredentials(Guid? learner, Guid achievement, bool pending, bool valid, bool expired)
        {
            const string query = "exec record.SelectGradebookEnrollmentsForCredentials @LearnerId, @AchievementId, @CredentialStatuses";

            var statuses = new List<string>();

            if (pending)
                statuses.Add(CredentialStatus.Pending.ToString());

            if (valid)
                statuses.Add(CredentialStatus.Valid.ToString());

            if (expired)
                statuses.Add(CredentialStatus.Expired.ToString());

            using (var db = CreateContext())
            {
                var parameters = new SqlParameter[] {
                    new SqlParameter("@LearnerId", learner ?? (object) DBNull.Value),
                    new SqlParameter("@AchievementId", achievement),
                    new SqlParameter("@CredentialStatuses", StringHelper.Join(statuses, ","))
                };
                return db.Database.SqlQuery<GradebookEnrollmentsForCredentialItem>(query, parameters).ToArray();
            }
        }

        public VCredential GetCredential(Guid achievement, Guid user)
        {
            using (var db = CreateContext())
                return db.VCredentials.FirstOrDefault(x => x.AchievementIdentifier == achievement
                    && x.UserIdentifier == user);
        }

        public List<VCredential> GetCredentials(IEnumerable<Guid> achievements, Guid user)
        {
            using (var db = CreateContext())
                return db.VCredentials
                    .Where(x => achievements.Contains(x.AchievementIdentifier)
                             && x.UserIdentifier == user)
                    .ToList();
        }

        public VCredential GetCredential(Guid credential)
        {
            using (var db = CreateContext())
                return db.VCredentials.FirstOrDefault(x => x.CredentialIdentifier == credential);
        }

        public List<VCredential> GetCredentials(VCredentialFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db);

                if (filter.OrderBy.IsNotEmpty())
                    query = query.OrderBy(filter.OrderBy);
                else
                    query = query
                        .OrderBy(x => x.UserFullName)
                        .ThenBy(x => x.AchievementTitle);

                return query.ApplyPaging(filter).ToList();
            }
        }

        public List<VCredential> GetRecentCredentials(VCredentialFilter filter, int count)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db)
                    .OrderByDescending(x => x.CredentialGranted)
                    .ThenBy(x => x.AchievementTitle)
                    .ThenBy(x => x.UserFullName)
                    .AsQueryable();

                return query.Take(count).ToList();
            }
        }

        private static IQueryable<VCredential> CreateQuery(VCredentialFilter filter, InternalDbContext db)
        {
            var now = DateTimeOffset.UtcNow;
            var query = db.VCredentials.AsQueryable();

            // Guid

            if (filter.AchievementIdentifier.HasValue)
                query = query.Where(x => x.AchievementIdentifier == filter.AchievementIdentifier.Value);

            if (filter.AchievementIdentifiers.IsNotEmpty())
                query = query.Where(x => filter.AchievementIdentifiers.Any(a => a == x.AchievementIdentifier));

            if (filter.CredentialIdentifiers.IsNotEmpty())
                query = query.Where(x => filter.CredentialIdentifiers.Any(a => a == x.CredentialIdentifier));

            if (filter.DepartmentIdentifier.HasValue)
            {
                var learners = db.Memberships
                    .Where(x => x.GroupIdentifier == filter.DepartmentIdentifier && x.MembershipType == "Department")
                    .Select(x => x.UserIdentifier)
                    .ToArray();

                var achievements = db.TAchievementDepartments
                    .Where(x => x.DepartmentIdentifier == filter.DepartmentIdentifier.Value)
                    .Select(x => x.AchievementIdentifier)
                    .ToArray();

                query = query.Where(credential =>
                    learners.Any(learner => learner == credential.UserIdentifier)
                    && achievements.Any(achievement => achievement == credential.AchievementIdentifier)
                );
            }

            if (filter.UserGradebookIdentifier.HasValue)
                query = query.Where(x => db.QEnrollments.Any(y => y.LearnerIdentifier == x.UserIdentifier && y.GradebookIdentifier == filter.UserGradebookIdentifier.Value));

            if (filter.ItemGradebookIdentifier.HasValue)
            {
                var achievement = db.QGradebooks
                    .Where(x => x.GradebookIdentifier == filter.ItemGradebookIdentifier.Value)
                    .Select(x => x.AchievementIdentifier)
                    .FirstOrDefault();

                query = query.Where(x =>
                    x.AchievementIdentifier == achievement
                    || db.QGradeItems.Any(y => y.AchievementIdentifier == x.AchievementIdentifier && y.GradebookIdentifier == filter.ItemGradebookIdentifier.Value)
                );
            }

            if (filter.ProgramIdentifier.HasValue)
            {
                var achievement = db.TPrograms
                    .Where(x => x.ProgramIdentifier == filter.ProgramIdentifier.Value)
                    .Select(x => x.AchievementIdentifier)
                    .FirstOrDefault();

                query = query.Where(x =>
                    x.AchievementIdentifier == achievement
                );
            }

            if (filter.JournalSetupIdentifier.HasValue)
            {
                var achievement = db.QJournalSetups
                    .Where(x => x.JournalSetupIdentifier == filter.JournalSetupIdentifier.Value)
                    .Select(x => x.AchievementIdentifier)
                    .FirstOrDefault();

                query = query
                    .Where(x => x.AchievementIdentifier == achievement)
                    .Join(db.QJournalSetupUsers.Where(x => x.JournalSetupIdentifier == filter.JournalSetupIdentifier.Value && x.UserRole == "Learner"),
                        a => a.UserIdentifier,
                        b => b.UserIdentifier,
                        (a, b) => a
                    );
            }

            if (filter.OrganizationIdentifier.HasValue)
            {
                var orgIds = new List<Guid> { filter.OrganizationIdentifier.Value };

                var parentOrgId = db.QOrganizations
                    .Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier.Value)
                    .Select(x => x.ParentOrganizationIdentifier)
                    .Single();
                if (parentOrgId.HasValue)
                    orgIds.Add(parentOrgId.Value);

                query = query.Where(x => orgIds.Contains(x.OrganizationIdentifier));
            }

            if (filter.UserIdentifier.HasValue)
                query = query.Where(x => x.UserIdentifier == filter.UserIdentifier.Value);

            if (filter.ExcludeAchievements.IsNotEmpty())
                query = query.Where(x => !filter.ExcludeAchievements.Contains(x.AchievementIdentifier));

            // Boolean

            if (filter.IsPendingExpiry.HasValue && filter.IsPendingExpiry.Value)
                query = query.Where(x =>
                       x.CredentialExpirationExpected.HasValue
                    && x.CredentialExpirationExpected.Value < now
                    && (!x.CredentialExpired.HasValue
                      || x.CredentialExpired.Value != x.CredentialExpirationExpected.Value
                      || (x.CredentialExpired.Value == x.CredentialExpirationExpected.Value && x.CredentialStatus == "Valid")
                      )
                    );

            if (filter.IsPendingReminderRequest.HasValue && filter.IsPendingReminderRequest.Value)
            {
                if (filter.CredentialStatus == CredentialStatus.Expired.ToString())
                    query = query.Where(x => x.CredentialExpired.HasValue
                    && !x.CredentialExpirationReminderRequested0.HasValue);

                else if (filter.CredentialStatus == CredentialStatus.Valid.ToString())
                    query = query.Where(x => !x.CredentialExpired.HasValue
                     && (!x.CredentialExpirationReminderRequested1.HasValue
                      || !x.CredentialExpirationReminderRequested2.HasValue
                      || !x.CredentialExpirationReminderRequested3.HasValue));
            }

            if (filter.IsGranted.HasValue)
            {
                if (filter.IsGranted.HasValue)
                    query = query.Where(x => x.CredentialGranted != null);
                else
                    query = query.Where(x => x.CredentialGranted == null);
            }

            if (filter.AchievementHasCertificate.HasValue)
            {
                if (filter.AchievementHasCertificate.Value)
                    query = query.Where(x => x.AchievementCertificateLayoutCode != null);
                else
                    query = query.Where(x => x.AchievementCertificateLayoutCode == null);
            }

            // Int32

            if (filter.EmployerGroupIdentifier.HasValue)
                query = query.Where(x => x.EmployerGroupIdentifier == filter.EmployerGroupIdentifier.Value);

            // String

            if (filter.AchievementTitle.HasValue())
                query = query.Where(x => x.AchievementTitle.Contains(filter.AchievementTitle));

            if (filter.CredentialStatus.HasValue())
                query = query.Where(x => x.CredentialStatus == filter.CredentialStatus);

            if (filter.EmployerGroupName.HasValue())
                query = query.Where(x => x.EmployerGroupName.Contains(filter.EmployerGroupName));

            if (filter.UserEmail.HasValue())
                query = query.Where(x => x.UserEmail.Contains(filter.UserEmail));

            if (filter.PersonCode.HasValue())
                query = query.Where(x => x.PersonCode.Contains(filter.PersonCode));

            if (filter.UserFullName.HasValue())
                query = query.Where(x => x.UserFullName.Contains(filter.UserFullName));

            if (filter.UserRegion.HasValue())
                query = query.Where(x => x.UserRegion == filter.UserRegion);

            if (filter.AchievementLabel.HasValue())
                query = query.Where(x => x.AchievementLabel == filter.AchievementLabel);

            if (filter.AchievementLabels != null && filter.AchievementLabels.Length > 0)
                query = query.Where(x => filter.AchievementLabels.Any(label => label == x.AchievementLabel));

            if (filter.AchievementTitle.HasValue())
                query = query.Where(x => x.AchievementTitle.Contains(filter.AchievementTitle));

            if (filter.EmployerGroupStatus.HasValue())
                query = query.Where(x => x.EmployerGroupStatus == filter.EmployerGroupStatus);

            // DateTimeOffset

            if (filter.CredentialExpiredSince.HasValue)
                query = query.Where(x => x.CredentialExpirationExpected >= filter.CredentialExpiredSince.Value);

            if (filter.CredentialExpiredBefore.HasValue)
                query = query.Where(x => x.CredentialExpirationExpected < filter.CredentialExpiredBefore.Value);

            if (filter.CredentialGrantedSince.HasValue)
                query = query.Where(x => x.CredentialGranted >= filter.CredentialGrantedSince.Value);

            if (filter.CredentialGrantedBefore.HasValue)
                query = query.Where(x => x.CredentialGranted < filter.CredentialGrantedBefore.Value);

            if (filter.CredentialRevokedSince.HasValue)
                query = query.Where(x => x.CredentialRevoked >= filter.CredentialRevokedSince.Value);

            if (filter.CredentialRevokedBefore.HasValue)
                query = query.Where(x => x.CredentialRevoked < filter.CredentialRevokedBefore.Value);

            if (filter.CredentialExpirationExpectedSince.HasValue)
                query = query.Where(x => x.CredentialExpirationExpected >= filter.CredentialExpirationExpectedSince.Value);

            if (filter.CredentialExpirationExpectedBefore.HasValue)
                query = query.Where(x => x.CredentialExpirationExpected < filter.CredentialExpirationExpectedBefore.Value);

            return query;
        }

        public Guid GetCredentialIdentifier(Guid? credential, Guid achievement, Guid user)
        {
            using (var db = new InternalDbContext())
            {
                var tombstone = db.QCredentialHistories
                    .Where(x => x.AchievementIdentifier == achievement && x.UserIdentifier == user)
                    .OrderByDescending(x => x.ChangeTime)
                    .FirstOrDefault();

                return GetCredentialIdentifier(credential, tombstone);
            }
        }

        public Guid GetCredentialIdentifier(Guid? credential, QCredentialHistory tombstone)
        {
            if (tombstone != null)
                return tombstone.AggregateIdentifier;

            if (credential.HasValue)
                return credential.Value;

            return UniqueIdentifier.Create();
        }

        public QCredentialHistory[] GetCredentialHistory(IEnumerable<Guid> users, Func<Guid, IEnumerable<Guid>> getAchievements)
        {
            using (var db = new InternalDbContext())
            {
                var count = 0;
                var result = new List<QCredentialHistory>();
                var predicate = PredicateBuilder.False<QCredentialHistory>();

                foreach (var user in users)
                {
                    if (count >= 10)
                        ExecQuery();

                    var achievements = getAchievements(user);

                    predicate = predicate.Or(x => x.UserIdentifier == user && achievements.Contains(x.AchievementIdentifier));

                    count++;
                }

                ExecQuery();

                return result.ToArray();

                void ExecQuery()
                {
                    if (count > 0)
                        result.AddRange(db.QCredentialHistories.AsExpandable().Where(predicate));

                    predicate = PredicateBuilder.False<QCredentialHistory>();
                    count = 0;
                }
            }
        }

        public ExpiredCredentialsSearchResult[] GetExpiredCredentials(ExpiredCredentialsSearchCriteria criteria, Guid organizationId)
        {
            using (var db = new InternalDbContext())
            {
                db.Configuration.ProxyCreationEnabled = false;
                db.Configuration.LazyLoadingEnabled = false;

                var query = db.VCmdsCredentials.AsQueryable().AsNoTracking()
                    .Where(x => !x.User.UtcArchived.HasValue
                             && SqlFunctions.DateDiff("DAY", x.CredentialExpirationExpected, DateTimeOffset.UtcNow) > 10
                             && x.User.Persons.Any(y => y.OrganizationIdentifier == organizationId && y.UserAccessGranted.HasValue)
                    );

                if (!string.IsNullOrEmpty(criteria.UserName))
                    query = query.Where(x => x.User.FullName.Contains(criteria.UserName));

                if (!string.IsNullOrEmpty(criteria.UserEmail))
                    query = query.Where(x => x.User.Email.Contains(criteria.UserEmail));

                if (!string.IsNullOrEmpty(criteria.AssetSubtype))
                    query = query.Where(x => x.AchievementLabel == criteria.AssetSubtype);

                if (!string.IsNullOrEmpty(criteria.AssetTitle))
                    query = query.Where(x => x.AchievementTitle.Contains(criteria.AssetTitle));

                if (criteria.ExpiredSince.HasValue)
                    query = query.Where(x => x.CredentialExpirationExpected >= criteria.ExpiredSince.Value);

                if (criteria.ExpiredBefore.HasValue)
                    query = query.Where(x => x.CredentialExpirationExpected < criteria.ExpiredBefore.Value);

                return query
                    .Select(x => new ExpiredCredentialsSearchResult
                    {
                        ContactFirstName = x.User.FirstName,
                        ContactLastName = x.User.LastName,
                        ContactEmail = x.User.Email,
                        AssetType = "Resource",
                        AssetSubtype = x.AchievementLabel,
                        AssetCode = "-",
                        AssetTitle = x.AchievementTitle,
                        Expired = x.CredentialExpirationExpected.Value,
                        DaysSinceExpiration = SqlFunctions.DateDiff("DAY", x.CredentialExpirationExpected, DateTimeOffset.UtcNow).Value
                    })
                    .OrderBy(x => x.ContactFirstName)
                    .ThenBy(x => x.ContactLastName)
                    .ThenBy(x => x.AssetType)
                    .ThenBy(x => x.AssetSubtype)
                    .ThenBy(x => x.AssetTitle)
                    .ToArray();
            }
        }

        #region Program Credentials

        class QueryResult
        {
            public Guid LearnerUserIdentifier { get; set; }
            public Guid CredentialIdentifier { get; set; }
        }

        public Guid[] GetLearnerProgramCredentials(Guid program)
        {
            const string query = "exec records.GetProgramLearnerAchievements @ProgramIdentifier";

            using (var db = CreateContext())
            {
                var parameters = new SqlParameter[] {
                    new SqlParameter("@ProgramIdentifier", program)
                };
                return db.Database.SqlQuery<QueryResult>(query, parameters).Select(x => x.CredentialIdentifier).ToArray();

            }
        }

        public Guid[] GetLearnerTaskAndProgramCredentials(Guid program)
        {
            const string query = "exec records.GetProgramAndTaskLearnersCredentials @ProgramIdentifier";

            using (var db = new InternalDbContext(false))
            {
                return db.Database
                    .SqlQuery<QueryResult>(query, new SqlParameter("ProgramIdentifier", program)).Select(x => x.CredentialIdentifier).ToArray();
            }
        }

        #endregion

        #endregion

        #region Maintenance Helpers

        public CertificateWithMissingExpiry[] GetCertificatesWithMissingExpiry(Guid achievement)
        {
            var query = @"
SELECT
    U.UserIdentifier
  , A.AchievementIdentifier
  , C.CredentialIdentifier
  , A.ExpirationLifetimeQuantity AS AchievementExpirationLifetimeQuantity
  , A.ExpirationLifetimeUnit     AS AchievementExpirationLifetimeUnit
  , A.AchievementTitle
  , U.Email                      AS UserEmail
FROM
    achievements.QAchievement       AS A
INNER JOIN achievements.QCredential AS C
           ON C.AchievementIdentifier = A.AchievementIdentifier

INNER JOIN accounts.QOrganization   AS T
           ON T.OrganizationIdentifier = A.OrganizationIdentifier
INNER JOIN identities.[User]        AS U
           ON U.UserIdentifier = C.UserIdentifier
WHERE
    A.ExpirationLifetimeQuantity IS NOT NULL
    AND C.ExpirationLifetimeQuantity IS NULL
    AND C.CredentialStatus = 'Valid'
    AND A.AchievementIdentifier = @AchievementIdentifier
ORDER BY
    A.AchievementTitle
  , UserEmail
";
            using (var db = CreateContext())
            {
                var parameters = new SqlParameter[] {
                    new SqlParameter("@AchievementIdentifier", achievement)
                };
                return db.Database.SqlQuery<CertificateWithMissingExpiry>(query, parameters).ToArray();
            }
        }

        public QCredential[] GetCredentialsWithUnexpectedExpirationDelivery(ReminderType reminderType)
        {
            using (var db = new InternalDbContext())
            {
                if (reminderType == ReminderType.Today)
                    return db.QCredentials
                        .Where(x => x.CredentialExpirationReminderRequested0.HasValue && x.CredentialExpirationReminderDelivered0.HasValue && x.CredentialExpirationReminderDelivered0 < x.CredentialExpirationReminderRequested0)
                        .ToArray();

                if (reminderType == ReminderType.InOneMonth)
                    return db.QCredentials
                        .Where(x => x.CredentialExpirationReminderRequested1.HasValue && x.CredentialExpirationReminderDelivered1.HasValue && x.CredentialExpirationReminderDelivered1 < x.CredentialExpirationReminderRequested1)
                        .ToArray();

                if (reminderType == ReminderType.InTwoMonths)
                    return db.QCredentials
                        .Where(x => x.CredentialExpirationReminderRequested2.HasValue && x.CredentialExpirationReminderDelivered2.HasValue && x.CredentialExpirationReminderDelivered2 < x.CredentialExpirationReminderRequested2)
                        .ToArray();

                if (reminderType == ReminderType.InThreeMonths)
                    return db.QCredentials
                        .Where(x => x.CredentialExpirationReminderRequested3.HasValue && x.CredentialExpirationReminderDelivered3.HasValue && x.CredentialExpirationReminderDelivered3 < x.CredentialExpirationReminderRequested3)
                        .ToArray();
            }

            return new QCredential[0];
        }

        #endregion
    }
}

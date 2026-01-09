using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Courses.Read;
using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public class CourseDistributionSearch : ICourseDistributionSearch
    {
        internal InternalDbContext CreateContext() => new InternalDbContext(false);

        public List<CourseDistributionGridItem> GetCourseDistributionsByManager(Guid organizationId, Guid managerUserId)
        {
            return GetCourseDistributionGridItems(organizationId, managerUserId, null);
        }

        public List<CourseDistributionGridItem> GetCourseDistributionsByLearner(Guid organizationId, Guid learnerUserId)
        {
            return GetCourseDistributionGridItems(organizationId, null, learnerUserId);
        }

        private List<CourseDistributionGridItem> GetCourseDistributionGridItems(Guid organizationId, Guid? managerUserId, Guid? learnerUserId)
        {
            using (var db = new InternalDbContext())
            {
                var distributionsQuery = db.TCourseDistributions.Where(x => x.Product.OrganizationIdentifier == organizationId);

                if (managerUserId.HasValue)
                    distributionsQuery = distributionsQuery.Where(x => x.ManagerUserIdentifier == managerUserId);

                if (learnerUserId.HasValue)
                    distributionsQuery = distributionsQuery.Where(x => x.CourseEnrollment.LearnerUserIdentifier == learnerUserId);

                return distributionsQuery
                    .GroupJoin(
                        db.QActivities
                            .Where(x => x.ActivityType == "Assessment" && x.AssessmentFormIdentifier != null)
                            .Join(db.QAttempts,
                                activity => activity.AssessmentFormIdentifier,
                                attempt => attempt.FormIdentifier,
                                (activity, attempt) => new
                                {
                                    Attempt = attempt,
                                    CourseIdentifier = (Guid?)activity.Module.Unit.CourseIdentifier
                                }
                            )
                        ,
                        d => new { d.CourseIdentifier, LearnerUserIdentifier = (Guid?)d.CourseEnrollment.LearnerUserIdentifier },
                        a => new { a.CourseIdentifier, LearnerUserIdentifier = (Guid?)a.Attempt.LearnerUserIdentifier },
                        (d, a) => new
                        {
                            Distribution = d,
                            Attempts = a.Select(x => x.Attempt).DefaultIfEmpty()
                        }
                    )
                    .SelectMany(x => x.Attempts.Select(a => new CourseDistributionGridItem
                    {
                        LearnerUserIdentifier = (Guid?)x.Distribution.CourseEnrollment.LearnerUser.UserIdentifier,
                        LearnerUserName = x.Distribution.CourseEnrollment.LearnerUser.FullName,
                        LearnerUserEmail = x.Distribution.CourseEnrollment.LearnerUser.Email,
                        FormIdentifier = (Guid?)a.FormIdentifier,
                        FormTitle = a.Form.FormTitle,
                        AttemptIdentifier = (Guid?)a.AttemptIdentifier,
                        AttemptImported = a.AttemptImported,
                        AttemptStarted = a.AttemptStarted,
                        AttemptSubmitted = a.AttemptSubmitted,
                        AttemptGraded = a.AttemptGraded,
                        AttemptScore = a.AttemptScore,
                        CourseDistributionIdentifier = x.Distribution.CourseDistributionIdentifier,
                        ProductIdentifier = x.Distribution.ProductIdentifier,
                        CourseIdentifier = x.Distribution.CourseIdentifier,
                        EventIdentifier = x.Distribution.EventIdentifier,
                        ManagerUserIdentifier = x.Distribution.ManagerUserIdentifier,
                        Created = x.Distribution.Created,
                        Modified = x.Distribution.Modified,
                        CourseEnrollmentIdentifier = x.Distribution.CourseEnrollmentIdentifier,
                        DistributionAssigned = x.Distribution.DistributionAssigned,
                        DistributionStatus = x.Distribution.DistributionStatus,
                        DistributionRedeemed = x.Distribution.DistributionRedeemed,
                        DistributionExpiry = x.Distribution.DistributionExpiry,
                        DistributionComment = x.Distribution.DistributionComment,
                        ProductName = x.Distribution.Product.ProductName,
                        ProductImageUrl = x.Distribution.Product.ProductImageUrl
                    }))
                    .OrderByDescending(x => x.Created)
                    .ThenBy(x => x.CourseDistributionIdentifier)
                    .ToList();
            }
        }

        public TCourseDistribution GetCourseDistribution(Guid courseDistributionId)
        {
            using (var db = CreateContext())
            {
                return db.TCourseDistributions
                         .FirstOrDefault(x => x.CourseDistributionIdentifier == courseDistributionId);
            }
        }

        public int CountCourseDistributions(TCourseDistributionFilter filter)
        {
            using (var db = CreateContext())
                return CreateCourseDistributionQuery(filter, db).Count();
        }

        public List<TCourseDistribution> GetCourseDistributions(
            TCourseDistributionFilter filter,
            params Expression<Func<TCourseDistribution, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                return CreateCourseDistributionQuery(filter, db, includes)
                    .OrderBy(x => x.DistributionAssigned)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        private static IQueryable<TCourseDistribution> CreateCourseDistributionQuery(
            TCourseDistributionFilter filter,
            InternalDbContext db,
            params Expression<Func<TCourseDistribution, object>>[] includes)
        {
            var query = db.TCourseDistributions
                          .ApplyIncludes(includes)
                          .AsQueryable();

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.Product.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.ProductIdentifier.HasValue)
                query = query.Where(x => x.ProductIdentifier == filter.ProductIdentifier.Value);

            if (filter.CourseIdentifier.HasValue)
                query = query.Where(x => x.CourseIdentifier == filter.CourseIdentifier.Value);

            if (filter.ManagerUserIdentifier.HasValue)
                query = query.Where(x => x.ManagerUserIdentifier == filter.ManagerUserIdentifier.Value);

            if (filter.LearnerUserIdentifier.HasValue)
                query = query.Where(x => x.CourseEnrollment.LearnerUserIdentifier == filter.LearnerUserIdentifier);

            if (filter.CourseEnrollmentIdentifier.HasValue)
                query = query.Where(x => x.CourseEnrollmentIdentifier == filter.CourseEnrollmentIdentifier.Value);

            if (filter.DistributionStatus.HasValue())
                query = query.Where(x => x.DistributionStatus == filter.DistributionStatus);

            return query;
        }
    }
}

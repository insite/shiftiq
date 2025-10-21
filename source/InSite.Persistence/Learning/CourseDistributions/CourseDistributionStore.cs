using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using InSite.Application.Courses.Read;

namespace InSite.Persistence
{
    public class CourseDistributionStore : ICourseDistributionStore
    {
        internal InternalDbContext CreateContext() => new InternalDbContext(true);

        public void InsertCourseDistribution(TCourseDistribution distribution)
        {
            using (var db = CreateContext())
            {
                CourseDistributionPreInsert(distribution);

                db.TCourseDistributions.Add(distribution);
                db.SaveChanges();
            }
        }

        public void InsertCourseDistributions(IEnumerable<TCourseDistribution> list)
        {
            using (var db = CreateContext())
            {
                foreach (var d in list)
                {
                    CourseDistributionPreInsert(d);
                    db.TCourseDistributions.Add(d);
                }
                db.SaveChanges();
            }
        }

        private static void CourseDistributionPreInsert(TCourseDistribution distribution)
        {
            if (distribution.ProductIdentifier == Guid.Empty)
                throw new InvalidOperationException("ProductIdentifier is required.");

            if (!distribution.ManagerUserIdentifier.Equals(Guid.Empty) == false)
                throw new InvalidOperationException("ManagerUserIdentifier is required.");

            if (distribution.CourseDistributionIdentifier == Guid.Empty)
                distribution.CourseDistributionIdentifier = Guid.NewGuid();

            if (distribution.Created == default(DateTimeOffset))
                distribution.Created = DateTimeOffset.UtcNow;

            if (distribution.Modified == default(DateTimeOffset))
                distribution.Modified = distribution.Created;
        }

        public void UpdateCourseDistribution(TCourseDistribution distribution)
        {
            using (var db = CreateContext())
            {
                var entity = db.TCourseDistributions
                    .Single(x => x.CourseDistributionIdentifier == distribution.CourseDistributionIdentifier);

                entity.ProductIdentifier = distribution.ProductIdentifier;
                entity.CourseIdentifier = distribution.CourseIdentifier;
                entity.ManagerUserIdentifier = distribution.ManagerUserIdentifier;
                entity.CourseEnrollmentIdentifier = distribution.CourseEnrollmentIdentifier;

                entity.DistributionAssigned = distribution.DistributionAssigned;
                entity.DistributionStatus = distribution.DistributionStatus;
                entity.DistributionRedeemed = distribution.DistributionRedeemed;
                entity.DistributionExpiry = distribution.DistributionExpiry;
                entity.DistributionComment = distribution.DistributionComment;

                entity.Modified = distribution.Modified == default(DateTimeOffset)
                                                        ? DateTimeOffset.UtcNow
                                                        : distribution.Modified;

                db.SaveChanges();
            }
        }

        public void UpdateCourseDistribution<TCourseDistribution>(
            Guid courseDistributionId,
            params (Expression<Func<TCourseDistribution, object>> Property, object Value)[] updates)
        {
            using (var db = CreateContext())
            {
                var entity = db.TCourseDistributions
                    .SingleOrDefault(x => x.CourseDistributionIdentifier == courseDistributionId);

                if (entity == null)
                    return;

                foreach (var update in updates)
                {
                    MemberExpression memberExpression;

                    if (update.Property.Body is UnaryExpression unaryExpression)
                        memberExpression = unaryExpression.Operand as MemberExpression;
                    else
                        memberExpression = update.Property.Body as MemberExpression;

                    if (memberExpression == null)
                        throw new InvalidOperationException("Invalid property expression.");

                    var property = (PropertyInfo)memberExpression.Member;
                    property.SetValue(entity, update.Value);
                }

                if (!updates.Any(u =>
                    (u.Property.Body as MemberExpression)?.Member?.Name == nameof(InSite.Application.Courses.Read.TCourseDistribution.Modified)))
                {
                    entity.Modified = DateTimeOffset.UtcNow;
                }

                db.SaveChanges();
            }
        }

        public void DeleteCourseDistribution(Guid courseDistributionId)
        {
            using (var db = CreateContext())
            {
                var entities = db.TCourseDistributions
                    .Where(x => x.CourseDistributionIdentifier == courseDistributionId);

                db.TCourseDistributions.RemoveRange(entities);
                db.SaveChanges();
            }
        }
    }
}

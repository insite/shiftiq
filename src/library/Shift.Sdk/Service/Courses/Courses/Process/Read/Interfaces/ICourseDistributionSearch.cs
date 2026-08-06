using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace InSite.Application.Courses.Read
{
    public interface ICourseDistributionSearch
    {
        int CountCredits(Guid organizationId, Guid managerUserId);
        List<TCourseDistribution> GetCreditDistributions(Guid organizationId, Guid managerUserId, int take);
        List<CourseDistributionGridItem> GetCourseDistributionsByManager(Guid organizationId, Guid managerUserId, bool includeTransferred);
        List<CourseDistributionGridItem> GetCourseDistributionsByLearner(Guid organizationId, Guid learnerUserId);
        TCourseDistribution GetCourseDistribution(Guid courseDistributionId, params Expression<Func<TCourseDistribution, object>>[] includes);
        int CountCourseDistributions(TCourseDistributionFilter filter);
        List<TCourseDistribution> GetCourseDistributions(TCourseDistributionFilter filter, params Expression<Func<TCourseDistribution, object>>[] includes);
    }
}

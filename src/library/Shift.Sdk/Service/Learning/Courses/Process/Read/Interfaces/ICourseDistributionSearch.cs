using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace InSite.Application.Courses.Read
{
    public interface ICourseDistributionSearch
    {
        List<CourseDistributionGridItem> GetCourseDistributionsByManager(Guid organizationId, Guid managerUserId);
        List<CourseDistributionGridItem> GetCourseDistributionsByLearner(Guid organizationId, Guid learnerUserId);
        TCourseDistribution GetCourseDistribution(Guid courseDistributionId);
        int CountCourseDistributions(TCourseDistributionFilter filter);
        List<TCourseDistribution> GetCourseDistributions(TCourseDistributionFilter filter, params Expression<Func<TCourseDistribution, object>>[] includes);
    }
}

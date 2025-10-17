using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace InSite.Application.Courses.Read
{
    public interface ICourseDistributionStore
    {
        void InsertCourseDistribution(TCourseDistribution distribution);
        void InsertCourseDistributions(IEnumerable<TCourseDistribution> list);
        void UpdateCourseDistribution(TCourseDistribution distribution);
        void UpdateCourseDistribution<TCourseDistribution>(Guid courseDistributionId, params (Expression<Func<TCourseDistribution, object>> Property, object Value)[] updates);
        void DeleteCourseDistribution(Guid courseDistributionId);
    }
}

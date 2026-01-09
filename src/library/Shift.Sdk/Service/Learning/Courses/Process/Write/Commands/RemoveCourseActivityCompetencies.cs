using System;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class RemoveCourseActivityCompetencies : Command, IHasRun
    {
        public Guid ActivityId { get; set; }
        public Guid[] CompetencyIds { get; set; }

        public RemoveCourseActivityCompetencies(Guid courseId, Guid activityId, Guid[] competencyIds)
        {
            AggregateIdentifier = courseId;
            ActivityId = activityId;
            CompetencyIds = competencyIds;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var activity = course.Data.GetActivity(ActivityId);
            if (activity == null || !CompetencyIds.Any(x => activity.Competencies.Any(y => y.CompetencyStandardIdentifier == x)))
                return false;

            course.Apply(new CourseActivityCompetenciesRemoved(ActivityId, CompetencyIds));
            return true;
        }
    }
}

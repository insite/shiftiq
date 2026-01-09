using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseActivityCompetenciesRemoved : Change
    {
        public Guid ActivityId { get; set; }
        public Guid[] CompetencyIds { get; set; }

        public CourseActivityCompetenciesRemoved(Guid activityId, Guid[] competencyIds)
        {
            ActivityId = activityId;
            CompetencyIds = competencyIds;
        }
    }
}

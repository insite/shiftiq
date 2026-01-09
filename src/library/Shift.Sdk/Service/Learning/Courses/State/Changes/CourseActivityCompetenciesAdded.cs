using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseActivityCompetenciesAdded : Change
    {
        public Guid ActivityId { get; set; }
        public ActivityCompetency[] Competencies { get; set; }

        public CourseActivityCompetenciesAdded(Guid activityId, ActivityCompetency[] competencies)
        {
            ActivityId = activityId;
            Competencies = competencies;
        }
    }
}

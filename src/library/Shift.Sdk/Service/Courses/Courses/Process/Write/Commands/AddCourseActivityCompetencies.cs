using System;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Shift.Common;

namespace InSite.Application.Courses.Write
{
    public class AddCourseActivityCompetencies : Command, IHasRun
    {
        public Guid ActivityId { get; set; }
        public ActivityCompetency[] Competencies { get; set; }

        public AddCourseActivityCompetencies(Guid courseId, Guid activityId, ActivityCompetency[] competencies)
        {
            AggregateIdentifier = courseId;
            ActivityId = activityId;
            Competencies = competencies;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var activity = course.Data.GetActivity(ActivityId);
            if (activity == null ||
                Competencies.All(x => activity.Competencies.Any(y =>
                    y.CompetencyStandardIdentifier == x.CompetencyStandardIdentifier
                    && StringHelper.EqualsCaseSensitive(y.CompetencyCode, x.CompetencyCode, true)
                    && StringHelper.EqualsCaseSensitive(y.RelationshipType, x.RelationshipType, true)
            )))
            {
                return false;
            }

            course.Apply(new CourseActivityCompetenciesAdded(ActivityId, Competencies));
            return true;
        }
    }
}

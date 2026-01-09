using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class ResequenceCourseActivities : Command, IHasRun
    {
        public Guid ModuleId { get; set; }

        public ResequenceCourseActivities(Guid courseId, Guid moduleId)
        {
            AggregateIdentifier = courseId;
            ModuleId = moduleId;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var module = course.Data.GetModule(ModuleId);
            if (module == null)
                return false;

            var activities = module.Activities
                .OrderBy(x => x.GetIntValue(ActivityField.ActivitySequence))
                .ToList();

            var activitiesAndSequences = new List<CourseActivitiesResequenced.Activity>();

            for (int i = 0; i < activities.Count; i++)
            {
                if (activities[i].GetIntValue(ActivityField.ActivitySequence) == i + 1)
                    continue;

                activitiesAndSequences.Add(new CourseActivitiesResequenced.Activity
                {
                    ActivityId = activities[i].Identifier,
                    Sequence = i + 1
                });
            }

            if (activitiesAndSequences.Count == 0)
                return false;

            course.Apply(new CourseActivitiesResequenced(ModuleId, activitiesAndSequences.ToArray()));

            return true;
        }
    }
}

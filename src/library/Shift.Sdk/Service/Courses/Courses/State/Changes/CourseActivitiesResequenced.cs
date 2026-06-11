using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseActivitiesResequenced : Change
    {
        public class Activity
        {
            public Guid ActivityId { get; set; }
            public int Sequence { get; set; }
        }
            
        public Guid ModuleId { get; set; }
        public Activity[] Activities { get; set; }

        public CourseActivitiesResequenced(Guid moduleId, Activity[] activities)
        {
            ModuleId = moduleId;
            Activities = activities;
        }
    }
}

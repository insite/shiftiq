using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseActivityUrlModified : Change
    {
        public CourseActivityUrlModified(Guid activityId, string url, string target, string type)
        {
            ActivityId = activityId;
            Url = url;
            Target = target;
            Type = type;
        }

        public Guid ActivityId { get; set; }
        public string Url { get; set; }
        public string Target { get; set; }
        public string Type { get; set; }
    }
}

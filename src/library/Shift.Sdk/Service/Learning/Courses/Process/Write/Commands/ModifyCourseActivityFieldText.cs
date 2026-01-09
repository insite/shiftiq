using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseActivityFieldText : Command, IHasRun
    {
        public Guid ActivityId { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public ActivityField ActivityField { get; set; }

        public string Value { get; set; }

        public ModifyCourseActivityFieldText(Guid courseId, Guid activityId, ActivityField activityField, string value)
        {
            AggregateIdentifier = courseId;
            ActivityId = activityId;
            ActivityField = activityField;
            Value = value;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var activity = course.Data.GetActivity(ActivityId);
            if (activity == null || StringHelper.EqualsCaseSensitive(activity.GetTextValue(ActivityField), Value, true))
                return false;

            course.Apply(new CourseActivityFieldTextModified(ActivityId, ActivityField, Value));
            return true;
        }
    }
}

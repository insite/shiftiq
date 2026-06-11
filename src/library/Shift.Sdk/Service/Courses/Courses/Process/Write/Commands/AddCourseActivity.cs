using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Application.Courses.Write
{
    public class AddCourseActivity : Command, IHasRun
    {
        public Guid ModuleId { get; set; }
        public Guid ActivityId { get; set; }
        public int ActivityAsset { get; set; }
        public string ActivityName { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public ActivityType ActivityType { get; set; }

        public ContentContainer ActivityContent { get; set; }

        public AddCourseActivity(
            Guid courseId,
            Guid moduleId,
            Guid activityId,
            int activityAsset,
            string activityName,
            ActivityType activityType,
            ContentContainer activityContent
            )
        {
            AggregateIdentifier = courseId;
            ModuleId = moduleId;
            ActivityId = activityId;
            ActivityAsset = activityAsset;
            ActivityName = activityName;
            ActivityType = activityType;
            ActivityContent = activityContent;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            course.Apply(new CourseActivityAdded(ModuleId, ActivityId, ActivityAsset, ActivityName, ActivityType, ActivityContent));
            return true;
        }
    }
}

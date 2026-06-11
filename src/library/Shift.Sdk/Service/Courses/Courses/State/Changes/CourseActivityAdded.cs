using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Courses
{
    public class CourseActivityAdded : Change
    {
        public Guid ModuleId { get; set; }
        public Guid ActivityId { get; set; }
        public int ActivityAsset { get; set; }
        public string ActivityName { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public ActivityType ActivityType { get; set; }

        public ContentContainer ActivityContent { get; set; }

        public CourseActivityAdded(Guid moduleId, Guid activityId, int activityAsset, string activityName, ActivityType activityType, ContentContainer activityContent)
        {
            ModuleId = moduleId;
            ActivityId = activityId;
            ActivityAsset = activityAsset;
            ActivityName = activityName;
            ActivityType = activityType;
            ActivityContent = activityContent;
        }
    }
}

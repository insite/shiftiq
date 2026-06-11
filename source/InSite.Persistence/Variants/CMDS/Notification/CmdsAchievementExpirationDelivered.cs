using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Persistence.Plugin.CMDS
{
    public class CmdsAchievementExpirationDelivered : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public ReminderType ReminderType { get; set; }

        public string Priority { get; set; }

        public CmdsAchievementExpirationDelivered(ReminderType reminderType, string priority)
        {
            ReminderType = reminderType;
            Priority = priority;
        }
    }
}
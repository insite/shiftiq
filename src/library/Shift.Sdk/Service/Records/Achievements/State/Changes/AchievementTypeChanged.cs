using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class AchievementTypeChanged : Change
    {
        public string Type { get; private set; }

        public AchievementTypeChanged(string type)
        {
            Type = type;
        }
    }
}

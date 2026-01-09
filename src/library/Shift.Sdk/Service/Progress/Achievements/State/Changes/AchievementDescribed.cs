using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class AchievementDescribed : Change
    {
        public AchievementDescribed(string label, string title, string description, bool allowSelfDeclared)
        {
            Label = label;
            Title = title;
            Description = description;
            AllowSelfDeclared = allowSelfDeclared;
        }

        public string Label { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool AllowSelfDeclared { get; set; }
    }
}
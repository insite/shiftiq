using System;

namespace InSite.Domain.Events
{
    public class ApprenticeSummary
    {
        public Guid EventIdentifier { get; set; }
        public string EventTitle { get; set; }
        public Guid? AchievementIdentifier { get; set; }
        public string AchievementTitle { get; set; }
        public int MemberCount { get; set; }
        public int NoEmployerCount { get; set; }
        public int TotalCount { get; set; }
    }
}

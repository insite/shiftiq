using System;

namespace InSite.Domain.Events
{
    public class EventParticipationSummary
    {
        public Guid EventIdentifier { get; set; }
        public DateTimeOffset EventScheduledStart { get; set; }
        public DateTimeOffset? EventScheduledEnd { get; set; }
        public string EventSchedulingStatus { get; set; }
        public Guid AchievementIdentifier { get; set; }
        public string AchievementDescription { get; set; }
        public string AchievementTitle { get; set; }
        public int RegistrationCount { get; set; }
    }
}
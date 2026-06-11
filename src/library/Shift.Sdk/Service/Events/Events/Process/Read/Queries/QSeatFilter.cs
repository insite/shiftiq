using System;

using Shift.Common;

namespace InSite.Application.Events.Read
{
    [Serializable]
    public class QSeatFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public DateTimeOffset? EventScheduledSince { get; set; }
        public DateTimeOffset? EventScheduledBefore { get; set; }
        public Guid? AchievementIdentifier { get; set; }
        public string EventTitle { get; set; }
        public string SeatTitle { get; set; }
        public bool? IsAvailable { get; set; }
        public bool? IsTaxable { get; set; }
        public Guid? EventIdentifier { get; set; }
    }
}

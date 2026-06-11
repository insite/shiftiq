using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class AchievementExpiryChanged : Change
    {
        public Expiration Expiration { get; set; }

        public AchievementExpiryChanged(Expiration expiration)
        {
            Expiration = expiration;
        }
    }
}
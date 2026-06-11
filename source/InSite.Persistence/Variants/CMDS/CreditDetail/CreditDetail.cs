using System;

namespace InSite.Persistence.Plugin.CMDS
{
    public class CreditDetail
    {
        public DateTimeOffset? DateCompleted { get; set; }
        public DateTimeOffset? DateExpired { get; set; }
        public Boolean IsInTrainingPlan { get; set; }
        public Boolean IsRequired { get; set; }
        public Boolean? IsTimeSensitive { get; set; }
        public Guid ResourceIdentifier { get; set; }
        public String ResourceTitle { get; set; }
        public String ResourceType { get; set; }
        public Decimal ScoreScaled { get; set; }
        public String UserEmail { get; set; }
        public Guid UserIdentifier { get; set; }
        public Boolean? UserIsArchived { get; set; }
        public String ValidationStatus { get; set; }
    }
}

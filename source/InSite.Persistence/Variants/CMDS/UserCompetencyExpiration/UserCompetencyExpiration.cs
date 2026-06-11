using System;

namespace InSite.Persistence.Plugin.CMDS
{
    public class UserCompetencyExpiration
    {
        public Guid CompetencyStandardIdentifier { get; set; }
        public DateTimeOffset? ExpirationDate { get; set; }
        public Int32? LifetimeInMonths { get; set; }
        public DateTimeOffset? Notified { get; set; }
        public Guid UserIdentifier { get; set; }
        public DateTimeOffset? ValidationDate { get; set; }
        public String ValidationStatus { get; set; }
    }
}

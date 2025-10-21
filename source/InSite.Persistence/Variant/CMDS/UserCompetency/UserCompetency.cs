using System;

namespace InSite.Persistence.Plugin.CMDS
{
    public class UserCompetency
    {
        public Guid CompetencyStandardIdentifier { get; set; }
        public DateTimeOffset? ExpirationDate { get; set; }
        public Boolean IsModuleQuizCompleted { get; set; }
        public Boolean IsValidated { get; set; }
        public DateTimeOffset? Notified { get; set; }
        public DateTimeOffset? SelfAssessmentDate { get; set; }
        public String SelfAssessmentStatus { get; set; }
        public Guid UserIdentifier { get; set; }
        public String ValidationComment { get; set; }
        public DateTimeOffset? ValidationDate { get; set; }
        public String ValidationStatus { get; set; }
        public Guid? ValidatorUserIdentifier { get; set; }
    }
}

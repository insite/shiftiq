using System;

namespace InSite.Domain.Records
{
    public class ProgramValuesResult
    {
        public Guid ProgramIdentifier { get; set; }
        public string ProgramName { get; set; }
        public Guid? TaskIdentifier { get; set; }
        public Guid? CompletionTaskIdentifier { get; set; }
        public Guid? NotificationCompletedAdministratorMessageIdentifier { get; set; }
        public Guid? NotificationCompletedLearnerMessageIdentifier { get; set; }
        public Guid? AchievementIdentifier { get; set; }
    }
}

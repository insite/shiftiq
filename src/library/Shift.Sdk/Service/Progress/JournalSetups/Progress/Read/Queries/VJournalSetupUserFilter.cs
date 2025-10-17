using System;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Records.Read
{
    [Serializable]
    public class VJournalSetupUserFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? JournalSetupIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }
        public JournalSetupUserRole? Role { get; set; }
        public Guid? ValidatorUserIdentifier { get; set; }
        public Guid? ExcludeAchievementIdentifier { get; set; }
        public string UserKeyword { get; set; }
    }
}
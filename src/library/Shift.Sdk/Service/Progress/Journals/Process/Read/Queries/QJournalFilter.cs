using System;

using Shift.Common;

namespace InSite.Application.Records.Read
{
    [Serializable]
    public class QJournalFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? JournalSetupIdentifier { get; set; }
        public Guid? CompetencyStandardIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }
        public Guid? ValidatorUserIdentifier { get; set; }
    }
}

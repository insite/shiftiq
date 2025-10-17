using System;

using Shift.Common;

namespace InSite.Application.Records.Read
{
    [Serializable]
    public class QJournalSetupGroupFilter : Filter
    {
        public Guid JournalSetupIdentifier { get; set; }
        public string GroupName { get; set; }
    }
}

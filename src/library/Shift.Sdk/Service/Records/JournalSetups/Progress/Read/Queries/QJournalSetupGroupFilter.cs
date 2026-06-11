using System;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Records.Read
{
    [Serializable]
    public class QJournalSetupGroupFilter : Filter
    {
        public Guid JournalSetupIdentifier { get; set; }
        public JournalSetupUserRole GroupRole { get; set; }
        public string GroupName { get; set; }
    }
}

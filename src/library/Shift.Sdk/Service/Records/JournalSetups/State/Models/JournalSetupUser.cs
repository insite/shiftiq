using System;

using Shift.Constant;

namespace InSite.Domain.Records
{
    public class JournalSetupUser
    {
        public Guid Identifier { get; set; }
        public JournalSetupUserRole Role { get; set; }
    }
}

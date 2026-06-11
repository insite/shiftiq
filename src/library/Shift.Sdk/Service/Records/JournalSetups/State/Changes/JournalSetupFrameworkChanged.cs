using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class JournalSetupFrameworkChanged : Change
    {
        public Guid? Framework { get; }

        public JournalSetupFrameworkChanged(Guid? framework)
        {
            Framework = framework;
        }
    }
}

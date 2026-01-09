using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class JournalSetupRenamed : Change
    {
        public string Name { get; }

        public JournalSetupRenamed(string name)
        {
            Name = name;
        }
    }
}

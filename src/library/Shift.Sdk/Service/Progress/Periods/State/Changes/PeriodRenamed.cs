using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class PeriodRenamed : Change
    {
        public PeriodRenamed(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
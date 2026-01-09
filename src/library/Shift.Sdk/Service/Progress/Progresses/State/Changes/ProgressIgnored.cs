using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class ProgressIgnored : Change
    {
        public bool IsIgnored { get; }

        public ProgressIgnored(bool isIgnored)
        {
            IsIgnored = isIgnored;
        }
    }
}

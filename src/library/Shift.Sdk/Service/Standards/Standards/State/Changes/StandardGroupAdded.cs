using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Standards
{
    public class StandardGroupAdded : Change
    {
        public StandardGroup[] Groups { get; }

        public StandardGroupAdded(StandardGroup[] groups)
        {
            Groups = groups;
        }
    }
}

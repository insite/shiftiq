using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Standards
{
    public class StandardConnectionAdded : Change
    {
        public StandardConnection[] Connections { get; }

        public StandardConnectionAdded(StandardConnection[] connections)
        {
            Connections = connections;
        }
    }
}

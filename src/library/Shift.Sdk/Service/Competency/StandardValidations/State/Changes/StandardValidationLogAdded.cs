using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Standards
{
    public class StandardValidationLogAdded : Change
    {
        public StandardValidationLog[] Logs { get; }

        public StandardValidationLogAdded(StandardValidationLog[] logs)
        {
            Logs = logs;
        }
    }
}

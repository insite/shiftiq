using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Standards
{
    public class StandardValidationLogModified : Change
    {
        public StandardValidationLog Log { get; }

        public StandardValidationLogModified(StandardValidationLog log)
        {
            Log = log;
        }
    }
}

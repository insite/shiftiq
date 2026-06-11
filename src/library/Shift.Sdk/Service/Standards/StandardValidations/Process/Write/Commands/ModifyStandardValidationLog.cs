using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Standards;

namespace InSite.Application.StandardValidations.Write
{
    public class ModifyStandardValidationLog : Command
    {
        public StandardValidationLog Log { get; set; }

        public ModifyStandardValidationLog(Guid standardValidationId, Guid logId, Guid authorUserId, DateTimeOffset posted, string status, string comment)
            : this(standardValidationId, new StandardValidationLog(logId, authorUserId, posted, status, comment))
        {
        }

        public ModifyStandardValidationLog(Guid standardValidationId, StandardValidationLog log)
        {
            AggregateIdentifier = standardValidationId;
            Log = log;
        }
    }
}

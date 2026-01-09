using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Standards;

namespace InSite.Application.StandardValidations.Write
{
    public class AddStandardValidationLog : Command
    {
        public StandardValidationLog[] Logs { get; set; }

        public AddStandardValidationLog(Guid standardValidationId, Guid logId, string status, string comment)
            : this(standardValidationId, new[] { new StandardValidationLog(logId, status, comment) })
        {
        }

        public AddStandardValidationLog(Guid standardValidationId, Guid logId, Guid authorUserId, DateTimeOffset posted, string status, string comment)
            : this(standardValidationId, new[] { new StandardValidationLog(logId, authorUserId, posted, status, comment) })
        {
        }

        public AddStandardValidationLog(Guid standardValidationId, StandardValidationLog[] logs)
        {
            AggregateIdentifier = standardValidationId;
            Logs = logs;
        }
    }
}

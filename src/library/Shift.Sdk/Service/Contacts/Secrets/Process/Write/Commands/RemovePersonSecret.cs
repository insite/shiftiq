using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.People.Write
{
    public class RemovePersonSecret : Command
    {
        public Guid PersonId { get; set; }
        public Guid OrganizationId { get; set; }

        public RemovePersonSecret(Guid personId, Guid secretId)
        {
            AggregateIdentifier = secretId;
            PersonId = personId;
        }
    }
}

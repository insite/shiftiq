using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.People.Write
{
    public class CreatePerson : Command
    {
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }

        public CreatePerson(Guid personId, Guid userId, Guid organizationId)
        {
            AggregateIdentifier = personId;
            UserId = userId;
            OrganizationId = organizationId;
        }
    }
}
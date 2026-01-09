using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.People.Write
{
    public class DeletePerson : Command
    {
        public DeletePerson(Guid personId)
        {
            AggregateIdentifier = personId;
        }
    }
}

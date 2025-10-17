using System;

using Common.Timeline.Commands;

namespace InSite.Application.Standards.Write
{
    public class RemoveStandard : Command
    {
        public RemoveStandard(Guid standardId)
        {
            AggregateIdentifier = standardId;
        }
    }
}

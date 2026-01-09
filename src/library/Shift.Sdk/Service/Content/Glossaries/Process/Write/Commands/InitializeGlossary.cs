using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Glossaries.Write
{
    public class InitializeGlossary : Command
    {
        public InitializeGlossary(Guid aggregate)
        {
            AggregateIdentifier = aggregate;
        }
    }
}
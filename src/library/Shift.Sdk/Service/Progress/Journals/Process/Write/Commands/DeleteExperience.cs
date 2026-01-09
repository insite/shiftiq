using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Journals.Write
{
    public class DeleteExperience : Command
    {
        public Guid Experience { get; }

        public DeleteExperience(Guid journal, Guid experience)
        {
            AggregateIdentifier = journal;
            Experience = experience;
        }
    }
}

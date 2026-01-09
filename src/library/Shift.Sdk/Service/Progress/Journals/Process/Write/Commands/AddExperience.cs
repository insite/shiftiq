using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Journals.Write
{
    public class AddExperience : Command
    {
        public Guid Experience { get; }

        public AddExperience(Guid journal, Guid experience)
        {
            AggregateIdentifier = journal;
            Experience = experience;
        }
    }
}

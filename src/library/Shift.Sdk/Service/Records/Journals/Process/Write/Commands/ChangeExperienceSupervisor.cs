using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Journals.Write
{
    public class ChangeExperienceSupervisor : Command
    {
        public Guid Experience { get; }
        public string Supervisor { get; }

        public ChangeExperienceSupervisor(Guid journal, Guid experience, string supervisor)
        {
            AggregateIdentifier = journal;
            Experience = experience;
            Supervisor = supervisor;
        }
    }
}

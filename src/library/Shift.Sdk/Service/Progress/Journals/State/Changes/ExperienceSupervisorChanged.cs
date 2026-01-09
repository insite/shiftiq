using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class ExperienceSupervisorChanged : Change
    {
        public Guid Experience { get; }
        public string Supervisor { get; }

        public ExperienceSupervisorChanged(Guid experience, string supervisor)
        {
            Experience = experience;
            Supervisor = supervisor;
        }
    }
}

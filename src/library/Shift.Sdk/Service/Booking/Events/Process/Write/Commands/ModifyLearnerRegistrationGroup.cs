using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ModifyLearnerRegistrationGroup : Command
    {
        public Guid? LearnerRegistrationGroup { get; }

        public ModifyLearnerRegistrationGroup(Guid @event, Guid? learnerRegistrationGroup)
        {
            AggregateIdentifier = @event;
            LearnerRegistrationGroup = learnerRegistrationGroup;
        }
    }
}

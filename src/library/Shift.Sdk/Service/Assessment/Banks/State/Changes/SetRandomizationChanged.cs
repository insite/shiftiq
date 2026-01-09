using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class SetRandomizationChanged : Change
    {
        public Guid Set { get; set; }
        public Randomization Randomization { get; set; }

        public SetRandomizationChanged(Guid set, Randomization randomization)
        {
            Set = set;
            Randomization = randomization;
        }
    }
}

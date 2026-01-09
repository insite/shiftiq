using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class LearnerRegistrationGroupModified : Change
    {
        public Guid? LearnerRegistrationGroup { get; set; }

        public LearnerRegistrationGroupModified(Guid? learnerRegistrationGroup)
        {
            LearnerRegistrationGroup = learnerRegistrationGroup;
        }
    }
}

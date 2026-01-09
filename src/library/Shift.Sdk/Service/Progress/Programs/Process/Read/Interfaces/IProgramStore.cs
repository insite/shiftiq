using System;
using System.Collections.Generic;

namespace InSite.Application.Records.Read
{
    public interface IProgramStore
    {
        List<TaskEnrollment> TaskCompleted(Guid LearnerIdentifier, Guid OrganizationIdentifier, Guid ObjectIdentifier);
        void TaskViewed(Guid LearnerIdentifier, Guid OrganizationIdentifier, Guid ObjectIdentifier);
        void ProgramCompleted(Guid ProgramIdentifier, Guid LearnerIdentifier, Guid OrganizationIdentifier, Guid? AchievementIdentifier);
    }
}

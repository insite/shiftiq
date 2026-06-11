using System;

namespace InSite.Application.Records.Read
{
    public interface IProgramService
    {
        void CompletionOfProgramAchievement(Guid ProgramIdentifier , Guid LearnerIdentifier, Guid OrganizationIdentifier);
    }
}

using System;
using System.Collections.Generic;

using InSite.Domain.Records;

namespace InSite.Application.Records.Read
{
    public interface IProgramSearch
    {
        List<SubmittedProgram> GetProgramsForSubmit(Guid organizationId, List<Guid> programsIds);

        ProgramValuesResult GetProgramValues(Guid programId, Guid taskObjectId);

        List<Guid> GetProgramIds(Guid taskObjectId);
        List<Guid> GetProgramIdsForStandaloneAchievements(Guid taskObjectId, out List<Guid?> objects);

        bool IsProgramFullyCompletedByLearner(Guid programId, Guid userId);
        bool IsTaskCompletionPrerequisite(Guid programId, Guid taskObjectId);
        bool IsTaskCompletedByLearner(Guid value, Guid userIdentifier);
    }
}

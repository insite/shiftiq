using System;
using System.Collections.Generic;

using InSite.Domain.Records;

using Shift.Common;

namespace InSite.Application.Records.Read
{
    public interface IProgramSearch
    {
        Guid? GetGroupEnrollmentProgramId(Guid userId, Guid objectId);
        bool IsTaskEnrollmentExist(Guid userId, Guid objectId);

        int CountProgramGroups(Guid programId, string keyword);
        List<ProgramGroup> GetProgramGroups(Guid programId, string keyword, Paging paging);

        List<SubmittedProgram> GetProgramsForSubmit(Guid organizationId, List<Guid> programsIds, string language);

        ProgramValuesResult GetProgramValues(Guid programId, Guid taskObjectId);

        List<Guid> GetProgramIds(Guid taskObjectId);
        List<Guid> GetProgramIdsForStandaloneAchievements(Guid taskObjectId, out List<Guid?> objects);

        bool IsProgramFullyCompletedByLearner(Guid programId, Guid userId);
        bool IsTaskCompletionPrerequisite(Guid programId, Guid taskObjectId);
        bool IsTaskCompletedByLearner(Guid value, Guid userIdentifier);
    }
}

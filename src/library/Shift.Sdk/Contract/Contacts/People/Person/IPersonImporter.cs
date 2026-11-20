using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shift.Contract
{
    public interface IPersonImporter
    {
        Task<ImportPersonResult[]> ImportAsync(
            Guid organizationId,
            string fullNamePolicy,
            string timeZone,
            string employerGroupName,
            string[] groupNames,
            Guid submittedBy,
            string submittedByName,
            IEnumerable<ImportPerson> imports
        );

        Task<ImportPersonResult[]> ImportPendingPeopleAsync(
            Guid organizationId,
            string fullNamePolicy,
            string timeZone,
            string emloyerGroupName,
            string[] groupNames,
            string submittedByName,
            IEnumerable<ImportPendingPerson> imports
        );
    }
}

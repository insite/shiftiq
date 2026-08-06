using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using InSite.Application.Files.Read;

namespace Shift.Contract
{
    public interface IPersonImportReporter
    {
        Task<FileStorageModel> SaveReportAsync(Guid organizationId, Guid userId, string timeZone, IEnumerable<ImportPersonResult> imports, bool isAutoImport);
    }
}

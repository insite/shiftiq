using System;

namespace InSite.Application.Standards.Read
{
    public interface IStandardValidationSearch
    {
        bool Exists(Guid standardId, Guid userId, Guid? excludeValidationId = null);
        QStandardValidation GetStandardValidation(Guid standardValidationId);
        QStandardValidation GetStandardValidation(Guid standardId, Guid userId);
        QStandardValidationLog GetStandardValidationLog(Guid logId);
        int CountStandardValidationLogs(Guid standardValidationId);
    }
}

using System;

using InSite.Domain.Standards;

namespace InSite.Application.Standards.Read
{
    public interface IStandardValidationStore
    {
        void Insert(StandardValidationCreated e);
        void Delete(StandardValidationRemoved e);
        void Update(StandardValidationTimestampsModified e);
        void Update(StandardValidationFieldTextModified e);
        void Update(StandardValidationFieldDateOffsetModified e);
        void Update(StandardValidationFieldBoolModified e);
        void Update(StandardValidationFieldGuidModified e);
        void Update(StandardValidationFieldsModified e);
        void Update(StandardValidationSelfValidated e);
        void Update(StandardValidationStatusModified e);
        void Update(StandardValidationSubmittedForValidation e);
        void Update(StandardValidationValidated e);
        void Update(StandardValidationExpired e);
        void Update(StandardValidationNotified e);
        void Update(StandardValidationLogAdded e);
        void Update(StandardValidationLogModified e);
        void Update(StandardValidationLogRemoved e);
        void DeleteAll(Guid value);
        void DeleteAll();
    }
}

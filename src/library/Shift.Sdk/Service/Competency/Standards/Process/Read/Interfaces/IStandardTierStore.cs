using System;

using InSite.Domain.Standards;

namespace InSite.Application.Standards.Read
{
    public interface IStandardTierStore
    {
        void Insert(StandardCreated e);
        void Delete(StandardRemoved e);
        void Update(StandardFieldGuidModified e);
        void Update(StandardFieldsModified e);
        void DeleteAll(Guid value);
        void DeleteAll();
    }
}

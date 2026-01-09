using System;

using InSite.Domain.Standards;

namespace InSite.Application.Standards.Read
{
    public interface IStandardStore
    {
        void Insert(StandardCreated e);
        void Delete(StandardRemoved e);
        void Update(StandardTimestampsModified e);
        void Update(StandardCategoryAdded e);
        void Update(StandardCategoryRemoved e);
        void Update(StandardConnectionAdded e);
        void Update(StandardConnectionRemoved e);
        void Update(StandardContainmentAdded e);
        void Update(StandardContainmentModified e);
        void Update(StandardContainmentRemoved e);
        void Update(StandardContentModified e);
        void Update(StandardOrganizationAdded e);
        void Update(StandardOrganizationRemoved e);
        void Update(StandardAchievementAdded e);
        void Update(StandardAchievementRemoved e);
        void Update(StandardGroupAdded e);
        void Update(StandardGroupRemoved e);
        void Update(StandardFieldTextModified e);
        void Update(StandardFieldDateOffsetModified e);
        void Update(StandardFieldBoolModified e);
        void Update(StandardFieldIntModified e);
        void Update(StandardFieldDecimalModified e);
        void Update(StandardFieldGuidModified e);
        void Update(StandardFieldsModified e);
        void DeleteAll(Guid value);
        void DeleteAll();
    }
}

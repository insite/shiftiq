using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.UI.Admin.Database.Entities
{
    public partial class SearchCriteria : SearchCriteriaController<TEntityFilter>
    {
        public override TEntityFilter Filter
        {
            get
            {
                var filter = new TEntityFilter()
                {
                    SubsystemType = SubsystemType.Text,
                    SubsystemName = SubsystemName.Text,
                    SubsystemComponent = SubsystemComponent.Text,
                    EntityName = EntityName.Text,

                    StorageStructure = StorageStructure.Text,
                    StorageSchema = StorageSchema.Text,
                    StorageTable = StorageTable.Text,
                    StorageKey = StorageKey.Text,

                    Keyword = Keyword.Text,
                    CollectionSlug = CollectionSlug.Text
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                SubsystemType.Text = value.SubsystemType;
                SubsystemName.Text = value.SubsystemName;
                SubsystemComponent.Text = value.SubsystemComponent;
                EntityName.Text = value.EntityName;

                StorageStructure.Text = value.StorageStructure;
                StorageSchema.Text = value.StorageSchema;
                StorageTable.Text = value.StorageTable;
                StorageKey.Text = value.StorageKey;

                Keyword.Text = value.Keyword;
                CollectionSlug.Text = value.CollectionSlug;
            }
        }

        public override void Clear()
        {
            SubsystemType.Text = null;
            SubsystemName.Text = null;
            SubsystemComponent.Text = null;
            EntityName.Text = null;

            StorageStructure.Text = null;
            StorageSchema.Text = null;
            StorageTable.Text = null;
            StorageKey.Text = null;

            Keyword.Text = null;
            CollectionSlug.Text = null;
        }
    }
}
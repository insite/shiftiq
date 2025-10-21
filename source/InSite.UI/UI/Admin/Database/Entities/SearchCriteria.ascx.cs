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
                    ComponentType = ComponentType.Text,
                    ComponentName = ComponentName.Text,
                    ComponentPart = ComponentPart.Text,
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
                ComponentType.Text = value.ComponentType;
                ComponentName.Text = value.ComponentName;
                ComponentPart.Text = value.ComponentPart;
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
            ComponentType.Text = null;
            ComponentName.Text = null;
            ComponentPart.Text = null;
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
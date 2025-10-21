using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.UI.Admin.Learning.Catalogs
{
    public partial class SearchCriteria : SearchCriteriaController<TCatalogFilter>
    {
        public override TCatalogFilter Filter
        {
            get
            {
                var filter = new TCatalogFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    CatalogName = CatalogName.Text
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                CatalogName.Text = value.CatalogName;
            }
        }

        public override void Clear()
        {
            CatalogName.Text = null;
        }
    }
}
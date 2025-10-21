using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.UI.Admin.Learning.Categories.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<TCollectionItemFilter>
    {
        public override TCollectionItemFilter Filter
        {
            get
            {
                var filter = new TCollectionItemFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    CollectionName = Edit.CollectionName,
                    ItemFolder = CategoryFolder.Value,
                    ItemNameContains = CategoryName.Text
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                CategoryFolder.Value = value.ItemFolder;
                CategoryName.Text = value.ItemNameContains;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            CategoryFolder.OrganizationIdentifier = Organization.Identifier;
            CategoryFolder.CollectionIdentifier = Edit.GetCollectionId();
            CategoryFolder.RefreshData();
        }

        public override void Clear()
        {
            CategoryFolder.Value = null;
            CategoryName.Text = null;
        }
    }
}
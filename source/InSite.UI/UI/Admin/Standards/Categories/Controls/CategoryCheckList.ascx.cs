using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Persistence;

using Shift.Constant;

namespace InSite.Admin.Standards.Categories.Controls
{
    public partial class CategoryCheckList : System.Web.UI.UserControl
    {
        protected Guid AssetID
        {
            get => (Guid?)ViewState[nameof(AssetID)] ?? Guid.Empty;
            set => ViewState[nameof(AssetID)] = value;
        }

        protected Guid AssetThumbprint
        {
            get => (Guid?)ViewState[nameof(AssetThumbprint)] ?? Guid.Empty;
            set => ViewState[nameof(AssetThumbprint)] = value;
        }

        public void LoadData(Standard asset)
        {
            var organizationId = asset.OrganizationIdentifier;
            var assetID = asset.StandardIdentifier;

            AssetID = assetID;
            AssetThumbprint = asset.StandardIdentifier;

            var categories = TCollectionItemCache.Select(new TCollectionItemFilter
            {
                OrganizationIdentifier = organizationId,
                CollectionName = CollectionName.Standards_Standards_Category_Name,
                ItemFolder = asset.StandardType
            });
            var selCatIDs = StandardClassificationSearch.SelectByAssetIdentifier(asset.StandardIdentifier).Select(ac => ac.CategoryIdentifier);

            var dt = new DataTable();
            dt.Columns.Add("CategoryIdentifier", typeof(Guid));
            dt.Columns.Add("CategoryName");
            dt.Columns.Add("Selected", typeof(bool));

            foreach (var category in categories)
            {
                var row = dt.NewRow();
                row["CategoryIdentifier"] = category.ItemIdentifier;
                row["CategoryName"] = category.ItemName;
                row["Selected"] = selCatIDs.Contains(category.ItemIdentifier);
                dt.Rows.Add(row);
            }

            Repeater.DataSource = dt;
            Repeater.DataBind();

            Instructions.Text = categories.Count == 0
                ? $"There are no categories defined for the <strong>{asset.StandardType}</strong> standard type."
                : string.Empty;
        }

        protected void IsSelected_CheckedChanged(object sender, EventArgs e)
        {
            var checkBox = (CheckBox)sender;

            var standardIdentifier = AssetThumbprint;
            var categoryIdentifier = Guid.Parse(checkBox.Attributes["data-id"]);
            var ac = StandardClassificationSearch.Select(standardIdentifier, categoryIdentifier).FirstOrDefault();

            if (checkBox.Checked)
            {
                // Add
                if (ac == null)
                {
                    ac = new StandardClassification
                    {
                        CategoryIdentifier = categoryIdentifier,
                        StandardIdentifier = standardIdentifier
                    };

                    StandardClassificationStore.Insert(ac);
                }
            }
            else
            {
                // Remove
                if (ac != null)
                {
                    ac = new StandardClassification
                    {
                        CategoryIdentifier = categoryIdentifier,
                        StandardIdentifier = standardIdentifier
                    };

                    StandardClassificationStore.Delete(ac);
                }
            }
        }
    }
}
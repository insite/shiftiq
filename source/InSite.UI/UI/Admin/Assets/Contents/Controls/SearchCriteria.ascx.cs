using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Assets.Contents.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<TContentFilter>
    {
        public override TContentFilter Filter
        {
            get
            {
                var filter = new TContentFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    ContentLabel = ContentLabel.Text,
                    ContentLanguage = ContentLanguage.Value,
                    TextHTML = TextHTML.Text,
                    ContainerType = ContainerType.Value,
                    ContainerIdentifier = ContainerIdentifier.Text
                };

                GetCheckedShowColumns(filter);

                filter.OrderBy = SortColumns.Value;

                return filter;
            }
            set
            {
                ContentLabel.Text = value.ContentLabel;
                ContentLanguage.Value = value.ContentLanguage;
                TextHTML.Text = value.TextHTML;
                ContainerIdentifier.Text = value.ContainerIdentifier;
                ContainerType.Value = value.ContainerType;

                SortColumns.Value = value.OrderBy;
            }
        }

        public override void Clear()
        {
            ContentLabel.Text = null;
            ContentLanguage.Value = null;
            TextHTML.Text = null;
            ContainerIdentifier.Text = null;
            ContainerType.Value = null;
        }
    }
}
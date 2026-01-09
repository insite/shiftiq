using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Constant;

namespace InSite.Admin.Standards.Collections.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<StandardFilter>
    {
        public override StandardFilter Filter
        {
            get
            {
                var filter = new StandardFilter
                {
                    OrganizationIdentifier = Organization.Key,
                    StandardTypes = new[] { StandardType.Collection },

                    StandardLabel = AssetLabel.Text,
                    Title = ExternalTitle.Text,
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                AssetLabel.Text = value.StandardLabel;
                ExternalTitle.Text = value.Title;
            }
        }

        public override void Clear()
        {
            AssetLabel.Text = null;
            ExternalTitle.Text = null;
        }
    }
}
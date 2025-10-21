using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.UI.Portal.Standards.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<StandardFilter>
    {
        public override StandardFilter Filter
        {
            get
            {
                var filter = new StandardFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    Title = StandardTitle.Text,
                    StandardLabel = StandardLabel.Text,
                    Code = StandardCode.Text,
                    Number = AssetNumber.ValueAsInt
                };
                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                StandardTitle.Text = value.Title;
                StandardLabel.Text = value.StandardLabel;
                StandardCode.Text = value.Code;
                AssetNumber.ValueAsInt = value.Number;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        public override void Clear()
        {
            StandardTitle.Text = null;
            StandardLabel.Text = null;
            StandardCode.Text = null;
            AssetNumber.ValueAsInt = null;
        }
    }
}
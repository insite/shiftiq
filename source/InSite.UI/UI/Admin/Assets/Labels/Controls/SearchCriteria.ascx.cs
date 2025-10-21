using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Utilities.Labels.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<LabelFilter>
    {
        public override LabelFilter Filter
        {
            get
            {
                var filter = new LabelFilter
                {
                    LabelName = LabelName.Text,
                    LabelTranslation = LabelTranslation.Text,
                    OrganizationIdentifier = LabelOrganization.Value
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                LabelName.Text = value.LabelName;
                LabelTranslation.Text = value.LabelTranslation;

                if (!Identity.IsOperator)
                {
                    LabelOrganization.Value = Identity.Organization.Identifier;
                    LabelOrganization.Enabled = false;
                }
            }
        }

        public override void Clear()
        {
            LabelName.Text = null;
            LabelTranslation.Text = null;
            LabelOrganization.Value = null;

            if (!Identity.IsOperator)
            {
                LabelOrganization.Value = Identity.Organization.Identifier;
                LabelOrganization.Enabled = false;
            }
        }
    }
}
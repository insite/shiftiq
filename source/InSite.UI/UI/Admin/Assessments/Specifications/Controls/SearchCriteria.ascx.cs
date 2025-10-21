using InSite.Application.Banks.Read;
using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.Admin.Assessments.Specifications.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QBankSpecificationFilter>
    {
        public override QBankSpecificationFilter Filter
        {
            get
            {
                var filter = new QBankSpecificationFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    SpecAsset = ValueConverter.ToInt32Nullable(SpecAsset.Text),
                    SpecName = SpecName.Text,
                    SpecType = SpecType.Text,
                    OrderBy = OrderBy.Value
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                SpecAsset.Text = ValueConverter.ToString(value.SpecAsset);
                SpecName.Text = value.SpecName;
                SpecType.Text = value.SpecType;
                OrderBy.Value = value.OrderBy;
            }
        }

        public override void Clear()
        {
            SpecAsset.Text = null;
            SpecName.Text = null;
            SpecType.Text = null;
            OrderBy.ClearSelection();
        }
    }
}
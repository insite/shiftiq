using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

namespace InSite.UI.Admin.Records.Rurbics.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QRubricFilter>
    {
        public override QRubricFilter Filter
        {
            get
            {
                var filter = new QRubricFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    RubricTitle = RubricTitle.Text,
                    CreatedSince = CreatedSince.Value,
                    CreatedBefore = CreatedBefore.Value
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                RubricTitle.Text = value.RubricTitle;
                CreatedSince.Value = value.CreatedSince;
                CreatedBefore.Value = value.CreatedBefore;
            }
        }

        public override void Clear()
        {
            RubricTitle.Text = null;
            CreatedSince.Value = null;
            CreatedBefore.Value = null;
        }
    }
}
using InSite.Application.Surveys.Read;
using InSite.Common.Web.UI;

namespace InSite.UI.Portal.Surveys.Controls
{
    public partial class RespondSearchCriteria : SearchCriteriaController<QResponseSessionFilter>
    {
        public override QResponseSessionFilter Filter
        {
            get
            {
                var filter = new QResponseSessionFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    RespondentUserIdentifier = User.Identifier
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {

            }
        }

        public override void Clear()
        {

        }
    }
}
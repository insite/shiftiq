using InSite.Application.Surveys.Read;
using InSite.Common.Web.UI;

namespace InSite.UI.Portal.Workflow.Forms.Controls
{
    public partial class SubmitSearchCriteria : SearchCriteriaController<QResponseSessionFilter>
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
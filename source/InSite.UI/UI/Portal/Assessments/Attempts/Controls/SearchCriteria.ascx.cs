using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.UI.Portal.Assessments.Attempts.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QAttemptFilter>
    {
        public override QAttemptFilter Filter
        {
            get
            {
                var filter = new QAttemptFilter
                {
                    LearnerUserIdentifier = User.UserIdentifier,
                    FormOrganizationIdentifier = Organization.Identifier
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
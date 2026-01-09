using System;
using System.Linq;

using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;

namespace InSite.Admin.Assessments.Assessor.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QAttemptFilter>
    {
        protected override bool EnableSearchValidation => true;

        public override QAttemptFilter Filter
        {
            get
            {
                var organizationPersonTypes = OrganizationRole.ValuesArray.Length > 0
                    ? OrganizationRole.ValuesArray
                    : OrganizationRole.FlattenOptions().Select(x => x.Value).ToArray();

                var filter = new QAttemptFilter
                {
                    FormOrganizationIdentifier = Identity.Organization.Identifier,
                    GradingAssessorIdentifier = Identity.User.Identifier,
                    CandidateOrganizationIdentifiers = new[] { Identity.Organization.Identifier },
                    CandidateType = null,
                    OrganizationPersonTypes = organizationPersonTypes,
                    IsCompleted = false
                };

                GetCheckedShowColumns(filter);

                if (HideLearnerName.Checked)
                    filter.ShowColumns.Remove("EXAM CANDIDATE");

                filter.OrderBy = SortColumns.Value;

                return filter;
            }
            set
            {
                OrganizationRole.Values = value.OrganizationPersonTypes;

                SortColumns.Value = value.OrderBy;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetCheckAll(OrganizationRole, "Organization Role");
        }

        public override void Clear()
        {
            CandidateType.ClearSelection();

            OrganizationRole.Values = new[] { "Learner" };
        }
    }
}
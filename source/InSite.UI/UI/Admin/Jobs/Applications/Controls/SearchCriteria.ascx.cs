using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Jobs.Applications.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<TApplicationFilter>
    {
        protected override string[] DefaultShowColumns => new[] { "Employer Name", "Job Position", "Candidate", "Cover Letter", "Resume", "Updated" };

        public override TApplicationFilter Filter
        {
            get
            {
                var filter = new TApplicationFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    OpportunityIdentifier = JobID.Value,
                    UserIdentifier = CandidateContactID.Value,
                    EmployerName = EmployerName.Text,
                    JobTitle = JobPosition.Text,
                    DateUpdatedSince = DateUpdatedSince.Value?.UtcDateTime,
                    DateUpdatedBefore = DateUpdatedBefore.Value?.UtcDateTime
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                JobID.Value = value.OpportunityIdentifier;
                CandidateContactID.Value = value.UserIdentifier;
                EmployerName.Text = value.EmployerName;
                JobPosition.Text = value.JobTitle;

                DateUpdatedSince.Value = value.DateUpdatedSince;
                DateUpdatedBefore.Value = value.DateUpdatedBefore;
            }
        }

        public override void Clear()
        {
            JobID.Value = null;
            CandidateContactID.Value = null;
            EmployerName.Text = null;
            JobPosition.Text = null;

            DateUpdatedSince.Value = null;
            DateUpdatedBefore.Value = null;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                CandidateContactID.Filter.IsCandidate = true;
        }
    }
}
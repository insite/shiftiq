using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.UI.Portal.Jobs.Employers.Opportunities.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<TOpportunityFilter>
    {
        public override TOpportunityFilter Filter
        {
            get
            {
                var filter = new TOpportunityFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    PositionType = PositionType.Value,
                    JobLocation = JobLocation.Text,
                    JobType = EmploymentType.Value,
                    TemplateIdentifier = StreamIdentifier.ValueAsGuid,
                    JobTitle = JobTitle.Text,
                    CompanyName = EmployerName.Text,

                    IsPublished = true,
                    IsClosed = false,
                    IsArchived = false,

                    OrderBy = "WhenCreated desc"
                };
                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                PositionType.Value = value.PositionType;
                JobLocation.Text = value.JobLocation;
                EmploymentType.Value = value.JobType;
                JobTitle.Text = value.JobTitle;
                EmployerName.Text = value.CompanyName;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            StreamIdentifier.ListFilter.OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier;
            StreamIdentifier.ListFilter.GroupType = "Department";
        }

        public override void Clear()
        {
            EmployerName.Text = null;
            PositionType.Value = null;
            JobLocation.Text = null;
            EmploymentType.Value = null;
            JobTitle.Text = null;
            EmployerName.Text = null;
        }
    }
}
using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Constant;

namespace InSite.UI.Portal.Jobs.Candidates.Opportunities.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<TOpportunityFilter>
    {
        public Guid? DefaultOccupationIdentifier { get; set; }

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

                    OccupationStandardIdentifier = !IsPostBack && DefaultOccupationIdentifier.HasValue ? DefaultOccupationIdentifier : OccupationIdentifier.ValueAsGuid,

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

                OccupationIdentifier.ValueAsGuid = !IsPostBack && DefaultOccupationIdentifier.HasValue ? DefaultOccupationIdentifier : value.OccupationStandardIdentifier;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            OccupationIdentifier.ListFilter.OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier;
            OccupationIdentifier.ListFilter.StandardTypes = new[] { StandardType.Profile };
            OccupationIdentifier.RefreshData();

            var fastDepartment = Guid.Parse("9ffc8191-c34d-4a9f-a834-34e3ae317292");
            var fastCandidate = PersonCriteria.BindFirst(
                x => new { x.OccupationStandardIdentifier },
                new PersonFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    UserIdentifier = User.Identifier
                });

            if (fastCandidate.OccupationStandardIdentifier.HasValue
                && MembershipSearch.Exists(x => x.UserIdentifier == User.Identifier && x.GroupIdentifier == fastDepartment)
                )
            {
                BindOccupation(fastCandidate.OccupationStandardIdentifier);
            }

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

            if (IsPostBack)
                OccupationIdentifier.ClearSelection();
        }

        internal void BindOccupation(Guid? occupation)
        {
            OccupationIdentifier.ValueAsGuid = occupation;
            DefaultOccupationIdentifier = occupation;
        }
    }
}
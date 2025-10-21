using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Jobs.Opportunities.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<TOpportunityFilter>
    {
        protected override string[] DefaultShowColumns => new[] { "Employer Name", "Job Position", "Job Type", "Published", "Closed" };

        public override TOpportunityFilter Filter
        {
            get
            {
                var filter = new TOpportunityFilter
                {
                    OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                    OccupationStandardIdentifier = OccupationStandardIdentifier.ValueAsGuid,

                    CompanyName = EmployerName.Text,
                    PositionType = PositionType.Text,
                    JobLocation = JobLocation.Text,
                    JobType = JobType.Text,
                    IsPublished = IsPublished.ValueAsBoolean,
                    JobTitle = JobTitle.Text,

                    PublishedSince = DatePublishedSince.Value?.UtcDateTime,
                    PublishedBefore = DatePublishedBefore.Value?.UtcDateTime,
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                OccupationStandardIdentifier.ValueAsGuid = value.OccupationStandardIdentifier;

                EmployerName.Text = value.CompanyName;
                PositionType.Text = value.PositionType;
                JobLocation.Text = value.JobLocation;
                JobType.Text = value.JobType;
                IsPublished.ValueAsBoolean = value.IsPublished;
                JobTitle.Text = value.JobTitle;

                DatePublishedSince.Value = value.PublishedSince;
                DatePublishedBefore.Value = value.PublishedBefore;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            OccupationStandardIdentifier.ListFilter.OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier;
            OccupationStandardIdentifier.ListFilter.StandardTypes = new[] { Shift.Constant.StandardType.Profile };
        }

        public override void Clear()
        {
            OccupationStandardIdentifier.ValueAsGuid = null;

            EmployerName.Text = null;
            PositionType.Text = null;
            JobLocation.Text = null;
            JobType.Text = null;
            IsPublished.ClearSelection();
            JobTitle.Text = null;

            DatePublishedSince.Value = null;
            DatePublishedBefore.Value = null;
        }
    }
}
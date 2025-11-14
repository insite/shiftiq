using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Constant;

namespace InSite.UI.Portal.Jobs.Employers.Candidates.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<JobPersonFilter>
    {
        public override JobPersonFilter Filter
        {
            get
            {
                var filter = new JobPersonFilter
                {
                    OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                    IsActivelySeeking = true,
                    IsApproved = true,
                    City = City.Text,
                    WillingToRelocate = WillingToRelocate.ValueAsBoolean,
                    CurrentJobTitle = CurrentJobTitle.Text,
                    AreaOfInterest = OccupationIdentifier.ValueAsGuid,
                    Qualification = Qualification.Text,
                    FullName = FullName.Text,
                };
                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                FullName.Text = value.FullName;
                City.Text = value.City;
                WillingToRelocate.ValueAsBoolean = value.WillingToRelocate;
                CurrentJobTitle.Text = value.CurrentJobTitle;
                OccupationIdentifier.ValueAsGuid = value.AreaOfInterest;
                Qualification.Text = value.Qualification;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            OccupationIdentifier.ListFilter.OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier;
            OccupationIdentifier.ListFilter.StandardTypes = new[] { StandardType.Profile };
            OccupationIdentifier.RefreshData();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        public override void Clear()
        {
            FullName.Text = null;
            City.Text = null;
            WillingToRelocate.ValueAsBoolean = null;
            CurrentJobTitle.Text = null;
            OccupationIdentifier.ValueAsGuid = null;
            Qualification.Text = null;
        }
    }
}
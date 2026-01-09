using System;

using InSite.Application.Events.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Desktops.Custom.SkilledTradesBC.Distributions.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QEventFilter>
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EventBillingType.Settings.CollectionName = CollectionName.Activities_Exams_Billing_Type;
            EventBillingType.Settings.OrganizationIdentifier = Organization.OrganizationIdentifier;

            EventPublicationStatus.Settings.CollectionName = CollectionName.Activities_Exams_Publication_Status;
            EventPublicationStatus.Settings.OrganizationIdentifier = Organization.OrganizationIdentifier;

            EventSchedulingStatus.Settings.CollectionName = CollectionName.Activities_Exams_Scheduling_Status;
            EventSchedulingStatus.Settings.OrganizationIdentifier = Organization.OrganizationIdentifier;
        }

        public override QEventFilter Filter
        {
            get
            {
                var filter = new QEventFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    EventNumber = EventNumber.ValueAsInt,
                    EventScheduledSince = EventScheduledSince.Value,
                    EventScheduledBefore = EventScheduledBefore.Value,
                    DistributionExpectedSince = DistributionExpectedSince.Value,
                    DistributionExpectedBefore = DistributionExpectedBefore.Value,
                    UndistributedExamsInclusion = UndistributedExamsInclusion.Value.ToEnum(InclusionType.Include),
                    ExamType = ExamType.Value,
                    EventFormat = EventExamFormat.Paper.Value,
                    EventTitle = EventTitle.Text,
                    IncludeEventSchedulingStatus = EventSchedulingStatus.Value,
                    EventPublicationStatus = EventPublicationStatus.Value,
                    EventClassCode = EventClassCode.Text,
                    EventBillingType = EventBillingType.Value,
                    Venue = Venue.Text,
                    VenueOffice = VenueOffice.Text
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                EventScheduledSince.Value = value.EventScheduledSince;
                EventScheduledBefore.Value = value.EventScheduledBefore;
                DistributionExpectedSince.Value = value.DistributionExpectedSince;
                DistributionExpectedBefore.Value = value.DistributionExpectedBefore;
                UndistributedExamsInclusion.Value = value.UndistributedExamsInclusion.GetName(InclusionType.Include);
                ExamType.Value = value.ExamType;
                EventFormat.Value = EventExamFormat.Paper.Value;
                EventNumber.ValueAsInt = value.EventNumber;
                EventTitle.Text = value.EventTitle;
                EventBillingType.Value = value.EventBillingType;
                EventSchedulingStatus.Value = value.IncludeEventSchedulingStatus;
                EventPublicationStatus.Value = value.EventPublicationStatus;
                EventClassCode.Text = value.EventClassCode;
                Venue.Text = value.Venue;
                VenueOffice.Text = value.Venue;
            }
        }

        public override void Clear()
        {
            EventScheduledSince.Value = null;
            EventScheduledBefore.Value = null;
            DistributionExpectedSince.Value = null;
            DistributionExpectedBefore.Value = null;
            UndistributedExamsInclusion.ClearSelection();
            ExamType.Value = null;
            EventFormat.Value = EventExamFormat.Paper.Value;
            EventNumber.ValueAsInt = null;
            EventTitle.Text = null;
            EventPublicationStatus.ClearSelection();
            EventSchedulingStatus.ClearSelection();
            EventClassCode.Text = null;
            EventBillingType.ClearSelection();
            Venue.Text = null;
            VenueOffice.Text = null;
        }
    }
}
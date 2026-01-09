using System;
using System.Linq;

using InSite.Application.Contacts.Read;
using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Events.Exams.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QEventFilter>
    {
        private string DefaultExamType => Request.QueryString["type"];

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CustomDateRange.AutoPostBack = true;
            CustomDateRange.ValueChanged += CustomDateRange_ValueChanged;

            EventBillingType.Settings.CollectionName = CollectionName.Activities_Exams_Billing_Type;
            EventBillingType.Settings.OrganizationIdentifier = Organization.OrganizationIdentifier;

            EventRequisitionStatus.Settings.CollectionName = CollectionName.Activities_Exams_Requisition_Status;
            EventRequisitionStatus.Settings.OrganizationIdentifier = Organization.OrganizationIdentifier;

            EventSchedulingStatus.Settings.CollectionName = CollectionName.Activities_Exams_Scheduling_Status;
            EventSchedulingStatus.Settings.OrganizationIdentifier = Organization.OrganizationIdentifier;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var filter = new QGroupSelectorFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                GroupType = GroupTypes.Venue,
                IsEventLocation = true
            };

            var groups = ServiceLocator.GroupSearch.GetSelectorGroups(filter, false);

            var data = groups.Select(x => new
            {
                Value = x.GroupIdentifier,
                Text = x.GroupName
            })
                .ToList();

            VenueLocationGroup.LoadItems(data, "Value", "Text");
        }

        private void CustomDateRange_ValueChanged(object sender, EventArgs e)
        {
            OnCustomDateRangeChanged(CustomDateRange.Value, EventScheduledSince, EventScheduledBefore);
        }

        private void OnCustomDateRangeChanged(string value, DateTimeOffsetSelector selectorSince, DateTimeOffsetSelector selectorBefore)
        {
            var hasValue = value.IsNotEmpty();

            if (hasValue && value.StartsWith("Next"))
            {
                var start = DateTimeOffset.UtcNow.AddDays(1);

                if (value == "Next 30 Days")
                {
                    selectorSince.Value = start;
                    selectorBefore.Value = start.AddDays(30);
                }
                else if (value == "Next 60 Days")
                {
                    selectorSince.Value = start;
                    selectorBefore.Value = start.AddDays(60);
                }
                else if (value == "Next 90 Days")
                {
                    selectorSince.Value = start;
                    selectorBefore.Value = start.AddDays(90);
                }
            }

            selectorSince.Enabled = selectorBefore.Enabled = !hasValue;
        }

        public override QEventFilter Filter
        {
            get
            {
                var filter = new QEventFilter
                {
                    EventType = "Exam",
                    OrganizationIdentifier = Organization.Identifier,

                    EventScheduledSince = EventScheduledSince.Value,
                    EventScheduledBefore = EventScheduledBefore.Value,
                    ExamType = !IsPostBack && DefaultExamType.IsNotEmpty() ? DefaultExamType : ExamType.Value,
                    EventFormat = EventFormat.Value,
                    EventTitle = EventTitle.Text,
                    EventNumber = EventNumber.ValueAsInt,
                    EventMaterialTrackingStatus = EventMaterialTrackingStatus.Value,
                    IncludeEventSchedulingStatus = EventSchedulingStatus.Value,
                    EventRequisitionStatus = EventRequisitionStatus.Value,
                    EventClassCode = EventClassCode.Text,
                    EventBillingType = EventBillingType.Value,
                    VenueLocationIdentifier = VenueLocationGroup.ValuesAsGuidArray,
                    VenueOffice = VenueOffice.Text,
                    ExamFormName = ExamFormName.Text
                };

                var bulkNumbers = Request.QueryString["bulk"];
                if (bulkNumbers.IsNotEmpty())
                    filter.EventNumbers = EventHelper.IntArrayFromBase64(bulkNumbers);

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                EventBillingType.Value = value.EventBillingType;
                EventClassCode.Text = value.EventClassCode;
                EventFormat.Value = value.EventFormat;
                EventMaterialTrackingStatus.Value = value.EventMaterialTrackingStatus;
                EventNumber.ValueAsInt = value.EventNumber;
                EventSchedulingStatus.Value = value.IncludeEventSchedulingStatus;
                EventScheduledSince.Value = value.EventScheduledSince;
                EventScheduledBefore.Value = value.EventScheduledBefore;
                EventTitle.Text = value.EventTitle;
                VenueLocationGroup.ValuesAsGuid = value.VenueLocationIdentifier;
                VenueOffice.Text = value.Venue;
                ExamFormName.Text = value.ExamFormName;

                if (!IsPostBack && DefaultExamType.IsNotEmpty())
                    SetExamType(DefaultExamType);
                else
                    ExamType.Value = value.ExamType;
            }
        }

        public override void Clear()
        {
            CustomDateRange.ClearSelection();
            EventScheduledSince.Value = null;
            EventScheduledBefore.Value = null;
            EventFormat.ClearSelection();
            EventNumber.ValueAsInt = null;
            EventTitle.Text = null;
            EventSchedulingStatus.ClearSelection();
            EventMaterialTrackingStatus.ClearSelection();
            EventClassCode.Text = null;
            EventBillingType.ClearSelection();
            EventRequisitionStatus.ClearSelection();
            VenueLocationGroup.ClearSelection();
            VenueOffice.Text = null;
            ExamFormName.Text = null;

            if (!SetExamType(DefaultExamType))
                ExamType.Value = null;
        }

        private bool SetExamType(string type)
        {
            ExamType.EnsureDataBound();

            var option = ExamType.FindOptionByValue(type, true);
            if (option == null)
                return false;

            option.Selected = true;

            return true;
        }
    }
}
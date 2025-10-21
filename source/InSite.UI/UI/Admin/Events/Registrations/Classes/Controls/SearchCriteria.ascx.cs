using System;

using InSite.Application.Registrations.Read;
using InSite.Common;
using InSite.Common.Web.UI;

using Shift.Constant;

namespace InSite.Admin.Events.Registrations.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QRegistrationFilter>
    {

        public override QRegistrationFilter Filter
        {
            get
            {
                var filter = new QRegistrationFilter
                {
                    OrganizationIdentifier = Organization.Identifier,

                    EventTitle = EventTitle.Text,
                    CandidateName = CandidateName.Text,
                    CandidateCode = CandidateCode.Text,
                    CandidateEmailEnabled = CandidateEmailEnabled.ValueAsBoolean,
                    ApprovalStatus = ApprovalStatus.Value,
                    AttendanceStatus = AttendanceStatus.Value,
                    EventScheduledSince = EventScheduledSince.Value,
                    EventScheduledBefore = EventScheduledBefore.Value,
                    RegistrationRequestedSince = RegistrationRequestedSince.Value?.UtcDateTime,
                    RegistrationRequestedBefore = RegistrationRequestedBefore.Value?.UtcDateTime,
                    SeatAvailable = SeatAvailable.ValueAsBoolean,
                    RegistrationRequestedByName = RegistrationRequestedByName.Text,
                    RegistrationEmployerName = RegistrationEmployerName.Text,
                    RegistrationEmployerStatus = RegistrationEmployerStatus.Text,
                    RegistrationEmployerRegion = RegistrationEmployerRegion.Value,
                    RegistrationComment = RegistrationComment.Text,
                    VenueName = VenueName.Text,
                    IncludeInT2202 = IncludeInT2202.ValueAsBoolean,
                    PaymentStatus = PaymentStatus.Value,
                    EventType = "Class",
                    BillingCode = BillingCode.Text
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                EventTitle.Text = value.EventTitle;
                CandidateName.Text = value.CandidateName;
                CandidateCode.Text = value.CandidateCode;
                CandidateEmailEnabled.ValueAsBoolean = value.CandidateEmailEnabled;
                ApprovalStatus.Value = value.ApprovalStatus;
                AttendanceStatus.Value = value.AttendanceStatus;
                EventScheduledSince.Value = value.EventScheduledSince;
                EventScheduledBefore.Value = value.EventScheduledBefore;
                RegistrationRequestedSince.Value = value.RegistrationRequestedSince;
                RegistrationRequestedBefore.Value = value.RegistrationRequestedBefore;
                SeatAvailable.ValueAsBoolean = value.SeatAvailable;
                RegistrationRequestedByName.Text = value.RegistrationRequestedByName;
                RegistrationEmployerName.Text = value.RegistrationEmployerName;
                RegistrationEmployerStatus.Text = value.RegistrationEmployerStatus;
                RegistrationEmployerRegion.Value = value.RegistrationEmployerRegion;
                RegistrationComment.Text = value.RegistrationComment;
                VenueName.Text = value.VenueName;
                IncludeInT2202.ValueAsBoolean = value.IncludeInT2202;
                PaymentStatus.Value = value.PaymentStatus;
                BillingCode.Text = value.BillingCode;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RegistrationEmployerRegion.Settings.CollectionName = CollectionName.Contacts_People_Location_Region;
            RegistrationEmployerRegion.Settings.OrganizationIdentifier = Organization.Key;

            CandidateCode.EmptyMessage = GetEmptyMessage("Person Code");
        }

        public override void Clear()
        {
            EventTitle.Text = null;
            CandidateCode.Text = null;
            CandidateName.Text = null;
            CandidateEmailEnabled.ClearSelection();
            ApprovalStatus.ClearSelection();
            AttendanceStatus.ClearSelection();
            EventScheduledSince.Value = null;
            EventScheduledBefore.Value = null;
            RegistrationRequestedSince.Value = null;
            RegistrationRequestedBefore.Value = null;
            SeatAvailable.ClearSelection();
            RegistrationRequestedByName.Text = null;
            RegistrationEmployerName.Text = null;
            RegistrationEmployerStatus.Text = null;
            RegistrationEmployerRegion.ClearSelection();
            RegistrationComment.Text = null;
            VenueName.Text = null;
            IncludeInT2202.ClearSelection();
            PaymentStatus.ClearSelection();
            BillingCode.Text = null;
        }

        protected static string GetEmptyMessage(string text) => LabelHelper.GetLabelContentText(text);
    }
}
using System;
using System.Linq;

using InSite.Application.Contacts.Read;
using InSite.Application.Registrations.Read;
using InSite.Common;
using InSite.Common.Web.UI;

using Shift.Constant;

namespace InSite.Admin.Events.Registrations.Exams.Controls
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
                    CandidateType = CandidateType.Text,
                    ApprovalStatus = ApprovalStatus.Value,
                    AttendanceStatus = AttendanceStatus.Value,
                    EventScheduledSince = EventScheduledSince.Value,
                    EventScheduledBefore = EventScheduledBefore.Value,
                    RegistrationRequestedSince = RegistrationRequestedSince.Value,
                    RegistrationRequestedBefore = RegistrationRequestedBefore.Value,
                    RegistrationComment = RegistrationComment.Text,
                    VenueLocationIdentifier = VenueLocationGroup.ValuesAsGuidArray,
                    EventType = "Exam",
                    ExamFormName = ExamFormName.Text
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                EventTitle.Text = value.EventTitle;
                CandidateName.Text = value.CandidateName;
                CandidateCode.Text = value.CandidateCode;
                CandidateType.Text = value.CandidateType;
                ApprovalStatus.Value = value.ApprovalStatus;
                AttendanceStatus.Value = value.AttendanceStatus;
                EventScheduledSince.Value = value.EventScheduledSince;
                EventScheduledBefore.Value = value.EventScheduledBefore;
                RegistrationRequestedSince.Value = value.RegistrationRequestedSince;
                RegistrationRequestedBefore.Value = value.RegistrationRequestedBefore;
                RegistrationComment.Text = value.RegistrationComment;
                VenueLocationGroup.ValuesAsGuid = value.VenueLocationIdentifier;
                ExamFormName.Text = value.ExamFormName;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CandidateCode.EmptyMessage = GetEmptyMessage("Person Code");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var filter = new QGroupSelectorFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    GroupType = GroupTypes.Venue,
                    IsRegistrationEventLocation = true
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
        }

        public override void Clear()
        {
            EventTitle.Text = null;
            CandidateCode.Text = null;
            CandidateName.Text = null;
            ApprovalStatus.Value = null;
            AttendanceStatus.Value = null;
            EventScheduledSince.Value = null;
            EventScheduledBefore.Value = null;
            RegistrationRequestedSince.Value = null;
            RegistrationRequestedBefore.Value = null;
            RegistrationComment.Text = null;
            ExamFormName.Text = null;
            VenueLocationGroup.ClearSelection();
        }

        protected static string GetEmptyMessage(string text) => LabelHelper.GetLabelContentText(text);
    }
}
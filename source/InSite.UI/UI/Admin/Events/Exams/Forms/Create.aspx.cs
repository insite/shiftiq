using System;
using System.Linq;
using System.Web;

using InSite.Application.Contacts.Read;
using InSite.Application.Events.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Events.Exams.Forms
{
    public partial class Create : AdminBasePage
    {
        private static readonly object Lock = new object();

        private string ReturnUrl
        {
            get => ViewState[nameof(ReturnUrl)] as string;
            set => ViewState[nameof(ReturnUrl)] = value;
        }

        private string DefaultExamType => Request.QueryString["type"];

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            BillingCode.Settings.CollectionName = CollectionName.Activities_Exams_Billing_Type;
            BillingCode.Settings.OrganizationIdentifier = Organization.OrganizationIdentifier;

            CreationType.AutoPostBack = true;
            CreationType.ValueChanged += CreationType_ValueChanged;

            ExamType.AutoPostBack = true;
            ExamType.ValueChanged += ExamType_ValueChanged;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write))
                HttpResponseHelper.Redirect("/ui/admin/events/exams/search");

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            if (Request.UrlReferrer != null)
                ReturnUrl = Request.UrlReferrer.ToString();

            SetDefaultInputValues();
        }

        private void SetDefaultInputValues()
        {
            if (!string.IsNullOrEmpty(DefaultExamType))
            {
                ExamType.EnsureDataBound();

                var item = ExamType.FindOptionByValue(DefaultExamType, true);
                if (item != null)
                    item.Selected = true;
            }

            ExamTypeChanged(ExamType.Value);

            CreationType.EnsureDataBound();
            CreationType.SetVisibleOptions(CreationTypeEnum.One);

            CancelButton.NavigateUrl = ReturnUrl.IsNotEmpty()
                ? ReturnUrl
                : "/ui/admin/events/exams/search?type=" + HttpUtility.UrlEncode(DefaultExamType);
        }

        private void CreationType_ValueChanged(object sender, EventArgs e)
        {
            OnCreationTypeSelected();
        }

        private void OnCreationTypeSelected()
        {
            NewSection.Visible = CreationType.ValueAsEnum == CreationTypeEnum.One;
        }

        private void ExamType_ValueChanged(object sender, EventArgs e)
        {
            ExamTypeChanged(ExamType.Value);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var id = Save();

            if (id.HasValue)
                HttpResponseHelper.Redirect($"/ui/admin/events/exams/outline?event={id}");
        }

        private Guid? Save()
        {
            var eventTime = EventScheduled.Value ?? DateTimeOffset.UtcNow;

            if (eventTime < DateTimeOffset.Now)
            {
                EditorStatus.Indicator = AlertType.Error;
                EditorStatus.Text = "You cannot backdate an exam event. Please enter a future date and time in the Scheduled Date field.";
                return null;
            }

            var identifier = UniqueIdentifier.Create();

            lock (Lock)
            {
                var number = Sequence.Increment(Organization.OrganizationIdentifier, SequenceType.Event);

                var command = new ScheduleExam(identifier, Organization.OrganizationIdentifier)
                {
                    ExamType = ExamType.Value,
                    ExamDuration = EventHelper.GetDuration(BillingCode.GetSelectedOption().Text),

                    EventStartTime = eventTime,
                    EventFormat = EventFormat.Value,
                    EventNumber = number,
                    EventClassCode = ClassCode.Text,
                    EventBillingCode = BillingCode.Value,
                    EventSource = "Shift UI",

                    CapacityMaximum = !ExamType.Value.StartsWith("Individual") && MaximumParticipantCount.ValueAsInt.HasValue ? MaximumParticipantCount.ValueAsInt.Value : 1,
                };

                if (VenueLocationIdentifier.HasValue)
                {
                    var venue = ServiceLocator.GroupSearch.GetGroup(VenueLocationIdentifier.Value.Value);
                    command.VenueIdentifier = venue.GroupIdentifier;
                    command.VenueRoom = LocationRoom.Text;
                    command.EventTitle = EventHelper.GetTitle(Guid.Empty, venue.GroupName, ServiceLocator.EventSearch);
                }

                ServiceLocator.SendCommand(command);

                return command.AggregateIdentifier;
            }
        }

        private void ExamTypeChanged(string examType)
        {
            var isClass = examType == EventExamType.Class.Value;
            var isArc = examType == EventExamType.Arc.Value;

            EventTypeHelp.InnerHtml = ExamTypeComboBox.GetDescription(examType);
            ClassCodeField.Visible = isClass;
            LocationRoomField.Visible = isClass;

            var parentGroupIdentifier = GetVenueParentByExamType(examType);

            VenueLocationIdentifier.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
            VenueLocationIdentifier.Filter.GroupType = GroupTypes.Venue;
            VenueLocationIdentifier.Filter.AncestorGroupIdentifier = parentGroupIdentifier;
            VenueLocationIdentifier.Filter.ParentGroupIdentifier = parentGroupIdentifier;
            VenueLocationIdentifier.Filter.AncestorName = isArc ? "ARC" : null;
        }

        internal static Guid? GetVenueParentByExamType(string examType)
        {
            // Resolve the activity type to a group name.
            if (examType == EventExamType.IndividualA.Value)
                examType = "Individual (Accommodated Only)";
            else if (examType == EventExamType.IndividualN.Value)
                examType = "Individual";

            // Find the group that has the same name as the activity type.
            var parent = ServiceLocator.GroupSearch
                .GetGroups(new QGroupFilter { GroupName = examType, OrganizationIdentifier = Organization.Identifier })
                .FirstOrDefault();

            return parent?.GroupIdentifier;
        }
    }
}

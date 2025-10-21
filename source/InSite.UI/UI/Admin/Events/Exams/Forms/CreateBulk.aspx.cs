using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Events.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

using CustomValidator = System.Web.UI.WebControls.CustomValidator;

namespace InSite.Admin.Events.Exams.Forms
{
    public partial class CreateBulk : AdminBasePage
    {
        private static readonly object Lock = new object();

        private class EventInfo
        {
            public string ExamType { get; set; }
            public string EventFormat { get; set; }
            public string BillingCode { get; set; }
            public DateTimeOffset? EventScheduled { get; set; }
            public int? MaximumParticipantCount { get; set; }
            public Guid? VenueGroupIdentifier { get; set; }
        }

        private class EventControls
        {
            public CustomValidator RowValidator { get; private set; }
            public ExamTypeComboBox ExamType { get; private set; }
            public EventFormatComboBox EventFormat { get; private set; }
            public ItemNameComboBox BillingCode { get; private set; }
            public DateTimeOffsetSelector EventScheduled { get; private set; }
            public NumericBox MaximumParticipantCount { get; private set; }
            public FindGroup VenueGroupIdentifier { get; private set; }

            private EventControls()
            {

            }

            public static EventControls Create(RepeaterItem item)
            {
                return new EventControls
                {
                    RowValidator = (CustomValidator)item.FindControl("RowValidator"),
                    ExamType = (ExamTypeComboBox)item.FindControl("ExamType"),
                    EventFormat = (EventFormatComboBox)item.FindControl("EventFormat"),
                    BillingCode = (ItemNameComboBox)item.FindControl("BillingCode"),
                    EventScheduled = (DateTimeOffsetSelector)item.FindControl("EventScheduled"),
                    MaximumParticipantCount = (NumericBox)item.FindControl("MaximumParticipantCount"),
                    VenueGroupIdentifier = (FindGroup)item.FindControl("VenueGroupIdentifier"),
                };
            }

            public void SetInputValues(EventInfo info)
            {
                ExamType.Value = info.ExamType;
                EventFormat.Value = info.EventFormat;
                BillingCode.Value = info.BillingCode;
                EventScheduled.Value = info.EventScheduled;
                MaximumParticipantCount.ValueAsInt = info.MaximumParticipantCount;
                VenueGroupIdentifier.Value = info.VenueGroupIdentifier;
            }

            public EventInfo GetInputValues()
            {
                return new EventInfo
                {
                    ExamType = ExamType.Value,
                    EventFormat = EventFormat.Value,
                    BillingCode = BillingCode.Value,
                    EventScheduled = EventScheduled.Value,
                    MaximumParticipantCount = MaximumParticipantCount.ValueAsInt,
                    VenueGroupIdentifier = VenueGroupIdentifier.Value
                };
            }

            public bool Validate(out string error)
            {
                var sb = new StringBuilder();

                if (string.IsNullOrEmpty(ExamType.Value))
                    RequiredField("Exam Type");

                if (string.IsNullOrEmpty(EventFormat.Value))
                    RequiredField("Exam Format");

                if (string.IsNullOrEmpty(BillingCode.Value))
                    RequiredField("Billing Code");

                if (!EventScheduled.Value.HasValue)
                    RequiredField("Start Date/Time");
                else if (EventScheduled.Value < DateTimeOffset.Now)
                    sb.Append("<li>You cannot backdate an exam event. Please enter a future date and time in the Scheduled Date field.</li>");

                if (!VenueGroupIdentifier.HasValue)
                    RequiredField("Venue");

                var isValid = sb.Length == 0;

                error = isValid ? null : sb.Insert(0, "<ul>").Append("</ul>").ToString();

                return isValid;

                void RequiredField(string fieldName)
                {
                    sb.AppendFormat("<li><strong>{0}</strong> is required field</li>", fieldName);
                }
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EventRepeater.ItemCreated += EventRepeater_ItemCreated;
            EventRepeater.ItemDataBound += EventRepeater_ItemDataBound;
            EventRepeater.ItemCommand += EventRepeater_ItemCommand;

            AddRowButton.Click += AddRowButton_Click;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write))
                HttpResponseHelper.Redirect("/ui/admin/events/exams/search");

            if (!IsPostBack)
                PageHelper.AutoBindHeader(this);
        }

        private void EventRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var controls = EventControls.Create(e.Item);

            controls.RowValidator.ServerValidate += RowValidator_ServerValidate;

            controls.ExamType.AutoPostBack = true;
            controls.ExamType.ValueChanged += ExamType_ValueChanged;
        }

        private void RowValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var validator = (CustomValidator)source;
            var repeaterItem = (RepeaterItem)validator.NamingContainer;
            var controls = EventControls.Create(repeaterItem);

            args.IsValid = controls.Validate(out var message);

            if (!args.IsValid)
                validator.ErrorMessage = $"Row #{repeaterItem.ItemIndex + 1:n0} failed validation: " + message;
        }

        private void EventRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = (EventInfo)e.Item.DataItem;
            var controls = EventControls.Create(e.Item);

            controls.VenueGroupIdentifier.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
            controls.VenueGroupIdentifier.Filter.GroupType = GroupTypes.Venue;

            controls.BillingCode.Settings.CollectionName = CollectionName.Activities_Exams_Billing_Type;
            controls.BillingCode.Settings.OrganizationIdentifier = Organization.OrganizationIdentifier;
            controls.BillingCode.RefreshData();

            controls.SetInputValues(dataItem);
        }

        private void EventRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var items = GetItems();

                items.RemoveAt(e.Item.ItemIndex);

                SetItems(items);
            }
        }

        private void AddRowButton_Click(object sender, EventArgs e)
        {
            var items = GetItems();

            items.Add(new EventInfo());

            SetItems(items);
        }

        private void ExamType_ValueChanged(object sender, EventArgs e)
        {
            var repeaterItem = (RepeaterItem)((System.Web.UI.Control)sender).NamingContainer;
            var controls = EventControls.Create(repeaterItem);
            var examType = controls.ExamType.Value;

            var parentGroupIdentifier = Create.GetVenueParentByExamType(examType);

            controls.VenueGroupIdentifier.Filter.ParentGroupIdentifier = parentGroupIdentifier;
            controls.VenueGroupIdentifier.Filter.AncestorGroupIdentifier = parentGroupIdentifier;
            controls.VenueGroupIdentifier.Filter.AncestorName = examType == EventExamType.Arc.Value ? "ARC" : null;
            controls.VenueGroupIdentifier.Value = null;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var items = GetItems();
            if (items.Count == 0)
            {
                ScreenStatus.AddMessage(AlertType.Error, "No items to create.");
                return;
            }

            var numbers = new List<int>();

            lock (Lock)
            {
                foreach (var item in items)
                {
                    var identifier = UniqueIdentifier.Create();
                    var number = Sequence.Increment(Organization.OrganizationIdentifier, SequenceType.Event);

                    var command = new ScheduleExam(identifier, Organization.OrganizationIdentifier)
                    {
                        ExamType = item.ExamType,
                        ExamDuration = EventHelper.GetDuration(item.BillingCode),

                        EventStartTime = item.EventScheduled.Value,
                        EventFormat = item.EventFormat,
                        EventNumber = number,
                        EventBillingCode = item.BillingCode,
                        EventSource = "Shift UI",

                        CapacityMaximum = !item.ExamType.StartsWith("Individual") ? item.MaximumParticipantCount : 1,
                    };

                    if (item.VenueGroupIdentifier.HasValue)
                    {
                        var venue = ServiceLocator.GroupSearch.GetGroup(item.VenueGroupIdentifier.Value);
                        command.VenueIdentifier = venue.GroupIdentifier;
                        command.EventTitle = EventHelper.GetTitle(Guid.Empty, venue.GroupName, ServiceLocator.EventSearch);
                    }

                    ServiceLocator.SendCommand(command);

                    numbers.Add(number);
                }
            }

            HttpResponseHelper.Redirect("/ui/admin/events/exams/search?bulk=" + EventHelper.IntArrayToBase64(numbers));
        }

        private List<EventInfo> GetItems()
        {
            var result = new List<EventInfo>();

            foreach (RepeaterItem ri in EventRepeater.Items)
            {
                var controls = EventControls.Create(ri);
                var info = controls.GetInputValues();

                result.Add(info);
            }

            return result;
        }

        private void SetItems(IEnumerable<EventInfo> items)
        {
            EventRepeater.DataSource = items;
            EventRepeater.DataBind();

            SaveButton.Visible = items.Any();
        }

    }
}

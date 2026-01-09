using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

using Shift.Common.Timeline.Commands;

using InSite.Admin.Events.Classes.Controls;
using InSite.Application.Events.Write;
using InSite.Application.Gradebooks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Admin.Events.Classes.Controls;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

using AggregateOutline = InSite.Admin.Logs.Aggregates.Outline;

namespace InSite.Admin.Events.Classes.Forms
{
    public partial class Outline : AdminBasePage
    {
        private const string SearchUrl = "/ui/admin/events/classes/search";

        private Guid? EventID => Guid.TryParse(Request["event"], out var eventID) ? eventID : (Guid?)null;

        private string Panel => Request["panel"];

        private string Tab => Request["tab"];

        private string SubTab => Request["subtab"];

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AddGradebookIdentifier.Click += AddGradebookIdentifier_Click;
            FindGradebook.Click += FindGradebook_Click;

            MandatorySurveyFormIdentifier.AutoPostBack = true;
            MandatorySurveyFormIdentifier.ValueChanged += MandatorySurveyFormIdentifier_ValueChanged;

            ClassTabContent.ControlAdded += ClassTabContent_ControlAdded;
        }

        private void ClassTabContent_ControlAdded(object sender, EventArgs e)
        {
            var classSetup = Organization.Toolkits.Events.AllowClassRegistrationFields
                ? ((ClassTabNav)ClassTabContent.GetControl()).ClassSetup
                : (ClassSetup)ClassTabContent.GetControl();

            classSetup.FormsShown += (a, b) => LoadRegistrations(true);
            classSetup.FormsHidden += (a, b) => LoadRegistrations(false);
        }

        private void MandatorySurveyFormIdentifier_ValueChanged(object sender, FindEntityValueChangedEventArgs e)
        {
            ServiceLocator.SendCommand(new ModifyMandatorySurvey(EventID.Value, e.NewValue));
        }

        private void FindGradebook_Click(object sender, EventArgs e)
        {
            FindGradebookIdentifier.Visible = true;
            AddGradebookIdentifier.Visible = true;
        }

        private void AddGradebookIdentifier_Click(object sender, EventArgs e)
        {
            if (!EventID.HasValue || !FindGradebookIdentifier.ValueAsGuid.HasValue)
                return;

            var gradebookId = FindGradebookIdentifier.ValueAsGuid.Value;
            var gradebook = ServiceLocator.RecordSearch.GetGradebook(gradebookId);

            var commands = new List<Command>();

            if (gradebook.IsLocked)
                commands.Add(new UnlockGradebook(gradebookId));

            commands.Add(new AddGradebookEvent(gradebookId, EventID.Value, false));

            if (gradebook.IsLocked)
                commands.Add(new LockGradebook(gradebookId));

            ServiceLocator.SendCommands(commands);

            GradebookGrid.LoadData(EventID.Value);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            LoadData();

            var panel = Panel;
            if (panel == "content")
            {
                ContentTab.IsSelected = true;

                var tab = Tab;
                if (tab == "Title")
                    ContentTitleTab.IsSelected = true;
                else if (tab == "Summary")
                    ContentSummaryTab.IsSelected = true;
                else if (tab == "Description")
                    ContentDescriptionTab.IsSelected = true;
                else if (tab == "Materials")
                    ContentMaterialsTab.IsSelected = true;
                else if (tab == "ClassLink")
                    ContentClassLinkTab.IsSelected = true;
                else if (tab == "Instructions")
                {
                    ContentInstructionsTab.IsSelected = true;
                    SelectSubTab(ContentInstructionsTab, "ContentInstructions");
                }
                else if (tab == "Form")
                    ContentSurveyTab.IsSelected = true;
            }
            else if (panel == "seats")
                SeatTab.IsSelected = true;
            else if (panel == "registrations")
                RegistrationTab.IsSelected = true;
            else if (panel == "gradebooks")
                GradebookTab.IsSelected = true;
            else if (panel == "class")
                ClassTab.IsSelected = true;
            else if (panel == "privacy")
                PrivacyTab.IsSelected = true;
            else if (panel == "notifications")
                NotificationTab.IsSelected = true;
        }

        private void SelectSubTab(NavItem navItem, string prefix)
        {
            if (!SubTab.HasValue())
                return;

            var nav = GetBootstrapNav(navItem.Controls);
            if (nav == null)
                return;

            foreach (var subControl in nav.Controls)
            {
                var tab = subControl as NavItem;

                if (tab != null && tab.ID == $"{prefix}{HttpUtility.UrlDecode(SubTab).Replace(" ", "")}Tab")
                {
                    tab.IsSelected = true;
                    break;
                }
            }
        }

        private Nav GetBootstrapNav(ControlCollection controls)
        {
            Nav nav = null;

            foreach (var control in controls)
            {
                nav = control as Nav;

                if (nav != null) return nav;
            }

            return nav;
        }

        private void LoadData()
        {
            if (EventID == null)
            {
                HttpResponseHelper.Redirect(SearchUrl);
                return;
            }

            var ev = ServiceLocator.EventSearch.GetEvent(EventID.Value, x => x.VenueLocation, x => x.GradebookEvents.Select(y => y.Gradebook));
            if (ev == null || ev.OrganizationIdentifier != Organization.OrganizationIdentifier)
            {
                HttpResponseHelper.Redirect(SearchUrl);
                return;
            }

            var isPublished = string.Equals(ev.EventPublicationStatus, PublicationStatus.Published.GetDescription(), StringComparison.OrdinalIgnoreCase);
            var isCancelled = string.Equals(ev.EventSchedulingStatus, "Cancelled", StringComparison.OrdinalIgnoreCase);

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{ev.EventTitle} <span class='form-text'>scheduled {ev.EventScheduledStart.FormatDateOnly(User.TimeZone)}</span>");

            AddSeat.NavigateUrl = $"/ui/admin/events/seats/add?event={EventID}";
            AddGradebook.NavigateUrl = $"/ui/admin/records/gradebooks/open?event={EventID}";
            FindGradebookIdentifier.ListFilter.OrganizationIdentifier = Organization.OrganizationIdentifier;

            DownloadJsonLink.NavigateUrl = $"/ui/admin/events/classes/download?event={EventID}";

            if (Panel == "registrations")
                RegistrationTab.IsSelected = true;
            else if (Panel == "comment")
                CommentTab.IsSelected = true;

            bool showForms;

            if (Organization.Toolkits.Events.AllowClassRegistrationFields)
            {
                var classTabNav = (ClassTabNav)ClassTabContent.LoadControl("~/UI/Admin/Events/Classes/Controls/ClassTabNav.ascx");
                showForms = classTabNav.ClassSetup.LoadData(ev, isPublished, CanEdit);
                classTabNav.ClassSettings.LoadData(ev);
            }
            else
            {
                var classSetup = (ClassSetup)ClassTabContent.LoadControl("~/UI/Admin/Events/Classes/Controls/ClassSetup.ascx");
                showForms = classSetup.LoadData(ev, isPublished, CanEdit);
            }

            var content = ContentEventClass.Deserialize(ev.Content);

            if (string.IsNullOrEmpty(content.Title.Default))
                content.Title.Default = ev.EventTitle;

            ContentTitle.LoadData(content.Title);
            ContentSummary.LoadData(content.Summary);
            ContentDescription.LoadData(content.Description);
            ContentMaterials.LoadData(content.MaterialsForParticipation);
            ContentContact.LoadData(content.Get(EventInstructionType.Contact.GetName()));
            ContentAccommodation.LoadData(content.Get(EventInstructionType.Accommodation.GetName()));
            ContentAdditional.LoadData(content.Get(EventInstructionType.Additional.GetName()));
            ContentCancellation.LoadData(content.Get(EventInstructionType.Cancellation.GetName()));
            ContentCompletion.LoadData(content.Get(EventInstructionType.Completion.GetName()));
            ContentClassLink.LoadData(content.ClassLink);

            MandatorySurveyFormIdentifier.Value = ev.MandatorySurveyFormIdentifier;

            LoadRegistrations(showForms);

            SeatsGrid.LoadData(ev.EventIdentifier, CanEdit);
            GradebookGrid.LoadData(ev.EventIdentifier);

            CommentTabControl.IsReadOnly = !CanEdit;
            CommentTabControl.LoadData(ev.EventIdentifier, EventType.Class);

            PrivacyTabControl.BindControls(ev.EventIdentifier, CanEdit);

            NotificationTabControl.LoadData(ev, CanEdit);

            PreviewLink.NavigateUrl = $"/ui/portal/events/classes/outline?event={EventID}";

            PublishLink.NavigateUrl = $"/ui/admin/events/classes/publish?event={EventID}";
            PublishLink.Visible = CanEdit
                && !isPublished
                && !isCancelled;

            UnpublishLink.NavigateUrl = $"/ui/admin/events/classes/publish?event={EventID}";
            UnpublishLink.Visible = CanEdit && isPublished;

            CancelLink.NavigateUrl = $"/ui/admin/events/classes/cancel?event={EventID}";
            CancelLink.Visible = CanEdit && !isCancelled;

            DeleteLink.NavigateUrl = $"/ui/admin/events/classes/delete?event={EventID}";

            CopyLink.NavigateUrl = $"/ui/admin/events/classes/create?action=duplicate&event={EventID}";

            EditContentTitleLink.NavigateUrl = $"/ui/admin/events/classes/describe?event={EventID}&tab=Title";
            EditContentSummaryLink.NavigateUrl = $"/ui/admin/events/classes/describe?event={EventID}&tab=Summary";
            EditContentDescriptionLink.NavigateUrl = $"/ui/admin/events/classes/describe?event={EventID}&tab=Description";
            EditContentMaterialsLink.NavigateUrl = $"/ui/admin/events/classes/describe?event={EventID}&tab=Materials";
            EditContentContactLink.NavigateUrl = $"/ui/admin/events/classes/describe?event={EventID}&tab=Instructions&subtab=Contact";
            EditContentAccommodationLink.NavigateUrl = $"/ui/admin/events/classes/describe?event={EventID}&tab=Instructions&subtab=Accommodation";
            EditContentAdditionalLink.NavigateUrl = $"/ui/admin/events/classes/describe?event={EventID}&tab=Instructions&subtab=Additional";
            EditContentCancellationLink.NavigateUrl = $"/ui/admin/events/classes/describe?event={EventID}&tab=Instructions&subtab=Cancellation";
            EditContentCompletionLink.NavigateUrl = $"/ui/admin/events/classes/describe?event={EventID}&tab=Instructions&subtab=Completion";
            EditContentClassLinkLink.NavigateUrl = $"/ui/admin/events/classes/describe?event={EventID}&tab=ClassLink";

            ViewHistoryLink.NavigateUrl = AggregateOutline.GetUrl(EventID.Value, $"/ui/admin/events/classes/outline?event={EventID}");

            AddSeat.Visible = CanEdit;
            DeleteLink.Visible = CanDelete;
            CopyLink.Visible = CanEdit;
            EditContentTitleLink.Visible = CanEdit;
            EditContentSummaryLink.Visible = CanEdit;
            EditContentDescriptionLink.Visible = CanEdit;
            EditContentMaterialsLink.Visible = CanEdit;
            EditContentContactLink.Visible = CanEdit;
            EditContentAccommodationLink.Visible = CanEdit;
            EditContentAdditionalLink.Visible = CanEdit;
            EditContentCancellationLink.Visible = CanEdit;
            EditContentCompletionLink.Visible = CanEdit;
            EditContentClassLinkLink.Visible = CanEdit;
            NewClassLink.Visible = CanEdit;

            Separator1.Visible = CanEdit;
            Separator2.Visible = CanEdit;
        }

        private void LoadRegistrations(bool showForms)
        {
            Registrations.LoadData(EventID.Value, CanEdit, showForms, "event&panel=registrations");
        }
    }
}
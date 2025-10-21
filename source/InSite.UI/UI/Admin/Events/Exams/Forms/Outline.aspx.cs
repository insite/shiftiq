using System;
using System.Web;
using System.Web.UI;

using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Events;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Events.Exams.Forms
{
    public partial class Outline : AdminBasePage
    {
        private const string SearchUrl = "/ui/admin/events/exams/search";

        private Guid EventIdentifier => Guid.TryParse(Request.QueryString["event"], out var id) ? id : Guid.Empty;

        private string Panel => Request["panel"];

        private string Tab => Request["tab"];

        private string SubTab => Request["subtab"];

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Detail.ExamFormsChanged += Detail_RelationshipsModified;
            Detail.Published += Detail_Published;

            CandidatePanel.ExamAssigned += CandidatePanel_ExamAssigned;
            CandidatePanel.Refreshed += CandidatePanel_Refreshed;

            AttendeePanel.Refreshed += AttendeePanel_Refreshed;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            Open();


            if (Panel == "content")
            {
                ContentSection.IsSelected = true;
                if (Tab == "Instructions")
                    SelectSubTab(InstructionsTab);
            }

            SetActiveSection();
        }

        private void SetActiveSection()
        {
            var panel = Request.QueryString["panel"];
            if (panel.IsEmpty())
                return;

            if (panel == "candidates")
                CandidateSection.IsSelected = true;
            else if (panel == "seats")
                SeatPanel.IsSelected = true;
            else if (panel == "contacts")
                AttendeeSection.IsSelected = true;
            else if (panel == "notification")
                TimerSection.IsSelected = true;
        }

        private void Detail_RelationshipsModified(object sender, EventArgs e)
        {
            var ev = LoadEvent();
            var eventTitle = EventHelper.GetTitle(EventIdentifier, ev.VenueLocationName, ServiceLocator.EventSearch);

            var command = new RetitleEvent(ev.EventIdentifier, eventTitle);
            ServiceLocator.SendCommand(command);
        }

        private void Detail_Published(object sender, Controls.ExamMarkGrid.PublishedEventArgs e)
        {
            if (e.Count == -1)
            {
                EditorStatus.AddMessage(AlertType.Warning, "There are no unpublished attempts");
            }
            else if (!string.IsNullOrEmpty(e.CandidateName))
            {
                if (e.Count > 0)
                    EditorStatus.AddMessage(AlertType.Success, $"Attempt for {e.CandidateName} is published");
                else
                    EditorStatus.AddMessage(AlertType.Error, $"Attempt for {e.CandidateName} was not published");
            }
            else
                EditorStatus.AddMessage(AlertType.Success, $"<strong>{e.Count}</strong> attempt(s) have been published");
        }

        private void CandidatePanel_ExamAssigned(object sender, EventArgs e)
        {

        }

        private void AttendeePanel_Refreshed(object sender, EventArgs e)
        {
            SetupExamCandidates(null, true);
            ValidateTrainingProvider();
        }

        private void CandidatePanel_Refreshed(object sender, EventArgs e)
        {
            var canWrite = Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write);

            AttendeePanel.LoadData(EventIdentifier, canWrite);
            SetupExamCandidates(null, false);
        }

        private QEvent LoadEvent() =>
            ServiceLocator.EventSearch.GetEvent(EventIdentifier, x => x.VenueOffice, x => x.VenueLocation);

        private void Open()
        {
            var @event = LoadEvent();
            if (@event == null || @event.OrganizationIdentifier != Organization.OrganizationIdentifier)
            {
                HttpResponseHelper.Redirect(SearchUrl);
                return;
            }

            var aggregate = ServiceLocator.SnapshotRepository.Get<EventAggregate>(EventIdentifier);

            SetInputValues(aggregate.Data, @event);

            ValidateTrainingProvider();

            PreviewLink.NavigateUrl = $"/ui/portal/events/exams/outline?event={EventIdentifier}";

            EditContentTitleLink.NavigateUrl = $"/ui/admin/events/exams/describe?event={EventIdentifier}&tab=Title";
            EditContentSummaryLink.NavigateUrl = $"/ui/admin/events/exams/describe?event={EventIdentifier}&tab=Summary";
            EditContentDescriptionLink.NavigateUrl = $"/ui/admin/events/exams/describe?event={EventIdentifier}&tab=Description";
            EditContentMaterialsLink.NavigateUrl = $"/ui/admin/events/exams/describe?event={EventIdentifier}&tab=Materials";
            EditContentContactLink.NavigateUrl = $"/ui/admin/events/exams/describe?event={EventIdentifier}&tab=Instructions&subtab=Contact";
            EditContentAccommodationLink.NavigateUrl = $"/ui/admin/events/exams/describe?event={EventIdentifier}&tab=Instructions&subtab=Accommodation";
            EditContentAdditionalLink.NavigateUrl = $"/ui/admin/events/exams/describe?event={EventIdentifier}&tab=Instructions&subtab=Additional";
            EditContentCancellationLink.NavigateUrl = $"/ui/admin/events/exams/describe?event={EventIdentifier}&tab=Instructions&subtab=Cancellation";
            EditContentCompletionLink.NavigateUrl = $"/ui/admin/events/exams/describe?event={EventIdentifier}&tab=Instructions&subtab=Completion";

            var content = ContentEventClass.Deserialize(@event.Content);

            if (string.IsNullOrEmpty(content.Title.Default))
                content.Title.Default = @event.EventTitle;

            ContentTitle.LoadData(content.Title);
            ContentSummary.LoadData(content.Summary);
            ContentDescription.LoadData(content.Description);
            ContentMaterials.LoadData(content.MaterialsForParticipation);
            ContentContact.LoadData(content.Get(EventInstructionType.Contact.GetName()));
            ContentAccommodation.LoadData(content.Get(EventInstructionType.Accommodation.GetName()));
            ContentAdditional.LoadData(content.Get(EventInstructionType.Additional.GetName()));
            ContentCancellation.LoadData(content.Get(EventInstructionType.Cancellation.GetName()));
            ContentCompletion.LoadData(content.Get(EventInstructionType.Completion.GetName()));
        }

        private void SetInputValues(EventState state, QEvent @event)
        {
            var canWrite = Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write);
            var canDelete = Identity.IsGranted(Route.ToolkitName, PermissionOperation.Delete);

            var seatsReturnUrl = new ReturnUrl($"event={@event.EventIdentifier}&panel=seats");

            AddSeat.Visible = canWrite;
            //AddSeat.NavigateUrl = $"/ui/admin/events/seats/add?event={@event.EventIdentifier}";
            AddSeat.NavigateUrl = seatsReturnUrl.GetRedirectUrl($"/ui/admin/events/seats/add?event={@event.EventIdentifier}");

            SeatsGrid.LoadData(@event.EventIdentifier, canWrite);

            Detail.SetInputValues(state, @event, canWrite, canDelete);

            PageHelper.AutoBindHeader(this, null, @event.EventTitle);

            AttendeePanel.LoadData(@event.EventIdentifier, canWrite);
            SetupExamCandidates(@event, true);

            CommentPanel.IsReadOnly = !canWrite;
            CommentPanel.LoadData(@event.EventIdentifier, EventType.Exam);

            if (!IsPostBack)
                ShowPanel();
        }

        private void SetupExamCandidates(QEvent @event, bool refreshGrid)
        {
            if (!refreshGrid)
                return;

            var registrationIdentifier = !IsPostBack && Guid.TryParse(Request["registration"], out var registrationIdentifierTemp) ? registrationIdentifierTemp : (Guid?)null;
            var canWrite = Identity.IsGranted(Route.ToolkitNumber, PermissionOperation.Write);

            CandidatePanel.LoadData(@event, registrationIdentifier, canWrite);
        }

        private void ShowPanel()
        {
            if (!string.IsNullOrEmpty(Panel))
            {
                if (Panel.ToLower() == "candidates")
                    CandidateSection.IsSelected = true;
                else if (Panel.ToLower() == "comment")
                    CommentSection.IsSelected = true;
                else if (Panel.ToLower() == "notification")
                    TimerSection.IsSelected = true;
            }
            else if (!string.IsNullOrEmpty(Tab))
            {
                Detail.ShowTab(Tab);
            }
        }

        private void ValidateTrainingProvider()
        {
            if (Detail.EventExamType != EventExamType.Class.Value)
                return;

            var status = Detail.EventSchedulingStatus;
            if (status.IsEmpty() || !status.StartsWith("Approved"))
                return;

            var tpCount = ServiceLocator.EventSearch.CountAttendees(new QEventAttendeeFilter
            {
                EventIdentifier = EventIdentifier,
                ContactRole = "Training Provider"
            });

            if (tpCount == 0)
                EditorStatus.AddMessage(AlertType.Error, "A training provider contact has not been added to this event.");
        }

        private void SelectSubTab(NavItem navItem)
        {
            if (SubTab.HasValue())
            {
                var nav = GetBootstrapNav(navItem.Controls);

                if (nav != null)
                {
                    foreach (var subControl in nav.Controls)
                    {
                        var tab = subControl as NavItem;

                        if (tab != null && tab.ID == $"{HttpUtility.UrlDecode(SubTab).Replace(" ", "")}SubTab")
                        {
                            tab.IsSelected = true;
                            break;
                        }
                    }
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
    }
}
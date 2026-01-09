using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Registrations.Read;
using InSite.Application.Registrations.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Events.Candidates.Forms
{
    public partial class Edit : AdminBasePage, IHasParentLinkParameters, IOverrideWebRouteParent
    {
        #region Properties

        private const string SearchUrl = "/ui/admin/events/exams/search";

        private Guid RegistrationIdentifier
            => Guid.TryParse(Request.QueryString["registration"], out var value) ? value : Guid.Empty;

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ApprovalPanelButton.Click += (s, a) =>
            {
                var registration = GetRegistration();

                ServiceLocator.SendCommand(
                    new ChangeApproval(RegistrationIdentifier, ApprovalStatus.SelectedValue, null,
                        new ProcessState(ExecutionState.Started),
                        registration.ApprovalStatus));
            };

            AddAccommodationButton.Click += AddAccommodationButton_Click;
            AccommodationsRepeater.ItemCommand += AccommodationsRepeater_ItemCommand;

            SchoolID.AutoPostBack = true;
            SchoolID.ValueChanged += (s, a) => BindInstructors();

            SaveButton.Click += (s, a) => { if (Page.IsValid) Save(); };
        }

        public override void ApplyAccessControl()
        {
            base.ApplyAccessControl();

            SaveButton.Visible = CanEdit;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Request.QueryString["status"] == "saved")
                SetStatus(ScreenStatus, StatusType.Saved);

            ApprovalStatus.DataValueField = "ItemName";
            ApprovalStatus.DataTextField = "ItemName";
            ApprovalStatus.DataSource = TCollectionItemCache.Select(new TCollectionItemFilter 
            { 
                OrganizationIdentifier = Organization.Key,
                CollectionName = CollectionName.Registrations_Approval_Status
            });
            ApprovalStatus.DataBind();

            if (ApprovalStatus.Items.Count == 0)
                ApprovalStatusText.Text = "None";

            if (Request["panel"] == "schools")
                SchoolTab.IsSelected = true;

            Open();

            CancelButton.NavigateUrl = GetParentUrl(null);
        }

        #endregion

        #region Methods (accommodations)

        private static readonly Regex AccommodationTypeTimePattern =
            new Regex("\\(\\+(?:(?:(?<Hours>\\d+(?:\\.\\d+)?)hr)|(?:(?<Minutes>\\d+)min))\\)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private void AddAccommodationButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var type = AccommodationTypeSelectorView.IsActive
                ? AccommodationTypeSelector.Value
                : AccommodationTypeText.Text.CleanTrim();
            var name = AccommodationName.Text;

            AccommodationTypeSelector.Value = null;
            AccommodationTypeSelectorView.IsActive = true;
            AccommodationTypeText.Text = null;
            AccommodationName.Text = null;

            AddAccommodation(type, name);

            var registration = GetRegistration();

            BindCandidateTimeLimit(registration);
            BindAccommodations();
        }

        private void AccommodationsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                DeleteAccommodation((string)e.CommandArgument);

                var registration = GetRegistration();

                BindCandidateTimeLimit(registration);
                BindAccommodations();
            }
        }

        public void AddAccommodation(string type, string name)
        {
            if (type.IsEmpty())
                return;

            if (type.Length > 50)
                type = type.Substring(0, 50);

            var timeExtension = 0;
            var item = TCollectionItemCache.SelectFirst(new TCollectionItemFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                CollectionName = CollectionName.Activities_Exams_Accommodation_Type,
                ItemName = type
            });

            if (item == null)
            {
                var timeMatch = AccommodationTypeTimePattern.Match(type);
                if (timeMatch.Success)
                {
                    if (decimal.TryParse(timeMatch.Groups["Hours"]?.Value, out var hours))
                        timeExtension = (int)(hours * 60);
                    else if (int.TryParse(timeMatch.Groups["Minutes"]?.Value, out var minutes))
                        timeExtension = minutes;
                }
            }
            else
            {
                type = item.ItemName;

                if (item.ItemHours.HasValue)
                    timeExtension = (int)(item.ItemHours * 60);
            }

            ServiceLocator.SendCommand(new GrantAccommodation(RegistrationIdentifier, type, name, timeExtension));
            ServiceLocator.SendCommand(new LimitExamTime(RegistrationIdentifier));
        }

        public void BindAccommodations()
        {
            var accommodations = ServiceLocator.RegistrationSearch.GetAccommodations(RegistrationIdentifier);

            AccommodationField.Visible = accommodations.Count > 0;

            if (accommodations.Count > 0)
            {
                AccommodationsRepeater.DataSource = accommodations;
                AccommodationsRepeater.DataBind();
            }

            var allTypes = ServiceLocator.RegistrationSearch.GetAccommodationTypes(Organization.OrganizationIdentifier);
            AccommodationTypeSelector.AdditionalOptions = allTypes;
            AccommodationTypeSelector.RefreshData();
        }

        public void DeleteAccommodation(string type)
        {
            ServiceLocator.SendCommand(new RevokeAccommodation(RegistrationIdentifier, type));
            ServiceLocator.SendCommand(new LimitExamTime(RegistrationIdentifier));
        }

        public void BindCandidateTimeLimit(QRegistration registration)
        {
            CandidateTimeLimit.Text = registration.ExamTimeLimit.HasValue
                ? $"{Math.Round(registration.ExamTimeLimit.Value / 60.0, 1):n1} h"
                : "None";
            CandidateTimeLimitUpdatePanel.Update();
        }

        #endregion

        #region Methods (open)

        private void Open()
        {
            var registration = GetRegistration();
            if (registration == null || registration.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect(SearchUrl);

            var @event = ServiceLocator.EventSearch.GetEvent(registration.EventIdentifier);
            if (@event == null || @event.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect(GetParentUrl(null));

            var candidate = PersonSearch.Select(Organization.OrganizationIdentifier, registration.CandidateIdentifier, x => x.User);
            if (candidate == null)
                HttpResponseHelper.Redirect(GetParentUrl(null));

            var venue = @event.VenueLocationIdentifier.HasValue
                ? ServiceLocator.GroupSearch.GetGroup(@event.VenueLocationIdentifier.Value)
                : null;
            var form = registration.ExamFormIdentifier.HasValue
                ? ServiceLocator.BankSearch.GetForm(registration.ExamFormIdentifier.Value)
                : null;

            PageHelper.AutoBindHeader(
                Page,
                qualifier: $"{candidate.User.FullName} <span class='form-text'>{candidate.PersonCode}</span>");

            SchoolID.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
            SchoolID.Filter.GroupType = GroupTypes.Team;
            SchoolID.Filter.GroupLabel = "School";

            GradingStatus.Text = registration.GradingStatus.IfNullOrEmpty("N/A");

            var approval = GetProcess(registration.ApprovalProcess);

            BindPublicationPanel(registration.ApprovalStatus, approval);

            {
                ApprovalStatus.SelectedValue = registration.ApprovalStatus ?? "Pending";
                ApprovalReason.Text = registration.ApprovalReason;
                SettingsAttendanceStatus.Value = registration.AttendanceStatus;
                CandidateType.Value = registration.CandidateType;

                var eligibility = GetProcess(registration.EligibilityProcess);
                if (eligibility.Execution != ExecutionState.Undefined)
                {
                    EligibilityStatus.InnerText = eligibility.Execution.ToString();

                    if (eligibility.HasErrors)
                    {
                        EligibilityErrors.DataSource = eligibility.Errors;
                        EligibilityErrors.DataBind();
                    }

                    if (eligibility.HasWarnings)
                    {
                        EligibilityWarnings.DataSource = eligibility.Warnings;
                        EligibilityWarnings.DataBind();
                    }
                }
                else
                {
                    EligibilityStatus.InnerText = $"Not Completed";
                }
            }

            EventStartTime.Text = @event.EventScheduledStart.Format(User.TimeZone, true);
            EventFormat.Text = @event.EventFormat;
            EventTitle.Text = @event.EventTitle;
            EventNumber.Text = $"{@event.ExamType} {@event.EventNumber}-{@event.EventBillingType}";

            if (venue != null)
            {
                EventVenue.Text = venue.GroupName;
                EventVenueRoom.InnerText = @event.VenueRoom;
            }
            else
                EventVenue.Text = "None";

            if (form != null)
            {
                FormTitle.Text = form.FormTitle;
                FormName.Text = form.FormName;
                FormCode.Text = form.FormCode;
            }
            else
            {
                FormTitle.Text = "No Exam";
                FormCodeField.Visible = false;
                FormNameField.Visible = false;
            }

            RegistrationPassword.Text = registration.RegistrationPassword;

            BindCandidateTimeLimit(registration);
            BindAccommodations();

            SchoolID.Value = registration.SchoolIdentifier;

            BindInstructors();

            CancelButton.NavigateUrl = GetParentUrl(null);

            SaveButton.Visible = CanEdit;
            ApprovalPanelButton.Visible = CanEdit;
        }

        private void BindPublicationPanel(string status, ProcessState process)
        {
            ApprovalPanel.Visible = true;

            if (process.HasErrors)
            {
                ApprovalPanel.CssClass = "alert alert-danger";
                ApprovalPanelText.Text = "Publication <strong>failed</strong>. " + string.Join(", ", process.Errors);
            }
            else if (status.IsNotEmpty())
            {
                if (status == "Started")
                {
                    ApprovalPanel.CssClass = "alert alert-warning";
                    ApprovalPanelText.Text = "Publication <strong>pending</strong>. Waiting for Direct Access...";
                }
                else
                {
                    ApprovalPanel.CssClass = "alert alert-success";
                    ApprovalPanelText.Text = "Publication <strong>succeeded</strong>.";
                }
            }
            else
            {
                ApprovalPanel.Visible = false;
            }
        }

        private void BindInstructors()
        {
            var hasSchool = SchoolID.HasValue;

            NewContactsLink.Visible = hasSchool;
            InstructorsField.Visible = hasSchool;

            if (!hasSchool)
                return;

            var school = SchoolID.Value;
            NewContactsLink.NavigateUrl = $"/ui/admin/events/candidates/new-person?school={school}&registration={RegistrationIdentifier}";
            NewContactsLink.Visible = CanEdit;

            var schoolMembers = MembershipSearch.Bind(x => x.User, x => x.GroupIdentifier == school, nameof(User.FullName));
            var instructors = ServiceLocator.RegistrationSearch.GetInstructors(RegistrationIdentifier);

            Instructors.Items.Clear();

            foreach (var user in schoolMembers)
            {
                var item = new System.Web.UI.WebControls.ListItem(user.FullName, user.UserIdentifier.ToString());
                item.Selected = instructors.Find(x => x.UserIdentifier == user.UserIdentifier) != null;

                Instructors.Items.Add(item);
            }

            InstructorsField.Visible = Instructors.Items.Count > 0;
        }

        private ProcessState GetProcess(string json)
        {
            if (json.HasValue())
                return ServiceLocator.Serializer.Deserialize<ProcessState>(json);
            return new ProcessState();
        }

        #endregion

        #region Methods (save)

        private void Save()
        {
            var registration = GetRegistration();

            DetectApprovalChange(registration);
            DetectCandidateTypeChange(registration);
            DetectPasswordChange(registration);
            DetectAttendanceChange(registration);
            DetectSchoolChange(registration);

            HttpResponseHelper.Redirect(GetParentUrl(null));
        }

        private void DetectApprovalChange(QRegistration registration)
        {
            var status = ApprovalStatus.SelectedValue;
            var reason = ApprovalReason.Text;

            var statusChanged = registration.ApprovalStatus != status;
            var reasonChanged = (registration.ApprovalReason ?? string.Empty) != reason;

            if (statusChanged || reasonChanged)
                ServiceLocator.SendCommand(new ChangeApproval(registration.RegistrationIdentifier, status, reason, new ProcessState(ExecutionState.Started), registration.ApprovalStatus));
        }

        private void DetectCandidateTypeChange(QRegistration registration)
        {
            var type = CandidateType.Value;
            var typeChanged = registration.CandidateType != type;
            if (typeChanged)
                ServiceLocator.SendCommand(new ChangeCandidateType(registration.RegistrationIdentifier, type));
        }

        private void DetectAttendanceChange(QRegistration registration)
        {
            var status = SettingsAttendanceStatus.Value;

            var statusChanged = registration.AttendanceStatus != status;

            if (statusChanged)
                ServiceLocator.SendCommand(new TakeAttendance(registration.RegistrationIdentifier, status, null, null));
        }

        private void DetectPasswordChange(QRegistration registration)
        {
            var password = RegistrationPassword.Text;

            if (string.IsNullOrWhiteSpace(password))
                return;

            var isDifferent = registration.RegistrationPassword != password;

            if (isDifferent)
                ServiceLocator.SendCommand(new ChangeRegistrationPassword(registration.RegistrationIdentifier, password));
        }

        private void DetectSchoolChange(QRegistration registration)
        {
            var commands = new List<ICommand>();

            GetSchoolChanges(registration, commands);

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);
        }

        private void GetSchoolChanges(QRegistration registration, List<ICommand> commands)
        {
            var schoolId = SchoolID.Value;

            if (schoolId != registration.SchoolIdentifier)
            {
                if (schoolId.HasValue)
                    commands.Add(new AssignSchool(registration.RegistrationIdentifier, schoolId.Value));
                else
                    commands.Add(new UnassignSchool(registration.RegistrationIdentifier));
            }

            List<Guid> addInstructors, removeInstructors;
            var oldInstructors = ServiceLocator.RegistrationSearch.GetInstructors(registration.RegistrationIdentifier).Select(x => x.UserIdentifier).ToList();

            if (schoolId.HasValue)
            {
                var newInstructors = new List<Guid>();

                foreach (System.Web.UI.WebControls.ListItem item in Instructors.Items)
                {
                    if (item.Selected)
                        newInstructors.Add(Guid.Parse(item.Value));
                }

                addInstructors = new List<Guid>();
                removeInstructors = new List<Guid>();

                foreach (var oldInstructor in oldInstructors)
                {
                    if (!newInstructors.Contains(oldInstructor))
                        removeInstructors.Add(oldInstructor);
                }

                foreach (var newInstructor in newInstructors)
                {
                    if (!oldInstructors.Contains(newInstructor))
                        addInstructors.Add(newInstructor);
                }
            }
            else
            {
                addInstructors = null;
                removeInstructors = oldInstructors;
            }

            foreach (var instructor in removeInstructors)
                commands.Add(new RemoveInstructor(registration.RegistrationIdentifier, instructor));

            if (addInstructors != null)
            {
                foreach (var instructor in addInstructors)
                    commands.Add(new AddInstructor(registration.RegistrationIdentifier, instructor));
            }
        }

        #endregion

        #region Methods (helpers)

        private QRegistration GetRegistration() =>
            ServiceLocator.RegistrationSearch.GetRegistration(RegistrationIdentifier);

        #endregion

        #region Methods (navigation back)

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent) =>
            GetParentLinkParameters(parent, null);

        #endregion

    }
}
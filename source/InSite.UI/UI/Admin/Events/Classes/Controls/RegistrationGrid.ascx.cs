using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Events.Classes.Reports;
using InSite.Admin.Events.Registrations.Reports;
using InSite.Application.Contacts.Read;
using InSite.Application.Events.Read;
using InSite.Application.Registrations.Read;
using InSite.Application.Registrations.Write;
using InSite.Common;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Messages;
using InSite.Domain.Reports;
using InSite.Persistence;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Common.Timeline.Commands;
using Shift.Constant;
using Shift.Sdk.UI;

using CheckBox = System.Web.UI.WebControls.CheckBox;

namespace InSite.Admin.Events.Classes.Controls
{
    public partial class RegistrationGrid : SearchResultsGridViewController<QRegistrationFilter>
    {
        private const string EmptyKey = "---";

        #region Classes

        [Serializable]
        private class CachedFilter
        {
            public string CandidateName { get; set; }
            public string[] ApprovalStatuses { get; set; }
            public string[] AttendanceStatuses { get; set; }
        }

        private class PersonItem
        {
            public string UserFullName { get; set; }
            public bool HasPerson { get; set; }
            public Guid? GroupId { get; set; }
            public string GroupName { get; set; }
            public string PersonCode { get; set; }
            public string OccupationInterest { get; set; }
        }

        private class RegistrationGridItem
        {
            public bool HasPerson { get; set; }
            public bool IncludeInT2202 { get; set; }

            public DateTimeOffset? RegistrationRequestedOn { get; set; }

            public decimal? RegistrationFee { get; set; }

            public Guid CandidateIdentifier { get; set; }
            public Guid EventIdentifier { get; set; }
            public Guid RegistrationIdentifier { get; set; }
            public Guid? EmployerIdentifier { get; set; }
            public Guid? FormIdentifier { get; set; }

            public int? RegistrationSequence { get; set; }

            public string ApprovalStatus { get; set; }
            public string AttendanceStatus { get; set; }
            public string EmployerName { get; set; }
            public string FormCode { get; set; }
            public string PaymentStatus { get; set; }
            public string UserFullName { get; set; }
            public string PersonCode { get; set; }
            public string OccupationInterest { get; set; }
            public string BillingCode { get; set; }

            public List<QAccommodation> AccommodationList { get; set; }

            public List<RegistrationFormItem> Forms { get; set; } = new List<RegistrationFormItem>();

            public List<RegistrationFormItem> AssignedForms => Forms.Where(x => x.FormIdentifier != null).ToList();

            public string Accommodations
            {
                get
                {
                    if (AccommodationList == null || AccommodationList.Count == 0)
                        return "None";

                    var html = new StringBuilder();
                    foreach (var item in AccommodationList)
                    {
                        html.Append($"<div class='text-nowrap'>{item.AccommodationName}</div>");
                    }

                    return html.ToString().IfNullOrEmpty("None");
                }
            }
        }

        private class RegistrationFormItem
        {
            public Guid? FormIdentifier { get; set; }

            public bool ShowPassword { get; set; }

            public DateTimeOffset? EligibilityUpdated { get; set; }

            public string EligibilityStatus { get; set; }
            public string FormName { get; set; }
            public string FormTitle { get; set; }
            public string Password { get; set; }

            public string EligibilityStatusHtml
            {
                get
                {
                    if (EligibilityStatus == null)
                        return string.Empty;

                    var indicator = "info";
                    if (EligibilityStatus == RegistrationEligibility.Eligible)
                        indicator = "success";
                    else if (EligibilityStatus == RegistrationEligibility.NotEligible)
                        indicator = "danger";

                    var result = $"<div class=\"badge bg-{indicator}\">{EligibilityStatus}</div>";

                    if (EligibilityUpdated.HasValue)
                        result += $"<div class=\"form-text text-body-secondary\">Sent {EligibilityUpdated.Value.Format(CurrentSessionState.Identity.User.TimeZone)}</div>";

                    return result;
                }
            }
        }

        #endregion

        #region Properties

        protected override bool IsFinder => false;

        private bool CanWrite
        {
            get => (bool)ViewState[nameof(CanWrite)];
            set => ViewState[nameof(CanWrite)] = value;
        }

        private string ReturnParams
        {
            get => (string)ViewState[nameof(ReturnParams)];
            set => ViewState[nameof(ReturnParams)] = value;
        }

        private Guid EventId
        {
            get => (Guid)ViewState[nameof(EventId)];
            set => ViewState[nameof(EventId)] = value;
        }

        protected bool ShowForms
        {
            get => (bool)ViewState[nameof(ShowForms)];
            private set => ViewState[nameof(ShowForms)] = value;
        }

        private bool MultipleFormsAllowed
        {
            get => (bool)(ViewState[nameof(MultipleFormsAllowed)] ?? false);
            set => ViewState[nameof(MultipleFormsAllowed)] = value;
        }

        private int? FormAttachmentLimit
        {
            get => (int?)ViewState[nameof(FormAttachmentLimit)];
            set => ViewState[nameof(FormAttachmentLimit)] = value;
        }

        private const string CachedFiltersKey = "RegistrationGrid.CachedFilters";

        private Dictionary<Guid, CachedFilter> CachedFilters =>
            (Dictionary<Guid, CachedFilter>)(Session[CachedFiltersKey]
                ?? (Session[CachedFiltersKey] = new Dictionary<Guid, CachedFilter>()));

        #endregion

        #region Fields

        private ReturnUrl _returnUrl;

        private bool _hasData;
        private bool _isAllIncludeInT2202;

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FilterButton.Click += FilterButton_Click;

            ApprovalStatusComboBox.AutoPostBack = true;
            ApprovalStatusComboBox.ValueChanged += (a, b) => Search();

            AttendanceStatusComboBox.AutoPostBack = true;
            AttendanceStatusComboBox.ValueChanged += (a, b) => Search();

            AddFormSelector.AutoPostBack = true;
            AddFormSelector.ItemsRequested += AddFormSelector_ItemsRequested;
            AddFormSelector.ValueChanged += AddFormSelector_ValueChanged;

            Grid.DataBinding += Grid_DataBinding;
            Grid.RowCreated += Grid_RowCreated;
            Grid.RowDataBound += Grid_RowDataBound;
            Grid.RowCommand += Grid_RowCommand;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var scriptManager = ScriptManager.GetCurrent(Page);
            scriptManager.RegisterPostBackControl(DownloadDropDown);
        }

        #endregion

        #region Event handlers

        private void FilterButton_Click(object sender, EventArgs e)
        {
            Search();

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(RegistrationGrid),
                "filter_focus",
                $"inSite.common.baseInput.focus('{FilterTextBox.ClientID}', true);",
                true);
        }

        private void Grid_DataBinding(object sender, EventArgs e)
        {
            _returnUrl = null;
        }

        private void Grid_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                var allT2202 = (CheckBox)e.Row.FindControl("AllT2202");
                allT2202.AutoPostBack = true;
                allT2202.CheckedChanged += AllT2202_CheckedChanged;
            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var T2202 = (CheckBox)e.Row.FindControl("T2202");
                T2202.AutoPostBack = true;
                T2202.CheckedChanged += T2202_CheckedChanged;

                var formId = (FindBankForm)e.Row.FindControl("FormIdentifier");
                formId.Filter.EventIdentifier = EventId;
                formId.AutoPostBack = true;
                formId.ValueChanged += FormIdentifier_ValueChanged;

                var formRepeater = (Repeater)e.Row.FindControl("FormRepeater");
                formRepeater.ItemCommand += FormRepeater_ItemCommand;
                formRepeater.ItemDataBound += FormRepeater_ItemDataBound;
            }
        }

        private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                BindHeaderRow(e);
            else if (e.Row.RowType == DataControlRowType.DataRow)
                BindContentRow(e);
        }

        private void BindHeaderRow(GridViewRowEventArgs e)
        {
            var allT2202 = (CheckBox)e.Row.FindControl("AllT2202");
            allT2202.Checked = _isAllIncludeInT2202;
            allT2202.Enabled = _hasData;
        }

        private void BindContentRow(GridViewRowEventArgs e)
        {
            var item = (RegistrationGridItem)e.Row.DataItem;

            var isUserExist = item.UserFullName != null;
            var isPersonExist = item.HasPerson;

            e.Row.CssClass = !isUserExist
                ? "table-danger"
                : !isPersonExist
                    ? "table-warning"
                    : null;

            var approvalStatus = item.ApprovalStatus;
            var isRegistered = StringHelper.Equals(approvalStatus, "Registered");
            var isMoved = StringHelper.Equals(approvalStatus, "Moved");
            var isWaitlisted = StringHelper.Equals(approvalStatus, "Waitlisted");
            var needCompleteRegistration = !isRegistered && !isMoved
                && !StringHelper.Equals(approvalStatus, "Registration Cancelled")
                && !StringHelper.Equals(approvalStatus, "Waitlist Removed")
                && !StringHelper.Equals(approvalStatus, "Invitation Declined");

            var completeRegisterLink = (IconLink)e.Row.FindControl("CompleteRegisterLink");
            completeRegisterLink.Visible = isPersonExist && needCompleteRegistration;

            var cardLink = (IconLink)e.Row.FindControl("CardLink");
            cardLink.Visible = isPersonExist && !needCompleteRegistration;

            var editLink = (IconLink)e.Row.FindControl("EditLink");
            editLink.NavigateUrl = GetRedirectUrl($"/ui/admin/registrations/classes/edit?id={item.RegistrationIdentifier}");
            editLink.Visible = isPersonExist;

            var sendInviteButton = (IconButton)e.Row.FindControl("SendInviteButton");
            sendInviteButton.Visible = isPersonExist && isWaitlisted;
            sendInviteButton.ConfirmText = Organization.Toolkits.Events?.SendRegistrationInvitationToRequestedBy == true
                ? "An invitation to complete this registration will be sent to the person who added the individual to the waitlist. Do you want to proceed?"
                : "An invitation to complete this registration will be sent to the Registrant. Do you want to proceed?";

            var voidItemButton = e.Row.FindControl("VoidItemButton");
            voidItemButton.Visible = CanWrite;

            var addFormButton = (IconButton)e.Row.FindControl("AddFormButton");
            addFormButton.Visible = CanWrite
                && MultipleFormsAllowed
                && item.AssignedForms.Count > 0
                && (FormAttachmentLimit == null || item.AssignedForms.Count < FormAttachmentLimit.Value);
        }

        private void FormRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var form = (RegistrationFormItem)e.Item.DataItem;

            var cancelAssessmentButton = (IconButton)e.Item.FindControl("CancelAssessmentButton");
            cancelAssessmentButton.Visible = form.EligibilityStatus != "Eligible";
        }

        private void FormRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "DeleteForm")
                return;

            var row = (GridViewRow)((Repeater)source).NamingContainer;
            var registrationId = Grid.GetDataKey<Guid>(row);

            DeleteForm(registrationId, Guid.Parse((string)e.CommandArgument));
        }

        private void Grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var registrationId = Grid.GetDataKey<Guid>(e);

            if (e.CommandName == "SendInvite")
                SendInvite(registrationId);
        }

        private void SendInvite(Guid registrationId)
        {
            var registration = RegistrationInvitationHelper.SendInvitation(
                registrationId,
                User.UserIdentifier,
                true,
                ServiceLocator.SendCommand,
                ServiceLocator.RegistrationSearch
            );

            if (registration == null)
                return;

            var recipient = SendRegistrationInvitationMessage(registration);

            SearchWithCurrentPageIndex(Filter);

            var message = $"The invitation has been sent to {recipient?.UserFullName} " +
                $"at {recipient?.UserEmail}";

            if (recipient != registration.Candidate)
                message += $" for {registration.Candidate?.UserFullName}.";

            StatusAlert.AddMessage(AlertType.Success, message);
        }

        private VPerson SendRegistrationInvitationMessage(QRegistration registration)
        {
            var domain = ServiceLocator.AppSettings.Partition.Domain;
            var registrationRelativeUrl = $"/ui/portal/events/classes/register?event={registration.EventIdentifier}&candidate={registration.CandidateIdentifier}";
            var registrationAbsoluteUrl = UrlHelper.GetAbsoluteUrl(domain, ServiceLocator.AppSettings.Environment, registrationRelativeUrl, Organization.Code);

            var recipient = Organization.Toolkits.Events?.SendRegistrationInvitationToRequestedBy == true
                ? registration.RegistrationRequestedByPerson ?? registration.Candidate
                : registration.Candidate;

            ServiceLocator.AlertMailer.Send(
                Organization.OrganizationIdentifier,
                recipient.UserIdentifier,
                new AlertRegistrationInvitation
                {
                    CandidateFullName = registration.Candidate.UserFullName,
                    ClassTitle = registration.Event.EventTitle,
                    ClassRegistrationLink = registrationAbsoluteUrl,
                    RegistrationEndTime = DateTimeOffset.UtcNow.AddHours(RegistrationInvitationHelper.InvitationExpiresInHours).
                        Format(registration.Candidate.UserTimeZone),
                    ClassStartTime = registration.Event.EventScheduledStart.Format(registration.Candidate.UserTimeZone),
                    ClassAchievement = GetAchievementName(registration.Event)
                }
            );

            return recipient;

            string GetAchievementName(QEvent _event)
            {
                if (_event == null || !_event.AchievementIdentifier.HasValue)
                    return null;

                return ServiceLocator.AchievementSearch.GetAchievement(registration.Event.AchievementIdentifier.Value)?.AchievementTitle;
            }
        }

        private void DeleteForm(Guid registrationId, Guid formId)
        {
            if (MultipleFormsAllowed)
                ServiceLocator.SendCommand(new RemoveExamForm(registrationId, formId));
            else
                ServiceLocator.SendCommand(new UnassignExamForm(registrationId));

            ServiceLocator.SendCommand(new LimitExamTime(registrationId));

            SearchWithCurrentPageIndex(Filter);
        }

        private void AllT2202_CheckedChanged(object sender, EventArgs e)
        {
            var allT2202 = (CheckBox)sender;

            OnChangeT2202(allT2202.Checked);

            var message = allT2202.Checked
                ? "All registration have been included to T2202"
                : "All registration have been excluded to T2202";

            StatusAlert.AddMessage(AlertType.Success, message);
        }

        private void OnChangeT2202(bool include)
        {
            var filter = Filter.Clone();
            filter.HasCandidate = true;

            var registrations = ServiceLocator.RegistrationSearch.GetRegistrations(filter);
            var commands = new List<Command>();

            foreach (var registration in registrations)
            {
                if (include)
                {
                    if (!registration.IncludeInT2202)
                        commands.Add(new IncludeRegistrationToT2202(registration.RegistrationIdentifier));
                }
                else if (registration.IncludeInT2202)
                {
                    commands.Add(new ExcludeRegistrationFromT2202(registration.RegistrationIdentifier));
                }
            }

            ServiceLocator.SendCommands(commands);

            SearchWithCurrentPageIndex(Filter);
        }

        private void T2202_CheckedChanged(object sender, EventArgs e)
        {
            var T2202 = (CheckBox)sender;
            var row = (GridViewRow)T2202.NamingContainer;
            var grid = (Grid)row.NamingContainer;
            var registrationId = grid.GetDataKey<Guid>(row);

            var command = T2202.Checked
                ? new IncludeRegistrationToT2202(registrationId)
                : (Command)new ExcludeRegistrationFromT2202(registrationId);

            ServiceLocator.SendCommand(command);
        }

        private void FormIdentifier_ValueChanged(object sender, FindEntityValueChangedEventArgs e)
        {
            var formId = (FindBankForm)sender;
            if (formId.Value == null)
                return;

            var row = (GridViewRow)formId.NamingContainer;
            var grid = (Grid)row.NamingContainer;
            var registrationId = grid.GetDataKey<Guid>(row);

            if (MultipleFormsAllowed)
            {
                ServiceLocator.SendCommand(new AddExamForm(registrationId, Guid.NewGuid(), formId.Value.Value));
                ServiceLocator.SendCommand(new LimitExamTime(registrationId, formId.Value.Value));
            }
            else
            {
                ServiceLocator.SendCommand(new AssignExamForm(registrationId, formId.Value.Value, null));
                ServiceLocator.SendCommand(new LimitExamTime(registrationId));
            }

            SearchWithCurrentPageIndex(Filter);
        }

        private void AddFormSelector_ItemsRequested(object sender, EventArgs e)
        {
            var registrationId = GetAddFormRegistration();

            AddFormSelector.Filter.EventIdentifier = EventId;
            AddFormSelector.Filter.ExcludeFormIdentifiers = GetAssignedForms(registrationId);
        }

        private void AddFormSelector_ValueChanged(object sender, FindEntityValueChangedEventArgs e)
        {
            var registrationId = GetAddFormRegistration();
            var formId = AddFormSelector.Value;

            AddFormSelector.Value = null;
            AddFormRegistrationIdentifier.Value = null;

            if (registrationId == null || formId == null)
                return;

            ServiceLocator.SendCommand(new AddExamForm(registrationId.Value, Guid.NewGuid(), formId.Value));
            ServiceLocator.SendCommand(new LimitExamTime(registrationId.Value, formId.Value));

            SearchWithCurrentPageIndex(Filter);
        }

        private Guid[] GetAssignedForms(Guid? registrationId)
        {
            var registration = registrationId.HasValue
                ? ServiceLocator.RegistrationSearch.GetRegistration(registrationId.Value, x => x.ExamForms)
                : null;

            if (registration == null)
                return null;

            var forms = new List<Guid>();

            if (registration.ExamFormIdentifier.HasValue)
                forms.Add(registration.ExamFormIdentifier.Value);

            forms.AddRange(registration.ExamForms.Select(x => x.ExamFormIdentifier));

            return forms.ToArray();
        }

        #endregion

        #region Methods (public)

        public void LoadData(Guid eventId, bool canWrite, bool showForms, string returnParams, int? page)
        {
            CanWrite = canWrite;
            ShowForms = showForms;
            ReturnParams = returnParams;
            EventId = eventId;
            MultipleFormsAllowed = showForms && Organization.Toolkits.Events?.AllowMultipleAssessmentForms == true;
            FormAttachmentLimit = MultipleFormsAllowed
                ? ServiceLocator.EventSearch.GetEvent(eventId)?.FormAttachmentLimit
                : null;

            AddButton.Visible = canWrite;
            AddButton.NavigateUrl = GetRedirectUrl($"/ui/admin/registrations/classes/add?event={eventId}");

            BindStatuses();

            Grid.Columns.FindByName("AssessmentForm").Visible = ShowForms;
            Grid.Columns.FindByName("AccommodationsForm").Visible = ServiceLocator.RegistrationSearch.HasAccommodations(eventId);
            Grid.Columns.FindByName("Employer").HeaderText = LabelHelper.GetLabelContentText("Employer at time of Registration");

            LoadCachedFilter(eventId);

            Search(eventId, page);
        }

        private void LoadCachedFilter(Guid eventId)
        {
            if (!CachedFilters.TryGetValue(eventId, out var cachedFilter))
                return;

            FilterTextBox.Text = cachedFilter.CandidateName;
            ApprovalStatusComboBox.Values = cachedFilter.ApprovalStatuses;
            AttendanceStatusComboBox.Values = cachedFilter.AttendanceStatuses;
        }

        private void SaveCachedFilter(Guid eventId)
        {
            CachedFilters[eventId] = new CachedFilter
            {
                CandidateName = FilterTextBox.Text,
                ApprovalStatuses = ApprovalStatusComboBox.ValuesArray,
                AttendanceStatuses = AttendanceStatusComboBox.ValuesArray
            };
        }

        private void Search()
        {
            Search(Filter.EventIdentifier.Value, 1);
        }

        private void Search(Guid eventId, int? page)
        {
            SaveCachedFilter(eventId);

            var filter = new QRegistrationFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                EventIdentifier = eventId,
                CandidateName = FilterTextBox.Text,
                ApprovalStatuses = ApprovalStatusComboBox.Values.Select(x => x == EmptyKey ? null : x).ToArray(),
                AttendanceStatuses = AttendanceStatusComboBox.Values.Select(x => x == EmptyKey ? null : x).ToArray()
            };

            Search(filter, (page ?? 1) - 1);
        }

        private void BindStatuses()
        {
            var registrations = ServiceLocator.RegistrationSearch
                .GetRegistrations(new QRegistrationFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    EventIdentifier = EventId
                });

            var approvalStatuses = registrations
                .GroupBy(x => x.ApprovalStatus)
                .Select(x => new { Value = x.Key ?? EmptyKey, Text = $"{x.Key ?? "No Status"} ({x.Count()})" })
                .OrderBy(x => x.Value == EmptyKey ? 1 : 0)
                .ThenBy(x => x.Value)
                .ToList();

            ApprovalStatusComboBox.LoadItems(approvalStatuses, "Value", "Text");
            ApprovalStatusComboBox.Enabled = approvalStatuses.Count > 0;

            var attendanceStatuses = registrations
                .GroupBy(x => x.AttendanceStatus)
                .Select(x => new { Value = x.Key ?? EmptyKey, Text = $"{x.Key ?? "No Status"} ({x.Count()})" })
                .OrderBy(x => x.Value == EmptyKey ? 1 : 0)
                .ThenBy(x => x.Value)
                .ToList();

            AttendanceStatusComboBox.LoadItems(attendanceStatuses, "Value", "Text");
            AttendanceStatusComboBox.Enabled = attendanceStatuses.Count > 0;
        }

        #endregion

        #region Methods (select data)

        protected override int SelectCount(QRegistrationFilter filter)
        {
            return ServiceLocator.RegistrationSearch.CountRegistrations(filter);
        }

        protected override IListSource SelectData(QRegistrationFilter filter)
        {
            var registrations = GetRegistrations(filter);
            var users = GetUsers(registrations.Select(x => x.CandidateIdentifier).Distinct());

            var data = registrations
                .Select(x =>
                {
                    users.TryGetValue(x.CandidateIdentifier, out var user);

                    return new RegistrationGridItem
                    {
                        ApprovalStatus = x.ApprovalStatus,
                        AttendanceStatus = x.AttendanceStatus,
                        CandidateIdentifier = x.CandidateIdentifier,
                        EmployerIdentifier = x.EmployerIdentifier,
                        EmployerName = x.Employer?.GroupName,
                        EventIdentifier = x.EventIdentifier,
                        FormCode = x.Form?.FormCode,
                        FormIdentifier = x.ExamFormIdentifier,
                        Forms = GetForms(x),
                        HasPerson = user?.HasPerson ?? false,
                        IncludeInT2202 = x.IncludeInT2202,
                        PaymentStatus = string.Equals(x.Payment?.PaymentStatus, "Completed", StringComparison.OrdinalIgnoreCase) ? "Paid" : x.Payment?.PaymentStatus,
                        RegistrationFee = x.RegistrationFee,
                        RegistrationIdentifier = x.RegistrationIdentifier,
                        RegistrationRequestedOn = x.RegistrationRequestedOn,
                        RegistrationSequence = x.RegistrationSequence,
                        UserFullName = user?.UserFullName,
                        PersonCode = user?.PersonCode,
                        OccupationInterest = user?.OccupationInterest,
                        AccommodationList = x.Accommodations.ToList(),
                        BillingCode = x.BillingCode
                    };
                })
                .ToList();

            _hasData = data.Count > 0;
            _isAllIncludeInT2202 = data.All(x => x.IncludeInT2202);

            return data.ToSearchResult();
        }

        private List<QRegistration> GetRegistrations(QRegistrationFilter filter)
        {
            filter.OrderBy = "RegistrationSequence,RegistrationRequestedOn,CandidateIdentifier";

            var includes = new List<Expression<Func<QRegistration, object>>>
            {
                x => x.Payment,
                x => x.Form,
                x => x.Accommodations,
                x => x.Employer
            };

            if (MultipleFormsAllowed)
                includes.Add(x => x.ExamForms.Select(f => f.Form));

            var data = ServiceLocator.RegistrationSearch.GetRegistrations(filter, includes.ToArray());

            return data;
        }

        private List<RegistrationFormItem> GetForms(QRegistration registration)
        {
            var showPassword = ShowForms && StringHelper.Equals(registration.ApprovalStatus, "Registered");

            var forms = new List<RegistrationFormItem>
            {
                new RegistrationFormItem
                {
                    FormIdentifier = registration.ExamFormIdentifier,
                    FormName = registration.Form?.FormName,
                    FormTitle = registration.Form?.FormTitle,
                    Password = registration.RegistrationPassword,
                    ShowPassword = showPassword,
                    EligibilityStatus = registration.EligibilityStatus,
                    EligibilityUpdated = registration.EligibilityUpdated
                }
            };

            if (!MultipleFormsAllowed || registration.ExamFormIdentifier == null)
                return forms;

            foreach (var examForm in registration.ExamForms.OrderBy(x => x.Sequence))
            {
                forms.Add(new RegistrationFormItem
                {
                    FormIdentifier = examForm.ExamFormIdentifier,
                    FormName = examForm.Form?.FormName,
                    FormTitle = examForm.Form?.FormTitle,
                    Password = examForm.Password,
                    ShowPassword = showPassword,
                    EligibilityStatus = examForm.EligibilityStatus,
                    EligibilityUpdated = examForm.EligibilityUpdated
                });
            }

            return forms;
        }

        private Dictionary<Guid, PersonItem> GetUsers(IEnumerable<Guid> userFilter)
        {
            var users = UserSearch
                .Bind(
                    u => new
                    {
                        u.UserIdentifier,
                        u.FullName,
                        u.Email,
                        Person = u.Persons
                            .Where(x => x.OrganizationIdentifier == Organization.Identifier)
                            .Select(x => new
                            {
                                x.EmployerGroup,
                                x.PersonCode,
                                x.OccupationStandard
                            })
                            .FirstOrDefault()
                    },
                    new UserFilter { IncludeUserIdentifiers = userFilter.ToArray() }
                )
                .ToDictionary(x => x.UserIdentifier, x => new PersonItem
                {
                    UserFullName = x.FullName,
                    HasPerson = x.Person != null,
                    GroupId = x.Person?.EmployerGroup?.GroupIdentifier,
                    GroupName = x.Person?.EmployerGroup?.GroupName,
                    PersonCode = x.Person?.PersonCode,
                    OccupationInterest = x.Person?.OccupationStandard?.ContentName
                });

            return users;
        }

        #endregion

        #region Methods (export)

        protected override IEnumerable<DownloadColumn> GetDownloadColumns(IList dataList)
        {
            return new[]
            {
                new DownloadColumn("RegistrationSequence", "#", null, 15, HorizontalAlignment.Center),
                new DownloadColumn("RegistrationRequestedOn", "Registered", "MMM dd, yyyy", 15),
                new DownloadColumn("UserFullName", "Registrant"),
                new DownloadColumn("ApprovalStatus", "Approval", null, 15),
                new DownloadColumn("AttendanceStatus", "Attendance", null, 15),
                new DownloadColumn("RegistrationFee", "Fee", "$ ###,##0.00", 15, HorizontalAlignment.Right)
            };
        }

        protected override void SetupExportButton(DropDownButton button)
        {
            base.SetupExportButton(button);

            button.Items.Add(new DropDownButtonItem
            {
                Name = "REGISTRATIONREPORT",
                IconName = "file-pdf",
                Text = "Registration Report (*.pdf)"
            });

            button.Items.Add(new DropDownButtonItem
            {
                Name = "REGISTRATIONREPORTXLSX",
                IconName = "file-excel",
                Text = "Registration Report (*.xlsx)"
            });

            button.Items.Add(new DropDownButtonItem
            {
                Name = "DETAILEDREGISTRATIONREPORT",
                IconName = "file-pdf",
                Text = "Registration Report (Detailed) (*.pdf)"
            });

            button.Items.Add(new DropDownButtonItem
            {
                Name = "DETAILEDREGISTRATIONREPORTXLSX",
                IconName = "file-excel",
                Text = "Registration Report (Detailed) (*.xlsx)"
            });

            button.Items.Add(new DropDownButtonItem
            {
                Name = "ATTENDEELIST",
                IconName = "file-pdf",
                Text = "Attendee List (*.pdf)"
            });

            button.Items.Add(new DropDownButtonItem
            {
                Name = "ATTENDEELISTXLSX",
                IconName = "file-excel",
                Text = "Attendee List (*.xlsx)"
            });

            button.Items.Add(new DropDownButtonItem
            {
                Name = "SCORESREPORT",
                IconName = "file-pdf",
                Text = "Scores Report (*.pdf)"
            });

            button.Items.Add(new DropDownButtonItem
            {
                Name = "SCORESREPORTXLSX",
                IconName = "file-excel",
                Text = "Scores Report (*.xlsx)"
            });

            button.Items.Add(new DropDownButtonItem
            {
                Name = "APPRENTICESCORES",
                IconName = "file-excel",
                Text = "Most Improved Report (*.xlsx)"
            });

            button.Items.Add(new DropDownButtonItem
            {
                Name = "EXAMLOGINCREDENTIALS",
                IconName = "file-pdf",
                Text = "Exam Login Credentials (*.pdf)"
            });

            button.Items.Add(new DropDownButtonItem
            {
                Name = "EXAMLOGINCREDENTIALSXLSX",
                IconName = "file-excel",
                Text = "Exam Login Credentials (*.xlsx)"
            });
        }

        protected override void OnExportButtonClick(string commandName)
        {
            if (commandName == "REGISTRATIONREPORT")
            {
                var @event = ServiceLocator.EventSearch.GetEvent(Filter.EventIdentifier.Value);

                var date = DateTimeOffset.Now.FormatDateOnly(User.TimeZone);

                var report = (RegistrationReport)LoadControl("~/UI/Admin/Events/Registrations/Classes/Reports/RegistrationReport.ascx");
                report.LoadReport(Filter, false);

                var siteContent = new StringBuilder();
                using (var stringWriter = new StringWriter(siteContent))
                {
                    using (var htmlWriter = new HtmlTextWriter(stringWriter))
                        report.RenderControl(htmlWriter);
                }

                var settings = new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
                {
                    PageOrientation = PageOrientationType.Portrait,
                    Viewport = new HtmlConverterSettings.ViewportSize(1400, 980),
                    MarginTop = 22,
                    MarginBottom = 22,
                    Dpi = 240,

                    HeaderTextLeft = "Registration Report",
                    HeaderFontName = "Calibri",
                    HeaderFontSize = 19,
                    HeaderSpacing = 7.8f,

                    FooterTextLeft = "Registration Report",
                    FooterTextCenter = date,
                    FooterTextRight = "Page [page] of [topage]",
                    FooterFontName = "Calibri",
                    FooterFontSize = 10,
                    FooterSpacing = 8.1f,
                };

                var data = HtmlConverter.HtmlToPdf(siteContent.ToString(), settings);

                Response.SendFile($"RegistrationReport-{@event.EventTitle}", "pdf", data);

            }
            else if (commandName == "REGISTRATIONREPORTXLSX")
            {
                var @event = ServiceLocator.EventSearch.GetEvent(Filter.EventIdentifier.Value);
                Response.SendFile($"RegistrationReport-{@event.EventTitle}", "xlsx", RegistrationReport.GetXlsx(Filter));
            }
            else if (commandName == "DETAILEDREGISTRATIONREPORT")
            {
                var @event = ServiceLocator.EventSearch.GetEvent(Filter.EventIdentifier.Value);

                var date = DateTimeOffset.Now.FormatDateOnly(User.TimeZone);

                var report = (DetailedRegistrationReport)LoadControl("~/UI/Admin/Events/Registrations/Classes/Reports/DetailedRegistrationReport.ascx");
                report.LoadReport(Filter);

                var siteContent = new StringBuilder();
                using (var stringWriter = new StringWriter(siteContent))
                {
                    using (var htmlWriter = new HtmlTextWriter(stringWriter))
                        report.RenderControl(htmlWriter);
                }

                var settings = new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
                {
                    PageOrientation = PageOrientationType.Landscape,
                    Viewport = new HtmlConverterSettings.ViewportSize(1400, 980),
                    MarginTop = 22,
                    MarginBottom = 22,
                    Dpi = 240,

                    HeaderTextLeft = "Registration Report (Detailed)",
                    HeaderFontName = "Calibri",
                    HeaderFontSize = 19,
                    HeaderSpacing = 7.8f,

                    FooterTextLeft = "Registration Report (Detailed)",
                    FooterTextCenter = date,
                    FooterTextRight = "Page [page] of [topage]",
                    FooterFontName = "Calibri",
                    FooterFontSize = 10,
                    FooterSpacing = 8.1f,
                };

                var data = HtmlConverter.HtmlToPdf(siteContent.ToString(), settings);

                Response.SendFile($"RegistrationReportDetailed-{@event.EventTitle}", "pdf", data);

            }
            else if (commandName == "DETAILEDREGISTRATIONREPORTXLSX")
            {
                var @event = ServiceLocator.EventSearch.GetEvent(Filter.EventIdentifier.Value);
                var report = (DetailedRegistrationReport)LoadControl("~/UI/Admin/Events/Registrations/Classes/Reports/DetailedRegistrationReport.ascx");
                Response.SendFile($"RegistrationReportDetailed-{@event.EventTitle}", "xlsx", report.GetXlsx(Filter));
            }
            else if (commandName == "ATTENDEELIST")
            {
                var report = (AttendeeListReport)LoadControl("../Reports/AttendeeListReport.ascx");
                var @event = ServiceLocator.EventSearch.GetEvent(Filter.EventIdentifier.Value);

                report.LoadReport(@event, User);

                var siteContent = new StringBuilder();
                using (var stringWriter = new StringWriter(siteContent))
                {
                    using (var htmlWriter = new HtmlTextWriter(stringWriter))
                        report.RenderControl(htmlWriter);
                }

                var settings = new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
                {
                    PageOrientation = PageOrientationType.Portrait,
                    Viewport = new HtmlConverterSettings.ViewportSize(980, 1400),
                    MarginLeft = 22,
                    MarginTop = 22,
                };

                var data = HtmlConverter.HtmlToPdf(siteContent.ToString(), settings);

                Response.SendFile($"ClassList-{@event.EventTitle}", "pdf", data);
            }
            else if (commandName == "ATTENDEELISTXLSX")
            {
                var @event = ServiceLocator.EventSearch.GetEvent(Filter.EventIdentifier.Value);
                Response.SendFile($"ClassList-{@event.EventTitle}", "xlsx", AttendeeListReport.GetXlsx(@event, User));
            }
            else if (commandName == "SCORESREPORT")
            {
                var report = (ScoresReport)LoadControl("../Reports/ScoresReport.ascx");
                var @event = ServiceLocator.EventSearch.GetEvent(Filter.EventIdentifier.Value, x => x.Registrations);
                var date = DateTimeOffset.Now.FormatDateOnly(User.TimeZone);

                report.LoadReport(@event);

                var siteContent = new StringBuilder();
                using (var stringWriter = new StringWriter(siteContent))
                {
                    using (var htmlWriter = new HtmlTextWriter(stringWriter))
                        report.RenderControl(htmlWriter);
                }

                var settings = new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
                {
                    PageOrientation = PageOrientationType.Landscape,
                    Viewport = new HtmlConverterSettings.ViewportSize(980, 1400),
                    MarginTop = 22,
                    MarginBottom = 22,
                    Dpi = 240,

                    HeaderTextLeft = "Class Scores Report",
                    HeaderFontName = "Calibri",
                    HeaderFontSize = 19,
                    HeaderSpacing = 7.8f,

                    FooterTextLeft = "Class Scores Report",
                    FooterTextCenter = date,
                    FooterTextRight = "Page [page] of [topage]",
                    FooterFontName = "Calibri",
                    FooterFontSize = 10,
                    FooterSpacing = 8.1f,
                };

                var data = HtmlConverter.HtmlToPdf(siteContent.ToString(), settings);

                Response.SendFile($"ScoresReport-{@event.EventTitle}", "pdf", data);
            }
            else if (commandName == "SCORESREPORTXLSX")
            {
                var report = (ScoresReport)LoadControl("../Reports/ScoresReport.ascx");
                var @event = ServiceLocator.EventSearch.GetEvent(Filter.EventIdentifier.Value, x => x.Registrations);
                Response.SendFile($"ScoresReport-{@event.EventTitle}", "xlsx", report.GetXlsx(@event));
            }
            else if (commandName == "APPRENTICESCORES")
            {
                var data = ApprenticeScoresReport.GetXlsx(Filter);
                Response.SendFile("MostImprovedReport", "xlsx", data);
            }
            else if (commandName == "EXAMLOGINCREDENTIALS")
            {
                var @event = ExamLoginCredentialsReport.GetEventInfo(Filter.EventIdentifier.Value);
                Response.SendFile($"ExamLoginCredentials-{@event.EventTitle}", "pdf", ExamLoginCredentialsReport.GetPdf(@event));
            }
            else if (commandName == "EXAMLOGINCREDENTIALSXLSX")
            {
                var @event = ExamLoginCredentialsReport.GetEventInfo(Filter.EventIdentifier.Value);
                Response.SendFile($"ExamLoginCredentials-{@event.EventTitle}", "xlsx", ExamLoginCredentialsReport.GetXlsx(@event));
            }
            else
            {
                base.OnExportButtonClick(commandName);
            }
        }

        #endregion

        #region Helpers

        private Guid? GetAddFormRegistration() =>
            Guid.TryParse(AddFormRegistrationIdentifier.Value, out var value) ? value : (Guid?)null;

        protected string GetRedirectUrl(string format, params object[] args)
        {
            var url = string.Format(format, args);

            if (ReturnParams == null)
                return url;

            if (_returnUrl == null)
            {
                var @params = HttpUtility.ParseQueryString(ReturnParams);
                @params["grid-page"] = (GridPageIndex + 1).ToString();

                _returnUrl = new ReturnUrl(@params.ToString());
            }

            return _returnUrl.GetRedirectUrl(url);
        }

        protected string GetInvitationSentTime()
        {
            var item = Page.GetDataItem();
            var registrationId = (Guid)DataBinder.Eval(item, "RegistrationIdentifier");

            var time = RegistrationInvitationHelper.GetInvitationSentTime(registrationId, ServiceLocator.RegistrationSearch);

            return time.HasValue
                ? TimeZones.Format(time.Value, User.TimeZone, true)
                : null;
        }

        #endregion
    }
}

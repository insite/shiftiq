using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

using InSite.Application.Events.Read;
using InSite.Application.Registrations.Read;
using InSite.Application.Registrations.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Timeline.Commands;
using Shift.Constant;
using Shift.Sdk.UI;

using AggregateOutline = InSite.Admin.Logs.Aggregates.Outline;
using CheckBox = System.Web.UI.WebControls.CheckBox;

namespace InSite.Admin.Events.Registrations.Forms
{
    public partial class Edit : AdminBasePage, IOverrideWebRouteParent, IHasParentLinkParameters
    {
        #region Properties

        private const string SearchUrl = "/ui/admin/events/registrations/search";

        private Guid RegistrationId => Guid.TryParse(Request.QueryString["id"], out var value) ? value : Guid.Empty;

        private Guid EventId
        {
            get => (Guid)(ViewState[nameof(EventId)] ?? Guid.Empty);
            set => ViewState[nameof(EventId)] = value;
        }

        private bool MultipleFormsAllowed
        {
            get => (bool)(ViewState[nameof(MultipleFormsAllowed)] ?? false);
            set => ViewState[nameof(MultipleFormsAllowed)] = value;
        }

        private bool HasExamForm
        {
            get => (bool)(ViewState[nameof(HasExamForm)] ?? false);
            set => ViewState[nameof(HasExamForm)] = value;
        }

        #endregion

        #region Classes

        private class FormItem
        {
            public Guid? FormIdentifier { get; set; }
            public Guid[] ExcludeFormIdentifiers { get; set; }
            public bool IsEligible { get; set; }
            public bool ShowNumber { get; set; }
            public int Number { get; set; }
            public string Password { get; set; }
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AttendanceStatus.AutoPostBack = true;
            AttendanceStatus.ValueChanged += AttendanceStatus_ValueChanged;

            Employer.AutoPostBack = true;
            Employer.ValueChanged += (s, a) => OnEmployerValueChanged();

            AccommodationTypeSelector.AutoPostBack = true;
            AccommodationTypeSelector.ValueChanged += (s, a) => UpdateAddAccommodationConfirm();

            AddAccommodationButton.Click += AddAccommodationButton_Click;

            AccommodationsRepeater.ItemCommand += AccommodationsRepeater_ItemCommand;

            FormRepeater.ItemDataBound += FormRepeater_ItemDataBound;

            SaveButton.Click += (s, a) => Save();
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

            MultipleFormsAllowed = Organization.Toolkits.Events?.AllowMultipleAssessmentForms == true;

            Open();

            CancelButton.NavigateUrl = GetParentUrl(null);
        }

        private void Open()
        {
            var includes = new List<Expression<Func<QRegistration, object>>>
            {
                x => x.Event,
                x => x.Candidate
            };

            if (MultipleFormsAllowed)
                includes.Add(x => x.ExamForms);

            var registration = ServiceLocator.RegistrationSearch.GetRegistration(RegistrationId, includes.ToArray());
            if (registration == null || registration.Event.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(
                Page,
                qualifier: $"{registration.Event.EventTitle} <span class='form-text'>for </span> {registration.Candidate.UserFullName}");

            Employer.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
            Employer.Filter.GroupType = GroupTypes.Employer;

            RegistrationRequestedBy.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
            RegistrationRequestedBy.Value = registration.RegistrationRequestedBy;

            ClassTitle.Text = $"<a href=\"/ui/admin/events/classes/outline?event={registration.Event.EventIdentifier}\">{registration.Event.EventTitle} </a>";
            CandidateName.Text = $"<a href=\"/ui/admin/contacts/people/edit?contact={registration.Candidate.UserIdentifier}\">{registration.Candidate.UserFullName}</a>";

            EventId = registration.EventIdentifier;
            MultipleFormsAllowed = Organization.Toolkits.Events?.AllowMultipleAssessmentForms == true;

            FormRepeater.DataSource = GetFormItems(registration);
            FormRepeater.DataBind();

            ApprovalStatus.Value = registration.ApprovalStatus;

            WorkBasedHoursToDate.ValueAsInt = registration.WorkBasedHoursToDate;
            RegistrationSequence.Text = registration.RegistrationSequence.HasValue ? registration.RegistrationSequence.ToString() : "None";
            RegistrationRequestedOn.Text = registration.RegistrationRequestedOn.FormatDateOnly(User.TimeZone, nullValue: "None");

            Employer.Value = registration.EmployerIdentifier.HasValue ? ServiceLocator.ContactSearch.GetGroup(registration.EmployerIdentifier.Value)?.GroupIdentifier : null;
            OnEmployerValueChanged();

            AttendanceStatus.Value = registration.AttendanceStatus;

            RegistrationComment.Text = registration.RegistrationComment;
            RegistrationFee.ValueAsDecimal = registration.RegistrationFee;
            BillingCodePanel.Visible = registration.Event.BillingCodeEnabled;
            BillingCode.Text = registration.BillingCode;
            IncludeInT2202.Checked = registration.IncludeInT2202;
            Score.ValueAsDecimal = registration.Score * 100m;

            SetLabels(registration.Event.EventType);

            HasExamForm = registration.ExamFormIdentifier.HasValue;

            BindAccommodations();

            Seat.LoadItems(
                ServiceLocator.EventSearch.GetSeats(registration.EventIdentifier),
                "SeatIdentifier", "SeatTitle");
            Seat.ValueAsGuid = registration.SeatIdentifier;

            var seat = registration.SeatIdentifier.HasValue ? ServiceLocator.EventSearch.GetSeat(registration.SeatIdentifier.Value) : null;
            var configuration = seat?.Configuration != null ? JsonConvert.DeserializeObject<SeatConfiguration>(seat.Configuration) : null;

            PriceOptionField.Visible = false;

            if (configuration?.Prices != null && configuration.Prices.Count > 1)
            {
                var price = configuration.Prices.Find(x => x.Amount == registration.RegistrationFee);

                if (price != null)
                {
                    PriceOptionField.Visible = true;
                    PriceOption.Text = price.Name;
                }
            }

            var isMoved = registration.ApprovalStatus.HasValue() && registration.ApprovalStatus == "Moved";

            PaidLabel.Visible = registration.PaymentIdentifier.HasValue && !isMoved;
            MovedLabel.Visible = isMoved;

            if (registration.PaymentIdentifier.HasValue)
            {
                var payment = ServiceLocator.PaymentSearch.GetPayment(registration.PaymentIdentifier.Value);
                var invoice = payment != null ? ServiceLocator.InvoiceSearch.GetInvoice(payment.InvoiceIdentifier) : null;

                Seat.Enabled = invoice?.ReferencedInvoiceIdentifier == null;
                RegistrationFee.Enabled = invoice?.ReferencedInvoiceIdentifier == null;

                if (invoice != null && invoice.InvoiceNumber.HasValue)
                {
                    InvoiceNumberSection.Visible = true;
                    InvoiceNumber.Text = $"<a href=\"/ui/admin/sales/invoices/outline?id={invoice.InvoiceIdentifier}\">{invoice.InvoiceNumber}</a>";
                }
            }

            var returnUrl = new ReturnUrl("id");

            CardButton.NavigateUrl = returnUrl.GetRedirectUrl($"/ui/admin/registrations/classes/card?id={registration.RegistrationIdentifier}");
            MoveButton.NavigateUrl = $"/ui/admin/registrations/classes/move?id={registration.RegistrationIdentifier}";
            MoveButton.Visible = !string.Equals(registration.GradingStatus, "Cancelled", StringComparison.OrdinalIgnoreCase);
            HistoryButton.NavigateUrl = AggregateOutline.GetUrl(registration.RegistrationIdentifier, $"/ui/admin/registrations/classes/edit?id={registration.RegistrationIdentifier}");

            var showForms = ServiceLocator.EventSearch.CountEventAssessmentForms(new QEventAssessmentFormFilter { EventIdentifier = registration.EventIdentifier }) > 0;

            FormRepeater.Visible = showForms;

            SaveButton.Visible = CanEdit;
        }

        private List<FormItem> GetFormItems(QRegistration registration)
        {
            var items = new List<FormItem>
            {
                new FormItem
                {
                    FormIdentifier = registration.ExamFormIdentifier,
                    Password = registration.RegistrationPassword,
                    IsEligible = StringHelper.Equals(registration.EligibilityStatus, "Eligible")
                }
            };

            if (MultipleFormsAllowed && registration.ExamFormIdentifier.HasValue)
            {
                foreach (var examForm in registration.ExamForms.OrderBy(x => x.Sequence))
                {
                    items.Add(new FormItem
                    {
                        FormIdentifier = examForm.ExamFormIdentifier,
                        Password = examForm.Password,
                        IsEligible = StringHelper.Equals(examForm.EligibilityStatus, "Eligible")
                    });
                }
            }

            var assigned = items
                .Where(x => x.FormIdentifier.HasValue)
                .Select(x => x.FormIdentifier.Value)
                .ToArray();

            var number = 0;

            foreach (var item in items)
            {
                item.Number = ++number;
                item.ShowNumber = items.Count > 1;
                item.ExcludeFormIdentifiers = assigned.Where(x => x != item.FormIdentifier).ToArray();
            }

            return items;
        }

        private void SetLabels(string eventType)
        {
            EventTitleLabel.InnerText = $"{eventType} Event Title";
            ScoreHelpBlock.InnerText = $"The total score assigned to the registrant for this {eventType} event.";
        }

        #endregion

        #region Event handlers

        private void AttendanceStatus_ValueChanged(object sender, ComboBoxValueChangedEventArgs e)
        {
            if (!StringHelper.Equals(AttendanceStatus.Value, "Withdrawn/Cancelled"))
                return;

            IncludeInT2202.Checked = false;
            RegistrationFeeUpdatePanel.Update();
        }

        private void OnEmployerValueChanged()
        {
            var groupId = Employer.Value;

            if (!groupId.HasValue)
            {
                EmployerStatus.Visible = false;
                EmployerStatus.InnerText = string.Empty;
                return;
            }

            var group = ServiceLocator.GroupSearch.GetGroup(groupId.Value);
            var statusId = group?.GroupStatusItemIdentifier;

            var employerStatus = TCollectionItemCache.GetName(statusId);

            EmployerStatus.Visible = employerStatus.HasValue();
            EmployerStatus.InnerText = employerStatus ?? string.Empty;
        }

        private void FormRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var item = (FormItem)e.Item.DataItem;

            var formIdentifier = (FindBankForm)e.Item.FindControl("FormIdentifier");
            formIdentifier.Filter.EventIdentifier = EventId;
            formIdentifier.Filter.ExcludeFormIdentifiers = item.ExcludeFormIdentifiers;
            formIdentifier.Value = item.FormIdentifier;
            formIdentifier.Enabled = !item.IsEligible;
            formIdentifier.AllowClear = !item.IsEligible;

            if (item.IsEligible)
            {
                var examForm = item.FormIdentifier.HasValue
                    ? ServiceLocator.BankSearch.GetForm(item.FormIdentifier.Value)
                    : null;

                if (examForm != null && examForm.FormCode.IsNotEmpty())
                {
                    formIdentifier.Enabled = true;
                    formIdentifier.Filter.FormCode = examForm.FormCode;
                }
            }
        }

        #endregion

        #region Methods (save)

        private void Save()
        {
            if (!IsValid)
                return;

            var registration = ServiceLocator.RegistrationSearch.GetRegistration(RegistrationId, x => x.ExamForms);
            var commands = CreateCommands(registration);

            ServiceLocator.SendCommands(commands);

            ProcessInvitation(registration);

            var url = GetParentUrl(null);

            HttpResponseHelper.Redirect(url);
        }

        private void ProcessInvitation(QRegistration registration)
        {
            if (string.Equals(registration.ApprovalStatus, ApprovalStatus.Value))
                return;

            if (string.Equals(ApprovalStatus.Value, "Invitation Sent", StringComparison.OrdinalIgnoreCase))
            {
                RegistrationInvitationHelper.SendInvitation(
                    RegistrationId,
                    User.UserIdentifier,
                    false,
                    ServiceLocator.SendCommand,
                    ServiceLocator.RegistrationSearch
                );
            }
            else if (string.Equals(ApprovalStatus.Value, "Invitation Declined", StringComparison.OrdinalIgnoreCase))
            {
                RegistrationInvitationHelper.DeleteInvitation(
                    registration.RegistrationIdentifier,
                    ServiceLocator.AppSettings.Partition.Domain,
                    ServiceLocator.AppSettings.Environment,
                    ServiceLocator.RegistrationSearch,
                    ServiceLocator.AlertMailer
                );
            }
        }

        private List<ICommand> CreateCommands(QRegistration registration)
        {
            var commands = new List<ICommand>();

            var employerIdentifier = Employer.Value;
            if (employerIdentifier != registration.EmployerIdentifier)
                commands.Add(new AssignEmployer(registration.RegistrationIdentifier, employerIdentifier));

            if (!string.Equals(registration.ApprovalStatus ?? string.Empty, ApprovalStatus.Value))
                commands.Add(new ChangeApproval(registration.RegistrationIdentifier, ApprovalStatus.Value, "Modified manually", null, registration.ApprovalStatus));

            if (!string.Equals(registration.RegistrationComment ?? string.Empty, RegistrationComment.Text))
                commands.Add(new CommentRegistration(registration.RegistrationIdentifier, RegistrationComment.Text));

            if (registration.RegistrationFee != RegistrationFee.ValueAsDecimal)
                commands.Add(new AssignRegistrationFee(registration.RegistrationIdentifier, RegistrationFee.ValueAsDecimal));

            if (!string.Equals(registration.AttendanceStatus ?? string.Empty, AttendanceStatus.Value))
                commands.Add(new TakeAttendance(registration.RegistrationIdentifier, AttendanceStatus.Value, null, null));

            var score = Score.ValueAsDecimal / 100m;
            if (registration.Score != score)
                commands.Add(new ChangeGrade(registration.RegistrationIdentifier, null, score));

            if (WorkBasedHoursToDate.ValueAsInt != registration.WorkBasedHoursToDate)
                commands.Add(new AssignRegistrationHoursWorked(registration.RegistrationIdentifier, WorkBasedHoursToDate.ValueAsInt));

            var seatIdentifier = Seat.ValueAsGuid;

            if (RegistrationFee.ValueAsDecimal != registration.RegistrationFee || seatIdentifier != registration.SeatIdentifier)
                commands.Add(new AssignSeat(registration.RegistrationIdentifier, seatIdentifier, RegistrationFee.ValueAsDecimal, registration.BillingCustomer));

            if (BillingCode.Text != registration.BillingCode)
                commands.Add(new ModifyRegistrationBillingCode(registration.RegistrationIdentifier, BillingCode.Text));

            if (registration.IncludeInT2202 != IncludeInT2202.Checked)
            {
                if (IncludeInT2202.Checked)
                    commands.Add(new IncludeRegistrationToT2202(registration.RegistrationIdentifier));
                else
                    commands.Add(new ExcludeRegistrationFromT2202(registration.RegistrationIdentifier));
            }

            if (registration.RegistrationRequestedBy != RegistrationRequestedBy.Value)
                commands.Add(new ModifyRegistrationRequestedBy(registration.RegistrationIdentifier, RegistrationRequestedBy.Value));

            AddFormCommands(registration, commands);

            return commands;
        }

        private void AddFormCommands(QRegistration registration, List<ICommand> commands)
        {
            var isFirst = true;
            var registrationId = registration.RegistrationIdentifier;

            foreach (RepeaterItem repeaterItem in FormRepeater.Items)
            {
                var previousFormId = GetPreviousForm(repeaterItem);
                var selectedFormId = ((FindBankForm)repeaterItem.FindControl("FormIdentifier")).Value;

                var wasEligible = StringHelper.Equals(GetEligibilityStatus(registration, previousFormId, isFirst), "Eligible");
                var isEligible = ((CheckBox)repeaterItem.FindControl("IsEligible")).Checked;

                if (previousFormId != selectedFormId)
                {
                    AddExamFormCommand(commands, registrationId, previousFormId, selectedFormId, wasEligible);

                    if (MultipleFormsAllowed && selectedFormId != null)
                        commands.Add(new LimitExamTime(registrationId, selectedFormId.Value));
                    else
                        commands.Add(new LimitExamTime(registrationId));
                }

                var formId = selectedFormId ?? previousFormId;

                if (wasEligible != isEligible)
                {
                    var status = isEligible
                        ? RegistrationEligibility.Eligible
                        : RegistrationEligibility.NotEligible;

                    if (MultipleFormsAllowed && formId != null)
                        commands.Add(new ChangeEligibility(registrationId, formId.Value, status, null));
                    else
                        commands.Add(new ChangeEligibility(registrationId, status, null));
                }

                AddPasswordCommand(registration, commands, repeaterItem, isFirst, previousFormId, selectedFormId);

                isFirst = false;
            }
        }

        private void AddExamFormCommand(List<ICommand> commands, Guid registrationId, Guid? previousFormId, Guid? selectedFormId, bool wasEligible)
        {
            if (MultipleFormsAllowed)
            {
                if (previousFormId == null)
                    commands.Add(new AddExamForm(registrationId, Guid.NewGuid(), selectedFormId.Value));
                else if (selectedFormId != null)
                    commands.Add(new ChangeExamForm(registrationId, previousFormId.Value, selectedFormId.Value));
                else if (!wasEligible)
                    commands.Add(new RemoveExamForm(registrationId, previousFormId.Value));
            }
            else
            {
                if (selectedFormId != null)
                    commands.Add(new AssignExamForm(registrationId, selectedFormId.Value, previousFormId));
                else if (!wasEligible)
                    commands.Add(new UnassignExamForm(registrationId));
            }
        }

        private void AddPasswordCommand(
            QRegistration registration, List<ICommand> commands, RepeaterItem repeaterItem,
            bool isFirst, Guid? previousFormId, Guid? selectedFormId)
        {
            var password = ((ITextBox)repeaterItem.FindControl("RegistrationPassword")).Text.Trim();
            if (password.IsEmpty())
                return;

            if (isFirst)
            {
                if (password != registration.RegistrationPassword)
                    commands.Add(new ChangeRegistrationPassword(registration.RegistrationIdentifier, password));

                return;
            }

            if (selectedFormId == null)
                return;

            var examForm = GetExamForm(registration, previousFormId);

            if (password != examForm?.Password)
                commands.Add(new ChangeExamFormPassword(registration.RegistrationIdentifier, selectedFormId.Value, password));
        }

        private static Guid? GetPreviousForm(RepeaterItem repeaterItem)
        {
            var value = ((System.Web.UI.WebControls.Literal)repeaterItem.FindControl("OriginalFormIdentifier")).Text;

            return Guid.TryParse(value, out var formId) ? formId : (Guid?)null;
        }

        private static string GetEligibilityStatus(QRegistration registration, Guid? formId, bool isFirst) =>
            isFirst ? registration.EligibilityStatus : GetExamForm(registration, formId)?.EligibilityStatus;

        private static QRegistrationExamForm GetExamForm(QRegistration registration, Guid? formId) =>
            formId.HasValue
                ? registration.ExamForms.FirstOrDefault(x => x.ExamFormIdentifier == formId.Value)
                : null;

        #endregion

        #region Methods (navigation back)

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent) =>
            GetParentLinkParameters(parent, null);

        #endregion

        #region Methods (accommodations)

        private static readonly Regex AccommodationTypeTimePattern =
            new Regex("\\(\\+(?:(?:(?<Hours>\\d+(?:\\.\\d+)?)hr)|(?:(?<Minutes>\\d+)min))\\)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private void AddAccommodationButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            AddAccommodation(AccommodationTypeSelector.Value, AccommodationTypeSelector.Value);

            BindAccommodations();
            AccommodationTypeSelector.Value = null;
            UpdateAddAccommodationConfirm();
        }

        private void AccommodationsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                DeleteAccommodation((string)e.CommandArgument);
                BindAccommodations();
            }
        }

        private int GetTimeExtension(string type, out string resolvedType)
        {
            resolvedType = type;

            if (type.IsEmpty())
                return 0;

            if (type.Length > 50)
                resolvedType = type = type.Substring(0, 50);

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
                resolvedType = item.ItemName;

                if (item.ItemHours.HasValue)
                    timeExtension = (int)(item.ItemHours * 60);
            }

            return timeExtension;
        }

        private void UpdateAddAccommodationConfirm()
        {
            var type = AccommodationTypeSelector.Value;
            var isTimeExtension = type.IsNotEmpty() && GetTimeExtension(type, out _) > 0;

            AddAccommodationButton.ConfirmText = !HasExamForm && isTimeExtension
                ? "There's no form attached to this registration. Are you sure you want to add a time accommodation?"
                : null;
        }

        public void AddAccommodation(string type, string name)
        {
            if (type.IsEmpty())
                return;

            var timeExtension = GetTimeExtension(type, out var resolvedType);

            ServiceLocator.SendCommand(new GrantAccommodation(RegistrationId, resolvedType, name, timeExtension));
            ServiceLocator.SendCommand(new LimitExamTime(RegistrationId));
        }

        public void BindAccommodations()
        {
            var accommodations = ServiceLocator.RegistrationSearch.GetAccommodations(RegistrationId);

            AccommodationField.Visible = accommodations.Count > 0;

            if (accommodations.Count > 0)
            {
                AccommodationsRepeater.DataSource = accommodations;
                AccommodationsRepeater.DataBind();
            }

            var allTypes = ServiceLocator.RegistrationSearch.GetAccommodationTypes(Organization.OrganizationIdentifier);
            AccommodationTypeSelector.Settings.AdditionalOptions = allTypes;
            AccommodationTypeSelector.RefreshData();

            var showTimeExtWarning = !HasExamForm && accommodations.Any(x => (x.TimeExtension ?? 0) > 0);

            TimeExtensionWarning1.Visible = showTimeExtWarning;
            TimeExtensionWarning2.Visible = showTimeExtWarning;
        }

        public void DeleteAccommodation(string type)
        {
            ServiceLocator.SendCommand(new RevokeAccommodation(RegistrationId, type));
            ServiceLocator.SendCommand(new LimitExamTime(RegistrationId));
        }

        #endregion
    }
}

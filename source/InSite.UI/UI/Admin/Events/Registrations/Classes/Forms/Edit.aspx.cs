using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Events.Read;
using InSite.Application.Registrations.Read;
using InSite.Application.Registrations.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

using AggregateOutline = InSite.Admin.Logs.Aggregates.Outline;

namespace InSite.Admin.Events.Registrations.Forms
{
    public partial class Edit : AdminBasePage, IOverrideWebRouteParent, IHasParentLinkParameters
    {
        #region Properties

        private const string SearchUrl = "/ui/admin/events/registrations/search";

        private Guid RegistrationId => Guid.TryParse(Request.QueryString["id"], out var value) ? value : Guid.Empty;

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
            AddAccommodationButton.Click += AddAccommodationButton_Click;
            AccommodationsRepeater.ItemCommand += AccommodationsRepeater_ItemCommand;

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

            Open();

            CancelButton.NavigateUrl = GetParentUrl(null);
        }

        private void Open()
        {
            var registration = ServiceLocator.RegistrationSearch.GetRegistration(RegistrationId, x => x.Event, x => x.Candidate);
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

            var isEligible = StringHelper.Equals(registration.EligibilityStatus, "Eligible");

            IsEligible.Checked = isEligible;
            IsEligible.Enabled = isEligible;

            FormIdentifier.Filter.EventIdentifier = registration.EventIdentifier;
            FormIdentifier.Value = registration.ExamFormIdentifier;
            FormIdentifier.Enabled = !isEligible;
            FormIdentifier.AllowClear = !isEligible;

            if (isEligible)
            {
                var examForm = registration.ExamFormIdentifier.HasValue
                    ? ServiceLocator.BankSearch.GetForm(registration.ExamFormIdentifier.Value)
                    : null;

                if (examForm != null && examForm.FormCode.IsNotEmpty())
                {
                    FormIdentifier.Enabled = true;
                    FormIdentifier.Filter.FormCode = examForm.FormCode;
                }
            }

            RegistrationPassword.Text = registration.RegistrationPassword;
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

            FormField.Visible = showForms;
            RegistrationPasswordField.Visible = showForms;
            AccommodationCard.Visible = showForms;

            SaveButton.Visible = CanEdit;
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

        #endregion

        #region Methods (save)

        private void Save()
        {
            if (!IsValid)
                return;

            var registration = ServiceLocator.RegistrationSearch.GetRegistration(RegistrationId);
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
                    ServiceLocator.AppSettings.Security.Domain,
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
            AddRegistrationPasswordCommand(registration, commands);

            return commands;
        }

        private void AddFormCommands(QRegistration registration, List<ICommand> commands)
        {
            var registrationId = registration.RegistrationIdentifier;
            var isEligible = registration.EligibilityStatus == "Eligible";
            var previousFormId = registration.ExamFormIdentifier;
            var selectedFormId = FormIdentifier.Value;

            if (previousFormId != selectedFormId)
            {
                if (selectedFormId != null)
                    commands.Add(new AssignExamForm(registrationId, selectedFormId.Value, previousFormId));
                else if (!isEligible)
                    commands.Add(new UnassignExamForm(registrationId));

                commands.Add(new LimitExamTime(registrationId));
            }

            if (isEligible && !IsEligible.Checked)
                commands.Add(new ChangeEligibility(registrationId, "Not Eligible", null));
            else if (!isEligible && IsEligible.Checked)
                commands.Add(new ChangeEligibility(registrationId, "Eligible", null));
        }

        private void AddRegistrationPasswordCommand(QRegistration registration, List<ICommand> commands)
        {
            var password = RegistrationPassword.Text.Trim();

            if (string.IsNullOrEmpty(password) || string.Equals(password, registration.RegistrationPassword))
                return;

            commands.Add(new ChangeRegistrationPassword(registration.RegistrationIdentifier, password));
        }

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
        }

        private void AccommodationsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                DeleteAccommodation((string)e.CommandArgument);
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

            ServiceLocator.SendCommand(new GrantAccommodation(RegistrationId, type, name, timeExtension));
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
            AccommodationTypeSelector.AdditionalOptions = allTypes;
            AccommodationTypeSelector.RefreshData();
        }

        public void DeleteAccommodation(string type)
        {
            ServiceLocator.SendCommand(new RevokeAccommodation(RegistrationId, type));
            ServiceLocator.SendCommand(new LimitExamTime(RegistrationId));
        }


        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Contacts.Read;
using InSite.Application.Events.Read;
using InSite.Application.Gateways.Write;
using InSite.Application.Groups.Write;
using InSite.Application.Invoices.Read;
using InSite.Application.Invoices.Write;
using InSite.Application.Registrations.Write;
using InSite.Common;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Contacts;
using InSite.Domain.Events;
using InSite.Domain.Invoices;
using InSite.Domain.Messages;
using InSite.Domain.Payments;
using InSite.Domain.Registrations;
using InSite.Persistence;
using InSite.UI.Admin.Events.Classes.Controls;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;
using InSite.UI.Portal.Billing.Utilities;
using InSite.UI.Portal.Events.Classes.Models;
using InSite.Web.Data;
using InSite.Web.Helpers;
using InSite.Web.Security;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Events.Classes
{
    public partial class Register : PortalBasePage
    {
        #region Classes

        private class SavedUser
        {
            public QUser User { get; set; }
            public RegistrantChangedField[] ChangedFields { get; set; }
        }

        private class StepInfo
        {
            public int Number { get; }
            public string Title { get; }
            public bool IsCompleted { get; }

            public StepInfo(int number, string title, bool isCompleted)
            {
                Number = number;
                Title = title;
                IsCompleted = isCompleted;
            }
        }

        #endregion

        #region Properties

        private Guid? EventIdentifier => Guid.TryParse(Request["event"], out var id) ? id : (Guid?)null;

        private Guid? ProvidedCandidateIdentifier => Guid.TryParse(Request["candidate"], out var id) ? id : (Guid?)null;

        private bool RegisterEmployee => Request["registeremployee"] == "1";

        private Guid? ActualCandidateIdentifier => ProvidedCandidateIdentifier == null && !RegisterEmployee ? User.UserIdentifier : ProvidedCandidateIdentifier;

        private Guid? PaymentIdentifier
        {
            get => (Guid?)ViewState[nameof(PaymentIdentifier)];
            set => ViewState[nameof(PaymentIdentifier)] = value;
        }

        private Guid? InvoiceIdentifier;

        private bool? _fullAccess;
        private bool FullAccess
        {
            get
            {
                if (_fullAccess == null)
                {
                    var action = TActionSearch.Get("ui/admin/events/classes/outline");
                    _fullAccess = CurrentSessionState.Identity.IsGranted(action.PermissionParentActionIdentifier);
                }

                return _fullAccess.Value;
            }
        }

        private bool IsAdminOutline => Request["adminoutline"] == "1";

        private int StepNumber => ViewState[nameof(StepNumber)] as int? ?? 1;
        private void SetStepNumber(int stepNumber)
        {
            ViewState[nameof(StepNumber)] = stepNumber;

            var stepList = new List<StepInfo>();

            AddStep(1, ActualCandidateIdentifier == User.UserIdentifier ? "Update your account information" : "Enter participant");

            if (IsEmployerPanelVisible)
                AddStep(2, "Select employer");

            AddStep(3, "Seat Selection");
            AddStep(4, "Enter payment info");
            AddStep(5, "Confirm");

            TopProgressBar.DataSource = stepList;
            TopProgressBar.DataBind();

            void AddStep(int number, string stepTitle)
            {
                stepList.Add(new StepInfo(stepList.Count + 1, Translate(stepTitle), stepNumber >= number));
            }
        }

        private decimal PaymentAmount
        {
            get => ViewState[nameof(PaymentAmount)] as decimal? ?? 0;
            set => ViewState[nameof(PaymentAmount)] = value;
        }

        private UnmaskedCreditCard CreditCard
        {
            get => (UnmaskedCreditCard)ViewState[nameof(CreditCard)];
            set => ViewState[nameof(CreditCard)] = value;
        }

        private string BillingCode
        {
            get => (string)ViewState[nameof(BillingCode)];
            set => ViewState[nameof(BillingCode)] = value;
        }

        private bool IsInvited
        {
            get => (bool)ViewState[nameof(IsInvited)];
            set => ViewState[nameof(IsInvited)] = value;
        }

        /// <remarks>
        /// If the URL explicitly declares that this is an invitation link, then the availability check can be skipped.
        /// </remarks>
        private bool IsExplicitInvitation
            => StringHelper.Equals(Request.QueryString["invitation"], "1");

        private bool IsEmployerPanelVisible
        {
            get => (bool)ViewState[nameof(IsEmployerPanelVisible)];
            set => ViewState[nameof(IsEmployerPanelVisible)] = value;
        }

        #endregion

        #region Fields

        private static readonly PortalFieldHandler<AccordionPanel> _fieldHandler = new PortalFieldHandler<AccordionPanel>();

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            BackButton1.Click += BackButton1_Click;
            BackButton2.Click += BackButton2_Click;
            BackButton3.Click += BackButton3_Click;
            BackButton4.Click += BackButton3_Click;

            NextButton1.Click += NextButton1_Click;
            NextButton2.Click += NextButton2_Click;
            IAgreeButton.Click += IAgreeButton_Click;
            NextButton3.Click += NextButton3_Click;
            NextButton4.Click += NextButton4_Click;

            SaveButton.Click += SaveButton_Click;

            EmployerGroupIdentifier.AutoPostBack = true;
            EmployerGroupIdentifier.ValueChanged += EmployerGroupIdentifier_ValueChanged;

            ChangeSeatButton.Click += ChangeSeatButton_Click;

            MultiplePrice.AutoPostBack = true;
            MultiplePrice.SelectedIndexChanged += MultiplePrice_SelectedIndexChanged;

            Agreed.AutoPostBack = true;
            Agreed.CheckedChanged += Agreed_CheckedChanged;

            CompanyTypeExisting.AutoPostBack = true;
            CompanyTypeExisting.CheckedChanged += CompanyType_CheckedChanged;

            CompanyTypeNew.AutoPostBack = true;
            CompanyTypeNew.CheckedChanged += CompanyType_CheckedChanged;

            NewCompanyContactType.AutoPostBack = true;
            NewCompanyContactType.SelectedIndexChanged += NewCompanyContactType_SelectedIndexChanged;

            SeatRequiredValidator.ServerValidate += SeatRequiredValidator_ServerValidate;
            SeatRequiredValidator2.ServerValidate += SeatRequiredValidator_ServerValidate;

            EmailUniqueValidator.ServerValidate += EmailUniqueValidator_ServerValidate;
            PersonCodeUniqueValidator.ServerValidate += PersonCodeUniqueValidator_ServerValidate;

            AvailabilityValidator.ServerValidate += AvailabilityValidator_ServerValidate;

            PersonCodeIdAvailability.AutoPostBack = true;
            PersonCodeIdAvailability.CheckedChanged += TradeworkerIdAvailability_CheckedChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Page.MaintainScrollPositionOnPostBack = false;

            if (IsPostBack)
                return;

            LoadData();
            SetStepNumber(1);
        }

        #endregion

        #region Event handlers

        private void BackButton1_Click(object sender, EventArgs e)
        {
            PersonDetailsPanel.Visible = PersonDetailsPanel.IsSelected = true;
            EmployerPanel.Visible = false;
            SeatPanel.Visible = false;
            PaymentSection.Visible = false;
            ConfirmSection.Visible = false;
        }

        private void BackButton2_Click(object sender, EventArgs e)
        {
            if (!IsEmployerPanelVisible)
            {
                BackButton1_Click(sender, e);
                return;
            }

            EmployerPanel.Visible = EmployerPanel.IsSelected = true;
            SeatPanel.Visible = false;
            PaymentSection.Visible = false;
            ConfirmSection.Visible = false;
        }

        private void BackButton3_Click(object sender, EventArgs e)
        {
            SeatPanel.Visible = SeatPanel.IsSelected = true;
            PaymentSection.Visible = false;
            ConfirmSection.Visible = false;
        }

        private void NextButton1_Click(object sender, EventArgs e)
        {
            if (!IsEmployerPanelVisible)
            {
                NextButton2_Click(sender, e);
                return;
            }

            if (!Page.IsValid)
                return;

            EmployerPanel.Visible = EmployerPanel.IsSelected = true;

            ShowCompanyPanel();

            if (StepNumber < 2)
                SetStepNumber(2);
        }

        private void NextButton2_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            PaymentSection.Visible = false;
            ConfirmSection.Visible = false;
            SeatPanel.Visible = true;

            SeatPanel.IsSelected = true;

            if (!string.IsNullOrEmpty(SelectedSeat.Value))
                ShowSeatDetails(null, null, null);

            SetStepNumber(3);
        }

        private void IAgreeButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            Agreed.Checked = true;
            NextButton3_Click(sender, e);
        }

        private void NextButton3_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            var price = SinglePrice.Visible
                ? decimal.Parse(SinglePrice.Text, NumberStyles.Currency)
                : (MultiplePrice.Visible ? decimal.Parse(MultiplePrice.SelectedValue) : 0);

            if (price == 0)
            {
                CreditCardPanel.Visible = false;

                // Show Cart details logic...

                ShowConfirmationDetails();

                SetStepNumber(5);
            }
            else
            {
                ConfirmSection.Visible = false;
                PaymentSection.Visible = PaymentSection.IsSelected = true;

                PaymentDetail.Visible = true;
                PaymentDetail.SetInputValues(price, true);

                NextButton4.Enabled = true;

                SetStepNumber(4);
            }
        }

        private void NextButton4_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var payment = PaymentDetail.GetInputValues();

            if (payment.Card != null)
            {
                if (!payment.Card.IsValid)
                {
                    PaymentAlert.AddMessage(AlertType.Error, payment.Card.ErrorMessage);
                    return;
                }

                BillingCode = null;
                BillingCodePanel.Visible = false;
                BillingCodeConfirmation.InnerText = null;

                CreditCard = payment.Card;
                CreditCardPanel.Visible = true;
                CardDetailConfirm.SetInputValues(payment.Card);
            }
            else if (payment.BillingCode.IsNotEmpty())
            {
                BillingCode = payment.BillingCode;
                BillingCodePanel.Visible = true;
                BillingCodeConfirmation.InnerText = payment.BillingCode;

                CreditCard = null;
                CreditCardPanel.Visible = false;
            }
            else
            {
                PaymentAlert.AddMessage(AlertType.Error, "Unexpected payment type");
                return;
            }

            PaymentAmount = payment.PaymentAmount;
            PaymentAmountLiteral.Text = payment.PaymentAmount.ToString("n2");

            ShowConfirmationDetails();

            SetStepNumber(5);

            PaymentIdentifier = UniqueIdentifier.Create();

            PaymentSection.Visible = false;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var @event = ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value, x => x.Registrations);

            if (IsRegistered(@event))
            {
                HttpResponseHelper.Redirect(GetOutlineLink());
                return;
            }

            if (CreditCardPanel.Visible)
            {
                if (ServiceLocator.PaymentSearch.GetPayment(PaymentIdentifier.Value) != null)
                {
                    StatusAlert.AddMessage(AlertType.Error, Translate("The same payment information cannot be re-submitted."));
                }
                else
                {
                    StartPayment(@event);

                    if (CheckPaymentStatus())
                    {
                        var registrationId = SaveData(@event);
                        var url = $"/ui/portal/events/classes/register-success?registration={registrationId}";

                        if (IsAdminOutline)
                            url += "&adminoutline=1";

                        HttpResponseHelper.Redirect(url);
                    }
                }
            }
            else
            {
                var registrationId = SaveData(@event);

                if (BillingCodePanel.Visible && BillingCode.IsNotEmpty())
                    ServiceLocator.SendCommand(new ModifyRegistrationBillingCode(registrationId, BillingCode));

                HttpResponseHelper.Redirect(GetNavigateBackLink());
            }
        }

        private void Agreed_CheckedChanged(object sender, EventArgs e) => OnAgreedCheckedChanged();

        private void TradeworkerIdAvailability_CheckedChanged(object sender, EventArgs e) => OnTradeworkerIdAvailabilityCheckedChanged();

        private void OnAgreedCheckedChanged()
        {
            NextButton3.Enabled = Agreed.Checked;

            PaymentSection.Visible = false;
            ConfirmSection.Visible = false;
        }

        private void OnTradeworkerIdAvailabilityCheckedChanged()
        {
            PersonCode.Enabled = !PersonCodeIdAvailability.Checked;
            PersonCodeRequiredValidator.Visible = !PersonCodeIdAvailability.Checked;
        }

        private void EmployerGroupIdentifier_ValueChanged(object sender, EventArgs e)
        {
            BindCompanyDetails();
        }

        private void MultiplePrice_SelectedIndexChanged(object sender, EventArgs e)
        {
            Agreement.Visible = true;
        }

        private void ChangeSeatButton_Click(object sender, EventArgs e)
        {
            ShowSeatDetails(null, null, null);
        }

        private void CompanyType_CheckedChanged(object sender, EventArgs e)
        {
            ShowCompanyPanel();
        }

        private void NewCompanyContactType_SelectedIndexChanged(object sender, EventArgs e)
        {
            NewCompanyContactFields.Visible = NewCompanyContactType.SelectedValue == "New";
        }

        private void SeatRequiredValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = !string.IsNullOrEmpty(SelectedSeat.Value);

            if (!args.IsValid)
                return;

            var seatIdentifier = Guid.Parse(SelectedSeat.Value);
            var seat = ServiceLocator.EventSearch.GetSeat(seatIdentifier);

            try
            {
                GetSeatPrice(seat);
            }
            catch (FormatException)
            {
                args.IsValid = false;
            }
        }

        private void EmailUniqueValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ActualCandidateIdentifier.HasValue
                || !ServiceLocator.UserSearch.IsUserExist(Email.Text);
        }

        private void PersonCodeUniqueValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (PersonCode.Text.Trim().Length == 0)
                return;

            if (PersonCodeIdAvailability.Checked)
                return;

            if (ActualCandidateIdentifier.HasValue)
            {
                args.IsValid = !PersonCriteria.Exists(new PersonFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    ExcludeUserIdentifiers = new[] { ActualCandidateIdentifier.Value },
                    CodeExact = PersonCode.Text.Trim()
                });

                if (args.IsValid)
                    return;
            }
            else
            {
                args.IsValid = !PersonCriteria.Exists(new PersonFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    CodeExact = PersonCode.Text.Trim()
                });

                if (args.IsValid)
                    return;
            }

            PersonCodeUniqueValidator.ErrorMessage = $"Learner ID entered is assigned to another user; you may proceed with this registration without a Learner ID number by selecting the checkbox below this field";
        }

        private void AvailabilityValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (IsExplicitInvitation || IsInvited)
                return;

            var @event = ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value, x => x.Registrations);

            var availableCount = @event.CapacityMaximum.HasValue && @event.CapacityMaximum > 0
                ? @event.CapacityMaximum - @event.ConfirmedRegisteredCount
                : null;

            args.IsValid = availableCount == null || availableCount > 0;

            if (args.IsValid)
                return;

            AvailabilityValidator.ErrorMessage = $"There are no available seats in this class";
        }

        #endregion

        #region Payment

        private void StartPayment(QEvent @event)
        {
            InvoiceIdentifier = UniqueIdentifier.Create();
            var invoiceNumber = Sequence.Increment(@event.OrganizationIdentifier, SequenceType.Invoice);

            var paymentAmount = PaymentAmount;
            var card = CreditCard;
            var productIdentifier = GetOrCreateProduct(@event);
            var paymentDescription = $"{@event.EventTitle} activity registration for {FirstName.Text} {LastName.Text}";

            ServiceLocator.SendCommand(new DraftInvoice(InvoiceIdentifier.Value, Organization.Identifier, invoiceNumber, User.UserIdentifier, new[]
            {
                new InvoiceItem
                {
                    Identifier = UuidFactory.Create(),
                    Product = productIdentifier,
                    Quantity = 1,
                    Price = paymentAmount,
                    Description = paymentDescription
                }
            }));
            ServiceLocator.SendCommand(new SubmitInvoice(InvoiceIdentifier.Value));

            var startPaymentCommand = new StartPayment(
                PaymentIdentifiers.BamboraGateway,
                Organization.Identifier,
                (Guid)InvoiceIdentifier,
                PaymentIdentifier.Value,
                new PaymentInput(
                    Invoice.FormatInvoiceNumber(invoiceNumber),
                    paymentAmount,
                    new UnmaskedCreditCard
                    {
                        CardNumber = card.CardNumber,
                        CardholderName = card.CardholderName,
                        ExpiryMonth = card.ExpiryMonth,
                        ExpiryYear = card.ExpiryYear,
                        SecurityCode = card.SecurityCode
                    },
                    null,
                    Request.UserHostAddress
                )
            );

            ServiceLocator.SendCommand(startPaymentCommand);
        }

        private Guid GetOrCreateProduct(QEvent @event)
        {
            var seatIdentifier = Guid.Parse(SelectedSeat.Value);
            var product = ServiceLocator.InvoiceSearch.GetProduct(seatIdentifier);

            if (product == null)
            {
                var seat = ServiceLocator.EventSearch.GetSeat(seatIdentifier);

                product = new TProduct
                {
                    ProductIdentifier = seatIdentifier,
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    ProductType = "Seat",
                    ProductName = seat.SeatTitle,
                    ProductDescription = $"{@event.EventTitle}",
                    CreatedBy = User.Identifier,
                    ModifiedBy = User.Identifier
                };

                ServiceLocator.InvoiceStore.InsertProduct(product);
            }

            return seatIdentifier;
        }

        private bool CheckPaymentStatus()
        {
            var error = CheckPaymentStatus(PaymentIdentifier.Value);

            if (string.IsNullOrEmpty(error))
                return true;

            StatusAlert.AddMessage(AlertType.Error, error);

            return false;
        }

        #endregion

        #region Helper methods

        public static string CheckPaymentStatus(Guid paymentId)
        {
            var gateway = ServiceLocator.ChangeRepository.Get<GatewayAggregate>(PaymentIdentifiers.BamboraGateway);
            var payment = gateway.Data.FindPayment(paymentId);

            if (payment == null)
                return LabelHelper.GetTranslation("Sorry, your payment wasn't saved successfully, please contact support team.");

            switch (payment.Status)
            {
                case PaymentStatus.Completed:
                    return null;

                case PaymentStatus.Aborted:
                    return payment.Error.Message ?? LabelHelper.GetTranslation("Transaction aborted");

                case PaymentStatus.Started:
                    return LabelHelper.GetTranslation("Your payment started. Please check the payment status later.");

                default:
                    return LabelHelper.GetTranslation("Sorry, your payment is failed, please contact support team.");
            }
        }

        private void LoadData()
        {
            if (!Classes.RegisterEmployee.RegisterLinkIsVisible() && ActualCandidateIdentifier == null)
                NavigateToSearch();

            var @event = GetAndValidateEvent();

            if (ActualCandidateIdentifier.HasValue)
                RegistrationHelper.CheckMandatorySurveyResponse(@event, ActualCandidateIdentifier.Value);

            SetTextsAndHeader(@event);

            var cancelUrl = GetNavigateBackLink();

            CancelButton1.NavigateUrl = cancelUrl;
            CancelButton2.NavigateUrl = cancelUrl;
            CancelButton3.NavigateUrl = cancelUrl;
            CancelButton4.NavigateUrl = cancelUrl;
            CancelButton5.NavigateUrl = cancelUrl;

            NewCompanyProvinceSelector.RefreshData();
            NewCompanyProvinceSelector.Value = Organization.PlatformCustomization.TenantLocation.Province;

            CompanyOptions.Visible = !CompanySelectionDisabled();
            CompanyNameDisplay.Visible = CompanySelectionDisabled();

            PaymentDetail.Mode = @event.BillingCodeEnabled ? PaymentMode.BillTo : PaymentMode.Card;

            InitRegistration(@event);

            ShowRegistrationFields(@event);
        }

        private QEvent GetAndValidateEvent()
        {
            if (EventIdentifier == null)
                NavigateToSearch();

            var @event = ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value, x => x.Registrations, x => x.Achievement);

            if (@event == null || @event.OrganizationIdentifier != Organization.OrganizationIdentifier)
                NavigateToSearch();

            var allowedForRegistrationByAnyone = @event.AllowRegistrationWithLink.HasValue && @event.AllowRegistrationWithLink.Value;

            if (TGroupPermissionSearch.IsAccessDenied(EventIdentifier.Value, Identity) && !allowedForRegistrationByAnyone)
                HttpResponseHelper.Redirect("/ui/portal/events/classes/search");

            CheckInvited(@event);

            if (!HasAccess(@event) || !BindPerson() || IsRegistered(@event))
                HttpResponseHelper.Redirect(GetOutlineLink());

            return @event;
        }

        private void CheckInvited(QEvent @event)
        {
            var registration = @event.Registrations.FirstOrDefault(x => x.CandidateIdentifier == ActualCandidateIdentifier);

            IsInvited = registration != null
                && string.Equals(registration.ApprovalStatus, "Invitation Sent")
                && RegistrationInvitationHelper.IsInvitationValid(registration.RegistrationIdentifier, ServiceLocator.RegistrationSearch);
        }

        private void SetTextsAndHeader(QEvent @event)
        {
            PortalMaster.SidebarVisible(false);

            PageHelper.AutoBindHeader(this);

            var subtitle = $"{@event.EventScheduledStart.FormatDateOnly(User.TimeZone)}";
            if (@event.EventScheduledEnd.HasValue && @event.EventScheduledEnd.Value.Date != @event.EventScheduledStart.Date)
                subtitle += $" - {@event.EventScheduledEnd.Value.FormatDateOnly(User.TimeZone)}";

            PortalMaster.Breadcrumbs.BindTitleAndSubtitleNoTranslate(@event.EventTitle, subtitle);

            var helpText = ServiceLocator.ContentSearch.GetTooltipText("[Learner Number Help]", Organization.OrganizationIdentifier);
            if (!string.IsNullOrEmpty(helpText))
                PersonCodeHelpText.Text = helpText;

            var employerLabelText = ServiceLocator.ContentSearch.GetTooltipText("[Employer Panel Label]", Organization.OrganizationIdentifier);
            if (!string.IsNullOrEmpty(employerLabelText))
                EmployerPanel.Title = employerLabelText;
            else
                EmployerPanel.Title = "Employer";

            SeatPanel.Title = $"{@event.EventType} Registration";
        }

        private void InitRegistration(QEvent @event)
        {
            var registration = ActualCandidateIdentifier.HasValue ? @event.Registrations.FirstOrDefault(x => x.CandidateIdentifier == ActualCandidateIdentifier) : null;

            WorkBasedHoursToDate.ValueAsInt = registration?.WorkBasedHoursToDate;

            if (registration?.SeatIdentifier != null)
                ShowSeatDetails(registration.SeatIdentifier, registration.RegistrationFee, registration.BillingCustomer);
            else
                BindSeats(@event.EventIdentifier);
        }

        private void ShowRegistrationFields(QEvent @event)
        {
            IsEmployerPanelVisible = true;

            var organization = ServiceLocator.OrganizationSearch.GetModel(Organization.Identifier);
            var orgFields = organization.Fields.ClassRegistration;
            var regFields = organization.Toolkits.Events.AllowClassRegistrationFields
                ? @event.GetRegistrationFields()
                : new List<RegistrationField>();

            foreach (var defaultField in PortalFieldInfo.ClassRegistration)
            {
                var orgField = orgFields?.FirstOrDefault(x => x.FieldName == defaultField.FieldName);
                var regFieldName = defaultField.FieldName.ToEnum<RegistrationFieldName>();
                var regField = regFields?.FirstOrDefault(x => x.FieldName == regFieldName);

                var fieldData = _fieldHandler.Init(PersonDetailsPanel, defaultField, orgField, regField);

                if (regFieldName == RegistrationFieldName.EmployerPanel)
                    IsEmployerPanelVisible = fieldData.IsVisible;
            }

            if (PersonCodeRequiredValidator.Visible && !@event.PersonCodeIsRequired)
                PersonCodeRequiredValidator.Visible = false;

            PersonalGroup.Visible = true;
            PersonalGroup.Visible = FirstNameField.Visible
                || MiddleNameField.Visible
                || LastNameField.Visible
                || EmailField.Visible
                || BirthdateField.Visible
                || PersonCodeField.Visible
                || WorkBasedHoursToDateField.Visible
                || FirstLanguageField.Visible;

            AddressGroup.Visible = true;
            AddressGroup.Visible = Street1Field.Visible
                || CityField.Visible
                || CountryField.Visible
                || ProvinceField.Visible
                || PostalCodeField.Visible;

            EmergencyContactGroup.Visible = true;
            EmergencyContactGroup.Visible = EmergencyContactNameField.Visible
                || EmergencyContactPhoneField.Visible
                || EmergencyContactRelationshipField.Visible;

            PhoneNumbersGroup.Visible = true;
            PhoneNumbersGroup.Visible = PhoneField.Visible
                || PhoneHomeField.Visible
                || PhoneWorkField.Visible
                || PhoneMobileField.Visible
                || PhoneOtherField.Visible;

            PersonCodeIdAvailabilityField.Visible = PersonCodeField.Visible;
        }

        private bool IsRegistered(QEvent @event)
        {
            var registration = ActualCandidateIdentifier.HasValue
                ? @event.Registrations.FirstOrDefault(x => x.CandidateIdentifier == ActualCandidateIdentifier)
                : null;

            return registration != null && string.Equals(registration.ApprovalStatus, "Registered", StringComparison.OrdinalIgnoreCase);
        }

        private bool HasAccess(QEvent @event)
        {
            return FullAccess
                || IsInvited
                || IsExplicitInvitation
                || @event.Availability != EventAvailabilityType.Full && @event.Availability != EventAvailabilityType.Over;
        }

        private void ShowConfirmationDetails()
        {
            var @event = ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value, x => x.VenueLocation, x => x.VenueLocation);
            var eventContent = ContentEventClass.Deserialize(@event.Content);
            var classStartDate = @event.EventScheduledStart.FormatDateOnly(User.TimeZone);
            var classEndDate = @event.EventScheduledEnd.HasValue ? @event.EventScheduledEnd.Value.FormatDateOnly(User.TimeZone) : "N/A";
            var classCancellationPolicy = eventContent.Get(EventInstructionType.Cancellation.GetName())?.Default;
            var classCancellationPolicyText = !string.IsNullOrEmpty(classCancellationPolicy) ? Markdown.ToHtml(classCancellationPolicy) : null;

            var venueAddress = @event.VenueLocationIdentifier.HasValue ? ServiceLocator.GroupSearch.GetAddress(@event.VenueLocationIdentifier.Value, AddressType.Physical) : null;
            var venueAddressText = venueAddress != null ? ClassVenueAddressInfo.GetAddress(venueAddress) : "N/A";
            var participantName = MiddleName.Text.HasValue() ? $"{FirstName.Text} {MiddleName.Text} {LastName.Text}" : $"{FirstName.Text} {LastName.Text}";
            var participantAddress = GetParticipantAddress();

            var (employer, employerAddress) = GetEmployer();
            var employerAddressText = employerAddress != null ? ClassVenueAddressInfo.GetAddress(employerAddress) : "N/A";

            var seatIdentifier = Guid.Parse(SelectedSeat.Value);
            var seat = ServiceLocator.EventSearch.GetSeat(seatIdentifier);
            var seatContent = ContentSeat.Deserialize(seat.Content);
            var seatAgreement = seatContent.GetAgreementHtml();

            ConfirmClassName.Text = @event.EventTitle;
            ConfirmClassDates.Text = $"{classStartDate} - {classEndDate}";
            ConfirmClassVenue.Text = @event.VenueLocationName ?? "N/A";
            ConfirmClassVenueAddress.Text = venueAddressText;

            ConfirmClassVenueAddress.NavigateUrl = venueAddress?.GetGMapsAddressLink();

            ConfirmParticipantName.Text = participantName;
            ConfirmParticipantEmail.Text = Email.Text;
            ConfirmParticipantAddress.Text = participantAddress;

            ConfirmParticipantAddress.NavigateUrl = LocationHelper.GetGMapsAddressLink(Street1.Text, null, City.Text, Province.Value, Country.Value, PostalCode.Text);

            ConfirmEmployerName.Text = employer?.GroupName ?? "N/A";
            ConfirmEmployerEmail.Text = EmployerContactEmail.Text;
            ComfirmEmployerEmailDiv.Visible = EmployerContactColumn.Visible;
            ComfirmEmployerEmailDiv.Attributes["class"] = AddressArea.Visible ? "d-flex pb-3 border-bottom" : "d-flex pt-2";
            ConfirmEmployerAddress.Text = employerAddressText;
            ConfirmEmployerAddressDiv.Visible = AddressArea.Visible;
            ConfirmEmployerAddressDiv.Attributes["class"] = AddressArea.Attributes["class"];

            ConfirmEmployerAddress.NavigateUrl = employerAddress?.GetGMapsAddressLink();

            ConfirmEmployerPhone.Text = EmployerPhone.Text;
            ComfirmEmployerPhonesDiv.Visible = EmployerPhoneField.Visible;

            ConfirmEmployerFaxDiv.Visible = EmployerFaxField.Visible;
            ConfirmEmployerFax.Text = EmployerFax.Text;

            ComfirmEmployerPhonesDiv.Visible = EmployerPhoneArea.Visible;

            ConfirmSeatName.Text = seat.SeatTitle;
            ConfirmSeatAgreement.Text = seatAgreement;
            BillingCustomerDiv.Visible = !string.IsNullOrEmpty(BillingCustomer.Value);

            ConfirmClassRefundPanel.Visible = !string.IsNullOrEmpty(classCancellationPolicyText);
            ConfirmClassRefundContent.Text = !string.IsNullOrEmpty(classCancellationPolicyText) ? classCancellationPolicyText : "N/A";

            ConfirmSection.Visible = ConfirmSection.IsSelected = true;
        }

        private bool BindPerson()
        {
            if (ActualCandidateIdentifier == null && !RegisterEmployee)
                return false;

            var candidateUser = ActualCandidateIdentifier.HasValue ? UserSearch.Select(ActualCandidateIdentifier.Value) : null;

            if (ActualCandidateIdentifier.HasValue && candidateUser == null)
                return false;

            var candidatePerson = candidateUser != null
                ? PersonSearch.Select(Organization.OrganizationIdentifier, candidateUser.UserIdentifier, x => x.HomeAddress)
                : null;

            DisableFields(candidateUser, candidatePerson);

            WorkBasedHoursToDate.ValueAsInt = null;

            Email.Text = candidateUser?.Email.ToLower();
            Email.Enabled = Email.Text.IsEmpty();

            FirstName.Text = candidateUser?.FirstName;
            LastName.Text = candidateUser?.LastName;
            Birthdate.Value = candidatePerson?.Birthdate;
            PersonCode.Text = candidatePerson?.PersonCode;

            if (!RegisterEmployee)
            {
                MiddleName.Text = candidateUser?.MiddleName;

                EmergencyContactName.Text = candidatePerson?.EmergencyContactName;
                EmergencyContactPhone.Text = candidatePerson?.EmergencyContactPhone;
                EmergencyContactRelationship.Text = candidatePerson?.EmergencyContactRelationship;
                FirstLanguage.Checked = string.Equals(candidatePerson?.FirstLanguage, "Not English", StringComparison.OrdinalIgnoreCase);

                Phone.Text = candidatePerson?.Phone;
                PhoneHome.Text = candidatePerson?.PhoneHome;
                PhoneWork.Text = candidatePerson?.PhoneWork;
                PhoneMobile.Text = candidateUser?.PhoneMobile;
                PhoneOther.Text = candidatePerson?.PhoneOther;

                BindAddress(candidatePerson?.HomeAddress);
            }

            EmployerGroupIdentifier.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
            EmployerGroupIdentifier.Filter.GroupType = GroupTypes.Employer;

            if (RegisterEmployee || candidateUser == null || !FullAccess && candidateUser.UserIdentifier != User.UserIdentifier)
            {
                var employerGroupIdentifier = PersonSearch.Select(Organization.Identifier, User.UserIdentifier)?.EmployerGroupIdentifier;
                if (employerGroupIdentifier.HasValue)
                {
                    EmployerGroupIdentifier.Value = employerGroupIdentifier;
                }
                else
                {
                    var roles = MembershipSearch.Select(x => x.UserIdentifier == User.UserIdentifier && x.MembershipType == MembershipType.EmployerContact && x.Group.OrganizationIdentifier == Organization.OrganizationIdentifier);
                    if (roles.Length > 1)
                        EmployerGroupIdentifier.Value = roles.FirstOrDefault()?.GroupIdentifier;
                }
            }
            else
            {
                EmployerGroupIdentifier.Value = candidatePerson.EmployerGroupIdentifier;
            }

            BindCompanyDetails();

            return true;
        }

        private void DisableFields(User candidateUser, Person candidatePerson)
        {
            var isReadOnly = candidatePerson != null && candidatePerson.UserIdentifier != User.UserIdentifier;

            FirstName.ReadOnly = isReadOnly;
            MiddleName.ReadOnly = isReadOnly && !string.IsNullOrEmpty(candidateUser?.MiddleName);
            LastName.ReadOnly = isReadOnly;
            Birthdate.Enabled = !isReadOnly || candidatePerson?.Birthdate == null;
            PersonCode.ReadOnly = isReadOnly && !string.IsNullOrEmpty(candidatePerson?.PersonCode);
            PersonCodeIdAvailability.Enabled = !PersonCode.ReadOnly;
        }

        private void BindAddress(Address homeAddress)
        {
            if (homeAddress != null)
            {
                Country.RefreshData();
                Province.RefreshData();

                Street1.Text = homeAddress.Street1;
                City.Text = homeAddress.City;
                Country.Value = homeAddress.Country ?? Organization.PlatformCustomization.TenantLocation.Country;
                Province.Value = homeAddress.Province ?? Organization.PlatformCustomization.TenantLocation.Province;
                PostalCode.Text = homeAddress.PostalCode;
            }
        }

        private void BindCompanyDetails()
        {
            EmployerContactName.Text = "None";
            EmployerContactPhoneNumber.Text = "None";
            EmployerContactEmail.Text = "None";

            var managerReference = EmployerGroupIdentifier.HasValue
                ? MembershipSearch.SelectFirst(x => x.GroupIdentifier == EmployerGroupIdentifier.Value.Value && x.MembershipType == MembershipType.EmployerContact)
                : null;

            Person manager = null;
            if (managerReference != null)
            {
                manager = PersonSearch.Select(Organization.Identifier, managerReference.UserIdentifier, x => x.User);

                if (manager != null)
                {
                    EmployerContactName.Text = manager.User.FullName;
                    EmployerContactEmail.Text = manager.User.Email;

                    EmployerContactPhoneNumberField.Visible = !string.IsNullOrEmpty(manager.Phone);

                    if (!string.IsNullOrEmpty(manager.Phone))
                        EmployerContactPhoneNumber.Text = manager.Phone;
                }
            }

            var company = EmployerGroupIdentifier.HasValue ? ServiceLocator.GroupSearch.GetGroup(EmployerGroupIdentifier.Value.Value) : null;
            var address = company != null ? ServiceLocator.GroupSearch.GetAddress(company.GroupIdentifier, AddressType.Shipping) : null;
            var addressText = address != null ? ClassVenueAddressInfo.GetAddress(address) : null;
            var isAddressVisible = !string.IsNullOrEmpty(addressText);
            var isPhoneVisible = !string.IsNullOrEmpty(company?.GroupPhone);

            CompanyName.Text = company?.GroupName;

            AddressArea.Visible = isAddressVisible;
            AddressArea.Attributes["class"] = isPhoneVisible ? "d-flex pb-3 border-bottom" : "d-flex pt-2";
            EmployerGroupName.InnerText = company?.GroupName;
            EmployerAddress.Text = addressText;

            EmployerAddress.NavigateUrl = address.GetGMapsAddressLink();

            EmployerPhoneField.Visible = isPhoneVisible;
            EmployerPhone.Text = company?.GroupPhone;

            EmployerPhoneArea.Visible = isPhoneVisible;

            AddressAndPhoneColumn.Visible = isAddressVisible || isPhoneVisible;

            EmployerContactColumn.Visible = manager != null;
        }

        private Guid SaveData(QEvent @event)
        {
            var employer = GetOrCreateEmployer();
            var savedUser = SaveUser(employer);
            var user = savedUser.User;

            var registration = @event.Registrations.FirstOrDefault(x => x.CandidateIdentifier == user.UserIdentifier);
            var registrationIdentifier = registration?.RegistrationIdentifier;
            var invoice = InvoiceIdentifier.HasValue ? new InvoiceSearch().GetInvoice((Guid)InvoiceIdentifier) : null;
            var payment = PaymentIdentifier.HasValue ? ServiceLocator.PaymentSearch.GetPayment(PaymentIdentifier.Value) : null;
            var commands = new List<ICommand>();

            if (registration == null)
            {
                registrationIdentifier = UniqueIdentifier.Create();
                commands.Add(new RequestRegistration(registrationIdentifier.Value, Organization.OrganizationIdentifier, EventIdentifier.Value, user.UserIdentifier, null, "Registered", null, null, null));
            }
            else
            {
                commands.Add(new ChangeApproval(registrationIdentifier.Value, "Registered", null, null, registration.ApprovalStatus));
            }

            commands.Add(new IncludeRegistrationToT2202(registrationIdentifier.Value));

            var workBasedHoursToDate = WorkBasedHoursToDate.ValueAsInt;

            commands.Add(new AssignRegistrationHoursWorked(registrationIdentifier.Value, workBasedHoursToDate));

            if (employer != null)
                commands.Add(new AssignEmployer(registrationIdentifier.Value, employer.GroupIdentifier));

            commands.Add(CreateSeatCommand(registrationIdentifier.Value));

            if (payment != null)
                commands.Add(new AssignRegistrationPayment(registrationIdentifier.Value, PaymentIdentifier.Value));

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            if (invoice != null)
                SendPaidInvoiceInformation(invoice);

            if (savedUser.ChangedFields != null)
                SendChangeRegistrantContactInformation(user, @event.EventTitle, savedUser.ChangedFields);

            if (PersonCodeIdAvailability.Checked && Organization.OrganizationIdentifier == OrganizationIdentifiers.RCABC)
                RegistrationHelper.SendPersonCodeNotEnteredAlert(user, @event.EventTitle, savedUser.ChangedFields);

            if (@event.LearnerRegistrationGroupIdentifier.HasValue)
                RegistrationHelper.AddLearnerToRegistrationGroup(@event.LearnerRegistrationGroupIdentifier.Value, user.UserIdentifier, Organization.OrganizationIdentifier);

            RegistrationHelper.SendRegistratrionCompleteAlert(registrationIdentifier.Value, user.UserIdentifier);

            return registrationIdentifier.Value;
        }

        private void SendPaidInvoiceInformation(VInvoice invoice = null)
        {
            if (invoice != null)
                ProductHelper.SendInvoicePaid(invoice, User.Identifier, User.TimeZone, false, false);
        }

        private void SendChangeRegistrantContactInformation(QUser user, string eventTitle, RegistrantChangedField[] changedFields)
        {
            ServiceLocator.AlertMailer.Send(Organization.OrganizationIdentifier, user.UserIdentifier, new AlertRegistrantContactInformationChanged
            {
                ContactLoginName = user.FullName,
                ContactEmail = user.Email,
                EventName = eventTitle,
                ContactChangedFields = BuildChangeFieldList()
            });

            string BuildChangeFieldList()
            {
                var md = new StringBuilder();
                md.AppendLine("Field Name | Old Value | New Value");
                md.AppendLine("-- | -- | --");
                foreach (var changedField in changedFields)
                    md.AppendLine($"{changedField.FieldName} | {changedField.OldValue} | {changedField.NewValue}");
                return md.ToString();
            }
        }

        private QGroup GetOrCreateEmployer()
        {
            if (CompanyTypeExisting.Checked)
            {
                return EmployerGroupIdentifier.HasValue && MembershipPermissionHelper.CanModifyMembership(EmployerGroupIdentifier.Value.Value)
                    ? ServiceLocator.GroupSearch.GetGroup(EmployerGroupIdentifier.Value.Value)
                    : null;
            }

            var groupName = NewCompanyName.Text.Trim();

            var group = ServiceLocator.GroupSearch
                .GetGroups(new QGroupFilter
                {
                    GroupName = groupName,
                    OrganizationIdentifier = Organization.Identifier,
                    GroupType = GroupTypes.Employer
                })
                .FirstOrDefault();

            if (group == null)
                group = CreateNewEmployerGroup();
            else if (!MembershipPermissionHelper.CanModifyMembership(group))
                return null;

            Guid userIdentifier;

            if (NewCompanyContactType.SelectedValue == "Existing")
            {
                userIdentifier = User.UserIdentifier;
            }
            else
            {
                UserFactory factory = new UserFactory();
                factory.RegisterUser(
                    NewCompanyContactEmail.Text.Trim(),
                    Organization.Identifier,
                    NewCompanyContactFirstName.Text,
                    NewCompanyContactLastName.Text,
                    null,
                    group.GroupIdentifier,
                    NewCompanyContactPhone.Text,
                    null,
                    Organization.Toolkits.Contacts?.DefaultMFA ?? false
                    );

                userIdentifier = factory.User.UserIdentifier;
            }

            MembershipHelper.Save(group.GroupIdentifier, userIdentifier, MembershipType.EmployerContact);

            return group;
        }

        private (QGroup, QGroupAddress) GetEmployer()
        {
            if (!CompanyTypeExisting.Checked)
                return GetNewEmployerGroup();

            if (EmployerGroupIdentifier.Value == null)
                return (null, null);

            var employer = ServiceLocator.GroupSearch.GetGroup(EmployerGroupIdentifier.Value.Value);
            var address = ServiceLocator.GroupSearch.GetAddress(employer.GroupIdentifier, AddressType.Shipping);

            return (employer, address);
        }

        private (QGroup, QGroupAddress) GetNewEmployerGroup()
        {
            var employer = new QGroup
            {
                GroupType = GroupTypes.Employer,
                GroupName = NewCompanyName.Text.Trim()
            };

            var address = new QGroupAddress
            {
                AddressIdentifier = UniqueIdentifier.Create(),
                Street1 = NewCompanyStreet1.Text,
                City = NewCompanyCity.Text,
                Province = NewCompanyProvinceSelector.Value,
                PostalCode = NewCompanyPostalCode.Text,
                Country = "Canada"
            };

            return (employer, address);
        }

        private QGroup CreateNewEmployerGroup()
        {
            var id = UniqueIdentifier.Create();
            var commands = new List<Command>();

            var organization = CurrentSessionState.Identity.Organization.Identifier;
            var name = NewCompanyName.Text.Trim();

            commands.Add(new CreateGroup(id, organization, GroupTypes.Employer, name));
            commands.Add(new DescribeGroup(id, null, null, null, "Company"));

            var address = new GroupAddress
            {
                Street1 = NewCompanyStreet1.Text,
                City = NewCompanyCity.Text,
                Province = NewCompanyProvinceSelector.Value,
                PostalCode = NewCompanyPostalCode.Text,
                Country = "Canada"
            };

            commands.Add(new ChangeGroupAddress(id, AddressType.Shipping, address));

            ServiceLocator.SendCommands(commands);

            return ServiceLocator.GroupSearch.GetGroup(id);
        }

        private SavedUser SaveUser(QGroup employer)
        {
            QUser user;
            QPerson person;
            RegistrantChangedField[] changedFields = null;

            if (ActualCandidateIdentifier.HasValue)
            {
                user = ServiceLocator.UserSearch.GetUser(ActualCandidateIdentifier.Value);
                person = ServiceLocator.PersonSearch.GetPerson(user.UserIdentifier, Organization.OrganizationIdentifier, x => x.HomeAddress);

                changedFields = GetCandidateFields(user, person, employer);

                if (person != null)
                    PersonStore.Update(person);

                UserStore.Update(user, OrganizationSearch.GetPersonFullNamePolicy(Organization.Identifier));
            }
            else
            {
                UserFactory factory = new UserFactory();
                factory.RegisterUser(
                    Email.Text,
                    Organization.Identifier,
                    FirstName.Text,
                    LastName.Text,
                    null,
                    employer?.GroupIdentifier,
                    Phone.Text,
                    null,
                    Organization.Toolkits.Contacts?.DefaultMFA ?? false
                );

                user = factory.User;
                person = factory.Person;

                GetCandidateFields(user, person, employer);

                person.EmailEnabled = Email.Text.IsNotEmpty();
                person.Region = employer?.GroupRegion;

                UserStore.Update(user, OrganizationSearch.GetPersonFullNamePolicy(Organization.Identifier));
                PersonStore.Update(person);
            }

            if (employer != null && !MembershipSearch.Exists(x => x.GroupIdentifier == employer.GroupIdentifier && x.UserIdentifier == user.UserIdentifier))
            {
                MembershipHelper.Save(employer.GroupIdentifier, user.UserIdentifier, "Employee");
            }

            return new SavedUser { User = user, ChangedFields = changedFields };
        }

        private RegistrantChangedField[] GetCandidateFields(QUser user, QPerson person, QGroup employer)
        {
            var changedFields = new List<RegistrantChangedField>();

            ChangeField(() => user.FirstName = FirstName.Text, "First Name", user.FirstName, FirstName.Text, changedFields);
            ChangeField(() => user.MiddleName = MiddleName.Text, "Middle Name", user.MiddleName, MiddleName.Text, changedFields);
            ChangeField(() => user.LastName = LastName.Text, "Last Name", user.LastName, LastName.Text, changedFields);
            ChangeField(() => user.PhoneMobile = Shift.Common.Phone.Format(PhoneMobile.Text), "Phone (Mobile)", user.PhoneMobile, Shift.Common.Phone.Format(PhoneMobile.Text), changedFields);

            if (person != null)
            {
                var homeAddress = person.GetAddress(ContactAddressType.Home);

                ChangeField(() => person.Birthdate = Birthdate.Value, "Birthdate", $"{person.Birthdate: MMM d, yyyy}", $"{Birthdate.Value: MMM d, yyyy}", changedFields);

                if (!PersonCodeIdAvailability.Checked)
                    ChangeField(() => person.PersonCode = PersonCode.Text, "Learner ID Number", person.PersonCode, PersonCode.Text, changedFields);

                ChangeField(() => person.EmergencyContactName = EmergencyContactName.Text, "Emergency Contact Name", person.EmergencyContactName, EmergencyContactName.Text, changedFields);
                ChangeField(() => person.EmergencyContactPhone = Shift.Common.Phone.Format(EmergencyContactPhone.Text), "Emergency Contact Phone", person.EmergencyContactPhone, Shift.Common.Phone.Format(EmergencyContactPhone.Text), changedFields);
                ChangeField(() => person.EmergencyContactRelationship = EmergencyContactRelationship.Text, "Emergency Contact Relationship", person.EmergencyContactRelationship, EmergencyContactRelationship.Text, changedFields);
                ChangeField(() => person.FirstLanguage = FirstLanguage.Checked ? "Not English" : "English", "First Language", person.FirstLanguage, FirstLanguage.Checked ? "Not English" : "English", changedFields);
                ChangeField(() => person.Phone = Shift.Common.Phone.Format(Phone.Text), "Phone", person.Phone, Shift.Common.Phone.Format(Phone.Text), changedFields);
                ChangeField(() => person.PhoneHome = Shift.Common.Phone.Format(PhoneHome.Text), "Phone (Home)", person.PhoneHome, Shift.Common.Phone.Format(PhoneHome.Text), changedFields);
                ChangeField(() => person.PhoneWork = Shift.Common.Phone.Format(PhoneWork.Text), "Phone (Work)", person.PhoneWork, Shift.Common.Phone.Format(PhoneWork.Text), changedFields);
                ChangeField(() => person.PhoneOther = Shift.Common.Phone.Format(PhoneOther.Text), "Phone (Other)", person.PhoneOther, Shift.Common.Phone.Format(PhoneOther.Text), changedFields);
                ChangeField(() => homeAddress.Street1 = Street1.Text, "Street 1 (Home Address)", homeAddress.Street1, Street1.Text, changedFields);
                ChangeField(() => homeAddress.City = City.Text, "City (Home Address)", homeAddress.City, City.Text, changedFields);
                ChangeField(() => homeAddress.Country = Country.Value, "Country (Home Address)", homeAddress.Country, Country.Value, changedFields);
                ChangeField(() => homeAddress.Province = Province.Value, "Province (Home Address)", homeAddress.Province, Province.Value, changedFields);
                ChangeField(() => homeAddress.PostalCode = PostalCode.Text, "Postal Code", homeAddress.PostalCode, PostalCode.Text, changedFields);
                ChangeField(person, employer, changedFields);
            }

            return changedFields.Count > 0 ? changedFields.ToArray() : null;
        }

        private void ChangeField(Action setAction, string fieldName, string oldValue, string newValue, List<RegistrantChangedField> changedFields)
        {
            if (oldValue == null)
                oldValue = "";

            if (newValue == null)
                newValue = "";

            if (oldValue != newValue)
                changedFields.Add(new RegistrantChangedField { FieldName = fieldName, OldValue = oldValue, NewValue = newValue });

            setAction.Invoke();
        }

        private void ChangeField(QPerson person, QGroup employer, List<RegistrantChangedField> changedFields)
        {
            if (person.EmployerGroupIdentifier == employer?.GroupIdentifier)
                return;

            var oldValue = person.EmployerGroupIdentifier.HasValue
                ? ServiceLocator.GroupSearch.GetGroup(person.EmployerGroupIdentifier.Value)?.GroupName
                : "";

            changedFields.Add(new RegistrantChangedField { FieldName = "Employer", OldValue = oldValue, NewValue = employer?.GroupName ?? "" });

            person.EmployerGroupIdentifier = employer?.GroupIdentifier;
        }

        private ICommand CreateSeatCommand(Guid registrationIdentifier)
        {
            var seatIdentifier = Guid.Parse(SelectedSeat.Value);
            var seat = ServiceLocator.EventSearch.GetSeat(seatIdentifier);
            var price = GetSeatPrice(seat);

            return new AssignSeat(registrationIdentifier, seatIdentifier, price, BillingCustomer.Value);
        }

        private decimal? GetSeatPrice(QSeat seat)
        {
            var configuration = JsonConvert.DeserializeObject<SeatConfiguration>(seat.Configuration);

            if (configuration.Prices == null)
                return null;

            if (configuration.Prices.Count == 1 && !configuration.Prices[0].Name.HasValue())
                return configuration.Prices[0].Amount;

            return decimal.Parse(MultiplePrice.SelectedValue);
        }

        private void ShowSeatDetails(Guid? defaultSeatIdentifier, decimal? priceAmount, string billingCustomer)
        {
            if (defaultSeatIdentifier.HasValue)
                SelectedSeat.Value = defaultSeatIdentifier.ToString();

            BindSeats(EventIdentifier.Value);

            PriceArea.Visible = true;
            FreePrice.Visible = false;
            SinglePrice.Visible = false;
            MultiplePrice.Visible = false;
            Agreement.Visible = false;

            var seatIdentifier = Guid.Parse(SelectedSeat.Value);
            var seat = ServiceLocator.EventSearch.GetSeat(seatIdentifier);
            var configuration = JsonConvert.DeserializeObject<SeatConfiguration>(seat.Configuration);

            if (configuration.Prices == null)
            {
                FreePrice.Visible = true;
                Agreement.Visible = true;
            }
            else if (configuration.Prices.Count == 1 && !configuration.Prices[0].Name.HasValue())
            {
                SinglePrice.Visible = true;
                SinglePrice.Text = $"{configuration.Prices[0].Amount:c2}";

                Agreement.Visible = true;
            }
            else
            {
                BindMultiplePrice(configuration, priceAmount);
            }

            var content = ContentSeat.Deserialize(seat.Content);
            var agreement = content.GetAgreementHtml();

            AgreementText.Text = agreement;
            Agreed.Checked = false;

            OnAgreedCheckedChanged();

            BindBillingCustomers(configuration);

            if (billingCustomer.HasValue())
                BillingCustomer.Value = billingCustomer;
        }

        private void BindMultiplePrice(SeatConfiguration configuration, decimal? priceAmount)
        {
            MultiplePrice.Visible = true;
            MultiplePrice.Items.Clear();

            var prices = GetPrices(configuration);
            var showUnapplicableSeats = Organization.Toolkits.Events.ShowUnapplicableSeats;
            var enabledCount = 0;
            System.Web.UI.WebControls.ListItem enabledItem = null;

            foreach (var pricePair in prices)
            {
                if (!showUnapplicableSeats && !pricePair.Applicable)
                    continue;

                var price = pricePair.Price;
                var item = new System.Web.UI.WebControls.ListItem($"{price.Name} {price.Amount:c2}", price.Amount.ToString());

                MultiplePrice.Items.Add(item);

                if (!pricePair.Applicable)
                {
                    item.Enabled = false;
                    continue;
                }

                enabledCount++;

                if (price.Amount == priceAmount)
                {
                    item.Selected = true;
                    Agreement.Visible = true;
                }
                else
                {
                    enabledItem = item;
                }
            }

            if (enabledCount == 1 && enabledItem != null)
            {
                enabledItem.Selected = true;
                Agreement.Visible = true;
            }
        }

        private List<(SeatConfiguration.Price Price, bool Applicable)> GetPrices(SeatConfiguration configuration)
        {
            var existingEmployer = CompanyTypeExisting.Checked
                && EmployerGroupIdentifier.HasValue
                && EmployerGroupIdentifier.Value != Guid.Empty;

            string employerStatus;
            if (existingEmployer)
            {
                var employer = ServiceLocator.GroupSearch.GetGroup(EmployerGroupIdentifier.Value.Value);
                employerStatus = employer != null ? TCollectionItemCache.GetName(employer.GroupStatusItemIdentifier) : null;
            }
            else
                employerStatus = null;

            var hideStatuslessPrices = Organization.OrganizationIdentifier == OrganizationIdentifiers.RCABC
                && string.Equals(employerStatus, "Active Member", StringComparison.OrdinalIgnoreCase)
                && configuration.Prices.Any(x => string.Equals(x.GroupStatus, employerStatus, StringComparison.OrdinalIgnoreCase))
                && configuration.Prices.Any(x => !x.GroupStatus.HasValue());

            var result = new List<(SeatConfiguration.Price Price, bool Applicable)>();

            if (configuration.Prices != null)
            {
                foreach (var price in configuration.Prices)
                {
                    var applicable = (string.IsNullOrEmpty(price.GroupStatus) || StringHelper.Equals(price.GroupStatus, employerStatus))
                        && (!hideStatuslessPrices || price.GroupStatus.HasValue());

                    result.Add((price, applicable));
                }
            }

            return result;
        }

        private void BindSeats(Guid eventIdentifier)
        {
            var seats = ServiceLocator.EventSearch
                .GetSeats(eventIdentifier, false);

            if (!Organization.Toolkits.Events.ShowUnapplicableSeats)
            {
                var nonEmptySeats = new List<QSeat>();
                foreach (var seat in seats)
                {
                    var configuration = JsonConvert.DeserializeObject<SeatConfiguration>(seat.Configuration);
                    var prices = GetPrices(configuration);

                    if (prices.Any(x => x.Applicable))
                        nonEmptySeats.Add(seat);
                }

                seats = nonEmptySeats;
            }

            if (seats.Count == 1)
                SelectedSeat.Value = seats[0].SeatIdentifier.ToString();

            OneSeatTitle.Visible = seats.Count <= 1;
            MultipleSeatTitle.Visible = seats.Count >= 2;

            SeatRepeater.DataSource = seats;
            SeatRepeater.DataBind();
        }

        private void BindBillingCustomers(SeatConfiguration configuration)
        {
            var hasData = configuration.BillingCustomers != null && configuration.BillingCustomers.Count > 0;

            BillingCustomerField.Visible = hasData;

            if (hasData)
                BillingCustomer.LoadItems(configuration.BillingCustomers);
        }

        private string GetParticipantAddress()
        {
            var addressText = new StringBuilder();

            if (Street1.Text.HasValue())
                addressText.AppendLine($"<div>{Street1.Text}</div>");

            string line = string.Empty;

            if (City.Text.HasValue())
                line = $"{City.Text}";

            if (line.Length > 0 && Country.Value.IsNotEmpty())
                line += ", ";

            if (Country.Value.HasValue())
                line += Country.Value;

            if (line.Length > 0 && Province.Value.IsNotEmpty())
                line += ", ";

            if (Province.Value.HasValue())
                line += Province.Value;

            if (PostalCode.Text.HasValue())
                line += $" {PostalCode.Text}";

            if (!(line == string.Empty))
                addressText.AppendLine($"<div>{line}</div>");

            return addressText.ToString();
        }

        private void ShowCompanyPanel()
        {
            var isNew = CompanyTypeNew.Checked;

            ExistingCompanyPanel.Visible = !isNew;
            AddressAndPhoneColumn.Visible = !isNew;
            EmployerContactColumn.Visible = !isNew;

            NewEmployerColumn.Visible = isNew;
            NewEmployerContactColumn.Visible = isNew;

            if (!isNew)
                BindCompanyDetails();
        }

        private void NavigateToSearch()
            => HttpResponseHelper.Redirect("/ui/portal/events/classes/search", true);

        private string GetNavigateBackLink()
            => !IsAdminOutline ? GetOutlineLink() : $"/ui/admin/events/classes/outline?event={EventIdentifier}&panel=registrations";

        private string GetOutlineLink()
        {
            return "/ui/portal/events/classes/outline" + GetOutlineLinkArgs();
        }

        private string GetOutlineLinkArgs()
        {
            return "?event=" + EventIdentifier;
        }

        protected static string GetDescription(object item)
        {
            var seat = (QSeat)item;
            return ContentSeat.Deserialize(seat.Content).Description.Default;
        }

        public static bool CompanySelectionDisabled()
        {
            return (Identity.Organization.Toolkits.Events?.CompanySelectionAndCreationDisabledDuringRegistration) ?? false;
        }

        #endregion
    }
}
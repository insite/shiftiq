using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Contacts.Read;
using InSite.Application.Events.Read;
using InSite.Application.Gateways.Write;
using InSite.Application.Invoices.Read;
using InSite.Application.Invoices.Write;
using InSite.Application.Registrations.Read;
using InSite.Application.Registrations.Write;
using InSite.Common;
using InSite.Common.Web;
using InSite.Domain.Invoices;
using InSite.Domain.Payments;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;
using InSite.UI.Portal.Billing.Utilities;
using InSite.UI.Portal.Events.Classes.Controls;
using InSite.UI.Portal.Events.Classes.Models;
using InSite.Web.Data;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;

using PersonItem = Shift.Sdk.UI.PersonItem;

namespace InSite.UI.Portal.Events.Classes
{
    public partial class RegisterMultiple : PortalBasePage
    {
        #region Properties

        private Guid? EventIdentifier => Guid.TryParse(Request["event"], out var id) ? id : (Guid?)null;

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

        private int StepNumber => ViewState[nameof(StepNumber)] as int? ?? 1;
        private void SetStepNumber(int stepNumber)
        {
            ViewState[nameof(StepNumber)] = stepNumber;

            var steps = new[]
            {
                new { Number = 1, Title = Translate("Enter participants"), IsCompleted = stepNumber >= 1 },
                new { Number = 2, Title = Translate("Select employer"), IsCompleted = stepNumber >= 2 },
                new { Number = 3, Title = Translate("Choose payment option"), IsCompleted = stepNumber >= 3 },
                new { Number = 4, Title = Translate("Enter payment info"), IsCompleted = stepNumber >= 4 },
                new { Number = 5, Title = Translate("Confirm"), IsCompleted = stepNumber == 5 },
            };

            TopProgressBar.DataSource = steps;
            TopProgressBar.DataBind();
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

        private decimal SeatPrice
        {
            get => ViewState[nameof(SeatPrice)] as decimal? ?? 0;
            set => ViewState[nameof(SeatPrice)] = value;
        }

        private Guid SelectedSeatIdentifier
        {
            get => (Guid)ViewState[nameof(SelectedSeatIdentifier)];
            set => ViewState[nameof(SelectedSeatIdentifier)] = value;
        }

        private string BillingCustomer
        {
            get => (string)ViewState[nameof(BillingCustomer)];
            set => ViewState[nameof(BillingCustomer)] = value;
        }

        private List<PersonItem> People
        {
            get => (List<PersonItem>)ViewState[nameof(People)];
            set => ViewState[nameof(People)] = value;
        }

        private string CodeLabel => LabelHelper.GetLabelContentText("Person Code");

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SeatDetail.AgreedChanged += SeatDetail_AgreedChanged;
            SeatDetail.SeatChanged += SeatDetail_SeatChanged;

            BackButton.Click += BackButton_Click;
            BackButton2.Click += BackButton2_Click;
            BackButton3.Click += BackButton3_Click;
            BackButton4.Click += BackButton3_Click;

            ApplyPersonListButton.Click += ApplyPersonListButton_Click;
            ApplyPersonCsvButton.Click += ApplyPersonCsvButton_Click;
            NextButton2.Click += NextButton2_Click;
            NextButton3.Click += NextButton3_Click;
            NextButton4.Click += NextButton4_Click;

            SaveButton.Click += SaveButton_Click;

            AvailabilityValidator.ServerValidate += AvailabilityValidator_ServerValidate;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Page.MaintainScrollPositionOnPostBack = false;

            if (IsPostBack)
                return;

            SetStepNumber(1);
            LoadData();
        }

        #endregion

        #region Event handlers

        private void SeatDetail_AgreedChanged(object sender, BooleanValueArgs args)
        {
            NextButton3.Enabled = args.Value;

            if (args.Value)
                NextButton3_Click(null, new EventArgs());
        }

        private void SeatDetail_SeatChanged(object sender, EventArgs e)
        {
            SeatDetail.ShowSeatDetails(EmployerDetail.EmployerIdentifier);

            NextButton3.Enabled = SeatDetail.IsAgreed;

            PaymentSection.Visible = false;
            ConfirmSection.Visible = false;
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            PersonsPanel.Visible = PersonsPanel.IsSelected = true;
        }

        private void BackButton2_Click(object sender, EventArgs e)
        {
            EmployerPanel.Visible = EmployerPanel.IsSelected = true;
        }

        private void BackButton3_Click(object sender, EventArgs e)
        {
            SeatPanel.Visible = SeatPanel.IsSelected = true;
            PaymentSection.Visible = false;
            ConfirmSection.Visible = false;
        }

        private void ApplyPersonListButton_Click(object sender, EventArgs e)
        {
            PersonListTab.IsSelected = true;

            if (!Page.IsValid)
                return;

            OnApplyPersonlist(PersonList.GetPeople());
        }

        private void ApplyPersonCsvButton_Click(object sender, EventArgs e)
        {
            PersonListCsvTab.IsSelected = true;

            if (!Page.IsValid)
                return;

            OnApplyPersonlist(PersonListCsv.GetPeople());
        }

        private void OnApplyPersonlist(List<PersonItem> people)
        {
            if (people.Count == 0 || ValidatePeople(people) == null)
                return;

            var emptyEmails = new List<PersonItem>();

            foreach (var personItem in people)
            {
                if (string.IsNullOrEmpty(personItem.Email))
                    emptyEmails.Add(personItem);
            }

            if (emptyEmails.Count > 0)
            {
                var domain = ServiceLocator.AppSettings.Application.EmailDomain;
                var emails = UserSearch.CreateUniqueEmailsForOrganization(Organization.Code, domain, emptyEmails.Count);
                for (int i = 0; i < emails.Count; i++)
                    emptyEmails[i].Email = emails[i];
            }

            People = people;

            EmployerPanel.Visible = EmployerPanel.IsSelected = true;

            EmployerDetail.ShowCompanyPanel();

            if (StepNumber < 2)
                SetStepNumber(2);
        }

        private List<(PersonItem, QUser, QPerson)> ValidatePeople(List<PersonItem> people)
        {
            var emails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var codes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var result = new List<(PersonItem, QUser, QPerson)>();

            for (var i = 0; i < people.Count; i++)
            {
                var p = people[i];
                var (user, person, error) = ValidateRow(emails, codes, i + 1, p);
                if (!string.IsNullOrEmpty(error))
                {
                    StatusAlert.AddMessage(AlertType.Error, error);
                    return null;
                }
                result.Add((p, user, person));
            }

            return result;
        }

        private (QUser, QPerson, string) ValidateRow(HashSet<string> emails, HashSet<string> codes, int rowNumber, PersonItem p)
        {
            var hasEmail = p.Email.IsNotEmpty();
            var hasCode = p.PersonCode.IsNotEmpty();

            if (hasEmail && !emails.Add(p.Email))
                return (null, null, $"Row {rowNumber}: the email address <strong>{p.Email}</strong> appeared more than once in the list.");

            if (hasCode && !codes.Add(p.PersonCode))
                return (null, null, $"Row {rowNumber}: the {CodeLabel} <strong>{p.PersonCode}</strong> appeared more than once in the list.");

            if (!hasCode && !p.Birthdate.HasValue)
                return (null, null, $"Row {rowNumber}: either Birthdate or {CodeLabel} should be specified.");

            var user = hasEmail ? ServiceLocator.UserSearch.GetUserByEmail(p.Email) : null;
            var person = hasCode ? ServiceLocator.PersonSearch.GetPerson(p.PersonCode, Organization.OrganizationIdentifier) : null;

            if (user == null && person != null)
                user = ServiceLocator.UserSearch.GetUser(person.UserIdentifier);
            else if (user != null && person == null)
                person = ServiceLocator.PersonSearch.GetPerson(user.UserIdentifier, Organization.OrganizationIdentifier);

            if (IsAlreadyRegistered(user))
                return (null, null, $"Row {rowNumber}: this person is already registered in this class.");

            if (!IsPersonalInfoMatched(user, person, p))
            {
                return (null, null, $"Row {rowNumber}: the email address or {CodeLabel} entered for " +
                    $"{p.FirstName} {p.LastName} is already associated with another contact. " +
                    $"Please review your entry or contact {Organization.CompanyName} for assistance."
                );
            }

            return (user, person, null);
        }

        private bool IsAlreadyRegistered(QUser user)
        {
            if (user == null)
                return false;

            var filter = new QRegistrationFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                CandidateIdentifier = user.UserIdentifier,
                EventIdentifier = EventIdentifier
            };

            return ServiceLocator.RegistrationSearch.GetRegistration(filter) != null;
        }

        private bool IsPersonalInfoMatched(QUser user, QPerson person, PersonItem p)
        {
            return
                   (user == null ||
                    string.Equals(user.FirstName, p.FirstName, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(user.LastName, p.LastName, StringComparison.OrdinalIgnoreCase)
                   )
                && (person == null ||
                    user?.UserIdentifier == person.UserIdentifier &&
                    (p.PersonCode.IsEmpty() || string.Equals(person.PersonCode, p.PersonCode, StringComparison.OrdinalIgnoreCase)));
        }

        private void NextButton2_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            PaymentSection.Visible = false;
            ConfirmSection.Visible = false;
            SeatPanel.Visible = true;

            SeatPanel.IsSelected = true;

            if (SeatDetail.SelectedSeatIdentifier.HasValue)
            {
                SeatDetail.ShowSeatDetails(EmployerDetail.EmployerIdentifier);

                NextButton3.Enabled = SeatDetail.IsAgreed;

                PaymentSection.Visible = false;
                ConfirmSection.Visible = false;
            }

            SetStepNumber(3);
        }

        private void NextButton3_Click(object sender, EventArgs e)
        {
            var price = SeatDetail.Price * People.Count;

            SeatPrice = SeatDetail.Price;
            SelectedSeatIdentifier = SeatDetail.SelectedSeatIdentifier.Value;
            BillingCustomer = SeatDetail.BillingCustomer;

            if (price == 0)
            {
                ConfirmPayment.SetCreditCardVisibility(false);

                // Show Cart details logic...

                ShowConfirmationDetails();
                SetStepNumber(5);

                return;
            }

            ConfirmSection.Visible = false;
            PaymentSection.Visible = PaymentSection.IsSelected = true;

            PaymentDetail.Visible = true;
            PaymentDetail.SetInputValues(price, true);

            NextButton4.Enabled = true;

            SetStepNumber(4);
        }

        private void NextButton4_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            ConfirmPayment.SetCreditCardVisibility(true);

            var payment = PaymentDetail.GetInputValues();

            if (!payment.Card.IsValid)
            {
                PaymentAlert.AddMessage(AlertType.Error, payment.Card.ErrorMessage);
                return;
            }

            PaymentAmount = payment.PaymentAmount;
            CreditCard = payment.Card;

            ConfirmPayment.SetPayment(payment);

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

            if (SeatDetail.Price == 0)
            {
                if (SaveData(@event))
                    HttpResponseHelper.Redirect(GetNavigateBackLink());

                return;
            }

            if (ServiceLocator.PaymentSearch.GetPayment(PaymentIdentifier.Value) != null)
            {
                StatusAlert.AddMessage(AlertType.Error, Translate("The same payment information cannot be re-submitted."));
                return;
            }

            InvoiceIdentifier = UniqueIdentifier.Create();

            var commands = new List<Command>();

            var invoiceNumber = CreateInvoice(@event, commands);
            StartPayment(invoiceNumber, commands);

            ServiceLocator.SendCommands(commands);

            if (CheckPaymentStatus() && SaveData(@event))
            {
                var url = $"/ui/portal/events/classes/register-multiple-success?invoice={InvoiceIdentifier}";
                HttpResponseHelper.Redirect(url);
            }
        }

        private void AvailabilityValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var @event = ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value, x => x.Registrations);

            if (@event == null)
            {
                NavigateToSearch();
                return;
            }

            var availableCount = @event.CapacityMaximum.HasValue && @event.CapacityMaximum > 0
                ? @event.CapacityMaximum - @event.ConfirmedRegisteredCount
                : null;

            args.IsValid = People != null && (availableCount == null || availableCount >= People.Count);

            if (args.IsValid)
                return;

            if (People == null)
                AvailabilityValidator.ErrorMessage = $"Something went wrong, please refresh the page and re-enter the information";
            else if (availableCount <= 0)
                AvailabilityValidator.ErrorMessage = $"There are no available seats in this class";
            else
            {
                var employeeText = People.Count == 1 ? "employee" : "employees";
                var seatText = availableCount == 1 ? "seat" : "seats";

                AvailabilityValidator.ErrorMessage = $"You are trying to register {People.Count} {employeeText} but available only {availableCount} {seatText}";
            }
        }

        #endregion

        #region Payment

        private int CreateInvoice(QEvent @event, List<Command> commands)
        {
            var list = People;
            var productIdentifier = GetOrCreateProduct(@event);
            var invoiceItems = new List<InvoiceItem>();

            foreach (var personItem in list)
            {
                var invoiceItem = new InvoiceItem
                {
                    Identifier = UniqueIdentifier.Create(),
                    Product = productIdentifier,
                    Quantity = 1,
                    Price = SeatPrice,
                    Description = $"{@event.EventTitle} activity registration for {personItem.FirstName} {personItem.LastName}"
                };
                invoiceItems.Add(invoiceItem);
            }

            var invoiceNumber = Sequence.Increment(Organization.Identifier, SequenceType.Invoice);

            commands.Add(new DraftInvoice(InvoiceIdentifier.Value, Organization.Identifier, invoiceNumber, User.UserIdentifier, invoiceItems.ToArray()));
            commands.Add(new SubmitInvoice(InvoiceIdentifier.Value));
            return invoiceNumber;
        }

        private void StartPayment(int invoiceNumber, List<Command> commands)
        {
            var card = CreditCard;

            var startPaymentCommand = new StartPayment(
                PaymentIdentifiers.BamboraGateway,
                Organization.Identifier,
                (Guid)InvoiceIdentifier,
                PaymentIdentifier.Value,
                new PaymentInput(
                    Invoice.FormatInvoiceNumber(invoiceNumber),
                    PaymentAmount,
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

            commands.Add(startPaymentCommand);
        }

        private Guid GetOrCreateProduct(QEvent @event)
        {
            var seatIdentifier = SeatDetail.SelectedSeatIdentifier.Value;
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
            var error = Register.CheckPaymentStatus(PaymentIdentifier.Value);

            if (string.IsNullOrEmpty(error))
                return true;

            StatusAlert.AddMessage(AlertType.Error, error);

            return false;
        }

        #endregion

        #region Helper methods

        private void LoadData()
        {
            if (EventIdentifier == null
                || TGroupPermissionSearch.IsAccessDenied(EventIdentifier.Value, Identity)
                )
            {
                NavigateToSearch();
            }

            var @event = ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value, x => x.Registrations, x => x.Achievement);
            if (@event == null || @event.OrganizationIdentifier != Organization.OrganizationIdentifier || !@event.AllowMultipleRegistrations)
            {
                NavigateToSearch();
                return;
            }

            if (!HasAccess(@event))
            {
                HttpResponseHelper.Redirect(GetOutlineLink());
                return;
            }

            PortalMaster.SidebarVisible(false);

            PageHelper.AutoBindHeader(this);

            var subtitle = $"{@event.EventScheduledStart.FormatDateOnly(User.TimeZone)}";
            if (@event.EventScheduledEnd.HasValue && @event.EventScheduledEnd.Value.Date != @event.EventScheduledStart.Date)
                subtitle += $" - {@event.EventScheduledEnd.Value.FormatDateOnly(User.TimeZone)}";

            PortalMaster.Breadcrumbs.BindTitleAndSubtitleNoTranslate(@event.EventTitle, subtitle);

            CancelButton1.NavigateUrl = GetNavigateBackLink();
            CancelButton12.NavigateUrl = GetNavigateBackLink();
            CancelButton2.NavigateUrl = GetNavigateBackLink();
            CancelButton3.NavigateUrl = GetNavigateBackLink();
            CancelButton4.NavigateUrl = GetNavigateBackLink();
            CancelButton5.NavigateUrl = GetNavigateBackLink();

            PersonList.LoadData(@event);
            SeatDetail.LoadData(@event);
        }

        public void ShowConfirmationDetails()
        {
            var (employer, address) = EmployerDetail.GetEmployer();
            var employerContactEmail = EmployerDetail.ContactEmail;

            ConfirmPayment.ShowConfirmationDetails(
                EventIdentifier.Value,
                employer,
                address,
                employerContactEmail,
                SeatDetail.SelectedSeatIdentifier.Value,
                SeatDetail.BillingCustomer,
                People
            );

            ConfirmSection.Visible = ConfirmSection.IsSelected = true;
        }

        private bool HasAccess(QEvent @event)
        {
            return FullAccess
                || @event.Availability != EventAvailabilityType.Full && @event.Availability != EventAvailabilityType.Over;
        }

        private bool SaveData(QEvent @event)
        {
            var invoice = InvoiceIdentifier.HasValue ? new InvoiceSearch().GetInvoice((Guid)InvoiceIdentifier) : null;
            var employer = EmployerDetail.GetOrCreateEmployer();

            var users = SaveUsers(employer);
            if (users == null)
                return false;

            foreach (var user in users)
            {
                var commands = new List<ICommand>();
                var registrationIdentifier = UniqueIdentifier.Create();

                commands.Add(new RequestRegistration(registrationIdentifier, Organization.OrganizationIdentifier, EventIdentifier.Value, user, null, "Registered", null, null, null));
                commands.Add(new IncludeRegistrationToT2202(registrationIdentifier));

                if (employer != null)
                    commands.Add(new AssignEmployer(registrationIdentifier, employer.GroupIdentifier));

                commands.Add(new AssignSeat(registrationIdentifier, SelectedSeatIdentifier, SeatPrice, BillingCustomer));

                if (PaymentIdentifier.HasValue)
                    commands.Add(new AssignRegistrationPayment(registrationIdentifier, PaymentIdentifier.Value));

                foreach (var command in commands)
                    ServiceLocator.SendCommand(command);

                RegistrationHelper.SendRegistratrionCompleteAlert(registrationIdentifier, user);
            }

            if (invoice != null)
                SendPaidInvocieInformation(invoice);

            return true;
        }

        private void SendPaidInvocieInformation(VInvoice invoice)
        {
            ProductHelper.SendInvoicePaid(invoice, User.Identifier, User.TimeZone, false, false);
        }

        private List<Guid> SaveUsers(QGroup employer)
        {
            var list = ValidatePeople(People);
            if (list == null)
                return null;

            var result = new List<Guid>();

            foreach (var (item, user, person) in list)
            {
                var userId = SaveUser(employer, item, user, person);

                if (employer != null
                    && (
                        user == null
                        || !MembershipSearch.Exists(x => x.UserIdentifier == userId && x.GroupIdentifier == employer.GroupIdentifier)
                    )
                )
                {
                    MembershipHelper.Save(employer.GroupIdentifier, userId, "Employee");
                }

                result.Add(userId);
            }

            return result;
        }

        private Guid SaveUser(QGroup employer, PersonItem item, QUser user, QPerson person)
        {
            var isNewUser = user == null;
            var isNewPerson = person == null;

            if (isNewUser)
            {
                user = UserFactory.Create();
                user.FirstName = item.FirstName;
                user.LastName = item.LastName;
                user.Email = item.Email;
            }

            if (isNewPerson)
            {
                person = UserFactory.CreatePerson(Organization.OrganizationIdentifier);
                person.UserIdentifier = user.UserIdentifier;
                person.EmailEnabled = true;
                person.Region = employer?.GroupRegion;
            }

            if (item.Birthdate.HasValue && person.Birthdate == null)
                person.Birthdate = item.Birthdate;

            if (item.Phone.HasValue())
                person.Phone = item.Phone;

            if (employer != null)
                person.EmployerGroupIdentifier = employer.GroupIdentifier;

            if (item.PersonCode.HasValue() && person.PersonCode == null)
                person.PersonCode = item.PersonCode;

            if (isNewUser)
                UserStore.Insert(user, person);
            else if (isNewPerson)
                PersonStore.Insert(person);
            else
                PersonStore.Update(person);

            return user.UserIdentifier;
        }

        private void NavigateToSearch()
            => HttpResponseHelper.Redirect("/ui/portal/events/classes/search", true);

        private string GetNavigateBackLink()
            => GetOutlineLink();

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

        #endregion
    }
}
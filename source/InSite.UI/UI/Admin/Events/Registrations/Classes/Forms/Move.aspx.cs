using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Events.Read;
using InSite.Application.Gateways.Write;
using InSite.Application.Invoices.Read;
using InSite.Application.Invoices.Write;
using InSite.Application.Payments.Read;
using InSite.Application.Registrations.Read;
using InSite.Application.Registrations.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Invoices;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Portal.Events.Classes.Models;

using Shift.Common;
using Shift.Constant;

using FindEntityValueChangedEventArgs = Shift.Sdk.UI.FindEntityValueChangedEventArgs;

namespace InSite.Admin.Events.Registrations.Forms
{
    public partial class Move : AdminBasePage, IHasParentLinkParameters
    {
        private const string SearchUrl = "/ui/admin/registrations/classes/search";

        private Guid RegistrationIdentifier => Guid.TryParse(Request["id"], out var result) ? result : Guid.Empty;

        private Guid CandidateId
        {
            get => (Guid)ViewState[nameof(CandidateId)];
            set => ViewState[nameof(CandidateId)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            UniqueRegistrationValidator.ServerValidate += UniqueRegistrationValidator_ServerValidate;

            SaveButton.Click += SaveButton_Click;

            MoveToEvent.AutoPostBack = true;
            MoveToEvent.ValueChanged += MoveToEvent_ValueChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanEdit)
                HttpResponseHelper.Redirect(SearchUrl);

            var registration = ServiceLocator.RegistrationSearch.GetRegistration(RegistrationIdentifier, x => x.Event, x => x.Candidate);
            if (registration == null || registration.Event.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect(SearchUrl);

            if (string.Equals(registration.ApprovalStatus, "Cancelled", StringComparison.OrdinalIgnoreCase))
                HttpResponseHelper.Redirect(GetEditUrl());

            CandidateId = registration.CandidateIdentifier;

            PageHelper.AutoBindHeader(
                Page,
                qualifier: $"{registration.Event.EventTitle} <span class=form-text>for </span> {registration.Candidate.UserFullName}");

            ClassTitle.Text = registration.Event.EventTitle;

            MoveToEvent.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
            MoveToEvent.Filter.EventType = "Class";
            MoveToEvent.Filter.EventScheduledSince = DateTimeOffset.UtcNow;
            MoveToEvent.Filter.ExcludeEventIdentifier = registration.EventIdentifier;

            var seat = registration.SeatIdentifier.HasValue
                ? ServiceLocator.EventSearch.GetSeat(registration.SeatIdentifier.Value)
                : null;

            SeatField.Visible = seat != null;
            SeatTitle.Text = seat?.SeatTitle;

            RegistrationFeeField.Visible = registration.RegistrationFee.HasValue;
            RegistrationFee.Text = $"${registration.RegistrationFee:n2}";

            CancelButton.NavigateUrl = GetEditUrl();
        }

        private void UniqueRegistrationValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var filter = new QRegistrationFilter
            {
                EventIdentifier = MoveToEvent.Value.Value,
                CandidateIdentifier = CandidateId
            };

            args.IsValid = ServiceLocator.RegistrationSearch.CountRegistrations(filter) == 0;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
                Save();
        }

        private void MoveToEvent_ValueChanged(object sender, FindEntityValueChangedEventArgs e)
        {
            NewSeatField.Visible = MoveToEvent.Value.HasValue;

            if (MoveToEvent.Value.HasValue)
            {
                MoveToSeat.Value = null;
                MoveToSeat.EventIdentifier = MoveToEvent.Value.Value;
                MoveToSeat.RefreshData();
            }
        }

        private void Save()
        {
            var moveToEvent = MoveToEvent.HasValue ? ServiceLocator.EventSearch.GetEvent(MoveToEvent.Value.Value) : null;
            if (moveToEvent == null)
                return;

            var original = ServiceLocator.RegistrationSearch.GetRegistration(RegistrationIdentifier, x => x.Event);
            var newId = UniqueIdentifier.Create();

            var commands = new List<ICommand>();

            AddCancelCommand(moveToEvent, original.ApprovalStatus, commands);
            AddRegistrationCommands(moveToEvent, original, newId, commands);
            AddCommentCommands(moveToEvent, original, newId, commands);
            AddSeatCommands(original, newId, commands);

            var hasCredit = AddInvoiceAndCreditCommands(moveToEvent, original, newId, commands);

            ServiceLocator.SendCommands(commands);

            if (hasCredit)
                RegistrationHelper.SendRegistratrionCompleteAlert(newId, original.CandidateIdentifier);

            HttpResponseHelper.Redirect(GetEditUrl(newId));
        }

        private void AddCancelCommand(QEvent moveToEvent, string approvalStatus, List<ICommand> commands)
        {
            commands.Add(new CancelRegistration(RegistrationIdentifier, $"Moved to {moveToEvent.EventTitle}", false));
            commands.Add(new ChangeApproval(RegistrationIdentifier, "Moved", null, null, approvalStatus));
        }

        private void AddRegistrationCommands(QEvent moveToEvent, QRegistration original, Guid newId, List<ICommand> commands)
        {
            commands.Add(new RequestRegistration(
                newId,
                Organization.OrganizationIdentifier,
                moveToEvent.EventIdentifier,
                original.CandidateIdentifier,
                original.AttendanceStatus,
                original.ApprovalStatus,
                original.RegistrationFee,
                original.RegistrationComment,
                original.RegistrationSource
            ));

            if (original.RegistrationRequestedBy != User.Identifier)
                commands.Add(new ModifyRegistrationRequestedBy(newId, original.RegistrationRequestedBy));

            var instructors = ServiceLocator.RegistrationSearch.GetInstructors(original.RegistrationIdentifier);
            foreach (var instructor in instructors)
                commands.Add(new AddInstructor(newId, instructor.UserIdentifier));

            if (original.CustomerIdentifier.HasValue)
                commands.Add(new AssignCustomer(newId, original.CustomerIdentifier));

            if (original.EmployerIdentifier.HasValue)
                commands.Add(new AssignEmployer(newId, original.EmployerIdentifier));

            if (original.WorkBasedHoursToDate.HasValue)
                commands.Add(new AssignRegistrationHoursWorked(newId, original.WorkBasedHoursToDate));

            if (original.SchoolIdentifier.HasValue)
                commands.Add(new AssignSchool(newId, original.SchoolIdentifier.Value));

            var accommodations = ServiceLocator.RegistrationSearch.GetAccommodations(original.RegistrationIdentifier);
            foreach (var accommodation in accommodations)
                commands.Add(new GrantAccommodation(newId, accommodation.AccommodationType, accommodation.AccommodationName, accommodation.TimeExtension));

            if (original.Grade.HasValue() || original.Score.HasValue)
                commands.Add(new ChangeGrade(newId, original.Grade, original.Score));

            commands.Add(new LimitExamTime(newId));

            if (original.IncludeInT2202)
                commands.Add(new IncludeRegistrationToT2202(newId));
        }

        private void AddCommentCommands(QEvent moveToEvent, QRegistration original, Guid newId, List<ICommand> commands)
        {
            var commentLineTo = $"({DateTime.UtcNow:yyyy-MM-dd}): Moved from '{original.Event.EventTitle}'";

            var newComment = !string.IsNullOrEmpty(original.RegistrationComment)
                ? original.RegistrationComment + "\r\n\r\n" + commentLineTo
                : commentLineTo;

            commands.Add(new CommentRegistration(newId, newComment));

            var commentLineFrom = $"({DateTime.UtcNow:yyyy-MM-dd}): Moved to '{moveToEvent.EventTitle}'";

            var newCommentFrom = !string.IsNullOrEmpty(original.RegistrationComment)
                ? original.RegistrationComment + "\r\n\r\n" + commentLineFrom
                : commentLineFrom;

            commands.Add(new CommentRegistration(original.RegistrationIdentifier, newCommentFrom));
        }

        private void AddSeatCommands(QRegistration original, Guid newId, List<ICommand> commands)
        {
            var seatId = MoveToSeat.ValueAsGuid;
            if (seatId == null)
                return;

            commands.Add(new AssignSeat(newId, seatId.Value, original.RegistrationFee, original.BillingCustomer));
        }

        private bool AddInvoiceAndCreditCommands(QEvent moveToEvent, QRegistration original, Guid newRegistrationId, List<ICommand> commands)
        {
            if (MoveToSeat.ValueAsGuid == null || original.SeatIdentifier == null || original.RegistrationFee == null)
                return false;

            var originalPayment = original.PaymentIdentifier.HasValue
                ? ServiceLocator.PaymentSearch.GetPayment(original.PaymentIdentifier.Value)
                : null;

            if (originalPayment == null)
                return false;

            var originalInvoice = ServiceLocator.InvoiceSearch.GetInvoice(originalPayment.InvoiceIdentifier);
            var registrtaionFee = original.RegistrationFee.Value;

            AddInvoiceCommands(moveToEvent, original, newRegistrationId, originalPayment, originalInvoice, registrtaionFee, commands);
            AddCreditCommands(moveToEvent, original, originalPayment, originalInvoice, registrtaionFee, commands);

            return true;
        }

        private void AddInvoiceCommands(
            QEvent moveToEvent,
            QRegistration original,
            Guid newRegistrationId,
            QPayment originalPayment,
            VInvoice originalInvoice,
            decimal registrtaionFee,
            List<ICommand> commands
            )
        {
            var invoiceId = UniqueIdentifier.Create();
            var invoiceNumber = Sequence.Increment(moveToEvent.OrganizationIdentifier, SequenceType.Invoice);
            var productId = GetOrCreateProduct(moveToEvent);
            var user = UserSearch.Select(original.CandidateIdentifier);
            var paymentDescription = $"{moveToEvent.EventTitle} activity registration for {user.FullName}";

            commands.Add(new DraftInvoice(invoiceId, moveToEvent.OrganizationIdentifier, invoiceNumber, originalInvoice.CustomerIdentifier, new[]
            {
                new InvoiceItem
                {
                    Identifier = UuidFactory.Create(),
                    Product = productId,
                    Quantity = 1,
                    Price = registrtaionFee,
                    Description = paymentDescription
                }
            }));
            commands.Add(new SubmitInvoice(invoiceId));

            var reference = new ReferenceInvoice(invoiceId, originalPayment.InvoiceIdentifier);
            commands.Add(reference);

            var paymentId = UniqueIdentifier.Create();
            var orderNumber = Invoice.FormatInvoiceNumber(invoiceNumber);

            var importPayment = new ImportPayment(
                PaymentIdentifiers.BamboraGateway,
                originalPayment.OrganizationIdentifier,
                invoiceId,
                paymentId,
                UserIdentifiers.Someone,
                orderNumber,
                PaymentStatus.Completed,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow,
                registrtaionFee,
                null,
                null,
                null,
                null
            );

            commands.Add(importPayment);

            commands.Add(new AssignRegistrationPayment(newRegistrationId, paymentId));
        }

        private void AddCreditCommands(
            QEvent moveToEvent,
            QRegistration original,
            QPayment payment,
            VInvoice invoice,
            decimal registrtaionFee,
            List<ICommand> commands
            )
        {
            var invoiceId = UniqueIdentifier.Create();
            var invoiceNumber = Sequence.Increment(moveToEvent.OrganizationIdentifier, SequenceType.Invoice);

            var originalInvoiceNumber = invoice.InvoiceNumber.HasValue
                ? Invoice.FormatInvoiceNumber(invoice.InvoiceNumber.Value)
                : invoice.InvoiceIdentifier.ToString();

            var paymentDescription = $"Credit for invoice #{originalInvoiceNumber}";
            var product = ServiceLocator.InvoiceSearch.GetProduct(original.SeatIdentifier.Value);

            commands.Add(new DraftInvoice(invoiceId, moveToEvent.OrganizationIdentifier, invoiceNumber, invoice.CustomerIdentifier, new[]
            {
                new InvoiceItem
                {
                    Identifier = UuidFactory.Create(),
                    Product = product.ProductIdentifier,
                    Quantity = 1,
                    Price = -registrtaionFee,
                    Description = paymentDescription
                }
            }));
            commands.Add(new SubmitInvoice(invoiceId));

            var reference = new ReferenceInvoice(invoiceId, payment.InvoiceIdentifier);
            commands.Add(reference);

            var paymentId = UniqueIdentifier.Create();
            var orderNumber = Invoice.FormatInvoiceNumber(invoiceNumber);

            var importPayment = new ImportPayment(
                PaymentIdentifiers.BamboraGateway,
                payment.OrganizationIdentifier,
                invoiceId,
                paymentId,
                UserIdentifiers.Someone,
                orderNumber,
                PaymentStatus.Completed,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow,
                -registrtaionFee,
                null,
                null,
                null,
                null
            );

            commands.Add(importPayment);
        }

        private Guid GetOrCreateProduct(QEvent moveToEvent)
        {
            var seatIdentifier = MoveToSeat.ValueAsGuid.Value;
            var product = ServiceLocator.InvoiceSearch.GetProduct(seatIdentifier);

            if (product != null)
                return seatIdentifier;

            var seat = ServiceLocator.EventSearch.GetSeat(seatIdentifier);

            product = new TProduct
            {
                ProductIdentifier = seatIdentifier,
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                ProductType = "Seat",
                ProductName = seat.SeatTitle,
                ProductDescription = $"{moveToEvent.EventTitle}",
                CreatedBy = User.Identifier,
                ModifiedBy = User.Identifier
            };

            ServiceLocator.InvoiceStore.InsertProduct(product);

            return seatIdentifier;
        }

        private string GetEditUrl(Guid? id = null) => $"/ui/admin/registrations/classes/edit?id={id ?? RegistrationIdentifier}";

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/edit") ? $"id={RegistrationIdentifier}" : null;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

using InSite.Admin.Contacts.People.Utilities;
using InSite.Application.Contacts.Read;
using InSite.Application.Gateways.Write;
using InSite.Application.Groups.Write;
using InSite.Application.Invoices.Read;
using InSite.Application.People.Write;
using InSite.Common;
using InSite.Common.Web;
using InSite.Domain.Payments;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;
using InSite.UI.Portal.Billing.Models;
using InSite.UI.Portal.Billing.Utilities;
using InSite.Web.Data;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

using static InSite.UI.Portal.Billing.Models.PriceSelectionModel;

namespace InSite.UI.Portal.Billing
{
    public partial class Checkout : PortalBasePage
    {
        #region Properties

        private Guid? PaymentIdentifier
        {
            get => (Guid?)ViewState[nameof(PaymentIdentifier)];
            set => ViewState[nameof(PaymentIdentifier)] = value;
        }

        private bool IsNewUser => (bool)(ViewState[nameof(IsNewUser)]
            ?? (ViewState[nameof(IsNewUser)] = !Identity.IsAuthenticated));

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SubmitButton.Click += SubmitButton_Click;

            BillState.AutoPostBack = true;
            BillState.ValueChanged += BillState_ValueChanged;

            ValidCart.ServerValidate += ValidCart_ServerValidate;
            ValidCreditCard.ServerValidate += ValidCreditCard_ServerValidate;
            UniqueEmail.ServerValidate += UniqueEmail_ServerValidate;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHeader.Visible = IsNewUser;
            AccountInfo.Visible = IsNewUser;

            ContinueShopping.HRef = Identity.IsAuthenticated
                ? "/ui/portal/management/dashboard/catalog"
                : "/ui/portal/billing/catalog";

            var cart = CartStorage.Get();

            SubmitButton.Visible = IsCartNotEmpty();

            BindSummary();
            PortalMaster.ShowAvatar();
            PageHelper.AutoBindHeader(this, null, "Checkout");
            PortalMaster.HideBreadcrumbsOnly();

            if (Identity.IsAuthenticated)
                OverrideHomeLink("/ui/portal/management/dashboard/home");
            else
                OverrideHomeLink("/ui/portal/billing/catalog");
        }

        #endregion

        #region UI Event Handling

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            const string homeUrl = "/ui/portal/management/dashboard/home?added=1";

            var loginUrl = $"/ui/lobby/signin?returnurl={HttpUtility.UrlEncode(homeUrl)}";

            if (PaymentIdentifier != null)
                HttpResponseHelper.Redirect(loginUrl);

            if (!IsValid || !DoCheckout())
                return;

            if (!IsNewUser)
                HttpResponseHelper.Redirect(homeUrl);

            SummaryPanel.Visible = false;
            SubmitButton.Visible = false;
            PageHeader.Visible = false;
            PaymentSuccess.Visible = true;
        }

        private void BillState_ValueChanged(object sender, ComboBoxValueChangedEventArgs e)
            => BindSummary();

        private void ValidCart_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = IsCartNotEmpty();
            if (!args.IsValid)
                ValidCart.ErrorMessage = Translate("Cart is empty.");
        }

        private void ValidCreditCard_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var card = GetCreditCardDetails();

            args.IsValid = card.IsValid;
            ValidCreditCard.ErrorMessage = card.ErrorMessage;
        }

        private void UniqueEmail_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = !ServiceLocator.UserSearch.IsUserExist(args.Value.Trim());

            if (!args.IsValid)
            {
                var returnUrl = HttpUtility.UrlEncode(Request.RawUrl);
                var loginUrl = $"/ui/lobby/signin?ReturnUrl={returnUrl}";

                UniqueEmail.ErrorMessage = $"The email '{args.Value}' is already in use. " +
                    $"<a href=\"{loginUrl}\">You need to login before you can complete the purchase</a>.";
            }
        }

        #endregion

        #region Payment processing

        private bool DoCheckout()
        {
            var userId = IsNewUser ? CreateUser() : User.Identifier;
            var cart = CartStorage.Get();
            var orders = OrderHelper.BuildOrdersFromCart(cart, userId);

            if (!ProcessPayment(userId, cart, orders))
            {
                if (IsNewUser)
                    UserStore.Delete(userId);

                return false;
            }

            if (IsNewUser)
            {
                GrantAccessForUser(userId);
                AttachPaymentToUser(userId);
            }

            var classEvents = CreateClasses(cart, orders);

            var distributions = DistributionHelper.BuildDistributionsFromOrders(
                orders,
                classEvents,
                cart.Mode,
                userId,
                DateTimeOffset.UtcNow
            );

            if (distributions.Count > 0)
                ServiceLocator.CourseDistributionStore.InsertCourseDistributions(distributions);

            SendInvoiceMessage(orders);
            SavePurchasedCount(userId, cart, orders);

            cart.Reset(PriceSelectionMode.ALaCarte, null, null);

            UpdateHeader();

            return true;
        }

        private void SavePurchasedCount(Guid userId, CartState cart, List<TOrder> orders)
        {
            if (cart.Mode != PriceSelectionMode.ALaCarte)
                return;

            var itemsCount = orders.SelectMany(x => x.OrderItems).Sum(x => x.OrderItemQuantity);
            if (itemsCount > 0)
            {
                var existAmount = TPersonFieldSearch.GetSkillsCheckPurchasedCount(Organization.OrganizationIdentifier, userId);
                TPersonFieldStore.SetSkillsCheckPurchasedCount(Organization.OrganizationIdentifier, userId, existAmount + itemsCount);
            }
        }

        private bool ProcessPayment(Guid userId, CartState cart, List<TOrder> orders)
        {
            var card = GetCreditCardDetails();
            var taxRate = GetSelectedProvinceTaxRate();

            var paymentId = InvoiceHelper.ProcessOrderPayment(
                orders,
                userId,
                cart.Mode,
                taxRate,
                card
            );

            var error = InSite.UI.Portal.Events.Classes.Register.CheckPaymentStatus(paymentId);
            if (!string.IsNullOrEmpty(error))
            {
                EditorStatus.AddMessage(AlertType.Error, $"Payment Failed: {error}");
                return false;
            }

            PaymentIdentifier = paymentId;

            return true;
        }

        private Guid CreateUser()
        {
            var user = UserFactory.Create();
            user.Email = Email.Text.Trim();
            user.FirstName = FirstName.Text;
            user.LastName = LastName.Text;
            user.TimeZone = Organization.TimeZone.Id;

            UserStore.Insert(user, Organization.Toolkits?.Contacts?.FullNamePolicy);

            return user.UserIdentifier;
        }

        private void GrantAccessForUser(Guid userId)
        {
            var newPerson = UserFactory.CreatePerson(Organization.Identifier);
            newPerson.UserIdentifier = userId;
            newPerson.EmailEnabled = true;
            newPerson.EmployerGroupIdentifier = GetOrCreateEmployerGroupId();

            PersonStore.Insert(newPerson);

            var person = ServiceLocator.PersonSearch.GetPerson(userId, Organization.Identifier, x => x.User);

            ServiceLocator.SendCommand(new GrantPersonAccess(person.PersonIdentifier, DateTimeOffset.Now, "SkillsCheck Checkout"));

            if (newPerson.EmployerGroupIdentifier.HasValue)
                MembershipHelper.Save(newPerson.EmployerGroupIdentifier.Value, userId, MembershipType.Employee);

            if (Identity?.Organization?.Toolkits?.Sales?.ManagerGroup.HasValue == true)
                MembershipHelper.Save(Identity.Organization.Toolkits.Sales.ManagerGroup.Value, userId, null);

            PersonHelper.SendAccountCreated(Organization.OrganizationIdentifier, Organization.LegalName, person.User, person);
        }

        private void AttachPaymentToUser(Guid userId)
        {
            ServiceLocator.SendCommand(new ModifyPaymentCreatedBy(PaymentIdentifiers.BamboraGateway, PaymentIdentifier.Value, userId));
        }

        private Guid? GetOrCreateEmployerGroupId()
        {
            var groupName = Company.Text.Trim();
            if (groupName.IsEmpty())
                return null;

            var filter = new QGroupFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                GroupName = groupName,
                GroupType = GroupTypes.Employer
            };

            var groupId = ServiceLocator.GroupSearch.GetGroups(filter).FirstOrDefault()?.GroupIdentifier;
            if (groupId == null)
            {
                groupId = UniqueIdentifier.Create();
                ServiceLocator.SendCommand(new CreateGroup(groupId.Value, Organization.Identifier, GroupTypes.Employer, groupName));
            }

            return groupId;
        }

        #endregion

        #region Data operations

        private void BindSummary()
        {
            var cart = CartStorage.Get();

            if (cart.Items.Count == 0)
            {
                EmptyState.Visible = true;
                SummaryRepeater.DataSource = null;
                SummaryRepeater.DataBind();
                SetTotals(0m);
                UpdateHeader();
                return;
            }

            EmptyState.Visible = false;
            PackageHeaderBlock.Visible = false;

            switch (cart.Mode)
            {
                case PriceSelectionMode.ALaCarte:
                    BindAlaCarte(cart);
                    break;

                case PriceSelectionMode.Subscribe:
                    BindSubscribe(cart);
                    break;

                case PriceSelectionMode.Package:
                    BindPackage(cart);
                    break;

                default:
                    EmptyState.Visible = true;
                    SummaryRepeater.DataSource = null;
                    SummaryRepeater.DataBind();
                    SetTotals(0m);
                    break;
            }

            UpdateHeader();
        }

        private void BindAlaCarte(CartState cart)
        {
            var (lines, subtotal) = BuildLines(cart, showLinePrices: true, addPackSuffix: false);
            SummaryRepeater.DataSource = lines;
            SummaryRepeater.DataBind();

            BindTotalsForCart(cart);
        }

        private void BindSubscribe(CartState cart)
        {
            var (lines, subtotal) = BuildLines(cart, showLinePrices: true, addPackSuffix: true);
            SummaryRepeater.DataSource = lines;
            SummaryRepeater.DataBind();

            BindTotalsForCart(cart);
        }

        private void BindPackage(CartState cart)
        {
            var package = cart.PackageProductId.HasValue
                ? ServiceLocator.InvoiceSearch.GetProduct(cart.PackageProductId.Value)
                : null;

            var packName = package?.ProductName ?? "Package";
            var packSize = package?.ProductQuantity;
            var packPrice = package?.ProductPrice ?? 0m;

            PackageHeaderBlock.Visible = true;
            PackageHeaderText.Text = $"{packName} ({packSize}-pack) – {Currency(packPrice)}";

            var (lines, _) = BuildLines(cart, showLinePrices: false, addPackSuffix: false);
            SummaryRepeater.DataSource = lines;
            SummaryRepeater.DataBind();

            BindTotalsForCart(cart);
        }

        private void BindTotalsForCart(CartState cart)
        {
            var subtotal = ComputeSubtotal(cart);
            var taxableBase = ComputeTaxableBase(cart);
            var taxRate = GetSelectedProvinceTaxRate();
            var tax = Math.Round(taxableBase * taxRate, 2, MidpointRounding.AwayFromZero);
            var total = subtotal + tax;

            SubtotalLit.InnerText = Currency(subtotal);
            TaxLit.InnerText = Currency(tax);
            TotalLit.InnerText = Currency(total);
        }

        private (IEnumerable<CartProductItem> lines, decimal subtotal) BuildLines(
            CartState cart,
            bool showLinePrices,
            bool addPackSuffix)
        {
            var list = new List<CartProductItem>(cart.Items.Count);
            decimal subtotal = 0m;

            foreach (var kv in cart.Items)
            {
                var pid = kv.Key;
                var qty = kv.Value;

                var p = ServiceLocator.InvoiceSearch.GetProduct(pid);
                var name = p?.ProductName ?? "(Unknown)";

                if (addPackSuffix && (p?.ProductQuantity ?? 0) > 0)
                    name = $"{name} ({p.ProductQuantity}-pack)";

                decimal unit = p?.ProductPrice ?? 0m;
                decimal lineTotal = showLinePrices ? unit * qty : 0m;

                subtotal += (showLinePrices ? lineTotal : 0m);

                list.Add(new CartProductItem
                {
                    ProductId = pid,
                    Name = name,
                    Url = "/",
                    Qty = qty,
                    UnitPrice = lineTotal
                });
            }

            list = list.OrderBy(x => x.Name).ToList();
            return (list, subtotal);
        }

        private static List<(Guid, Guid, Guid)> CreateClasses(CartState cart, List<TOrder> orders)
        {
            if (cart.Mode != PriceSelectionMode.ALaCarte && cart.Mode != PriceSelectionMode.Package)
                return new List<(Guid, Guid, Guid)>();

            var classEventVenueGroup = Organization.Toolkits.Sales?.ProductClassEventVenueGroup;
            var invoiceId = orders.FirstOrDefault().InvoiceIdentifier;

            return ProductHelper.CreateClassEvent(invoiceId, classEventVenueGroup);
        }

        private void SendInvoiceMessage(List<TOrder> orders)
        {
            var invoiceId = orders.FirstOrDefault().InvoiceIdentifier;
            ProductHelper.SendPaidInvoiceInformation(invoiceId, IsNewUser);
        }

        #endregion

        #region Helper Methods

        private bool IsCartNotEmpty()
        {
            var cart = CartStorage.Get();

            return (cart.Mode == PriceSelectionMode.ALaCarte || cart.Mode == PriceSelectionMode.Subscribe) && cart.Items.Count > 0
                || cart.Mode == PriceSelectionMode.Package && cart.PackageProductId.HasValue;

        }

        private void SetTotals(decimal amount)
        {
            SubtotalLit.InnerText = Currency(amount);
            TotalLit.InnerText = Currency(amount);
        }

        private static string Currency(decimal amount) => string.Format("{0:C}", amount);

        private decimal ComputeSubtotal(CartState cart)
        {
            switch (cart.Mode)
            {
                case PriceSelectionMode.ALaCarte:
                    return cart.Items.Sum(kv =>
                    {
                        var p = ServiceLocator.InvoiceSearch.GetProduct(kv.Key);
                        var unit = p?.ProductPrice ?? 0m;
                        return unit * kv.Value;
                    });

                case PriceSelectionMode.Package:
                    {
                        if (!cart.PackageProductId.HasValue) return 0m;
                        var pkg = ServiceLocator.InvoiceSearch.GetProduct(cart.PackageProductId.Value);
                        return pkg?.ProductPrice ?? 0m;
                    }

                case PriceSelectionMode.Subscribe:
                    return cart.Items.Sum(kv =>
                    {
                        var pkg = ServiceLocator.InvoiceSearch.GetProduct(kv.Key);
                        var unit = pkg?.ProductPrice ?? 0m;
                        return unit * kv.Value;
                    });

                default: return 0m;
            }
        }

        private decimal ComputeTaxableBase(CartState cart)
        {
            switch (cart.Mode)
            {
                case PriceSelectionMode.ALaCarte:
                    return cart.Items.Sum(kv =>
                    {
                        var p = ServiceLocator.InvoiceSearch.GetProduct(kv.Key);
                        if (p == null || !p.IsTaxable) return 0m;
                        var unit = p.ProductPrice ?? 0m;
                        return unit * kv.Value;
                    });

                case PriceSelectionMode.Package:
                    {
                        if (!cart.PackageProductId.HasValue) return 0m;
                        var pkg = ServiceLocator.InvoiceSearch.GetProduct(cart.PackageProductId.Value);
                        if (pkg == null || !pkg.IsTaxable) return 0m;
                        return pkg.ProductPrice ?? 0m;
                    }

                case PriceSelectionMode.Subscribe:
                    return cart.Items.Sum(kv =>
                    {
                        var pkg = ServiceLocator.InvoiceSearch.GetProduct(kv.Key);
                        if (pkg == null || !pkg.IsTaxable) return 0m;
                        var unit = pkg.ProductPrice ?? 0m;
                        return unit * kv.Value;
                    });

                default: return 0m;
            }
        }

        private decimal GetSelectedProvinceTaxRate()
        {
            var region = BillState.Value;
            if (string.IsNullOrWhiteSpace(region))
            {
                TaxPercentage.Text = "Calculated at Checkout";
                return 0m;
            }

            var orgId = Organization.Identifier;
            var taxes = ServiceLocator.InvoiceSearch.GetTaxes(new TTaxFilter
            {
                OrganizationIdentifier = orgId,
                RegionCode = region
            }) ?? new List<TTax>();

            var rate = taxes.Sum(t => t.TaxRate);
            if (rate > 1m) rate /= 100m;

            var pct = rate * 100m;
            TaxPercentage.Text = (pct % 1m == 0m) ? $"{pct:0}%" : $"{pct:0.##}%";

            return Math.Max(0m, rate);
        }

        private UnmaskedCreditCard GetCreditCardDetails()
        {
            var card = new UnmaskedCreditCard();
            CreditCardCreator.Create(card, CardName.Text, CardNumber.Text, CardExpiry.Text, CardCvc.Text, LabelHelper.GetTranslation);
            return card;
        }

        #endregion

        #region View Updates

        private void UpdateHeader()
        {
            if (Master is PortalMaster pm)
                pm.UpdateHeaderCartBadge();
        }

        #endregion
    }
}

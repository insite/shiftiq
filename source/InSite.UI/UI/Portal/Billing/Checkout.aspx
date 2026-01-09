<%@ Page Language="C#" CodeBehind="Checkout.aspx.cs" Inherits="InSite.UI.Portal.Billing.Checkout" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="../../Portal/Home/Management/Controls/DashboardNavigation.ascx" TagName="DashboardNavigation" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style>
        .cardish {
            background: #fff;
            border: 1px solid rgba(0,0,0,.08);
            border-radius: .75rem;
            box-shadow: 0 .125rem .5rem rgba(0,0,0,.06);
        }

        .flex-same-size {
            flex-grow: 1;
            flex-shrink: 1;
            flex-basis: 0;
        }

        .payment-overlay {
            width: 100%;
            height: 100%;
            position: absolute;
            top: 0;
            left: 0;
            background: rgba( 205, 205, 205, 0.4 )  url('/images/animations/loader.gif') 50% 50% no-repeat;
            z-index: 9000;
        }

        .no-scroll {
            overflow: hidden;
        }
    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">
    <uc:DashboardNavigation runat="server" ID="DashboardNavigation" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <div runat="server" id="PaymentOverlay"></div>

    <insite:Alert runat="server" ID="EditorStatus" />

    <div runat="server" id="PaymentSuccess" visible="false" enableviewstate="false" class="alert d-flex bg-primary text-white" role="alert">
        <i class="fa-solid fa-check-circle fs-xl mt-1 me-3"></i>
        <div>
            Payment was successfull.<br>
            You will receive email with login instructions shortly.
        </div>
    </div>

    <insite:ValidationSummary runat="server" ValidationGroup="Checkout" />

    <asp:CustomValidator runat="server" ID="ValidCart" Display="None" ValidationGroup="Checkout" />
    <asp:CustomValidator runat="server" ID="ValidCreditCard" Display="None" ValidationGroup="Checkout" />
    <asp:CustomValidator runat="server" ID="UniqueEmail" Display="None" ControlToValidate="Email" ValidationGroup="Checkout" />

    <div runat="server" id="PageHeader" class="row">
        <div class="col-12">
            <h2 class="mb-2">Checkout</h2>
            <hr class="my-2 text-secondary" />
        </div>
    </div>

    <asp:UpdatePanel ID="SummaryPanel" runat="server">
        <ContentTemplate>

            <div class="row">
                <div class="col-xl-8 mb-3">
                    <!-- Column 1: Order Summary -->
                    <asp:PlaceHolder runat="server" ID="PackageHeaderBlock" Visible="false">
                        <div class="fw-semibold mb-3">
                            <asp:Literal runat="server" ID="PackageHeaderText" />
                        </div>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder runat="server" ID="EmptyState" Visible="false">
                        <div class="alert alert-info mt-2">Your cart is empty.</div>
                    </asp:PlaceHolder>

                    <asp:Repeater runat="server" ID="SummaryRepeater">
                        <ItemTemplate>
                            <div class="d-flex justify-content-between align-items-start py-2">
                                <div class="pe-3">
                                    <span class="fw-semibold"><%# Eval("Name") %></span>
                                    <span class="text-muted ms-2">× <%# Eval("Qty") %></span>
                                </div>
                                <div class="text-nowrap">
                                    <asp:Literal runat="server" ID="LinePriceLit" Text='<%# Eval("PriceFormatted") %>' />
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                </div>
                <div class="col-xl-4 mb-3 pt-2">

                    <div class="d-flex justify-content-between mb-1">
                        <span>Subtotal</span>
                        <span runat="server" ID="SubtotalLit"></span>
                    </div>

                    <div class="d-flex justify-content-between mb-1">
                        <span>Tax <span class="fst-italic text-muted">(<asp:Literal runat="server" ID="TaxPercentage" Text="Calculated at Checkout" />)</span></span>
                        <span runat="server" ID="TaxLit"></span>
                    </div>

                    <div class="d-flex justify-content-between fw-semibold">
                        <span>Total</span>
                        <span runat="server" ID="TotalLit"></span>
                    </div>

                </div>
            </div>

            <div class="row mt-3">
                <div class="col-xl-8 mb-3 mb-xl-0">
                    <!-- Column 2: Payment -->
                    <div class="cardish p-4 h-100">
                        <div class="row">
                            <div class="col-xl-6">

                                <h5 class="mb-3">Payment</h5>

                                <div class="mb-3">
                                    <label class="form-label">
                                        Name on Card
                                        <insite:RequiredValidator runat="server" FieldName="Card Name" ControlToValidate="CardName" ValidationGroup="Checkout" />
                                    </label>
                                    <insite:TextBox runat="server" ID="CardName" CssClass="form-control" MaxLength="32" />
                                </div>

                                <div class="mb-3">
                                    <label class="form-label">
                                        Credit Card Number
                                        <insite:RequiredValidator runat="server" FieldName="Card Number" ControlToValidate="CardNumber" ValidationGroup="Checkout" />
                                    </label>
                                    <insite:TextBox runat="server" ID="CardNumber" CssClass="form-control" MaxLength="19" EmptyMessage="0000 0000 0000 0000"
                                        data-format="{&quot;creditCard&quot;:true}" />
                                </div>

                                <div class="row">
                                    <div class="col-6 mb-3">
                                        <label class="form-label">
                                            MM / YY
                                            <insite:RequiredValidator runat="server" FieldName="Expiry Date" ControlToValidate="CardExpiry" ValidationGroup="Checkout" />
                                        </label>
                                        <insite:TextBox runat="server" ID="CardExpiry" CssClass="form-control" MaxLength="5" EmptyMessage="mm/yy"
                                            data-format="{&quot;date&quot;:true,&quot;datePattern&quot;:[&quot;m&quot;,&quot;y&quot;]}" />
                                    </div>
                                    <div class="col-6 mb-3">
                                        <label class="form-label">
                                            CVC
                                            <insite:RequiredValidator runat="server" FieldName="Cvc" ControlToValidate="CardCvc" ValidationGroup="Checkout" />
                                        </label>
                                        <insite:TextBox runat="server" ID="CardCvc" CssClass="form-control" MaxLength="4" EmptyMessage="000" />
                                    </div>
                                </div>

                            </div>
                            <div class="col-xl-6">

                                <h5 class="mb-3">Billing Address</h5>

                                <div class="mb-3">
                                    <label class="form-label">
                                        Country
                                        <insite:RequiredValidator runat="server" FieldName="Country" ControlToValidate="BillCountry" ValidationGroup="Checkout" />
                                    </label>
                                    <insite:TextBox runat="server" ID="BillCountry" CssClass="form-control" Text="Canada" Enabled="false" />
                                </div>
                                <div class="mb-3">
                                    <label class="form-label">
                                        Street
                                        <insite:RequiredValidator runat="server" FieldName="Street" ControlToValidate="BillStreet" ValidationGroup="Checkout" />
                                    </label>
                                    <insite:TextBox runat="server" ID="BillStreet" CssClass="form-control" MaxLength="50" />
                                </div>

                                <div class="mb-3">
                                    <label class="form-label">
                                        City
                                        <insite:RequiredValidator runat="server" FieldName="City" ControlToValidate="BillCity" ValidationGroup="Checkout" />
                                    </label>
                                    <insite:TextBox runat="server" ID="BillCity" CssClass="form-control" MaxLength="50" />
                                </div>

                                <div class="mb-3">
                                    <label class="form-label">
                                        Province
                                        <insite:RequiredValidator runat="server" FieldName="Province" ControlToValidate="BillState" ValidationGroup="Checkout" />
                                    </label>
                                    <insite:ProvinceComboBox ID="BillState" runat="server" EnableSearch="true" UseCodeAsValue="true" OnlyCanada="true" />
                                </div>
                                <div class="mb-3">
                                    <label class="form-label">
                                        Postal
                                        <insite:RequiredValidator runat="server" FieldName="Postal Code" ControlToValidate="BillPostal" ValidationGroup="Checkout" />
                                    </label>
                                    <insite:TextBox runat="server" ID="BillPostal" CssClass="form-control" MaxLength="7"
                                        data-format='{"blocks":[3,3],"delimiter":" ","uppercase":true}' />
                                </div>

                            </div>
                        </div>

                    </div>

                </div>
                <div runat="server" id="AccountInfo" class="col-xl-4">
                    <!-- Column 3: Account info -->
                    <div class="cardish p-4 h-100">
                        <h5 class="mb-3">SkillsCheck Account Info</h5>

                        <div class="mb-3">
                            <label class="form-label">
                                First Name
                                <insite:RequiredValidator runat="server" FieldName="First Name" ControlToValidate="FirstName" ValidationGroup="Checkout" />
                            </label>
                            <insite:TextBox runat="server" ID="FirstName" CssClass="form-control" MaxLength="40" />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">
                                Last Name
                                <insite:RequiredValidator runat="server" FieldName="Last Name" ControlToValidate="LastName" ValidationGroup="Checkout" />
                            </label>
                            <insite:TextBox runat="server" ID="LastName" CssClass="form-control" MaxLength="40" />
                        </div>


                        <div class="mb-3">
                            <label class="form-label">
                                Company
                                <insite:RequiredValidator runat="server" FieldName="Company" ControlToValidate="Company" ValidationGroup="Checkout" />
                            </label>
                            <insite:TextBox runat="server" ID="Company" CssClass="form-control" MaxLength="90" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label">
                                Email Address
                                <insite:RequiredValidator runat="server" FieldName="Email" ControlToValidate="Email" ValidationGroup="Checkout" />
                            </label>
                            <insite:TextBox runat="server" ID="Email" CssClass="form-control" TextMode="Email" MaxLength="254" />
                        </div>
                    </div>

                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

    <div class="mt-4 text-end">
        <a runat="server" id="ContinueShopping" class="btn btn-outline-primary me-2">Continue Shopping</a>
        <insite:Button runat="server" ID="SubmitButton" Text="Place Your Order" Size="Default" ButtonStyle="Primary" ValidationGroup="Checkout" />
    </div>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (function () {
                let saveIntervalHandler = null;

                const submitButton = document.getElementById("<%= SubmitButton.ClientID %>");

                submitButton.addEventListener("click", e => {
                    if (this.disabled) {
                        e.preventDefault();
                        return;
                    }

                    Page_IsValid = null;

                    lockScreen(true);

                    window.scrollTo({ top: 0 });

                    saveIntervalHandler = setInterval(onSaveValidationInterval, 500);
                });

                function onSaveValidationInterval() {
                    if (Page_IsValid === null) {
                        return;
                    }

                    clearInterval(saveIntervalHandler);

                    if (Page_IsValid === true) {
                        return;
                    }

                    lockScreen(false);
                }

                function lockScreen(show) {
                    const overlay = document.getElementById("<%= PaymentOverlay.ClientID %>");
                    const overlayClass = "payment-overlay";
                    if (show !== false) {
                        document.body.classList.add("no-scroll");
                        overlay.classList.add(overlayClass);
                        submitButton.disabled = true;
                    } else {
                        document.body.classList.remove("no-scroll");
                        overlay.classList.remove(overlayClass);
                        submitButton.disabled = false;
                    }
                }
            })();
        </script>
    </insite:PageFooterContent>
</asp:Content>

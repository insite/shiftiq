<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PriceSelector.ascx.cs" Inherits="InSite.UI.Portal.Billing.PriceSelector" %>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        .card-buttons .btn { --ar-btn-disabled-bg: var(--ar-btn-color); --ar-btn-disabled-color: var(--ar-btn-bg); }
    </style>
</insite:PageHeadContent>

<div class="row">
    <div class="col-lg-3 d-flex">
        <div class="my-lg-auto w-100 text-end mb-3">
            Build Package
        </div>
    </div>

    <div class="col-lg-5 d-flex">
        <div class="my-auto w-100">
            <insite:ProductPriceSelectorComboBox runat="server" ID="ProductPriceSelectorComboBox" AllowBlank="false" />
        </div>
    </div>

    <div class="col-lg-3 mt-3 mt-lg-0">
        <div class="d-grid gap-2 card-buttons">
            <insite:Button runat="server" ID="CartButton" Enabled="false"
                CssClass="btn btn-primary fw-semibold rounded-pill h-44 btn-xs"
                Text="View Cart">
            </insite:Button>
            <insite:Button runat="server" ID="CheckoutButton" Enabled="true"
                CssClass="btn btn-primary fw-semibold rounded-pill h-44 btn-xs"
                Text="Checkout">
            </insite:Button>
        </div>
    </div>
</div>

<div runat="server" id="CounterPanel" class="row mt-2">
    <div class="col-lg-3">
    </div>
    <div class="col-lg-5">
        <div class="d-flex flex-column lh-1 ps-2">
            <span runat="server" ID="SelectedCount" class="fw-semibold small text-body">0 selected</span>
            <span runat="server" ID="RemainingCount" class="text-muted small">0 remaining</span>
        </div>
    </div>
</div>

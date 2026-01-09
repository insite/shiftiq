<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CardDetailConfirm.ascx.cs" Inherits="InSite.UI.Portal.Billing.Controls.CardDetailConfirm" %>

<div class="row">
    <div class="col-lg-4">

        <div class="form-group mb-3">
            <label class="form-label">
                <insite:Literal runat="server" Text="Cardholder Name" />
            </label>
            <div>
                <asp:Literal runat="server" ID="CardholderName" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                <insite:Literal runat="server" Text="Card Number" />
            </label>
            <div>
                <asp:Literal runat="server" ID="CardNumber" />
            </div>
        </div>

        <div class="row">
            <div class="col-md-8">
                <div class="form-group mb-3">
                    <label class="form-label">
                        <insite:Literal runat="server" Text="Expiry Date" />
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="Expiry" />
                    </div>
                </div>
            </div>

            <div class="col-md-4">
                <div class="form-group mb-3">
                    <label class="form-label">
                        <insite:Literal runat="server" Text="CVC" />
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="SecurityCode" />
                    </div>
                </div>
            </div>
        </div>

    </div>
    <div runat="server" id="AddressPanel" class="col-lg-4">

        <h4><insite:Literal runat="server" Text="Billing Address" /></h4>

        <div runat="server" id="ContactNameField" class="form-group mb-3">
            <label class="form-label">
                <insite:Literal runat="server" Text="Contact Name" />
            </label>
            <asp:Literal runat="server" ID="BillingName" />
        </div>

        <div runat="server" id="ContactEmailField" class="form-group mb-3">
            <label class="form-label">
                <insite:Literal runat="server" Text="Contact Email" />
            </label>
            <asp:Literal runat="server" ID="BillingEmail" />
        </div>

        <div runat="server" id="ContactPhoneField" class="form-group mb-3">
            <label class="form-label">
                <insite:Literal runat="server" Text="Contact Phone" />
            </label>
            <asp:Literal runat="server" ID="BillingPhone" />
        </div>

        <div runat="server" id="ContactAddressField" class="form-group mb-3">
            <label class="form-label">
                <insite:Literal runat="server" Text="Address" />
            </label>
            <asp:Literal runat="server" ID="BillingAddress" />
        </div>

        <div runat="server" id="ContactCityField" class="form-group mb-3">
            <label class="form-label">
                <insite:Literal runat="server" Text="City" />
            </label>
            <asp:Literal runat="server" ID="BillingCity" />
        </div>

        <div runat="server" id="ContactProvinceField" class="form-group mb-3">
            <label class="form-label">
                <insite:Literal runat="server" Text="Province/State" />
            </label>
            <asp:Literal runat="server" ID="BillingProvince" />
        </div>

        <div runat="server" id="ContactPostalCodeField" class="form-group mb-3">
            <label class="form-label">
                <insite:Literal runat="server" Text="Postal Code" />
            </label>
            <asp:Literal runat="server" ID="BillingPostalCode" />
        </div>

        <div runat="server" id="ContactCountryField" class="form-group mb-3">
            <label class="form-label">
                <insite:Literal runat="server" Text="Country" />
            </label>
            <asp:Literal runat="server" ID="BillingCountry" />
        </div>

    </div>
</div>

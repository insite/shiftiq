<%@ Control Language="C#" CodeBehind="CardDetail.ascx.cs" Inherits="InSite.UI.Portal.Billing.Controls.CardDetail" %>

<div class="row">
    <div class="col-lg-4">

        <h3><insite:Literal runat="server" Text="Credit Card" /></h3>

        <div class="form-group mb-3">
            <label class="form-label">
                <insite:Literal runat="server" Text="Cardholder Name" />
                <insite:RequiredValidator runat="server" ControlToValidate="CardholderName" Display="None" />
            </label>
            <insite:TextBox runat="server" ID="CardholderName" MaxLength="32" />
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                <insite:Literal runat="server" Text="Card Number" />
                <insite:RequiredValidator runat="server" ControlToValidate="CardNumber" Display="None" />
            </label>
            <insite:TextBox runat="server" ID="CardNumber" MaxLength="19" EmptyMessage="0000 0000 0000 0000"
                data-format="{&quot;creditCard&quot;:true}" />
        </div>

        <div class="row">

            <div class="col-md-7">
                <div class="form-group mb-3">
                    <label class="form-label">
                        <insite:Literal runat="server" Text="Expiry Date" />
                        <insite:RequiredValidator runat="server" ControlToValidate="Expiry" Display="None" />
                    </label>
                    <insite:TextBox runat="server" ID="Expiry" MaxLength="5" EmptyMessage="mm/yy"
                        data-format="{&quot;date&quot;:true,&quot;datePattern&quot;:[&quot;m&quot;,&quot;y&quot;]}" />
                </div>
            </div>

            <div class="col-md-5">
                <div class="form-group mb-3">
                    <label class="form-label">
                        <insite:Literal runat="server" Text="CVC" />
                        <insite:RequiredValidator runat="server" ControlToValidate="SecurityCode" Display="None" />
                    </label>
                    <insite:TextBox runat="server" ID="SecurityCode" MaxLength="4" EmptyMessage="000" />
                </div>
            </div>
        </div>

    </div>
    <div runat="server" id="AddressPanel" class="col-lg-4">

        <h4><insite:Literal runat="server" Text="Billing Address" /></h4>

        <div runat="server" id="ContactNameField" class="form-group mb-3">
            <label class="form-label">
                <insite:Literal runat="server" Text="Contact Name" />
                <insite:RequiredValidator runat="server" ID="ContactNameRequiredValidator" ControlToValidate="ContactName" />
            </label>
            <insite:TextBox runat="server" ID="ContactName" MaxLength="50" />
        </div>

        <div runat="server" id="ContactEmailField" class="form-group mb-3">
            <label class="form-label">
                <insite:Literal runat="server" Text="Contact Email" />
                <insite:RequiredValidator runat="server" ID="ContactEmailRequiredValidator" ControlToValidate="ContactEmail" />
            </label>
            <insite:TextBox runat="server" ID="ContactEmail" MaxLength="100" />
        </div>

        <div runat="server" id="ContactPhoneField" class="form-group mb-3">
            <label class="form-label">
                <insite:Literal runat="server" Text="Contact Phone" />
                <insite:RequiredValidator runat="server" ID="ContactPhoneRequiredValidator" ControlToValidate="ContactPhone" />
            </label>
            <insite:TextBox runat="server" ID="ContactPhone" MaxLength="50" />
        </div>

        <div runat="server" id="ContactAddressField" class="form-group mb-3">
            <label class="form-label">
                <insite:Literal runat="server" Text="Address" />
                <insite:RequiredValidator runat="server" ID="ContactAddressRequiredValidator" ControlToValidate="ContactAddress" />
            </label>
            <insite:TextBox runat="server" ID="ContactAddress" MaxLength="50" />
        </div>

        <div runat="server" id="ContactCityField" class="form-group mb-3">
            <label class="form-label">
                <insite:Literal runat="server" Text="City" />
                <insite:RequiredValidator runat="server" ID="ContactCityRequiredValidator" ControlToValidate="ContactCity" />
            </label>
            <insite:TextBox runat="server" ID="ContactCity" MaxLength="50" />
        </div>

        <div runat="server" id="ContactProvinceField" class="form-group mb-3">
            <label class="form-label">
                <insite:Literal runat="server" Text="Province" />
                <insite:RequiredValidator runat="server" ID="ContactProvinceRequiredValidator" ControlToValidate="ContactProvince" />
            </label>
            <insite:ProvinceComboBox ID="ContactProvince" runat="server" EnableSearch="true" UseCodeAsValue="true" OnlyCanadaAndUnitedStates="true"/>
        </div>

        <div runat="server" id="ContactPostalCodeField" class="form-group mb-3">
            <label class="form-label">
                <insite:Literal runat="server" Text="PostalCode" />
                <insite:RequiredValidator runat="server" ID="ContactPostalCodeRequiredValidator" ControlToValidate="ContactPostalCode" />
            </label>
            <insite:TextBox runat="server" ID="ContactPostalCode" MaxLength="10" />
        </div>

        <div runat="server" id="ContactCountryField" class="form-group mb-3">
            <label class="form-label">
                <insite:Literal runat="server" Text="Country" />
                <insite:RequiredValidator runat="server" ID="ContactCountryRequiredValidator" ControlToValidate="ContactCountry" />
            </label>
            <insite:CountryComboBox ID="ContactCountry" runat="server" ValueAsCode="true" DropDown-Size="15" />
        </div>

    </div>
</div>

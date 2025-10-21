<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailConfigurationLocationAddress.ascx.cs" Inherits="InSite.UI.Admin.Accounts.Organizations.Controls.DetailConfigurationLocationAddress" %>

<div class="form-group mb-3">
    <label class="form-label">
        Street
        <insite:RequiredValidator runat="server" ID="LocationStreetRequiredValidator" 
            ControlToValidate="LocationStreet" FieldName="Street" ValidationGroup="Organization"
            Visible="false" />
    </label>
    <insite:TextBox runat="server" ID="LocationStreet" MaxLength="32" />
    <div class="form-text">Street address.</div>
</div>
                            
<div class="form-group mb-3">
    <label class="form-label">
        City
        <insite:RequiredValidator runat="server" ID="LocationCityRequiredValidator"
            ControlToValidate="LocationCity" FieldName="City" ValidationGroup="Organization"
            Visible="false" />
    </label>
    <insite:TextBox runat="server" ID="LocationCity" MaxLength="32" />
    <div class="form-text">City.</div>
</div>
                            
<div class="form-group mb-3">
    <label class="form-label">
        State/Province
        <insite:RequiredValidator runat="server" ID="LocationProvinceRequiredValidator"
            ControlToValidate="LocationProvince" FieldName="State/Province" ValidationGroup="Organization"
            Visible="false" />
    </label>
    <insite:TextBox runat="server" ID="LocationProvince" MaxLength="32" />
    <div class="form-text">Province or state.</div>
</div>
                            
<div class="form-group mb-3">
    <label class="form-label">
        Postal Code
        <insite:RequiredValidator runat="server" ID="LocationPostalCodeRequiredValidator"
            ControlToValidate="LocationPostalCode" FieldName="Postal Code" ValidationGroup="Organization"
            Visible="false" />
    </label>
    <insite:TextBox runat="server" ID="LocationPostalCode" MaxLength="32" />
    <div class="form-text">Postal code or zip code.</div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Country
        <insite:RequiredValidator runat="server" ID="LocationCountryRequiredValidator"
            ControlToValidate="LocationCountry" FieldName="Country" ValidationGroup="Organization"
            Visible="false" />
    </label>
    <insite:TextBox runat="server" ID="LocationCountry" MaxLength="32" />
    <div class="form-text">Country.</div>
</div>

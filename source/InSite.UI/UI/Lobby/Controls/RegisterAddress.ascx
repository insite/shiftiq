<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RegisterAddress.ascx.cs" Inherits="InSite.UI.Lobby.Controls.RegisterAddress" %>

<div runat="server" id="DescriptionField">
    <label class="form-label">
        <insite:Literal runat="server" Text="Description" />
        <insite:RequiredValidator runat="server" Display="None" ControlToValidate="Description" ValidationGroup="Register" />
    </label>
    <insite:TextBox runat="server" ID="Description" MaxLength="128" />
</div>

<div runat="server" id="PhoneField">
    <label class="form-label">
        <insite:Literal runat="server" Text="Phone" />
        <insite:RequiredValidator runat="server" Display="None" ControlToValidate="Phone" ValidationGroup="Register" />
    </label>
    <insite:TextBox runat="server" ID="Phone" MaxLength="20" />
    <div style="line-height:20px; font-size:0.8em; color:#999;">
        <insite:Literal runat="server" Text="Phone Help" />
    </div>
</div>

<div runat="server" id="MobileField">
    <label class="form-label">
        <insite:Literal runat="server" Text="Mobile" />
        <insite:RequiredValidator runat="server" Display="None" ControlToValidate="Mobile" ValidationGroup="Register" />
    </label>
    <insite:TextBox runat="server" ID="Mobile" MaxLength="20" />
    <div style="line-height:20px; font-size:0.8em; color:#999;">
        <insite:Literal runat="server" Text="Phone Help" />
    </div>
</div>

<div runat="server" id="Address1Field">
    <label class="form-label">
        <insite:Literal runat="server" Text="Address 1" />
        <insite:RequiredValidator runat="server" Display="None" ControlToValidate="Street1" ValidationGroup="Register" />
    </label>
    <insite:TextBox runat="server" ID="Street1" MaxLength="128" />
</div>

<div runat="server" id="Address2Field">
    <label class="form-label">
        <insite:Literal runat="server" Text="Address 2" />
        <insite:RequiredValidator runat="server" Display="None" ControlToValidate="Street2" ValidationGroup="Register" />
    </label>
    <insite:TextBox runat="server" ID="Street2" MaxLength="128" />
</div>

<div runat="server" id="CityField">
    <label class="form-label">
        <insite:Literal runat="server" Text="City" />
        <insite:RequiredValidator runat="server" Display="None" ControlToValidate="City" ValidationGroup="Register" />
    </label>
    <insite:TextBox runat="server" ID="City" MaxLength="256" />
</div>

<div runat="server" id="CountryField">
    <label class="form-label">
        <insite:Literal runat="server" Text="Country" />
        <insite:RequiredValidator runat="server" Display="None" ControlToValidate="Country" ValidationGroup="Register" />
    </label>
    <insite:CountryComboBox runat="server" ID="Country" />
</div>

<div runat="server" id="ProvinceField">
    <label class="form-label">
        <insite:Literal runat="server" Text="Province" />
        <insite:RequiredValidator runat="server" Display="None" ControlToValidate="Country" ValidationGroup="Register" />
    </label>
    <insite:ProvinceComboBox runat="server" ID="Province" CountryControl="Country" />
</div>

<div runat="server" id="PostalCodeField">
    <label class="form-label">
        <insite:Literal runat="server" Text="Postal Code" />
        <insite:RequiredValidator runat="server" Display="None" ControlToValidate="PostalCode" ValidationGroup="Register" />
    </label>
    <insite:TextBox runat="server" ID="PostalCode" MaxLength="64" />
</div>

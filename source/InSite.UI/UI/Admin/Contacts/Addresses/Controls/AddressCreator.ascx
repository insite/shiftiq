<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddressCreator.ascx.cs" Inherits="InSite.Admin.Contacts.Addresses.Controls.AddressCreator" %>

<h6 class="mt-3">Home Address</h6>

<div class="form-group mb-3">
    <label class="form-label">
        Description
    </label>
    <insite:TextBox ID="Description" runat="server" MaxLength="128" />
    <div class="form-text">
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Address 1
    </label>
    <insite:TextBox ID="Street1" runat="server" MaxLength="128" />
    <div class="form-text">
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Address 2
    </label>
    <insite:TextBox ID="Street2" runat="server" MaxLength="128" />
    <div class="form-text">
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        City
    </label>
    <insite:TextBox ID="City" runat="server" MaxLength="128" />
    <div class="form-text">
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        State/Province
    </label>
    <insite:TextBox ID="Province" runat="server" MaxLength="128" />
    <div class="form-text">
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Postal Code
    </label>
    <insite:TextBox ID="PostalCode" runat="server" MaxLength="16" />
    <div class="form-text">
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Country
    </label>
    <insite:FindCountry ID="CountrySelector" runat="server" EmptyMessage="Country" />
    <div class="form-text">
    </div>
    
</div>

<div class="form-group mb-3">
    <insite:CheckBox runat="server" ID="UseAsShippingAddress" Text="User as Shipping Address" />
</div>

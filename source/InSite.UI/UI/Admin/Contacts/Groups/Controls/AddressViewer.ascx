<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddressViewer.ascx.cs" Inherits="InSite.UI.Admin.Contacts.Groups.Controls.AddressViewer" %>

<div class="float-end">
    <insite:IconLink Name="pencil" runat="server" ID="ChangeAddress" ToolTip="Change Address" NavigateUrl="#" />
</div>

<h3 runat="server" id="AddressHeading"></h3>

<div class="form-group mb-3" runat="server" id="DescriptionField">
    <label class="form-label">
        <insite:Literal runat="server" Text="Description" />
    </label>
    <div>
        <asp:Literal ID="Description" runat="server" />
    </div>
</div>

<div class="form-group mb-3" runat="server" id="Street1Field">
    <label class="form-label">
        <insite:Literal runat="server" Text="Address 1" />
    </label>
    <div>
        <asp:Literal ID="Street1" runat="server" />
    </div>
</div>

<div class="form-group mb-3" runat="server" id="Street2Field">
    <label class="form-label">
        <insite:Literal runat="server" Text="Address 2" />
    </label>
    <div>
        <asp:Literal ID="Street2" runat="server" />
    </div>

</div>

<div class="form-group mb-3" runat="server" id="CityField">
    <label class="form-label">
        <insite:Literal runat="server" Text="City" />
    </label>
    <div>
        <asp:Literal ID="City" runat="server" />
    </div>
</div>

<div class="form-group mb-3" runat="server" id="ProvinceField">
    <label runat="server" id="ProvinceFieldLabel">
        State/Province
    </label>
    <div>
        <asp:Literal ID="Province" runat="server" />
    </div>
</div>

<div class="form-group mb-3" runat="server" id="PostalCodeField">
    <label runat="server" id="PostalCodeFieldLabel">
        <insite:Literal runat="server" Text="Postal Code" />
    </label>
    <div>
        <asp:Literal ID="PostalCode" runat="server" />
    </div>
</div>

<div class="form-group mb-3" runat="server" id="CountryField">
    <label class="form-label">
        <insite:Literal runat="server" Text="Country" />
    </label>
    <div>
        <asp:Literal ID="Country" runat="server" />
    </div>
</div>

<div class="form-group mb-3" runat="server" id="MapField">
    <asp:HyperLink
        Visible="false"
        Width="50%"
        CssClass="btn btn-primary"
        runat="server"
        NavigateUrl="https://www.google.com/maps"
        ID="MapURL"
        Target="_new">
        <i class="fas fa-map"></i>&nbsp;&nbsp;View Map
    </asp:HyperLink>
</div>

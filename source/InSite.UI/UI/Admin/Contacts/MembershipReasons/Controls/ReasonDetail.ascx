<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReasonDetail.ascx.cs" Inherits="InSite.UI.Admin.Contacts.MembershipReasons.Controls.ReasonDetail" %>

<div class="form-group mb-3">
    <label class="form-label">
        Membership
        <insite:RequiredValidator runat="server" ControlToValidate="MembershipGroup" FieldName="Membership" ValidationGroup="Reason" />
    </label>
    <insite:FindGroup runat="server" ID="MembershipGroup" />
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Reason Subtype
    </label>
    <insite:ItemNameComboBox runat="server" ID="ReasonSubtype" Width="100%">
        <Settings UseCurrentOrganization="true" UseGlobalOrganizationIfEmpty="true" CollectionName="Contacts/Settings/Referrers/Name" />
    </insite:ItemNameComboBox>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Person Occupation
    </label>
    <insite:TextBox runat="server" ID="PersonOccupation" MaxLength="100" />
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Reason Effective Date
        <insite:RequiredValidator runat="server" ControlToValidate="ReasonEffectiveDate" FieldName="Reason Effective Date" ValidationGroup="Reason" />
    </label>
    <insite:DateTimeOffsetSelector runat="server" ID="ReasonEffectiveDate" />
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Reason Expiry Date
    </label>
    <insite:DateTimeOffsetSelector runat="server" ID="ReasonExpiryDate" />
</div>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="InSite.Admin.Accounts.Permissions.Controls.Detail" %>

<div class="form-group mb-3">
    <label class="form-label">
		Permission Action (Parent)
		<insite:RequiredValidator runat="server" ControlToValidate="PermissionIdentifier" FieldName="Permission Action" ValidationGroup="Permission" />
    </label>
    <insite:FindPermission runat="server" ID="PermissionIdentifier" Width="100%" />
</div>

<div class="form-group mb-3">
    <label class="form-label">
		Group Type
		<insite:RequiredValidator runat="server" ControlToValidate="GroupType" FieldName="Group Type" ValidationGroup="Permission" />
    </label>
    <insite:GroupTypeComboBox runat="server" ID="GroupType" InputCssClass="form-control" />
</div>

<div class="form-group mb-3">
    <label class="form-label">
		Group
		<insite:RequiredValidator runat="server" ControlToValidate="GroupIdentifier" FieldName="Group" ValidationGroup="Permission" />
    </label>
    <insite:FindGroup runat="server" ID="GroupIdentifier" Width="100%" />
</div>

<div class="form-group mb-3">

    <label class="form-label">
        Access
    </label>
    
    <div><asp:CheckBox runat="server" ID="AllowExecute" Text="Execute" /></div>

    <div><asp:CheckBox runat="server" ID="AllowRead" Text="Read" /></div>
	<div><asp:CheckBox runat="server" ID="AllowWrite" Text="Write" /></div>

	<div><asp:CheckBox runat="server" ID="AllowCreate" Text="Create" /></div>
	<div><asp:CheckBox runat="server" ID="AllowDelete" Text="Delete" /></div>

	<div><asp:CheckBox runat="server" ID="AllowAdministrate" Text="Administrate" /></div>
	<div><asp:CheckBox runat="server" ID="AllowConfigure" Text="Configure (Full Control)" /></div>

    <div><asp:CheckBox runat="server" ID="AllowTrialAccess" Text="Trial Access" /></div>

</div>

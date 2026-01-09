<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DepartmentDetails.ascx.cs" Inherits="InSite.Cmds.Controls.Contacts.Departments.DepartmentDetails" %>

<div class="form-group mb-3">
    <label class="form-label">
        Name
        <insite:RequiredValidator runat="server" ControlToValidate="DepartmentName" FieldName="Name" ValidationGroup="DepartmentInfo" />
    </label>
    <div>
        <insite:TextBox runat="server" ID="DepartmentName" MaxLength="90" />
    </div>
    <div class="form-text">The name of the department.</div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Description
    </label>
    <div>
        <insite:TextBox runat="server" ID="Description" />
    </div>
    <div class="form-text"></div>
</div>

<div runat="server" id="DivisionPanel" class="form-group mb-3">
    <label class="form-label">
        District
    </label>
    <div>
        <cmds:DivisionSelector ID="DivisionIdentifier" runat="server" Width="270" />
    </div>
    <div class="form-text"></div>
</div>

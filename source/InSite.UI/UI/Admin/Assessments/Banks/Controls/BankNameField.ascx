<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BankNameField.ascx.cs" Inherits="InSite.Admin.Assessments.Banks.Controls.BankNameField" %>

<div class="form-group mb-3">
    <label class="form-label">
        Internal Name
        <insite:RequiredValidator runat="server" ID="BankNameRequiredValidator" ControlToValidate="InternalName" />
    </label>
    <div>
        <insite:TextBox runat="server" ID="InternalName" />
    </div>
    <div class="form-text">
        The name that uniquely identifies this bank for internal filing purposes.
    </div>
</div>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FieldInputName.ascx.cs" Inherits="InSite.Admin.Messages.Messages.Controls.FieldInputName" %>

<div class="form-group mb-3">
    <label class="form-label">
        Internal Name
        <insite:RequiredValidator runat="server" ID="InputValueValidator" ControlToValidate="InputValue" FieldName="Internal Name" />
    </label>
    <insite:TextBox runat="server" ID="InputValue" MaxLength="256" />
    <div class="form-text">
        The internal name is used as an internal reference for filing the message. It is required field, and it is not visible to message recipients.
    </div>
</div>

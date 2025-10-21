<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FieldInputNotification.ascx.cs" Inherits="InSite.Admin.Messages.Messages.Controls.FieldInputNotification" %>

<div class="form-group mb-3">
    <label class="form-label">
        Alert Trigger
        <insite:RequiredValidator runat="server" ID="AlertValidator" ControlToValidate="AlertInput" FieldName="Alert Trigger" />
    </label>
    <div>
        <insite:AlertComboBox runat="server" ID="AlertInput" AllowBlank="true" Width="100%" />
        <div runat="server" ID="AlertDescription"></div>
    </div>
    <div class="form-text">
        The application change that triggers this alert message.
    </div>
</div>

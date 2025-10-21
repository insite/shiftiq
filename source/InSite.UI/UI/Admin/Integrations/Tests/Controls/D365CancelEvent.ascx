<%@ Control Language="C#" CodeBehind="D365CancelEvent.ascx.cs" Inherits="InSite.UI.Admin.Integrations.Tests.Controls.D365CancelEvent" %>

<div class="form-group mb-3">
    <label class="form-label">
        Event
        <insite:RequiredValidator runat="server" ControlToValidate="EventIdentifier" FieldName="Event" ValidationGroup="D365" />
    </label>
    <div>
        <insite:FindEvent runat="server" ID="EventIdentifier" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Reason
    </label>
    <div>
        <insite:TextBox runat="server" ID="Reason" />
    </div>
</div>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="D365CancelRegistration.ascx.cs" Inherits="InSite.UI.Admin.Integrations.Tests.Controls.D365CancelRegistration" %>

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
        Candidate
        <insite:RequiredValidator runat="server" ControlToValidate="CandidateIdentifier" FieldName="Person" ValidationGroup="D365" />
    </label>
    <div>
        <insite:FindPerson runat="server" ID="CandidateIdentifier" Enabled="false" />
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

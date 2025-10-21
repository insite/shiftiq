<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="D365AddEvent.ascx.cs" Inherits="InSite.UI.Admin.Integrations.Tests.Controls.D365AddEvent" %>

<div class="form-group mb-3">
    <label class="form-label">
        EventId
        <insite:RequiredValidator runat="server" ControlToValidate="EventId" FieldName="EventId" ValidationGroup="D365" />
        <insite:PatternValidator runat="server" ControlToValidate="EventId" FieldName="EventId" ValidationGroup="D365"
            ValidationExpression="^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$"
            ErrorMessage="EventId: invalid GUID value" />
    </label>
    <div>
        <insite:TextBox runat="server" ID="EventId" />
    </div>
    <div class="form-text">New event identifier</div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        EventVenue
    </label>
    <div>
        <insite:FindGroup runat="server" ID="EventVenue" />
    </div>
    <div class="form-text">Uniquely identifies the physical location for the venue hosting the event.</div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        EventStart
    </label>
    <div>
        <insite:DateTimeOffsetSelector runat="server" ID="EventStart" />
    </div>
    <div class="form-text">The date and time (including time zone) for the scheduled start of the event.</div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        EventExamType
    </label>
    <div>
        <insite:ExamTypeComboBox runat="server" ID="EventExamType" />
    </div>
    <div class="form-text">The exam type, used in case a new exam need to be created.</div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        EventExamFormat
    </label>
    <div>
        <insite:EventFormatComboBox runat="server" ID="EventExamFormat" />
    </div>
    <div class="form-text">The exam format, used in case a new exam need to be created.</div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        EventBillingCode
    </label>
    <div>
        <insite:ItemNameComboBox runat="server" ID="EventBillingCode" />
    </div>
    <div class="form-text">The exam billing code, used in case a new exam need to be created.</div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        RegistrationLimit
    </label>
    <div>
        <insite:NumericBox runat="server" ID="RegistrationLimit" NumericMode="Integer" MinValue="0" Width="135px" />        
    </div>
    <div class="form-text">The maximum number of candidate registrations permitted in the event.</div>
</div>

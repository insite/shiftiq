<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="D365AddRegistration.ascx.cs" Inherits="InSite.UI.Admin.Integrations.Tests.Controls.D365AddRegistration" %>

<%@ Register TagPrefix="uc" TagName="AccommodationManager" Src="~/UI/Admin/Integrations/Tests/Controls/AccommodationManager.ascx" %>

<div class="form-group mb-3">
    <label class="form-label">
        RegistrationId
        <insite:RequiredValidator runat="server" ControlToValidate="RegistrationId" FieldName="RegistrationId" ValidationGroup="D365" />
        <insite:PatternValidator runat="server" ControlToValidate="RegistrationId" FieldName="RegistrationId" ValidationGroup="D365"
            ValidationExpression="^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$"
            ErrorMessage="RegistrationId: invalid GUID value" />
    </label>
    <div>
        <insite:TextBox runat="server" ID="RegistrationId" />
    </div>
    <div class="form-text">New registration identifier</div>
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
        Learner
    </label>
    <div>
        <insite:FindPerson runat="server" ID="LearnerIdentifier" />
    </div>
    <div class="form-text">
        Uniquely identifies the learner (i.e., user/candidate) registering for the event.
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        LearnerRegistrantType
    </label>
    <insite:ItemNameComboBox runat="server" ID="LearnerRegistrantType" />
    <div class="form-text">
        At the time of registration, a learner may be an Apprentice or a Challenger.
        This is an optional property to indicate the registrant's type when registration is first requested.
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Assessment
    </label>
    <div>
        <insite:TextBox runat="server" ID="Assessment" />
    </div>
    <div class="form-text">An alphanumeric code that should (in theory) uniquely identify an assessment form.</div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Accommodations
    </label>
    <uc:AccommodationManager runat="server" ID="Accommodations" />
    <div class="form-text">An array of (optional) accommodations.</div>
</div>

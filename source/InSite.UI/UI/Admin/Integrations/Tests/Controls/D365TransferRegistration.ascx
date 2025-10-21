<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="D365TransferRegistration.ascx.cs" Inherits="InSite.UI.Admin.Integrations.Tests.Controls.D365TransferRegistration" %>

<%@ Register TagPrefix="uc" TagName="AccommodationManager" Src="~/UI/Admin/Integrations/Tests/Controls/AccommodationManager.ascx" %>

<div class="form-group mb-3">
    <label class="form-label">
        RegistrationEvent
    </label>
    <div>
        <insite:FindEvent runat="server" ID="EventIdentifier" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        RegistrationCandidate
        <insite:RequiredValidator runat="server" ControlToValidate="CandidateIdentifier" FieldName="RegistrationCandidate" ValidationGroup="D365" />
    </label>
    <div>
        <insite:FindPerson runat="server" ID="CandidateIdentifier" Enabled="false" />
    </div>
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
        Learner
    </label>
    <div>
        <insite:FindPerson runat="server" ID="LearnerIdentifier" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Reason
    </label>
    <div>
        <insite:TextBox runat="server" ID="Reason" />
    </div>
    <div class="form-text">An optional description of the reason for the transfer.</div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Accommodations
    </label>
    <uc:AccommodationManager runat="server" ID="Accommodations" />
    <div class="form-text">An array of (optional) accommodations.</div>
</div>

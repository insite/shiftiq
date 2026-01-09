<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FieldInputForm.ascx.cs" Inherits="InSite.Admin.Messages.Messages.Controls.FieldInputForm" %>

<div class="form-group mb-3">
    <label class="form-label">
        Form
        <insite:RequiredValidator runat="server" ID="SurveyValidator" ControlToValidate="SurveySelector" FieldName="Form" />
    </label>
    <insite:FindWorkflowForm runat="server" ID="SurveySelector" />
    <div class="form-text">
        Select the form that form invitation recipients are expected to submit.
        <asp:HyperLink runat="server" ID="SurveyEdit" Visible="false" Text="Edit the form" />.
    </div>
</div>

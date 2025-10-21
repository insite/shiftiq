<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FieldInputSurvey.ascx.cs" Inherits="InSite.Admin.Messages.Messages.Controls.FieldInputSurvey" %>

<div class="form-group mb-3">
    <label class="form-label">
        Survey Form
        <insite:RequiredValidator runat="server" ID="SurveyValidator" ControlToValidate="SurveySelector" FieldName="Survey Form" />
    </label>
    <insite:FindSurveyForm runat="server" ID="SurveySelector" />
    <div class="form-text">
        Select the form that survey invitation recipients are expected to submit.
        <asp:HyperLink runat="server" ID="SurveyEdit" Visible="false" Text="Edit the survey form" />.
    </div>
</div>

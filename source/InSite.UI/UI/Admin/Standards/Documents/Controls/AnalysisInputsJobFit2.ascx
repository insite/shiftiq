<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnalysisInputsJobFit2.ascx.cs" Inherits="InSite.Admin.Standards.Documents.Controls.AnalysisInputsJobFit2" %>

<div class="form-group mb-3">
    <label class="form-label">
        Competency Profile
        <insite:RequiredValidator runat="server" ControlToValidate="ProfileSelector" FieldName="Competency Profile" ValidationGroup="Analysis" />
    </label>
    <div>
        <insite:FindStandard runat="server" ID="ProfileSelector" />
    </div>
</div>

<div class="form-group mb-3">
    <div>
        <insite:Button runat="server" ID="AnalyzeButton" ButtonStyle="Success" CausesValidation="true" ValidationGroup="Analysis" Icon="far fa-chart-bar" Text="Compare" />
    </div>
</div>

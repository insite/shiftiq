<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnalysisInputsJobFit1.ascx.cs" Inherits="InSite.Admin.Standards.Documents.Controls.AnalysisInputsJobFit1" %>

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
    <label class="form-label">
        NOS
        <insite:RequiredValidator runat="server" ControlToValidate="NosSelector" FieldName="NOS" ValidationGroup="Analysis" />
    </label>
    <div>
        <insite:FindStandard runat="server" ID="NosSelector" />
    </div>
</div>

<div class="form-group mb-3">
    <div>
        <insite:Button runat="server" ID="AnalyzeButton" ButtonStyle="Success" CausesValidation="true" ValidationGroup="Analysis" Text="Compare" Icon="far fa-chart-bar" />
    </div>
</div>

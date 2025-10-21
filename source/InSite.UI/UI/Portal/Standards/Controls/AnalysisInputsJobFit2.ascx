<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnalysisInputsJobFit2.ascx.cs" Inherits="InSite.UI.Portal.Standards.Controls.AnalysisInputsJobFit2" %>

<div class="form-group mb-3">
    <label class="form-label" for="<%# ProfileSelector.ClientID %>">
        <insite:Literal runat="server" Text="Competency Profile" />
        <insite:RequiredValidator runat="server" ControlToValidate="ProfileSelector" FieldName="Competency Profile" ValidationGroup="Analysis" />
    </label>
    <div>
        <insite:FindStandard runat="server" ID="ProfileSelector" EnableTranslation="true" />
    </div>
</div>

<div class="mb-3">
    <insite:Button runat="server" ID="AnalyzeButton" ButtonStyle="Success" CausesValidation="true" ValidationGroup="Analysis" Text="Compare" Icon="far fa-chart-bar" />
</div>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnalysisInputsJobFit1.ascx.cs" Inherits="InSite.UI.Portal.Standards.Controls.AnalysisInputsJobFit1" %>

<div class="mb-1 mt-4" style="text-align:right;">
    <a href="/ui/portal/standards/documents/create?type=Customized%20Occupation%20Profile&action=Job%20Comparison%20Tool" title="Content Test">
        <insite:Literal runat="server" Text="Add new Competency Profile" />
        <i class="far fa-circle-plus"></i>
    </a>
</div>

<div class="form-group mb-3">
    <label class="form-label" for="<%# ProfileSelector.ClientID %>">
        <insite:Literal runat="server" Text="Competency Profile" />
        <insite:RequiredValidator runat="server" ControlToValidate="ProfileSelector" FieldName="Competency Profile" ValidationGroup="Analysis" />
    </label>
    <div>
        <insite:FindStandard runat="server" ID="ProfileSelector" EnableTranslation="true" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label" for="<%# NosSelector.ClientID %>">
        <insite:Literal runat="server" Text="NOS" />
        <insite:RequiredValidator runat="server" ControlToValidate="NosSelector" FieldName="NOS" ValidationGroup="Analysis" />
    </label>
    <div>
        <insite:FindStandard runat="server" ID="NosSelector" EnableTranslation="true" />
    </div>
</div>

<div class="mb-3">
    <insite:Button runat="server" ID="AnalyzeButton" ButtonStyle="Success" CausesValidation="true" ValidationGroup="Analysis" Text="Compare" Icon="far fa-chart-bar" />
</div>
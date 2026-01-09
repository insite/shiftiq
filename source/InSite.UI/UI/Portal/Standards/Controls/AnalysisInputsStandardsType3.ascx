<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnalysisInputsStandardsType3.ascx.cs" Inherits="InSite.UI.Portal.Standards.Controls.AnalysisInputsStandardsType3" %>

<div class="form-group mb-3">
    <label class="form-label" for="<%# CompetencySelector.ClientID %>">
        <insite:Literal runat="server" Text="Competency" />
        <insite:RequiredValidator runat="server" ControlToValidate="CompetencySelector" FieldName="Competency" ValidationGroup="Analysis" />
    </label>
    <div>
        <insite:FindStandard runat="server" ID="CompetencySelector" EnableTranslation="true" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label" for="<%# StandardTypeSelector.ClientID %>">
        <insite:Literal runat="server" Text="Standard Type" />
        <insite:RequiredValidator runat="server" ControlToValidate="StandardTypeSelector" FieldName="Standard Type" ValidationGroup="Analysis" />
    </label>
    <div>
        <insite:StandardTypeComboBox runat="server" ID="StandardTypeSelector" Width="150px" />
    </div>
</div>
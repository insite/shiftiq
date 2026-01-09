<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnalysisInputsStandardsType1.ascx.cs" Inherits="InSite.UI.Portal.Standards.Controls.AnalysisInputsStandardsType1" %>

<div class="form-group mb-3">
    <label class="form-label" for="<%# StandardSelector1.ClientID %>">
        <insite:Literal runat="server" Text="Standard 1" />
        <insite:RequiredValidator runat="server" ControlToValidate="StandardSelector1" FieldName="Standard 1" ValidationGroup="Analysis" />
    </label>
    <div>
        <insite:StandardTypeComboBox runat="server" ID="StandardTypeSelector1" EmptyMessage="Type" Width="150px" style="display:inline;" />
        <div style="width:calc(100% - 155px); display:inline-block;">
            <insite:FindStandard runat="server" ID="StandardSelector1" EmptyMessage="Standard" style="display:inline;" EnableTranslation="true" />
        </div>
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label" for="<%# StandardSelector2.ClientID %>">
        <insite:Literal runat="server" Text="Standard 2" />
        <insite:RequiredValidator runat="server" ControlToValidate="StandardSelector2" FieldName="Standard 2" ValidationGroup="Analysis" />
    </label>
    <div>
        <insite:StandardTypeComboBox runat="server" ID="StandardTypeSelector2" EmptyMessage="Type" Width="150px" style="display:inline;" />
        <div style="width:calc(100% - 155px); display:inline-block;">
            <insite:FindStandard runat="server" ID="StandardSelector2" EmptyMessage="Standard" style="display:inline;" EnableTranslation="true" />
        </div>
    </div>
</div>
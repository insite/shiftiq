<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnalysisInputsStandardsType3.ascx.cs" Inherits="InSite.Admin.Standards.Documents.Controls.AnalysisInputsStandardsType3" %>

<div class="form-group mb-3">
    <label class="form-label">
        Competency
        <insite:RequiredValidator runat="server" ControlToValidate="CompetencySelector" FieldName="Competency" ValidationGroup="Analysis" />
    </label>
    <div>
        <insite:FindStandard runat="server" ID="CompetencySelector" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Standard Type
        <insite:RequiredValidator runat="server" ControlToValidate="StandardTypeSelector" FieldName="Standard Type" ValidationGroup="Analysis" />
    </label>
    <div>

                        <insite:StandardTypeComboBox runat="server" ID="StandardTypeSelector"  />

    </div>
</div>

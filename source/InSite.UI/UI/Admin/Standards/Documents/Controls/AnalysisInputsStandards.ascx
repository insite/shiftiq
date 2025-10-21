<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnalysisInputsStandards.ascx.cs" Inherits="InSite.Admin.Standards.Documents.Controls.AnalysisInputsStandards" %>

<%@ Register TagPrefix="uc" TagName="AnalysisInputsStandardsType1" Src="~/UI/Admin/Standards/Documents/Controls/AnalysisInputsStandardsType1.ascx" %>
<%@ Register TagPrefix="uc" TagName="AnalysisInputsStandardsType2" Src="~/UI/Admin/Standards/Documents/Controls/AnalysisInputsStandardsType2.ascx" %>
<%@ Register TagPrefix="uc" TagName="AnalysisInputsStandardsType3" Src="~/UI/Admin/Standards/Documents/Controls/AnalysisInputsStandardsType3.ascx" %>

<div class="form-group mb-3">
    <label class="form-label">
        Comparison Type
    </label>
    <div>
        <insite:RadioButton runat="server" ID="ComparisonType1" GroupName="ComparisonType"
            Text="Compare Two" SubText="Select and compare two different standards" />
        <insite:RadioButton runat="server" ID="ComparisonType2" GroupName="ComparisonType" Disabled="true"
            Text="Compare One to All" SubText="Select to compare a specific standard type to an entire standard type" />
        <insite:RadioButton runat="server" ID="ComparisonType3" GroupName="ComparisonType"
            Text="Shared Competency" SubText="Select for all frameworks that contain a particular competency" />
    </div>
</div>

<uc:AnalysisInputsStandardsType1 runat="server" ID="InputsType1" />
<uc:AnalysisInputsStandardsType2 runat="server" ID="InputsType2" />
<uc:AnalysisInputsStandardsType3 runat="server" ID="InputsType3" />

<div runat="server" id="OutputSettings" class="form-group mb-3">
    <label class="form-label">Expected Output</label>
    <div>
        <insite:CheckBox runat="server" ID="OutputOverlap" Text="% Overlap" Checked="true" />
        <insite:CheckBox runat="server" ID="OutputShared" Text="Shared Competencies" Checked="true" />
        <insite:CheckBox runat="server" ID="OutputMissing" Text="Missing Competencies" Checked="true" />
    </div>
</div>

<div class="form-group mb-3">
    <div>
        <insite:Button runat="server" ID="AnalyzeButton" ButtonStyle="Success" CausesValidation="true" ValidationGroup="Analysis" Text="Compare" Icon="far fa-chart-bar" />
    </div>
</div>
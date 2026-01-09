<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnalysisInputsStandards.ascx.cs" Inherits="InSite.UI.Portal.Standards.Controls.AnalysisInputsStandards" %>

<%@ Register TagPrefix="uc" TagName="AnalysisInputsStandardsType1" Src="AnalysisInputsStandardsType1.ascx" %>
<%@ Register TagPrefix="uc" TagName="AnalysisInputsStandardsType2" Src="AnalysisInputsStandardsType2.ascx" %>
<%@ Register TagPrefix="uc" TagName="AnalysisInputsStandardsType3" Src="AnalysisInputsStandardsType3.ascx" %>

<div class="form-group mb-3">
    <label class="form-label">
        <insite:Literal runat="server" Text="Comparison Type" />
    </label>
    <div class="mb-2">
        <insite:RadioButton runat="server" ID="ComparisonType1" GroupName="ComparisonType" Text="Compare Two" />
        <small><insite:Literal runat="server" Text="Select and compare two different standards" /></small>
    </div>
    <div class="mb-2">
        <insite:RadioButton runat="server" ID="ComparisonType2" GroupName="ComparisonType" Text="Compare One to All" />
        <small><insite:Literal runat="server" Text="Select a Framework to Compare to all other Frameworks" /></small>
    </div>
    <div>
        <insite:RadioButton runat="server" ID="ComparisonType3" GroupName="ComparisonType" Text="Shared Competency" />
        <small><insite:Literal runat="server" Text="Select for all frameworks that contain a particular competency" /></small>
    </div>
</div>

<uc:AnalysisInputsStandardsType1 runat="server" ID="InputsType1" />
<uc:AnalysisInputsStandardsType2 runat="server" ID="InputsType2" />
<uc:AnalysisInputsStandardsType3 runat="server" ID="InputsType3" />

<div runat="server" id="OutputSettings" class="form-group mb-3">
    <label class="form-label">
        <insite:Literal runat="server" Text="Expected Output" />
    </label>
    <div>
        <insite:CheckBox runat="server" ID="OutputOverlap" Text="% Overlap" Checked="true" />
    </div>
    <div>
        <insite:CheckBox runat="server" ID="OutputShared" Text="Shared Competencies" Checked="true" />
    </div>
    <div>
        <insite:CheckBox runat="server" ID="OutputMissing" Text="Missing Competencies" Checked="true" />
    </div>
</div>

<div class="mb-3">
    <insite:Button runat="server" ID="AnalyzeButton" ButtonStyle="Success" CausesValidation="true" ValidationGroup="Analysis" Text="Compare" Icon="far fa-chart-bar" />
</div>
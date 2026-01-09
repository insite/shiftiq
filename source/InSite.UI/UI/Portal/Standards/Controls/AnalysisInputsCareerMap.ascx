<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnalysisInputsCareerMap.ascx.cs" Inherits="InSite.UI.Portal.Standards.Controls.AnalysisInputsCareerMap" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>
        <div class="form-group mb-3">
            <label class="form-label" for="<%# Benchmark.ClientID %>">
                <insite:Literal runat="server" Text="Benchmark" />
                <insite:RequiredValidator runat="server" ControlToValidate="Benchmark" FieldName="Benchmark" ValidationGroup="Analysis" />
            </label>
            <div>
                <insite:ComboBox runat="server" ID="BenchmarkType" EnableTranslation="true" Width="210" style="display:inline-block;">
                    <Items>
                        <insite:ComboBoxOption />
                        <insite:ComboBoxOption Value="National Occupation Standard" Text="NOS" />
                        <insite:ComboBoxOption Value="Occupation Profile" Text="Competency Profile" />
                    </Items>
                </insite:ComboBox>
                <insite:FindStandard runat="server" ID="Benchmark" Width="335" style="display:inline-block;" EnableTranslation="true" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label" for="<%# Comparison.ClientID %>">
                <insite:Literal runat="server" Text="Comparison" />
                <insite:RequiredValidator runat="server" ControlToValidate="Comparison" FieldName="Comparison" ValidationGroup="Analysis" />
            </label>
            <div>
                <insite:ComboBox runat="server" ID="ComparisonType" EnableTranslation="true" Width="210" style="display:inline-block;">
                    <Items>
                        <insite:ComboBoxOption />
                        <insite:ComboBoxOption Value="National Occupation Standard" Text="NOS" />
                        <insite:ComboBoxOption Value="Occupation Profile" Text="Competency Profile" />
                    </Items>
                </insite:ComboBox>
                <insite:FindStandard runat="server" ID="Comparison" Width="335" style="display:inline-block;" EnableTranslation="true" />
            </div>
        </div>
    </ContentTemplate>
</insite:UpdatePanel>

<div class="form-group mb-3">
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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnalysisInputsCareerMap.ascx.cs" Inherits="InSite.Admin.Standards.Documents.Controls.AnalysisInputsCareerMap" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />
<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>
        <div class="form-group mb-3">
            <label class="form-label">
                Benchmark
                <insite:RequiredValidator runat="server" ControlToValidate="Benchmark" FieldName="Benchmark" ValidationGroup="Analysis" />
            </label>
            <div>
                <div class="row">
                    <div class="col-md-4">
                        <insite:ComboBox runat="server" ID="BenchmarkType">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="National Occupation Standard" Text="NOS" />
                                <insite:ComboBoxOption Value="Customized Occupation Profile" Text="Occupational Profile" />
                            </Items>
                        </insite:ComboBox>
                    </div>
                    <div class="col-md-8">
                        <insite:FindStandard runat="server" ID="Benchmark" EnableTranslation="true" />
                    </div>
                </div>
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Comparison
                <insite:RequiredValidator runat="server" ControlToValidate="Comparison" FieldName="Comparison" ValidationGroup="Analysis" />
            </label>
            <div>
                <div class="row">
                    <div class="col-md-4">
                        <insite:ComboBox runat="server" ID="ComparisonType">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="National Occupation Standard" Text="NOS" />
                                <insite:ComboBoxOption Value="Customized Occupation Profile" Text="Occupational Profile" />
                            </Items>
                        </insite:ComboBox>
                    </div>
                    <div class="col-md-8">
                        <insite:FindStandard runat="server" ID="Comparison" EnableTranslation="true" />
                    </div>
                </div>
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

<div class="form-group mb-3">
    <div>
        <insite:Button runat="server" ID="AnalyzeButton" ButtonStyle="Success" CausesValidation="true" ValidationGroup="Analysis" Icon="far fa-chart-bar" Text="Compare" />
    </div>
</div>

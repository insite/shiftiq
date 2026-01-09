<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnalysisInputsStandardsType1.ascx.cs" Inherits="InSite.Admin.Standards.Documents.Controls.AnalysisInputsStandardsType1" %>

<div class="form-group mb-3">
    <label class="form-label">
        Standard 1
        <insite:RequiredValidator runat="server" ControlToValidate="StandardSelector1" FieldName="Standard 1" ValidationGroup="Analysis" />
    </label>
    <div>
        <div class="row">
            <div class="col-md-4">
                <insite:StandardTypeComboBox runat="server" ID="StandardTypeSelector1" EmptyMessage="Type" />
            </div>
            <div class="col-md-8">
                <insite:FindStandard runat="server" ID="StandardSelector1" EmptyMessage="Standard" />
            </div>
        </div>
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Standard 2
        <insite:RequiredValidator runat="server" ControlToValidate="StandardSelector2" FieldName="Standard 2" ValidationGroup="Analysis" />
    </label>
    <div>
        <div class="row">
            <div class="col-md-4">
                <insite:StandardTypeComboBox runat="server" ID="StandardTypeSelector2" EmptyMessage="Type" />
            </div>
            <div class="col-md-8">
                <insite:FindStandard runat="server" ID="StandardSelector2" EmptyMessage="Standard" />
            </div>
        </div>
    </div>
</div>

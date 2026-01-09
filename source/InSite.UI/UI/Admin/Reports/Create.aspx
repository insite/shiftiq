<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.UI.Admin.Reports.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master"%>

<%@ Register TagPrefix="uc" TagName="PrivacyGroupManager" Src="~/UI/Admin/Reports/Controls/PrivacyGroupManager.ascx" %>
<%@ Register TagPrefix="uc" TagName="JsonEditor" Src="~/UI/Admin/Reports/Controls/JsonEditor.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="StatusAlert" />
    <insite:ValidationSummary runat="server" ValidationGroup="Report" />
    <insite:ValidationSummary runat="server" ValidationGroup="Upload" />

    <section class="mb-4">
		<div class="row">
			<div class="col-xl-6">

                <div class="card border-0 shadow-lg">
			        <div class="card-body">
                        <h3>
                            <insite:Literal runat="server" Text="Report" />
                        </h3>

                        <div class="row">
                            <div class="col-md-12">
                                <div class="settings">
                                    <div class="form-group mb-3">
                                        <div>
                                            <insite:CreationTypeComboBox runat="server" ID="CreationType" TypeName="Report" AllowBlank="false" />
                                        </div>
                                    </div>

                                    <div runat="server" id="ReportSelectorField" class="form-group mb-3">
                                        <asp:Label runat="server" CssClass="form-label" AssociatedControlID="ReportSelector">Report</asp:Label>
                                        <insite:RequiredValidator runat="server" FieldName="Report" ControlToValidate="ReportSelector" Display="None" ValidationGroup="Report" />
                                        <div>
                                            <insite:ReportComboBox runat="server" ID="ReportSelector" />
                                        </div>
                                    </div>

                                    <insite:Container runat="server" ID="ReportInputsContainer">
                                        <div class="form-group mb-3">
                                            <asp:Label runat="server" CssClass="form-label" AssociatedControlID="ReportTitle">Report Title</asp:Label>
                                            <insite:RequiredValidator runat="server" FieldName="Report Title" ControlToValidate="ReportTitle" Display="None" ValidationGroup="Report" />
                                            <div>
                                                <insite:TextBox runat="server" ID="ReportTitle" MaxLength="100" />
                                            </div>
                                        </div>

                                        <div runat="server" id="ReportTypeField" class="form-group mb-3">
                                            <asp:Label runat="server" CssClass="form-label" AssociatedControlID="ReportType">Report Type</asp:Label>
                                            <insite:RequiredValidator runat="server" FieldName="Report Type" ControlToValidate="ReportType" Display="None" ValidationGroup="Report" />
                                            <div>
                                                <insite:ComboBox runat="server" ID="ReportType">
                                                    <Items>
                                                        <insite:ComboBoxOption />
                                                        <insite:ComboBoxOption Value="Custom" Text="Custom Report" />
                                                        <insite:ComboBoxOption Value="Shared" Text="Shared Report" />
                                                    </Items>
                                                </insite:ComboBox>
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <asp:Label runat="server" CssClass="form-label" AssociatedControlID="ReportDescription">Report Description</asp:Label>
                                            <div>
                                                <insite:TextBox runat="server" ID="ReportDescription" TextMode="MultiLine" Rows="6" MaxLength="300" />
                                            </div>
                                        </div>
                                    </insite:Container>

                                    <insite:Container runat="server" ID="JsonFileUploadContainer">
                                        <div class="row">
                                            <div class="form-group mb-3">
                                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="JsonFileUpload">Select and Upload JSON File</asp:Label>
                                                <insite:RequiredValidator runat="server" FieldName="JSON File" ControlToValidate="JsonFileUpload" Display="None" ValidationGroup="Upload" />
                                                <insite:CustomValidator runat="server" ID="JsonFileUploadExtensionValidator"
                                                    ControlToValidate="JsonFileUpload" Display="Dynamic" ValidateEmptyText="false" ValidationGroup="Upload"
                                                    ErrorMessage="Invalid file type. File types supported: .json"
                                                    ClientValidationFunction="jsonCreator.ValidateJsonFileUpload" />
                                                <div>
                                                    <div class="input-group">
                                                        <insite:TextBox runat="server" ReadOnly="true" style="background-color:#fff;" />
                                                        <button class="btn btn-icon btn-outline-secondary" data-upload="#<%= JsonFileUpload.ClientID %>" title="Browse" type="button"><i class="far fa-folder-open"></i></button>
                                                        <insite:Button runat="server" ID="JsonFileUploadButton"
                                                            ToolTip="Upload" ButtonStyle="OutlineSecondary" Icon="far fa-upload"
                                                            ValidationGroup="Upload" />
                                                    </div>
                                                    <div class="d-none">
                                                        <asp:FileUpload runat="server" ID="JsonFileUpload" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </insite:Container>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>

            </div>

			<div runat="server" id="PrivacyGroupsField" class="col-xl-6 mt-3 mt-xl-0">
                <div class="card border-0 shadow-lg">
			        <div class="card-body">
                        <h3>
                            <insite:Literal runat="server" Text="Groups" />
                        </h3>
                        <uc:PrivacyGroupManager runat="server" ID="PrivacyGroups" />
                    </div>
                </div>
            </div>

			<div runat="server" id="JsonInputField" class="col-xl-6 mt-3 mt-xl-0">
                <div class="card border-0 shadow-lg">
			        <div class="card-body">
                        <h3>
                            <insite:Literal runat="server" Text="Uploaded JSON" />
                            <insite:RequiredValidator runat="server" FieldName="Uploaded JSON" ControlToValidate="JsonInput" Display="None" ValidationGroup="Report" />
                            <asp:CustomValidator runat="server" ID="JsonSchemaValidator" ValidateEmptyText="false" ValidationGroup="Report" Display="None"  />
                        </h3>
                        <insite:TextBox runat="server" ID="JsonInput" TextMode="MultiLine" Rows="15" />
                    </div>
                </div>
            </div>
        </div>
    </section>

    <section runat="server" id="JsonSection" class="mb-3">
        <div class="row">
			<div class="col-xl-12">

                <div class="card border-0 shadow-lg">
			        <div class="card-body">
                        <h3>
                            <insite:Literal runat="server" Text="JSON" />
                        </h3>

                        <div class="row">
                            <div class="col-md-12">
                                <uc:JsonEditor runat="server" ID="ReportData" />
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </section>

    <section class="mb-3">
        <insite:SaveButton runat="server" ID="SaveButton" CausesValidation="true" ValidationGroup="Report" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </section>

    <insite:PageFooterContent runat="server"> 
        <script type="text/javascript">

            (function () {
                $('[data-upload]').each(function () {
                    var $btn = $(this);
                    var uploadSelector = $btn.data('upload');
                    $(uploadSelector).on('change', function () {
                        var fileName = '';

                        if (this.files) {
                            if (this.files.length > 0) {
                                fileName = this.files[0].name;
                            }
                        } else if (this.value) {
                            fileName = this.value.split(/(\\|\/)/g).pop();
                        }

                        $btn.closest('.input-group').find('input[type="text"]').val(fileName);
                    });
                }).on('click', function () {
                    var uploadSelector = $(this).data('upload');
                    $(uploadSelector).click();
                });
            })();

            (function () {
                var jsonCreator = window.jsonCreator = window.jsonCreator || {};

                jsonCreator.ValidateJsonFileUpload = function (s, e) {
                    if (!e.Value)
                        return;

                    var ext = '';
                    var index = e.Value.lastIndexOf('.');
                    if (index > 0)
                        ext = e.Value.substring(index).toLowerCase();

                    e.IsValid = ext === '.json';
                };
            })();

        </script>
    </insite:PageFooterContent>

</asp:Content>

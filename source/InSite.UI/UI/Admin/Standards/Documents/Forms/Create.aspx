<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Admin.Standards.Documents.Forms.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="CreatorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Documents" />
    <insite:ValidationSummary runat="server" ValidationGroup="Copy" />
    <insite:ValidationSummary runat="server" ValidationGroup="Upload1ValidationGroup" />
    <insite:ValidationSummary runat="server" ValidationGroup="Upload2ValidationGroup" />

    <section runat="server" ID="GeneralSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-file-alt me-1"></i>
            Document
        </h2>

        <div class="row mb-3">
            <div class="col-md-6">
                <insite:CreationTypeComboBox runat="server" ID="CreationType" TypeName="Document" />
            </div>
        </div>

        <asp:Panel runat="server" ID="NewSection">
            <div class="card border-0 shadow-lg mb-3">

                <div class="card-body">

                    <div class="row mb-3">
                        <div class="col-md-6">

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Document Type
                                    <insite:RequiredValidator runat="server" ControlToValidate="DocumentType" FieldName="Document Type" ValidationGroup="Documents" />
                                </label>
                                <div>
                                    <insite:ComboBox runat="server" ID="DocumentType" Width="100%" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Title
                                    <insite:RequiredValidator runat="server" ControlToValidate="TitleInput" FieldName="Title" ValidationGroup="Documents" />
                                </label>
                                <div>
                                    <insite:TextBox runat="server" ID="TitleInput" />
                                </div>
                            </div>

                            <div runat="server" id="BaseStandardField" class="form-group mb-3" visible="false">
                                <label class="form-label">
                                    <asp:Literal runat="server" ID="BaseStandardLabel" />
                                </label>
                                <div>
                                    <div style="margin-bottom:8px;">
                                        <insite:ComboBox runat="server" ID="BaseStandardTypeSelector" Visible="false" EmptyMessage="Document Type" Width="100%" />
                                    </div>
                                    <insite:FindStandard runat="server" ID="BaseStandardSelector" Visible="false" />
                                </div>
                            </div>

                        </div>
                    </div>
                </div>

            </div>

            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Documents" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </asp:Panel>

        <asp:Panel runat="server" ID="DuplicateSection">
            <div class="card border-0 shadow-lg mb-3">

                <div class="card-body">

                    <div class="row mb-3">
                        <div class="col-md-6">

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Document
                                </label>
                                <div>
                                    <insite:FindStandard runat="server" ID="DocumentStandardSelector"/>
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Document Type
                                    <insite:RequiredValidator runat="server" ControlToValidate="DocumentTypeDuplicate" FieldName="Document Type" ValidationGroup="Documents" />
                                </label>
                                <div>
                                    <insite:ComboBox runat="server" ID="DocumentTypeDuplicate" Width="100%" Enabled="true" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Title
                                    <insite:RequiredValidator runat="server" ControlToValidate="TitleDuplicate" FieldName="Title" ValidationGroup="Documents" />
                                </label>
                                <div>
                                    <insite:TextBox runat="server" ID="TitleDuplicate" />
                                </div>
                            </div>

                        </div>
                    </div>

                </div>
            </div>

            <insite:SaveButton runat="server" ID="CopyButton" ValidationGroup="Copy" />
            <insite:CancelButton runat="server" ID="CopyCancelButton" />

        </asp:Panel>

        <asp:Panel runat="server" ID="UploadSection">

            <div class="row mb-3">
                <div class="col-md-6">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Select and Upload Document JSON File
                                    <insite:RequiredValidator runat="server" ControlToValidate="JsonFileUpload" FieldName="JSON File" Display="Dynamic" ValidationGroup="Upload1ValidationGroup" />
                                    <insite:CustomValidator runat="server" ID="JsonFileUploadExtensionValidator"
                                        ControlToValidate="JsonFileUpload" Display="Dynamic" ValidateEmptyText="false" ValidationGroup="Upload1ValidationGroup"
                                        ErrorMessage="Invalid file type. File types supported: .json"
                                        ClientValidationFunction="gradebookCreator.ValidateJsonFileUpload" />
                                </label>
                                <div>
                                    <insite:FileUploadV1 runat="server" ID="JsonFileUpload" LabelText="" FileUploadType="Unlimited" AllowedExtensions=".json" />
                                </div>
                                <div class="form-text"></div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Uploaded JSON
                                    <insite:RequiredValidator runat="server" ControlToValidate="JsonInput" FieldName="Uploaded JSON" Display="Dynamic" ValidationGroup="Upload2ValidationGroup" />
                                </label>
                                <div>
                                    <insite:TextBox runat="server" ID="JsonInput" TextMode="MultiLine" Rows="15" AllowHtml="true" />
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>

            <insite:SaveButton runat="server" ID="UploadSaveButton" ValidationGroup="Upload2ValidationGroup" />
            <insite:CancelButton runat="server" ID="UploadCancelButton" />
        </asp:Panel>
    </section>

</asp:Content>

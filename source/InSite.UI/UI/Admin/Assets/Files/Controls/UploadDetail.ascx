<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UploadDetail.ascx.cs" Inherits="InSite.UI.Admin.Assets.Files.Controls.UploadDetail" %>

<%@ Register TagPrefix="uc" TagName="FilePermissionList" Src="FilePermissionList.ascx" %>

<div class="row">
    <div class="col-lg-8">

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <h3>Upload Details</h3>

                <div class="row">
                    <div class="col-lg-6">

                        <div class="card h-100">
                            <div class="card-body">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Document Status
                                        <insite:RequiredValidator runat="server" ControlToValidate="FileStatus" FieldName="Document Status" ValidationGroup="File" />
                                    </label>
                                    <div>
                                        <insite:ItemNameComboBox runat="server" ID="FileStatus">
                                            <Settings
                                                CollectionName="Assets/Files/Document/Status"
                                                UseCurrentOrganization="true"
                                                UseGlobalOrganizationIfEmpty="true"
                                            />
                                        </insite:ItemNameComboBox>
                                    </div>
                                    <div class="form-text">
                                    </div>
                                </div>

                                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="DocumentTypePanel" />

                                <insite:UpdatePanel runat="server" ID="DocumentTypePanel">
                                    <ContentTemplate>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Document Type
                                            </label>
                                            <div>
                                                <insite:ItemNameComboBox runat="server" ID="FileCategory">
                                                    <Settings
                                                        CollectionName="Assets/Files/Document/Type"
                                                        UseCurrentOrganization="true"
                                                        UseGlobalOrganizationIfEmpty="true"
                                                    />
                                                </insite:ItemNameComboBox>
                                            </div>
                                            <div class="form-text">
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Document Subtype
                                            </label>
                                            <div>
                                                <insite:DocumentSubTypeComboBox runat="server" ID="FileSubcategory" />
                                            </div>
                                            <div class="form-text">
                                            </div>
                                        </div>

                                    </ContentTemplate>
                                </insite:UpdatePanel>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Description
                                    </label>
                                    <div>
                                        <insite:TextBox runat="server" ID="FileDescription" TextMode="MultiLine" Rows="4" />
                                    </div>
                                    <div class="form-text">
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                    <div class="col-lg-6">

                        <div class="card h-100">
                            <div class="card-body">

                                <div runat="server" id="UploadField" class="form-group mb-3">
                                    <label class="form-label">
                                        Upload Document
                                        <insite:RequiredValidator runat="server" ControlToValidate="File" FieldName="Document" ValidationGroup="File" />
                                    </label>
                                    <div>
                                        <insite:FileUploadV2 runat="server" ID="File" LabelText="" OnClientFileUploaded="File_OnFileUploaded" />
                                    </div>
                                    <div class="form-text">
                                    </div>
                                </div>

                                <div runat="server" id="LinkField" class="form-group mb-3" visible="false">
                                    <label class="form-label">
                                        Uploaded Document
                                    </label>
                                    <div>
                                        <a runat="server" id="FileLink" href="#" target="_blank">
                                            <i class="far fa-external-link me-2"></i>
                                            <asp:Literal runat="server" ID="FileName" />
                                        </a>
                                        (<asp:Literal runat="server" ID="FileSize" />)
                                    </div>
                                    <div class="form-text">
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Document Name
                                        <insite:RequiredValidator runat="server" ControlToValidate="DocumentName" FieldName="Document Name" ValidationGroup="File" />
                                        <asp:CustomValidator runat="server" ID="IssueDocumentNameValidator"
                                            ControlToValidate="DocumentName"
                                            ValidationGroup="File"
                                            ErrorMessage="This document name is already used by another attachment in the issue"
                                            Display="None"
                                        />
                                    </label>
                                    <div>
                                        <insite:TextBox runat="server" ID="DocumentName" MaxLength="200" />
                                    </div>
                                    <div class="form-text">
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Allow User to Open File
                                    </label>
                                    <div class="d-flex gap-3">
                                        <insite:RadioButton runat="server" ID="AllowLearnerToViewYes" GroupName="AllowLearnerToView" Text="Yes" />
                                        <insite:RadioButton runat="server" ID="AllowLearnerToViewNo" GroupName="AllowLearnerToView" Text="No" />                                        
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Expiry Date
                                    </label>
                                    <div>
                                        <insite:DateSelector runat="server" ID="FileExpiry" />
                                    </div>
                                    <div class="form-text">
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Date Received
                                    </label>
                                    <div>
                                        <insite:DateSelector runat="server" ID="FileReceived" />
                                    </div>
                                    <div class="form-text">
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Alternate Date
                                    </label>
                                    <div>
                                        <insite:DateSelector runat="server" ID="FileAlternated" />
                                    </div>
                                    <div class="form-text">
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>

    </div>
    <div class="col-lg-4">

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <h3>Approval</h3>

                <div class="mb-3">
                    <insite:CheckBox runat="server" ID="IsReviewed" Text="Reviewed" />
                    <insite:CheckBox runat="server" ID="IsApproved" Text="Approved" />
                </div>

                <h3>Permissions</h3>

                <uc:FilePermissionList runat="server" ID="PermissionList" />

            </div>
        </div>

    </div>
</div>

<insite:PageFooterContent runat="server">
    <script>
        function File_OnFileUploaded() {
            const input = document.getElementById("<%= DocumentName.ClientID %>");
            input.value = inSite.common.fileUploadV2.getFileName("<%= File.ClientID %>");
        }
    </script>
</insite:PageFooterContent>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompanyDetail.ascx.cs" Inherits="InSite.Cmds.Controls.Contacts.Companies.CompanyDetails" %>

<asp:CustomValidator runat="server" ID="CompanyDomainValidator" ErrorMessage="Web Site is invalid" ValidationGroup="CompanyInfo" />

<div class="row">
    <div class="col-lg-6">

        <div class="form-group mb-3">
            <label class="form-label">
                Full Name
                <insite:RequiredValidator runat="server" ControlToValidate="CompanyName" FieldName="Name" ValidationGroup="CompanyInfo" />
            </label>
            <insite:TextBox runat="server" ID="CompanyName" MaxLength="100" />
            <div class="form-text">
                The name of the organization.
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Short Name
                <insite:RequiredValidator runat="server" ControlToValidate="Acronym" FieldName="Short Name" ValidationGroup="CompanyInfo" />
            </label>
            <insite:TextBox runat="server" ID="Acronym" MaxLength="50" />
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Description
            </label>
            <insite:TextBox runat="server" ID="Description" TextMode="MultiLine" />
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Web Site
                <insite:RequiredValidator runat="server" ControlToValidate="WebSiteUrl" FieldName="WebSiteUrl" ValidationGroup="CompanyInfo" />
            </label>
            <insite:TextBox ID="WebSiteUrl" runat="server" MaxLength="128" />
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Organization Code
                <insite:RequiredValidator runat="server" ControlToValidate="OrganizationCode" FieldName="Organization Code" ValidationGroup="CompanyInfo" />
            </label>
            <insite:TextBox ID="OrganizationCode" runat="server" MaxLength="20" />
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Settings
            </label>
            <div>
                <asp:CheckBox ID="EnableDivisions" runat="server" Text="Enable Divisions" />
            </div>
        </div>

    </div>
    <div class="col-lg-6">

        <div runat="server" id="RowAttachments" visible="false" class="form-group mb-3">
            <label class="form-label">
                Downloads
            </label>
            <div>
                <asp:HyperLink runat="server" ID="CompanyAttachmentEditorLink" Text="Edit downloads for this organization" />
            </div>
        </div>

        <div runat="server" id="LogoField" visible="false" class="form-group mb-3">
            <label class="form-label">
                Logo
            </label>
            <div>
                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="LogoUpdatePanel" />

                <insite:UpdatePanel runat="server" ID="LogoUpdatePanel">
                    <ContentTemplate>
                        <div class="mb-3">
                            <insite:Button runat="server" ID="UploadLogoButton" Icon="fas fa-fw fa-upload" ButtonStyle="OutlinePrimary" ToolTip="Upload Photo" 
                                OnClientClick="companyEditor.showLogoUploader(); return false;" />
                            <insite:Button runat="server" ID="ReplaceLogoButton" Icon="fas fa-fw fa-upload" ButtonStyle="OutlinePrimary" ToolTip="Replace Photo" 
                                OnClientClick="companyEditor.showLogoUploader(); return false;" />
                            <insite:Button runat="server" ID="RemoveLogoButton" Icon="fas fa-fw fa-trash-alt" ButtonStyle="OutlinePrimary" ToolTip="Delete Photo" 
                                OnClientClick="companyEditor.removeLogo(); return false;" />
                        </div>

                        <div class="card">
                            <div class="card-body">
                                <asp:Image ID="LogoImage" runat="server" Visible="false" />
                            </div>
                        </div>

                        <div class="d-none">
                            <insite:FileUploadV1 runat="server" ID="LogoUpload" 
                                AllowedExtensions=".gif,.jpg,.jpeg,.png" 
                                OnClientFileUploaded="companyEditor.onLogoUploaded"
                            />
                        </div>
                    </ContentTemplate>
                </insite:UpdatePanel>
            </div>
            <div class="form-text">
                Upload a logo by clicking the <strong>Upload</strong> button.<br/>
                Supported image file formats: *.gif, *.jpg, *.png
            </div>
        </div>

    </div>
</div>

<insite:PageFooterContent runat="server">
<script type="text/javascript">

    (function () {
        var companyEditor = window.companyEditor = window.companyEditor || {};

        companyEditor.removeLogo = function () {
            if (confirm('Are you sure you want to remove this logo?')) {
                __doPostBack("<%= RemoveLogoButton.UniqueID %>", "");
            }
        };

        companyEditor.showLogoUploader = function () {
            inSite.common.fileUploadV1.trigger('<%= LogoUpload.ClientID %>');
        };

        companyEditor.onLogoUploaded = function () {
            document.getElementById('<%= LogoUpdatePanel.ClientID %>').ajaxRequest('upload');
        };
    })();

</script>
</insite:PageFooterContent>
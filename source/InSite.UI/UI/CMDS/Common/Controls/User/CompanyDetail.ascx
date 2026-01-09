<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompanyDetail.ascx.cs" Inherits="InSite.Cmds.Controls.Contacts.Companies.CompanyDetails" %>

<asp:CustomValidator runat="server" ID="CompanyDomainValidator" ErrorMessage="Web Site is invalid" ValidationGroup="CompanyInfo" />

<div class="row">
    <div class="col-lg-6">

        <div class="form-group mb-3">
            <label class="form-label">
                Organization name
        <insite:RequiredValidator runat="server" ControlToValidate="CompanyName" FieldName="Name" ValidationGroup="CompanyInfo" />
            </label>
            <div>
                <insite:TextBox runat="server" ID="CompanyName" MaxLength="100" EmptyMessage="Amazing Exploration, Inc." />
            </div>
            <div class="form-text">Full legal company name</div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Organization abbreviation
        <insite:RequiredValidator runat="server" ControlToValidate="Acronym" FieldName="Short Name" ValidationGroup="CompanyInfo" />
            </label>
            <div>
                <insite:TextBox runat="server" ID="Acronym" MaxLength="50" EmptyMessage="Amazing Exp" />
            </div>
            <div class="form-text">Short-form abbreviated company name</div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Organization code
        <insite:RequiredValidator runat="server" ControlToValidate="OrganizationCode" FieldName="Organization Code" ValidationGroup="CompanyInfo" />
            </label>
            <div>
                <insite:TextBox ID="OrganizationCode" runat="server" MaxLength="20" EmptyMessage="amazing" />
            </div>
            <div class="form-text">Web-address subdomain name</div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Website URL
        <insite:RequiredValidator runat="server" ControlToValidate="WebSiteUrl" FieldName="WebSiteUrl" ValidationGroup="CompanyInfo" />
            </label>
            <div>
                <insite:TextBox ID="WebSiteUrl" runat="server" MaxLength="128" EmptyMessage="https://www.amazingexploration.com" />
            </div>
            <div class="form-text">Fully qualified URL to company website</div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Description</label>
            <div>
                <insite:TextBox runat="server" ID="Description" TextMode="MultiLine" />
            </div>
            <div class="form-text"></div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Settings</label>
            <div>
                <asp:CheckBox ID="EnableDivisions" runat="server" Text="Structure departments under divisions" />
            </div>
            <div class="form-text"></div>
        </div>

    </div>
    <div class="col-lg-6">

        <div runat="server" id="LogoField" visible="false" class="form-group mb-3">
            <label class="form-label">
                Organization logo
            </label>
            <div>
                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="LogoUpdatePanel" />

                <insite:UpdatePanel runat="server" ID="LogoUpdatePanel">
                    <ContentTemplate>

                        <div class="mb-1">
                            <insite:Button runat="server" ID="UploadLogoButton" Icon="fas fa-upload" Text="Upload" ButtonStyle="OutlinePrimary" 
                                OnClientClick="companyEditor.showLogoUploader(); return false;" />
                            <insite:Button runat="server" ID="ReplaceLogoButton" Icon="fas fa-upload" Text="Replace" ButtonStyle="OutlinePrimary" 
                                OnClientClick="companyEditor.showLogoUploader(); return false;" />
                            <insite:Button runat="server" ID="RemoveLogoButton" Icon="fas fa-trash-alt" Text="Remove" ButtonStyle="OutlinePrimary" 
                                OnClientClick="companyEditor.removeLogo(); return false;" />
                        </div>
                        
                        <div class="form-text mb-3">
                            Supported image file formats: *.gif, *.jpg, *.png
                        </div>

                        <asp:Image ID="LogoImage" runat="server" Visible="false" CssClass="img-thumbnail rounded-0" />

                        <div class="d-none">
                            <insite:FileUploadV1 runat="server" ID="LogoUpload" 
                                AllowedExtensions=".gif,.jpg,.png" 
                                FileUploadType="Image"
                                OnClientFileUploaded="companyEditor.onLogoUploaded"
                            />
                        </div>
                    </ContentTemplate>
                </insite:UpdatePanel>
            </div>
            
        </div>

        <div runat="server" id="RowAttachments" visible="false" class="form-group mb-3">
            <label class="form-label">
                Downloads
            </label>
            <div>
                <asp:HyperLink runat="server" ID="CompanyAttachmentEditorLink" Text="Edit downloads for this organization" />
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
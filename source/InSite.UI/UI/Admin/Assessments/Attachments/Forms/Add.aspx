<%@ Page Language="C#" CodeBehind="Add.aspx.cs" Inherits="InSite.Admin.Assessments.Attachments.Forms.Add" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="Details" Src="../Controls/AttachmentDetails.ascx" %>
<%@ Register TagPrefix="uc" TagName="ImageThumbnail" Src="../Controls/AttachmentImageThumbnail.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Upload" />
    <insite:ValidationSummary runat="server" ValidationGroup="Attach" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="UploadTab" Title="Upload File" Icon="far fa-upload" IconPosition="BeforeText">
            <div class="row mt-3">
                <div class="col-lg-6">

                    <div class="card border-0 shadow-lg">
                        <div class="card-body">

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    File
                                </label>
                                <insite:FileUploadV1 runat="server" ID="FileInput"
                                    LabelText=""
                                    FileUploadType="Image"
                                    OnClientFileUploaded="addAttachment.onFileUploaded"
                                />
                                <asp:Button runat="server" ID="FileUploadedButton" CssClass="d-none" />
                            </div>

                        </div>
                    </div>

                </div>
            </div>

            <div class="mt-3">
                <insite:CancelButton runat="server" ID="CancelButton1" CausesValidation="false" />
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="AttachTab" Title="Attachment File" Icon="far fa-paperclip" IconPosition="BeforeText" Visible="false">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">
                    <div class="row">
                        <div class="col-lg-6 mb-3 mb-lg-0">

                            <div class="card">
                                <div class="card-body">
                                    <h3>Attachment</h3>

                                    <uc:Details runat="server" ID="Details" ValidationGroup="Attach" />
                                </div>
                            </div>

                        </div>
                        <div runat="server" id="ThumbnailImageColumn" class="col-lg-6" visible="false">

                            <div class="card">
                                <div class="card-body">
                                    <h3>Thumbnail</h3>

                                    <uc:ImageThumbnail runat="server" ID="Thumbnail" />
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>

            <div class="mt-3">
                <insite:SaveButton runat="server" ID="AttachButton" ValidationGroup="Attach" CausesValidation="true" />
                <insite:CancelButton runat="server" ID="CancelButton2" CausesValidation="false" />
            </div>
        </insite:NavItem>

    </insite:Nav>

    <insite:PageFooterContent runat="server">
        <script>
            (() => {
                var instance = window.addAttachment = window.addAttachment || {};

                instance.onFileUploaded = () => {
                    document.getElementById('<%= FileUploadedButton.ClientID %>').click();
                }
            })();
        </script>
    </insite:PageFooterContent>
</asp:Content>

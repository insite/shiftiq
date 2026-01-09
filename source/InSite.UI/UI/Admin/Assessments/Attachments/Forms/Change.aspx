<%@ Page Language="C#" CodeBehind="Change.aspx.cs" Inherits="InSite.Admin.Assessments.Attachments.Forms.Change" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="Details" Src="../Controls/AttachmentDetails.ascx" %>
<%@ Register TagPrefix="uc" TagName="ImageThumbnail" Src="../Controls/AttachmentImageThumbnail.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />
    <insite:ValidationSummary runat="server" ValidationGroup="ImageUpload" />

    <section>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <h4 class="card-title mb-3">
                    <i class="far fa-paperclip me-1"></i>
                    Change Attachment
                </h4>

                <div class="row">
                    <div class="col-lg-6">

                        <div class="card">
                            <div class="card-body">

                                <h3>Attachment</h3>

                                <uc:Details runat="server" ID="Details" ValidationGroup="Assessment" />

                            </div>
                        </div>

                    </div>
                    <div runat="server" id="ImageColumn" class="col-lg-6" visible="false">

                        <div class="card">
                            <div class="card-body">

                                <h3>Image</h3>

                                <div class="form-group mb-3">
                                    <div>
                                        <uc:ImageThumbnail runat="server" ID="Thumbnail" />
                                    </div>
                                </div>
                        
                                <div runat="server" id="ImageUploadField" class="form-group mb-3">
                                    <label class="form-label">
                                        Replace With File
                                    </label>
                                    <insite:FileUploadV1 runat="server" ID="FileInput"
                                        LabelText=""
                                        AllowedExtensions=".png,.gif,.jpg,.jpeg,.bmp"
                                        FileUploadType="Image"
                                        OnClientFileUploaded="changeAttachment.onImageUploaded"
                                    />
                                    <asp:Button runat="server" ID="ImageUploadedButton" CssClass="d-none" />
                                </div>

                                <div runat="server" id="ImageReplacementField" class="form-group mb-3">
                                    <label class="form-label">
                                        Replace With File
                                    </label>
                                    <div>
                                        <div class="input-group">
                                            <insite:TextBox runat="server" ID="ImageReplacementName" ReadOnly="true" style="background-color:#fff;" />
                                            <insite:Button runat="server" ID="RemoveImageReplacementButton" ButtonStyle="OutlineDanger" Icon="far fa-times" ToolTip="Remove" />
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>

                    </div>
                </div>

            </div>
        </div>
    </section>

    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Assessment" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

    <insite:PageFooterContent runat="server">
        <script>
            (() => {
                var instance = window.changeAttachment = window.changeAttachment || {};

                instance.onImageUploaded = () => {
                    document.getElementById('<%= ImageUploadedButton.ClientID %>').click();
                }
            })();
        </script>
    </insite:PageFooterContent>
</asp:Content>

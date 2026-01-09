<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProfilePictureUpload.ascx.cs" Inherits="InSite.UI.Layout.Common.Controls.ProfilePictureUpload" %>


<div class="accordion-item" style="border:none;">
    <div class="rounded-3">
        <div class="d-block d-sm-flex align-items-center">
            <asp:Image runat="server" ID="ProfileImage" CssClass="d-block rounded-circle mx-sm-0 mx-auto mb-3 mb-sm-0" Style="max-height:150px;max-width:150px;margin-left: auto !important;margin-right: auto !important;" />
        </div>
        <div class="ps-sm-3 text-center text-sm-start mt-3">
            <button class="btn btn-outline-secondary shadow btn-sm mb-2" type="button" data-bs-toggle="collapse" data-bs-target='<%= "#" + ProfileUploadPanel.ClientID %>' aria-expanded="false" aria-controls='<%= ProfileUploadPanel.ClientID %>'>
                <i class="far fa-edit me-2"></i>Replace
            </button>
            <insite:Button runat="server" ID="DeleteProfilePicture" ButtonStyle="OutlineDanger" Icon="far fa-trash-alt" CssClass="btn btn-outline-danger shadow btn-sm mb-2" OnClick="DeleteButton_Click" />
            <div class="p mb-0 fs-ms form-text">Upload a JPG or PNG image (recommended size = <%= InSite.Web.Helpers.ProfilePictureHelper.MaxProfileImageSize %>x<%= InSite.Web.Helpers.ProfilePictureHelper.MaxProfileImageSize %>px)</div>
        </div>
        <div class="accordion-collapse collapse" runat="server" id="ProfileUploadPanel" aria-labelledby='item-header-1234' data-bs-parent="#orders-accordion" enableviewstate="false">
            <div class="accordion-body pt-4 rounded-top-0 rounded-3" style="padding-bottom: 0;">
                <div class="pb-4">
                    <insite:FileUploadV2 runat="server" ID="ProfilePictureToUploadV2" LabelText="Select profile picture to upload" AllowedExtensions=".jpg,.jpeg,.gif,.png" FileUploadType="Image" />
                    <div class="pt-2">
                        <insite:SaveButton runat="server" ID="UploadProfilePicture" OnClick="UploadButton_Click" />
                        <insite:CancelButton runat="server" ID="CancelProfilePicture" ButtonStyle="OutlineSecondary" OnClientClick="return myAccount.onCancelProfilePicture();" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

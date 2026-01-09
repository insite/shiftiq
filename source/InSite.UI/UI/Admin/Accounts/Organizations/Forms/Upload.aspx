<%@ Page Language="C#" CodeBehind="Upload.aspx.cs" Inherits="InSite.Admin.Accounts.Organizations.Forms.Upload" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">

        .thumbnail-container {
            margin-top: 10px;
            padding: 0;
            border: solid 1px #eee;
            text-align: center;
        }

        .thumbnail-container img {
            padding: 10px;
        }

    </style>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="EditorStatus" />

    <div class="card border-0 shadow-lg">
        <div class="card-body">

            <h4 class="card-title mb-3">
                <i class="far fa-city me-1"></i>
                Organization
            </h4>

            <div class="row mb-3">
                <div class="col-lg-6">
                    <div class="form-group mb-3">
                        <label class="form-label">Logo</label>
                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="LogoFileUpdatePanel" />
                        <insite:UpdatePanel runat="server" ID="LogoFileUpdatePanel">
                            <ContentTemplate>
                                <insite:FileUploadV1 runat="server" ID="LogoFileInput"
                                    AllowedExtensions=".gif,.jpg,.jpeg,.png"
                                    LabelText=""
                                    FileUploadType="Image"
                                    OnClientFileUploaded="uploadForm.onLogoFileUploaded"
                                />

                                <div class="thumbnail-container" style="width: 200px;">
                                    <asp:Image runat="server" ID="LogoThumbnail" style="max-width: 200px;" />
                                </div>

                                <asp:Button runat="server" ID="LogoFileRefreshButton" style="display:none;" />
                            </ContentTemplate>
                        </insite:UpdatePanel>
                        <div class="form-text">
                            Image URL for the organization's company logo.
                        </div>
                    </div>
                </div>
                <div class="col-lg-6">
                    <div class="form-group mb-3">
                        <label class="form-label">Wallpaper</label>
                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="WallpaperFileUpdatePanel" />
                        <insite:UpdatePanel runat="server" ID="WallpaperFileUpdatePanel">
                            <ContentTemplate>
                                <div>
                                    <insite:FileUploadV1 runat="server" ID="WallpaperFileInput"
                                        AllowedExtensions=".gif,.jpg,.jpeg,.png"
                                        LabelText=""
                                        FileUploadType="Image"
                                        OnClientFileUploaded="uploadForm.onWallpaperFileUploaded"
                                    />

                                    <div class="thumbnail-container" style="width: 100%; height: 200px;">
                                        <asp:Image runat="server" ID="WallpaperThumbnail" style="max-width: 100%; max-height: 200px;" />
                                    </div>

                                    <asp:Button runat="server" ID="WallpaperFileRefreshButton" style="display:none;" />
                                </div>
                            </ContentTemplate>
                        </insite:UpdatePanel>
                        <div class="form-text">
                            Image URL for the organization's default wallpaper.
                        </div>
                    </div>
                </div>
            </div>

            <insite:SaveButton runat="server" ID="SaveButton" />
            <insite:CancelButton runat="server" ID="CancelButton" />

        </div>
    </div>

    <insite:PageFooterContent runat="server">
        <script>
            (() => {
                var uploadForm = window.uploadForm = window.uploadForm || {};

                uploadForm.onLogoFileUploaded = () => {
                    __doPostBack("<%= LogoFileRefreshButton.UniqueID %>", "")
                }

                uploadForm.onWallpaperFileUploaded = () => {
                    __doPostBack("<%= WallpaperFileRefreshButton.UniqueID %>", "")
                }
            })();
        </script>
    </insite:PageFooterContent>

</asp:Content>

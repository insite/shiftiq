<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PhotoSection.ascx.cs" Inherits="InSite.UI.Admin.Contacts.Groups.Controls.PhotoSection" %>

<%@ Import Namespace="Shift.Common" %>
<%@ Import Namespace="InSite.Common.Web" %>

<div class="card border-0 shadow-lg">
    <div class="card-body">
        <insite:Nav runat="server">

            <insite:NavItem runat="server" ID="GalleryTab" Title="Gallery">
                <insite:UpdatePanel runat="server" ID="GalleryUpdatePanel">
                    <ContentTemplate>
                        <asp:Repeater runat="server" ID="GalleryItemRepeater">
                            <HeaderTemplate><div id="<%# GalleryItemRepeater.ClientID %>" class="row"></HeaderTemplate>
                            <FooterTemplate></div></FooterTemplate>
                            <ItemTemplate>
                                <div class="col-lg-3 col-md-3 col-sm-6 mb-4">
                                    <a href="<%# Eval("ResourceUrl") %>" class="gallery-item rounded-3<%# (bool)Eval("IsVideo") ? " gallery-video" : string.Empty %>"<%# (bool)Eval("HasCaption") ? string.Format(" data-sub-html='<h6 class=\"fs-sm text-light\">{0}</h6>'", Eval("CaptionText")) : string.Empty %><%# (bool)Eval("IsVideo") ? string.Empty : " data-download-url='" + GetDownloadUrl(Eval("ID")) + "'" %>>
                                        <img src="<%# Eval("ThumbnailUrl") %>" alt="" />
                                        <%# (bool)Eval("HasCaption") ? string.Format("<span class=\"gallery-caption\">{0}</span>", Eval("CaptionText")) : string.Empty %>
                                    </a>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ContentTemplate>
                </insite:UpdatePanel>
            </insite:NavItem>

            <insite:NavItem runat="server" ID="ManageTab" Title="Manage">
                <div class="row settings">

                    <div class="col-lg-4 mb-3 mb-lg-0">
                        <h4 class="card-title mb-3">Upload</h4>

                        <insite:UpdatePanel runat="server">
                            <Triggers>
                                <asp:PostBackTrigger ControlID="UploadButton" />
                            </Triggers>
                            <ContentTemplate>
                                <insite:LoadingPanel runat="server" ID="VideoUploadLoadingPanel" />

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Upload Type
                                        <insite:RequiredValidator runat="server" ControlToValidate="UploadType" FieldName="Upload Type" ValidationGroup="GroupPhotosUpload" />
                                    </label>
                                    <insite:ComboBox runat="server" ID="UploadType">
                                        <Items>
                                            <insite:ComboBoxOption Value="image" Text="Image" />
                                            <insite:ComboBoxOption Value="video" Text="Video" />
                                        </Items>
                                    </insite:ComboBox>
                                    <div class="form-text">
                                    </div>
                                </div>

                                <insite:Container runat="server" ID="ImageUploadContainer">
                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Upload Image
                                            <insite:RequiredValidator runat="server" FieldName="Upload Image" ControlToValidate="ImageUpload" Display="Dynamic" ValidationGroup="GroupPhotosUpload" />
                                            <insite:FileExtensionValidator runat="server" ControlToValidate="ImageUpload" Display="None" ValidationGroup="GroupPhotosUpload"
                                                FileExtensions="png,gif,jpg,jpeg,bmp" />
                                        </label>
                                        <div>
                                            <div class="input-group">
                                                <insite:TextBox runat="server" ReadOnly="true" style="background-color: #fff;" />
                                                <button runat="server" id="ImageOpenUpload" class="btn btn-icon btn-outline-secondary" title="Browse" type="button"><i class="far fa-folder-open"></i></button>
                                            </div>
                                            <div class="d-none">
                                                <asp:FileUpload runat="server" ID="ImageUpload" Width="100%" />
                                            </div>
                                        </div>
                                        <div class="form-text">
                                            File types supported: .png, .gif, .jpg, .bmp
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Image Caption
                                        </label>
                                        <insite:TextBox runat="server" ID="ImageCaption" />
                                        <div class="form-text">
                                        </div>
                                    </div>
                                </insite:Container>

                                <insite:Container runat="server" ID="VideoUploadContainer">
                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Video URL
                                            <insite:RequiredValidator runat="server" ControlToValidate="VideoUrl" FieldName="Video URL" ValidationGroup="GroupPhotosUpload" />
                                            <insite:CustomValidator runat="server" ID="VideoUrlValidator" ErrorMessage="Invalid Video URL" ValidationGroup="GroupPhotosUpload"
                                                ClientValidationFunction="gallery.validateVideoUrl" Display="None" />
                                        </label>
                                        <div>
                                            <insite:TextBox runat="server" ID="VideoUrl" />
                                            <asp:HiddenField runat="server" ID="VideoInfo" />
                                        </div>
                                        <div class="form-text">
                                            Services supported: Vimeo, YouTube
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Video Caption
                                        </label>
                                            <insite:TextBox runat="server" ID="VideoCaption" />
                                        <div class="form-text">
                                        </div>
                                    </div>
                                </insite:Container>

                                <div class="mt-3">
                                    <insite:Button runat="server" ID="UploadButton" ButtonStyle="OutlinePrimary"
                                        Icon="fas fa-upload" Text="Upload"
                                        CausesValidation="true" ValidationGroup="GroupPhotosUpload" />
                                </div>
                            </ContentTemplate>
                        </insite:UpdatePanel>
                    </div>

                    <div class="col-lg-8">
                        <insite:UpdatePanel runat="server" ID="ManagerUpdatePanel">
                            <ContentTemplate>
                                <insite:Container runat="server" ID="ManageContainer">
                                    <h4 class="card-title mb-3">Photos</h4>

                                    <asp:Repeater runat="server" ID="ManageItemRepeater">
                                        <HeaderTemplate>
                                            <table id='<%# ManageItemRepeater.ClientID %>' class="table table-striped table-sm table-gallery-manage">
                                                <thead>
                                                    <tr>
                                                        <th></th>
                                                        <th>Name</th>
                                                        <th class="cell-size">Size</th>
                                                        <th>Caption</th>
                                                        <th></th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                        </HeaderTemplate>
                                        <FooterTemplate></tbody></table></FooterTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td class="cell-img">
                                                    <a runat="server" visible='<%# !(bool)Eval("IsVideo") %>' target="_blank" href='<%# GetDownloadUrl(Eval("ID")) %>'>
                                                        <img src='<%# Eval("ThumbnailUrl") %>' alt="" />
                                                    </a>
                                                    <a runat="server" visible='<%# (bool)Eval("IsVideo") %>' target="_blank" href='<%# Eval("ResourceUrl") %>'>
                                                        <img src='<%# Eval("ThumbnailUrl") %>' alt="" />
                                                    </a>
                                                </td>
                                                <td class="cell-name" title="<%# Eval("ResourceName") %>">
                                                    <%# Eval("ResourceName") %>
                                                </td>
                                                <td class="cell-size"><%# Eval("Size") %></td>
                                                <td class="cell-caption">
                                                    <insite:TextBox runat="server" ID="Caption" Text='<%# Eval("CaptionText") %>' />
                                                </td>
                                                <td class="cell-cmd">
                                                    <insite:IconButton runat="server" CommandName="Delete" ToolTip="Delete Resource" Name="trash-alt" ConfirmText="Are you sure you want to delete this resource?" />
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>

                                    <div class="mt-3 text-end">
                                        <insite:SaveButton runat="server" ID="ManageSaveButton" CausesValidation="false" ButtonStyle="OutlineSuccess" />
                                        <insite:CancelButton runat="server" ID="ManageCancelButton" CausesValidation="false" ButtonStyle="OutlineSecondary" />
                                    </div>
                                </insite:Container>
                            </ContentTemplate>
                        </insite:UpdatePanel>
                    </div>

                </div>
            </insite:NavItem>

        </insite:Nav>
    </div>
</div>

<insite:PageHeadContent runat="server" ID="CommonStyle">
    <style type="text/css">
        .table-gallery-manage td.cell-name {
            max-width: 270px;
            overflow: hidden;
            text-overflow: ellipsis;
            white-space: nowrap;
            vertical-align: middle;
        }

        .table-gallery-manage td.cell-size {
            white-space: nowrap;
            text-align: right;
            vertical-align: middle;
        }

        .table-gallery-manage th.cell-size {
            text-align: right;
        }

        .table-gallery-manage td.cell-caption {
            width: 40%;
        }

        .table-gallery-manage td.cell-img {
            vertical-align: middle;
            width: 50px;
        }

        .table-gallery-manage td.cell-img img {
            aspect-ratio: 3/2;
            display: block;
            width: 35px;
            height: 100%;
            object-fit: cover;
        }

        .table-gallery-manage td.cell-cmd {
            white-space: nowrap;
            vertical-align: middle;
            width: 35px;
        }

        .gallery-item {
            aspect-ratio: 3/2;
        }

            .gallery-item > img {
                width: 100%;
                height: 100%;
                object-fit: cover;
            }
    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server" ID="CommonScript">
    <script type="text/javascript">
        (function () {
            var instance = window.gallery = window.gallery || {};

            var galleryContainer;
            var $imageUpload, $imageOpenUpload, $videoUrl, $videoLoadingPanel, $videoCaption, $videoInfo;

            var youTubeLinkPatterns = <%= JsonHelper.SerializeJsObject(UrlHelper.YouTubeLinkPatterns.Select(x => x.ToString())) %>;
            var vimeoLinkPatterns = <%= JsonHelper.SerializeJsObject(UrlHelper.VimeoLinkPatterns.Select(x => x.ToString())) %>;

            for (var i = 0; i < youTubeLinkPatterns.length; i++)
                youTubeLinkPatterns[i] = new RegExp(youTubeLinkPatterns[i]);

            for (var i = 0; i < vimeoLinkPatterns.length; i++)
                vimeoLinkPatterns[i] = new RegExp(vimeoLinkPatterns[i]);


            Sys.Application.add_load(function () {
                if (!galleryContainer || !document.body.contains(galleryContainer)) {
                    galleryContainer = document.getElementById('<%= GalleryItemRepeater.ClientID %>');
                    lightGallery(galleryContainer, {
                        selector: '.gallery-item',
                        autoplayFirstVideo: false
                    });
                }

                if (!$imageUpload || $imageUpload.length == 0 || !document.body.contains($imageUpload[0])) {
                    $imageUpload = $(document.getElementById('<%= ImageUpload.ClientID %>'));

                    $imageOpenUpload = $(document.getElementById('<%= ImageOpenUpload.ClientID %>'))
                        .on('click', onImageOpenUploadClick);

                    $(document.getElementById('<%= ImageUpload.ClientID %>'))
                        .on('change', onImageUploadChange);

                    $videoUrl = $(document.getElementById('<%= VideoUrl.ClientID %>'))
                        .on('change', onVideoUrlChange);

                    $videoLoadingPanel = $(document.getElementById('<%= VideoUploadLoadingPanel.ClientID %>'));
                    $videoCaption = $(document.getElementById('<%= VideoCaption.ClientID %>'));
                    $videoInfo = $(document.getElementById('<%= VideoInfo.ClientID %>'));
                }
            });

            instance.validateVideoUrl = function (s, e) {
                var videoUrl = document.getElementById('<%= VideoUrl.ClientID %>').value;
                var videoInfo = document.getElementById('<%= VideoInfo.ClientID %>').value;
                e.IsValid = videoUrl.length == 0 || videoInfo != null && videoInfo.length > 0;
            };

            function onImageOpenUploadClick() {
                $imageUpload.click();
            }

            function onImageUploadChange() {
                var fileName = '';

                if (this.files) {
                    if (this.files.length > 0) {
                        fileName = this.files[0].name;
                    }
                } else if (this.value) {
                    fileName = this.value.split(/(\\|\/)/g).pop();
                }

                $imageOpenUpload.closest('.input-group').find('input[type="text"]').val(fileName);

                var fileNameWithoutExt = '';
                if (fileName) {
                    var matches = fileName.match(/(.+?)\.[^\.]/);
                    if (matches.length == 2)
                        fileNameWithoutExt = matches[1];
                }

                document.getElementById('<%= ImageCaption.ClientID %>').value = fileNameWithoutExt;
            }

            function onVideoUrlChange() {
                $videoCaption.val('');
                $videoInfo.val('');

                var oEmbedUrl = null;
                var url = $videoUrl.val();

                if (isYouTubeUrl(url)) {
                    oEmbedUrl = 'https://www.youtube.com/oembed?url=' + encodeURIComponent(url) + '&format=json';
                } else if (isVimeoUrl(url)) {
                    oEmbedUrl = 'https://vimeo.com/api/oembed.json?url=' + encodeURIComponent(url);
                }

                if (!oEmbedUrl)
                    return;

                $videoLoadingPanel.show();

                $.ajax({
                    type: 'GET',
                    dataType: 'json',
                    url: oEmbedUrl,
                    success: function (result) {
                        var title = result['title'];
                        if (title)
                            $videoCaption.val(title)

                        $videoInfo.val(JSON.stringify(result));
                    },
                    complete: function () {
                        $videoLoadingPanel.hide();
                    },
                });
            }

            function isYouTubeUrl(url) {
                for (var i = 0; i < youTubeLinkPatterns.length; i++) {
                    if (youTubeLinkPatterns[i].test(url))
                        return true;
                }

                return false;
            }

            function isVimeoUrl(url) {
                for (var i = 0; i < vimeoLinkPatterns.length; i++) {
                    if (vimeoLinkPatterns[i].test(url))
                        return true;
                }

                return false;
            }
        })();

    </script>
</insite:PageFooterContent>

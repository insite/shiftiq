<%@ Page Language="C#" CodeBehind="Backgrounds.aspx.cs" Inherits="InSite.UI.Portal.Accounts.Users.SelectBackground" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

<section class="container">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:RadioButton runat="server" ID="RadioNone" Text="None" Value="None" GroupName="Radio" Checked="true" />
    <insite:RadioButton runat="server" ID="RadioColor" Text="Color" Value="Color" GroupName="Radio" />
    <insite:RadioButton runat="server" ID="RadioImage" Text="Image" Value="Image" GroupName="Radio" />

    <asp:Panel runat="server" ID="NonePanel" style="padding-top: 10px;">
        <insite:SaveButton runat="server" ID="SaveNoneButton" />
    </asp:Panel>

    <asp:Panel runat="server" ID="ColorPanel">
        <div style="padding: 10px 0;">
            <input runat="server" id="ColorSelector" class="form-control form-control-color" type="color" value="#35cfe3" />
        </div>
        <insite:SaveButton runat="server" ID="SaveColorButton" />
    </asp:Panel>

    <asp:Panel runat="server" ID="ImagePanel" style="padding-top: 10px;">
        <insite:Accordion runat="server" IsFlush="false">
            <insite:AccordionPanel runat="server" Icon="far fa-tv" Title="Wallpapers">

                <div class="row settings">
                    <div class="col-md-12">

                        <div class="text-start">
                            <insite:ClearButton runat="server" ID="ClearWallpaperButton" />
                        </div>

                        <asp:Repeater runat="server" ID="WallpaperRepeater">
                            <HeaderTemplate><div class="row"></HeaderTemplate>
                            <FooterTemplate></div></FooterTemplate>
                            <ItemTemplate>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class='wallpaper-thumbnail' data-id="<%# Eval("Filename") %>">
                                        <div class="image-container" data-url="<%# VirtualPathUtility.ToAbsolute((string)Eval("Url")) %>">
                                            <i class='far fa-file-image'></i>
                                        </div>
                                        <span><%# Eval("Title") %></span>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>

                        <asp:HiddenField runat="server" ID="SelectedWallpaper" />

                    </div>
                </div>
        
                <div style="display:none;">
                    <insite:SaveButton runat="server" ID="SaveWallpaperButton" ValidationGroup="Wallpaper" />
                </div>

            </insite:AccordionPanel>
        </insite:Accordion>
    </asp:Panel>

</section>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        .wallpaper-thumbnail {
            border-radius: 5px;
            border: 1px solid #eeeeee;
            margin: 15px 0;
            overflow: hidden;
            cursor: pointer;
        }

            .wallpaper-thumbnail > .image-container {
                text-align: center;
                width: 228px;
                height: 134px;
                margin: 16px auto 11px auto;
                background-color: rgba(0, 0, 0, 0);
                background-size: cover !important;
            }

                .wallpaper-thumbnail > .image-container > i.fa,
                .wallpaper-thumbnail > .image-container > i.fas,
                .wallpaper-thumbnail > .image-container > i.far,
                .wallpaper-thumbnail > .image-container > i.fab,
                .wallpaper-thumbnail > .image-container > i.fal {
                    font-size: 56px;
                    margin: 39px 0;
                    color: #d0d0d0;
                }

            .wallpaper-thumbnail > span {
                text-align: center;
                display: block;
                padding: 0 0 12px 0;
            }

            .wallpaper-thumbnail:not(.selected):hover {
                border: 1px solid #dddddd;
            }

            .wallpaper-thumbnail.selected {
                border-color: #66afe9;
                outline: 0;
                box-shadow: inset 0 1px 1px rgba(0,0,0,.075), 0 0 8px rgba(102, 175, 233, .6);
            }
    </style>

</insite:PageHeadContent>

<insite:PageFooterContent runat="server">

    <script type="text/javascript">
        (function () {
            var $input = null;

            function initThumbnail() {
                var $thumbnail = $(this);

                var wallpaperPath = $input.val();
                if (wallpaperPath.length > 0 && $thumbnail.data('id').toUpperCase() == wallpaperPath.toUpperCase())
                    $thumbnail.addClass('selected');

                $thumbnail.find('> .image-container').each(function () {
                    var $imgContainer = $(this);

                    var img = new Image();
                    $(img).data('container', $imgContainer);
                    img.onload = onThumbnailImgLoad;
                    img.src = '/Web/Persistence/thumbnail.ashx?width=228&height=134&fill=0&imageUrl=' + String($imgContainer.data('url'));
                });
            }

            function onThumbnailImgLoad() {
                var $img = $(this);

                var $imgContainer = $img.data('container');
                if ($imgContainer) {
                    $imgContainer.empty();
                    $imgContainer.css('background-image', 'url(' + $img.prop('src') + ')');
                }
            }

            function onThumbnailClick() {
                var $this = $(this);
                if ($this.hasClass('selected'))
                    return;

                $this.closest('.row').find('.wallpaper-thumbnail.selected').removeClass('selected');
                $this.addClass('selected');

                $input.val($this.data('id'));
                __doPostBack('<%= SaveWallpaperButton.UniqueID %>', '');
            }

            Sys.Application.add_load(function () {
                $input = $('#<%= SelectedWallpaper.ClientID %>');
                if ($input.data('inited') === true)
                    return;

                $('.wallpaper-thumbnail')
                    .each(initThumbnail)
                    .on('click', onThumbnailClick);

                $input.data('inited', true);
            });
        })();
    </script>

</insite:PageFooterContent>

</asp:Content>

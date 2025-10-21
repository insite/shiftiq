<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImageRepeater.ascx.cs" Inherits="InSite.Admin.Assessments.Attachments.Controls.ImageRepeater" %>

<insite:PageHeadContent runat="server" ContentKey="BankImageRepeater">

    <style type="text/css">
        .bank-image-repeater .file-thumbnail > .file-preview {
            text-align: center;
        }

        .bank-image-repeater .file-thumbnail > label {
            line-height: 1;
            margin: 0;
            font-size: 0.8em;
        }

        .bank-image-repeater .file-thumbnail > span {
            color: #000;
            line-height: 1.1;
            margin-bottom: 0.2em;
        }

        .bank-image-repeater .file-thumbnail > div.icon-wrapper {
            position: absolute;
            right: 10px;
        }

            .bank-image-repeater .file-thumbnail > div.icon-wrapper > .label {
                padding: 0.1em 0.6em 0.1em;
            }

    </style>

</insite:PageHeadContent>

<insite:Alert runat="server" ID="ControlAlert" />

<div runat="server" id="Container" class="bank-image-repeater">
    <asp:Repeater runat="server" ID="Repeater">
        <ItemTemplate>
            <div class="col-md-3 col-sm-4 col-files">
                <div class="file-thumbnail">
                    <div class="file-preview">
                        <a href='<%# Eval("Url") %>'><img alt='' src='<%# string.Format("{0}?timestamp={1}", Eval("Url"), DateTime.UtcNow.Ticks) %>'></a>
                    </div>
                    <div class="icon-wrapper">
                        <%# Eval("Icon") %>
                    </div>
                    <insite:Container runat="server" Visible='<%# !(bool)Eval("IsAttached") %>'>
                        <label>File Name</label>
                        <span runat="server" title='<%# Eval("Name") %>' class="file-name"><%# Eval("Name") %></span>
                        <label>Dimension</label>
                        <span class="img-size"><span>0 x 0</span></span>
                    </insite:Container>
                    <insite:Container runat="server" Visible='<%# (bool)Eval("IsAttached") %>'>
                        <label>Asset Title</label>
                        <span runat="server" title='<%# Eval("Attachment.Title") %>' class="file-title"><%# Eval("Attachment.Title") %></span>
                        <label>Asset #</label>
                        <span><%# Eval("Attachment.Number") %></span>
                        <label>Condition</label>
                        <span><%# Eval("Attachment.Condition") %></span>
                        <label>Publication Status</label>
                        <span><%# Eval("Attachment.PublicationStatus") %></span>
                        <label>File Name</label>
                        <span runat="server" title='<%# Eval("Name") %>' class="file-name"><%# Eval("Name") %></span>
                        <label>Dimension</label>
                        <span><%# Eval("Attachment.Dimension") %></span>
                    </insite:Container>
                    <div style="clear:both;"></div>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>

<insite:PageFooterContent runat="server" ContentKey="BankImageRepeater">

    <script type="text/javascript">

        (function () {
            var instance = window.attachmentsImageRepeater = window.attachmentsImageRepeater || {};
            var containers = {};

            $(window).on('resize', onWindowResize);

            instance.init = function (id) {
                var isAdded = containers.hasOwnProperty(id);
                var $container = $getContainer(id);

                if ($container.length !== 1) {
                    if (isAdded)
                        delete containers[id];

                    return;
                }                    

                if (!isAdded)
                    containers[id] = {};

                var data = containers[id];
                data.prevColPerRow = 0;
                data.$images = $container.find('img').on('load', onImageLoaded);

                $container.parents('.tab-pane').each(function () {
                    $('[data-bs-target="#' + this.id + '"][data-bs-toggle]')
                        .off('shown.bs.tab', onTabShow)
                        .on('shown.bs.tab', onTabShow);
                });

                $container.parents('.panel-collapse')
                    .off('shown.bs.collapse', onPanelCollapse)
                    .on('shown.bs.collapse', onPanelCollapse);

                updateColumns($container, data);

                data.$images.each(function () {
                    this.src = this.src;
                });
            };

            // events

            function onWindowResize() {
                updateImages();
            }

            function onTabShow() {
                updateImages();
            }

            function onPanelCollapse() {
                updateImages();
            }

            function onImageLoaded() {
                var $this = $(this);

                var $filePreview = $this.closest('.file-preview');
                if ($filePreview.length === 1) {
                    $this.css({ opacity: 1 });

                    setImageOffset($this);
                }

                var $sizeContainer = $this.closest('.file-thumbnail').find('> .img-size').empty();
                if ($sizeContainer.length !== 1)
                    return;

                var width = parseInt($this.prop('naturalWidth'));
                if (isNaN(width))
                    width = 0;

                var height = parseInt($this.prop('naturalHeight'));
                if (isNaN(height))
                    height = 0;

                $sizeContainer.append($('<span>').text(String(width) + ' x ' + String(height)));
            }

            // methods

            function updateImages() {
                for (var id in containers) {
                    if (!containers.hasOwnProperty(id))
                        continue;

                    var $container = $getContainer(id);
                    if ($container.length !== 1) {
                        delete containers[id];
                        continue;
                    }

                    if (!$container.is(':visible'))
                        continue;

                    var data = containers[id];
                    data.$images.each(function () {
                        setImageOffset(this);
                    });

                    updateColumns($container, data);
                }
            }

            function updateColumns($container, data) {
                var colPerRow = 4;
                if ($(window).width() < 991)
                    colPerRow = 3;

                if (data.prevColPerRow == colPerRow)
                    return;

                data.prevColPerRow = colPerRow;

                var $row = null;
                var $cols = $container.find('div.col-files').detach();

                $container.empty();

                for (var i = 0; i < $cols.length; i++) {
                    if (i % colPerRow == 0)
                        $container.append($row = $('<div class="row">'));

                    $row.append($cols[i]);
                }
            }

            function setImageOffset(img) {
                var $img = $(img);
                var $filePreview = $img.closest('.file-preview');
                if ($filePreview.length == 0)
                    return;

                var maxWidth = $filePreview.width();
                var maxHeight = $filePreview.height();

                var imageWidth = $img.width();
                var imageHeight = $img.height();

                var hOffset = 0, vOffset = 0;

                if (imageWidth < maxWidth)
                    hOffset = (maxWidth - imageWidth) / 2;

                if (imageHeight < maxHeight)
                    vOffset = (maxHeight - imageHeight) / 2;

                $img.css({
                    marginTop: vOffset,
                    marginLeft: hOffset,
                    marginBottom: vOffset,
                    marginRight: hOffset,
                });
            }

            function $getContainer(id) {
                return $('div#' + String(id) + '.bank-image-repeater');
            }
        })();

    </script>

</insite:PageFooterContent>

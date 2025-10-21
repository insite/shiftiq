<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttachmentImageThumbnail.ascx.cs" Inherits="InSite.Admin.Assessments.Attachments.Controls.AttachmentImageThumbnail" %>

<div style="display:block; transition:none;">
    <img runat="server" id="ThumbnailImage" alt="Uploaded Image" style="max-height:100%; max-width:100%;" />
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            var $image = $('img#<%= ThumbnailImage.ClientID %>').on('load', onImageLoaded);
            if ($image.length == 0)
                return;
            
            var isResized = false;

            // init

            $image.each(function () {
                this.src = this.src;
            });

            $(window).on('resize', onWindowResize).on('click', onWindowClick);

            onWindowResize();

            // events handlers

            function onImageLoaded() {
                var $this = $(this);

                var $filePreview = $this.closest('div');
                if ($filePreview.length === 1) {
                    $this.css({ opacity: 1 });

                    setImageOffset($this);
                }
            }

            function onWindowClick() {
                if (isResized)
                    onWindowResize();
            }

            function onWindowResize() {
                isResized = true;

                if (!$image.is(':visible'))
                    return;

                $image.each(function () {
                    setImageOffset(this);
                });

                isResized = false;
            }

            // methods

            function setImageOffset(img) {
                var $img = $(img);

                var $preview = $img.closest('div');
                if ($preview.length == 0)
                    return;

                var maxWidth = $preview.width();
                var maxHeight = maxWidth / 1.777;

                if (maxWidth == 0)
                    return;

                $preview.height(maxHeight);

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
        })();
    </script>
</insite:PageFooterContent>
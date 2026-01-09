<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FieldWebTemplateImage.ascx.cs" Inherits="InSite.UI.Layout.Common.Controls.Editor.FieldWebTemplateImage" %>

<insite:TextBox runat="server" MaxLength="12" />

<insite:TextBox  runat="server" ID="ImgAlt" EmptyMessage="Img Alt Text" MaxLength="64" Width="20%" />
<insite:TextBox  runat="server" ID="ImgUrl" EmptyMessage="Img URL" MaxLength="512" style="width:calc(100% - 20% - 51px);" />
<button class="btn btn-default" title="Browse" type="button"
    data-upload="#<%= FileUpload.ClientID %>" data-btn="#<%= FileUploadButton.ClientID %>">
    <i class="far fa-folder-open"></i>
</button>

<div class="d-none">
    <asp:FileUpload runat="server" ID="FileUpload" Width="100%" />
    <asp:Button runat="server" ID="FileUploadButton" CausesValidation="true" />
    <insite:RequiredValidator runat="server" ID="FileUploadValidator" ControlToValidate="FileUpload" Display="None" />
</div>

<insite:PageFooterContent runat="server" ID="CommonScript">
    <script type="text/javascript">
        (function () {
            $('[data-upload]').each(function () {
                var $browseBtn = $(this);
                var $upload = $($browseBtn.data('upload'));
                var $uploadBtn = $($browseBtn.data('btn'));

                if ($upload.length !== 1 || $uploadBtn.length !== 1)
                    return;

                $upload.on('change', function () {
                    var fileName = '';

                    if (this.files) {
                        if (this.files.length > 0) {
                            fileName = this.files[0].name;
                        }
                    } else if (this.value) {
                        fileName = this.value.split(/(\\|\/)/g).pop();
                    }

                    if (typeof fileName === 'string' && fileName.length > 0)
                        $uploadBtn.click();
                });
            }).on('click', function () {
                var uploadSelector = $(this).data('upload');
                $(uploadSelector).click();
            });
        })();
    </script>
</insite:PageFooterContent>
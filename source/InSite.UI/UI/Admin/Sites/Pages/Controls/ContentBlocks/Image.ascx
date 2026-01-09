<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Image.ascx.cs" Inherits="InSite.Admin.Sites.Pages.Controls.ContentBlocks.Image" %>

<insite:TextBox runat="server" ID="ImgAlt" EmptyMessage="Img Alt Text" MaxLength="64" Width="20%" />
<insite:TextBox runat="server" ID="ImgUrl" EmptyMessage="Img URL" MaxLength="512" style="width:calc(100% - 20% - 51px);" />
<insite:Button runat="server" ID="FileUploadButton" Icon="far fa-folder-open" title="Browse" ButtonStyle="Default" CausesValidation="true" />

<div class="d-none">
    <asp:FileUpload runat="server" ID="FileUpload" Width="100%" />
    <insite:RequiredValidator runat="server" ID="FileUploadValidator" ControlToValidate="FileUpload" Display="None" />
</div>

<insite:PageFooterContent runat="server" ID="CommonScript">
    <script type="text/javascript">
        (function () {
            var instance; {
                var contentBlocks = window.contentBlocks = window.contentBlocks || {};
                instance = contentBlocks.image = contentBlocks.image || {};
            }

            instance.init = function (triggerId, uploadId) {
                var $trigger = $('#' + String(triggerId));
                var $upload = $('#' + String(uploadId));

                if ($trigger.length !== 1 || $upload.length !== 1 || $trigger.data('data'))
                    return;

                var data = {
                    $trigger: $trigger,
                    $upload: $upload,
                    submit: false
                };

                $trigger
                    .data('data', data)
                    .on('click', onTriggerClick);

                $upload
                    .data('data', data)
                    .on('change', onUploadChange);
            };

            function onTriggerClick(e) {
                var data = $(this).data('data');
                if (!data)
                    return;

                if (data.submit === true) {
                    data.submit = false;
                    return;
                }

                e.preventDefault();

                data.$upload.click();
            }

            function onUploadChange() {
                var data = $(this).data('data');
                if (!data)
                    return;

                var fileName = '';

                if (this.files) {
                    if (this.files.length > 0) {
                        fileName = this.files[0].name;
                    }
                } else if (this.value) {
                    fileName = this.value.split(/(\\|\/)/g).pop();
                }

                if (typeof fileName === 'string' && fileName.length > 0) {
                    data.submit = true;
                    data.$trigger[0].click();
                }
            }
        })();
    </script>
</insite:PageFooterContent>
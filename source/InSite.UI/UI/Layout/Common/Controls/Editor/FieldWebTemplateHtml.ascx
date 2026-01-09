<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FieldWebTemplateHtml.ascx.cs" Inherits="InSite.UI.Layout.Common.Controls.Editor.FieldWebTemplateHtml" %>

<div>
    <insite:TextBox runat="server" ID="HtmlInput" Width="100%" TextMode="MultiLine" AllowHtml="true" />

    <div class="d-none">
        <asp:FileUpload runat="server" ID="FileUpload" />
        <asp:Button runat="server" ID="UploadFileButton" CausesValidation="true" CssClass="btn-upload" />
        <insite:RequiredValidator runat="server" ID="FileUploadRequiredValidator" ControlToValidate="FileUpload"
            FieldName="File" Display="None" />
        <insite:FileExtensionValidator runat="server" ID="FileUploadExtensionValidator" ControlToValidate="FileUpload"
            FileExtensions="png,gif,jpg,jpeg,doc,docx,ppt,pptx,xls,xlsx,txt,pdf,zip" Display="None" />
    </div>
</div>

<insite:PageFooterContent runat="server" ID="CommonScript">
    <script type="text/javascript">

        (function () {
            // public methods

            var instance = window.webTemplateHtml = window.webTemplateHtml || {};

            instance.init = function (selector) {
                var $input = $(selector);
                if ($input.length !== 1)
                    return;

                initEditor($input);

                $input.parents('.tab-pane').each(function () {
                    $('[data-bs-target="#' + this.id + '"][data-bs-toggle]').on('shown.bs.tab', function () {
                        initEditor($input);
                    });
                });

                initUpload($input);
            };

            // initialization

            function initEditor($input) {
                if ($input.data('inited') === true || !$input.is(':visible'))
                    return;

                $input.summernote({
                    disableDragAndDrop: true,
                    disableResizeEditor: true,
                    dialogsInBody: true,
                    minHeight: 130,
                    shortcuts: false,
                    toolbar: [
                        ['cleaner', ['cleaner']],
                        ['style', ['style']],
                        ['style', ['bold', 'italic', 'underline', 'clear']],
                        ['font', ['strikethrough', 'superscript', 'subscript']],
                        ['color', ['color']],
                        ['para', ['ul', 'ol', 'paragraph', 'height']],
                        ['table', ['table']],
                        ['insert', ['link']],
                        ['view', ['fullscreen', 'codeview']]
                    ],
                    cleaner: {
                        action: 'button',
                        newline: '<br>',
                        icon: '<i class="fas fa-sparkles"></i>',
                        keepHtml: true,
                        keepOnlyTags: ['<p>', '<br>', '<ul>', '<li>', '<b>', '<strong>', '<i>', '<a>', '<img>'],
                        keepClasses: false,
                        badTags: ['style', 'script', 'html', 'embed', 'noframes', 'noscript', 'applet'],
                        badAttributes: ['class', 'start', 'style'],
                        limitChars: false,
                        limitDisplay: 'both',
                        limitStop: false
                    }
                });

                $input.data('inited', true);
            }

            function initUpload($input) {
                var $parent = $input.closest('div');
                var $formGroup = $parent.closest('.form-group').on('drop', onFileDrop);
                var $uploadButton = $formGroup.find('.btn-upload');
                var $upload = $formGroup.find('input[type="file"]').on('change', onUploadFileChanged);
                var $dropMessage = $('<div class="loading-panel content-field-dropmessage"><div><div>Drop File Here</div></div></div>')
                    .on('dragenter', onDropMessageDragEnter).on('dragleave', onDropMessageDragLeave);
                var $loadingMessage = $('<div class="loading-panel content-field-loadingmessage"><div><div><i class="fa fa-spinner fa-pulse fa-3x"></i></div></div></div>');

                $upload.data('refs', { $button: $uploadButton, $input: $input, $loading: $loadingMessage });
                $formGroup.data('refs', { $upload: $upload, $button: $uploadButton, $dropMessage: $dropMessage, $loading: $loadingMessage }).append($dropMessage).append($loadingMessage);

                $parent.append(
                    $('<div class="form-text my-3">').append(
                        document.createTextNode('Attach files by dragging and dropping or '),
                        $('<a href="#fileupload">').text('selecting').on('click', onFileUploadClick).data('refs', { $upload: $upload }),
                        document.createTextNode(' them. File types supported: .png .gif .jpg .jpeg .doc .docx .ppt .pptx .xls .xlsx .txt .pdf .zip'),
                    )
                );
            }

            function uploadFile(form, formData, $loading, $input) {
                $loading.show();

                $.ajax({
                    url: form.action,
                    type: form.method,
                    data: formData,
                    dataType: 'json',
                    cache: false,
                    contentType: false,
                    processData: false,

                    complete: function () {
                        $loading.hide();
                    },
                    success: function (data) {
                        var msg = '';

                        if (data.Type == 'OK') {
                            var url = data.Path + '/' + data.Name;

                            $input.summernote('focus');
                            $input.summernote('setLastRange');

                            if (data.IsImage)
                                $input.summernote('insertImage', url, data.Name);
                            else
                                $input.summernote('createLink', { text: data.Name, url: url, newWindow: false });
                        } else if (data.Type == 'ERROR') {
                            if (data.Messages.length == 0) {
                                msg = 'An error occurred during uploading the file.';
                            } else {
                                for (var i = 0; i < data.Messages.length; i++) {
                                    if (msg != null)
                                        msg += '\r\n\r\n';

                                    msg += data.Messages[i];
                                }
                            }
                        } else {
                            msg = 'Unexpected response type: ' + String(data.Type);
                        }

                        if (msg.length > 0)
                            alert(msg);
                    },
                    error: function () {
                        alert('An error occurred during uploading the file.');
                    }
                });
            }

            // event handlers

            function onDropMessageDragEnter(e) {
                $(this).addClass('drag-over');
            }

            function onDropMessageDragLeave(e) {
                var $this = $(this);
                if (e.relatedTarget === null || $this.has(e.relatedTarget).length <= 0 || e.relatedTarget.nodeType !== 3 && e.relatedTarget !== e.target)
                    $(this).removeClass('drag-over');
            }

            function onFileUploadClick(e) {
                e.preventDefault();

                var refs = $(this).data('refs');

                refs.$upload.click();
            }

            function onUploadFileChanged(e) {
                if (this.value == null || this.value.length == 0)
                    return;

                var refs = $(this).data('refs');
                var form = $('form#aspnetForm')[0];
                var $eventTarget = $('#__EVENTTARGET').val(refs.$button.prop('name'));
                var formData = new FormData(form);
                $eventTarget.val('');

                uploadFile(form, formData, refs.$loading, refs.$input);
            }

            function onFileDrop(e) {
                var refs = $(this).data('refs');
                refs.$dropMessage.removeClass('drag-over');

                var dt = e.originalEvent.dataTransfer;
                if (dt === null || typeof dt === 'undefined')
                    return;

                var files = dt.files;
                if (files.length == 0)
                    return;

                e.preventDefault();

                var $form = $('form');

                var formData = new FormData();
                $form.find('input').each(function () {
                    if (this.name === refs.$upload.prop('name')) {
                        formData.append(this.name, files[0]);
                    } else if (this.name === '__EVENTTARGET') {
                        formData.append(this.name, refs.$button.prop('name'));
                    } else if (this.type == 'submit' || this.type == 'button') {
                        if (this.name == refs.$button.prop('name'))
                            formData.append(this.name, this.value);
                    } else {
                        formData.append(this.name, this.value);
                    }
                });

                var uploadRefs = refs.$upload.data('refs');

                uploadFile($form.get(0), formData, refs.$loading, uploadRefs.$input);
            }
        })();

    </script>
</insite:PageFooterContent>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditDetailContent.ascx.cs" Inherits="InSite.Admin.Courses.Resources.Controls.EditDetailContent" %>

<insite:PageHeadContent runat="server">
    <style type="text/css">

        iframe.html-content {
            width: 100%;
            border: none;
        }

        .modal.link-dialog {
            z-index: 8501;
        }

        .editor-busy-panel {
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            z-index: 2;
            display: none;
            cursor: wait;
        }

            .editor-busy-panel > div {
                display: table;
                width: 100%;
                height: 100%;
            }

                .editor-busy-panel > div > div {
                    display: table-cell;
                    vertical-align: middle;
                    text-align: center;
                    background-color: rgba(255, 255, 255, 0.65);
                }

                    .editor-busy-panel > div > div > span {
                        margin-top: 8px;
                        display: block;
                    }

    </style>
</insite:PageHeadContent>

<insite:Modal runat="server" ID="HtmlEditorWindow" Title="HTML Editor" Width="950px" EnableStaticBackdrop="true">
    <ContentTemplate>
        <div style="margin-bottom:15px;">
            <a href="#save" data-action="save" class="btn btn-sm btn-success"><i class='fas fa-cloud-upload me-1'></i> Save</a>
            <a href="#close" data-action="close" data-dismiss="modal" class="btn btn-sm btn-default"><i class='fas fa-ban me-1'></i> Close</a>
        </div>

        <div class="file-drop-zone">
            <div class="editor">

            </div>
        </div>

        <div style="margin: 15px 0;">
            Attach files by dragging and dropping or <a href="#fileupload" data-action="fileupload">selecting</a> them.
            <span class="small-print">
                File types supported: .doc .docx .gif .jpg .pdf .png .ppt .xls .xlsx .zip
            </span>
        </div>

        <div class="editor-busy-panel">
            <div>
                <div>
                    <i class="fa fa-spinner fa-pulse fa-3x"></i>
                </div>
            </div>
        </div>
    </ContentTemplate>
</insite:Modal>

<insite:Modal runat="server" ID="TextEditorWindow" Title="Markdown Editor" Width="950px" EnableStaticBackdrop="true">
    <ContentTemplate>
        <div style="margin-bottom:15px;">
            <a href="#save" data-action="save" class="btn btn-sucess"><i class='fas fa-cloud-upload me-2'></i> Save</a>
            <a href="#close" data-action="close" data-dismiss="modal" class="btn btn-default"><i class='fas fa-ban me-2'></i> Close</a>
        </div>

        <div class="file-drop-zone">
            <textarea style="width:100%; height:600px;"></textarea>
        </div>

        <div style="margin: 15px 0;" class="fileupload-message">
            Attach files by dragging and dropping or <a href="#fileupload" data-action="fileupload">selecting</a> them.
            <span class="small-print">
                File types supported: .png .gif .jpg .jpeg .doc .docx .ppt .pptx .xls .xlsx .txt .pdf .zip
            </span>
        </div>

        <div class="editor-busy-panel">
            <div>
                <div>
                    <i class="fa fa-spinner fa-pulse fa-3x"></i>
                </div>
            </div>
        </div>
    </ContentTemplate>
</insite:Modal>

<div class="d-none">
    <asp:FileUpload runat="server" ID="FileUpload" />
    <asp:Button runat="server" ID="UploadFileButton" ValidationGroup="UploadFile" CausesValidation="true" />
    <insite:RequiredValidator runat="server" 
        ControlToValidate="FileUpload"
        FieldName="File"
        ValidationGroup="UploadFile"
        Display="None" />
    <insite:FileExtensionValidator runat="server"
        ControlToValidate="FileUpload"
        FileExtensions="png,gif,jpg,jpeg,doc,docx,ppt,pptx,xls,xlsx,txt,pdf,zip"
        ValidationGroup="UploadFile"
        Display="None" />
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">

        (function () {
            var assetEditor = window.assetEditor = window.assetEditor || {};

            var $currentEditorWindow = null;
            var $currentEditorInput = null;
            var currentEditorCallback = null;

            var $htmlEditor = null;
            var textEditor = null;

            assetEditor.init = function () {
                Sys.Application.add_load(assetEditor.load);

                assetEditor.updateIFrameInput('input[data-iframe]');

                var allowUpload = <%= (!string.IsNullOrEmpty(UploadFolderPath) ? "true" : "false") %>;
                if (!allowUpload) {
                    $('.fileupload-message').remove();
                }

                assetEditor.fileUpload.reset();
            };

            assetEditor.load = function () {
                Sys.Application.remove_load(assetEditor.load);
            };

            assetEditor.updateIFrameInput = function (selector) {
                $(selector).each(function () {
                    var $this = $(this);
                    updateIFrame($this.data('iframe'), $this.val());
                });
            };

            assetEditor.updateIFrameHeight = function (iframe) {
                var $frame = $(iframe);
                if (!$frame.is(':visible'))
                    return;

                var frameHeight = $(iframe.contentWindow.document).find('body > div').outerHeight(true);
                if (frameHeight == null)
                    iframe.contentWindow.document.body.scrollHeight;

                $frame.css('height', frameHeight == 0 ? '' : String(frameHeight + 5) + 'px');
            }
                    
            // Initialize editors

            assetEditor.showHtmlEditor = function ($input, callback) {
                currentEditorCallback = callback;
                $currentEditorInput = $input;
                $currentEditorWindow = $(modalManager.show('<%= HtmlEditorWindow.ClientID %>'));
                $htmlEditor = $currentEditorWindow.find('> .modal-dialog > .modal-content > .modal-body > div > .editor');

                if ($currentEditorWindow.data('inited') !== true) {
                    var prevEditorHeight = null;

                    function onEditorResize() {
                        if ($currentEditorWindow == null && !$currentEditorWindow.is(':visible'))
                            return;

                        var height = $currentEditorWindow.find('> .modal-dialog > .modal-content').height();
                        if (prevEditorHeight === height)
                            return;

                        prevEditorHeight = height;
                        $currentEditorWindow.modal('handleUpdate');
                    }

                    $htmlEditor.summernote({
                        disableDragAndDrop: true,
                        disableResizeEditor: true,
                        dialogsInBody: true,
                        minHeight: 450,
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
                        callbacks: {
                            onDialogShown: function () {
                                $(document).one('hidden.bs.modal', function () {
                                    $('.modal:visible').length && $('body').addClass('modal-open');
                                });
                            },
                            onChange: onEditorResize,
                            onEnter: onEditorResize,
                        },
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

                    $currentEditorWindow.find('[data-action="save"]').on('click', function () {
                        $currentEditorInput.val($htmlEditor.summernote('code'));
                        
                        currentEditorCallback('update');

                        modalManager.close($currentEditorWindow);

                        return false;
                    });

                    $currentEditorWindow.find('[data-action="close"]').on('click', function () {

                        modalManager.close($currentEditorWindow);

                        return false;
                    });

                    $currentEditorWindow.find('[data-action="fileupload"]').on('click', function () {
                        assetEditor.fileUpload.show();
                        return false;
                    });

                    $currentEditorWindow.data('inited', true);
                }

                $currentEditorWindow.one('closed.modal.insite', function () {
                    $htmlEditor.summernote('reset');
                    currentEditorCallback = null;
                    $currentEditorWindow = null;
                    $currentEditorInput = null;
                    $htmlEditor = null;
                });

                $htmlEditor.summernote('code', $currentEditorInput.val());
            };

            assetEditor.showTextEditor = function ($input, callback) {
                currentEditorCallback = callback;
                $currentEditorInput = $input;
                $currentEditorWindow = $(modalManager.show('<%= TextEditorWindow.ClientID %>'));
                var $textInput = $currentEditorWindow.find('> .modal-dialog > .modal-content > .modal-body > div > textarea');

                if ($currentEditorWindow.data('inited') !== true) {
                    var prevEditorHeight = null;

                    textEditor = new SimpleMDE({
                        element: $textInput.get(0),
                        spellChecker: false,
                        autoDownloadFontAwesome: false,
                        parsingConfig: {
                            strikethrough: false
                        },
                        renderingConfig: {
                            singleLineBreaks: false
                        },
                        status: false,
                        toolbar: [
                            'bold',
                            'italic',
                            'heading',
                            '|',
                            'quote',
                            'unordered-list',
                            'ordered-list',
                            '|',
                            'link',
                            'image',
                            '|',
                            'preview',
                            '|',
                            'guide'
                        ],
                    });

                    textEditor.codemirror.setSize('100%', '');
                    textEditor.codemirror.on('viewportChange', function () {
                        if ($currentEditorWindow == null && !$currentEditorWindow.is(':visible'))
                            return;

                        var height = $currentEditorWindow.find('> .modal-dialog > .modal-content').height();
                        if (prevEditorHeight === height)
                            return;

                        prevEditorHeight = height;
                        $currentEditorWindow.modal('handleUpdate');
                    });

                    $currentEditorWindow.find('[data-action="save"]').on('click', function () {
                        $currentEditorInput.val(textEditor.value());

                        currentEditorCallback('update');

                        modalManager.close($currentEditorWindow);

                        return false;
                    });

                    $currentEditorWindow.find('[data-action="close"]').on('click', function () {

                        modalManager.close($currentEditorWindow);

                        return false;
                    });

                    $currentEditorWindow.find('[data-action="fileupload"]').on('click', function () {
                        assetEditor.fileUpload.show();
                        return false;
                    });

                    $currentEditorWindow.data('inited', true);
                }

                $currentEditorWindow.one('closed.modal.insite', function () {
                    currentEditorCallback = null;
                    $currentEditorWindow = null;
                    $currentEditorInput = null;
                    textEditor.value('');
                }).one('shown.bs.modal', function () {
                    textEditor.codemirror.focus();
                    textEditor.codemirror.refresh();
                    textEditor.codemirror.scrollTo(0, 0);
                    $currentEditorWindow.modal('handleUpdate');
                });

                textEditor.value($currentEditorInput.val());
            };

            assetEditor.fileUpload = (function () {
                $('.file-drop-zone').on('drop', onFileDrop);

                return {
                    show: function () {
                        $('#<%= FileUpload.ClientID %>').first().click();
                    },
                    reset: function () {
                        var $input = $('#<%= FileUpload.ClientID %>');
                        $input.replaceWith($input.clone(false).on('change', onFileChanged));
                    },
                };

                // private methods

                function onFileChanged(e) {
                    if (this.value == null || this.value.length == 0)
                        return;

                    var form = $('form#aspnetForm')[0];
                    var $eventTarget = $('#__EVENTTARGET').val('<%= UploadFileButton.UniqueID %>');
                    var formData = new FormData(form);
                    $eventTarget.val('');

                    uploadFile(form, formData);
                }

                function onFileDrop(e) {
                    var dt = e.originalEvent.dataTransfer;
                    if (dt == null || typeof dt == 'undefined')
                        return;

                    var files = dt.files;
                    if (files.length == 0)
                        return;

                    e.preventDefault();
                    e.stopPropagation();

                    var $form = $('form');

                    var formData = new FormData();
                    $form.find('input').each(function () {
                        if (this.name === '<%= FileUpload.UniqueID %>') {
                            formData.append(this.name, files[0]);
                        } else if (this.name === '__EVENTTARGET') {
                            formData.append(this.name, '<%= UploadFileButton.UniqueID %>');
                        } else if (this.type == 'submit' || this.type == 'button') {
                            if (this.name == '<%= UploadFileButton.UniqueID %>') {
                                formData.append(this.name, this.value);
                            }
                        } else {
                            formData.append(this.name, this.value);
                        }
                    });

                    uploadFile($form.get(0), formData);
                }

                function uploadFile(form, formData) {
                    var $busyPanel = $currentEditorWindow.find('.editor-busy-panel').show();

                    $.ajax({
                        url: form.action,
                        type: form.method,
                        data: formData,
                        dataType: 'json',
                        cache: false,
                        contentType: false,
                        processData: false,

                        complete: function () {
                            $busyPanel.hide();
                        },
                        success: function (data) {
                            var msg = '';

                            if (data.Type == 'OK') {
                                if ($currentEditorWindow == null)
                                    return;

                                if ($htmlEditor == null)
                                    onContentTextFileUploaded(data.Path, data.Name, data.IsImage);
                                else
                                    onContentHtmlFileUploaded(data.Path, data.Name, data.IsImage);
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

                function onContentTextFileUploaded(folder, filename, isImage) {
                    var selection = textEditor.codemirror.doc.getSelection(' ');
                    var hasSelection = selection.length > 0;
                    var text = hasSelection ? selection : filename;

                    var link = '[' + text + '](' + folder + '/' + filename + ')';
                    if (isImage)
                        link = '![' + text + '](' + folder + '/' + filename + ')';

                    if (hasSelection) {
                        textEditor.codemirror.doc.replaceSelection(link);
                    } else {
                        var cursor = textEditor.codemirror.doc.getCursor();
                        textEditor.codemirror.doc.replaceRange(link, cursor, cursor);
                    }

                    var cursor = textEditor.codemirror.doc.getCursor();
                    textEditor.codemirror.doc.setSelection({
                        line: cursor.line,
                        ch: cursor.ch - link.length
                    }, {
                        line: cursor.line,
                        ch: cursor.ch
                    });

                    textEditor.codemirror.focus();
                }

                function onContentHtmlFileUploaded(folder, name, isImage) {
                    if ($htmlEditor == null || typeof $htmlEditor.summernote === 'undefined')
                        return;

                    var url = folder + '/' + name;
                    if (isImage)
                        $htmlEditor.summernote('insertImage', url);
                    else
                        $htmlEditor.summernote('createLink', { text: name, url: url, newWindow: false });
                }
            })();

            // iframe content

            function updateIFrame(selector, html) {
                var $iframe = $(selector);
                if ($iframe.length != 1)
                    return;

                if (html == null || typeof html != 'string' || html.length == 0) {
                    $iframe.css('display', 'none');
                    return;
                }

                $iframe.css('display', '');

                var iframe = $iframe.get(0);
                iframe.contentWindow.document.open();
                iframe.contentWindow.document.write('<html>');
                iframe.contentWindow.document.write('<head>');
                iframe.contentWindow.document.write('  <link href="/library/fonts/source-sans-pro/400italic-700italic-400-700-300_subset--latin__latin-ext.css" rel="stylesheet" type="text/css">');
                iframe.contentWindow.document.write('  <style type="text/css">');
                iframe.contentWindow.document.write('    body { color: #545454; font-family: Calibri,"Source Sans Pro",Helvetica,Arial; font-size: 16px; overflow:hidden; }');
                iframe.contentWindow.document.write('    body > div { margin-bottom: 16px; }');
                iframe.contentWindow.document.write('    img { max-width: 100%; }');
                iframe.contentWindow.document.write('  </style>');
                iframe.contentWindow.document.write('</head>');
                iframe.contentWindow.document.write('<body>');
                iframe.contentWindow.document.write('<div>');
                iframe.contentWindow.document.write(html);
                iframe.contentWindow.document.write('</div>');
                iframe.contentWindow.document.write('</body>');
                iframe.contentWindow.document.write('</html>');
                iframe.contentWindow.document.close();

                assetEditor.updateIFrameHeight(iframe);

                setTimeout(function (iframe) {
                    assetEditor.updateIFrameHeight(iframe);
                }, 1000, iframe);
            }
        })();

        assetEditor.init();

    </script>
</insite:PageFooterContent>

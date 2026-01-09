<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Field.ascx.cs" Inherits="InSite.UI.Layout.Common.Controls.Editor.Field" %>

<div class="form-group mb-3 content-field" id="<%= ClientID %>">
    <div class="commands">
        <span runat="server" id="LanguageOutput" class="label lang-out"></span>
        <insite:Button runat="server" ID="RequestTranslationButton" ButtonStyle="Primary" Size="ExtraSmall" ToolTip="Request Google Translation" Icon="fas fa-globe" />
    </div>
    <label class="form-label">
        <strong><%= string.IsNullOrEmpty(Title) ? "&nbsp;" : HttpUtility.HtmlEncode(Title) %></strong>
        <%= string.IsNullOrEmpty(ToolTip) ? "&nbsp;" : "<div class='form-text mt-3 mb-3'>" + HttpUtility.HtmlEncode(ToolTip) + "</div>" %>
        <insite:CustomValidator runat="server" ID="ClientStateRequiredValidator" Enabled="false" Display="Dynamic" />
    </label>
    <div>
        <insite:TextBox runat="server" ID="TranslationText" Width="100%" />
        <asp:HiddenField runat="server" ID="StateInput" />
        <div class="d-none">
            <asp:FileUpload runat="server" ID="FileUpload" />
            <asp:Button runat="server" ID="UploadFileButton" CausesValidation="true" CssClass="btn-upload" />
            <insite:RequiredValidator runat="server" ID="FileUploadRequiredValidator" ControlToValidate="FileUpload"
                FieldName="File" Display="None" />
            <insite:FileExtensionValidator runat="server" ID="FileUploadExtensionValidator" ControlToValidate="FileUpload"
                FileExtensions="png,gif,jpg,jpeg,doc,docx,ppt,pptx,xls,xlsx,txt,pdf,zip" Display="None" />
            <insite:CustomValidator runat="server" ID="FileUploadImageSizeValidator" ControlToValidate="FileUpload"
                ClientValidationFunction="contentEditorField.validateImageFileSize" />
            <insite:CustomValidator runat="server" ID="FileUploadDocumentSizeValidator" ControlToValidate="FileUpload"
                ClientValidationFunction="contentEditorField.validateDocumentFileSize" />
        </div>
    </div>
    <div class="form-text"><%= Description %></div>
</div>

<insite:PageHeadContent runat="server" ID="CommonStyle">
    <style type="text/css">
        body.drag-active .content-field-dropmessage {
            display: block !important;
        }

        .content-field-dropmessage,
        .content-field-loadingmessage {
            z-index: 501;
        }

        .content-field-dropmessage > div > div {
            font-size: 1.1em;
        }

        .content-field-dropmessage.drag-over > div > div {
            font-weight: bold;
        }

        .content-field > .commands > span.lang-out {
            text-transform: uppercase;
        }

        .content-field table.translation-list > tr > td.lang-name {
            font-weight: bold;
            width: 40px;
            padding: 5px;
            vertical-align: top;
        }

        .content-field table.translation-list > tr > td.lang-name a {
            text-transform: uppercase;
        }

        .content-field table.translation-list > tr > td.lang-value {
            padding: 5px;
            word-break: break-word;
        }
    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server" ID="CommonScript">
    <script type="text/javascript">
        (function () {
            var $currentDragTarget = null;
            var isDraggingFile = false;

            $(document)
                .on('dragenter dragstart dragend dragleave dragover drag drop', false)
                .on('dragenter', function (e) {
                    $currentDragTarget = $(e.target);

                    var dt = e.originalEvent.dataTransfer;
                    isDraggingFile = dt.types != null && (dt.types.indexOf && dt.types.indexOf('Files') != -1 || dt.types.contains('application/x-moz-file'));

                    var $body = $('body').addClass('drag-active');
                    if (isDraggingFile)
                        $body.addClass('drag-file');
                }).on('drop dragleave', function (e) {
                    if ($currentDragTarget !== null && $currentDragTarget.is(e.target) && (e.relatedTarget === null || e.relatedTarget.nodeType !== 3 && e.relatedTarget !== e.target)) {
                        $currentDragTarget = null;

                        var $body = $('body').removeClass('drag-active');
                        if (isDraggingFile)
                            $body.removeClass('drag-file');
                    }
                });
        })();

        (function () {
            // public methods

            var instance = window.contentEditorField = window.contentEditorField || {};

            function ControlState($langOutput, $textInput, $stateInput, type) {
                var obj = this;
                var ensureInited = null;
                var setValue = null;
                var getValue = null;

                if (type === 'md') {
                    ensureInited = function () {
                        if ($textInput.data('inited') === true || !$textInput.is(':visible'))
                            return;

                        var editor = new SimpleMDE({
                            element: $textInput[0],
                            forceSync: true,
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

                        editor.codemirror.on('change', onTextChanged);

                        $textInput.data('simple-mde', editor);

                        editor.codemirror.refresh();

                        $textInput.data('inited', true);
                    };

                    setValue = function (value) {
                        if ($textInput.data('inited') === true)
                            $textInput.data('simple-mde').value(value);
                        else
                            $textInput.val(value);
                    };
                    getValue = function () {
                        if ($textInput.data('inited') === true)
                            return $textInput.data('simple-mde').value();
                        else
                            return $textInput.val();
                    };

                    obj.insertFileLink = function (folder, filename, isImage) {
                        var editor = $textInput.data('simple-mde');
                        var selection = editor.codemirror.doc.getSelection(' ');
                        var hasSelection = selection.length > 0;
                        var text = hasSelection ? selection : filename;

                        var link = '[' + text + '](' + folder + '/' + filename + ')';
                        if (isImage)
                            link = '![' + text + '](' + folder + '/' + filename + ')';

                        if (hasSelection) {
                            editor.codemirror.doc.replaceSelection(link);
                        } else {
                            var cursor = editor.codemirror.doc.getCursor();
                            editor.codemirror.doc.replaceRange(link, cursor, cursor);
                        }

                        var cursor = editor.codemirror.doc.getCursor();
                        editor.codemirror.doc.setSelection({
                            line: cursor.line,
                            ch: cursor.ch - link.length
                        }, {
                            line: cursor.line,
                            ch: cursor.ch
                        });

                        editor.codemirror.focus();
                    };
                } else if (type === 'html') {
                    ensureInited = function () {
                        if ($textInput.data('inited') === true || !$textInput.is(':visible'))
                            return;

                        $textInput.summernote({
                            focus: true,
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
                            callbacks: {
                                onChange: onTextChanged,
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

                        $textInput.data('inited', true);
                    };

                    setValue = function (value) {
                        if ($textInput.data('inited') === true)
                            $textInput.summernote('code', value);
                        else
                            $textInput.val(value);
                    };
                    getValue = function () {
                        if ($textInput.data('inited') === true)
                            return $textInput.summernote('code');
                        else
                            return $textInput.val();
                    };

                    obj.insertFileLink = function (folder, name, isImage) {
                        var url = folder + '/' + name;

                        $textInput.summernote('focus');

                        if (isImage)
                            $textInput.summernote('insertImage', url);
                        else
                            $textInput.summernote('createLink', { text: name, url: url, newWindow: false });
                    };
                } else {
                    $textInput.on('change', onTextChanged);

                    setValue = function (value) { $textInput.val(value); };
                    getValue = function () { return $textInput.val(); };

                    obj.insertFileLink = function () { alert('NOT IMPLEMENTED'); };
                }

                if (ensureInited !== null) {
                    ensureInited();

                    $textInput.parents('.tab-pane').each(function () {
                        $('[data-bs-target="#' + this.id + '"][data-bs-toggle]').on('shown.bs.tab', function () {
                            ensureInited();
                        });
                    });
                }

                obj.update = function (json) {
                    $stateInput.val(json);

                    obj.refresh();
                };

                obj.refresh = function () {
                    var text = instance.bind($textInput.data('simple-mde'), $stateInput, $langOutput, function (lang) {
                        contentEditorField.setLanguage($stateInput, lang);

                        obj.refresh();
                    });

                    setValue(text);
                }

                function onTextChanged() {
                    contentEditorField.setText($stateInput, getValue());
                }
            }

            instance.getState = function ($input) {
                var state = $input.val();

                if (state) {
                    try {
                        return JSON.parse(state);
                    } catch(e) {
                        return null;
                    }
                }

                return null;
            };

            instance.bind = function (simplemde, $text, $lang, onSelect) {
                var state = instance.getState($text);
                if (!state || !state.data)
                    return;

                var lang = state.lang;
                if (!lang)
                    lang = 'en';
                else
                    lang = lang.toLowerCase();

                $lang.text(lang).trigger('updated.content-field', [lang]);

                var result = '';
                var hasData = false;

                var $table = $('<table style="margin-top:5px;" class="translation-list">');

                var data = state.data;
                for (var key in data) {
                    if (!data.hasOwnProperty(key))
                        continue;

                    hasData = true;

                    var name = key.toLowerCase();
                    if (name !== lang) {
                        var html = simplemde != null ? simplemde.options.previewRender(data[key]) : data[key];

                        $table.append(
                            $('<tr>').append(
                                $('<td class="lang-name">').append(
                                    $('<a href="#">').data('key', key).on('click', function (e) {
                                        e.preventDefault();

                                        var key = $(this).data('key');
                                        if (!key)
                                            return;

                                        onSelect(key.toLowerCase());
                                    }).text(name)
                                ),
                                $('<td class="lang-value">').html(html)
                            )
                        );
                    } else {
                        result = data[key];
                    }
                }

                $text.next('table').remove();

                if (hasData)
                    $text.after($table);

                return result;
            };

            instance.setText = function ($input, value) {
                var state = instance.getState($input);
                if (!state)
                    return;

                if (!state.data)
                    state.data = {};

                if (value)
                    value = value.trim();

                state.data[state.lang.toLowerCase()] = value;

                var json = JSON.stringify(state);

                $input.val(json);
            };

            instance.setLanguage = function ($input, value) {
                var state = instance.getState($input);
                if (!state)
                    return;

                if (value)
                    value = value.trim();

                state.lang = value;

                var json = JSON.stringify(state);

                $input.val(json);
            };

            instance.init = function (options) {
                var $stateInput = $('#' + options.stateId);
                if ($stateInput.length !== 1)
                    return;

                var isRefresh = false;

                var ctrlState = $stateInput.data('state');
                if (!ctrlState) {
                    var $lang = $('#' + options.langId);
                    var $text = $('#' + options.textId);

                    ctrlState = new ControlState($lang, $text, $stateInput, options.type);
                    $stateInput.data('state', ctrlState);
                    isRefresh = true;

                    if (options.upload === true)
                        initFileUpload($text, $stateInput);
                }

                if (options.state) {
                    ctrlState.update(options.state);
                    isRefresh = false;
                }

                if (isRefresh)
                    ctrlState.refresh();
            };
            
            instance.validateImageFileSize = function (s, e) {
                if (!e.Value)
                    return;

                var files = $('#' + $(s).data('val-controltovalidate')).prop('files');
                if (!files || files.length === 0)
                    return;

                var dotIndex = e.Value.lastIndexOf('.');
                if (dotIndex < 1)
                    return;

                var ext = e.Value.substring(dotIndex).toLowerCase();
                if (ext !== '.png' && ext !== '.gif' && ext !== '.jpg' && ext !== '.jpeg')
                    return;

                e.IsValid = files[0].size > 0 && files[0].size <= <%= Organization.PlatformCustomization.UploadSettings.Images.MaximumFileSize %>;
            };

            instance.validateDocumentFileSize = function (s, e) {
                if (!e.Value)
                    return;

                var files = $('#' + $(s).data('val-controltovalidate')).prop('files');
                if (!files || files.length === 0)
                    return;

                var dotIndex = e.Value.lastIndexOf('.');
                if (dotIndex < 1)
                    return;

                var ext = e.Value.substring(dotIndex).toLowerCase();
                if (ext === '.png' || ext === '.gif' || ext === '.jpg' || ext === '.jpeg')
                    return;

                e.IsValid = files[0].size > 0 && files[0].size <= <%= Organization.PlatformCustomization.UploadSettings.Documents.MaximumFileSize %>;
            };

            // initialization

            // private methods

            function initFileUpload($text, $state) {
                var $parent = $text.closest('div');
                var $formGroup = $parent.closest('.form-group').on('drop', onFileDrop);
                var $uploadButton = $formGroup.find('.btn-upload');
                var $upload = $formGroup.find('input[type="file"]').on('change', onUploadFileChanged);
                var $dropMessage = $('<div class="loading-panel content-field-dropmessage"><div><div>Drop File Here</div></div></div>')
                    .on('dragenter', onDropMessageDragEnter).on('dragleave', onDropMessageDragLeave);
                var $loadingMessage = $('<div class="loading-panel content-field-loadingmessage"><div><div><i class="fa fa-spinner fa-pulse fa-3x"></i></div></div></div>');

                $upload.data('refs', { $button: $uploadButton, $loading: $loadingMessage, $state: $state });
                $formGroup.data('refs', { $upload: $upload, $button: $uploadButton, $dropMessage: $dropMessage, $loading: $loadingMessage, $state: $state }).append($dropMessage).append($loadingMessage);

                $parent.append(
                    $('<div class="form-text" style="margin: 15px 0;">').append(
                        document.createTextNode('Attach files by dragging and dropping or '),
                        $('<a href="#fileupload">').text('selecting').on('click', onFileUploadClick).data('refs', { $upload: $upload }),
                        document.createTextNode(' them. File types supported: .png .gif .jpg .jpeg .doc .docx .ppt .pptx .xls .xlsx .txt .pdf .zip'),
                    )
                );
            }

            function uploadFile(form, formData, $loading, state) {
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
                        var messages = data.Messages;

                        if (data.Type == 'OK') {
                            state.insertFileLink(data.Path, data.Name, data.IsImage);
                        } else if (data.Type == 'ERROR') {
                            if (messages.length == 0)
                                messages = ['An error occurred during uploading the file.'];
                        } else {
                            messages = ['Unexpected response type: ' + String(data.Type)];
                        }

                        if (messages && messages.length > 0) {
                            var text = '';

                            for (var i = 0; i < messages.length; i++) {
                                if (i !== 0)
                                    text += '\r\n\r\n';

                                text += data.Messages[i];
                            }

                            alert(text);
                        }
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

                $(this).data('refs').$upload.click();
            }

            function onUploadFileChanged(e) {
                if (this.value == null || this.value.length == 0)
                    return;

                var refs = $(this).data('refs');
                var form = $('form#aspnetForm')[0];
                var $eventTarget = $('#__EVENTTARGET').val(refs.$button.prop('name'));
                var formData = new FormData(form);
                $eventTarget.val('');

                uploadFile(form, formData, refs.$loading, refs.$state.data('state'));
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

                uploadFile($form.get(0), formData, refs.$loading, refs.$state.data('state'));
            }
        })();

    </script>
</insite:PageFooterContent>

(function () {
    const $fileForm = $('<form enctype="multipart/form-data" class="d-none">')
        .appendTo('body');
    const $fileInput = $('<input name="file" multiple="" type="file" />')
        .appendTo($fileForm)
        .on('change', onUploadFileChanged);
    const settings = {
        initElement: window.attempts.initElement
    };

    let $currentContainer = null;
    let $currentDragTarget = null;
    let isDraggingFile = false;

    $(document)
        .on('dragenter dragstart dragend dragleave dragover drag drop', false)
        .on('dragenter', function (e) {
            $currentDragTarget = $(e.target);

            const dt = e.originalEvent.dataTransfer;
            isDraggingFile = dt.types != null && (dt.types.indexOf && dt.types.indexOf('Files') != -1 || dt.types.contains('application/x-moz-file'));

            const $body = $('body').addClass('drag-active');
            if (isDraggingFile)
                $body.addClass('drag-file');
        }).on('drop dragleave', function (e) {
            if ($currentDragTarget !== null && $currentDragTarget.is(e.target) && (e.relatedTarget === null || e.relatedTarget.nodeType !== 3 && e.relatedTarget !== e.target)) {
                $currentDragTarget = null;

                const $body = $('body').removeClass('drag-active');
                if (isDraggingFile)
                    $body.removeClass('drag-file');
            }
        });

    $(window).on('attempts:init', function () {
        $('.card-body > .form-group.composed-essay-input > textarea').each(function () {
            if (this.name)
                settings.initElement(this, 'answer:markdown', function (el) {
                    inSite.common.baseEditor.init(init, {
                        id: el.name,
                        textarea: el
                    });
                });
        });
    });

    function init(options) {
        const $textarea = $(options.textarea);
        if ($textarea.length != 1 || !!$textarea.data('simple-mde'))
            return true;

        if (!$textarea.is(':visible'))
            return false;

        const mdeOptions = {
            element: options.textarea,
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
                'ordered-list'
            ]
        };

        const $parent = $textarea.closest('div.form-group');
        const parentRefs = {
            $textArea: $textarea
        };

        if (typeof _isAnswerKioskMode !== 'boolean' || _isAnswerKioskMode !== true) {
            $parent.on('drop', onFileDrop);
            const $loadingMessage = $('<div class="loading-panel upload-loadingmessage"><div><div><i class="fa fa-spinner fa-pulse fa-3x"></i></div></div></div>');
            const $dropMessage = $('<div class="loading-panel upload-dropmessage"><div><div>Drop File Here</div></div></div>')
                .on('dragenter', onDropMessageDragEnter).on('dragleave', onDropMessageDragLeave);

            $textarea.after($dropMessage).after($loadingMessage).after(
                $('<small class="text-body-secondary d-block mt-3 mb-3">').append(
                    document.createTextNode('Attach files by dragging and dropping or '),
                    $('<a href="#fileupload">').text('selecting').on('click', function onFileUploadClick(e) {
                        e.preventDefault();

                        $currentContainer = $parent;
                        $fileInput.click();
                    }),
                    document.createTextNode(' them.'), '<br/>',
                    document.createTextNode('File types supported: .png .gif .jpg .jpeg .doc .docx .ppt .pptx .xls .xlsx .txt .pdf .zip')
                )
            );

            mdeOptions.toolbar = [
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
            ];
            parentRefs.$dropMessage = $dropMessage;
            parentRefs.$loading = $loadingMessage;
        }

        const editor = new SimpleMDE(mdeOptions);

        $(editor.codemirror.display.input.textarea).addClass('disable-validation');

        editor.codemirror.on('change', function () {
            $textarea.trigger('change');
        });

        $textarea.data('simple-mde', editor);

        $parent.data('refs', parentRefs);

        return true;
    }

    function uploadFile(formData, $loading, $textArea) {
        formData.append('action', 'upload');
        formData.append('session', _sessionId);

        $loading.show();

        $.ajax({
            type: 'POST',
            data: formData,
            dataType: 'json',

            cache: false,
            contentType: false,
            processData: false,

            complete: function () {
                $loading.hide();
            },
            success: function (data) {
                if (data.type !== 'UPLOADED') {
                    helper.showStatus('danger', '', 'An error occurred during uploading the file.');
                } else if (data.errors && data.errors.length > 0) {
                    let msg = '';

                    for (let i = 0; i < data.errors.length; i++) {
                        if (msg.length > 0)
                            msg += '\r\n\r\n';

                        msg += data.errors[i];
                    }

                    helper.showStatus('danger', '', msg);
                } else if (data.links && data.links.length > 0) {
                    const editor = $textArea.data('simple-mde');

                    let text = '';
                    for (let i = 0; i < data.links.length; i++) {
                        const link = data.links[i];
                        if (typeof link !== 'string' || link.length === 0)
                            continue;

                        if (text.length > 0)
                            text += '\r\n';

                        text += link;
                    }

                    if (text.length > 0) {
                        const selection = editor.codemirror.doc.getSelection(' ');
                        const hasSelection = selection.length > 0;

                        if (hasSelection) {
                            editor.codemirror.doc.replaceSelection(text);
                        } else {
                            const cursor = editor.codemirror.doc.getCursor();
                            editor.codemirror.doc.replaceRange(text, cursor, cursor);
                        }

                        const cursor = editor.codemirror.doc.getCursor();
                        editor.codemirror.doc.setSelection({
                            line: cursor.line,
                            ch: cursor.ch - text.length
                        }, {
                            line: cursor.line,
                            ch: cursor.ch
                        });

                        editor.codemirror.focus();
                    }
                }
            },
            error: function () {
                helper.showStatus('danger', '', 'An error occurred during uploading the file.');
            }
        });
    }

    // event handlers

    function onDropMessageDragEnter(e) {
        $(this).addClass('drag-over');
    }

    function onDropMessageDragLeave(e) {
        const $this = $(this);
        if (e.relatedTarget === null || $this.has(e.relatedTarget).length <= 0 || e.relatedTarget.nodeType !== 3 && e.relatedTarget !== e.target)
            $this.removeClass('drag-over');
    }

    function onFileDrop(e) {
        const refs = $(this).data('refs');
        refs.$dropMessage.removeClass('drag-over');

        const dt = e.originalEvent.dataTransfer;
        if (dt === null || typeof dt === 'undefined')
            return;

        const files = dt.files;
        if (files.length == 0)
            return;

        e.preventDefault();

        const formData = new FormData();
        formData.append('file', files[0]);

        uploadFile(formData, refs.$loading, refs.$textArea);
    }

    function onUploadFileChanged(e) {
        if (this.value == null || this.value.length == 0)
            return;

        if ($currentContainer === null)
            return;

        const refs = $currentContainer.data('refs');
        const form = $(this).closest('form').get(0);
        const formData = new FormData(form);

        uploadFile(formData, refs.$loading, refs.$textArea);
    }
})();
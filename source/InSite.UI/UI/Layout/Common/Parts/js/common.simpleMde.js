(function () {
    var instance = inSite.common.simpleMde = inSite.common.simpleMde || {};

    instance.init = function (options) {
        if (!options.$textInput || !options.$textInput.length)
            return;

        var hasFileUpload = !!options.$uploadInput && !!options.$uploadTrigger;
        if (hasFileUpload)
            options.$uploadInput
                .off('change', onUploadFileChanged)
                .on('change', onUploadFileChanged);

        options.$textInput.each(function () {
            var refsData = {
                $textArea: $(this)
            };

            if (!!refsData.$textArea.data('simple-mde'))
                return;

            if (hasFileUpload) {
                var $parent = refsData.$textArea.closest('div').on('drop', onFileDrop);

                refsData.$uploadInput = options.$uploadInput;
                refsData.$uploadTrigger = options.$uploadTrigger;
                refsData.$loading = $('<div class="loading-panel upload-loadingmessage"><div><div><i class="fa fa-spinner fa-pulse fa-3x"></i></div></div></div>');
                refsData.$dropMessage = $('<div class="loading-panel upload-dropmessage"><div><div>Drop File Here</div></div></div>')
                    .on('dragenter', onDropMessageDragEnter).on('dragleave', onDropMessageDragLeave);
                refsData.$textArea.after(refsData.$dropMessage).after(refsData.$loading).after(
                    refsData.$attachBlock = $('<div class="form-text my-3">').append(
                        document.createTextNode('Attach files by dragging and dropping or '),
                        $('<a href="#fileupload">').text('selecting').on('click', onFileUploadClick).data('refs', refsData),
                        document.createTextNode(' them.'), '<br/>',
                        (!options.supportedFileTypes ? null : document.createTextNode('File types supported: ' + String(options.supportedFileTypes)))
                    )
                );
            }

            var mdeOptions = {
                element: this,
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
            };

            if (options.mdeOptions)
                mdeOptions = $.extend(mdeOptions, options.mdeOptions);

            var editor = new SimpleMDE(mdeOptions);

            refsData.$textArea.data('simple-mde', editor);
            $parent.data('refs', refsData);

            $(editor.codemirror.display.wrapper).parents('.tab-pane').each(function () {
                $('[data-bs-target="#' + this.id + '"][data-bs-toggle]').on('shown.bs.tab', function () {
                    refreshEditor();
                });
            });

            if (options.onTextChange)
                editor.codemirror.on("change", options.onTextChange);

            function refreshEditor() {
                if ($(editor.codemirror.display.wrapper).is(':visible'))
                    editor.codemirror.refresh();
            }
        });
    };

    //instance.removeSimpleMDE = function ($textInput) {
    //    if (!$textInput || !$textInput.length)
    //        return;

    //    options.$textInput.each(function () {
    //        var $this = $(this);
    //        var $parent = $(this).closest('div');

    //        var refsData = $parent.data('refs');
    //        if (!refsData)
    //            return;

    //        if (options.$uploadInput) {
    //            options.$uploadInput.off('change', onUploadFileChanged);
    //            refsData.$loading.remove();
    //            refsData.$dropMessage.remove();
    //            refsData.$attachBlock.remove();
    //        }

    //        var editor = refsData.$textArea.data('simple-mde');
    //        editor.toTextArea();
    //        refsData.$textArea.data('simple-mde', null);

    //        $parent.off('drop', onFileDrop).data('refs', null);

    //        refsData.$textArea.data('simple-mde', editor);
    //        $parent.data('refs', refsData);

    //        $(editor.codemirror.display.wrapper).parents('.panel-group').on('shown.bs.collapse', function () {
    //            refreshEditor();
    //        });

    //        $(editor.codemirror.display.wrapper).parents('.tab-pane').each(function () {
    //            $('a[href="#' + this.id + '"]').on('shown.bs.tab', function () {
    //                refreshEditor();
    //            });
    //        });

    //        function refreshEditor() {
    //            if ($(editor.codemirror.display.wrapper).is(':visible'))
    //                editor.codemirror.refresh();
    //        }
    //    });
    //};

    // methods

    function uploadFile(form, formData, $loading, $textArea) {
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
                if (data.errors && data.errors.length > 0) {
                    var msg = '';

                    for (var i = 0; i < data.errors.length; i++) {
                        if (msg.length > 0)
                            msg += '\r\n\r\n';

                        msg += data.errors[i];
                    }

                    alert(msg);
                } else if (data.links && data.links.length > 0) {
                    var editor = $textArea.data('simple-mde');

                    var text = '';
                    for (var i = 0; i < data.links.length; i++) {
                        var link = data.links[i];
                        if (typeof link !== 'string' || link.length === 0)
                            continue;

                        if (text.length > 0)
                            text += '\r\n';

                        text += link;
                    }

                    if (text.length > 0) {
                        var selection = editor.codemirror.doc.getSelection(' ');
                        var hasSelection = selection.length > 0;

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
                    }
                } else {
                    alert('An error occurred during uploading the file.');
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

        var refs = $(this).data('refs');

        refs.$uploadInput.data('refs', refs).click();
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
            if (this.name === refs.$uploadInput.attr('name')) {
                formData.append(this.name, files[0]);
            } else if (this.name === '__EVENTTARGET') {
                formData.append(this.name, refs.$uploadTrigger.attr('name'));
            } else if (this.type == 'submit' || this.type == 'button') {
                if (this.name == refs.$uploadTrigger.attr('name'))
                    formData.append(this.name, this.value);
            } else {
                formData.append(this.name, this.value);
            }
        });

        uploadFile($form.get(0), formData, refs.$loading, refs.$textArea);
    }

    function onUploadFileChanged(e) {
        if (this.value == null || this.value.length == 0)
            return;

        var refs = $(this).data('refs');
        var form = $('form#aspnetForm')[0];
        var $eventTarget = $('#__EVENTTARGET').val(refs.$uploadTrigger.attr('name'));
        var formData = new FormData(form);
        $eventTarget.val('');

        uploadFile(form, formData, refs.$loading, refs.$textArea);
    }

})();
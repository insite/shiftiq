(function () {
    var instance; {
        var inSite = window.inSite = window.inSite || {};
        var common = inSite.common = inSite.common || {};
        instance = common.editorUpload = common.editorUpload || {};
    }

    var isDragFileDetectionEnabled = false;

    // public methods

    instance.enableDragFileDetection = enableDragFileDetection;

    instance.init = function (options) {
        if (!options || typeof options.id !== 'string' || options.id.length == 0)
            return;

        var $control = $('#' + options.id);
        if ($control.length != 1 || $control.data('inited') == true)
            return;

        var $uploadButton = $control.find('input[type="button"]').first();
        var $upload = $control.find('input[type="file"]');
        var $dropMessage = $('<div class="loading-panel upload-dropmessage"><div><div>Drop File Here</div></div></div>');
        var $loadingMessage = $('<div class="loading-panel upload-loadingmessage"><div><div><i class="fa fa-spinner fa-pulse fa-3x"></i></div></div></div>');

        var $dropContainer = $control;
        if (options.dCont) {
            var $el = $(document.getElementById(options.dCont));

            if ($el.length != 1)
                $el = $control.parents(options.dCont);

            if ($el.length != 1) {
                var obj = inSite.common.getObjByName(options.dCont);
                if (obj && typeof obj == 'function') {
                    $el = obj();
                    if ($el instanceof HTMLElement)
                        $el = $($el);
                    else if (!($el instanceof jQuery))
                        $el = null;
                }
            }

            if ($el.length == 1)
                $dropContainer = $el;
        }

        var refs = {
            $upload: $upload,
            $button: $uploadButton,
            $dropMessage: $dropMessage,
            $loading: $loadingMessage
        };

        $dropContainer
            .on('drop', function (e) { onFileDrop.call(this, e, refs); })
            .append($dropMessage)
            .append($loadingMessage);
        $upload
            .on('change', function (e) { onUploadFileChanged.call(this, e, refs); })
            .on('cancel', function (e) { e.stopPropagation(); });
        $dropMessage.on('dragenter', onDropMessageDragEnter).on('dragleave', onDropMessageDragLeave);

        {
            var $attachLink = instance.createContainer().append(
                document.createTextNode('Attach files by dragging and dropping or '),
                $('<a href="#fileupload">')
                    .text('selecting')
                    .on('click', function (e) { onFileUploadClick.call(this, e, refs); }),
                document.createTextNode(' them.')
            );

            if (options.extensions)
                $attachLink.append(document.createTextNode(' File types supported: ' + options.extensions));

            $control.append($attachLink);
        }

        $control.data('inited', true);

        enableDragFileDetection();
    };

    instance.createContainer = function () {
        return $('<div class="form-text my-3">');
    };

    // methods

    function uploadFile(form, formData, $loading) {
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
                if (typeof data != 'object' || !data.func)
                    return;

                var callback = inSite.common.getObjByName(String(data.func));
                if (typeof callback == 'function')
                    callback(data.args);
            },
            error: function (e) {
                var messages;
                if (!!e.responseJSON && e.responseJSON instanceof Array)
                    messages = e.responseJSON;
                else
                    messages = [];

                var text = '';

                for (var i = 0; i < messages.length; i++) {
                    var m = messages[i];
                    if (typeof m != 'string' || m.length == 0)
                        continue;

                    if (text.length > 0)
                        text += '\r\n\r\n';

                    text += m;
                }

                if (text.length == 0)
                    text = 'An error occurred during uploading the file.';

                alert(text);
            }
        });
    }

    function enableDragFileDetection() {
        if (isDragFileDetectionEnabled)
            return;

        var $currentDragTarget = null;
        var isDraggingFile = false;

        $(document)
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
            })
            .on('dragenter dragstart dragend dragleave dragover drag drop', false);

        isDragFileDetectionEnabled = true;
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

    function onFileUploadClick(e, refs) {
        e.preventDefault();

        if (typeof refs == 'undefined')
            return;

        refs.$upload.click();
    }

    function onUploadFileChanged(e, refs) {
        if (typeof refs == 'undefined' || this.value == null || this.value.length == 0)
            return;

        var $form = $('form#aspnetForm');

        var innerFormMapping = [];

        $form.find('form').each(function () {
            var item = {
                $form: $(this),
                $div: $('<div>')
            };

            item.$div.insertAfter(item.$form);
            item.$div.append(item.$form.children().detach());
            item.$form.detach();

            innerFormMapping.push(item);
        });

        var form = $form[0];
        var $eventTarget = $('#__EVENTTARGET').val(refs.$button.prop('name'));
        var formData = new FormData(form);
        $eventTarget.val('');

        uploadFile(form, formData, refs.$loading);

        for (var i = 0; i < innerFormMapping.length; i++) {
            var item = innerFormMapping[i];
            item.$form.insertAfter(item.$div);
            item.$form.append(item.$div.children().detach());
            item.$div.detach();
        }
    }

    function onFileDrop(e, refs) {
        if (typeof refs == 'undefined')
            return;

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

        uploadFile($form.get(0), formData, refs.$loading);
    }
})();
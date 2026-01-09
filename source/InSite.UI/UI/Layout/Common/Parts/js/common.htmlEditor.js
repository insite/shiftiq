(function () {
    var instance; {
        var inSite = window.inSite = window.inSite || {};
        var common = inSite.common = inSite.common || {};
        instance = common.htmlEditor = common.htmlEditor || {};
    }

    var defaultOptions = {
        id: '',
    };

    instance.init = function (options) {
        options = $.extend({}, defaultOptions, options);

        if (!options.id)
            return;

        inSite.common.baseEditor.init(init, options);
    };

    instance.onFileUploaded = function (options) {
        var $text = $(document.getElementById(options.textId));
        if ($text.length != 1)
            return;

        {
            $text.summernote('focus');

            if (options.img)
                $text.summernote('insertImage', options.url);
            else
                $text.summernote('createLink', { text: options.title, url: options.url, newWindow: false });
        }

        if (options.msgs && options.msgs.length > 0) {
            var text = '';

            for (var i = 0; i < options.msgs.length; i++) {
                if (i !== 0)
                    text += '\r\n\r\n';

                text += options.msgs[i];
            }

            alert(text);
        }
    };

    function init(options) {
        var textEl = document.getElementById(options.id);
        if (!textEl)
            return true;

        var $text = $(textEl);
        if (!!$text.data('iHtml'))
            return true;

        if (!$text.is(':visible'))
            return false;

        var editorOptions = {
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
            callbacks: {},
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
            },
            lang: 'en-US', // Change to your chosen language
            imageAttributes: {
                icon: '<i class="note-icon-pencil"/>',
                removeEmpty: false, // true = remove attributes | false = leave empty if present
                disableUpload: true // true = don't display Upload Options | Display Upload Options
            }
        };

        editorOptions.popover = $.extend(true, {}, $.summernote.options.popover);
        editorOptions.popover.image.splice(0, 0, ['custom', ['imageAttributes']]);

        var data = {};
        var hasTranslation = !!options.translationId;

        if (hasTranslation) {
            data.translationId = options.translationId;

            editorOptions.callbacks.onChange = onTextUpdated;
        }

        $text.data('iHtml', data).summernote(editorOptions);

        if (hasTranslation) {
            inSite.common.editorTranslation.addLoad(options.translationId, options.id, onTranslationUpdated);
            inSite.common.editorTranslation.addLangChanged(options.translationId, options.id, onTranslationUpdated);
        }

        return true;
    }

    function onTextUpdated(html, $editable) {
        var data = $(this).data('iHtml');

        inSite.common.editorTranslation.setText(data.translationId, html);
    }

    function onTranslationUpdated(id) {
        var $element = $(document.getElementById(id));
        if ($element.length != 1)
            return;

        var data = $element.data('iHtml');

        var text = inSite.common.editorTranslation.getText(data.translationId);
        if (text == null)
            text = '';

        $element.summernote('code', text);
    }
})();

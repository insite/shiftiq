(function () {
    var instance; {
        var inSite = window.inSite = window.inSite || {};
        var common = inSite.common = inSite.common || {};
        instance = common.markdownEditor = common.markdownEditor || {};
    }

    var $fullscreen = null;
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
            var editor = $text.data('iMd').editor;
            var selection = editor.codemirror.doc.getSelection(' ');
            var hasSelection = selection.length > 0;
            var text = hasSelection ? selection : options.title;

            var link = '[' + text + '](' + options.url + ')';

            if (typeof options.img == 'undefined')
                options.img = (/\.(gif|jpg|jpeg|tiff|png)$/i).test(options.url);

            if (options.img)
                link = '!' + link;

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

    instance.getEditor = function (id) {
        const element = document.getElementById(id);
        return element ? $(element).data('iMd')?.editor : null;
    };

    function init(options) {
        var textEl = document.getElementById(options.id);
        if (!textEl)
            return true;

        var $text = $(textEl);
        if (!!$text.data('iMd'))
            return true;

        if (!$text.is(':visible'))
            return false;

        var mdeOptions = {
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
                'side-by-side',
                'fullscreen',
                '|',
                'guide'
            ],
            insertTexts: {
                image: ['![', '](https://)'], 
                link: ['[', '](https://)']
            },
            previewRender: function (plainText, preview) {
                if (data.onPreview) {
                    var args = {
                        preview: preview,
                        text: plainText,
                        html: null
                    };

                    data.onPreview(args);

                    if (typeof args.html == 'string')
                        return args.html;

                    plaingText = args.text;
                }

                return $(this.element).data('iMd').editor.markdown(plainText);
            },
        };

        execFunctionByPath(options.onSetup, mdeOptions, options);

        mdeOptions.element = textEl;

        var editor = new SimpleMDE(mdeOptions);

        editor.codemirror.on('refresh', onCodeMirrorRefresh);
        editor.codemirror.on('change', onTextUpdated);

        if (textEl.style.width)
            $text.parent('div').css('width', textEl.style.width);

        var cmWrapper = editor.codemirror.getWrapperElement();
        var cmScroller = editor.codemirror.getScrollerElement();

        if (textEl.style.height) {
            cmWrapper.style.height = textEl.style.height;
        }

        if (textEl.style.minHeight) {
            cmWrapper.style.minHeight = textEl.style.minHeight;
            cmScroller.style.minHeight = textEl.style.minHeight;
        }

        var data = {
            editor: editor,
            save: saveText,

            onSetText: getFunction(options.onSetText),
            onGetText: getFunction(options.onGetText),
            onPreview: getFunction(options.onPreview)
        };

        if (data.onSetText) {
            var args = { text: $text.val() };
            data.onSetText(args);
            mdeOptions.initialValue = args.text;
        }

        $text.data('iMd', data);

        if (options.translationId) {
            data.translationId = options.translationId;
            data.save = saveTranslation;

            inSite.common.editorTranslation.addLoad(options.translationId, options.id, onTranslationUpdated);
            inSite.common.editorTranslation.addLangChanged(options.translationId, options.id, onTranslationUpdated);
        }

        editor.codemirror.refresh();

        execFunctionByPath(options.onInited, editor);

        return true;
    }

    function saveText(data, text) {
        data.editor.element.value = text;
    }

    function saveTranslation(data, text) {
        data.editor.element.value = text;
        inSite.common.editorTranslation.setText(data.translationId, text);
    }

    function onTextUpdated(editor, change) {
        var $element = $(editor.getTextArea());
        var data = $element.data('iMd');
        var text = data.editor.value();

        if (data.onGetText) {
            var args = { text: text };
            data.onGetText(args);
            text = args.text;
        }

        data.save(data, text);

        // This line is required to render more than 12ish rows on the initial load
        setTimeout(() => data.editor.codemirror.refresh());
    }

    function onTranslationUpdated(id) {
        var $element = $(document.getElementById(id));
        if ($element.length != 1)
            return;

        var data = $element.data('iMd');

        var text = inSite.common.editorTranslation.getText(data.translationId);
        if (text == null)
            text = '';

        if (data.onSetText) {
            var args = { text: text };
            data.onSetText(args);
            text = args.text;
        }

        data.editor.value(text);
    }

    function onCodeMirrorRefresh(s) {
        var $element = $(s.getTextArea());
        var data = $element.data('iMd');

        if (data.editor.isFullscreenActive()) {
            if ($fullscreen !== null) {
                if ($fullscreen.is($element))
                    return;

                moveBack($fullscreen);
            } else {
                $('body').addClass('input-md-fullscreen');
            }

            moveToRoot($element);

            $fullscreen = $element;
        } else {
            if ($fullscreen !== null)
                moveBack($fullscreen);

            $fullscreen = null;

            $('body').removeClass('input-md-fullscreen');
        }

        function moveToRoot($el) {
            if ($el.data('placeholder'))
                return;

            var $wrapper = $el.closest('div.input-md-wrapper');
            var $placeholder = $('<div>').insertAfter($wrapper);
            $el.data('placeholder', $placeholder);
            $wrapper.detach().appendTo(theForm);
        }

        function moveBack($el) {
            var $placeholder = $el.data('placeholder');
            if (!$placeholder)
                return;

            $el.closest('div.input-md-wrapper').detach().insertBefore($placeholder);
            $el.data('placeholder', null);
            $placeholder.remove();
        }
    }

    function getFunction(value) {
        if (typeof value != 'undefined') {
            if (typeof value == 'string')
                value = inSite.common.getObjByName(value);

            if (typeof value == 'function')
                return value;
        }

        return null;
    }

    function execFunctionByPath(path) {
        var fn = getFunction(path);
        if (fn)
            fn.apply(null, Array.prototype.slice.call(arguments, 1));
    }
})();

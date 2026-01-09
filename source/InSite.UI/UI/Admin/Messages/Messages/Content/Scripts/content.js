(function () {
    var instance = window.messageContent = window.messageContent || {};
    var placeholderName = 'placeholder';
    var $activeMenu = null;
    var options = null;
    var loadeded = false;
    var state = {
        isPreview: false,
        isSideBySide: false,
        hasContent: false
    };
    var mdeStateChangedHandler = null;
    var mdeContentChangedHandler = null;
    var autoSaveHandler = null;
    var isAutoSave = false;

    $(window).on('click', onWindowClick);

    $(document).ready(function () {
        if (loadeded)
            return;

        loadeded = true;

        $(document.getElementById(options.docStyleId)).on('change', onDocStyleChange);
        $(document.getElementById(options.docTypeId)).on('change', onDocTypeChange);
    });

    instance.init = function (o) {
        if (loadeded)
            return;

        options = o;
    };

    instance.onMdeSetup = function (options) {
        var insertIndex = 0;

        if (options.toolbar instanceof Array && options.toolbar.length > 0) {
            insertIndex = options.toolbar.length;

            for (var i = 0; i < options.toolbar.length; i++) {
                if (options.toolbar[i] == 'image') {
                    insertIndex = i + 1;
                    break;
                }
            }
        } else {
            options.toolbar = [];
        }

        options.toolbar.splice(insertIndex, 0, '|', {
            name: placeholderName,
            action: onToolbarSubmenuAction,
            className: 'fa fa-brackets-curly',
            title: 'Insert Placeholder'
        });

        if (options.toolbar[0] == '|')
            options.toolbar.splice(0, 1);
    };

    instance.onMdeInited = function (editor) {
        editor.codemirror.on('update', onMdeStateChanged);
        editor.codemirror.on('change', onAutoSave);
        editor.codemirror.on('change', onPreviewRefresh);

        for (var i = 0; i < editor.toolbar.length; i++) {
            var item = editor.toolbar[i];
            if (typeof item == 'object' && item.name == placeholderName) {
                if (!item.$menu)
                    item.$menu = createPlaceholderMenu(editor).insertAfter(editor.element);

                break;
            }
        }
    };

    instance.onMdePreview = function (args) {
        args.html = '<p></p>';

        onMdeStateChanged();
    };

    instance.onAutoSaveStart = function () {
        isAutoSave = true;
        showAutoSaveIndicator(true);
    };

    instance.onAutoSaveEnd = function () {
        isAutoSave = false;
        showAutoSaveIndicator(false);
    };

    function onDocStyleChange() {
        onPreviewRefresh();
        onAutoSave();
    }

    function onDocTypeChange() {
        onPreviewRefresh();
        onAutoSave();
    }

    function onMdeStateChanged(isTimeout) {
        if (mdeStateChangedHandler != null) {
            clearTimeout(mdeStateChangedHandler);
            mdeStateChangedHandler = null;
        }

        if (isTimeout !== true) {
            mdeStateChangedHandler = setTimeout(onMdeStateChanged, 10, true);
            return;
        }

        var editor = $(document.getElementById(options.inputId)).data('iMd').editor;
        var isPreview = editor.isPreviewActive();
        var isSideBySide = editor.isSideBySideActive();

        if (state.isPreview == isPreview && state.isSideBySide == isSideBySide)
            return;

        state.isPreview = isPreview;
        state.isSideBySide = isSideBySide;

        var $parent = $(editor.element.parentElement);

        if (editor.isSideBySideActive()) {
            var $mdePreview = $parent.find('.editor-preview-active-side').css('display', 'none');

            var $msPreview = $parent.find('.message-preview-side');
            if ($msPreview.length == 0)
                $msPreview = $createPreview().addClass('message-preview-side').insertAfter($mdePreview);

            $msPreview.addClass('mp-active');
        } else if (editor.isPreviewActive()) {
            var $mdePreview = $parent.find('.editor-preview-active').css('display', 'none');

            var $msPreview = $parent.find('.message-preview');
            if ($msPreview.length == 0)
                $msPreview = $createPreview().addClass('message-preview').insertBefore($mdePreview);

            $msPreview.addClass('mp-active');
        } else {
            $parent
                .find('.message-preview,.message-preview-side')
                .removeClass('mp-active');

            $parent
                .find('.editor-preview,.editor-preview-side')
                .css('display', '');
        }

        onPreviewRefresh(true);

        function $createPreview() {
            return $('<div><div><iframe></iframe></div></div>');
        }
    }

    function onPreviewRefresh(isTimeout) {
        if (mdeContentChangedHandler != null) {
            clearTimeout(mdeContentChangedHandler);
            mdeContentChangedHandler = null;
        }

        if (isTimeout !== true) {
            mdeContentChangedHandler = setTimeout(onPreviewRefresh, 250, true);
            return;
        }

        var editor = $(document.getElementById(options.inputId)).data('iMd').editor;
        setTimeout(() => editor.codemirror.refresh(), 0);

        var $root = $(editor.element.parentElement);

        var html = '<!DOCTYPE html>';
        var $iframes = null;

        if (state.isPreview || state.isSideBySide)
            $iframes = $root.find('.mp-active iframe');

        if ($iframes != null && $iframes.length > 0) {
            html += getPreviewHtml(editor.codemirror.getValue());
            state.hasContent = true;
        } else if (state.hasContent) {
            state.hasContent = false;
            $iframes = $root.find('.message-preview iframe,.message-preview-side iframe');
        } else {
            return;
        }

        editor.codemirror.focus();

        $iframes.each(function () {
            var doc = $(this).contents()[0];

            doc.open();
            doc.write(html);
            doc.close();
        });
    }

    function onAutoSave(isTimeout) {
        if (autoSaveHandler != null) {
            clearTimeout(autoSaveHandler);
            autoSaveHandler = null;
        }

        if (isAutoSave || isTimeout !== true) {
            autoSaveHandler = setTimeout(onAutoSave, 1500, true);
            return;
        }

        document.getElementById(options.autoSaveId).ajaxRequest('autosave');
    }

    function onToolbarSubmenuAction(editor) {
        var $menu = this.$menu;
        if (!$menu)
            return;

        if ($activeMenu != null && $activeMenu.is($menu)) {
            closeActiveMenu();
            return;
        }

        var $link = $(editor.toolbarElements[this.name]);

        var linkPosition = $link.position();
        var parentPosition = $link.offsetParent().position();

        var menuWidth = $menu.outerWidth();
        var menuTopMargin = parseInt($menu.css('margin-top'));
        if (isNaN(menuTopMargin))
            menuTopMargin = 0;

        var menuPosition = {
            top: linkPosition.top + parentPosition.top + $link.height() + menuTopMargin,
            left: linkPosition.left + parentPosition.left
        };

        if ($link.offset().left + menuWidth > $(window).width())
            menuPosition.left = menuPosition.left - menuWidth + $link.outerWidth();

        $menu.css(menuPosition);

        if (editor.isFullscreenActive())
            $menu.addClass('fullscreen');
        else
            $menu.removeClass('fullscreen');

        setTimeout(function () {
            $activeMenu = $menu.show();
        }, 0);
    }

    function onPlaceholderMenuClick(e) {
        e.preventDefault();

        var $this = $(this);
        var editor = $this.closest('ul').data('editor');
        if (!editor)
            return;

        var value = $this.data('value');
        if (typeof value != 'string' || value.length == 0)
            return;

        var type = $this.data('type');
        var doc = editor.codemirror.doc;
        var cursor = doc.getCursor();
        var range = {
            line: cursor.line,
            ch: cursor.ch
        };
        var currentLine = doc.getLine(cursor.line);
        var prefix = '';
        var postfix = '';

        if (type == 'block') {
            if (cursor.ch > 0) {
                prefix = '\n\n';
            } else if (cursor.line > 0 && doc.getLine(cursor.line - 1).length > 0) {
                prefix = '\n';
            }

            if (cursor.ch == 0 && currentLine.length == 0 || cursor.ch >= currentLine.length - 1) {
                var lineCount = doc.lineCount();
                if (lineCount - cursor.line > 2 && doc.getLine(cursor.line + 1).length == 0) {
                    range.line += 2;
                    range.ch = 0;
                } else if (lineCount - cursor.line > 1) {
                    range.line += 1;
                    range.ch = 0;
                }
            }

            postfix = '\n\n';
        } else {
            if (cursor.ch > 0 && currentLine[cursor.ch - 1] != ' ')
                prefix = ' ';

            if (cursor.ch < currentLine.length - 1 && currentLine[cursor.ch] != ' ')
                postfix = ' ';
        }

        doc.replaceRange(prefix + value + postfix, cursor, range);

        editor.codemirror.focus();

        closeActiveMenu();
    }

    function onWindowClick(e) {
        if ($activeMenu == null)
            return;

        if (e.target && ($activeMenu.is(e.target) || $.contains($activeMenu.get(0), e.target)))
            return;

        closeActiveMenu();
    }

    function getPreviewHtml(value) {
        messageMarkdown.setDocType($(document.getElementById(options.docTypeId)).val());

        var domain = window.location.protocol + '//' + window.location.host;
        var html = '<html><head><link rel="stylesheet" href="' + domain + '/UI/Layout/common/parts/css/markdown/common.css">';

        var docStyle = $(document.getElementById(options.docStyleId)).val();
        if (docStyle != null && typeof docStyle == 'string' && docStyle.length > 0)
            html += '<link rel="stylesheet" href="' + domain + docStyle + '">';

        html += '<link rel="stylesheet" href="' + domain + '/UI/Layout/common/parts/css/markdown/markdown.editor.css">';
        html += '</head><body>';
        html += messageMarkdown.makeHtml(value);
        html += `<script type="text/javascript">
(function () {
    var anchors = document.querySelectorAll("a");

    for (var i = 0; i < anchors.length; i++)
        anchors[i].addEventListener("click", onAnchorClick);

    function onAnchorClick(e) {
        e.preventDefault();
    }
})();
</script>`;
        html += '</body></html>';

        return html;
    }

    function closeActiveMenu() {
        $activeMenu.hide();
        $activeMenu = null;
    }

    function createPlaceholderMenu(editor) {
        var $menu = $('<ul class="dropdown-menu dd-placeholder">').append(
            $('<li><a data-value="$Recipient-First-Name" class="dropdown-item">Recipient First Name</a></li>'),
            $('<li><a data-value="$Recipient-Last-Name" class="dropdown-item">Recipient Last Name</a></li>'),
            $('<li><a data-value="$RecipientEmail" class="dropdown-item">Recipient Email</a></li>'),
            $('<li><a data-value="$Recipient-Person-Code" class="dropdown-item">Recipient Person Code</a></li>'),
            $('<li><hr class="dropdown-divider"></li>'),
            $('<li><a data-type="block" data-value="$Unsubscribe-Link" class="dropdown-item">Unsubscribe Link</a></li>'),
            $('<li><a data-type="block" data-value="$Social-Media-Links" class="dropdown-item">Social Media Links</a></li>')
        );

        $menu.data('editor', editor).on('click', 'a', onPlaceholderMenuClick);

        return $menu;
    }

    function showAutoSaveIndicator(show) {
        var $indicator = $('#autosave-indicator');
        if ($indicator.is(':animated')) {
            setTimeout(showAutoSaveIndicator, 20, show);
        } else {
            if (show)
                $indicator.fadeIn(100);
            else
                $indicator.fadeOut(100);
        }
    }
})();
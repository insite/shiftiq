(function () {
    const instance = inSite.common.baseInput = inSite.common.baseInput || {};

    // public methods

    instance.init = function (id) {
        const idType = typeof id;

        let $elem = null;

        if (idType === 'string') {
            if (id.length > 0) {
                if (id[0] === '#') {
                    $elem = $(id);
                } else if (typeof id === 'string' && id.length > 0) {
                    $elem = $(document.getElementById(id));
                }
            }
        } else if (idType === 'object') {
            if (id instanceof HTMLElement)
                $elem = $(id);
            else if (id instanceof jQuery)
                $elem = id;
        }

        if ($elem === null || $elem.length !== 1 || !($elem[0] instanceof HTMLElement) || $elem.data('baseInput'))
            return;

        const settings = {};

        let _onfocus = onFocus
        let _onblur = onBlur;
        let isNum = false;

        $elem
            .data('baseInput', settings)
            .on('keypress', onInputKeyPress);

        if ($elem.hasClass('insite-numeric')) {
            const numericSettings = $elem.data('settings');

            if (numericSettings) {
                $elem.attr('data-settings', null);
                $elem.data('settings', null)

                settings.numeric = numericSettings;

                $elem
                    .on('keypress', onNumericKeypress)
                    .on('change', onNumericChange);

                _onfocus = onNumericFocus;
                _onblur = onNumericBlur;

                isNum = true;
            }
        }

        if (!isNum) {
            const translationId = $elem.data('translation');

            if (translationId) {
                $elem.attr('data-translation', null);

                settings.translationId = translationId;

                $elem.on('input', onUpdateTranslation);

                const elemId = $elem.attr('id');

                inSite.common.editorTranslation.addLoad(settings.translationId, elemId, onTranslationUpdated);
                inSite.common.editorTranslation.addLangChanged(settings.translationId, elemId, onTranslationUpdated);
            }

            if ($elem.data('breakHtml') === 1)
                $elem.on('input', onBreakHtml);

            if ($elem.data('masked') === 1) {
                var mask = $elem.val();
                if (mask.length > 0)
                    $elem
                        .data('mask', {
                            text: mask,
                            isMask: true
                        })
                        .on('click', onMaskedClick)
                        .on('keydown', onMaskedKeyDown)
                        .on('input', onMaskedInput);
            }
        }

        $elem
            .on('focus', _onfocus)
            .on('blur', _onblur);

        if ($elem.is(':focus'))
            _onfocus.call($elem[0]);
        else
            _onblur.call($elem[0]);
    };

    instance.focus = function (id, cursorToEnd) {
        const input = document.getElementById(id);
        if (!input)
            return;

        const $input = $(input).focus();

        if (cursorToEnd)
            input.selectionStart = input.selectionEnd = $input.val().length;
    };

    // event handlers

    function onBreakHtml(e) {
        const oldValue = this.value;
        const type = e.originalEvent.inputType;
        const isDeleteBackward = type == 'deleteWordBackward' || type == 'deleteContentBackward';
        const isDeleteForward = type == 'deleteWordForward' || type == 'deleteContentForward';

        let newValue = oldValue;
        let cStart = this.selectionStart;
        let cEnd = this.selectionEnd;

        if (cStart == cEnd && cStart < newValue.length && (isDeleteBackward || isDeleteForward)) {
            const ch1 = newValue[cStart - 1];
            const ch2 = newValue[cStart];

            if (isHtmlTag(ch1, ch2)) {
                if (isDeleteBackward) {
                    newValue = newValue.substring(0, cStart - 1) + newValue.substring(cStart);
                    cStart--;
                    cEnd--;
                } else if (isDeleteForward) {
                    newValue = newValue.substring(0, cStart) + newValue.substring(cStart + 1);
                }
            }
        }

        newValue = breakHtml(newValue);

        if (oldValue == newValue)
            return;

        if (cStart == cEnd) {
            const diff = newValue.length - oldValue.length;
            if (diff > 0) {
                cStart += diff;
                cEnd += diff;
            }
        }

        this.value = newValue;
        this.setSelectionRange(cStart, cEnd);

        function isHtmlTag(ch1, ch2) {
            if (ch1 == '<') {
                return ch2 >= 'a' && ch2 <= 'z' || ch2 >= 'A' && ch2 <= 'Z' || ch2 == '!' || ch2 == '/' || ch2 == '?';
            } else if (ch1 == '&') {
                return ch2 >= 'a' && ch2 <= 'z' || ch2 >= 'A' && ch2 <= 'Z' || ch2 == '#';
            }

            return false
        }

        function breakHtml(input) {
            let index = -1;
            let index1 = -2;
            let index2 = -2;
            let prevInsertIndex = 0;
            let output = '';

            while (true) {
                if (index1 != -1 && index1 <= index)
                    index1 = input.indexOf('<', index + 1);

                if (index2 != -1 && index2 <= index)
                    index2 = input.indexOf('&', index + 1);

                if (index1 != -1 && index1 < index2)
                    index = index1;
                else if (index2 != -1)
                    index = index2;
                else
                    break;

                var ch1 = input[index];
                var ch2 = input[index + 1];

                if (ch1 === '<') {
                    if (isLatinLetter(ch2) || ch2 == '!' || ch2 == '/' || ch2 == '?') {
                        insert(ch1 + ' ' + ch2);
                        prevInsertIndex = index + 2;
                    }
                }
                else if (ch1 == '&') {
                    if (isLatinLetter(ch2) || ch2 == '#') {
                        if (index > 0 && input[index - 1] != ' ')
                            insert(' ' + ch1 + ' ' + ch2);
                        else
                            insert(ch1 + ' ' + ch2);

                        prevInsertIndex = index + 2;
                    }
                }
                else
                    alert('breakHtml: unexpected character (' + ch1 + ')');
            }

            if (output.length == 0)
                return input;

            if (prevInsertIndex < input.length)
                output += input.substring(prevInsertIndex, input.length);

            return output;

            function insert(value) {
                output += input.substring(prevInsertIndex, index) + value;
            }

            function isLatinLetter(ch) {
                return ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z';
            }
        }
    }

    function onInputKeyPress(e) {
        if (e.which == 13) {
            if (e.currentTarget != null && typeof e.currentTarget != 'undefined' && e.currentTarget.tagName === 'INPUT') {
                $(e.currentTarget).trigger('change');

                e.preventDefault();
            }
        }
    }

    function onMaskedClick() {
        var $this = $(this);
        var data = $this.data('mask');

        data.isMask = $this.val() == data.text;

        if (data.isMask) {
            this.selectionStart = 0;
            this.selectionEnd = this.value.length;
        }
    }

    function onMaskedKeyDown(e) {
        var $this = $(this);
        var data = $this.data('mask');

        data.isMask = $this.val() == data.text;

        if (!data.isMask)
            return;

        this.selectionStart = 0;
        this.selectionEnd = this.value.length;

        if (e.which == 33 || e.which >= 34 && e.which <= 40) {

            e.preventDefault();
            e.stopPropagation();
            return;
        }
    }

    function onMaskedInput(e) {
        var $this = $(this);
        var data = $this.data('mask');

        if (data.isMask) {
            $this.val(e.originalEvent.data);
            data.isMask = $this.val() == data.text;
        }
    }

    function onFocus() {
        const $this = $(this);

        $this.removeClass('liEmpty');

        const placeholder = $this.prop('placeholder');
        if (typeof placeholder === 'string' && placeholder.length > 0) {
            $this
                .data('placeholder', placeholder)
                .prop('placeholder', '');
        }
    }

    function onBlur() {
        const $this = $(this);

        if ($this.val().length == 0)
            $this.addClass('liEmpty');

        const placeholder = $this.data('placeholder');
        if (typeof placeholder === 'string' && placeholder.length > 0) {
            $this
                .prop('placeholder', placeholder)
                .data('placeholder', '');
        }
    }

    function onNumericFocus() {
        const $this = $(this);

        const settings = $this.data('baseInput');
        if (settings.numeric.group == true) {
            const value = $this.val();
            if (value.length > 0) {
                let decimal;
                let integer;

                let decimalIndex = value.lastIndexOf('.');
                if (decimalIndex >= 0) {
                    decimal = value.substring(decimalIndex);
                    integer = value.substring(0, decimalIndex)
                } else {
                    decimal = '';
                    integer = value;
                }

                if (integer.match(/^\d{1,3}(,\d{3})*$/g))
                    $this.val(integer.replaceAll(',', '') + decimal);
            }
        }

        onFocus.call(this);

        $this.select();
    }

    function onNumericBlur() {
        onBlur.call(this);

        const $this = $(this)
        const settings = $this.data('baseInput');

        if (settings.numeric.group == true) {
            const groupSize = 3;
            const value = $this.val();

            if (value.length > groupSize) {
                let decimal;
                let integer;

                const decimalIndex = value.lastIndexOf('.');
                if (decimalIndex >= 0) {
                    decimal = value.substring(decimalIndex);
                    integer = value.substring(0, decimalIndex)
                } else {
                    decimal = '';
                    integer = value;
                }

                if (integer.match(/^\d+$/g)) {
                    let groupIndex = integer.length % groupSize;

                    let groupedInteger = '';

                    if (groupIndex > 0)
                        groupedInteger = integer.substring(0, groupIndex);

                    while (groupIndex < integer.length) {
                        if (groupedInteger.length)
                            groupedInteger += ',';

                        groupedInteger += integer.substring(groupIndex, groupIndex = groupIndex + groupSize);
                    }

                    $this.val(groupedInteger + decimal);
                }
            }
        }
    }

    function onNumericKeypress(e) {
        if (e.charCode === 0 || e.altKey === true || e.ctrlKey === true || e.which == 13 || e.which >= 48 && e.which <= 57 || e.key === '.' || e.key === '-')
            return;

        e.preventDefault();

        const $this = $(this);

        const timeoutHandler = parseInt($this.data('invalid-timeout'));
        if (!isNaN(timeoutHandler))
            clearTimeout(timeoutHandler);
        else
            $(this).addClass('input-error');

        $this.data('invalid-timeout', setTimeout(function ($this) {
            $this.removeClass('input-error');
            $this.data('invalid-timeout', null);
        }, 250, $this));
    }

    function onNumericChange(e) {
        const $this = $(this);
        const settings = $this.data('baseInput');

        let value = parseFloat($this.val());

        if (isNaN(value) || !isFinite(value)) {
            $this.val('');
            return;
        }

        const isInteger = settings.numeric.mode === 'integer';

        let max = settings.numeric.max;
        let min = settings.numeric.min;

        if (typeof max !== 'number') {
            if (isInteger)
                max = 2147483647;
            else
                max = Number.MAX_SAFE_INTEGER;
        }

        if (typeof min !== 'number') {
            if (isInteger)
                min = -2147483648;
            else
                min = Number.MIN_SAFE_INTEGER;
        }

        if (value > max)
            value = max;

        if (value < min)
            value = min;

        if (isInteger)
            $this.val(value.toFixed());
        else
            $this.val(value.toFixed(settings.numeric.decimals));
    }

    function onUpdateTranslation(e) {
        const $this = $(this);
        const data = $this.data('baseInput');

        inSite.common.editorTranslation.setText(data.translationId, $this.val());
    }

    function onTranslationUpdated(id) {
        const $this = $(document.getElementById(id));
        if ($this.length != 1)
            return;

        const data = $this.data('baseInput');

        let text = inSite.common.editorTranslation.getText(data.translationId);
        if (text == null)
            text = '';

        $this.val(text);
    }
})();

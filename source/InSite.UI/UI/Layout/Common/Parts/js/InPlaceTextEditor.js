var inPlaceTextEditor = {
    // constants

    __confirmClose: 'Changes you have made will be lost. Do you want to close the editor?',

    // fields

    __controls: new Array(),
    __currentEditor: null,
    __isEnabled: true,

    // methods

    __init: function () {
        for (var i = 0; i < inPlaceTextEditor.__controls.length; i++) {
            inPlaceTextEditor.__initControl(inPlaceTextEditor.__controls[i]);
        }
    },
    __initControl: function (ctrl) {
        var $ctrl = $('#' + ctrl.id);
        if ($ctrl.length == 0 || $ctrl.data('inited') === true)
            return;

        $ctrl.on('click', function () {
            inPlaceTextEditor.__onShowEditor(ctrl);
        });

        $('#' + ctrl.id + '_save').on('click', function () {
            inPlaceTextEditor.__onSaveClicked(ctrl);

            return false;
        });

        $('#' + ctrl.id + '_input').on('keypress', function (e) {
            if (e.which == 13 && this.tagName == 'INPUT' && this.type == 'text') {
                e.preventDefault();
                e.stopPropagation();

                inPlaceTextEditor.__onSaveClicked(ctrl);
            }
        });

        $('#' + ctrl.id + '_cancel').on('click', function () {
            inPlaceTextEditor.__onCancelClicked(ctrl);

            return false;
        });

        $ctrl.data('inited', true);
    },
    __getControl: function (id) {
        for (var i = 0; i < inPlaceTextEditor.__controls.length; i++) {
            if (inPlaceTextEditor.__controls[i].id === id)
                return inPlaceTextEditor.__controls[i];
        }

        return null;
    },
    __setBoxValue: function (control, value) {
        var $control = $('#' + control.id);

        if (value == null || value.length === 0) {
            $control.addClass('ipte-empty').html(control.emptyMessage);
            return;
        }

        $control.removeClass('ipte-empty');

        if ($control.data('ismarkdown') !== 'no') {
            $.ajax({
                type: 'POST',
                dataType: "json",
                url: '/api/contents/markdown-to-html',
                data: { markdown: value },
                error: function () { alert('An error occurred during operation.'); },
                success: function (data) { $control.html(data); }
            });
        } else {
            $control.html(value);
        }
    },
    __getControlInput: function (id) {
        return document.getElementById(id + '_input');
    },
    __setControlValue: function (id, value) {
        inPlaceTextEditor.__getControlInput(id).value = value;
    },
    __getControlValue: function (id) {
        return inPlaceTextEditor.__getControlInput(id).value;
    },
    __editorVisible: function (id, visible) {
        if (visible) {
            document.getElementById(id).style.display = 'none';
            document.getElementById(id + '_editor').style.display = '';

            inPlaceTextEditor.__getControlInput(id).focus();
        } else {
            document.getElementById(id).style.display = '';
            document.getElementById(id + '_editor').style.display = 'none';
        }
    },
    __executeFunction: function (funcName, context) {
        var args = [].slice.call(arguments).splice(2);
        var namespaces = funcName.split(".");
        var func = namespaces.pop();

        for (var i = 0; i < namespaces.length; i++) {
            context = context[namespaces[i]];
        }

        return context[func].apply(this, args);
    },

    // event handlers

    __onShowEditor: function (control) {
        if (!inPlaceTextEditor.__isEnabled)
            return;

        if (inPlaceTextEditor.__currentEditor != null) {
            var currentControl = inPlaceTextEditor.__getControl(inPlaceTextEditor.__currentEditor.id);
            if (inPlaceTextEditor.__getControlValue(currentControl.id) !== inPlaceTextEditor.__currentEditor.text) {
                if (!confirm(inPlaceTextEditor.__confirmClose))
                    return;
            }

            inPlaceTextEditor.__onCancelClicked(currentControl);
        }

        inPlaceTextEditor.__editorVisible(control.id, true);

        inPlaceTextEditor.__currentEditor = { id: control.id, text: inPlaceTextEditor.__getControlValue(control.id) };
    },
    __onSaveClicked: function (control) {
        if (inPlaceTextEditor.__currentEditor == null || !inPlaceTextEditor.__isEnabled)
            return;

        var currentControl = inPlaceTextEditor.__getControl(inPlaceTextEditor.__currentEditor.id);
        var value = inPlaceTextEditor.__getControlValue(currentControl.id);

        inPlaceTextEditor.__setBoxValue(currentControl, value);

        inPlaceTextEditor.__editorVisible(control.id, false);

        var prevValue = inPlaceTextEditor.__currentEditor.text;

        inPlaceTextEditor.__currentEditor = null;

        if (currentControl.callbackFunc != null)
            inPlaceTextEditor.__executeFunction(currentControl.callbackFunc, window, currentControl.id, currentControl.callbackData, value, prevValue);
    },
    __onCancelClicked: function (control) {
        if (!inPlaceTextEditor.__isEnabled)
            return;

        if (inPlaceTextEditor.__currentEditor != null) {
            var currentControl = inPlaceTextEditor.__getControl(inPlaceTextEditor.__currentEditor.id);
            inPlaceTextEditor.__setBoxValue(currentControl, inPlaceTextEditor.__currentEditor.text);
            inPlaceTextEditor.__setControlValue(currentControl.id, inPlaceTextEditor.__currentEditor.text);
        }

        inPlaceTextEditor.__editorVisible(control.id, false);

        inPlaceTextEditor.__currentEditor = null;
    },

    // "public" methods

    register: function (v1, v2, v3, v4) {
        var item = inPlaceTextEditor.__getControl(v1);
        if (item != null)
            return;

        var ctrl = { id: v1, callbackData: v2, emptyMessage: v3, callbackFunc: v4 };

        inPlaceTextEditor.__controls.push(ctrl);
        inPlaceTextEditor.__initControl(ctrl);
    },
    setControlValue: function (id, value) {
        var currentControl = inPlaceTextEditor.__getControl(id);
        if (currentControl == null)
            return;

        inPlaceTextEditor.__setBoxValue(currentControl, value);
        inPlaceTextEditor.__setControlValue(currentControl.id, value);
    },
    setBoxValue: function (id, value) {
        var currentControl = inPlaceTextEditor.__getControl(id);
        if (currentControl == null)
            return;

        inPlaceTextEditor.__setBoxValue(currentControl, value);
    },
    enable: function (enable) {
        if (typeof enable !== 'boolean' || enable === inPlaceTextEditor.__isEnabled)
            return false;

        if (inPlaceTextEditor.__currentEditor != null) {
            var currentControl = inPlaceTextEditor.__getControl(inPlaceTextEditor.__currentEditor.id);
            if (inPlaceTextEditor.__getControlValue(currentControl.id) !== inPlaceTextEditor.__currentEditor.text) {
                if (!confirm(inPlaceTextEditor.__confirmClose))
                    return false;
            }

            inPlaceTextEditor.__onCancelClicked(currentControl);
        }

        inPlaceTextEditor.__isEnabled = enable;

        if (enable) {
            for (var i = 0; i < inPlaceTextEditor.__controls.length; i++)
                $('#' + inPlaceTextEditor.__controls[i].id).removeClass('disabled');
        } else {
            for (var i = 0; i < inPlaceTextEditor.__controls.length; i++)
                $('#' + inPlaceTextEditor.__controls[i].id).addClass('disabled');
        }

        return true;
    },
};


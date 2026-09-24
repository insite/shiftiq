(function () {
    const instance = inSite.common.updatePanel = inSite.common.updatePanel || {};

    const _settings = {};
    let handlersRegistered = false;

    function ensureRequestManagerHandlers() {
        if (handlersRegistered)
            return;

        if (inSite.common.getObjByName('Sys.WebForms.PageRequestManager')) {
            const mng = Sys.WebForms.PageRequestManager.getInstance();
            mng.add_beginRequest(onBeginRequest);
            mng.add_pageLoaded(onPageLoaded);
            mng.add_endRequest(onEndRequest);
            handlersRegistered = true;
        }
    }

    $(document).ready(function () {
        ensureRequestManagerHandlers();
    });

    instance.init = function (settings) {
        ensureRequestManagerHandlers();

        if (!settings || typeof settings.id != 'string' || typeof settings.name != 'string')
            return;

        const el = document.getElementById(settings.id);
        if (!el)
            return;

        if (settings.postBack)
            settings.postBack = new Function(settings.postBack);

        _settings[settings.id] = settings;
        _settings[settings.name] = settings;

        el.ajaxRequest = function (value) {
            request(this.id, value);
        };
    };

    function onBeginRequest(s, e) {
        const names = e.get_updatePanelsToUpdate();
        for (let i = 0; i < names.length; i++) {
            execEventHandler(s, names[i], 'onRequestStart', {
                asyncTarget: s._postBackSettings.asyncTarget,
                sourceElement: s._postBackSettings.sourceElement,
                originalArgs: e
            });
        }
    }

    function onPageLoaded(s, e) {
        const panels = e.get_panelsUpdated();
        for (let i = 0; i < panels.length; i++) {
            execEventHandler(s, panels[i].id, 'onResponseEnd', {
                asyncTarget: s._postBackSettings.asyncTarget,
                sourceElement: s._postBackSettings.sourceElement,
                originalArgs: e
            });
        }
    }

    function onEndRequest(s, e) {
        const err = e.get_error();
        if (!err)
            return;

        let msg = err.message || 'An error occurred while processing the request on the server.';
        msg = msg.replace(/^Sys\.WebForms\.PageRequestManager[a-zA-Z]*:\s*/, '');

        e.set_errorHandled(true);

        alert('Error: ' + msg);

        for (let n in _settings) {
            execEventHandler(s, n, 'onRequestError', {
                error: err,
                message: msg
            });
        }
    }

    function request(id, value) {
        if (!_settings.hasOwnProperty(id))
            return;

        const data = _settings[id];
        if (!data.postBack)
            return;

        const $panel = $(document.getElementById(data.id));
        if ($panel.length != 1)
            return;

        const inputs = document.getElementsByName(data.name);
        for (let i = 0; i < inputs.length; i++)
            inputs[i].remove();

        if (typeof value != 'string')
            value = '';

        const $input = $('<input type="hidden">').attr('name', data.name).val(value);

        $panel.append($input);

        data.postBack();

        setTimeout(function () {
            $input.remove();
        });
    }

    function execEventHandler(sender, name, event, args) {
        if (!_settings.hasOwnProperty(name))
            return;

        const s = _settings[name];
        if (!s.hasOwnProperty(event))
            return;

        const fn = inSite.common.getObjByName(s[event]);
        if (typeof fn != 'function')
            return;

        const panel = document.getElementById(s.id);
        if (!panel)
            return;

        fn.call(panel, sender, args);
    }
})();

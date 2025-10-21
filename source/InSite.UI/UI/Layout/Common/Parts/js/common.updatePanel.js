(function () {
    var instance = inSite.common.updatePanel = inSite.common.updatePanel || {};

    var _settings = {};

    $(document).ready(function () {
        if (inSite.common.getObjByName('Sys.WebForms.PageRequestManager')) {
            Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(onBeginRequest);
            Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(onPageLoaded);
        }
    });

    instance.init = function (settings) {
        if (!settings || typeof settings.id != 'string' || typeof settings.name != 'string')
            return;

        var el = document.getElementById(settings.id);
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
        var names = e.get_updatePanelsToUpdate();
        for (var i = 0; i < names.length; i++) {
            execEventHandler(s, names[i], 'onRequestStart', {
                asyncTarget: s._postBackSettings.asyncTarget,
                sourceElement: s._postBackSettings.sourceElement,
                originalArgs: e
            });
        }
    }

    function onPageLoaded(s, e) {
        var panels = e.get_panelsUpdated();
        for (var i = 0; i < panels.length; i++) {
            execEventHandler(s, panels[i].id, 'onResponseEnd', {
                asyncTarget: s._postBackSettings.asyncTarget,
                sourceElement: s._postBackSettings.sourceElement,
                originalArgs: e
            });
        }
    }

    function request(id, value) {
        if (!_settings.hasOwnProperty(id))
            return;

        var data = _settings[id];
        if (!data.postBack)
            return;

        var $panel = $(document.getElementById(data.id));
        if ($panel.length != 1)
            return;

        var inputs = document.getElementsByName(data.name);
        for (var i = 0; i < inputs.length; i++)
            inputs[i].remove();

        if (typeof value != 'string')
            value = '';

        var $input = $('<input type="hidden">').attr('name', data.name).val(value);

        $panel.append($input);

        data.postBack();

        setTimeout(function () {
            $input.remove();
        });
    }

    function execEventHandler(sender, name, event, args) {
        if (!_settings.hasOwnProperty(name))
            return;

        var s = _settings[name];
        if (!s.hasOwnProperty(event))
            return;

        var fn = inSite.common.getObjByName(s[event]);
        if (typeof fn != 'function')
            return;

        var panel = document.getElementById(s.id);
        if (!panel)
            return;

        fn.call(panel, sender, args);
    }
})();
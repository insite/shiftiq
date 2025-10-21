(function () {
    var instance;
    {
        var inSite = window.inSite = window.inSite || {};
        var common = inSite.common = inSite.common || {};
        instance = common.editorTranslation = common.editorTranslation || {};
    }

    var languages = null;
    var __settings = {};
    var mdConverter = null;
    var callbackOnLoad = {};
    var callbackOnLangChanged = {};

    // public methods

    instance.setup = function (data) {
        if (languages == null)
            languages = data;
    };

    instance.init = function (options) {
        if (typeof options.id != 'string' || !options.state)
            return;

        var $input = $('#' + options.id);
        var $output = $('#' + options.output);
        var $container = $('#' + options.container);

        if ($input.length !== 1)
            return;

        if ($output.length > 1)
            $output = $();

        if ($container.length > 1)
            $container = $();

        var settings = {
            $input: $input,
            $output: $output,
            $container: $container,
            isMarkdown: !!options.isMd,
            changed: options.changed,

            onSetText: getFunction(options.onSetText),
            onGetText: getFunction(options.onGetText),
        };

        $input.data('id', options.id).val(JSON.stringify(options.state));

        __settings[options.id] = settings;

        bind(settings);

        if (settings.changed)
            execCallbacks(callbackOnLoad, options.id);
    };

    instance.addLoad = function (id, arg1, arg2) {
        var settings = getSettings(id);
        addCallback(callbackOnLoad, !!settings && settings.changed, id, arg1, arg2);
    };

    instance.addLangChanged = function (id, arg1, arg2) {
        addCallback(callbackOnLangChanged, false, id, arg1, arg2);
    };

    instance.setText = function (id, text, lang) {
        var settings = getSettings(id);
        if (settings != null)
            setText(settings, text, lang);
    };

    instance.setDefaultText = function (id, text) {
        instance.setText(id, text, 'en');
    };

    instance.getText = function (id, lang) {
        var settings = getSettings(id);
        if (settings != null)
            return getText(settings, lang);
    };

    instance.setLang = function (id, lang) {
        var settings = getSettings(id);
        if (settings != null)
            setLang(settings, lang);
    };

    instance.getLang = function (id) {
        var settings = getSettings(id);
        if (settings != null)
            return getLang(getState(settings));
    };

    instance.getDefaultText = function (id) {
        return instance.getText(id, 'en');
    };

    instance.exists = function (id) {
        return !!getSettings(id);
    };

    instance.setState = function (id, value) {
        var settings = getSettings(id);
        if (settings == null)
            return;

        if (typeof value.lang != 'string' || !value.lang || typeof value.data != 'object' || !languages.hasOwnProperty(value.lang))
            return;

        var state = getState(settings);

        state.lang = value.lang;

        for (var n in state.data) {
            if (state.data.hasOwnProperty(n))
                delete state.data[n];
        }

        for (var n in value.data) {
            if (value.data.hasOwnProperty(n) && languages.hasOwnProperty(n)) {
                var text = value.data[n];

                if (settings.onSetText) {
                    var args = { text: text };
                    settings.onSetText(args);
                    text = args.text;
                }

                state.data[n] = text;
            }
        }

        setState(settings, state);

        bind(settings);

        execCallbacks(callbackOnLangChanged, getId(settings));
    };

    instance.getState = function (id) {
        var settings = getSettings(id);
        if (settings != null)
            return getState(settings);
    };

    // events

    function onLangChanged(e) {
        e.preventDefault();

        var key = $(this).data('key');
        if (typeof key != 'object' || !key.id || !key.lang)
            return;

        var settings = getSettings(key.id);
        if (settings != null)
            setLang(settings, key.lang);
    }

    // private functions

    function getSettings(id) {
        if (__settings.hasOwnProperty(id))
            return __settings[id];

        return null;
    }

    function getId(settings) {
        return settings.$input.data('id');
    }

    function setState(settings, state) {
        var json = JSON.stringify(state);

        settings.$input.val(json);
    }

    function getState(settings) {
        var json = settings.$input.val();

        return JSON.parse(json);
    }

    function setLang(settings, lang) {
        if (!lang)
            return;

        lang = lang.toLowerCase().trim();
        if (!languages.hasOwnProperty(lang))
            return;

        var state = getState(settings);

        state.lang = lang;

        setState(settings, state);

        bind(settings);

        execCallbacks(callbackOnLangChanged, getId(settings));
    }

    function getLang(state) {
        var lang = state.lang;
        if (!lang)
            return 'en';
        else
            return lang;
    }

    function setText(settings, text, lang) {
        var state = getState(settings);

        if (!lang)
            lang = getLang(state);
        else if (!languages.hasOwnProperty(lang))
            return;

        if (text)
            text = text.trim();

        if (settings.onSetText) {
            var args = { text: text };
            settings.onSetText(args);
            text = args.text;
        }

        state.data[lang] = text;

        setState(settings, state);
    }

    function getText(settings, lang) {
        var state = getState(settings);

        if (!lang)
            lang = getLang(state);
        else
            lang = lang.toLowerCase();

        if (state.data.hasOwnProperty(lang)) {
            var text = state.data[lang];

            if (settings.onGetText) {
                var args = { text: text };
                settings.onGetText(args);
                text = args.text;
            }

            return text;
        }

        return null;
    }

    function bind(settings) {
        if (typeof settings == 'string')
            settings = getSettings(settings);

        if (settings == null || typeof settings != 'object')
            return;

        var id = getId(settings);
        var state = getState(settings);
        var lang = getLang(state);

        var langName = lang;
        if (languages.hasOwnProperty(langName))
            langName = languages[langName];

        if (settings.$output.length != 0)
            settings.$output.text(langName).data('code', lang).trigger('langChanged.translation', [lang]);

        if (settings.$container.length != 0) {
            var hasData = false;

            var $table = $('<table style="margin-top:5px;" class="translation-list">');
            var $tbody = $('<tbody>').appendTo($table);

            var data = state.data;
            for (var key in data) {
                if (!data.hasOwnProperty(key))
                    continue;

                var name = key.toLowerCase();
                if (name === lang)
                    continue;

                hasData = true;

                var value = data[key];

                if (settings.isMarkdown)
                    value = mdToHtml(value);

                $tbody.append(
                    $('<tr>').append(
                        $('<td class="lang-name">').append(
                            $('<a href="#">')
                                .data('key', { id: id, lang: key })
                                .on('click', onLangChanged)
                                .text(name)
                        ),
                        $('<td class="lang-value">').html(value)
                    )
                );
            }

            var $existTable = settings.$container.find('> table.translation-list');

            if ($existTable.length == 1) {
                if (hasData) {
                    $existTable.replaceWith($table);
                } else {
                    $existTable.remove();
                }
            } else if (hasData) {
                settings.$container.append($table);
            }
        }
    }

    function mdToHtml(value) {
        if (mdConverter == null) {
            if (typeof window.showdown == 'undefined')
                return value;

            mdConverter = new showdown.Converter({ simpleLineBreaks: true });
        }

        return mdConverter.makeHtml(value);
    }

    function addCallback(obj, execute, id, arg1, arg2) {
        if (typeof id != 'string' || id.length == 0)
            return;

        var uniqueId = null;
        var func = null;

        if (typeof arg1 == 'function') {
            func = arg1;
        } else if (typeof arg1 == 'string' && typeof arg2 == 'function') {
            if (arg1.length > 0)
                uniqueId = arg1;

            func = arg2;
        } else {
            return;
        }

        var list;
        if (obj.hasOwnProperty(id))
            list = obj[id];
        else
            obj[id] = list = [];

        var exists = false;

        if (uniqueId != null) {
            for (var i = 0; i < list.length; i++) {
                if (exists = list[i].id === uniqueId) {
                    list[i].func = func;
                    break;
                }
            }
        }

        if (!exists)
            list.push({ id: uniqueId, func: func });

        if (execute == true)
            func(uniqueId);
    }

    function execCallbacks(obj, id) {
        if (!obj.hasOwnProperty(id))
            return;

        var list = obj[id];

        for (var i = 0; i < list.length; i++) {
            var item = list[i];
            try {
                item.func(item.id);
            } catch (ex) {
                console.error(ex);
            }
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
})();
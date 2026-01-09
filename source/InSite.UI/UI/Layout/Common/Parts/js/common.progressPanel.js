(function () {
    var instance = inSite.common.progressPanel = inSite.common.progressPanel || {};

    var BindItem = defineClass({
        constructor: function () {
            this.requestVariables = {};
            this.contextVariables = {};
            this.contextItems = {};
        },
        get: function (name, id) {
            if (this.requestVariables.hasOwnProperty(name))
                return this.requestVariables[name];

            if (id && this.contextItems.hasOwnProperty(id)) {
                var item = this.contextItems[id];
                if (item.hasOwnProperty(name))
                    return item[name];
            }

            if (this.contextVariables.hasOwnProperty(name))
                return this.contextVariables[name];

            return '';
        },
        item: function (id) {
            if (id && this.contextItems.hasOwnProperty(id))
                return this.contextItems[id];

            return null;
        }
    });

    var registered = false;
    var panels = {};
    var cssClasses = Object.freeze({
        container: 'card card-default',
        header: 'card-header',
        body: 'card-body',
        footer: 'card-footer',
        btnDefault: 'btn btn-outline-secondary'
    });
    var $clickedSubmitter = null;

    $(document).ready(function () {
        if (inSite.common.getObjByName('Sys.WebForms.PageRequestManager')) {
            Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(onBeginRequest);
            Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(onPageLoaded);
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(onEndRequest);
        }
    });

    instance.register = register;

    function register() {
        if (registered)
            return;

        $(document).on('click', 'form button,form input[type=submit]', onSubmitClick);

        $('form').on('submit', onFormSubmit);

        if (window.__doPostBack) {
            var original__doPostBack = window.__doPostBack;
            window.__doPostBack = function () {
                original__doPostBack.apply(this, arguments);

                if (typeof window.Page_BlockSubmit == 'undefined' || Page_BlockSubmit === false)
                    onFormSubmit.apply(theForm)
            }
        }

        registered = true;
    }

    instance.init = function (settings) {
        if (!settings || typeof settings.id != 'string' || typeof settings.name != 'string' || !(settings.items instanceof Array) || settings.items.length == 0)
            return;

        var container = document.getElementById(settings.id);
        if (!container || $(container).data('progress_inited') === true)
            return;

        var data = {
            $container: $(container),

            name: settings.name,
            context: settings.context,
            request: null,
            isDisabled: settings.enabled === false
        };

        if (settings.callbacks)
            data.callbacks = settings.callbacks;

        if (settings.postBack)
            data.postBack = new Function(settings.postBack);

        if (settings.triggers) {
            var triggers = settings.triggers;
            triggers.postBack = triggers.postBack !== false;
            triggers.ajaxRequest = triggers.ajaxRequest !== false;

            var fnBody = '';

            if (!triggers.postBack && !triggers.ajaxRequest) {
                fnBody = 'return false;'
            } else if (triggers.postBack && triggers.ajaxRequest && (!triggers.items || triggers.items.length == 0)) {
                fnBody = 'return true;'
            } else {
                var requestBody = '';

                if (triggers.postBack)
                    requestBody += ' || data.request == "postback"';

                if (triggers.ajaxRequest)
                    requestBody += ' || data.request == "ajax"';

                var triggerBody = '';
                for (var i = 0; i < triggers.items.length; i++) {
                    var item = triggers.items[i];
                    if (item.type == 'ctrl' && item.target) {
                        triggerBody += ' || data.target == "' + item.target + '"';
                    }
                }

                fnBody = 'return (' + requestBody.substring(4) + ')';

                if (triggerBody)
                    fnBody += ' && (' + triggerBody.substring(4) + ')';
            }

            data.checkRequest = Function.apply(null, ['data', fnBody]);
        }

        container.start = function () {
            start(getData(this));
        };

        container.stop = function () {
            stop(getData(this));
        };

        container.disable = function (disable) {
            disable(getData(this), disable !== false);
        };

        container.enable = function (enable) {
            disable(getData(this), !(enable !== false));
        };

        container.isActive = function () {
            return getData(this).request != null;
        };

        container.getContext = function () {
            return getData(this).context;
        };

        render(settings, data);

        panels[settings.id] = data;

        data.$container.data('progress_inited', true);
    };

    function render(settings, data) {
        data.$header = null; {
            if (settings.title) {
                data.$header = $('<div>')
                    .addClass(cssClasses.header)
                    .text(settings.title);
            }
        }

        data.$footer = null; {
            if (settings.cancel) {
                data.cancel = settings.cancel;

                data.$footer = $('<div>').addClass(cssClasses.footer).append(
                    $('<a href="#cancel" data-action="cancel"><i class="fas fa-ban" style="margin-right:6px;"></i>Cancel</a>')
                        .addClass(cssClasses.btnDefault)
                );

                data.$footer.find('[data-action]').on('click', onFooterClick);
            }
        }

        var $body = $('<div>').addClass(cssClasses.body);

        data.items = []; {
            for (var i = 0; i < settings.items.length; i++) {
                var d = settings.items[i];
                if (d.type == 'pbar') {
                    var item = {
                        id: d.id,
                        type: d.type,
                        label: d.label === true,
                        $bar: $('<div class="progress-bar progress-bar-striped progress-bar-animated" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="0" style="width: 0%;">'),
                        $caption: null
                    };

                    if (d.label)
                        item.getLabel = getFormatFunction(d.label);

                    if (d.caption) {
                        item.getCaption = getFormatFunction(d.caption);
                        item.$caption = $('<div class="progress-caption"></div>');
                    }

                    $body.append(
                        $('<div class="progress">').append(
                            item.$bar
                        ),
                        item.$caption
                    );

                    data.items.push(item);
                } else if (d.type == 'status') {
                    var item = {
                        id: d.id,
                        type: d.type,
                        $text: $('<div class="progress-text">'),
                    };

                    if (d.text)
                        item.getText = getFormatFunction(d.text);

                    $body.append(
                        item.$text
                    );

                    data.items.push(item);
                }
            }
        }

        var width; {
            if (typeof settings.width == 'string')
                width = settings.width;
            else
                width = '450px';
        }

        data.$container.find('> div > div').append(
            $('<div>').addClass(cssClasses.container).css('max-width', width).append(
                data.$header,
                $body,
                data.$footer
            )
        );
    }

    function bind(data) {
        var dItem;

        if (data.request && data.request.dataItem) {
            dItem = data.request.dataItem;

            var dateNow = Date.now();
            var startedOn = parseInt(dItem.get('started_on'));
            if (isNaN(startedOn) || startedOn <= 0 || startedOn > dateNow)
                startedOn = data.request.startedOn;

            dItem.requestVariables['time_elapsed'] = dateDiffToString(dateNow - startedOn);
            dItem.requestVariables['running_ellipsis'] = Array(Math.floor(data.request.bindCounter % 8 / 2)).fill('.').join('');

            data.request.bindCounter += 1;
        } else {
            dItem = new BindItem();
        }

        for (var i = 0; i < data.items.length; i++) {
            var pItem = data.items[i];

            if (pItem.type == 'pbar') {
                var barData = dItem.item(pItem.id);

                if (barData == null) {
                    barData = {};

                    if (pItem.id)
                        dItem.contextItems[pItem.id] = barData;
                }

                if (typeof barData.value != 'number' || isNaN(barData.value) || barData.value < 0)
                    barData.value = 0;

                if (typeof barData.total != 'number' || isNaN(barData.total) || barData.total < 0)
                    barData.total = 0;

                if (typeof barData.percent != 'number' || isNaN(barData.percent) || barData.percent < 0)
                    barData.percent = 0;

                if (barData.value > barData.total)
                    barData.value = barData.total;

                if (barData.total > 0)
                    barData.percent = Math.floor(barData.value / barData.total * 100);

                var oldValue = parseInt(pItem.$bar.attr('aria-valuenow'));
                if (isNaN(oldValue))
                    oldValue = 0;

                if (barData.value >= oldValue)
                    pItem.$bar.css('transition', '');
                else
                    pItem.$bar.css('transition', 'none');

                pItem.$bar.attr('aria-valuemin', 0);
                pItem.$bar.attr('aria-valuemax', barData.total);
                pItem.$bar.attr('aria-valuenow', barData.value);
                pItem.$bar.css('width', String(barData.percent) + '%');

                if (pItem.getLabel)
                    pItem.$bar.html(pItem.getLabel(dItem, pItem.id));

                if (pItem.getCaption)
                    pItem.$caption.html(pItem.getCaption(dItem, pItem.id));
            } else if (pItem.type == 'status') {
                pItem.$text.html(pItem.getText(dItem, pItem.id));
            }
        }
    }

    function postBack(data, value) {
        if (!data.postBack)
            return;

        var inputs = document.getElementsByName(data.name);
        for (var i = 0; i < inputs.length; i++)
            inputs[i].remove();

        if (typeof value != 'string')
            value = '';

        var $input = $('<input type="hidden">').attr('name', data.name).val(value);

        data.$container.append($input);

        data.postBack();

        setTimeout(function () {
            $input.remove();
        });
    }

    function start(data) {
        if (data.request != null)
            return;

        var eArgs = {
            cancel: false
        };

        execCallback(data, 'pollStart', eArgs);

        if (eArgs.cancel === true)
            return;

        data.request = {
            startedOn: Date.now(),
            isActive: false,
            isStopped: false,
            isCancelled: false,
            dataItem: new BindItem(),
            bindHandler: null,
            requestHandler: null,
            noDataCount: 0,
            bindCounter: 0
        };

        bind(data);

        data.$container.show();
        data.request.bindHandler = setInterval(bind, 250, data);

        setTimeout(startRequest, 0, data);
    }

    function startRequest(data) {
        if (!data || !data.request || data.request.isActive == true)
            return;

        data.request.isActive = true;

        if (data.request.requestHandler != null) {
            clearTimeout(data.request.requestHandler);
            data.request.requestHandler = null;
        }

        $.ajax({
            type: 'GET',
            dataType: 'json',
            url: '/ProgressStatus.axd',
            cache: false,
            data: {
                context: data.context,
                cancelled: data.request.isCancelled
            },
            success: function (result) {
                if (typeof result != 'object' || !result.items && !result.variables) {
                    data.request.noDataCount += 1;
                    return;
                }

                data.request.noDataCount = 0;

                if (result.items)
                    data.request.dataItem.contextItems = result.items;

                if (result.variables)
                    data.request.dataItem.contextVariables = result.variables;

                if (result.complete === true)
                    data.request.isStopped = true;
            },
            error: function (xhr) {
                var eArgs = {
                    xhr: xhr,
                    stop: true
                };

                execCallback(data, 'pollError', eArgs);

                if (eArgs.stop !== true)
                    data.request.isStopped = true;

                console.error(xhr);
            },
            complete: function () {
                data.request.isActive = false;

                if (data.request.isStopped || data.request.noDataCount > 20) {
                    stop(data);
                    return;
                }

                var delay;

                if (data.request.noDataCount >= 12)
                    delay = 5000;
                else if (data.request.noDataCount >= 6)
                    delay = 2500;
                else if (data.request.noDataCount >= 4)
                    delay = 1000;
                else
                    delay = 500;

                setTimeout(startRequest, delay, data);
            },
        });
    }

    function stop(data) {
        if (data.request == null)
            return;

        data.request.isStopped = true;

        if (data.request.bindHandler != null) {
            clearInterval(data.request.bindHandler);
            data.request.bindHandler = null;
        }

        if (data.request.isActive)
            return;

        data.request = null;

        data.$container.hide();

        execCallback(data, 'pollStopped', {});
    }

    function disable(data, value) {
        data.isDisabled = value;
    }

    function onFooterClick(e) {
        var action = e.currentTarget.dataset.action;

        if (action == 'cancel') {
            e.preventDefault();
            e.stopPropagation();

            var data = getData(this);
            if (data.request == null)
                return;

            if (inSite.common.getObjByName('Sys.WebForms.PageRequestManager')) {
                var requestManager = Sys.WebForms.PageRequestManager.getInstance();
                if (requestManager.get_isInAsyncPostBack())
                    requestManager.abortPostBack();
            }

            if (data.cancel != 'Custom')
                window.location = inSite.common.updateQueryString('progressContext', data.context);

            if (data.cancel == 'PostBack')
                postBack(data, 'cancel');

            data.request.isCancelled = true;

            $(this).addClass("disabled");

            execCallback(data, 'cancelled', {});
        }
    }

    function onSubmitClick(e) {
        var $submitter = $(e.target).closest('button[name], input[type=submit][name]');
        if ($submitter.length != 0)
            $clickedSubmitter = $submitter;
        else
            $clickedSubmitter = null;
    }

    function onFormSubmit(e) {
        if (!this.__EVENTTARGET)
            return;

        var rData = {
            request: 'postback',
            target: null
        };

        if (!rData.target && e && e.originalEvent.submitter)
            rData.target = e.originalEvent.submitter.name;

        if ($clickedSubmitter) {
            if (!rData.target)
                rData.target = $clickedSubmitter.prop('name');

            $clickedSubmitter = null;
        }

        if (!rData.target)
            rData.target = this.__EVENTTARGET.value;

        if (rData.target)
            onRequestStart(rData);
    }

    function onBeginRequest(s, e) {
        var rData = {
            request: 'ajax',
            target: getRequestEventTarget(e.get_request()),
            updatePanels: e.get_updatePanelsToUpdate()
        };

        if (!rData.target)
            rData.target = s._postBackSettings.asyncTarget;

        onRequestStart(rData);
    }

    function onPageLoaded(s, e) {
        var updatedPanels = e.get_panelsUpdated();
        if (!updatedPanels || updatedPanels.length == 0)
            return;

        var rData = {
            target: null,
            updatePanels: []
        };

        if (s._postBackSettings.asyncTarget)
            rData.target = s._postBackSettings.asyncTarget;

        for (var x = 0; x < updatedPanels.length; x++) {
            var id = updatedPanels[x].id;
            if (!id)
                continue;

            for (var y = 0; y < s._updatePanelClientIDs.length; y++) {
                if (s._updatePanelClientIDs[y] === id) {
                    rData.updatePanels.push(s._updatePanelIDs[y]);
                    break;
                }
            }
        }

        onRequestEnd(rData);
    }

    function onEndRequest(s, e) {
        var rData = {
            target: getTargetFromRequest(),
            updatePanels: []
        };

        if (!rData.target && s._postBackSettings.asyncTarget)
            rData.target = s._postBackSettings.asyncTarget;

        if (rData.target) {
            if (onRequestEnd(rData) && !!e.get_error()) {
                var eArgs = {
                    alert: true,
                    error: e.get_error()
                };

                execCallback(data, 'submitError', eArgs);

                if (eArgs.alert !== false)
                    alert('An error occurred on the server side!');
            }
        }

        function getTargetFromRequest() {
            var response = e.get_response();
            if (response)
                return getRequestEventTarget(response.get_webRequest());

            return null;
        }
    }

    function onRequestStart(rData) {
        for (var n in panels) {
            if (!panels.hasOwnProperty(n))
                continue;

            var pData = panels[n];
            if (pData.isDisabled !== false || pData.request != null || !pData.checkRequest(rData))
                continue;

            {
                var eArgs = $.extend({ cancel: false }, rData);

                execCallback(pData, 'submit', eArgs);

                if (eArgs.cancel === true)
                    continue;
            }

            pData.startData = {};
            if (rData.target)
                pData.startData.target = rData.target;

            if (rData.updatePanels && rData.updatePanels.length > 0)
                pData.startData.updatePanels = rData.updatePanels;

            start(pData);
        }
    }

    function onRequestEnd(rData) {
        var isProcessed = false;

        for (var n in panels) {
            if (!panels.hasOwnProperty(n))
                continue;

            var pData = panels[n];
            if (pData.request == null)
                continue;

            var startData = pData.startData;
            var isMatch = !startData.target && !startData.updatePanels
                || !!rData.target && startData.target === rData.target;

            if (!isMatch && rData.updatePanels && rData.updatePanels.length > 0 && startData.updatePanels && startData.updatePanels.length > 0) {
                for (var x = 0; x < startData.updatePanels.length; x++) {
                    var panelId = startData.updatePanels[x];

                    for (var y = 0; y < rData.updatePanels.length; y++) {
                        if (rData.updatePanels[y] == panelId) {
                            isMatch = true;
                            break;
                        }
                    }

                    if (isMatch)
                        break;
                }
            }

            if (isMatch) {
                delete pData['startData'];

                stop(pData);

                isProcessed = true;
            }
        }

        return isProcessed;
    }

    function getData(el) {
        var id = $(el).closest('.progress-panel').prop('id');

        if (id && panels.hasOwnProperty(id))
            return panels[id];

        return null;
    }

    function getFormatFunction(text) {
        var body = 'return "' +
            text.replaceAll('\r', '\\r')
                .replaceAll('\n', '\\n')
                .replaceAll('"', '\\"')
                .replace(/(\{+)([a-zA-Z-_]+)(\}+)/g, function (match, open, name, close) {
                    if (open.length == 1 && close.length == 1)
                        return '" + item.get("' + name + '", id) + "'
                    else
                        return match;
                })
            + '";'

        return Function.apply(null, ['item', 'id', body]);
    }

    function defineClass(prototype) {
        var constructor = prototype.constructor;
        constructor.prototype = prototype;
        return constructor;
    }

    function dateDiffToString(value) {
        var hoursPerMs = 3600000;
        var minutesPerMs = 60000;
        var secondsPerMs = 1000;

        var hours = Math.floor(value / hoursPerMs);
        var minutes = Math.floor(value % hoursPerMs / minutesPerMs);
        var seconds = Math.floor(value % minutesPerMs / secondsPerMs);

        return String(hours).padStart(2, '0') + ':' + String(minutes).padStart(2, '0') + ':' + String(seconds).padStart(2, '0');
    }

    function getRequestEventTarget(request) {
        if (!request)
            return null;

        var body = request.get_body();
        if (!body)
            return null;

        var match = body.match(/(^|&)__EVENTTARGET=([^&]+)/);
        if (match && match.length == 3)
            return decodeURIComponent(match[2]);

        return null;
    }

    function execCallback(data, name, args) {
        if (!data.callbacks || !data.callbacks.hasOwnProperty(name))
            return;

        var callback = inSite.common.getObjByName(data.callbacks[name]);
        if (typeof callback == 'function')
            callback.call(data.$container[0], args);
    }
})();
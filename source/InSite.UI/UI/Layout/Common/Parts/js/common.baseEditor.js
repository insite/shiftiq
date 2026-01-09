(function () {
    var instance; {
        var inSite = window.inSite = window.inSite || {};
        var common = inSite.common = inSite.common || {};
        instance = common.baseEditor = common.baseEditor || {};
    }

    var count = 0;
    var items = {};

    instance.init = function (callback, options) {
        if (callback(options))
            return;

        if (count == 0)
            $(window).on('shown.bs.collapse shown.bs.tab shown.mdhtml.toggle shown.xeditable', onParentShown);

        if (!items.hasOwnProperty(options.id))
            count++;

        items[options.id] = {
            callback: callback,
            options: options
        };
    };

    function onParentShown() {
        if (count == 0)
            return;

        var removeIds = [];

        for (var id in items) {
            if (!items.hasOwnProperty(id))
                continue;

            var item = items[id];
            if (item.callback(item.options))
                removeIds.push(id);
        }

        for (var i = 0; i < removeIds.length; i++) {
            delete items[removeIds[i]];
            count--;
        }

        if (count == 0)
            $(window).off('shown.bs.collapse shown.bs.tab shown.mdhtml.toggle shown.xeditable', onParentShown);
    }
})();
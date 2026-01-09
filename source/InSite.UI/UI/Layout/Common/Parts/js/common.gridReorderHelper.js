(function () {
    var gridReorderHelper = inSite.common.gridReorderHelper = inSite.common.gridReorderHelper || {};

    // fields

    var isInited = false;
    var registeredItems = new Array();
    var itemId = null;

    // methods

    gridReorderHelper.init = function () {
        if (isInited)
            return;

        setActive(false);

        isInited = true;
    };

    gridReorderHelper.registerReorder = function (item) {
        if (inSite.common.stringHelper.isNullOrEmpty(item.id)
            || inSite.common.stringHelper.isNullOrEmpty(item.selector)
            || (inSite.common.stringHelper.isNullOrEmpty(item.updatePanelId) && inSite.common.stringHelper.isNullOrEmpty(item.callbackControlId))
            || getItem(item.id) != null)
            return;

        registeredItems.push(item);
    };

    gridReorderHelper.startReorder = function (id) {
        var item = getItem(id);
        if (item == null)
            return false;

        if (itemId != null) {
            gridReorderHelper.cancelReorder();

            setTimeout(function () { gridReorderHelper.startReorder(item.id); }, 500);
        } else {
            setActive(true);

            $(item.selector).sortable({
                forceHelperSize: true,
                items: typeof item.items === 'string' ? item.items : '> *',
                connectWith: item.connectWith,
                opacity: 0.8,
                start: onSortStart,
                stop: onSortStop
            }).disableSelection();

            var containerIndex = 0;
            $(item.selector).each(function () {
                var itemIndex = 0;
                $(this).find(item.items).each(function () {
                    $(this).attr('orderId', String(containerIndex) + ':' + String(itemIndex));
                    itemIndex++;
                });

                containerIndex++;
            });

            itemId = item.id;
        }

        return true;
    };

    gridReorderHelper.cancelReorder = function (disactivate) {
        if (itemId == null)
            return false;

        var item = getItem(itemId);
        if (item == null)
            return false;

        $(item.selector).disableSelection().sortable('destroy');

        doCallback(item, 'cancel-reorder');

        if (typeof disactivate === 'boolean' && disactivate)
            setActive(false);

        itemId = null;

        return true;
    };

    gridReorderHelper.saveReorder = function (disactivate) {
        if (itemId == null)
            return;

        var item = getItem(itemId);
        if (item == null)
            return;

        var data = '';

        $(item.selector).each(function () {
            var containerData = '';

            $(this).find(item.items).each(function () {
                var orderId = $(this).attr('orderId');
                if (typeof orderId != 'undefined')
                    containerData += orderId + ',';
            });

            data += (containerData.length > 1 ? containerData.substring(0, containerData.length - 1) : '') + ';';
        });

        if (data.length > 1) {
            $(item.selector).disableSelection().sortable('destroy');

            if (typeof disactivate === 'boolean' && disactivate)
                setActive(false);

            doCallback(item, 'save-reorder&' + data.substring(0, data.length - 1));
        } else {
            gridReorderHelper.cancelReorder(disactivate);
        }

        itemId = null;
    };

    gridReorderHelper.onSortStart = onSortStart;

    // event handlers

    function onSortStart(s, e) {
        e.placeholder.height(e.item.height());

        if (itemId == null)
            return;

        var item = getItem(itemId);
        if (item == null)
            return;

        refreshRows(item, [e.item, e.helper, e.placeholder]);

        if (typeof item.onStart === 'function')
            item.onStart(e.item, e.helper, e.placeholder, s.target);
    }

    function onSortStop(s, e) {
        refreshRows([e.placeholder]);

        if (itemId == null)
            return;

        var item = getItem(itemId);
        if (item == null)
            return;

        refreshRows(item, [e.item, e.helper, e.placeholder]);

        if (typeof item.onStop === 'function')
            item.onStop(e.placeholder);
    }

    // helpers

    function doCallback(item, value) {
        if (!inSite.common.stringHelper.isNullOrEmpty(item.updatePanelId)) {
            document.getElementById(item.updatePanelId).ajaxRequest(value);
        } else if (!inSite.common.stringHelper.isNullOrEmpty(item.callbackControlId)) {
            __doPostBack(item.callbackControlId, value);
        }
    }

    function refreshRows(item, excludeItems) {
        if (typeof item.updateItemStyle !== 'function')
            return;

        $(item.selector).each(function () {
            var index = 0;
            $(this).find(item.items).each(function () {
                var exclude = false;
                for (var y = 0; y < excludeItems.length; y++) {
                    if (this === excludeItems[y]) {
                        exclude = true;
                        break;
                    }
                }

                if (!exclude) {
                    item.updateItemStyle(this, index);

                    index++;
                }
            });
        });
    }

    function getItem(id) {
        if (id != null && typeof id == 'string' && id.length > 0) {
            var items = registeredItems;

            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                if (item.id === id)
                    return item;
            }
        }

        return null;
    }

    function setActive(active) {
        if (active) {
            $('.reorder-trigger').removeClass('reorder-inactive').addClass('reorder-active');
            $('.reorder-remove').remove();
        } else {
            $('.reorder-trigger').removeClass('reorder-active').addClass('reorder-inactive');
        }
    }
})();

$(document).ready(function () {
    inSite.common.gridReorderHelper.init();
});

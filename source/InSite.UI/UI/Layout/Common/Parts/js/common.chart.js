(function () {
    const instance = inSite.common.chart = inSite.common.chart || {};

    const _invisibleElements = [];
    const _initedCallbacks = {};

    $(function () { setTimeout(refreshInvisibleElements, 0); });
    $(window).one('load', function () { setTimeout(refreshInvisibleElements, 0); });

    instance.init = function (canvas, config) {
        if (!preInit(canvas))
            return;

        if (init(canvas, config) !== false)
            return;

        if (_invisibleElements.length == 0)
            $(document).on('shown.bs.collapse shown.bs.tab', refreshInvisibleElements);

        $(canvas).data('chart-config', config);

        _invisibleElements.push(canvas);
    }

    instance.onInited = function (id, fn) {
        if (typeof id != 'string' || id.length == 0 || typeof fn != 'function')
            return;

        const item = _initedCallbacks.hasOwnProperty(id) ? _initedCallbacks[id] : _initedCallbacks[id] = [];

        item.push(fn);
    };

    instance.showTooltips = function (chart, index, redraw) {
        const tooltipActive = chart.tooltip._active = chart.tooltip._active || [];

        let isUpdated = false;

        for (let x = 0; x < chart.data.datasets.length; x++) {
            const dataItem = chart.getDatasetMeta(x).data[index];

            let isExists = false;

            for (let y = 0; y < tooltipActive.length; y++) {
                if (dataItem._index == tooltipActive[y]._index) {
                    isExists = true;
                    break;
                }
            }

            if (isExists)
                continue;

            tooltipActive.push(dataItem);
            isUpdated = true;
        }

        if (isUpdated) {
            chart.tooltip.update(true);
            if (!chart.animating && redraw !== false)
                chart.draw();
        }
    };

    instance.hideTooltips = function (chart, index, redraw) {
        const tooltipActive = chart.tooltip._active;

        let isUpdated = false;

        if (tooltipActive instanceof Array && tooltipActive.length > 0) {
            for (let i = 0; i < tooltipActive.length; i++) {
                if (index == tooltipActive[i]._index) {
                    tooltipActive.splice(i, 1);
                    i--;
                    isUpdated = true;
                }
            }
        } else {
            isUpdated = true;
        }

        if (isUpdated) {
            chart.tooltip.update(true);
            if (!chart.animating && redraw !== false)
                chart.draw();
        }
    };

    instance.getLegendItemByXY = function (chart, x, y) {
        const hitBoxes = chart.legend.legendHitBoxes;

        if (hitBoxes instanceof Array && hitBoxes.length > 0) {
            for (let i = 0; i < hitBoxes.length; i++) {
                const hb = hitBoxes[i];
                if (y >= hb.top && y <= hb.top + hb.height && x >= hb.left && x <= hb.left + hb.width)
                    return chart.legend.legendItems[i];
            }
        }

        return null;
    };

    instance.getInstance = function (id) {
        const keys = Object.keys(Chart.instances);

        for (let i = 0; i < keys.length; i++) {
            const instance = Chart.instances[keys[i]];
            if (instance.canvas.id == id)
                return instance;
        }

        return null;
    };

    instance.breakLabel = function (text, maxLineLength, maxTextLength) {
        if (typeof text != 'string' || text.length == 0)
            return '';

        const words = text.split(' ');
        if (words.length <= 1)
            return text;

        const result = [];
        const hasMaxTextLength = typeof maxTextLength === 'number';

        let line = words[0];
        let resultLength = 0;

        for (let i = 1; i < words.length; i++) {
            const word = words[i];

            if (hasMaxTextLength && resultLength + word.length + line.length + 4 > maxTextLength) {
                result.push(line + '...');
                line = '';
                break;
            } else if (word.length + line.length + 1 > maxLineLength) {
                result.push(line);

                resultLength += line.length;

                line = word;
            } else {
                line += ' ' + word;
            }
        }

        if (line.length > 0)
            result.push(line);

        return result;
    };

    function preInit(canvas) {
        const chart = inSite.common.chart.getInstance(canvas.id);
        if (chart)
            chart.destroy();

        return document.contains(canvas);
    }

    function init(canvas, config) {
        const $canvas = $(canvas);

        if (!config && !(config = $canvas.data('chart-config')))
            return;

        if (!$canvas.is(':visible'))
            return false;

        const chart = new Chart(canvas, config);

        if (_initedCallbacks.hasOwnProperty(canvas.id)) {
            const callbacks = _initedCallbacks[canvas.id];

            delete _initedCallbacks[canvas.id];

            for (var i = 0; i < callbacks.length; i++)
                callbacks[i](canvas.id);
        }

        return chart;
    }

    function refreshInvisibleElements() {
        for (let i = 0; i < _invisibleElements.length; i++) {
            const canvas = _invisibleElements[i];

            if (!preInit(canvas) || init(canvas) !== false)
                _invisibleElements.splice(i--, 1);
        }

        if (_invisibleElements.length == 0)
            $(document).off('shown.bs.collapse shown.bs.tab', refreshInvisibleElements);
    }
})();

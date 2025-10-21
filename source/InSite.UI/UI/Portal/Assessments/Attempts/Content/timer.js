(function () {
    if (window.workerTimer)
        return;

    const instance = window.workerTimer = {};
    const timeouts = {
        id: 0,
        items: {},
        setAction: 'set-timeout',
        clearAction: 'clear-timeout',
    };
    const intervals = {
        id: 0,
        items: {},
        setAction: 'set-interval',
        clearAction: 'clear-interval',
    };

    function setTimer(fn, delay, args, container) {
        if (typeof fn != 'function')
            return;

        if (typeof delay != 'number' || isNaN(delay) || delay < 0)
            delay = 0;

        const id = container.id++;

        container.items[id] = { fn: fn, args: args };

        worker.postMessage({ action: container.setAction, id: id, delay: delay });

        return id;
    }

    function clearTimer(id, container) {
        if (typeof id != 'number' || isNaN(id) || id < 0 || id > container.id || !container.items.hasOwnProperty(id))
            return;

        worker.postMessage({ action: container.clearAction, id: id });

        delete container.items[id];
    }

    const worker = new Worker('/ui/portal/assessments/attempts/content/timer-worker.js');
    worker.addEventListener('message', function (evt) {
        const data = evt.data;
        const type = data.type;

        if (type == 'timeout') {
            if (!timeouts.items.hasOwnProperty(data.id))
                return;

            const timeout = timeouts.items[data.id];
            timeout.fn.apply(null, timeout.args);
            delete timeouts.items[data.id];
        } else if (type == 'interval') {
            if (!intervals.items.hasOwnProperty(data.id))
                return;

            const interval = intervals.items[data.id];
            interval.fn.apply(null, interval.args);
        }
    });

    instance.setTimeout = function (fn, delay) {
        return setTimer(fn, delay, Array.prototype.slice.call(arguments, 2), timeouts);
    };

    instance.getTimeoutCount = function () {
        return Object.keys(timeouts.items).length;
    };

    instance.clearTimeout = function (id) {
        return clearTimer(id, timeouts);
    };

    instance.setInterval = function (fn, delay) {
        return setTimer(fn, delay, Array.prototype.slice.call(arguments, 2), intervals);
    };

    instance.getIntervalCount = function () {
        return Object.keys(intervals.items).length;
    };

    instance.clearInterval = function (id) {
        return clearTimer(id, intervals);
    };
})();

const timeouts = {};
const intervals = {};

this.addEventListener('message', function (e) {
    const data = e.data;

    if (data.action == 'set-timeout') {
        timeouts[data.id] = setTimeout(onTimeout, data.delay, data.id);
    } else if (data.action == 'clear-timeout') {
        if (timeouts.hasOwnProperty(data.id)) {
            const id = timeouts[data.id];
            clearTimeout(id);
            delete timeouts[data.id];
        }
    } else if (data.action == 'set-interval') {
        intervals[data.id] = setInterval(onInterval, data.delay, data.id);
    } else if (data.action == 'clear-interval') {
        var id = intervals[data.id];
        clearInterval(id);
        delete intervals[data.id];
    }
});

function onTimeout(id) {
    this.postMessage({ type: 'timeout', id: id });
    delete timeouts[id];
}

function onInterval(id) {
    this.postMessage({ type: 'interval', id: id });
}

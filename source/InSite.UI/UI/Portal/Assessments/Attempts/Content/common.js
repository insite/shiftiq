(function () {
    const instance = window.attempts = window.attempts || {};

    const dataId = 'attempts' + String(Date.now());

    instance.initElement = function (el, namespace, func) {
        const data = getOrCreateData(el, namespace);
        if (data.inited === true)
            return;

        data.inited = true;

        func(el, data);
    };

    instance.getData = getData;

    function getData(obj, namespace) {
        if (!obj.hasOwnProperty(dataId))
            return null;

        result = obj[dataId];

        if (typeof namespace === 'string' && namespace.length > 0) {
            if (result.hasOwnProperty(namespace))
                return result[namespace];
        }

        return result;
    };

    function getOrCreateData(obj, namespace) {
        let result;

        if (!obj.hasOwnProperty(dataId)) {
            obj[dataId] = result = {};
        } else {
            result = obj[dataId];
        }

        if (typeof namespace === 'string' && namespace.length > 0) {
            if (result.hasOwnProperty(namespace))
                result = result[namespace];
            else
                result[namespace] = result = {};
        }

        return result;
    };
})();

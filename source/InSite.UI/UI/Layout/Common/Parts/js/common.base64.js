(function () {
    const instance = inSite.common.base64 = inSite.common.base64 || {};

    const indexToChar = '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-_'.split('');
    const charToIndex = (function () {
        const result = {};
        for (var i = 0; i < indexToChar.length; i++) {
            result[indexToChar[i]] = i;
        }
        return Object.freeze(result);
    })();

    instance.fromInt = function (value) {
        if (typeof value != 'number')
            return null;

        let result = '';

        while (true) {
            result = indexToChar[value & 0x3f] + result;

            value >>>= 6;

            if (value === 0)
                break;
        }

        return result;
    };

    instance.toInt = function (value) {
        if (typeof value != 'string')
            return null;

        const chars = value.split('');

        let result = 0;

        for (var i = 0; i < chars.length; i++) {
            result = (result << 6) + charToIndex[chars[i]];
        }

        return result;
    };
})();
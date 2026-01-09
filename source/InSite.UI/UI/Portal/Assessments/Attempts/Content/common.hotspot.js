(function () {
    if (!window.attempts)
        throw new Error('attempts is not initialized');

    if (window.attempts.hotspot)
        return;

    const instance = window.attempts.hotspot = {
        createImage: function (value) {
            return createImage(value);
        },
        createShapes: function (value) {
            return createShapes(value);
        },
        createPins: function (image, value) {
            return createPins(image, value);
        },
        createPin: function (image, data) {
            return createPin(image, data);
        },
        getClosestPinObj: function (obj) {
            return getClosestPinObj(obj);
        }
    };
    const pinRadius = 6;
    const iconFontFamily = '"Font Awesome 7 Pro"';
    const iconFontSize = 16;
    let iconFontItems = [];

    document.fonts.ready.then(function () {
        let isLoaded = false;

        document.fonts.forEach(function (v, k) {
            if (k.family == iconFontFamily && k.status == 'loaded')
                isLoaded = true;
        });

        if (!isLoaded)
            return;

        for (let i = 0; i < iconFontItems.length; i++)
            showIcon(iconFontItems[i]);

        iconFontItems = null;
    });

    function createImage(value) {
        const parts1 = value.split('|');
        if (parts1.length != 2)
            throwError();

        const parts2 = parts1[0].split('x');
        if (parts2.length != 2)
            throwError();

        const data = {
            src: parts1[1],
            width: parseInt(parts2[0]),
            height: parseInt(parts2[1])
        };

        if (data.src.length == 0 || isNaN(data.width) || isNaN(data.height) || data.width < 100 || data.height < 100)
            throwError();

        return inSite.common.hotspot.createImage(data, 1100, 800);

        function throwError() {
            throw new Error("Invalid image data");
        }
    }

    function createShapes(value) {
        if (value == null)
            return null;

        if (!(value instanceof Array) || value.length == 0)
            throw new Error("Invalid shapes array");

        const result = [];

        for (let i = 0; i < value.length; i++) {
            let shape = null;

            const parts = value[i].split(' ');
            if (parts.length > 1)
                shape = inSite.common.hotspot.getShape(i, parts[0], null, parts.slice(1));

            if (shape == null)
                throw new Error('Unexpected shape format: ' + value[i]);

            result.push(shape);
        }

        return result;
    }

    function createPins(image, value) {
        const result = [];

        if (value == null)
            return result;

        if (!(value instanceof Array))
            throw new Error("Invalid pins array");

        if (value.length == 0)
            return result;

        for (let i = 0; i < value.length; i++) {
            const data = {
                x: -1,
                y: -1,
                isCorrect: null
            };

            const parts = value[i].split(',');
            if (parts.length == 2) {
                data.x = parseInt(parts[0]);
                data.y = parseInt(parts[1]);
            } else if (parts.length == 3) {
                data.isCorrect = parseInt(parts[0]);
                if (isNaN(data.isCorrect) || data.isCorrect != 0 && data.isCorrect != 1)
                    throwError();
                else
                    data.isCorrect = data.isCorrect === 1;

                data.x = parseInt(parts[1]);
                data.y = parseInt(parts[2]);
            } else {
                throwError();
            }

            if (isNaN(data.x) || isNaN(data.y))
                throwError();

            const pin = createPin(image, data);
            if (pin.x < 0 || pin.x > 1 || pin.y < 0 || pin.y > 1)
                throwError();

            result.push(pin);

            function throwError() {
                throw new Error('Unexpected pin format: ' + value[i]);
            }
        }

        return result;
    }

    function createPin(image, data) {
        const group = new Konva.Group({
            name: 'pin',
        });
        group.add(new Konva.Circle({
            x: data.x,
            y: data.y,
            radius: pinRadius * 2
        }));
        group.add(new Konva.Circle({
            x: data.x,
            y: data.y,
            radius: pinRadius,
            strokeWidth: 2
        }));
        group.add(new Konva.Text({
            x: data.x,
            y: data.y,
            offsetX: iconFontSize / 2,
            offsetY: iconFontSize / 2,
            fontSize: iconFontSize,
            fontStyle: '400',
            visible: false
        }));

        const cover = group.children[1];
        const text = group.children[2];

        if (data.isCorrect === true) {
            cover.fill('#16c995');
            cover.stroke('white');

            text.text('\uf058');
            text.fill('#16c995');

            waitFontLoaded(group);
        } else if (data.isCorrect === false) {
            cover.fill('#f74f78');
            cover.stroke('white');

            text.text('\uf057');
            text.fill('#f74f78');

            waitFontLoaded(group);
        } else {
            cover.fill('white');
            cover.stroke('blue');
        }

        return {
            x: data.x / image.width,
            y: data.y / image.height,
            obj: group
        };
    }

    function getClosestPinObj(obj) {
        if (!(obj instanceof Konva.Node))
            return null;

        while (obj != null) {
            if (obj.getName() == 'pin')
                return obj;

            obj = obj.parent;
        }

        return null;
    }

    function waitFontLoaded(item) {
        if (iconFontItems !== null)
            iconFontItems.push(item);
        else
            showIcon(item);
    }

    function showIcon(item) {
        const base = item.children[0];
        const cover = item.children[1];
        const text = item.children[2];

        const newRadius = iconFontSize / 2 + iconFontSize * 0.04;

        base.radius(newRadius * 2);
        cover.radius(newRadius);
        cover.fill('white');
        text.fontFamily(iconFontFamily);
        text.visible(true);
    }
})();
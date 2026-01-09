(function () {
    const settings = {
        initElement: window.attempts.initElement,
        getData: window.attempts.getData,

        createImage: window.attempts.hotspot.createImage,
        createShapes: window.attempts.hotspot.createShapes,
        createPins: window.attempts.hotspot.createPins,
        createPin: window.attempts.hotspot.createPin,
        getClosestPinObj: window.attempts.hotspot.getClosestPinObj,

        namespace: 'answer:hotspot',
        selector: '.card-question > .card-body > .hotspot-image'
    };

    if (document.fonts)
        document.fonts.ready.then(function () {
            $(settings.selector).each(function () {
                const data = settings.getData(this, settings.namespace);
                if (!data?.konva)
                    return;

                const stage = data.konva.stage;
                if (stage)
                    stage.draw();
            });
        });

    $(window).on('resize', resizeAll).on('attempts:init', function () {
        $(settings.selector).each(function () {
            settings.initElement(this, settings.namespace, function (el, data) {
                const $container = $(el);

                data.inited = false;

                if (!data.konva || data.konva.container == null || !document.body.contains(data.konva.container)) {
                    $container.parents('.panel-group')
                        .off('shown.bs.tab', resizeAll)
                        .on('shown.bs.tab', resizeAll);

                    $container.parents('.tab-pane').each(function () {
                        $('[data-bs-target="#' + this.id + '"][data-bs-toggle]')
                            .off('shown.bs.tab', resizeAll)
                            .on('shown.bs.tab', resizeAll);
                    });

                    data.pinLimit = $container.data('pinLimit');
                    if (typeof data.pinLimit !== 'number' || isNaN(data.pinLimit) || data.pinLimit <= 0)
                        throw new Error("Invalid pinLimit");

                    data.image = settings.createImage($container.data('img'));
                    data.pins = settings.createPins(data.image, $container.data('pins'));
                    data.shapes = settings.createShapes($container.data('shapes'));

                    while (data.pins.length > data.pinLimit)
                        data.pins.shift();

                    data.konva = {
                        container: null,
                        stage: null,
                        layer: null
                    };

                    initKonva(el, data);

                    const textValue = data.pins.length > 0 ? '-1' : '';
                    $container.find('> input[type="text"]').val(textValue);
                } else {
                    data.image.updateSize();
                }
            });
        });
    });

    function writePins(data) {
        const $textInput = $(data.konva.stage.container().parentNode).find('input[type="text"]');
        if ($textInput.length != 1)
            throw new Error('Hotspot input not found');

        let textValue = '';

        for (let i = 0; i < data.pins.length; i++) {
            if (i > 0)
                textValue += ';';

            const pin = data.pins[i];

            textValue += pin.x.toFixed(17) + ',' + pin.y.toFixed(17);
        }

        $textInput.val(textValue).trigger('change');
    }

    function initKonva(el, data) {
        el.append(data.konva.container = document.createElement('div'))

        const stage = data.konva.stage = new Konva.Stage({
            container: data.konva.container,
            visible: false
        });
        stage.add(data.konva.layer = new Konva.Layer());

        stage.on('click tap', onStageClick);
        stage.on('scaled', () => onStageScaled(data.konva.layer));

        data.image.initKonva(data.konva.layer);

        if (data.shapes != null) {
            for (let i = 0; i < data.shapes.length; i++)
                data.shapes[i].initKonva(data.konva.layer);
        }

        if (data.pins.length > 0) {
            for (let i = 0; i < data.pins.length; i++)
                data.konva.layer.add(data.pins[i].obj);
        }

        onStageScaled(data.konva.layer);
    }

    function resizeAll() {
        $(settings.selector).each(function () {
            const data = settings.getData(this, settings.namespace);
            if (data?.konva)
                data.image.updateSize();
        });
    }

    function getData(obj) {
        return settings.getData(obj.container().parentNode, settings.namespace);
    }

    function onStageClick(e) {
        if (settings.getClosestPinObj(e.target) != null)
            return;

        const data = getData(e.currentTarget);
        const position = data.konva.stage.getRelativePointerPosition();
        const pin = settings.createPin(data.image, position);

        if (pin.x < 0 || pin.x > 1 || pin.y < 0 || pin.y > 1)
            return;

        data.pins.push(pin);
        data.konva.layer.add(pin.obj);

        while (data.pins.length > data.pinLimit) {
            data.pins.shift().obj.destroy();
        }

        writePins(data);
        onStageScaled(data.konva.layer);
    }

    function onStageScaled(layer) {
        const layerScale = layer.getAbsoluteScale().x;
        const objScale = {
            x: 1 / layerScale,
            y: 1 / layerScale
        };

        const pins = layer.find('.pin');
        for (let i = 0; i < pins.length; i++) {
            const pin = pins[i];
            for (let j = 0; j < pin.children.length; j++) {
                pin.children[j].scale(objScale);
            }
        }
    }
})();
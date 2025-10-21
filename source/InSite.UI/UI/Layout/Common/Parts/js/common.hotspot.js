(function () {
    if (inSite.common.hotspot)
        return;

    const instance = inSite.common.hotspot = {
        get shapeType() {
            return shapeType;
        },

        get settings() {
            return settings;
        },

        getShape(id, type, label, parameters) {
            if (type === shapeType.circle) {
                return new CircleShape(id, label, parameters);
            } else if (type === shapeType.rectangle) {
                return new RectangleShape(id, label, parameters);
            }

            throw new Error('Unexpected shape type: ' + String(type));
        },

        createImage(data, maxWidth, maxHeight) {
            const result = new HotspotImage(maxWidth, maxHeight);
            result.set(data);
            return result;
        },

        findShape(container, id) {
            if (!(container instanceof Konva.Container))
                return null;

            const obj = container.findOne('#shape' + String(id));
            if (obj) {
                const item = obj.getAttr(dataId);
                if (item)
                    return item;
            }

            return null;
        },

        getClosestShape(obj) {
            if (!(obj instanceof Konva.Node))
                return null;

            while (obj != null) {
                if (obj.getName() == 'shape') {
                    const item = obj.getAttr(dataId);
                    if (item)
                        return item;
                }

                obj = obj.parent;
            }

            return null;
        }
    };

    const dataId = 'shape' + String(Date.now());

    const shapeType = {
        get circle() {
            return 'CIRCLE';
        },
        get rectangle() {
            return 'RECTANGLE';
        },
    };

    const settings = (function () {
        const baseSize = 4;

        return Object.freeze({
            strokeWidth: baseSize,
            dash: [baseSize * 3, baseSize * 3],
            primaryColor: 'white',
            secondaryColor: '#296DE9',

            labelRadius: baseSize * 3,
            movePointRadius: baseSize * 2,
            minShapeSize: baseSize * 11,
        });
    })();

    const circleInterX = Math.sin(45 * Math.PI / 180);
    const circleInterY = Math.cos(45 * Math.PI / 180);

    class CircleShape {
        #id;
        #label;
        #parameters;

        #objLayer;
        #objRoot;
        #objBase;
        #objCover;
        #objLabel;

        constructor(id, label, params) {
            if (typeof id === 'undefined' || id == null)
                throw new Error('Undefined id');

            if (typeof label != 'string' || label.length == 0)
                label = null;

            params = parseParameters(params);

            if (params == null || params.length != 3)
                throw new Error('Invalid parameters array');

            this.#id = id;
            this.label = label;
            this.#parameters = params;
        }

        get id() {
            return this.#id;
        }

        get x() {
            return this.#objRoot ? this.#objRoot.x() : this.#parameters[0];
        }

        set x(value) {
            if (typeof value != 'number' || isNaN(value))
                throw new Error('Invalid value: ' + String(value));

            if (this.#objRoot)
                this.#objRoot.x(value);
            else
                this.#parameters[0] = value;
        }

        get y() {
            return this.#objRoot ? this.#objRoot.y() : this.#parameters[1];
        }

        set y(value) {
            if (typeof value != 'number' || isNaN(value))
                throw new Error('Invalid value: ' + String(value));

            if (this.#objRoot)
                this.#objRoot.y(value);
            else
                this.#parameters[1] = value;
        }

        get label() {
            return this.#label;
        }

        set label(value) {
            if (!this.#objLabel)
                this.#label = typeof value != 'string' || value.length == 0 ? null : value;
        }

        get radius() {
            return this.#objBase ? this.#objBase.radius() : this.#parameters[2];
        }

        set radius(value) {
            if (typeof value != 'number' || isNaN(value) || value <= 0)
                throw new Error('Invalid value: ' + String(value));

            if (this.#objRoot) {
                this.#objBase.radius(value);
                this.#objCover.radius(value);
            } else {
                this.#parameters[2] = value;
            }
        }

        position(value) {
            if (typeof value == 'undefined')
                return this.#objRoot ? this.#objRoot.position() : { x: this.#parameters[0], y: this.#parameters[1] };

            if (this.#objRoot) {
                this.#objRoot.position(value);
            } else {
                this.x = value.x;
                this.y = value.y;
            }
        }

        get type() {
            return shapeType.circle;
        }

        get rootObj() {
            return this.#objRoot;
        }

        get baseObj() {
            return this.#objBase;
        }

        initKonva(layer) {
            if (!(layer instanceof Konva.Layer))
                throw new Error('Invalid layer type');

            if (this.#objRoot)
                this.destroyKonva();

            this.#objLayer = layer;
            this.#objRoot = new Konva.Group({
                id: 'shape' + String(this.#id),
                name: 'shape',
                x: this.x,
                y: this.y
            });
            this.#objRoot.setAttr(dataId, this);

            this.#objRoot.add(this.#objBase = new Konva.Circle({
                name: 'base',
                radius: this.radius,
                stroke: settings.primaryColor
            }));
            this.#objRoot.add(this.#objCover = new Konva.Circle({
                name: 'cover',
                radius: this.radius,
                stroke: settings.secondaryColor,
                dash: settings.dash
            }));

            this.#objLayer.add(this.#objRoot);

            if (this.#label) {
                this.#objLabel = createLabel(this.#label);
                this.#objRoot.add(this.#objLabel);
                this.updateLabel();
            } else {
                this.#objLabel = null;
            }

            this.#objLayer.getStage().on('scaled', () => this.#onStageScaled());

            this.#onStageScaled();
        }

        destroyKonva() {
            if (!this.#objRoot)
                return;

            this.#objRoot.destroy();
            this.#objLayer = null;
            this.#objRoot = null;
            this.#objBase = null;
            this.#objCover = null;
            this.#objLabel = null;
        }

        updateLabel() {
            if (!this.#objLabel)
                return;

            const layer = this.#objLayer;
            const scale = layer.getAbsoluteScale();
            const objPos = this.#objRoot.position();
            const radius = this.#objBase.radius();
            const position = {
                x: 0,
                y: objPos.y > radius + settings.labelRadius ? -radius : radius
            };
            const sceneWidth = layer.width() / scale.x;

            if (objPos.x > sceneWidth - settings.labelRadius) {
                position.x = -radius * circleInterX;
                position.y = position.y * circleInterY;
            } else if (objPos.x < settings.labelRadius) {
                position.x = radius * circleInterX;
                position.y = position.y * circleInterY;
            }

            this.#objLabel.position(position);
        }

        parametersToString() {
            if (this.#objRoot) {
                const position = this.#objRoot.position();
                return parametersToString(position.x, position.y, this.#objBase.radius());
            } else {
                return parametersToString(this.#parameters[0], this.#parameters[1], this.#parameters[2]);
            }
        }

        #onStageScaled() {
            const layerScale = this.#objLayer.getAbsoluteScale();
            const objScale = 1 / layerScale.x;
            const strokeWidth = settings.strokeWidth * objScale;

            this.#objBase.strokeWidth(strokeWidth);
            this.#objCover.strokeWidth(strokeWidth);

            if (this.#objLabel)
                this.#objLabel.scale({
                    x: objScale,
                    y: objScale
                });
        }
    }

    class RectangleShape {
        #id;
        #label;
        #parameters;

        #objLayer;
        #objRoot;
        #objBase;
        #objCover;
        #objLabel;

        constructor(id, label, params) {
            if (typeof id === 'undefined' || id == null)
                throw new Error('Undefined id');

            if (typeof label != 'string' || label.length == 0)
                label = null;

            params = parseParameters(params);

            if (params == null || params.length != 4)
                throw new Error('Invalid parameters array');

            this.#id = id;
            this.label = label;
            this.#parameters = params;
        }

        get id() {
            return this.#id;
        }

        get x() {
            return this.#objRoot ? this.#objRoot.x() : this.#parameters[0];
        }

        set x(value) {
            if (typeof value != 'number' || isNaN(value))
                throw new Error('Invalid value: ' + String(value));

            if (this.#objRoot)
                this.#objRoot.x(value);
            else
                this.#parameters[0] = value;
        }

        get y() {
            return this.#objRoot ? this.#objRoot.x() : this.#parameters[1];
        }

        set y(value) {
            if (typeof value != 'number' || isNaN(value))
                throw new Error('Invalid value: ' + String(value));

            if (this.#objRoot)
                this.#objRoot.y(value);
            else
                this.#parameters[1] = value;
        }

        get label() {
            return this.#label;
        }

        set label(value) {
            if (!this.#objLabel)
                this.#label = typeof value != 'string' || value.length == 0 ? null : value;
        }

        get width() {
            return this.#objBase ? this.#objBase.width() : this.#parameters[2];
        }

        set width(value) {
            if (typeof value != 'number' || isNaN(value) || value <= 0)
                throw new Error('Invalid value: ' + String(value));

            if (this.#objRoot) {
                this.#objBase.width(value);
                this.#objCover.width(value);
            } else {
                this.#parameters[2] = value;
            }
        }

        get height() {
            return this.#objBase ? this.#objBase.height() : this.#parameters[3];
        }

        set width(value) {
            if (typeof value != 'number' || isNaN(value) || value <= 0)
                throw new Error('Invalid value: ' + String(value));

            if (this.#objRoot) {
                this.#objBase.height(value);
                this.#objCover.height(value);
            } else {
                this.#parameters[3] = value;
            }
        }

        position(value) {
            if (typeof value == 'undefined')
                return this.#objRoot ? this.#objRoot.position() : { x: this.#parameters[0], y: this.#parameters[1] };

            if (this.#objRoot) {
                this.#objRoot.position(value);
            } else {
                this.x = value.x;
                this.y = value.y;
            }
        }

        size(value) {
            if (typeof value == 'undefined')
                return this.#objBase ? this.#objBase.size() : { width: this.#parameters[2], height: this.#parameters[3] };

            if (this.#objRoot) {
                this.#objBase.size(value);
                this.#objCover.size(value);
            } else {
                this.width = value.width;
                this.height = value.height;
            }
        }

        get type() {
            return shapeType.rectangle;
        }

        get rootObj() {
            return this.#objRoot;
        }

        get baseObj() {
            return this.#objBase;
        }

        initKonva(layer) {
            if (!(layer instanceof Konva.Layer))
                throw new Error('Invalid layer type');

            if (this.#objRoot)
                this.destroyKonva();

            this.#objLayer = layer;
            this.#objRoot = new Konva.Group({
                id: 'shape' + String(this.#id),
                name: 'shape',
                x: this.x,
                y: this.y
            });
            this.#objRoot.setAttr(dataId, this);

            this.#objRoot.add(this.#objBase = new Konva.Rect({
                name: 'base',
                width: this.width,
                height: this.height,
                stroke: settings.primaryColor
            }));
            this.#objRoot.add(this.#objCover = new Konva.Rect({
                name: 'cover',
                width: this.width,
                height: this.height,
                stroke: settings.secondaryColor,
                dash: settings.dash

            }));

            this.#objLayer.add(this.#objRoot);

            if (this.#label) {
                this.#objLabel = createLabel(this.#label);
                this.#objRoot.add(this.#objLabel);
                this.updateLabel();
            }
            else {
                this.#objLabel = null;
            }

            this.#objLayer.getStage().on('scaled', () => this.#onStageScaled());

            this.#onStageScaled();
        }

        destroyKonva() {
            if (!this.#objRoot)
                return;

            this.#objRoot.destroy();
            this.#objLayer = null;
            this.#objRoot = null;
            this.#objBase = null;
            this.#objCover = null;
            this.#objLabel = null;
        }

        updateLabel() {
            if (!this.#objLabel)
                return;

            const objPos = this.#objRoot.position();
            const size = this.#objBase.size();

            this.#objLabel.position({
                x: size.width / 2,
                y: objPos.y > settings.labelRadius ? 0 : settings.labelRadius - objPos.y
            });
        }

        parametersToString() {
            if (this.#objRoot) {
                const position = this.#objRoot.position();
                const size = this.#objBase.size();
                return parametersToString(position.x, position.y, size.width, size.height);
            } else {
                return parametersToString(this.#parameters[0], this.#parameters[1], this.#parameters[2], this.#parameters[3]);
            }
        }

        #onStageScaled() {
            const layerScale = this.#objLayer.getAbsoluteScale();
            const objScale = 1 / layerScale.x;
            const strokeWidth = settings.strokeWidth * objScale;

            this.#objBase.strokeWidth(strokeWidth);
            this.#objCover.strokeWidth(strokeWidth);

            if (this.#objLabel)
                this.#objLabel.scale({
                    x: objScale,
                    y: objScale
                });
        }
    }

    class HotspotImage {
        #scale;
        #maxScale;
        #maxWidth;
        #maxHeight;

        #isChanged;
        #src;
        #width;
        #height;

        #imgObj;

        constructor(maxWidth, maxHeight) {
            if (typeof maxWidth != 'number' || isNaN(maxWidth) || maxWidth <= 0)
                throw new Error('Invalid maxWidth value: ' + String(maxWidth));

            if (typeof maxHeight != 'number' || isNaN(maxHeight) || maxHeight <= 0)
                throw new Error('Invalid maxHeight value: ' + String(maxHeight));

            this.#scale = -1;
            this.#maxScale = 1;
            this.#maxWidth = maxWidth;
            this.#maxHeight = maxHeight;

            this.#isChanged = false;
            this.#src = null;
            this.#width = null;
            this.#height = null;
        }

        get src() {
            return this.#src;
        }

        get width() {
            return this.#width;
        }

        get height() {
            return this.#height;
        }

        set(data) {
            if (this.#src == data?.src)
                return;

            if (data) {
                this.#src = data.src;
                this.#width = data.width;
                this.#height = data.height;

                const scaleX = this.#maxWidth / this.#width;
                const scaleY = this.#maxHeight / this.#height;

                this.#scale = -1;
                this.#maxScale = scaleX > scaleY ? scaleY : scaleX;

                if (this.#maxScale > 1)
                    this.#maxScale = 1;
            } else {
                this.#src = null;
                this.#width = null;
                this.#height = null;
                this.#scale = -1;
                this.#maxScale = 1;
            }

            this.#isChanged = true;

            if (this.#imgObj?.parent)
                this.initKonva(this.#imgObj.getLayer());
        }

        initKonva(layer) {
            if (!(layer instanceof Konva.Layer))
                throw new Error('Invalid layer type');

            const that = this;
            if (!that.#isChanged && that.#imgObj && that.#imgObj.getLayer() == layer)
                return;

            if (that.#imgObj) {
                that.#imgObj.destroy();
                that.#imgObj = null;
            }

            if (that.#src == null)
                return;

            let scale = that.#scale;
            if (scale === -1)
                scale = 1;

            const stage = layer.getStage();
            stage.size({
                width: that.#width * scale,
                height: that.#height * scale,
            });
            stage.scale({
                x: scale,
                y: scale
            });

            const imageDom = new Image();
            imageDom.onload = function () {
                layer.add(that.#imgObj = new Konva.Image({
                    x: 0,
                    y: 0,
                    width: that.#width,
                    height: that.#height,

                    image: this
                }));
                that.#imgObj.zIndex(0);
                that.updateSize();
            };
            imageDom.src = that.#src;

            that.#isChanged = false;
        }

        updateSize() {
            if (!this.#imgObj)
                return false;

            const stage = this.#imgObj.getStage();
            const container = stage.container();

            container.style.height = String(container.offsetHeight) + 'px';
            stage.hide();

            if (!$(container).is(':visible'))
                return false;

            let scale = container.offsetWidth / this.#width;
            if (scale > this.#maxScale)
                scale = this.#maxScale;

            if (scale == this.#scale) {
                container.style.height = '';
                stage.show();
                return true;
            }

            stage.size({
                width: this.#width * scale,
                height: this.#height * scale,
            });
            stage.scale({
                x: scale,
                y: scale
            });
            stage.fire('scaled')
            container.style.height = '';
            stage.show();

            this.#scale = scale;

            return true;
        }
    }

    function parseParameters(input) {
        if (!(input instanceof Array))
            return null;

        const result = [];

        for (let i = 0; i < input.length; i++) {
            const num = parseInt(input[i]);

            if (!isNaN(num))
                result.push(num);
            else
                return null;
        }

        return result;
    }

    function parametersToString() {
        let value = '';

        for (let i = 0; i < arguments.length; i++) {
            if (i > 0)
                value += ' ';

            value += String(Math.round(arguments[i]));
        }

        return value;
    }

    function createLabel(label) {
        const group = new Konva.Group({
            name: 'label',
        });

        group.add(new Konva.Circle({
            radius: settings.labelRadius,
            fill: settings.secondaryColor,
            stroke: settings.primaryColor,
            strokeWidth: 2,
        }));

        const text = new Konva.Text({
            text: label,
            fontSize: 14,
            fontFamily: 'Inter',
            fontStyle: '500',
            fill: settings.primaryColor
        });
        text.position({
            x: -text.width() / 2,
            y: -text.height() / 2,
        })
        group.add(text);

        return group;
    }
})();
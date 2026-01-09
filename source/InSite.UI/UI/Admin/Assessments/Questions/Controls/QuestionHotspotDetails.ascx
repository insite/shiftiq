<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionHotspotDetails.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Questions.Controls.QuestionHotspotDetails" %>

<asp:MultiView runat="server" ID="MultiView">
    <asp:View runat="server" ID="UploadInputView">
        <div class="card border-0 shadow-lg h-100">
            <div class="card-body d-flex flex-column">

                <h3 class="table-write-header">
                    Image
                </h3>

                <insite:Alert runat="server" ID="UploadStatus" />

                <div class="row">
                    <div class="col-6">
                        <insite:FileUploadV1 runat="server" ID="ImageUpload"
                            LabelText="Select and Upload Question Image:"
                            AllowedExtensions=".gif,.jpg,.jpeg,.png"
                            FileUploadType="Image"
                            OnClientFileUploaded="hotspotDetails.onQuestionImageUploaded"
                        />
                    </div>
                </div>

                <div class="mt-3">
                    <asp:Button runat="server" ID="ImageUploadedButton" CssClass="d-none" />
                    <insite:CancelButton runat="server" ID="UploadCancelButton" CausesValidation="false" />
                </div>

            </div>
        </div>
    </asp:View>
    <asp:View runat="server" ID="UploadConfirmView">
        <div class="card border-0 shadow-lg h-100">
            <div class="card-body d-flex flex-column">

                <h3 class="table-write-header">
                    Image
                </h3>

                <p>Are you sure you want to choose this image?</p>

                <div style="max-width:1100px;">
                    <img runat="server" alt="" ID="UploadConfirmImage" class="img-fluid" style="max-height:800px;" />
                </div>

                <div class="mt-3">
                    <insite:SaveButton runat="server" ID="UploadConfirmButton" Text="Confirm" />
                    <insite:CancelButton runat="server" ID="UploadConfirmCancelButton" CausesValidation="false" />
                </div>

            </div>
        </div>
    </asp:View>
    <asp:View runat="server" ID="ManageOptionsView">
        <div class="card border-0 shadow-lg h-100">
            <div class="card-body d-flex flex-column">

                <h3 class="table-write-header">
                    Image
                </h3>

                <div class="mb-3">
                    <insite:Button runat="server" ID="ChangeImageButton" Text="Change Image" Icon="far fa-image" ButtonStyle="Default" DisableAfterClick="true" GroupName="QuestionCommands" />

                    <insite:ButtonSpacer runat="server" />

                    <insite:Button runat="server" ID="AddRectangleOption2" Text="Add Rectangle" Icon="far fa-rectangle" ButtonStyle="Default" DisableAfterClick="true" GroupName="QuestionCommands" />
                    <insite:Button runat="server" ID="AddCircleOption2" Text="Add Circle" Icon="far fa-circle" ButtonStyle="Default" DisableAfterClick="true" GroupName="QuestionCommands" />
                </div>

                <div runat="server" id="QuestionImage">
                    <div style="width:1100px;height:800px;"></div>
                </div>
            </div>
        </div>

        <div class="card border-0 shadow-lg h-100 mt-3">
            <div class="card-body d-flex flex-column">

                <h3 class="table-write-header">
                    <span style="width:245px;">
                        <span style="width:110px;">Points</span>
                    </span>
                    Options
                </h3>

                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="OptionsUpdatePanel" />
                <insite:UpdatePanel runat="server" ID="OptionsUpdatePanel" Cssclass="form-group mb-3">
                    <ContentTemplate>
                        <asp:Repeater runat="server" ID="Repeater">
                            <HeaderTemplate>
                                <table id="<%# ClientID %>" class="table table-striped table-write-options table-write-hotspot"><tbody>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr data-id='<%# Eval("Sequence") %>'>
            
                                    <td class="text-end" style="width:65px;">
                                        <strong><%# Eval("Letter") %></strong>
                                        <insite:RequiredValidator runat="server" ID="TextRequiredValidator" FieldName="Option Text" ControlToValidate="OptionText" />
                                    </td>
                                    <td style="width:50px;">
                                        <span class="select-shape">
                                            <i class='<%# Eval("ShapeIcon", "far fa-{0} hotspot-icon-inactive") %>'></i>
                                            <i class='<%# Eval("ShapeIcon", "fas fa-{0} hotspot-icon-active") %>'></i>
                                        </span>
                                    </td>
                                    <td>
                                        <insite:TextBox runat="server" TranslationControl="OptionText" AllowHtml="true" />
                                        <div class="mt-1">
                                            <insite:EditorTranslation runat="server" ID="OptionText" Text='<%# Eval("Text") %>' ClientEvents-OnSetText="optionWriteRepeater.onSetTitleTranslation" ClientEvents-OnGetText="optionWriteRepeater.onGetTitleTranslation" />
                                        </div>
                                    </td>
                                    <td style="width:110px;">
                                        <insite:NumericBox runat="server" ID="Points" MinValue="0" MaxValue="999.99" ValueAsDecimal='<%# Eval("Points") %>' />
                                    </td>
                                    <td class="text-end" style="width:60px;">
                                        <span class="start-sort">
                                            <i class="fas fa-sort"></i>
                                        </span>
                                    </td>
                                    <td class="text-nowrap" style="width:30px;">
                                        <span style="line-height: 28px;">
                                            <insite:IconButton runat="server" CommandName="Delete" Name="trash-alt" ToolTip="Delete" Visible='<%# !(bool)Eval("IsReadOnly") %>' />
                                        </span>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </tbody></table>
                            </FooterTemplate>
                        </asp:Repeater>

                        <asp:HiddenField runat="server" ID="QuestionShapes" />
                        <asp:HiddenField runat="server" ID="ItemsOrder" ViewStateMode="Disabled" />

                        <div class="mt-3">
                            <insite:Button runat="server" ID="AddRectangleOption" Text="Add Rectangle" Icon="far fa-rectangle" ButtonStyle="Default" DisableAfterClick="true" GroupName="QuestionCommands" />
                            <insite:Button runat="server" ID="AddCircleOption" Text="Add Circle" Icon="far fa-circle" ButtonStyle="Default" DisableAfterClick="true" GroupName="QuestionCommands" />
                        </div>
                    </ContentTemplate>
                </insite:UpdatePanel>

            </div>
        </div>
    </asp:View>
</asp:MultiView>

<insite:PageHeadContent runat="server">
    <link rel="stylesheet" href="/UI/Admin/assessments/options/controls/write/style.css">

    <style type="text/css">
        table.table-write-hotspot > tbody > tr .select-shape {
            cursor: pointer;
            display: inline-block;
            width: 24px;
            text-align: center;
            line-height: 24px;
        }

            table.table-write-hotspot > tbody > tr .select-shape .hotspot-icon-inactive {
                display: inline-block;
            }

            table.table-write-hotspot > tbody > tr .select-shape .hotspot-icon-active {
                display: none;
            }

        table.table-write-hotspot.hotspot-selected > tbody > tr {
        }

            table.table-write-hotspot.hotspot-selected > tbody > tr.hotspot-selected {
                outline-style: dashed;
                outline-width: 2px;
            }

                table.table-write-hotspot.hotspot-selected > tbody > tr.hotspot-selected .select-shape .hotspot-icon-inactive {
                    display: none;
                }

                table.table-write-hotspot.hotspot-selected > tbody > tr.hotspot-selected .select-shape .hotspot-icon-active {
                    display: inline-block;
                }
    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">
<script src="/UI/Admin/assessments/options/controls/write/script.js"></script>
<script type="text/javascript">
    (function () {
        const instance = window.hotspotDetails = window.hotspotDetails || {};

        Sys.Application.add_load(function () {
            $('[data-actlike]')
                .off('click', onAlterButtonClick)
                .on('click', onAlterButtonClick);

            $('table.table-write-hotspot > tbody > tr .select-shape')
                .off('click', onSelectShape)
                .on('click', onSelectShape);
        });

        instance.onQuestionImageUploaded = function () {
            document.getElementById('<%= ImageUploadedButton.ClientID %>').click();
        };

        function onAlterButtonClick(e) {
            e.preventDefault();
            document.getElementById(this.dataset.actlike).click();
        }

        function onSelectShape(e) {
            const $row = $(this).closest('tr');

            const id = parseInt($row.data('id'));
            if (isNaN(id))
                return;

            if (instance.getSelectedShapeId() == id) {
                instance.deselectShape();
            } else {
                instance.selectShape(id);
            }
        }
    })();

    (function () {
        const instance = window.hotspotDetails = window.hotspotDetails || {};
        const shapesId = '<%= QuestionShapes.ClientID %>';
        const tableId = '<%= ClientID %>';
        const data = {
            image: inSite.common.hotspot.createImage(null, 1100, 800),
            konva: {
                container: null,
                stage: null,
                imageLayer: null,
                shapesLayer: null,
                moveLayer: null,
                shapes: null,
                select: null,
            }
        };
        const settings = inSite.common.hotspot.settings;
        const shapeType = inSite.common.hotspot.shapeType;

        let writeShapesHandler = null;

        Sys.Application.add_load(initKonva);

        $(window).on('resize', function () {
            data.image.updateSize();
        });

        if (document.fonts) {
            document.fonts.ready.then(function () {
                if (data.konva.stage)
                    data.konva.stage.draw();
            });
        }

        instance.drawImage = function (d) {
            data.image.set(d);
        };

        instance.selectShape = function (id) {
            if (data.konva.shapesLayer)
                selectShape(id);
        };

        instance.deselectShape = function () {
            deselectShape();
        };

        instance.getSelectedShapeId = function () {
            return data.konva.select == null ? null : data.konva.select.id;
        };

        function initKonva() {
            if (data.konva.container != null && document.body.contains(data.konva.container)) {
                let selectId = null;
                if (data.konva.select)
                    selectId = data.konva.select.id;

                deselectShape();
                drawShapes();

                if (selectId)
                    selectShape(selectId);

                return;
            }

            if (data.konva.stage != null) {
                data.konva.stage.destroy();
                data.konva.stage = null;
                data.konva.container = null;
                data.konva.select = null;
            }

            const container = data.konva.container = document.getElementById('<%= QuestionImage.ClientID %>');
            if (container == null)
                return;

            const stage = data.konva.stage = new Konva.Stage({
                container: container
            });
            stage.add(data.konva.imageLayer = new Konva.Layer());
            stage.add(data.konva.shapesLayer = new Konva.Layer());
            stage.add(data.konva.moveLayer = new Konva.Layer());

            stage.on('click tap', onStageClick);
            stage.on('scaled', onStageScaled)

            data.image.initKonva(data.konva.imageLayer);

            drawShapes();
        }

        function drawShapes() {
            const layer = data.konva.shapesLayer;
            if (layer != null)
                layer.destroyChildren();

            readShapes();

            const shapes = data.konva.shapes;
            for (let i = 0; i < shapes.length; i++) {
                const shape = shapes[i];

                shape.initKonva(layer);

                const obj = shape.rootObj;

                obj.draggable(true);

                obj.on('click tap', onShapeClick);
                obj.on('dragstart', onShapeDragStart);
                obj.on('dragmove', onShapeDragMove);
                obj.on('dragend', onShapeDragEnd);
                obj.on('mouseenter', onShapeMouseEnter);
                obj.on('mouseleave', onShapeMouseLeave);
            }
        }

        function readShapes() {
            const input = document.getElementById(shapesId);
            if (!input || input.value.length == 0) {
                data.konva.shapes = [];
                return;
            }

            const items = input.value.split('|');
            if (items.length == 0) {
                data.konva.shapes = null;
                return;
            }

            data.konva.shapes = [];

            for (let i = 0; i < items.length; i++) {
                let shape = null;

                const parts = items[i].split(' ');
                if (parts.length > 2) {
                    const id = parseInt(parts[0]);
                    if (!isNaN(id))
                        shape = inSite.common.hotspot.getShape(id, parts[1], inSite.common.stringHelper.toBase26(id - 1), parts.slice(2));
                }

                if (shape == null)
                    throw new Error('Unexpected shape format: ' + items[i]);

                data.konva.shapes.push(shape);
            }
        }

        function writeShapes(isLazy) {
            if (writeShapesHandler != null) {
                clearTimeout(writeShapesHandler);
                writeShapesHandler = null;
            }

            if (isLazy != false) {
                writeShapesHandler = setTimeout(writeShapes, 150, false);
                return;
            }

            const input = document.getElementById(shapesId);
            if (!input)
                return;

            let value = '';

            const shapes = data.konva.shapes;

            for (let x = 0; x < shapes.length; x++) {
                const shape = shapes[x];

                if (x > 0)
                    value += '|';

                value += String(shape.id) + ' ' + String(shape.type) + ' ' + shape.parametersToString();
            }

            input.value = value;
        }

        function createMovePoints() {
            const shape = data.konva.select;
            if (shape == null)
                return;

            const layer = data.konva.moveLayer;

            if (shape.type == shapeType.circle) {
                addPoints(2);
                layer.children[0].setAttr('movePoint', {
                    cursor: 'ew-resize',
                    position: 'left'
                });
                layer.children[1].setAttr('movePoint', {
                    cursor: 'ew-resize',
                    position: 'right'
                });
            } else if (shape.type == shapeType.rectangle) {
                addPoints(8);

                layer.children[0].fill(null).stroke(null).setAttr('movePoint', {
                    cursor: 'ns-resize',
                    position: 'top'
                });
                layer.children[1].setAttr('movePoint', {
                    cursor: 'nesw-resize',
                    position: 'top-right'
                });
                layer.children[2].setAttr('movePoint', {
                    cursor: 'ew-resize',
                    position: 'right'
                });
                layer.children[3].setAttr('movePoint', {
                    cursor: 'nwse-resize',
                    position: 'bottom-right'
                });
                layer.children[4].setAttr('movePoint', {
                    cursor: 'ns-resize',
                    position: 'bottom'
                });
                layer.children[5].setAttr('movePoint', {
                    cursor: 'nesw-resize',
                    position: 'bottom-left'
                });
                layer.children[6].setAttr('movePoint', {
                    cursor: 'ew-resize',
                    position: 'left'
                });
                layer.children[7].setAttr('movePoint', {
                    cursor: 'nwse-resize',
                    position: 'top-left'
                });
            }

            updateMovePoints();
            onStageScaled();

            function addPoints(count) {
                for (let i = 0; i < count; i++) {
                    const point = new Konva.Circle({
                        name: 'move-point',
                        radius: settings.movePointRadius,
                        fill: settings.primaryColor,
                        stroke: settings.secondaryColor,
                        strokeWidth: 1,
                        draggable: true
                    });

                    point.on('mouseenter', onMovePointMouseEnter);
                    point.on('mouseleave', onMovePointMouseLeave);
                    point.on('dragstart', onMovePointDragStart);
                    point.on('dragmove', onMovePointDragMove);
                    point.on('dragend', onMovePointDragEnd);

                    data.konva.moveLayer.add(point);
                }
            }
        }

        function updateMovePoints() {
            const shape = data.konva.select;
            if (shape == null)
                return;

            const layer = data.konva.moveLayer;
            const position = shape.position();

            if (shape.type == shapeType.circle) {
                const radius = shape.radius;

                layer.children[0].position({ // left
                    x: position.x - radius,
                    y: position.y
                });

                layer.children[1].position({ // right
                    x: position.x + radius,
                    y: position.y
                });
            } else if (shape.type == shapeType.rectangle) {
                const size = shape.size();
                const halfWidth = size.width / 2;
                const halfHeight = size.height / 2;

                layer.children[0].position({ // top
                    x: position.x + halfWidth,
                    y: position.y
                });

                layer.children[1].position({ // top-right
                    x: position.x + size.width,
                    y: position.y
                });

                layer.children[2].position({ // right
                    x: position.x + size.width,
                    y: position.y + halfHeight
                });

                layer.children[3].position({ // bottom-right
                    x: position.x + size.width,
                    y: position.y + size.height
                });

                layer.children[4].position({ // bottom
                    x: position.x + halfWidth,
                    y: position.y + size.height
                });

                layer.children[5].position({ // bottom-left
                    x: position.x,
                    y: position.y + size.height
                });

                layer.children[6].position({ // left
                    x: position.x,
                    y: position.y + halfHeight
                });

                layer.children[7].position({ // top-left
                    x: position.x,
                    y: position.y
                });
            }
        }

        function selectShape(obj) {
            const shape = typeof obj == 'number'
                ? inSite.common.hotspot.findShape(data.konva.shapesLayer, obj)
                : inSite.common.hotspot.getClosestShape(obj);
            const isShapeNull = shape == null;
            const isSelectNull = data.konva.select == null;
            const isChanged = isShapeNull != isSelectNull
                || !isShapeNull && !isSelectNull && shape != data.konva.select;

            if (!isChanged)
                return;

            deselectShape();

            if (isShapeNull)
                return;

            shape.rootObj.zIndex(data.konva.shapesLayer.children.length - 1);
            shape.baseObj.fill('rgba(255,255,255,0.25)');

            data.konva.select = shape;

            createMovePoints();

            $(document.getElementById(tableId))
                .addClass('hotspot-selected')
                .find('> tbody > tr[data-id="' + String(shape.id) + '"]')
                .addClass('hotspot-selected');
        }

        function deselectShape() {
            if (data.konva.select == null)
                return;

            data.konva.select.baseObj.fill(null);
            data.konva.select = null;
            data.konva.moveLayer.destroyChildren();
            $(document.getElementById(tableId)).removeClass('hotspot-selected').find('> tbody > tr.hotspot-selected').removeClass('hotspot-selected');
        }

        // event handlers

        function onStageClick(e) {
            const shape = inSite.common.hotspot.getClosestShape(e.target);
            if (shape == null)
                deselectShape();
        }

        function onStageScaled() {
            const shape = data.konva.select;
            if (shape == null)
                return;

            const layer = data.konva.moveLayer;
            const layerScale = layer.getAbsoluteScale().x;
            const pointScale = 1 / layerScale;
            const objScale = {
                x: pointScale,
                y: pointScale
            };
            const isRectangle = shape.type == shapeType.rectangle;

            const points = layer.find('.move-point');
            for (let i = 0; i < points.length; i++) {
                const point = points[i];
                
                point.scale(objScale);

                if (isRectangle) {
                    const position = point.getAttr('movePoint').position;
                    if (position == 'top' || position == 'right' || position == 'bottom' || position == 'left')
                        point.visible(layerScale > 0.75);
                }
            }
        }

        function onShapeClick(e) {
            selectShape(e.currentTarget);
        }

        function onShapeDragStart(e) {
            selectShape(e.currentTarget);
            data.konva.moveLayer.opacity(0.5);
        }

        function onShapeDragMove(e) {
            const shape = data.konva.select;
            const obj = e.currentTarget;
            const objPosition = obj.position();
            const imgWidth = data.image.width;
            const imgHeight = data.image.height;

            if (shape.type == shapeType.circle) {
                if (objPosition.x < 0)
                    obj.x(0);
                else if (objPosition.x > imgWidth)
                    obj.x(imgWidth);

                if (objPosition.y < 0)
                    obj.y(0);
                else if (objPosition.y > imgHeight)
                    obj.y(imgHeight);
            } else if (shape.type == shapeType.rectangle) {
                const objSize = obj.children[0].size();

                if (objPosition.x < 0)
                    obj.x(0);
                else if (objPosition.x > imgWidth - objSize.width)
                    obj.x(imgWidth - objSize.width);

                if (objPosition.y < 0)
                    obj.y(0);
                else if (objPosition.y > imgHeight - objSize.height)
                    obj.y(imgHeight - objSize.height);
            }

            shape.updateLabel();
            updateMovePoints();
        }

        function onShapeDragEnd(e) {
            data.konva.moveLayer.opacity(1);
            writeShapes();
        }

        function onShapeMouseEnter(e) {
            data.konva.stage.container().style.cursor = 'move';
        }

        function onShapeMouseLeave() {
            data.konva.stage.container().style.cursor = 'default';
        }

        function onMovePointMouseEnter(e) {
            data.konva.stage.container().style.cursor = e.currentTarget.getAttr('movePoint').cursor;
        }

        function onMovePointMouseLeave(e) {
            data.konva.stage.container().style.cursor = 'default';
        }

        function onMovePointDragStart(e) {
            e.currentTarget.getAttr('movePoint').originalPosition = e.currentTarget.position();

            const children = data.konva.moveLayer.children;
            for (let i = 0; i < children.length; i++) {
                const child = children[i];
                if (child != e.currentTarget)
                    child.opacity(0.35);
            }
        }

        function onMovePointDragMove(e) {
            const shape = data.konva.select;
            if (shape == null)
                return;

            const point = e.currentTarget;
            const pointData = point.getAttr('movePoint');

            if (pointData.cursor == 'ns-resize') {
                point.x(pointData.originalPosition.x);
            } else if (pointData.cursor == 'ew-resize') {
                point.y(pointData.originalPosition.y);
            }

            const shapePosition = shape.position();
            const pointPosition = pointData.position;
            const imgWidth = data.image.width;
            const imgHeight = data.image.height;

            if (shape.type == shapeType.circle) {
                const shapeRadius = shape.radius;

                let leftX = shapePosition.x - shapeRadius;
                let rightX = shapePosition.x + shapeRadius;

                if (pointPosition == 'left') {
                    leftX = point.x();

                    const diameter = rightX - leftX;

                    if (rightX < leftX || diameter < settings.minShapeSize)
                        point.x(leftX = rightX - settings.minShapeSize);
                    else if (diameter > imgWidth * 1.5)
                        point.x(leftX = rightX - imgWidth * 1.5);

                    if (leftX < -shapeRadius)
                        point.x(leftX = -shapeRadius);
                    else if (leftX > imgWidth - shapeRadius)
                        point.x(leftX = imgWidth - shapeRadius);


                } else if (pointPosition == 'right') {
                    rightX = point.x();

                    const diameter = rightX - leftX;

                    if (rightX < leftX || diameter < settings.minShapeSize)
                        point.x(rightX = leftX + settings.minShapeSize);
                    else if (diameter > imgWidth * 1.5)
                        point.x(rightX = leftX + imgWidth * 1.5);

                    if (rightX < shapeRadius)
                        point.x(rightX = shapeRadius);
                    else if (rightX > imgWidth + shapeRadius)
                        point.x(rightX = imgWidth + shapeRadius);
                }

                shape.radius = (rightX - leftX) / 2;
                shape.x = leftX + shape.radius;
                shape.updateLabel();
            } else if (shape.type == shapeType.rectangle) {
                const isTop = pointPosition == 'top-left' || pointPosition == 'top' || pointPosition == 'top-right';
                const isBottom = pointPosition == 'bottom-left' || pointPosition == 'bottom' || pointPosition == 'bottom-right';
                const isLeft = pointPosition == 'top-left' || pointPosition == 'left' || pointPosition == 'bottom-left';
                const isRight = pointPosition == 'top-right' || pointPosition == 'right' || pointPosition == 'bottom-right';

                const shapeSize = shape.size();

                let topY = shapePosition.y;
                let bottomY = shapePosition.y + shapeSize.height;
                let leftX = shapePosition.x;
                let rightX = shapePosition.x + shapeSize.width;

                if (isTop) {
                    topY = point.y();
                    if (topY < 0)
                        point.y(topY = 0);
                    else if (bottomY - topY < settings.minShapeSize)
                        point.y(topY = bottomY - settings.minShapeSize);
                } else if (isBottom) {
                    bottomY = point.y();
                    if (bottomY > imgHeight)
                        point.y(bottomY = imgHeight);
                    else if (bottomY - topY < settings.minShapeSize)
                        point.y(bottomY = topY + settings.minShapeSize);
                }

                if (isLeft) {
                    leftX = point.x();
                    if (leftX < 0)
                        point.x(leftX = 0);
                    else if (rightX - leftX < settings.minShapeSize)
                        point.x(leftX = rightX - settings.minShapeSize);
                } else if (isRight) {
                    rightX = point.x();
                    if (rightX > imgWidth)
                        point.x(rightX = imgWidth);
                    else if (rightX - leftX < settings.minShapeSize)
                        point.x(rightX = leftX + settings.minShapeSize);
                }

                shapeSize.width = rightX - leftX;
                shapeSize.height = bottomY - topY;

                shape.position({ x: leftX, y: topY });
                shape.size(shapeSize);
                shape.updateLabel();

                updateMovePoints();
            }
        }

        function onMovePointDragEnd(e) {
            e.currentTarget.getAttr('movePoint').originalPosition = null;

            const children = data.konva.moveLayer.children;
            for (let i = 0; i < children.length; i++)
                children[i].opacity(1);

            const pointerPos = data.konva.stage.getPointerPosition();
            const pointerObj = data.konva.stage.getIntersection(pointerPos);
            if (pointerObj != e.currentTarget)
                data.konva.stage.container().style.cursor = 'default';

            writeShapes();
        }
    })();
</script>
</insite:PageFooterContent>

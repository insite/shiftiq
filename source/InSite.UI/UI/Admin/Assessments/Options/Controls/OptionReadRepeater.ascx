<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OptionReadRepeater.ascx.cs" Inherits="InSite.Admin.Assessments.Options.Controls.OptionReadRepeater" %>

<asp:MultiView runat="server" ID="MultiView">

    <asp:View runat="server" ID="TrueOrFalseOptionView">
        <asp:Repeater runat="server" ID="TrueOrFalseOptionRepeater">
            <HeaderTemplate>
                <table class="table-option-read">
                    <%# GetOptionRepeaterTableHead("<th></th><th></th>", "<th></th>") %>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# GetTrueOrFalseOptionIcon((decimal)Eval("Points")) %></td>
                    <td><%# Eval("Letter") %>.</td>
                    <%# GetOptionRepeaterTitle(Container) %>
                    <td class="form-text option-points">
                        &bull; <%# GetOptionPoints((decimal)Eval("Points")) %>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate></tbody></table></FooterTemplate>
        </asp:Repeater>
    </asp:View>

    <asp:View runat="server" ID="SingleCorrectOptionView">
        <asp:Repeater runat="server" ID="SingleCorrectOptionRepeater">
            <HeaderTemplate>
                <table class="table-option-read">
                    <%# GetOptionRepeaterTableHead("<th></th><th></th>", "<th></th>") %>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# GetSingleCorrectOptionIcon((decimal)Eval("Points")) %></td>
                    <td><%# Eval("Letter") %>.</td>
                    <%# GetOptionRepeaterTitle(Container) %>
                    <td class="form-text option-points">
                        &bull; <%# GetOptionPoints((decimal)Eval("Points")) %>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate></tbody></table></FooterTemplate>
        </asp:Repeater>
    </asp:View>

    <asp:View runat="server" ID="MultipleCorrectOptionView">
        <asp:Repeater runat="server" ID="MultipleCorrectOptionRepeater">
            <HeaderTemplate>
                <table class="table-option-read">
                    <%# GetOptionRepeaterTableHead("<th></th><th></th>", null) %>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# GetMultipleCorrectOptionIcon((bool?)Eval("IsTrue")) %></td>
                    <td><%# Eval("Letter") %>.</td>
                    <%# GetOptionRepeaterTitle(Container) %>
                    <td class="form-text option-points">
                        &bull; <%# GetOptionPoints((decimal)Eval("Points")) %>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate></tbody></table></FooterTemplate>
        </asp:Repeater>
    </asp:View>

    <asp:View runat="server" ID="ComposedRubricView">
        <asp:Repeater runat="server" ID="ComposedRubricOptionRepeater">
            <HeaderTemplate>
                <table class="table-option-read">
                    <%# GetOptionRepeaterTableHead("<th></th>", "<th></th>") %>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("Letter") %>.</td>
                    <%# GetOptionRepeaterTitle(Container) %>
                    <td class="option-points form-text">
                        &bull; <%# GetOptionPoints((decimal)Eval("Points")) %>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate></tbody></table></FooterTemplate>
        </asp:Repeater>
    </asp:View>

    <asp:View runat="server" ID="BooleanTableOptionView">
        <asp:Repeater runat="server" ID="BooleanTableOptionRepeater">
            <HeaderTemplate>
                <table class="table-option-read">
                    <thead>
                        <tr>
                            <th></th>
                            <%# GetOptionRepeaterTableHeadTitleCols() %>
                            <th style="width:30px;" title="True"><i class='far fa-check'></i></th>
                            <th style="width:30px;" title="False"><i class='far fa-times'></i></th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("Letter") %>.</td>
                    <%# GetOptionRepeaterTitle(Container) %>
                    <td title="True"><%# GetBooleanTableOptionIcon((bool?)Eval("IsTrue"), true) %></td>
                    <td title="False"><%# GetBooleanTableOptionIcon((bool?)Eval("IsTrue"), false) %></td>
                    <td class="form-text option-points">
                        &bull; <%# GetOptionPoints((decimal)Eval("Points")) %>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate></tbody></table></FooterTemplate>
        </asp:Repeater>
    </asp:View>

    <asp:View runat="server" ID="MatchingView">
        <asp:Repeater runat="server" ID="MatchingPairsRepeater">
            <HeaderTemplate>
                <h5 class="p-1 bg-secondary">Matching Pairs</h5>
                <table class="table table-condensed table-read-matching-pairs"><tbody>
            </HeaderTemplate>
            <FooterTemplate></tbody></table></FooterTemplate>
            <ItemTemplate>
                <tr>
                    <td class="w-50"><%# Eval("Left") %></td>
                    <td class="w-50"><%# Eval("Right") %></td>
                    <td class="form-text pair-points">
                        &bull; <%# GetOptionPoints((decimal)Eval("Points")) %>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>

        <asp:Repeater runat="server" ID="MatchingDistractorsRepeater">
            <HeaderTemplate>
                <h5 class="p-1 bg-secondary">Matching Distractors</h5>
                <table class="table table-condensed"><tbody>
            </HeaderTemplate>
            <FooterTemplate></tbody></table></FooterTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("Value") %></td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </asp:View>

    <asp:View runat="server" ID="LikertView">
        <table class="table-likert-read" style="visibility:hidden;">
            <thead>
                <asp:Repeater runat="server" ID="LikertColumnRepeater1">
                    <HeaderTemplate>
                        <tr>
                            <td style="width:1px;" rowspan="2"></td>
                            <td rowspan="2"></td>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <td><%# Eval("Letter") %>.</td>
                    </ItemTemplate>
                    <FooterTemplate>
                        </tr>
                    </FooterTemplate>
                </asp:Repeater>
                
                <asp:Repeater runat="server" ID="LikertColumnRepeater2">
                    <HeaderTemplate>
                        <tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <td><%# ConvertToHtml((string)Eval("Content.Title.Default")) %></td>
                    </ItemTemplate>
                    <FooterTemplate>
                        </tr>
                    </FooterTemplate>
                </asp:Repeater>
            </thead>
            <tbody>
                <asp:Repeater runat="server" ID="LikertRowRepeater">
                    <ItemTemplate>
                        <tr>
                            <td><%# (int)Eval("Index") + 1 %>.</td>
                            <td><%# ConvertToHtml((string)Eval("Content.Title.Default")) %></td>
                            <asp:Repeater runat="server" ID="LikertOptionRepeater">
                                <ItemTemplate>
                                    <td class="likert-option"><%# Eval("Points") %></td>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </asp:View>

    <asp:View runat="server" ID="HotspotView">
        <div runat="server" id="HotspotImage" class="mb-3">
        </div>

        <asp:Repeater runat="server" ID="HotspotOptionRepeater">
            <HeaderTemplate>
                <table class="table-option-read">
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr data-id='<%# Eval("Letter") %>'>
                    <td><%# GetSingleCorrectOptionIcon((decimal)Eval("Points")) %></td>
                    <td><%# Eval("Letter") %>.</td>
                    <td><%# ConvertToHtml((string)Eval("Content.Title.Default")) %></td>
                    <td class="form-text option-points">
                        &bull; <%# GetOptionPoints((decimal)Eval("Points")) %>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate></tbody></table></FooterTemplate>
        </asp:Repeater>

    </asp:View>

    <asp:View runat="server" ID="OrderingView">
        <div runat="server" id="OrderingTopLabel" class="mb-3" visible="false"></div>

        <asp:Repeater runat="server" ID="OrderingSolutionRepeater">
            <HeaderTemplate>
                <table class="table-option-read">
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr data-id='<%# Eval("Letter") %>'>
                    <td><%# Eval("Letter") %>.</td>
                    <td>
                        <asp:Repeater runat="server" ID="OptionRepeater">
                            <HeaderTemplate><div></HeaderTemplate>
                            <FooterTemplate></div></FooterTemplate>
                            <ItemTemplate>
                                <div class="bg-white border rounded py-2 px-3 mb-3">
                                    <div class="mb-1 fw-bold">Option <%# Eval("Sequence") %></div>
                                    <div><%# Eval("Html") %></div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </td>
                    <td class="form-text option-points">
                        &bull; <%# GetOptionPoints((decimal)Eval("Points")) %>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate></tbody></table></FooterTemplate>
        </asp:Repeater>

        <div runat="server" id="OrderingBottomLabel" class="mt-3" visible="false"></div>
    </asp:View>

</asp:MultiView>

<insite:PageHeadContent runat="server" ID="CommonStyle">
    <style type="text/css">
        table.table-likert-read {
            width: 100%;
        }

        table.table-option-read,
        table.table-likert-read {
            margin-left: 10px;
        }

            table.table-option-read img,
            table.table-option-read iframe,
            table.table-likert-read img,
            table.table-likert-read iframe {
                max-width: 100%;
            }

            table.table-option-read td,
            table.table-likert-read > tbody td {
                vertical-align: top;
                padding: 5px;
            }

                table.table-option-read td p {
                    margin-bottom: 0.5rem !important;
                }

            table.table-option-read th,
            table.table-likert-read > thead td {
                vertical-align: bottom;
                padding: 5px;
            }

            table.table-likert-read > thead td {
                text-align: center;
            }

                table.table-likert-read > thead td p {
                    margin-bottom: 0 !important;
                }

            table.table-option-read .option-points,
            table.table-read-matching-pairs .pair-points {
                padding-top: 8px;
                padding-left: 0;
                white-space: nowrap;
            }

            table.table-likert-read > tbody > tr > td {
                vertical-align: middle;
            }

                table.table-likert-read > tbody > tr > td p {
                    margin-bottom: 0;
                }

                table.table-likert-read > tbody > tr > td.likert-option {
                    text-align: center;
                    border-color: #ddd;
                    border-style: dashed;
                    border-width: 1px;
                }

            table.table-option-read .no-right-pad {
                padding-right: 0 !important;
            }

            table.table-option-read .no-left-pad {
                padding-left: 0 !important;
            }

            table.table-option-read .small-right-pad {
                padding-right: 0.25em !important;
            }

            table.table-option-read .small-left-pad {
                padding-left: 0.25em !important;
            }

            table.table-option-read .x-wide {
                padding-right: 5em !important;
            }

            table.table-option-read.hotspot-selected > tbody > tr {
                opacity: 0.6;
            }

                table.table-option-read.hotspot-selected > tbody > tr.hotspot-selected {
                    opacity: 1;
                }

                    table.table-option-read.hotspot-selected > tbody > tr.hotspot-selected > td > i.far.fa-check-circle,
                    table.table-option-read.hotspot-selected > tbody > tr.hotspot-selected > td > i.far.fa-times-circle {
                        font-weight: 900;
                    }

    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server" ID="CommonScript">
    <script type="text/javascript">
        (function () {
            $('table.table-likert-read').each(function () {
                const $this = $(this);

                $this.parents('.tab-pane').each(function () {
                    $('[data-bs-target="#' + this.id + '"][data-bs-toggle]')
                        .off('shown.bs.tab', update)
                        .on('shown.bs.tab', update);
                });
            });

            $(window).on('resize', update);

            Sys.Application.add_load(function () {
                update();
            });

            update();

            function update() {
                $('table.table-likert-read').each(function () {
                    const $table = $(this);

                    let data = $table.data('likert');
                    if (!data)
                        $table.data('likert', data = {});

                    inSite.common.likert.update($table, data);
                });
            }
        })();
    </script>

    <script type="text/javascript">
        (function () {
            const instance = window.hotspotView = window.hotspotView || {};
            const data = {};

            instance.set = function (d) {
                if (!data.hasOwnProperty(d.id)) {
                    const item = {
                        containerId: d.id,
                        image: inSite.common.hotspot.createImage(d, 1100, 800),
                        shapes: [],
                        konva: {
                            container: null,
                            stage: null,
                            layer: null
                        }
                    };

                    for (let i = 0; i < d.shapes.length; i++) {
                        let shape = null;

                        const parts = d.shapes[i].split(' ');
                        if (parts.length > 2)
                            shape = inSite.common.hotspot.getShape(parts[0], parts[1], parts[0], parts.slice(2));

                        if (shape == null)
                            throw new Error('Unexpected shape format: ' + d.shapes[i]);

                        item.shapes.push(shape);
                    }

                    data[item.containerId] = item;
                }

                if (data.hasOwnProperty(d.id))
                    initKonva(data[d.id]);
            };

            $(window).on('resize', resizeAll);

            Sys.Application.add_load(function () {
                for (let id in data) {
                    if (data.hasOwnProperty(id))
                        initKonva(data[id]);
                }
            });

            if (document.fonts) {
                document.fonts.ready.then(function () {
                    for (let id in data) {
                        if (!data.hasOwnProperty(id))
                            continue;
                        const stage = data[id].konva.stage
                        if (stage)
                            stage.draw();
                    }
                });
            }

            function resizeAll() {
                for (let id in data) {
                    if (data.hasOwnProperty(id))
                        data[id].image.updateSize();
                }
            }

            function initKonva(item) {
                if (item.konva.container != null && document.body.contains(item.konva.container))
                    return;

                const container = item.konva.container = document.getElementById(item.containerId);
                if (container == null)
                    return;
                
                $(container)
                    .data('hotspot', item)
                    .parents('.tab-pane').each(function () {
                        $('[data-bs-target="#' + this.id + '"][data-bs-toggle]')
                            .off('shown.bs.tab', resizeAll)
                            .on('shown.bs.tab', resizeAll);
                    })
                    .end()
                    .next('table.table-option-read').find('> tbody > tr[data-id]')
                    .off('mouseenter', onRowMouseEnter)
                    .off('mouseleave', onRowMouseLeave)
                    .on('mouseenter', onRowMouseEnter)
                    .on('mouseleave', onRowMouseLeave);

                const stage = item.konva.stage = new Konva.Stage({
                    container: container,
                    visible: false
                });
                stage.add(item.konva.layer = new Konva.Layer());

                item.image.initKonva(item.konva.layer);

                drawShapes(item);
            }

            function drawShapes(item) {
                const layer = item.konva.layer;
                const shapes = item.shapes;
                for (let i = 0; i < shapes.length; i++) {
                    const shape = shapes[i];
                    shape.initKonva(layer);
                    shape.rootObj.on('mouseenter', onShapeMouseEnter);
                    shape.rootObj.on('mouseleave', onShapeMouseLeave);
                }
            }

            function onShapeMouseEnter(e) {
                const shape = inSite.common.hotspot.getClosestShape(e.currentTarget);
                shape.baseObj.fill('rgba(255,255,255,0.25)');

                $(shape.rootObj.getStage().container())
                    .next('table.table-option-read').addClass('hotspot-selected')
                    .find('> tbody > tr[data-id="' + String(shape.id) + '"]')
                    .addClass('hotspot-selected');
            }

            function onShapeMouseLeave(e) {
                const shape = inSite.common.hotspot.getClosestShape(e.currentTarget);
                shape.baseObj.fill(null);

                $(shape.rootObj.getStage().container())
                    .next('table.table-option-read').removeClass('hotspot-selected')
                    .find('> tbody > tr.hotspot-selected')
                    .removeClass('hotspot-selected');
            }

            function onRowMouseEnter() {
                const $this = $(this);
                const id = $this.data('id');
                
                if (id) {
                    const item = $this
                        .siblings('tr').removeClass('hotspot-selected').end()
                        .addClass('hotspot-selected')
                        .closest('table').addClass('hotspot-selected')
                        .prev('div').data('hotspot');

                    for (let i = 0; i < item.shapes.length; i++) {
                        const shape = item.shapes[i];
                        const fillColor = shape.id === id ? 'rgba(255,255,255,0.25)' : null;
                        shape.baseObj.fill(fillColor);
                    }
                } else {
                    onRowMouseLeave.call(this, arguments);
                }
            }

            function onRowMouseLeave() {
                const item = $(this)
                    .siblings('tr').removeClass('hotspot-selected').end()
                    .closest('table').removeClass('hotspot-selected')
                    .prev('div').data('hotspot');

                for (let i = 0; i < item.shapes.length; i++) {
                    const shape = item.shapes[i];
                    shape.baseObj.fill(null);
                }
            }
        })();
    </script>
</insite:PageFooterContent>
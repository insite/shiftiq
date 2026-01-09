<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LikertTableColumnGrid.ascx.cs" Inherits="InSite.Admin.Workflow.Forms.Questions.Controls.LikertTableColumnGrid" %>

<insite:PageHeadContent runat="server">
<style type="text/css">

    .dragtable-sortable {
        list-style-type: none !important;
        margin: 0 !important;
        padding: 0 !important;
        -moz-user-select: none !important;
    }

        .dragtable-sortable li {
            margin: 0 !important;
            padding: 0 !important;
            float: left !important;
            font-size: 1em !important;
        }

        .dragtable-sortable th,
        .dragtable-sortable td {
            border-left: 0px;
        }

        .dragtable-sortable li:first-child th,
        .dragtable-sortable li:first-child td {
            border-left: 1px solid #cccccc;
        }

    .dragtable-sortable .ui-sortable-helper {
        opacity: 0.7;
    }

    .dragtable-sortable .ui-sortable-placeholder {
        border: 0 none;
        border-bottom: 1px solid #cccccc;
        border-top: 1px solid #cccccc;
        background: #efefef !important;
        visibility: visible !important;
    }

        .dragtable-sortable .ui-sortable-placeholder * {
            opacity: 0.0;
            visibility: hidden;
        }

    div.survey-likert-column-grid-container {
        overflow-x: auto;
        overflow-y: visible;
        padding: 0 0 0 70px !important;
        width: 100%;
        height: 250px;
        position: relative;
        margin-left:10px;
    }

    table.survey-likert-column-grid {
        z-index: 999;
        border: none;
        border-collapse: collapse;
    }

        table.survey-likert-column-grid th,
        table.survey-likert-column-grid td {
            padding: 4px 8px;
        }

            table.survey-likert-column-grid th.col-head,
            table.survey-likert-column-grid td.col-head {
                border-bottom: 1px solid #d8d9da;
                text-align: center;
                height: 40px;
            }
            
            table.survey-likert-column-grid th.col-foot,
            table.survey-likert-column-grid td.col-foot {
                border-top: 1px solid #d8d9da;
                text-align: center;
                height: 40px;
            }

        table.survey-likert-column-grid > thead > tr > th.col-drag > span {
            cursor: move;
            display: block;
            height: 21px;
            margin: 0 auto;
            width: 20px;
        }

</style>
</insite:PageHeadContent>

<div id="<%= ClientID %>">
    <div class="row mb-3">
        <div class="col-md-4">
            <insite:Button runat="server" ID="NewColumnButton" Icon="fas fa-plus-circle" Text="New Column" ButtonStyle="Default" />
        </div>
        <div class="col-md-4">
            <insite:TextBox runat="server" ID="HighestRatingText" MaxLength="100" EmptyMessage="Highest Rating" />
        </div>
        <div class="col-md-4">
            <insite:TextBox runat="server" ID="LowestRatingText" MaxLength="100" EmptyMessage="Lowest Rating" />
        </div>
    </div>

    <div runat="server" id="NoColumns" class="alert alert-warning" role="alert" style="display:none;">
        Click <b>New Column</b> button to add a new column
    </div>

    <div runat="server" id="Container" style="display:none;">
        <table class="survey-likert-column-grid" style="position:absolute; z-index:1001; border-collapse:collapse; background-color:#fff;">
            <thead>
                <tr><th class="col-head"><div style='line-height:20px;'>&nbsp;</div></th></tr>
            </thead>
            <tbody>
                <tr><td><div style='line-height:43.33px;'>Text</div></td></tr>
                <tr><td><div style='line-height:43.33px;'>Points</div></td></tr>
                <tr><td><div style='line-height:43.33px;'>Category</div></td></tr>
            </tbody>
            <tfoot>
                <tr><td class="col-foot"><div>&nbsp;</div></td></tr>
            </tfoot>
        </table>

        <div class="survey-likert-column-grid-container">
            <table class="survey-likert-column-grid">
                <thead>
                    <tr>
                        <td class="col-head"><div>&nbsp;</div></td>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td></td>
                    </tr>
                    <tr>
                        <td></td>
                    </tr>
                    <tr>
                        <td></td>
                    </tr>
                </tbody>
                <tfoot>
                    <tr>
                        <td class="col-foot"></td>
                    </tr>
                </tfoot>
            </table>
        </div>
    </div>

    <asp:Panel runat="server" ID="ViewOnlyMessage" Width="780px" Visible="false">
        <div class="form-text">Columns cannot be added or removed because submissions to this form have been submitted.</div>
    </asp:Panel>

    <asp:HiddenField runat="server" ID="StateInput" />
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            var instance = window.likertTableColumnGrid = window.likertTableColumnGrid|| {};

            instance.setLanguage = function (lang) {
                setLanguage(lang);
            };

            var $container = $('div#<%= Container.ClientID %>');
            var $viewport = $container.find('.survey-likert-column-grid-container');
            var $table = $viewport.find('> table');
            var $stateInput = $('input#<%= StateInput.ClientID %>');
            var $noColumns = $('#<%= NoColumns.ClientID %>');

            var state = JSON.parse($stateInput.val());

            { // init
                $('#<%= NewColumnButton.ClientID %>').on('click', onAddItemClick);
                $('#<%= ClientID %> [data-content]').on('change', onContentChanged);

                $(window).on('resize', refreshWidth);

                $container.parents('.tab-pane').each(function () {
                    $('[data-bs-target="#' + this.id + '"][data-bs-toggle]').on('shown.bs.tab', refreshWidth);
                });

                $container.parents('.panel-collapse').on('shown.bs.collapse', refreshWidth);

                setLanguage(state.lang);

                $noColumns.show();
                $container.hide();

                for (var i = 0; i < state.items.length; i++) {
                    addItem(state.items[i]);
                }

                refreshWidth();
                startDragAndDrop();
            }

            // event handlers

            function onAddItemClick(e) {
                e.preventDefault();

                stopDragAndDrop();

                var item = {
                    id: -1,
                    text: {},
                    points: 0,
                    category: '',
                };

                addItem(item);

                state.items.push(item);

                onStateUpdated();

                refreshWidth();

                {
                    var container = $viewport.get(0);
                    container.scrollLeft = container.scrollWidth;
                }

                startDragAndDrop();
            }

            function onRemoveItemClick(e) {
                e.preventDefault();

                var index = getItemIndex(this);
                if (index === null)
                    return;

                if (!confirm('Are you sure you want to delete this column?'))
                    return;

                stopDragAndDrop();

                var table = $table[0];
                for (var i = 0; i < table.rows.length; i++) {
                    table.rows[i].cells[index].remove();
                }

                state.items.splice(index, 1);

                onStateUpdated();

                if (state.items.length === 0) {
                    $noColumns.show();
                    $container.hide();
                }

                refreshWidth();
                startDragAndDrop();
            }

            function onStateUpdated() {
                $stateInput.val(JSON.stringify(state));
            }

            function onTextChanged() {
                updateItem(this, function ($input, item) {
                    var lang = state.lang;
                    if (!lang)
                        lang = 'en';

                    item.text[lang] = $input.val();
                });
            }

            function onPointsChanged() {
                updateItem(this, function ($input, item) {
                    var value = parseFloat($input.val());
                    if (isNaN(value))
                        value = 0;

                    item.points = value;
                });
            }

            function onCategoryChanged() {
                updateItem(this, function ($input, item) {
                    item.category = $input.val();
                });
            }

            function onContentChanged() {
                var $input = $(this);

                var name = $input.data('content');

                if (!state.content.hasOwnProperty(name))
                    state.content[name] = {};

                state.content[name][state.lang] = $input.val();

                onStateUpdated();
            }

            // methods

            function setLanguage(language) {
                if (language) {
                    state.lang = language;
                    onStateUpdated();
                } else {
                    language = state.lang;
                }

                $('#<%= ClientID %> [data-content]').each(function () {
                    var $input = $(this).val('');
                    var name = $input.data('content');

                    if (state.content.hasOwnProperty(name)) {
                        var item = state.content[name];
                        if (item.hasOwnProperty(language)) {
                            $input.val(item[language]);
                        }
                    }
                });

                $('#<%= ClientID %> [data-item-text]').each(function () {
                    var $input = $(this);
                    var index = getItemIndex($input);
                    if (index === null)
                        return false;

                    $input.val('');

                    var text = state.items[index].text;
                    if (text.hasOwnProperty(language)) {
                        $input.val(text[language]);
                    }
                });
            }

            function addItem(item) {
                var text = '';
                var points = 0;
                var category = '';

                if (item.text.hasOwnProperty(state.lang))
                    text = item.text[state.lang];

                if (item.hasOwnProperty('points'))
                    points = item.points;

                if (item.hasOwnProperty('category'))
                    category = item.category;

                var $textInput = $('<input type="text" class="insite-text form-control" style="width:120px;">')
                    .attr('data-item-text', 1)
                    .on('change', onTextChanged)
                    .val(text);
                var $pointsInput = $('<input type="text" class="insite-numeric form-control" style="width:120px;">')
                    .on('change', onPointsChanged)
                    .data('settings', { decimals: 2, min: 0, max: 100 })
                    .val(points);
                var $categoryInput = $('<input type="text" class="insite-text form-control" maxlength="90" style="width:120px;">')
                    .on('change', onCategoryChanged)
                    .val(category);

                {
                    var table = $table[0];
                    var lastCellIndex = table.tHead.rows[0].cells.length - 1;

                    $('<th class="col-drag col-head"><span title="Drag &amp; Drop Column"><i title="Drag &amp; Drop Row" class="icon fas fa-exchange-alt x20"></i></span></th>')
                        .insertBefore(table.tHead.rows[0].cells[lastCellIndex]);

                    $('<td>').append($textInput).insertBefore(table.tBodies[0].rows[0].cells[lastCellIndex]);
                    $('<td>').append($pointsInput).insertBefore(table.tBodies[0].rows[1].cells[lastCellIndex]);
                    $('<td>').append($categoryInput).insertBefore(table.tBodies[0].rows[2].cells[lastCellIndex]);

                    $('<td class="col-foot">').append(
                        $('<a title="Delete Column" data-action="delete" href="javascript:void(0);"><i class="icon fas fa-trash-alt x20"></i></a>').on('click', onRemoveItemClick)
                    ).insertBefore(table.tFoot.rows[0].cells[lastCellIndex]);

                    inSite.common.baseInput.init($textInput);
                    inSite.common.baseInput.init($pointsInput);
                    inSite.common.baseInput.init($categoryInput);
                }

                if ($container.is(':hidden')) {
                    $noColumns.hide();
                    $container.show();
                }
            }

            function updateItem(el, upd) {
                var $el = $(el)
                var index = getItemIndex($el);
                if (index !== null) {
                    upd($el, state.items[index]);
                    onStateUpdated();
                }
            }

            function getItemIndex(el) {
                var cellIndex = null;

                var $cell = $(el).closest('td');
                if ($cell.length === 1)
                    cellIndex = $cell.get(0).cellIndex;

                if (typeof cellIndex !== 'number' || cellIndex < 0 || cellIndex >= state.items.length) {
                    alert('Error: invalid control state.');
                    return null;
                }

                return cellIndex;
            }

            function refreshWidth() {
                var table = $table[0];
                if (table.offsetWidth <= 0)
                    return;

                var lastCell = table.rows[0].cells[table.rows[0].cells.length - 1];
                var widthControl = lastCell.getElementsByTagName('div')[0];

                if (widthControl) {
                    var additionalTableWidth = $viewport.width() - (table.offsetWidth - widthControl.offsetWidth);

                    if (additionalTableWidth > 0) {
                        widthControl.style.width = additionalTableWidth + 'px';
                    } else {
                        widthControl.style.width = '0px';
                    }
                }
            }

            function startDragAndDrop() {
                $table.dragtable({
                    dragHandle: '> thead > tr > th.col-drag > span',
                    exact: false,
                    beforeStart: function () {
                        var $columns = $table.find('> thead > tr > th.col-drag');
                        if ($columns.length !== state.items.length) {
                            alert('Error: invalid control state.');
                            return false;
                        }

                        $columns.each(function (i) {
                            $(this).data('index', i);
                        });
                    },
                    beforeStop: function () {
                        var items = [];

                        $table.find('> thead > tr > th.col-drag').each(function (i) {
                            var index = $(this).data('index');

                            $(this).data('index', null);

                            if (index < 0 || index >= state.items.length)
                                return;

                            items.push(state.items[index]);
                        });

                        if (items.length !== state.items.length) {
                            alert('Error: invalid control state.');
                            return false;
                        }

                        state.items = items;

                        onStateUpdated();
                    }
                });
            }

            function stopDragAndDrop() {
                $table.dragtable('destroy');
            }
        })();
    </script>
</insite:PageFooterContent>


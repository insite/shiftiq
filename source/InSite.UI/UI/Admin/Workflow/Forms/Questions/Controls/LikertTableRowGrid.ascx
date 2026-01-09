<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LikertTableRowGrid.ascx.cs" Inherits="InSite.Admin.Workflow.Forms.Questions.Controls.LikertTableRowGrid" %>

<div class="mb-3">
    <insite:Button runat="server" ID="NewRowButton" Icon="fas fa-plus-circle" Text="New Row" ButtonStyle="Default" />
</div>

<div runat="server" id="NoRows" class="alert alert-warning" role="alert" style="display:none;">
    Click <b>New Row</b> button to add a new row
</div>

<table id='<%= ClientID %>' class="table table-striped survey-likert-row-grid" style="display:none;">
    <thead>
        <tr>
            <th style="width:35px;"></th>
            <th style="width:35px;">#</th>
            <th>Title</th>
            <th style="width:215px;">Category</th>
            <th style="width:35px;"></th>
        </tr>
    </thead>
    <tbody>
    </tbody>
</table>

<asp:HiddenField runat="server" ID="StateInput" />

<insite:PageHeadContent runat="server">
    <style type="text/css">
        table.survey-likert-row-grid {
        }

            table.survey-likert-row-grid > tbody > tr > td.cell-icon {
                line-height: 30px;
            }

            table.survey-likert-row-grid > tbody > tr > td.cell-text {
                line-height: 34px;
            }

            table.survey-likert-row-grid > tbody > tr > td span.drag-control {
                cursor: move;
                display: block;
                height: 21px;
                margin: 0 auto;
                width: 20px;
            }

            table.survey-likert-row-grid > tbody > tr.ui-sortable-helper {
                border: 1px solid #666666;
            }

            table.survey-likert-row-grid > tbody > tr.ui-sortable-placeholder {
                visibility: visible !important;
                outline: 1px dashed #666666 !important;
            }

    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            var instance = window.likertTableRowGrid = window.likertTableRowGrid || {};

            instance.setLanguage = function (lang) {
                setLanguage(lang);
            };

            var $table = $('#<%= ClientID %>');
            var $stateInput = $('input#<%= StateInput.ClientID %>');
            var $noRows = $('#<%= NoRows.ClientID %>');

            var state = JSON.parse($stateInput.val());

            { // init
                $('#<%= NewRowButton.ClientID %>').on('click', onAddItemClick);

                $noRows.show();
                $table.hide();

                for (var i = 0; i < state.items.length; i++) {
                    addItem(state.items[i]);
                }

                startDragAndDrop();
            }

            // event handlers

            function onAddItemClick(e) {
                e.preventDefault();

                stopDragAndDrop();

                var item = {
                    id: -1,
                    title: {},
                    category: '',
                };

                addItem(item);
                state.items.push(item);

                onStateUpdated();
                startDragAndDrop();
            }

            function onRemoveItemClick(e) {
                e.preventDefault();

                var $row = $(this).closest('tr');
                var item = $row.data('item');
                if (!item)
                    return;

                var index = state.items.indexOf(item);
                if (index == -1)
                    return;

                if (!confirm('Are you sure you want to delete this row?'))
                    return;

                stopDragAndDrop();

                $row.remove();
                state.items.splice(index, 1);

                onStateUpdated();

                if (state.items.length === 0) {
                    $noRows.show();
                    $table.hide();
                }

                startDragAndDrop();
            }

            function onStateUpdated() {
                $stateInput.val(JSON.stringify(state, function (key, value) {
                    if (key === '$row')
                        return undefined;

                    return value;
                }));
            }

            function onTitleChanged() {
                updateItem(this, function ($input, item) {
                    var lang = state.lang;
                    if (!lang)
                        lang = 'en';

                    item.title[lang] = $input.val();
                });
            }

            function onCategoryChanged() {
                updateItem(this, function ($input, item) {
                    item.category = $input.val();
                });
            }

            // methods

            function setLanguage(language) {
                if (language) {
                    state.lang = language;
                    onStateUpdated();
                } else {
                    language = state.lang;
                }

                $('#<%= ClientID %> [data-content="title"]').each(function () {
                    var $input = $(this);
                    var item = $input.closest('tr').data('item');

                    var title = '';
                    if (item.title.hasOwnProperty(language))
                        title = item.title[language];

                    $input.val(title);
                });
            }

            function addItem(item) {
                var sequence = '';
                var title = '';
                var category = '';

                if (item.hasOwnProperty('id') && item.id >= 0)
                    sequence = String(item.id + 1);

                if (item.title.hasOwnProperty(state.lang))
                    title = item.title[state.lang];

                if (item.hasOwnProperty('category'))
                    category = item.category;

                var $titleInput = $('<input type="text" class="insite-text form-control" style="width:100%;">')
                    .attr('data-content', 'title')
                    .on('change', onTitleChanged)
                    .val(title);
                var $categoryInput = $('<input type="text" class="insite-text form-control" maxlength="100" style="width:100%;">')
                    .on('change', onCategoryChanged)
                    .val(category);

                item.$row = $('<tr>').data('item', item).append(
                    $('<td class="cell-icon"><span class="drag-control"><i tabindex="-1" title="Drag &amp; Drop Row" class="icon fas fa-sort x20"></i></span></td>'),
                    $('<td class="cell-text">').html(sequence),
                    $('<td>').append($titleInput),
                    $('<td>').append($categoryInput),
                    $('<td class="cell-icon">').append(
                        $('<a tabindex="-1" title="Delete Row" href="javascript:void(0);"><i class="icon fas fa-trash-alt x20"></i></a>').on('click', onRemoveItemClick)
                    )
                ).appendTo($table.find('> tbody'));

                inSite.common.baseInput.init($titleInput);
                inSite.common.baseInput.init($categoryInput);

                if ($table.is(':hidden')) {
                    $noRows.hide();
                    $table.show();
                }
            }

            function updateItem(el, upd) {
                var $el = $(el)
                var item = $el.closest('tr').data('item');
                if (item) {
                    upd($el, item);
                    onStateUpdated();
                }
            }

            function startDragAndDrop() {
                $('table#<%= ClientID %> > tbody').sortable({
                    handle: 'span.drag-control',
                    forceHelperSize: true,
                    items: '> tr',
                    opacity: 0.8,
                    helper: function (e, $el) {
                        var source = $el[0];
                        var $helper = $el.clone();
                        var helper = $helper[0]

                        for (var i = 0; i < source.cells.length; i++) {
                            $(helper.cells[i]).width($(source.cells[i]).outerWidth());
                        }

                        return $helper;
                    },
                    start: function (s, e) {
                        if (document.activeElement && e.item.get(0).contains(document.activeElement) && document.activeElement.nodeName === 'INPUT') {
                            $(document.activeElement).trigger('change');
                        }

                        e.placeholder.height(e.item.height());
                    },
                    stop: function (s, e) {
                        var items = [];

                        $table.find('> tbody > tr').each(function (i) {
                            var item = $(this).data('item');
                            if (item && state.items.indexOf(item) != -1)
                                items.push(item);
                        });

                        if (items.length !== state.items.length) {
                            $(this).sortable('cancel');
                            alert('Error: invalid control state.');
                            return;
                        }

                        state.items = items;

                        onStateUpdated();
                    }
                });
            }

            function stopDragAndDrop() {
                $('table#<%= ClientID %> > tbody').sortable('destroy');
            }
        })();
    </script>
</insite:PageFooterContent>
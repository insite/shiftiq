(function () {
    var instance = inSite.common.searchDownload = inSite.common.searchDownload || {};
    var tables = {};
    var isShowHidden = null;

    instance.init = init;
    instance.showHidden = showHidden;

    // methods

    function init(stateId, tableId) {
        var data = createData(stateId, tableId);
        if (!data)
            return;

        var hasRows = false;

        data.$tBodies.find('> tr.column-item').each(function () {
            var $row = $(this);

            var index = $row.data('index');
            $row.removeAttr('data-index');

            var rowState = data.state[index];
            $row.data('state', rowState);

            var $visCheck = $('<input type="checkbox" class="visibility-dwn" checked title="Toggle Column Visibility">')
                .on('click', onRowVisibilityClick)
                .on('click', onToggleColumnClick);

            $row.find('> td.cmd-dwn').first().append($visCheck);

            setRowVisibility($row, rowState.visible);

            hasRows = true;
        });

        if (hasRows) {
            data.$toggle = $('<input type="checkbox" checked>')
                .on('click', onToggleAllClick)
                .attr('title', 'Toggle Column Visibility');

            var $head = data.$table.find('> thead');
            if ($head.length == 0)
                data.$table.prepend($head = $('<thead>'));

            $head.append(
                $('<tr>').append(
                    $('<td>').append(data.$toggle),
                    $('<td>')
                )
            );
        }

        data.$tBodies.sortable({
            items: '> tr.column-item',
            containment: 'document',
            cursor: 'grabbing',
            forceHelperSize: true,
            handle: '> td.title-dwn',
            axis: 'y',
            opacity: 0.65,
            tolerance: 'pointer',
            update: onSortableUpdate,
            helper: getSortableHelper,
        }).disableSelection();

        if (isShowHidden != null) {
            if (isShowHidden)
                data.$table.removeClass('hide-invisible-dwn');
            else
                data.$table.addClass('hide-invisible-dwn');
        }

        data.$table.data('data', data);

        tables[tableId] = data.$table;
    }

    function createData(stateId, tableId) {
        if (!stateId || !tableId)
            return null;

        var $state = $(document.getElementById(stateId));
        var $table = $(document.getElementById(tableId));

        if ($state.length != 1 || $table.length != 1)
            return null;

        var data = $table.data('data');
        if (data) {
            if (data.$table)
                return null;
        } else {
            data = {};
        }

        return $.extend({}, data, {
            $table: $table,
            $tBodies: $table.find('> tbody'),
            $state: $state,
            state: JSON.parse($state.val()),
            save: function () {
                this.$state.val(JSON.stringify(this.state));
            }
        });
    }

    function showHidden(value) {
        isShowHidden = value !== 'False';

        for (var n in tables) {
            if (!tables.hasOwnProperty(n))
                continue;

            var $table = tables[n].data('data').$table;
            if (isShowHidden)
                $table.removeClass('hide-invisible-dwn');
            else
                $table.addClass('hide-invisible-dwn');
        }
    }

    function getSortableHelper(e, el) {
        var $cells = el.children();
        return el.clone().children().each(function (index) {
            $(this).width($cells.eq(index).outerWidth());
        }).end();
    }

    function setRowVisibility($row, visible) {
        $row.find('> td.cmd-dwn input.visibility-dwn').prop('checked', visible);

        if (visible) {
            $row.removeClass('hidden-dwn');
        } else {
            $row.addClass('hidden-dwn');
        }

        var $tBody = $row.closest('tbody');
        if ($tBody.find('> tr.column-item:not(.hidden-dwn)').length > 0)
            $tBody.removeClass('hidden-dwn');
        else
            $tBody.addClass('hidden-dwn');
    }

    // event handlers

    function onSortableUpdate(e, ui) {
        var $table = $(e.target).closest('table');

        $table.find('> tbody > tr.column-item').each(function (index) {
            var state = $(this).data('state');

            state.number = index + 1;
        });

        $table.data('data').save();
    }

    function onRowVisibilityClick() {
        var $check = $(this);
        var $row = $check.closest('tr');
        var state = $row.data('state');

        state.visible = !state.visible;

        setRowVisibility($row, state.visible);

        $row.closest('table').data('data').save();
    }

    function onToggleAllClick() {
        var $chk = $(this);
        var $table = $chk.closest('table');
        var selector;

        if ($chk.prop('checked')) {
            selector = '> tbody > tr.column-item.hidden-dwn > td.cmd-dwn > input.visibility-dwn';
        } else {
            selector = '> tbody > tr.column-item:not(.hidden-dwn) > td.cmd-dwn > input.visibility-dwn';
        }

        $table
            .find(selector)
            .off('click', onToggleColumnClick)
            .click()
            .on('click', onToggleColumnClick);
    }

    function onToggleColumnClick() {
        var $table = $(this).closest('table');

        $table.data('data').$toggle.prop('checked', $table.find('> tbody > tr.column-item:not(.hidden-dwn)').length > 0);
    }
})();
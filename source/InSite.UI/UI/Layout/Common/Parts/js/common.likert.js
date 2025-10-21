(function () {
    if (inSite.common.likert)
        return;

    const maxTotalWidthCoef = 0.6;

    const instance = inSite.common.likert = {
        update: function () {
            return init.apply(this, arguments);
        }
    };

    function init($table, data) {
        if (!$table.is(':visible'))
            return false;

        if (!data.$cells) {
            const $cells = $table.find('thead > tr > td.text');

            let minWidth = null;
            $cells.each(function () {
                const $cell = $(this).css('width', '1px');

                const width = $cell.width();
                if (minWidth == null || minWidth < width)
                    minWidth = Math.floor(width);
            });

            data.cellMinWidth = minWidth;
            data.cellTotalMinWidth = minWidth * $cells.length;
            data.$cells = $cells;

            update($table, data);

            $table.css('visibility', 'visible');
        } else {
            update($table, data);
        }
    }

    function update($table, data) {
        if (data.$cells.length == 0)
            return;

        {
            let tableWidth = 0;
            $table.find('thead > tr > *').each(function () {
                tableWidth += $(this).width();
            });

            const cellWidth = (data.cellTotalMinWidth / tableWidth) < maxTotalWidthCoef
                ? Math.floor(tableWidth * maxTotalWidthCoef / data.$cells.length)
                : data.cellMinWidth;

            data.$cells.width(cellWidth);
        }

        {
            const $rows = $table.find('tbody > tr');

            let minHeight = null;
            $rows.each(function () {
                const $row = $(this).css('height', '1px');

                const height = $row.height();
                if (minHeight == null || minHeight < height)
                    minHeight = height;
            });

            $rows.height(minHeight);
        }
    }
})();
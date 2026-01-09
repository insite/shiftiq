(function () {
    var instance = window.optionWriteRepeater = window.optionWriteRepeater || {};

    instance.initReorder = function (tableId, hiddenId) {
        var $tbody = $('table#' + tableId + ' > tbody');
        if ($tbody.data('ui-sortable'))
            return;

        $tbody.data('hidden-id', hiddenId);

        $tbody.sortable({
            items: '> tr',
            //containment: 'parent',
            cursor: 'grabbing',
            opacity: 0.65,
            tolerance: 'pointer',
            axis: 'y',
            handle: 'span.start-sort',
            forceHelperSize: true,
            stop: function (e, a) {
                var $tbody = a.item.closest('tbody');
                var hiddenId = $tbody.data('hidden-id');
                var $input = $('#' + String(hiddenId));

                if ($input.length > 0) {
                    var result = '';

                    $tbody.find('> tr').each(function () {
                        result += ';' + String($(this).data('id'));
                    });

                    $input.val(result.length > 0 ? result.substring(1) : '');
                }
            },
        });
    };

    instance.onSetTitleTranslation = function (o) {
        o.text = questionTextEditor.fromInSiteMarkdown(o.text);
    };

    instance.onGetTitleTranslation = function (o) {
        o.text = questionTextEditor.toInSiteMarkdown(o.text);
    };
})();
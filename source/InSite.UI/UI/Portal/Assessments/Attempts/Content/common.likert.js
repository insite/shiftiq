(function () {
    const settings = {
        initElement: window.attempts.initElement,
        getData: window.attempts.getData,

        namespace: 'answer:likert',
        tableSelector: '.card-question > .card-body > .likert-matrix > table.table-likert'
    };

    $(window).on('resize', onWindowResize).on('attempts:init', function () {
        $(settings.tableSelector).each(function () {
            settings.initElement(this, settings.namespace, function (el, data) {
                const $table = $(el);

                $table.parents('.panel-group')
                    .off('shown.bs.tab', onTableShown)
                    .on('shown.bs.tab', onTableShown);

                $table.parents('.tab-pane').each(function () {
                    $('[data-bs-target="#' + this.id + '"][data-bs-toggle]')
                        .off('shown.bs.tab', onTableShown)
                        .on('shown.bs.tab', onTableShown);
                });

                data.inited = false;

                update($table, data);
            });
        });
    });

    function onTableShown() {
        update();
    }

    function onWindowResize() {
        update();
    }

    function update($table) {
        if (!$table)
            $table = $(settings.tableSelector);

        $table.each(function () {
            const data = settings.getData(this, settings.namespace);
            if (!data)
                return;

            inSite.common.likert.update($(this), data);
        });
    }
})();
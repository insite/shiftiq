(function () {
    if (_tabsSingleQuestionEnabled)
        return;

    const settings = {
        strings: {
            removeConfirm: _answerStrings.bookmark.removeConfirm,
            goToQuestion: _answerStrings.bookmark.goToQuestion,
            remove: _answerStrings.bookmark.remove,
        },
        initElement: window.attempts.initElement
    };

    const $topCollapsible = $('.accordion-bookmarks[data-position="top"] > .accordion-item > .accordion-collapse');
    const $topTable = $('.accordion-bookmarks[data-position="top"] .accordion-body table > tbody');
    const $bottomTable = $('.accordion-bookmarks[data-position="bottom"] .accordion-body table > tbody');

    let state = null;
    let questions = {};

    $(window).on('attempts:init', function () {
        state = null;
        questions = {};

        $('.card-question[data-question]').each(function () {
            settings.initElement(this, 'answer:bookmark', function (el) {
                const $el = $(el);

                const q = parseInt($el.data('question'));
                if (isNaN(q))
                    return;

                questions[q] = {
                    $panel: $el,
                    $add: $el.find('a[data-action="add-bookmark"]'),
                    $remove: $el.find('a[data-action="remove-bookmark"]'),
                    $view: $el.find('a[data-action="view-bookmarks"]'),
                };
            });
        });

        $('a[data-action="add-bookmark"]').each(function () {
            settings.initElement(this, 'answer:bookmark', function (el) {
                $(el).on('click', onQuestionAddBookmark);
            });
        });

        $('a[data-action="remove-bookmark"]').each(function () {
            settings.initElement(this, 'answer:bookmark', function (el) {
                $(el).on('click', onQuestionRemoveBookmark);
            });
        });

        $('a[data-action="view-bookmarks"]').each(function () {
            settings.initElement(this, 'answer:bookmark', function (el) {
                $(el).on('click', onQuestionViewBookmarks);
            });
        });

        loadState();
        setupBookmarks();
    });

    // event handlers

    function onTableRemoveBookmark(e) {
        e.preventDefault();

        const $this = $(this);

        const number = parseInt($this.closest('tr').data('question'));
        if (!number || isNaN(number) || !state.hasOwnProperty(number) || !questions.hasOwnProperty(number))
            return;

        helper.showConfirm(settings.strings.removeConfirm).done(function () {
            const $container = questions[number].$panel;

            helper.doActionAndPreservePosition($container, function () {
                removeBookmark(number);
            });
        });
    }

    function onTableViewBookmark(e) {
        e.preventDefault();

        const number = parseInt($(this).closest('tr').data('question'));
        if (!number || isNaN(number) || !state.hasOwnProperty(number) || !questions.hasOwnProperty(number))
            return;

        const $panel = questions[number].$panel;

        const $parentTabPane = $panel.parent('.tab-pane');
        if ($parentTabPane.length == 1)
            $(document.getElementById($parentTabPane.attr('aria-labelledby'))).tab('show');

        let scrollTo = $panel.offset().top - 80;

        if (scrollTo < 0)
            scrollTo = 0;

        $('html, body').animate({ scrollTop: scrollTo }, 250);
    }

    function onQuestionAddBookmark(e) {
        e.preventDefault();

        const $container = $(this).closest('[data-question]');
        const number = parseInt($container.data('question'));

        if (isNaN(number))
            return;

        helper.doActionAndPreservePosition($container, function () {
            addBookmark(number);
        });
    }

    function onQuestionRemoveBookmark(e) {
        e.preventDefault();

        const $container = $(this).closest('[data-question]');
        const number = parseInt($container.data('question'));

        if (isNaN(number))
            return;

        helper.doActionAndPreservePosition($container, function () {
            removeBookmark(number);
        });
    }

    function onQuestionViewBookmarks(e) {
        e.preventDefault();

        goToTopTable();
    }

    // methods

    function addBookmark(number) {
        if (!number || !state || state.hasOwnProperty(number))
            return;

        state[number] = true;
        state.order.push(number);

        setupBookmarks();
        saveState();
    }

    function removeBookmark(number) {
        if (!number || !state || !state.hasOwnProperty(number))
            return;

        delete state[number];

        for (let i = 0; i < state.order.length; i++) {
            if (state.order[i] == number) {
                state.order.splice(i, 1);
                break;
            }
        }

        setupBookmarks();
        saveState();
    };

    function goToTopTable() {
        $topCollapsible.collapse('show');

        $('html, body').animate({ scrollTop: 0 }, 250);
    };

    function setupBookmarks() {
        for (let p in questions) {
            if (!questions.hasOwnProperty(p))
                continue;

            const info = questions[p];

            if (state.hasOwnProperty(p)) {
                info.$add.hide();
                info.$remove.show();
                info.$view.show();
            } else {
                info.$add.show();
                info.$remove.hide();
                info.$view.hide();
            }
        }

        let hasData = false;

        $topTable.empty();
        $bottomTable.empty();

        const sortedOrder = state.order.slice().sort(function (a, b) {
            if (a > b)
                return 1;

            if (a < b)
                return -1;

            return 0;
        });

        for (let i = 0; i < sortedOrder.length; i++) {
            const p = sortedOrder[i];
            if (!state.hasOwnProperty(p) || !questions.hasOwnProperty(p))
                continue;

            hasData = true;

            const $panel = questions[p].$panel;
            const title = String($panel.data('question')) + ': ';
            let text = $panel.find('> .card-header > .question-text').text().trim();

            if (text.length > 100)
                text = text.substring(0, 100) + '...';

            $topTable.append(
                $('<tr>').data('question', p).append(
                    $('<td title="' + settings.strings.goToQuestion + '">').css('cursor', 'pointer').on('click', onTableViewBookmark).append(
                        $('<strong>').text(title)
                    ),
                    $('<td title="' + settings.strings.goToQuestion + '">').css('cursor', 'pointer').on('click', onTableViewBookmark).text(text),
                    $('<td>').append(
                        $('<a href="#remove-bookmark" title="' + settings.strings.remove + '">').append(
                            $('<i class="icon far fa-trash-alt">')
                        ).on('click', onTableRemoveBookmark)
                    )
                )
            );

            $bottomTable.append(
                $('<tr>').data('question', p).append(
                    $('<td title="' + settings.strings.goToQuestion + '">').css('cursor', 'pointer').on('click', onTableViewBookmark).append(
                        $('<strong>').text(title)
                    ),
                    $('<td title="' + settings.strings.goToQuestion + '">').css('cursor', 'pointer').on('click', onTableViewBookmark).text(text),
                    $('<td>').append(
                        $('<a href="#remove-bookmark" title="' + settings.strings.remove + '">').append(
                            $('<i class="icon far fa-trash-alt">')
                        ).on('click', onTableRemoveBookmark)
                    )
                )
            );
        }

        if (hasData) {
            $topTable.closest('.accordion-bookmarks').show();
            $bottomTable.closest('.accordion-bookmarks').show();
        } else {
            $topTable.closest('.accordion-bookmarks').hide();
            $bottomTable.closest('.accordion-bookmarks').hide();
        }
    }

    // state

    function saveState() {
        if (!state)
            return;

        const attempt = inSite.common.queryString['attempt'];
        if (!attempt)
            return;

        let value = [];
        for (let i = 0; i < state.order.length; i++) {
            const p = state.order[i];
            if (!state.hasOwnProperty(p))
                continue;

            const n = parseInt(p);
            if (isNaN(n))
                continue;

            value.push(n);
        }

        value = JSON.stringify(value);

        try {
            window.sessionStorage.setItem('user.exam.answer.' + attempt, value);
        } catch (e) {

        }
    }

    function loadState() {
        const attempt = inSite.common.queryString['attempt'];
        if (!attempt)
            return;

        let value = null;

        try {
            value = window.sessionStorage.getItem('user.exam.answer.' + attempt);
        } catch (e) {

        }

        try {
            if (value != null && value.length > 0) {
                value = JSON.parse(value);

                if (!(value instanceof Array))
                    value = [];
            } else {
                value = [];
            }
        } catch (e) {
            value = [];
        }

        state = { order: [] };

        for (let i = 0; i < value.length; i++) {
            const v = value[i];
            state[v] = true;
            state.order.push(v);
        }
    }
})();

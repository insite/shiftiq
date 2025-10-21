(function () {
    const settings = {
        initElement: window.attempts.initElement
    };

    const $reviewFeedbackWindow = $('#review-feedback-dialog')
        .find('modal-body').validator({
            focus: false,
            feedback: {
                success: 'fa fa-check',
                error: 'fa fa-times'
            },
        })
        .end();

    const $feedbackWindow = $('#add-question-feedback')
        .on('hidden.bs.modal', function (e) {
            e.preventDefault();

            if ($reviewFeedbackWindow.is(':visible')) {
                $('body').addClass('modal-open');

                if (isFeedbackUpdated)
                    feedbackList();
            }

            currentQuestion = null;

            $feedbackWindow.find('textarea[name="text"]')
                .val('')
                .closest('.form-group.has-error')
                .removeClass('has-error')
                .removeClass('has-danger');
        })
        .on('shown.bs.modal', focusFeedbackInput)
        .find('modal-body').validator({
            focus: false,
            feedback: {
                success: 'fa fa-check',
                error: 'fa fa-times'
            },
        })
        .end()
        .find('.btn-submit')
        .on('click', function (e) {
            if (e.isDefaultPrevented())
                return;

            e.preventDefault();

            const text = $feedbackWindow.find('textarea[name="text"]').val();

            if (text == null || text.length == 0) {
                $feedbackWindow.find('[required]').trigger('input');
                return;
            }

            isFeedbackUpdated = true;

            const $loadingPanel = $feedbackWindow.find('.loading-panel');

            $loadingPanel.show();

            const data = [];
            data.push({ name: 'action', value: 'comment-post' });
            data.push({ name: 'attemptId', value: _attemptId });
            data.push({ name: 'question', value: String(currentQuestion) });
            data.push({ name: 'text', value: text });

            $.ajax({
                type: 'POST',
                data: data,
                success: function (result) {
                    $feedbackWindow.modal('hide');
                    helper.showStatus('success', 'Feedback posted.', '');
                },
                error: function (xhr) {
                    helper.showStatus('danger', xhr.status, xhr.statusText);
                },
                complete: function () {
                    $loadingPanel.hide();
                },
            });
        })
        .end();

    let currentQuestion = null;
    let isFeedbackUpdated = false;

    $(window).on('attempts:init', function () {
        $('a[data-action="feedback-question"]').each(function () {
            settings.initElement(this, 'answer:feedback', function (el) {
                $(el).on('click', function (e) {
                    e.preventDefault();

                    feedbackEdit($(this).closest('[data-question]').data('question'));
                });
            });
        });

        $('a[data-action="review-feedback"]').each(function () {
            settings.initElement(this, 'answer:feedback', function (el) {
                $(el).on('click', function (e) {
                    e.preventDefault();

                    feedbackList();
                });
            });
        });
    });

    // methods

    function feedbackList() {
        const $feedbackList = $('#feedback-list');

        const $loadingPanel = $('#review-feedback-dialog').find('.loading-panel');
        $loadingPanel.show();

        const data = [];
        data.push({ name: 'action', value: 'comment-list' });
        data.push({ name: 'attemptId', value: _attemptId });

        $.ajax({
            type: 'POST',
            data: data,
            success: function (result) {
                $feedbackList.html(result);
            },
            error: function (xhr) {
                helper.showStatus('danger', xhr.status, xhr.statusText);
            },
            complete: function () {
                $loadingPanel.hide();
            },
        });

        $reviewFeedbackWindow.modal('show');
    }

    function feedbackEdit(question) {
        question = parseInt(question);
        if (isNaN(question))
            return;

        currentQuestion = question;
        isFeedbackUpdated = false;

        const $loadingPanel = $feedbackWindow.find('.loading-panel');
        const $textInput = $feedbackWindow.find('textarea[name=text]');

        $loadingPanel.show();

        const data = [];
        data.push({ name: 'action', value: 'comment-get' });
        data.push({ name: 'attemptId', value: _attemptId });
        data.push({ name: 'question', value: String(currentQuestion) });

        $.ajax({
            type: 'POST',
            data: data,
            success: function (result) {
                $textInput.val(result);
            },
            error: function (xhr) {
                helper.showStatus('danger', xhr.status, xhr.statusText);
            },
            complete: function () {
                $loadingPanel.hide();
                focusFeedbackInput();
            },
        });

        $feedbackWindow.modal('show');
    }

    function focusFeedbackInput() {
        const modal = bootstrap.Modal.getInstance($feedbackWindow);
        if (!modal._isShown || modal._isTransitioning)
            return;

        if ($feedbackWindow.find('.loading-panel').is(':visible'))
            return;

        $feedbackWindow.find('textarea[name=text]')[0].focus();
    }

    function feedbackDelete(question) {
        question = parseInt(question);
        if (isNaN(question))
            return;

        helper.showConfirm('Are you sure you want delete this feedback?')
            .always(function () {
                if ($reviewFeedbackWindow.is(':visible'))
                    $('body').addClass('modal-open');
            })
            .done(function () {
                const $loadingPanel = $feedbackWindow.find('.loading-panel');
                $loadingPanel.show();

                const data = [];
                data.push({ name: 'action', value: 'comment-delete' });
                data.push({ name: 'attemptId', value: _attemptId });
                data.push({ name: 'question', value: String(question) });

                $.ajax({
                    type: 'POST',
                    data: data,
                    success: function (result) {
                        $feedbackWindow.modal('hide');
                        feedbackList();
                        helper.showStatus('success', 'Feedback deleted.', '');
                    },
                    error: function (xhr) {
                        helper.showStatus('danger', xhr.status, xhr.statusText);
                    },
                    complete: function () {
                        $loadingPanel.hide();
                    },
                });
            });
    }

    window.feedbackList = {
        showList: function (e) {
            e.preventDefault();

            feedbackList();
        },
        onEditFeedback: function (e, el) {
            e.preventDefault();

            feedbackEdit($(el).data('question'));
        },
        onDeleteFeedback: function (e, obj) {
            e.preventDefault();

            feedbackDelete($(obj).data('question'));
        }
    };
})();
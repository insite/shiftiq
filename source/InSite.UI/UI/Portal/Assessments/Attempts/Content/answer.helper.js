(function () {
    const instance = window.helper = window.helper || {};

    const settings = Object.freeze({
        attempt: {
            confirmSome: _answerStrings.attempt.confirmSome,
            confirmAll: _answerStrings.attempt.confirmAll,
            confirmQuestion: _answerStrings.attempt.confirmQuestion,
            confirmReminder: _answerStrings.attempt.confirmReminder,
        },
        nextSection: {
            confirmSome: _answerStrings.nextSection.confirmSome,
            confirmAll: _answerStrings.nextSection.confirmAll,
            confirmQuestion: _answerStrings.nextSection.confirmQuestion,
            confirmReminder: _answerStrings.nextSection.confirmReminder,
        },
        nextQuestion: {
            confirmNotAnswered: _answerStrings.nextQuestion.confirmNotAnswered,
            confirmQuestion: _answerStrings.nextQuestion.confirmQuestion,
            confirmReminder: _answerStrings.nextQuestion.confirmReminder,
        },
        endBreak: {
            confirmSome: _answerStrings.endBreak.confirmSome,
            confirmAll: _answerStrings.endBreak.confirmAll,
            confirmQuestion: _answerStrings.endBreak.confirmQuestion,
            confirmReminder: _answerStrings.endBreak.confirmReminder,
        }
    });

    const $confirmWindow = $('div#confirm-dialog.modal')
        .on('hidden.bs.modal', function () {
            if (confirmAction === 'confirm')
                confirmDeferred.resolve();
            else
                confirmDeferred.reject();

            confirmAction = null;
            confirmDeferred = null;
        })
        .on('click', '[data-action]', onConfirmAction);
    const $infoWindow = $('div#info-dialog.modal')
        .on('click', onInfoClick);

    let confirmAction = null;
    let confirmDeferred = null;

    instance.confirmAttempt = function ($form) {
        const $questions = $form.find('.card-question');

        $form.data('bs.validator').validate();

        const total = $questions.length;
        const invalid = $questions.filter(function () { return $(this).hasClass('has-error'); }).length;
        let message = '';

        if (_tabsSingleQuestionEnabled) {
            if (invalid > 0)
                message += '<strong class="d-inline-block mb-2">' + settings.nextQuestion.confirmNotAnswered + '</strong>';
        } else {
            if (invalid > 0) {
                message += '<span class="d-inline-block mb-2">'
                    + settings.attempt.confirmSome.replace('$1', String(total - invalid)).replace('$2', String(total))
                    + '</span>';
            } else {
                message += '<span class="d-inline-block mb-2">'
                    + settings.attempt.confirmAll
                    + '</span>';
            }
        }

        message += '<span class="d-inline-block mb-2">' + settings.attempt.confirmQuestion + '</span>'
            + '<span class="d-inline-block mb-2">' + settings.attempt.confirmReminder + '</span>';

        return instance.showConfirm(message)
    };

    instance.confirmNextSection = function ($form) {
        const $questions = $form.find('.card-question');
        const total = $questions.length;

        let message = '';

        if (total > 0) {
            $form.data('bs.validator').validate();

            const invalid = $questions.filter(function () { return $(this).hasClass('has-error'); }).length;
            if (invalid > 0) {
                message += '<span class="d-inline-block mb-2">'
                    + settings.nextSection.confirmSome.replace('$1', String(total - invalid)).replace('$2', String(total))
                    + '</span>';
            } else {
                message += '<span class="d-inline-block mb-2">'
                    + settings.nextSection.confirmAll
                    + '</span>';
            }
        }

        message += '<span class="d-inline-block mb-2">' + settings.nextSection.confirmQuestion + '</span>'
            + '<span class="d-inline-block">' + settings.nextSection.confirmReminder + '</span>';

        return instance.showConfirm(message)
    };

    instance.confirmNextQuestion = function ($form) {
        $form.data('bs.validator').validate();

        if ($form.find('.card-question.has-error').length == 0)
            return $.Deferred().resolve().promise();

        const message = '<strong class="d-inline-block mb-2">' + settings.nextQuestion.confirmNotAnswered + '</strong>'
            + '<span class="d-inline-block mb-2">' + settings.nextQuestion.confirmQuestion + '</span>'
            + '<span class="d-inline-block">' + settings.nextQuestion.confirmReminder + '</span>';

        return instance.showConfirm(message)
    };

    instance.confirmEndBreak = function ($form) {
        const $questions = $form.find('.card-question');
        const total = $questions.length;

        let message = '';

        if (total > 0) {
            $form.data('bs.validator').validate();

            const invalid = $questions.filter(function () { return $(this).hasClass('has-error'); }).length;
            if (invalid > 0) {
                message += '<span class="d-inline-block mb-2">'
                    + settings.endBreak.confirmSome.replace('$1', String(total - invalid)).replace('$2', String(total))
                    + '</span>';
            } else {
                message += '<span class="d-inline-block mb-2">'
                    + settings.endBreak.confirmAll
                    + '</span>';
            }
        }

        message += '<span class="d-inline-block">' + settings.endBreak.confirmQuestion + '</span>';

        if (total > 0) {
            message += '<span class="d-inline-block mt-2">' + settings.endBreak.confirmReminder + '</span>';
        }

        return instance.showConfirm(message)
    };

    instance.focusError = function ($form) {
        const $panel = $form.find('.card-question.has-error:first')
        if ($panel.length === 0)
            return;

        let scroll = $panel.offset().top - $.fn.validator.Constructor.FOCUS_OFFSET;
        if (scroll < 0)
            scroll = 0;

        $('html, body').animate({ scrollTop: scroll }, 250);
    };

    instance.showConfirm = function (text) {
        confirmAction = null;
        confirmDeferred = $.Deferred();

        $confirmWindow.find('.modal-body').html(text);

        $confirmWindow.modal('show');

        return confirmDeferred.promise();
    };

    instance.closeConfirm = function () {
        confirmAction = null;
        $confirmWindow.modal('hide');
    };

    instance.showStatus = function (category, title, message) {
        $(
            '<div class="alert alert-' + category + ' alert-dismissible" role="alert">' +
            '<strong>' + title + '</strong> ' + message +
            '<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>' +
            '</div>'
        ).appendTo($('#app-growl')).fadeTo(5000, 500).slideUp(500, function () { $(this).alert('close'); });
    };

    function onConfirmAction(e) {
        if (confirmDeferred === null)
            return;

        const action = $(this).data('action');
        if (!action)
            return;

        e.preventDefault();

        if (confirmAction === null)
            confirmAction = action;

        $confirmWindow.modal('hide');
    }

    instance.showInfo = function (title, body) {
        $infoWindow.find('.modal-title').text(title);
        $infoWindow.find('.modal-body').html(body);
        $infoWindow.modal('show')
    }

    function onInfoClick(e) {
        const $target = $(e.target);

        const action = $target.data('action');
        if (action == 'close') {
            e.preventDefault();
            $infoWindow.modal('hide');
        }
    }

    instance.doActionAndPreservePosition = function ($el, action) {
        const $window = $(window);

        const scrollBefore = $window.scrollTop();
        const topBefore = $el.offset().top;

        action();

        const scrollAfter = $window.scrollTop();
        const topAfter = $el.offset().top;

        let newScrollTop = scrollBefore + (topAfter - topBefore);
        if (newScrollTop < 0)
            newScrollTop = 0;

        if (Math.abs(Math.floor(scrollAfter) - Math.floor(newScrollTop)) <= 3)
            return;

        $window.scrollTop(newScrollTop);
    };
})();

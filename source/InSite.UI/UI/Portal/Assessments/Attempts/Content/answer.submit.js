(function () {
    const settings = {
        accessTimerCount: _accessTimerCount,
        sessionId: _sessionId,
        pingInterval: _pingInterval,
        examShowTimer: _examShowTimer,
        sectionIndex: _sectionIndex,
        questionIndex: _questionIndex,
        questionNumber: _questionNumber,
        initElement: window.attempts.initElement,
    };

    const timer = {
        direction: '-',
        timerType: '-',
        tabType: '-',
        isTick: true,
        intervalHandler: null,
        timestamp: {
            current: null,
            new: null
        },
        server: null,
        isZeroTriggered: false,
        timeOverState: 0
    };

    const section = {
        flags: null,
        showWarningOnNext: true,
        isBreakTime: false,
        isEnforcedTimer: false,
        isTabInnerTimer: false
    };

    const requestState = {
        isSubmit: false,
        isComplete: false,
        isNextSection: false,
        isNextQuestion: false,
    };
    
    const defaultError = Object.freeze({
        title: 'Unexpected Error',
        message: 'Unable to send data to the server: a connection error occured.'
    });

    const timerDirection = Object.freeze({
        none: '-',
        increasing: '0',
        decreasing: '1',
    });

    class SubmitData {
        #values = {};
        #hasFiles = false;
        #hasData = false;

        append(a1, a2) {
            if (a2) {
                this.#values[a1] = a2;
                this.#hasData = true;
            } else if (a1.tagName == 'INPUT' && a1.type == 'file') {
                if (a1.files.length > 0) {
                    this.#values[a1.name] = a1.files[0];
                    this.#hasFiles = true;
                    this.#hasData = true;
                }
            } else if (a1.value) {
                this.#values[a1.name] = a1.value;
                this.#hasData = true;
            }
        }

        setup(ajaxOptions) {
            if (this.#hasFiles) {
                const data = new FormData();

                for (let name in this.#values) {
                    if (this.#values.hasOwnProperty(name))
                        data.append(name, this.#values[name]);
                }

                ajaxOptions.cache = false;
                ajaxOptions.contentType = false;
                ajaxOptions.processData = false;
                ajaxOptions.data = data;
            } else {
                ajaxOptions.data = this.#values;
            }
        }

        get hasData() {
            return this.#hasData;
        }
    }

    let submitQueue = new Array();
    let fastSubmitCounter = 3;
    let nextPingTime = null;
    let submitTimeoutHandler = null;
    let prevSubmitTime = null;
    let isLocked = false;

    const $form = $('form');
    const $cmds = $('.attempt-commands');
    const $timer = $('#timer');
    const $timerHours = $('<div class="timer-hours">00</div>');
    const $timerSeparator = $('<div class="timer-separator"> : </div>');
    const $timerMinutes = $('<div class="timer-minutes">00</div>');
    const $timerSeconds = $('<div class="timer-seconds">00</div>');

    let $lockPanel = null;

    {
        const $menuContainer = $('header.navbar > .container');
        if ($menuContainer.length == 1)
            $('#attempt-commands').detach().appendTo($menuContainer);
    }

    $timer.append($timerHours, $timerSeparator, $timerMinutes, $timerSeconds);

    $(window).on('attempts:init', function () {
        $form.find(':input[data-submit="true"]').each(function () {
            settings.initElement(this, 'answer:submit', function (el) {
                $(el).on('change', onExamInputChange);
            });
        });

        $('.exam-locked').removeClass('exam-locked');
    });

    $(document).ready(function () {
        submit(startTimer);
    });

    const $completeButton = $('[data-action="complete"]').off().on('click', function (e) {
        e.preventDefault();

        if (submitQueue.length > 0)
            submit();

        helper.confirmAttempt($form).done(function () {
            if (!requestState.isComplete) {
                lockExam('Loading...', null, true);
                completeExam();
            }
        }).fail(function () {
            helper.focusError($form);
        });
    });
    const $prevTabButton = $('[data-action="prev-tab"]').off();
    const $nextTabButton = $('[data-action="next-tab"]').off();
    const $nextSectionButton = $('[data-action="next-section"]').off();
    const $nextQuestionButton = $('[data-action="next-question"]').off();
    const $endBreakButton = $('[data-action="end-break"]').off();

    (function () {
        if (_tabsEnabled !== true || _tabsNavigationEnabled !== true) {
            $prevTabButton.remove();
            $nextTabButton.remove();
            return;
        }

        const $tabs = $(document.getElementById(_navId))
            .on('shown.bs.tab', onTabChanged)
            .find('ul.nav > li.nav-item > button.nav-link');

        onTabChanged();

        $prevTabButton.on('click', function () {
            const index = getIndex();
            if (index <= 0)
                return;

            $tabs.eq(index - 1).click();
            scrollTop();
        });

        $nextTabButton.on('click', function () {
            const index = getIndex();
            if (index + 1 > $tabs.length - 1)
                return;

            $tabs.eq(index + 1).click();
            scrollTop();
        });

        function onTabChanged() {
            const index = getIndex();
            const isFirst = index <= 0;
            const isLast = index >= $tabs.length - 1;

            $prevTabButton.toggle(!isFirst);
            $nextTabButton.toggle(!isLast);
            $completeButton.toggle(isLast);
        }

        function getIndex() {
            return $tabs.index($tabs.filter('.active'));
        }
    })();

    (function () {
        if (_tabsEnabled !== true || _tabsNavigationEnabled !== false || typeof settings.sectionIndex !== 'number' || settings.sectionIndex < 0) {
            removeNextButtons();
            return;
        }

        $(document.getElementById(_navId)).find('ul.nav > li.nav-item > button.nav-link').each(function () {
            const $this = $(this).addClass('disabled')
                .on('hidden.bs.tab', function () {
                    $(this).parent('li').css('cursor', 'not-allowed');
                })
                .on('shown.bs.tab', function () {
                    $(this).parent('li').css('cursor', '');
                });

            if (!$this.hasClass('active'))
                $this.parent('li').css('cursor', 'not-allowed');
        });

        const isSingleQuestion = _tabsSingleQuestionEnabled == true;

        if (isSingleQuestion && (typeof settings.questionIndex !== 'number' || settings.questionIndex < 0)) {
            removeNextButtons();
            return;
        }

        if (isSingleQuestion)
            renderSingleQuestionCounter();

        if (isSingleQuestion && settings.questionIndex >= _lastQuestionIndex || !isSingleQuestion && settings.sectionIndex >= _lastSectionIndex) {
            removeNextButtons();
            return;
        }

        $completeButton.hide();
        $endBreakButton.hide();

        if (isSingleQuestion) {
            $nextSectionButton.remove();
            $endBreakButton.on('click', function (e) {
                e.preventDefault();

                const promise = section.showWarningOnNext
                    ? helper.confirmEndBreak($form)
                    : $.Deferred().resolve().promise();

                promise.done(function () {
                    if (!requestState.isNextQuestion) {
                        $lockPanel = createLoadingPanel('Loading...', null, true);
                        submitNextQuestion();
                    }
                }).fail(function () {
                    helper.focusError($form);
                });
            });
            $nextQuestionButton.on('click', function (e) {
                e.preventDefault();

                const promise = section.showWarningOnNext
                    ? helper.confirmNextQuestion($form)
                    : $.Deferred().resolve().promise();

                promise.done(function () {
                    if (!requestState.isNextQuestion) {
                        $lockPanel = createLoadingPanel('Loading...', null, true);
                        submitNextQuestion();
                    }
                }).fail(function () {
                    helper.focusError($form);
                });
            });
        } else {
            $nextQuestionButton.remove();
            $endBreakButton.on('click', function (e) {
                e.preventDefault();

                const promise = section.showWarningOnNext
                    ? helper.confirmEndBreak($form)
                    : $.Deferred().resolve().promise();

                promise.done(function () {
                    if (!requestState.isNextSection) {
                        $lockPanel = createLoadingPanel('Loading...', null, true);
                        submitNextSection();
                    }
                }).fail(function () {
                    helper.focusError($form);
                });
            });
            $nextSectionButton.on('click', function (e) {
                e.preventDefault();

                const promise = section.showWarningOnNext
                    ? helper.confirmNextSection($form)
                    : $.Deferred().resolve().promise();

                promise.done(function () {
                    if (!requestState.isNextSection) {
                        $lockPanel = createLoadingPanel('Loading...', null, true);
                        submitNextSection();
                    }
                }).fail(function () {
                    helper.focusError($form);
                });
            });
        }

        function removeNextButtons() {
            $nextSectionButton.remove();
            $nextQuestionButton.remove();
            $endBreakButton.remove();
        }
    })();

    // event handlers

    function onExamInputChange() {
        const $this = $(this);
        const $panel = $this.closest('.card-question');
        if ($panel.length > 0)
            submitQueue.push($panel.get(0));

        if ($this.data('urgent') === true)
            preSubmit();
    }

    function onTimerTick() {
        if (timer.direction == timerDirection.none)
            return;

        if (timer.isTick) {
            renderTimer();

            $timer.addClass('tick');
        } else {
            $timer.removeClass('tick');
        }

        timer.isTick = !timer.isTick;
    }

    function onSubmitSuccess(result) {
        if (result === 'ERROR') {
            lockExam(defaultError.title, defaultError.message + ' (onSubmitSuccess)');
            return;
        }

        setupTimer(result);

        if (this.custom && this.custom.uniqueQueue) {
            const queue = this.custom.uniqueQueue;
            for (let i = 0; i < queue.length; i++)
                $(queue[i]).find(':input[name][data-submit="true"]').trigger('attempts:input:submitted');
        }
    }

    function onSubmitError(xhr) {
        if (xhr.readyState == 0) {
            const queue = this.custom.uniqueQueue;
            if (queue != null) {
                for (let i = 0; i < queue.length; i++) {
                    submitQueue.push(queue[i]);
                }
            }
            lockExam('Connection Lost', 'Unable to connect to the server. Check your internet connection and <a href="' + window.location.href + '">try again</a>.');
        } if (xhr.status === 403) {
            lockExam('Access Denied', 'The exam form cannot be opened in multiple tabs. Please ensure you have opened only one exam form and <a href="' + window.location.href + '">try again</a><span> after <span id="access-timer"></span></span>.');

            function updateAccessTimer() {
                const $timer = $('#access-timer');

                if (settings.accessTimerCount <= 0) {
                    $timer.parent().remove();
                    return;
                }

                html = '<b>' + settings.accessTimerCount + '</b> second';

                if (settings.accessTimerCount > 1)
                    html += 's';

                $timer.html(html);

                settings.accessTimerCount -= 1;

                workerTimer.setTimeout(updateAccessTimer, 1000);
            }

            updateAccessTimer();
        } else {
            lockExam(defaultError.title, defaultError.message + ' (onSubmitError)');
        }
    }

    function onSubmitComplete() {
        requestState.isSubmit = false;
        setSubmitTimeout();
        if (typeof this.custom.completeCallback === 'function')
            this.custom.completeCallback();
    }

    // methods

    function createLoadingPanel(title, message, showSpinner) {
        return $('<div class="loading-panel lock-panel">')
            .append(
                $('<div>').append(
                    $('<div>').append(
                        showSpinner === true ? $('<i class="fa fa-spinner fa-pulse fa-3x">') : null,
                        title ? $('<span>').html(title) : null,
                        message ? $('<p>').html(message) : null
                    )
                )
            )
            .appendTo($form)
            .show();
    }

    function renderSingleQuestionCounter() {
        if (typeof settings.questionNumber != 'number' || settings.questionNumber < 1)
            return;

        const text = _answerStrings.singleQuestionCount
            .replace('$1', String(settings.questionNumber))
            .replace('$2', String(_questionCount));

        $('.single-question-count').removeClass('d-none').text(text)
    }

    function removeLockPanel() {
        if ($lockPanel == null)
            return;

        $lockPanel.remove();
        $lockPanel = null;
    }

    function lockExam(title, message, showSpinner) {
        isLocked = true;

        clearSubmitTimeout();
        stopTimer();
        removeLockPanel();

        $(':submit').css('visibility', 'hidden');
        $('.exam-locked').removeClass('exam-locked');
        $form.find('.card-question > .card-header, .card-question > .card-body').each(function () {
            const $this = $(this);

            $this.height($this.height()).empty();
        });
        $cmds.remove();

        $lockPanel = createLoadingPanel(title, message, showSpinner);
    }

    function completeExam(redirectDelay) {
        requestState.isComplete = true;

        if ($.active > 0) {
            workerTimer.setTimeout(completeExam, 500, redirectDelay);
            return;
        }

        helper.closeConfirm();

        $.ajax({
            type: 'POST',
            data: { action: 'post' },
            success: function (data) {
                if (data !== 'ERROR') {
                    if (redirectDelay) {
                        workerTimer.setTimeout(function () { window.location.href = data; }, redirectDelay);
                    } else {
                        window.location.href = data;
                    }
                } else {
                    lockExam(defaultError.title, defaultError.message + ' (completeExam 1)');
                }
            },
            error: function () {
                lockExam(defaultError.title, defaultError.message + ' (completeExam 2)');
            },
            complete: function () {
                requestState.isComplete = false;
            }
        });
    }

    function nextSection() {
        requestState.isNextSection = true;

        if ($.active > 0) {
            workerTimer.setTimeout(nextSection, 500);
            return;
        }

        helper.closeConfirm();
        timer.timestamp.new = createTimestamp();

        $.ajax({
            type: 'POST',
            data: {
                action: 'next-section',
                session: settings.sessionId,
                index: settings.sectionIndex + 1,
            },
            success: function (data) {
                if (isLocked)
                    return;

                if (typeof data !== 'string' || data.length === 0) {
                    onError(2);
                    return;
                }

                if (data === 'ERROR') {
                    onError(3);
                    return;
                }

                const separatorIndex1 = data.indexOf('|');
                if (separatorIndex1 <= 0) {
                    onError(4);
                    return;
                }

                const separatorIndex2 = data.indexOf('|', separatorIndex1 + 1);
                if (separatorIndex2 <= 0) {
                    onError(5);
                    return;
                }

                const timerData = data.substring(0, separatorIndex1);
                if (!setupTimer(timerData))
                    return;

                const sectionIndex = parseInt(data.substring(separatorIndex1 + 1, separatorIndex2));
                if (isNaN(sectionIndex) || sectionIndex < 0) {
                    onError(6);
                    return;
                }

                const sectionHtml = data.substring(separatorIndex2 + 1);

                settings.sectionIndex = sectionIndex;

                if (settings.sectionIndex >= _lastSectionIndex) {
                    if (!section.isEnforcedTimer)
                        $completeButton.show();
                    $nextSectionButton.remove();
                    $endBreakButton.remove();
                }

                const $nav = $(document.getElementById(_navId));
                const $tabs = $nav.find('ul.nav > li.nav-item > button.nav-link');
                const $contents = $nav.find('div.tab-content > div.tab-pane');

                $contents.empty();

                const $tab = $tabs.eq(settings.sectionIndex);
                const $content = $contents.eq(settings.sectionIndex);

                $content.html(sectionHtml);
                $tab.tab('show');

                removeLockPanel();
                scrollTop();

                $(window).trigger('attempts:init');
            },
            error: function () {
                onError(1);
            },
            complete: function () {
                requestState.isNextSection = false;
            }
        });

        function onError(number) {
            lockExam(defaultError.title, defaultError.message + ' (nextSection ' + String(number) + ')');
        }
    }

    function nextQuestion() {
        requestState.isNextQuestion = true;

        if ($.active > 0) {
            workerTimer.setTimeout(nextQuestion, 500);
            return;
        }

        helper.closeConfirm();
        timer.timestamp.new = createTimestamp();

        $.ajax({
            type: 'POST',
            data: {
                action: 'next-question',
                session: settings.sessionId,
                index: settings.questionIndex + 1,
            },
            success: function (data) {
                if (isLocked)
                    return;

                if (typeof data !== 'string' || data.length === 0) {
                    onError(2);
                    return;
                }

                if (data === 'ERROR') {
                    onError(3);
                    return;
                }

                const separatorIndex1 = data.indexOf('|');
                if (separatorIndex1 <= 0) {
                    onError(4);
                    return;
                }

                const separatorIndex2 = data.indexOf('|', separatorIndex1 + 1);
                if (separatorIndex2 <= 0) {
                    onError(5);
                    return;
                }

                const separatorIndex3 = data.indexOf('|', separatorIndex2 + 1);
                if (separatorIndex3 <= 0) {
                    onError(6);
                    return;
                }

                const timerData = data.substring(0, separatorIndex1);
                if (!setupTimer(timerData))
                    return;

                const sectionIndex = parseInt(data.substring(separatorIndex1 + 1, separatorIndex2));
                if (isNaN(sectionIndex) || sectionIndex < 0) {
                    onError(7);
                    return;
                }

                const questionIndex = parseInt(data.substring(separatorIndex2 + 1, separatorIndex3));
                if (isNaN(questionIndex) || questionIndex < 0) {
                    onError(8);
                    return;
                }

                const sectionHtml = data.substring(separatorIndex3 + 1);
                const isSectionChanged = settings.sectionIndex != sectionIndex;

                settings.sectionIndex = sectionIndex;
                settings.questionIndex = questionIndex;

                if (settings.questionIndex >= _lastQuestionIndex) {
                    if (!section.isEnforcedTimer)
                        $completeButton.show();
                    $nextQuestionButton.remove();
                    $endBreakButton.remove();
                }

                const $nav = $(document.getElementById(_navId));
                const $tabs = $nav.find('ul.nav > li.nav-item > button.nav-link');
                const $contents = $nav.find('div.tab-content > div.tab-pane');

                $contents.empty();

                const $tab = $tabs.eq(settings.sectionIndex);
                const $content = $contents.eq(settings.sectionIndex);

                $content.html(sectionHtml);

                if (isSectionChanged)
                    $tab.tab('show');

                const question = $content.find('> div.card.card-question');
                if (question.length === 1 && (question.data('question') - 1) === settings.questionIndex)
                    settings.questionNumber = question.data('number');

                renderSingleQuestionCounter();
                removeLockPanel();
                scrollTop();

                $(window).trigger('attempts:init');
            },
            error: function () {
                onError(1);
            },
            complete: function () {
                requestState.isNextQuestion = false;
            }
        });

        function onError(number) {
            lockExam(defaultError.title, defaultError.message + ' (nextQuestion ' + String(number) + ')');
        }
    }

    function submitNextQuestion() {
        requestState.isNextQuestion = true;

        if (!submit(nextQuestion)) {
            workerTimer.setTimeout(submitNextQuestion, 100);
        }
    }

    function submitNextSection() {
        requestState.isNextSection = true;

        if (!submit(nextSection)) {
            workerTimer.setTimeout(submitNextSection, 100);
        }
    }

    function submit(complete) {
        clearSubmitTimeout();

        if ($.active != 0) {
            setSubmitTimeout();
            return false;
        }

        if (prevSubmitTime !== null) {
            const submitInterval = (Date.now() - prevSubmitTime) / 1000;
            if (submitInterval > (settings.pingInterval * 1.5)) {
                lockExam('Inactivity Detected', 'The exam suspended due to browser inactivity. Please click <a href="' + window.location.href + '">here</a> to continue. (1)');
                return;
            }
        }
        prevSubmitTime = Date.now();

        requestState.isSubmit = true;

        const ajaxOptions = {
            type: 'POST',
            custom: {
                uniqueQueue: null,
                completeCallback: complete
            },
            success: onSubmitSuccess,
            error: onSubmitError,
            complete: onSubmitComplete,
        };

        const submitData = new SubmitData();

        if (submitQueue.length > 0) {
            const queue = ajaxOptions.custom.uniqueQueue = $.unique(submitQueue);

            for (let i = 0; i < queue.length; i++)
                $(queue[i]).find(':input[name][data-submit="true"]').each(function () {
                    submitData.append(this);
                }).trigger('attempts:input:submitting');

            submitQueue = new Array();
        }

        if (submitData.hasData)
            submitData.append('action', 'submit');
        else
            submitData.append('action', 'ping');

        submitData.append('session', settings.sessionId);

        submitData.setup(ajaxOptions);

        timer.timestamp.new = createTimestamp();

        $.ajax(ajaxOptions);

        return true;
    }

    function clearSubmitTimeout() {
        if (submitTimeoutHandler == null)
            return;

        workerTimer.clearTimeout(submitTimeoutHandler);
        submitTimeoutHandler = null;
    }

    function setSubmitTimeout() {
        if (isLocked || submitTimeoutHandler !== null)
            return;

        if (fastSubmitCounter > 1) {
            nextPingTime = null;
            submitTimeoutHandler = workerTimer.setTimeout(preSubmit, 3000);
            fastSubmitCounter--;
        } else {
            submitTimeoutHandler = workerTimer.setTimeout(preSubmit, 5000);
        }
    }

    function preSubmit() {
        if (submitQueue.length > 0 || !nextPingTime || nextPingTime <= Date.now()) {
            nextPingTime = Date.now() + settings.pingInterval * 1000;
            submit();
        } else {
            clearSubmitTimeout();
            setSubmitTimeout();
        }
    }

    function createTimestamp() {
        return { requestStartTime: Date.now() }
    }

    function setupTimer(value) {
        const now = Date.now();
        const timestamp = timer.timestamp.new;

        if (!timestamp || timestamp.requestEndTime) {
            lockExam('Unexpected Error', 'Timestamp initialization error.');
            return false;
        }

        if (value === null)
            lockExam('Inactivity Detected', 'The exam suspended due to browser inactivity. Please click <a href="' + window.location.href + '">here</a> to continue. (2)');

        if (typeof value !== 'string')
            lockExam('Unexpected Error', 'Invalid timestamp response. (1)');

        const parts = value.split('.');
        if (parts.length !== 4)
            lockExam('Unexpected Error', 'Invalid timestamp response. (2)');

        timestamp.requestEndTime = now;
        timestamp.value = parseInt(parts[1]);

        let serverStartTime = parseInt(parts[2]);
        let serverEndTime = parseInt(parts[3]);

        if (isNaN(timestamp.value) || isNaN(serverStartTime) || isNaN(serverEndTime))
            lockExam('Unexpected Error', 'Invalid timestamp response. (3)');

        const delayDiff = Math.abs(Math.abs(serverStartTime - timestamp.requestStartTime) - Math.abs(timestamp.requestEndTime - serverEndTime));
        if (timer.server == null || timer.server.delayDiff > delayDiff) {
            const offset = ((serverStartTime - timestamp.requestStartTime) + (timestamp.requestEndTime - serverEndTime)) / 2;
            timer.server = {
                delayDiff: delayDiff,
                timeDiff: timestamp.requestEndTime - serverEndTime - offset
            };
        }

        timestamp.latency = timestamp.requestEndTime - serverEndTime - timer.server.timeDiff;

        timer.timestamp.current = timestamp;
        timer.timestamp.new = null;

        if (timer.isZeroTriggered && timestamp.value > 0)
            timer.isZeroTriggered = false;

        const flags = parts[0];
        if (section.flags != flags) {
            if (flags.length !== 5)
                lockExam('Unexpected Error', 'Invalid timestamp flags.');

            timer.direction = flags[4];

            section.flags = flags;
            section.showWarningOnNext = flags[3] != '0';
            section.isBreakTime = flags[2] == '1';
            section.isEnforcedTimer = flags[1] == '1';
            section.isTabInnerTimer = flags[0] == '1';

            const isLastItem = settings.questionIndex != null && settings.questionIndex >= _lastQuestionIndex
                || settings.sectionIndex != null && settings.sectionIndex >= _lastSectionIndex;

            if (section.isEnforcedTimer) {
                $completeButton.hide();
                $nextSectionButton.hide();
                $nextQuestionButton.hide();
                $endBreakButton.hide();
            } else if (section.isBreakTime) {
                $nextSectionButton.hide();
                $nextQuestionButton.hide();

                if (isLastItem)
                    $completeButton.show();
                else
                    $endBreakButton.show();
            } else {
                $nextSectionButton.show();
                $nextQuestionButton.show();
                $endBreakButton.hide();

                if (isLastItem)
                    $completeButton.show();
            }

            if (settings.examShowTimer && timer.direction != timerDirection.none)
                $timer.show();
            else
                $timer.hide();
        }

        renderTimer();

        return true;
    }

    function startTimer() {
        if (isLocked || timer.intervalHandler != null)
            return;

        renderTimer();

        timer.intervalHandler = workerTimer.setInterval(onTimerTick, 500);

        $(window).trigger('attempts:init');

        $cmds.show();
    }

    function stopTimer() {
        if (timer.intervalHandler === null)
            return;

        workerTimer.clearInterval(timer.intervalHandler);
        timer.intervalHandler = null;
    }

    function renderTimer() {
        const timestamp = timer.timestamp.current;
        if (!timestamp)
            return;

        let timeValue = null;

        if (timer.direction == timerDirection.increasing) {
            timeValue = timestamp.value + (Date.now() - timestamp.requestEndTime) + timestamp.latency;
        } else if (timer.direction == timerDirection.decreasing) {
            timeValue = timestamp.value - (Date.now() - timestamp.requestEndTime) - timestamp.latency;
        } else {
            return;
        }

        if (!timeValue || timeValue < 0)
            timeValue = 0;

        let hours = Math.floor(timeValue / 3600000);
        let minutes = Math.floor((timeValue - hours * 3600000) / 60000);
        let seconds = Math.floor((timeValue - hours * 3600000 - minutes * 60000) / 1000);

        if (timer.direction === timerDirection.decreasing) {
            if (hours == 0) {
                if (minutes == 0) {
                    if (timer.timeOverState < 2) {
                        $timer.addClass('time-over');
                        timer.timeOverState = 2;
                    }
                } else if (minutes < 5 || minutes == 5 && seconds == 0) {
                    if (timer.timeOverState < 1) {
                        helper.showStatus('warning', '', 'You have ' + String(minutes) + ' minute' + (minutes > 1 ? 's' : '') + ' remaining to complete this exam.');
                        timer.timeOverState = 1;
                    }
                } else {
                    if (timer.timeOverState != 0)
                        timer.timeOverState = 0;
                }
            }

            if (!timer.isZeroTriggered && timeValue <= 0) {
                timer.isZeroTriggered = true;

                if (!section.isTabInnerTimer || settings.sectionIndex >= _lastSectionIndex) {
                    if (!requestState.isComplete) {
                        lockExam('Time Is Over', '');
                        completeExam(2000);
                    }
                } else if (_tabsSingleQuestionEnabled == true) {
                    if (!requestState.isNextQuestion) {
                        $lockPanel = createLoadingPanel('Loading...', null, true);
                        submitNextQuestion();
                    }
                } else {
                    if (!requestState.isNextSection) {
                        $lockPanel = createLoadingPanel('Loading...', null, true);
                        submitNextSection();
                    }
                }
            }
        }

        hours = '00' + String(hours);
        minutes = '00' + String(minutes);
        seconds = '00' + String(seconds);

        $timerHours.html(hours.slice(-2));
        $timerMinutes.html(minutes.slice(-2));
        $timerSeconds.html(seconds.slice(-2));
    }

    function scrollTop() {
        const headerHeight = $('header.navbar:first').outerHeight();
        let scrollTo = $(document.getElementById(_navId)).offset().top - headerHeight - 24;

        if (scrollTo < 0)
            scrollTo = 0;

        $('html, body').animate({ scrollTop: scrollTo }, 250);
    }
})();
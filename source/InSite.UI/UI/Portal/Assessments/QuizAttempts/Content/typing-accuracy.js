(() => {
    let questionIndex = -1;
    let activeIndex = -1;
    let inputText = null;
    let isStarted = false;
    let isStopped = false;

    let elInputs = null;
    let elNext = null;
    let elComplete = null;
    let elQuestionPanel = null;
    let elUpdate = null;
    let elLoading = null;

    Sys.Application.add_load(() => {
        if (!elInputs) {
            initScreen();
        } else if (quizState.questionIndex !== questionIndex) {
            questionIndex = quizState.questionIndex;
            activeIndex = inputText.length;

            initInputs(quizState.inputIds);

            elLoading.classList.remove('show');
        }
    });

    function initScreen() {
        if (elInputs || !(quizState.inputIds instanceof Array) || quizState.inputIds.length === 0)
            return;

        questionIndex = quizState.questionIndex;

        elQuestionPanel = document.getElementById(quizSettings.questionPanelId);
        elUpdate = document.getElementById(quizSettings.updateId);
        elLoading = document.getElementById(quizSettings.loadingId);

        if (!elQuestionPanel || !elUpdate || !elLoading)
            return;

        elInputs = [];
        inputText = [];
        activeIndex = 0;

        initInputs(quizState.inputIds);

        document.addEventListener('focus', onDocumentFocus);

        window.addEventListener('stopped.quiztimer', () => {
            completeQuiz();
        });
    }

    function initInputs(inputIds) {
        elComplete = document.getElementById(quizState.completeId);
        if (elComplete) {
            if (!isStarted) {
                elComplete.classList.add('disabled');
            }
            elComplete.addEventListener('click', onCompleteClick);
        }

        elNext = document.getElementById(quizState.nextId);
        if (elNext) {
            if (!isStarted) {
                elNext.classList.add('disabled');
            }
            elNext.addEventListener('click', onNextClick);
        }

        for (const id of inputIds) {
            const input = document.getElementById(id);
            if (!(input instanceof HTMLInputElement) || input.type !== 'text')
                return;

            input.value = '';

            elInputs.push(input);
            inputText.push(input.value);

            input.addEventListener('keydown', onInputKeydown);
            input.addEventListener('blur', onInputBlur);
            input.addEventListener('focus', onInputFocus);
            input.addEventListener('paste', onInputPaste);
            input.addEventListener('contextmenu', onInputContextMenu);
            input.addEventListener('input', onInputInput);
        }

        focusActive();
    }

    function startQuiz() {
        if (isStarted)
            return;

        elUpdate.ajaxRequest('start');
        quizTimer.start();
        isStarted = true;
    }

    function nextQuestions() {
        if (!isStarted || isStopped)
            return false;

        elQuestionPanel.ajaxRequest("next");
        elLoading.classList.add('show');

        if (elNext) {
            elNext.classList.add('disabled');
        }

        return true;
    }

    function completeQuiz() {
        if (!isStarted || isStopped)
            return false;

        isStopped = true;
        quizTimer.stop();
        elUpdate.ajaxRequest('complete|' + String(quizTimer.getElapsed()) + '|' + JSON.stringify(inputText));
        elLoading.classList.add('show');

        if (elComplete) {
            elComplete.classList.add('disabled');
        }

        return true;
    }

    function focusActive() {
        elInputs[activeIndex].focus();
    }

    function focusNext() {
        if (isStopped)
            return;

        if (++activeIndex >= elInputs.length)
            activeIndex = 0;

        elInputs[activeIndex].focus();
    }

    function onDocumentFocus() {
        const el = document.activeElement;
        for (let i = 0; i < elInputs.length; i++) {
            if (elInputs[i] === el)
                break;
        }

        focusActive();
    }

    function onNextClick(e) {
        nextQuestions();
    }

    function onCompleteClick(e) {
        completeQuiz();
    }

    function onInputPaste(e) {
        e.preventDefault();
    }

    function onInputContextMenu(e) {
        e.preventDefault();
    }

    function onInputInput(e) {
        const diff = this.value.length - inputText[activeIndex].length;
        if (diff === 0)
            return;

        if (isStopped || diff > 1) {
            this.value = inputText[activeIndex];
            return;
        }

        if (!isStarted)
            startQuiz();

        if (elComplete) {
            elComplete.classList.remove('disabled');
        } else if (elNext) {
            elNext.classList.remove('disabled');
        }

        inputText[activeIndex] = this.value;
    }

    function onInputKeydown(e) {
        if (e.altKey === true || e.ctrlKey === true) {
            e.preventDefault();
        } else if (e.code === 'Tab') {
            e.preventDefault();
            focusNext();
        } else if (e.code === 'Enter') {
            e.preventDefault();
            if (activeIndex !== elInputs.length - 1
                || elNext && !nextQuestions()
                || elComplete && !completeQuiz()
            ) {
                focusNext();
            }
        }
    }

    function onInputBlur() {
        setTimeout(focusActive, 0);
    }

    function onInputFocus() {
        for (let i = 0; i < elInputs.length; i++) {
            const input = elInputs[i];
            if (input === this) {
                activeIndex = i;
                break;
            }
        }
    }
})();
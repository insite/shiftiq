(() => {
    let charElements = null;
    let activeIndex = -1;
    let charCount = -1;
    let inputText = '';
    let quizText = '';
    let isStarted = false;
    let isStopped = false;

    let elQuizText = null;
    let elFocusInput = null;
    let elUpdate = null;
    let elLoading = null;
    let elHeader = null;

    window.addEventListener('DOMContentLoaded', () => {
        if (elQuizText)
            return;

        elUpdate = document.getElementById(quizSettings.updateId);
        elLoading = document.getElementById(quizSettings.loadingId);
        elHeader = document.querySelector('header');

        if (!elUpdate || !elLoading)
            return;

        elQuizText = document.getElementById(quizSettings.textId);
        if (elQuizText)
            initText();
        else
            return;

        elFocusInput = document.createElement('input'); {
            elFocusInput.type = 'text';
            elFocusInput.classList.add('quiz-focus');
            elFocusInput.addEventListener('keydown', (e) => {
                if (e.code == 'Enter' || e.code == 'NumpadEnter') {
                    e.preventDefault();
                    return false;
                }
            });
            elQuizText.appendChild(elFocusInput);
        }

        elQuizText.classList.add('inited');

        setActiveChar(0);
        setTimeout(() => elFocusInput.focus({ preventScroll: true }), 0);
    });

    function initText() {
        if (elQuizText.classList.contains('inited'))
            return;
        
        const text = elQuizText.innerText;
        elQuizText.replaceChildren();

        charElements = [];
        quizText = '';
        charCount = 0;

        let wordEl = null, charEl = null, prevCh = null;

        createWord();

        for (let ch of text) {
            if (ch === '\r' || ch === '\f' || ch === '\t' || ch === '\v' || ch === '\0')
                continue;

            createChar();

            if (ch === '\n') {
                flushWord();
                charEl.innerHTML = '&nbsp;\n';
                elQuizText.appendChild(charEl);
            } else if (ch === ' ') {
                charEl.innerHTML = '&nbsp;';
                wordEl.appendChild(charEl);
            } else {
                charEl.innerText = ch;
                if (prevCh === ' ')
                    flushWord();
                wordEl.appendChild(charEl);
            }

            charElements.push(charEl);

            quizText += ch;
            charCount++;
            prevCh = ch;
        }

        flushWord();

        function createWord() {
            wordEl = document.createElement('div');
            wordEl.className = 'qword';
        }

        function createChar() {
            charEl = document.createElement('span');
            charEl.className = 'qchar';
        }

        function flushWord() {
            if (wordEl.childNodes.length > 0)
                elQuizText.appendChild(wordEl);

            createWord();
        }
    }

    window.addEventListener('stopped.quiztimer', () => {
        completeQuiz();
    });

    document.addEventListener('keydown', (e) => {
        elFocusInput.focus({ preventScroll: true });

        if (e.code == 'Backspace') {
            if (activeIndex > 0)
                removeChar();
        } else if (e.code == 'Enter') {
            if (isActiveIndexInRange())
                addChar('\n');
        } else if (isValidKey(e.code) && e.key.length == 1) {
            if (isActiveIndexInRange())
                addChar(e.key);
        }
    });

    document.addEventListener('click', (e) => {
        elFocusInput.focus({ preventScroll: true });
    });

    function startQuiz() {
        if (isStarted)
            return;

        elUpdate.ajaxRequest('start');
        quizTimer.start();
        isStarted = true;
    }

    function completeQuiz() {
        if (!isStarted || isStopped)
            return;

        isStopped = true;
        quizTimer.stop();
        elUpdate.ajaxRequest('complete|' + String(quizTimer.getElapsed()) + '|' + String(inputText));
        elLoading.classList.add('show');
    }

    function addChar(char) {
        if (!isStarted)
            startQuiz();

        if (isStopped)
            return;

        const cssClass = char === quizText[activeIndex] ? 'correct' : 'incorrect';

        charElements[activeIndex].classList.add(cssClass);

        setActiveChar(activeIndex + 1);
        ensureCursorInView(true);

        inputText += char;

        if (activeIndex >= charCount)
            completeQuiz();
    }

    function removeChar() {
        if (!isStarted || isStopped)
            return;

        const lastChar = charElements[activeIndex - 1];

        lastChar.classList.remove('correct', 'incorrect');

        setActiveChar(activeIndex - 1);
        ensureCursorInView(false);

        inputText = inputText.substring(0, inputText.length - 1);
    }

    function setActiveChar(index) {
        if (isActiveIndexInRange())
            charElements[activeIndex].classList.remove('active');

        activeIndex = index;

        if (isActiveIndexInRange())
            charElements[activeIndex].classList.add('active');
    }

    function ensureCursorInView(isForward) {
        const charIndex = activeIndex >= charCount ? charCount - 1 : activeIndex;
        if (charIndex < 0)
            return;

        const headerHeight = elHeader == null ? 0 : elHeader.offsetHeight;
        const element = charElements[charIndex];
        const elRect = element.getBoundingClientRect();
        const winHeight = window.innerHeight;

        if ((elRect.top - headerHeight) < 0)
            scrollTop();
        else if (elRect.bottom > winHeight)
            scrollBottom();
        else if (!isForward && (elRect.top - headerHeight - elRect.height) < 0)
            scrollTop();
        else if (isForward && (elRect.bottom + elRect.height) > winHeight)
            scrollBottom();

        function scrollTop() {
            window.scrollTo({
                top: window.scrollY + elRect.top - headerHeight - elRect.height - elRect.height * 0.25
            });
        }

        function scrollBottom() {
            window.scrollTo({
                top: window.scrollY + elRect.top - winHeight + elRect.height * 2 + elRect.height * 0.25
            });
        }
    }

    function isActiveIndexInRange() {
        return activeIndex >= 0 && activeIndex < charCount;
    }

    function isValidKey(code) {
        return code.startsWith('Key')
            || code.startsWith('Digit')
            || code.startsWith('Numpad')
            || code == 'Backquote'
            || code == 'Minus'
            || code == 'Equal'
            || code == 'Backslash'
            || code == 'BracketRight'
            || code == 'BracketLeft'
            || code == 'Semicolon'
            || code == 'Quote'
            || code == 'Slash'
            || code == 'Period'
            || code == 'Comma'
            || code == 'Space';
    }
})();
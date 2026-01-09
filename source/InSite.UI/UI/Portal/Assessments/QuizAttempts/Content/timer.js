(() => {
    if (window.quizTimer)
        return;

    const instance = window.quizTimer = {
        start: () => startTimer(),
        stop: () => stopTimer(),
        getElapsed: () => timerEnd == null ? null : (timerEnd - timerStart) / 1000
    };

    const container = getContainer();
    if (!container)
        return;

    let isTimerTick = true;
    let timeLimit = null; 
    let timerInterval = null;
    let timerStart = null;
    let timerEnd = null;
    let timerValue = null;
    let timerTick = 0;

    let outputMinute;
    let outputSeconds;

    window.addEventListener('DOMContentLoaded', () => {
        if (typeof quizSettings.timeLimit == 'number' && quizSettings.timeLimit > 0)
            timerValue = timeLimit = quizSettings.timeLimit * 1000;

        outputMinute = document.createElement('div'); {
            outputMinute.classList.add('timer-minutes');
        }

        const outputSeparator = document.createElement('div'); {
            outputSeparator.classList.add('timer-separator');
            outputSeparator.innerHTML = ' : ';
        }

        outputSeconds = document.createElement('div'); {
            outputSeconds.classList.add('timer-seconds');
        }

        container.append(outputMinute, outputSeparator, outputSeconds);

        renderTimer();

        container.classList.add('inited');
    });

    function startTimer() {
        if (timerStart != null)
            return;

        if (!!timeLimit)
            timerInterval = setInterval(onDecreasingTimerTick, 100);
        else
            timerInterval = setInterval(onIncreasingTimerTick, 100);

        timerStart = Date.now();

        renderTimer();
    }

    function stopTimer() {
        if (timerInterval === null)
            return;

        clearInterval(timerInterval);
        timerInterval = null;
        timerEnd = Date.now();
    }

    function onDecreasingTimerTick() {
        const timeNow = Date.now();
        const timeElapsed = timeNow - timerStart;

        timerValue = timeLimit - timeElapsed;

        if (timerValue <= 0) {
            timerValue = 0;

            stopTimer();

            container.dispatchEvent(new CustomEvent("stopped.quiztimer", {
                bubbles: true
            }));

            renderTimer();
        } else {
            checkTimerTick(timeElapsed);
        }
    }

    function onIncreasingTimerTick() {
        const timeNow = Date.now();
        const timeElapsed = timeNow - timerStart;

        timerValue = timeElapsed;

        checkTimerTick(timeElapsed);
    }

    function checkTimerTick(timeElapsed) {
        if (timeElapsed - timerTick < 500)
            return;

        timerTick = timeElapsed;

        if (isTimerTick)
            container.classList.add('tick');
        else
            container.classList.remove('tick');

        isTimerTick = !isTimerTick;

        renderTimer();
    }

    function getContainer() {
        const elements = document.querySelectorAll('.quiz-timer');
        return elements.length != 1 ? null : elements[0];
    }

    function renderTimer() {
        let minutes = Math.floor(timerValue / 60000);
        let seconds = Math.floor((timerValue - minutes * 60000) / 1000);

        minutes = '00' + String(minutes);
        seconds = '00' + String(seconds);

        outputMinute.innerText = minutes.slice(-2);
        outputSeconds.innerText = seconds.slice(-2);
    }
})();
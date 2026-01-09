(function () {
    if (inSite.common.inputAudio)
        return;

    const instance = inSite.common.inputAudio = {};

    const inputState = inSite.common.inputMedia.inputState;
    const eventId = inSite.common.inputMedia.eventId;

    const buttonId = Object.freeze({
        all: 0,
        startStop: 1,
        playPause: 2
    });

    const buttonObj = (function () {
        const result = {};
        result[buttonId.startStop] = false;
        result[buttonId.playPause] = false;
        return Object.freeze(result);
    })();

    const eventMapping = (() => {
        const result = {};
        result[eventId.starting] = 'ia-starting';
        result[eventId.started] = 'ia-started';
        result[eventId.paused] = 'ia-paused';
        result[eventId.resumed] = 'ia-resumed';
        result[eventId.stopped] = 'ia-stopped';
        result[eventId.uploading] = 'ia-uploading';
        result[eventId.uploaded] = 'ia-uploaded';
        result[eventId.cleared] = 'ia-cleared';
        return Object.freeze(result);
    })();

    const settings = {};

    instance.settings = function (maxRecTime, maxAttemptLimit) {
        if (Object.isFrozen(settings))
            return;

        settings.maxRecTime = maxRecTime;
        settings.maxAttemptLimit = maxAttemptLimit;

        Object.freeze(settings);
    };

    instance.event = (() => {
        const valueNameMapping = {};
        for (let n in eventId) {
            if (eventId.hasOwnProperty(n))
                valueNameMapping[eventId[n]] = n;
        }

        const result = {};
        for (let n in eventMapping) {
            if (eventMapping.hasOwnProperty(n))
                result[valueNameMapping[n]] = eventMapping[n];
        }

        return Object.freeze(result);
    })();
    instance.button = buttonId;

    class InputAudioState extends inSite.common.inputMedia.BaseState {
        #bitrate;
        #attemptLimit = null;

        #button;
        #disabledButton;
        #timer;
        #attempt;

        constructor(container) {
            if (!container.classList.contains('insite-input-audio'))
                throwInitError();
            
            const timer = getTimer(container);
            const attempt = getAttempt(container);

            super({
                container: container,
                input: container.querySelector('input[type="hidden"]'),

                uploadType: container.dataset['upload'],
                fnSubmit: container.dataset['submit'],
                timeLimit: timer.timeLimit,
                maxTime: settings.maxRecTime,
                isDisabled: container.dataset['disabled'] == '1',
                isReadOnly: container.dataset['readOnly'] == '1',
                isAutoUpload: container.dataset['autoUpload'] == '1',
                isAutoSubmit: container.dataset['autoSubmit'] == '1',
            });

            this.#bitrate = inSite.common.inputMedia.getAudioBitrate(container.dataset['bitrate']);
            this.#button = createButtonObj(container);
            this.#disabledButton = $.extend({}, buttonObj);
            this.#timer = timer.element;

            if (attempt) {
                this.#attempt = attempt.element;
                this.#attemptLimit = attempt.attemptLimit;

                if (typeof this.dataValue.attempt != 'number')
                    this.dataValue.attempt = 0;
                else if (this.dataValue.attempt < 0)
                    throwInitError();
                else if (this.dataValue.attempt > this.#attemptLimit)
                    this.dataValue.attempt = this.#attemptLimit;
            }

            this.#button.start.addEventListener('click', () => this.#onStartClick());

            if (this.#button.pause)
                this.#button.pause.addEventListener('click', () => this.#onPauseClick());

            this.update();
        }

        get attemptLimit() {
            return this.#attemptLimit;
        }

        getEventName(value) {
            return eventMapping[value];
        }

        getMimeExtMapping() {
            return inSite.common.inputMedia.audioMimeExt;
        }

        update() {
            super.update();
            this.#updateAttempt();
            this.#updateButtons();
        }

        updateTime(totalTime) {
            const minutes = Math.floor(totalTime / 60);
            const seconds = Math.trunc(totalTime % 60);
            const milliseconds = Math.round(totalTime % 1 * 1000);

            this.#timer.innerText = ('00' + String(minutes)).slice(-2) + ':'
                + ('00' + String(seconds)).slice(-2) + '.'
                + ('000' + String(milliseconds)).slice(-3);
        }

        canStart() {
            return super.canStart()
                && (this.#attemptLimit == null || this.dataValue.attempt < this.#attemptLimit)
        }

        canPause() {
            return this.#button.pause && super.canPause();
        }

        getStream() {
            return navigator.mediaDevices.getUserMedia({
                audio: inSite.common.inputMedia.audioConstraints.default
            });
        }

        getMediaRecorder(stream) {
            return new MediaRecorder(stream, {
                audioBitsPerSecond: this.#bitrate,
                audioBitrateMode: 'variable'
            });
        }

        onStartFailed(error) {
            super.onStartFailed(error);

            if (error.name == 'NotFoundError')
                alert('Microphone not found.\r\nPlease check if a microphone is connected to your device and if it is available in your system.');
            else
                alert('Microphone not authorized.\r\nPlease check your media permissions settings.');
        }

        onRecordingStart() {
            if (this.dataValue.attempt != null) {
                this.dataValue.attempt++;
                this.saveDataValue();
            }

            super.onRecordingStart();
        }

        onRecordingError() {
            super.onRecordingError();

            alert('Audio recording error.');
        }

        onActiveUpdated() {
            super.onActiveUpdated();

            if (!this.isActive)
                this.#updateButtons();
        }

        #onStartClick() {
            if (this.isActive)
                this.stopRecording();
            else
                this.startRecording();
        }

        #onPauseClick() {
            if (this.state == inputState.recording)
                this.pauseRecording();
            else if (this.state == inputState.paused)
                this.resumeRecording();
        }

        #updateAttempt() {
            if (this.#attemptLimit != null)
                this.#attempt.innerText = String(this.dataValue.attempt) + '/' + String(this.#attemptLimit);
        }

        #updateButtons() {
            const isStopped = this.state == inputState.stopped;
            const isWaiting = this.state == inputState.waiting;
            const isPaused = this.state == inputState.paused;
            const isRecording = this.state == inputState.recording;
            const isUploading = this.state == inputState.uploading;

            const start = this.#button.start;
            const startIcon = this.#button.startIcon;
            const pause = this.#button.pause;
            const pauseIcon = this.#button.pauseIcon;

            start.disabled = isWaiting || isUploading || this.#disabledButton[buttonId.startStop] == true || isStopped && !this.canStart();

            if (isRecording || isPaused) {
                if (start.classList.contains('btn-outline-danger')) {
                    start.classList.remove('btn-outline-danger');
                    start.classList.add('btn-danger');
                }
            } else {
                if (start.classList.contains('btn-danger')) {
                    start.classList.remove('btn-danger');
                    start.classList.add('btn-outline-danger');
                }
            }

            if (isStopped)
                startIcon.className = 'fas fa-microphone';
            else if (isWaiting)
                startIcon.className = 'fas fa-spinner fa-pulse';
            else if (isUploading)
                startIcon.className = 'fas fa-upload fa-fade';
            else
                startIcon.className = 'fas fa-stop';

            if (pause != null) {
                pause.disabled = !isPaused && !isRecording || this.#disabledButton[buttonId.playPause] == true;

                if (isPaused)
                    pauseIcon.className = 'fas fa-play';
                else
                    pauseIcon.className = 'fas fa-pause';
            }
        }

        setDisabled(value) {
            if (this.isReadOnly)
                return;

            this.isDisabled = value;
            this.update();
        }

        setButtonDisabled(id, disable) {
            if (this.isReadOnly)
                return;

            if (id == buttonId.all) {
                const obj = this.#disabledButton;
                for (let name in obj) {
                    if (obj.hasOwnProperty(name))
                        obj[name] = disable;
                }
            } else {
                this.#disabledButton[id] = disable;
            }

            this.update();
        }

        isButtonDisabled(id) {
            if (id == buttonId.all) {
                const obj = this.#disabledButton;
                for (let name in obj) {
                    if (obj.hasOwnProperty(name) && obj[name] != true)
                        return false;
                }
                return true;
            } else {
                return this.#disabledButton[id] == true;
            }
        }
    }

    class InputAudio {
        #state;

        constructor(state) {
            this.#state = state;
        }

        start() {
            this.#state.startRecording();
        }

        stop() {
            this.#state.stopRecording();
        }

        pause() {
            this.#state.pauseRecording();
        }

        resume() {
            this.#state.resumeRecording();
        }

        clear() {
            this.#state.clearRecording();
        }

        upload() {
            this.#state.uploadRecording();
        }

        submit() {
            this.#state.submitRecording();
        }

        disable() {
            this.#state.setDisabled(true);
        }

        enable() {
            this.#state.setDisabled(false);
        }

        disableButton(id) {
            if (typeof id == 'number' && (id == buttonId.all || buttonObj.hasOwnProperty(id)))
                this.#state.setButtonDisabled(id, true);
        }

        enableButton(id) {
            if (typeof id == 'number' && (id == buttonId.all || buttonObj.hasOwnProperty(id)))
                this.#state.setButtonDisabled(id, false);
        }

        isButtonDisabled(id) {
            if (typeof id != 'number' || id != buttonId.all && !buttonObj.hasOwnProperty(id))
                return null;

            return this.#state.isButtonDisabled(id);
        }

        hide() {
            this.#state.container.style.display = 'none';
            if ((this.#state.timeLimit ?? 0) > 0) {
                const next = this.#state.container.nextElementSibling;
                if (next && next.classList.contains('form-text'))
                    next.style.display = 'none';
            }
        }

        show() {
            this.#state.container.style.display = '';
            if ((this.#state.timeLimit ?? 0) > 0) {
                const next = this.#state.container.nextElementSibling;
                if (next && next.classList.contains('form-text'))
                    next.style.display = '';
            }
        }

        get state() {
            const value = this.#state.state;
            
            for (let name in inputState) {
                if (inputState.hasOwnProperty(name) && inputState[name] === value)
                    return name;
            }

            return 'unknown';
        }

        get mimeType() {
            return this.#state.recording.blob ? this.#state.recording.mime : null;
        }

        get fileName() {
            return this.#state.recording.blob ? this.#state.recording.name : null;
        }

        get mediaData() {
            return this.#state.recording.blob;
        }

        get isDisabled() {
            return this.#state.isDisabled;
        }

        get isReadOnly() {
            return this.#state.isReadOnly;
        }

        get timeLimit() {
            return this.#state.timeLimit ?? 0;
        }

        get attemptLimit() {
            return this.#state.attemptLimit ?? 0;
        }

        get attemptNow() {
            return this.#state.attemptLimit == null ? 0 : this.#state.dataValue.attempt;
        }
    }

    instance.init = function (id) {
        let elem = null;
        
        const idType = typeof id;
        if (idType == 'string') {
            if (id.length > 0)
                elem = document.getElementById(id);
        } else if (idType == 'object') {
            if (id instanceof HTMLElement)
                elem = id;
        }

        if (!elem || inSite.common.inputMedia.hasState(elem))
            return;

        const state = new InputAudioState(elem);

        elem.inputAudio = new InputAudio(state);
    }

    function createButtonObj(container) {
        const result = {
            start: container.querySelector('button[data-action="start"]'),
            startIcon: null,
            pause: container.querySelector('button[data-action="pause"]'),
            pauseIcon: null
        };

        if (!result.start)
            throwInitError();

        result.startIcon = result.start.querySelector(':scope > i:last-of-type');

        if (result.pause)
            result.pauseIcon = result.pause.querySelector(':scope > i:last-of-type');

        return result;
    }

    function getTimer(container) {
        const timerContainer = container.querySelector('[data-time]');
        if (!timerContainer)
            throwInitError();

        const result = {
            element: timerContainer.querySelector(':scope > span > span:last-of-type'),
            timeLimit: parseInt(timerContainer.dataset['time'])
        };

        if (!result.element)
            throwInitError();

        if (result.timeLimit > settings.maxRecTime)
            result.timeLimit = settings.maxRecTime;

        return result;
    }

    function getAttempt(container) {
        const attemptContainer = container.querySelector('[data-attempt]');
        if (!attemptContainer)
            return null;

        const result = {
            element: attemptContainer.querySelector(':scope > span > span:last-of-type'),
            attemptLimit: parseInt(attemptContainer.dataset['attempt'])
        };

        if (!result.element)
            throwInitError();

        if (isNaN(result.attemptLimit) || result.attemptLimit <= 0)
            throwInitError();

        if (result.attemptLimit > settings.maxAttemptLimit)
            result.attemptLimit = settings.maxAttemptLimit;

        return result;
    }

    function throwInitError(reason) {
        let text = 'InputAudio initialization error';
        if (reason)
            text += ': ' + reason;
        throw new Error(text);
    }
})();
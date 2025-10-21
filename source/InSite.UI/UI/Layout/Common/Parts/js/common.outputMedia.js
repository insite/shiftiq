(function () {
    if (inSite.common.outputMedia)
        return;

    const instance = inSite.common.outputMedia = {};

    const notImplementedError = 'Not Implemented';

    const allStates = [];
    const activeStates = [];

    let cleanUpHandler = null;

    const outputState = Object.freeze({
        none: 0,
        stopped: 1,
        waiting: 2,
        playing: 3,
        paused: 4
    });

    const eventId = Object.freeze({
        delete: 0,
        dataChanged: 1,
        dataLoading: 2,
        dataLoaded: 3,
        dataError: 4,
        starting: 5,
        muteChanged: 6,
        volumeChanged: 7,
        playing: 8,
        paused: 9,
        stopped: 10
    });

    const volumeIcon = (() => {
        const result = {};
        result[-1] = 'volume-xmark';
        result[0] = 'volume-off';
        result[1] = 'volume-low';
        result[2] = 'volume';
        result[3] = 'volume-high';
        return Object.freeze(result);
    })();

    class BaseSettings {
        #input;

        #enabled = true;
        #position = 0;
        #volume = 0.5;
        #muted = false;

        constructor(input) {
            if (!(input instanceof HTMLInputElement) || input.type != 'hidden')
                BaseSettings.#throwInitError();

            this.#input = input;
        }

        enabled(value) {
            if (typeof value == 'boolean')
                this.#enabled = value;
            else
                return this.#enabled;
        }

        position(value) {
            if (typeof value != 'number')
                return this.#position;

            this.#position = value;

            if (this.#position < 0)
                this.#position = 0;
            else if (this.#position > 1)
                this.#position = 1;
        }

        volume(value) {
            if (typeof value != 'number')
                return this.#muted ? 0 : this.#volume;

            this.#volume = value;

            if (this.#volume < 0)
                this.#volume = 0;
            else if (this.#volume > 1)
                this.#volume = 1;
        }

        muted(value) {
            if (typeof value == 'boolean')
                this.#muted = value;
            else
                return this.#muted;
        }

        setData(data) {
            this.enabled(data.enabled);
            this.position(data.position);
            this.volume(data.volume);
            this.muted(data.muted);
        }

        getData() {
            return {
                enabled: this.#enabled,
                position: this.#position,
                volume: this.#volume,
                muted: this.#muted
            };
        }

        saveData() {
            this.#input.value = JSON.stringify(this.getData());
        }

        static #throwInitError(reason) {
            let text = 'BaseSettings initialization error';
            if (reason)
                text += ': ' + reason;
            throw new Error(text);
        }
    };

    class Slider {
        #container;
        #progressBar;

        #value;
        #enabled;
        #valueMin;
        #valueMax;
        #onChange;
        #dragging;

        constructor(container) {
            this.#container = container;
            if (!this.#container.classList.contains('progress'))
                instance.Slider.#throwInitError();

            this.#progressBar = this.#container.querySelector(':scope > .progress-bar');
            if (!this.#progressBar)
                instance.Slider.#throwInitError();

            this.#valueMin = parseInt(this.#progressBar.getAttribute('aria-valuemin'));
            if (isNaN(this.#valueMin) || this.#valueMin < 0)
                this.#valueMin = 0;

            this.#valueMax = parseInt(this.#progressBar.getAttribute('aria-valuemax'));
            if (isNaN(this.#valueMax) || this.#valueMax < 0)
                this.#valueMax = 100;
            else if (this.#valueMax < this.#valueMin)
                this.#valueMax = this.#valueMin;

            this.#progressBar.setAttribute('aria-valuemin', this.#valueMin)
            this.#progressBar.setAttribute('aria-valuemax', this.#valueMax)

            this.#enabled = true;
            this.#dragging = false;

            this.#setValue(parseFloat(this.#progressBar.getAttribute('aria-valuenow')));
            this.#renderValue();

            const instance = this; {
                instance.#container.addEventListener('mousedown', onMouseDown);

                function onMouseDown(e) {
                    e.preventDefault();
                    e.stopPropagation();

                    if (!instance.#enabled || instance.#dragging)
                        return;

                    instance.#dragging = true;

                    document.addEventListener('mousemove', onMouseMove);
                    document.addEventListener('mouseup', onMouseUp);
                }

                function onMouseMove(e) {
                    instance.#setMouseEventValue(e);
                }

                function onMouseUp(e) {
                    instance.#dragging = false;

                    document.removeEventListener('mousemove', onMouseMove);
                    document.removeEventListener('mouseup', onMouseUp);

                    instance.#setMouseEventValue(e);
                }
            }
        }

        get isDragging() {
            return this.#dragging;
        }

        set onChange(value) {
            if (typeof value == 'function')
                this.#onChange = value;
        }

        enabled(value) {
            if (typeof value == 'boolean') {
                this.#enabled = value;
                this.#progressBar.style.opacity = this.#enabled ? '' : '0.5';
            } else {
                return this.#enabled;
            }
        }

        value(value) {
            if (typeof value == 'number') {
                const before = this.#value;

                this.#setValue(value);
                this.#renderValue();

                return this.#value !== before;
            } else {
                return this.#value;
            }
        }

        #setValue(value) {
            this.#value = value;

            if (isNaN(this.#value) || this.#value < 0)
                this.#value = 0;
            else if (this.#value > 1)
                this.#value = 1;
        }

        #setMouseEventValue(e) {
            const rect = this.#container.getBoundingClientRect();
            const posX = e.clientX - rect.left;
            const posValue = posX / rect.width;

            this.#setValue(posValue);
            this.#renderValue();

            if (this.#onChange)
                this.#onChange.call(this);
        }

        #renderValue() {
            const value = this.#value;
            const cssValue = Math.round((value + Number.EPSILON) * 100000) / 1000;
            const ariaValue = (this.#valueMax - this.#valueMin) * value + this.#valueMin;

            this.#progressBar.style.width = String(cssValue) + '%';
            this.#progressBar.setAttribute('aria-valuenow', ariaValue);
        }

        static #throwInitError() {
            throw new Error("Slider: initialization error");
        }
    };

    class BaseState {
        #source = null;

        #state = null;
        #isActive = false;
        #isRemoved = false;
        #isLoading = false;
        #isSeeking = false;
        #isAutoLoad = false;

        #isMediaEvent = false;

        #container;
        #settings;
        #fnSubmitDelete
        #media;
        #position;
        #volume;

        constructor(options) {
            if (!(options.container instanceof HTMLElement))
                BaseState.#throwInitError();

            if (!(options.media instanceof HTMLMediaElement))
                BaseState.#throwInitError();

            if (!(options.settings instanceof BaseSettings))
                BaseState.#throwInitError();

            if (!(options.position instanceof Slider))
                BaseState.#throwInitError();

            if (!(options.volume instanceof Slider))
                BaseState.#throwInitError();

            this.#state = null;
            this.#isActive = false;
            this.#isRemoved = false;
            this.#isLoading = false;
            this.#isSeeking = false;
            this.#isAutoLoad = options.autoLoad === true;

            this.#container = options.container;
            this.#settings = options.settings;
            this.#fnSubmitDelete = options.fnSubmitDelete ? new Function(options.fnSubmitDelete) : null;

            this.#position = options.position;
            this.#position.value(this.#settings.position());
            this.#position.onChange = this.#onPositionChanged.bind(this);

            this.#volume = options.volume;
            this.#volume.value(this.#settings.volume());
            this.#volume.onChange = this.#onVolumeChanged.bind(this);

            this.#media = options.media;
            this.#media.muted = this.#settings.muted();
            this.#media.volume = this.#volume.value();
            this.#media.onvolumechange = this.onMediaVolumeChanged.bind(this);
            this.#media.onerror = this.onMediaError.bind(this);
            this.#media.oncanplay = this.onMediaCanPlay.bind(this);
            this.#media.oncanplaythrough = this.onMediaCanPlay.bind(this);
            this.#media.onloadedmetadata = this.onMediaCanPlay.bind(this);
            this.#media.ondurationchange = this.onMediaDurationChange.bind(this);
            this.#media.onplay = this.onMediaPlay.bind(this);
            this.#media.onpause = this.onMediaPause.bind(this);
            this.#media.onplaying = this.onMediaPlaying.bind(this);
            this.#media.onseeking = this.onMediaSeeking.bind(this);
            this.#media.onseeked = this.onMediaSeeked.bind(this);
            this.#media.ontimeupdate = this.onMediaTimeUpdate.bind(this);
            this.#media.onwaiting = this.onMediaWaiting.bind(this);
            this.#media.onended = this.onMediaEnded.bind(this);

            addState(this);

            this.setState(outputState.none);
            this.#settings.saveData();
        }

        // other public

        get state() {
            return this.#state;
        }

        get container() {
            return this.#container;
        }

        get source() {
            return this.#source;
        }

        get media() {
            return this.#media;
        }

        get settings() {
            return this.#settings;
        }

        setMuted(value) {
            if (this.#settings.muted() == value)
                return;

            this.#settings.muted(this.#media.muted = value);
            this.#volume.value(this.#media.volume = this.#settings.volume());

            this.#updateVolume();

            this.#settings.saveData();

            this.dispatchEvent(eventId.muteChanged);
        }

        setVolume(value, updateSlider) {
            if (this.#settings.volume() == value)
                return;

            if (updateSlider == true)
                this.#volume.value(value);

            this.#settings.volume(value);
            this.#media.volume = this.#settings.volume();

            this.#updateVolume();
            this.#settings.saveData();

            this.dispatchEvent(eventId.volumeChanged);
        }

        setEnabled(value) {
            this.#settings.enabled(value);

            this.update();

            this.#settings.saveData();
        }

        setSource(value) {
            this.stopPlaying();

            if (typeof value == 'string' && value.length > 0) {
                this.#source = value;
            } else if (value instanceof Blob && value.size > 0) {
                this.#source = URL.createObjectURL(value);
            } else {
                this.#source = null;
            }

            this.setState(outputState.none);
            this.update();

            this.dispatchEvent(eventId.dataChanged);

            if (this.#isAutoLoad == true)
                this.loadSource();
        }

        // other event handlers

        #onPositionChanged() {
            if (this.#state == outputState.none || this.#state == outputState.waiting)
                return;

            if (isNaN(this.#media.duration) || this.#media.duration == Number.POSITIVE_INFINITY)
                return;

            if (!this.#position.isDragging) {
                this.#settings.position(this.#position.value());
                this.#media.currentTime = this.#media.duration * this.#settings.position();
            } else {
                this.update();
            }
        }

        #onVolumeChanged() {
            if (this.#settings.muted())
                this.#settings.muted(this.#media.muted = false);

            this.setVolume(this.#volume.value());
        }

        // update

        update() {
            this.updatePosition();
            this.#updateVolume();
        }

        updatePosition() {
            let timeFrom, timeThru;

            if (this.#state == outputState.none || isNaN(this.#media.duration)) {
                timeFrom = timeThru = '00:00';
                this.#position.enabled(false);
            } else if (this.#media.duration == Number.POSITIVE_INFINITY) {
                timeFrom = timeToString(this.#media.currentTime);
                timeThru = '--:--';
                this.#position.enabled(false);
            } else {
                timeThru = timeToString(this.#media.duration);

                if (this.#position.isDragging) {
                    timeFrom = timeToString(this.#media.duration * this.#position.value());
                } else {
                    timeFrom = timeToString(this.#media.currentTime);
                    this.#settings.position(this.#media.currentTime / this.#media.duration);
                    this.#position.value(this.#settings.position());
                }

                this.#position.enabled(!this.#isLoading && !this.#isSeeking && this.#settings.enabled());
            }

            this.updateTime(timeFrom, timeThru);

            function timeToString(value) {
                const minutes = Math.floor(value / 60);
                const seconds = Math.trunc(value % 60);

                return ('00' + String(minutes)).slice(-2) + ':' + ('00' + String(seconds)).slice(-2);
            }
        }

        #updateVolume() {
            if (this.#settings.muted()) {
                this.updateVolume(-1);
                return;
            }

            const volume = this.#settings.volume();
            if (volume == 0)
                this.updateVolume(0);
            else if (volume > (2 / 3))
                this.updateVolume(3);
            else if (volume > (1 / 3))
                this.updateVolume(2);
            else
                this.updateVolume(1);
        }

        updateVolume(level) {
            throw new Error(notImplementedError);
        }

        updateTime(from, thru) {
            throw new Error(notImplementedError);
        }

        // action methods

        startPlaying() {
            if (!this.canStartPlaying())
                return;

            if (this.#state == outputState.none) {
                this.loadSource();
                this.dispatchEvent(eventId.starting);
                this.#media.play();
            } else if (this.#state == outputState.paused || this.#state == outputState.stopped) {
                this.#media.play();
            }
        }

        pausePlaying() {
            if (this.canPausePlaying())
                this.#media.pause();
        }

        stopPlaying() {
            if (!this.canStopPlaying())
                return;

            this.#media.pause();
            this.#settings.position(this.#media.currentTime = 0);

            this.#isLoading = false;
            this.#isSeeking = false;

            this.setState(outputState.stopped);
            this.update();
            this.dispatchEvent(eventId.stopped);
        }

        submitDelete() {
            if (!this.canSubmitDelete())
                return;

            const event = this.dispatchEvent(eventId.delete, { cancelable: true });
            if (!event.defaultPrevented && this.#fnSubmitDelete)
                this.#fnSubmitDelete();
        }

        loadSource() {
            if (!this.canLoadSource())
                return;

            this.setState(outputState.waiting);
            this.update();

            this.dispatchEvent(eventId.dataLoading);

            this.#media.srcObject = null;
            this.#media.src = this.#source;
        }

        // media

        onMediaVolumeChanged() {
            if (this.#isMediaEvent)
                return;

            this.#isMediaEvent = true;

            this.setVolume(this.#media.volume, true);
            this.setMuted(this.#media.muted);

            this.#isMediaEvent = false;
        }

        onMediaError() {
            this.#source = null;

            this.setState(outputState.none);
            this.update();

            this.dispatchEvent(eventId.dataError);

            let errorCode = -1;
            let errorMessage = 'error undefinded';

            if (this.#media.error) {
                errorCode = this.#media.error.code;
                errorMessage = this.#media.error.message;

                if (!errorMessage) {
                    if (errorCode == MediaError.MEDIA_ERR_ABORTED)
                        errorMessage = 'aborted';
                    else if (errorCode == MediaError.MEDIA_ERR_NETWORK)
                        errorMessage = 'network error';
                    else if (errorCode == MediaError.MEDIA_ERR_DECODE)
                        errorMessage = 'decode error';
                    else if (errorCode == MediaError.MEDIA_ERR_SRC_NOT_SUPPORTED)
                        errorMessage = 'source not supported';
                    else
                        errorMessage = 'unknown error';
                }
            }

            alert('Can\'t load the media data: ' + errorMessage + ' (' + String(errorCode) + ')');
        }

        onMediaCanPlay() {
            if (this.#state != outputState.waiting)
                return;

            this.setState(outputState.stopped);
            this.update();

            this.dispatchEvent(eventId.dataLoaded);
        }

        onMediaDurationChange() {
            this.update()
        }

        onMediaPlay() {
            this.setState(outputState.playing);
            this.update();
            this.dispatchEvent(eventId.playing);
        }

        onMediaPlaying() {
            this.#isLoading = false;
            this.update();
        }

        onMediaPause() {
            this.setState(outputState.paused);
            this.update();
            this.dispatchEvent(eventId.paused);
        }

        onMediaSeeking() {
            this.#isSeeking = true;
            this.update();
        }

        onMediaSeeked() {
            this.#isSeeking = false;
            this.update();
        }

        onMediaTimeUpdate() {
            this.update();
        }

        onMediaWaiting() {
            this.#isLoading = true;
            this.update();
        }

        onMediaEnded() {
            this.stopPlaying();
        }

        // state

        setState(value) {
            this.#state = value;

            if (!this.#isRemoved)
                this.#isRemoved = !this.isInDom();

            if (this.#state == outputState.none || this.#state == outputState.stopped)
                this.#removeActive();
            else
                this.#addActive();
        }

        onStateRemoved() {
            this.stopPlaying();
            this.#removeActive();
        }

        canLoadSource() {
            return this.#source != null && this.#isRemoved != true && this.#state == outputState.none;
        }

        canStartPlaying() {
            return this.#source != null && this.#settings.enabled() && this.#isRemoved != true;
        }

        canPausePlaying() {
            return this.#state == outputState.playing;
        }

        canStopPlaying() {
            return this.#state != outputState.stopped && this.#state != outputState.none;
        }

        canSubmitDelete() {
            return this.#settings.enabled();
        }

        // active tracking

        onActiveUpdated() {

        }

        #addActive() {
            if (this.#isActive || this.#isRemoved)
                return;

            this.#isActive = true;

            addActive(this);
        }

        #removeActive() {
            if (!this.#isActive)
                return;

            this.#isActive = false;

            removeActive(this);
        }

        // helper methods

        getEventName(id) {
            throw new Error(notImplementedError);
        }

        dispatchEvent(id, options) {
            const name = this.getEventName(id);
            if (!name)
                throw new Error('Event name not found: ' + String(value));

            options = options ?? {};
            options.bubbles = true;

            const event = new CustomEvent(name, options);

            this.#container.dispatchEvent(event);

            return event;
        }

        isInDom() {
            return document.contains(this.#container);
        }

        static #throwInitError(reason) {
            let text = 'OutputMedia initialization error';
            if (reason)
                text += ': ' + reason;
            throw new Error(text);
        }
    };

    instance.BaseSettings = BaseSettings;
    instance.Slider = Slider;
    instance.BaseState = BaseState;

    instance.outputState = outputState;
    instance.eventId = eventId;
    instance.volumeIcon = volumeIcon;
    instance.hasState = hasState;

    function addState(state) {
        cleanUpStates();

        allStates.push(state);
    }

    function hasState(container) {
        for (let i = 0; i < allStates.length; i++) {
            if (allStates[i].container == container)
                return true;
        }

        return false;
    }

    function addActive(state) {
        activeStates.push(state);

        updateActive();
    }

    function removeActive(state) {
        for (let i = 0; i < activeStates.length; i++) {
            if (activeStates[i] === state) {
                activeStates.splice(i, 1);
                break;
            }
        }

        updateActive();
    }

    function updateActive() {
        for (let i = 0; i < allStates.length; i++)
            allStates[i].onActiveUpdated();
    }

    function cleanUpStates(execute) {
        if (cleanUpHandler != null) {
            clearTimeout(cleanUpHandler);
            cleanUpHandler = null;
        }

        if (execute !== true) {
            cleanUpHandler = setTimeout(cleanUpStates, 50, true);
            return;
        }

        for (let i = 0; i < allStates.length; i++) {
            const state = allStates[i];
            if (state.isRemoved) {
                allStates.splice(i--, 1);
                state.onStateRemoved();
            }
        }
    }
})();
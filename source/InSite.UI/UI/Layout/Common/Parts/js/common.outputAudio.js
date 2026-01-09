(function () {
    if (inSite.common.outputAudio)
        return;

    const instance = inSite.common.outputAudio = {};

    const outputState = inSite.common.outputMedia.outputState;
    const eventId = inSite.common.outputMedia.eventId;
    const volumeIcon = inSite.common.outputMedia.volumeIcon;

    const settings = {};

    const eventMapping = (() => {
        const result = {};
        result[eventId.delete] = 'ou-delete';
        result[eventId.dataChanged] = 'ou-datachanged';
        result[eventId.dataLoading] = 'ou-dataloading';
        result[eventId.dataLoaded] = 'ou-dataloaded';
        result[eventId.dataError] = 'ou-dataerror';
        result[eventId.starting] = 'ou-starting';
        result[eventId.muteChanged] = 'ou-mutechanged';
        result[eventId.volumeChanged] = 'ou-volumechanged';
        result[eventId.playing] = 'ou-playing';
        result[eventId.paused] = 'ou-paused';
        result[eventId.stopped] = 'ou-stopped';
        return Object.freeze(result);
    })();

    class AudioSettings extends inSite.common.outputMedia.BaseSettings {
        #attemptNow = 0;
        #attemptLimit = 0;

        constructor(input) {
            super(input);

            if (input.value) {
                const data = JSON.parse(input.value);
                this.setData(data);
            }
        }

        attemptNow(value) {
            if (typeof value != 'number')
                return this.#attemptNow;

            this.#attemptNow = Math.round(value);

            if (this.#attemptNow < 0)
                this.#attemptNow = 0;
            else if (this.#attemptNow > this.#attemptLimit)
                this.#attemptNow = this.#attemptLimit;
        }

        attemptLimit(value) {
            if (typeof value != 'number')
                return this.#attemptLimit;

            this.#attemptLimit = Math.round(value);

            if (this.#attemptLimit < 0)
                this.#attemptLimit = 0;
            else if (this.#attemptLimit > settings.maxAttemptLimit)
                this.#attemptLimit = settings.maxAttemptLimit;

            this.attemptNow(this.attemptNow());
        }

        setData(data) {
            super.setData(data);

            this.attemptLimit(data.attemptLimit);
            this.attemptNow(data.attemptNow);
        }

        getData() {
            const obj = super.getData();

            obj.attemptNow = this.#attemptNow;
            obj.attemptLimit = this.#attemptLimit;

            return obj;
        }
    }

    class AudioState extends inSite.common.outputMedia.BaseState {
        #position;
        #volume;
        #attempt;
        #button;

        constructor(container) {
            if (!container.classList.contains('insite-output-audio'))
                throwInitError();

            const position = createPositionObj(container);
            const volume = createVolumeObj(container);
            const attempt = createAttemptObj(container);
            const button = createButtonObj(container);

            super({
                container: container,
                media: new Audio(),
                settings: new AudioSettings(container.querySelector('input[type="hidden"]')),
                position: new inSite.common.outputMedia.Slider(position.progress),
                volume: new inSite.common.outputMedia.Slider(volume.progress),

                fnSubmitDelete: container.dataset['delete'],
                autoLoad: container.dataset['autoLoad'] == '1'
            });

            this.#position = position;
            this.#volume = volume;
            this.#attempt = attempt;
            this.#button = button;

            this.#button.play.addEventListener('click', this.#onPlayClick.bind(this));

            if (this.#button.delete != null) {
                this.#button.delete.disabled = false;
                this.#button.delete.addEventListener('click', this.#onDeleteClick.bind(this));
            }

            this.#volume.muteIcon.addEventListener('click', this.#onMuteClick.bind(this));

            this.setSource(container.dataset['audio']);

            this.update();
            this.#updateAttempts();
        }

        setAttemptLimit(value) {
            this.settings.attemptLimit(value);
            this.#updateAttempts();
            this.settings.saveData();
        }

        setAttemptNow(value) {
            this.settings.attemptNow(value);
            this.#updateAttempts();
            this.settings.saveData();
        }

        onActiveUpdated() {
            super.onActiveUpdated();

            if (!this.isActive)
                this.#updateButtons();
        }

        #onPlayClick() {
            if (this.state == outputState.playing)
                this.pausePlaying();
            else
                this.startPlaying();
        }

        #onDeleteClick() {
            this.submitDelete();
        }


        #onMuteClick() {
            this.setMuted(!this.settings.muted());
        }

        #updateAttempts() {
            const limit = this.settings.attemptLimit();

            if (limit > 0) {
                this.#attempt.container.style.display = '';
                this.#attempt.text.innerText = String(this.settings.attemptNow()) + '/' + String(limit);
            } else {
                this.#attempt.container.style.display = 'none';
            }
        }

        updateTime(from, thru) {
            this.#position.from.innerText = from;
            this.#position.thru.innerText = thru;
        }

        #updateButtons() {
            const isNone = this.state == outputState.none;
            const isStopped = this.state == outputState.stopped;
            const isWaiting = this.state == outputState.waiting;
            const isPlaying = this.state == outputState.playing;
            const isPaused = this.state == outputState.paused;

            const play = this.#button.play;
            const playIcon = this.#button.playIcon;

            play.disabled = isWaiting || this.source == null || !isPlaying && !this.settings.enabled();

            if (isWaiting)
                playIcon.className = 'fas fa-spinner fa-pulse';
            else if (isPlaying)
                playIcon.className = 'fas fa-pause';
            else
                playIcon.className = 'fas fa-play';

            if (this.#button.delete)
                this.#button.delete.disabled = !this.settings.enabled();
        }

        canSubmitDelete() {
            return this.#button.delete && super.canSubmitDelete();
        }

        getEventName(value) {
            return eventMapping[value];
        }

        update() {
            super.update();
            this.#updateButtons();
        }

        updateVolume(level) {
            this.#volume.muteIcon.className = 'fas fa-' + volumeIcon[level];
        }
    }

    class OutputAudio {
        #state;

        constructor(state) {
            this.#state = state;
        }

        setData(value) {
            this.#state.setSource(value);
        }

        loadData() {
            this.#state.loadSource();
        }

        play() {
            this.#state.startPlaying();
        }

        pause() {
            this.#state.pausePlaying();
        }

        stop() {
            this.#state.stopPlaying();
        }

        disable() {
            this.#state.setEnabled(false);
        }

        enable() {
            this.#state.setEnabled(true);
        }

        delete() {
            this.#state.submitDelete();
        }

        mute() {
            this.#state.setMuted(true);
        }

        unmute() {
            this.#state.setMuted(false);
        }

        hide() {
            this.#state.container.style.display = 'none';
        }

        show() {
            this.#state.container.style.display = '';
        }

        get volume() {
            return this.#state.settings.volume();
        }

        set volume(value) {
            this.#state.setVolume(value, true);
        }

        get position() {
            return this.#state.settings.position();
        }

        get duration() {
            return this.#state.media.duration;
        }

        get currentTime() {
            return this.#state.media.currentTime;
        }

        get attemptLimit() {
            return this.#state.settings.attemptLimit();
        }

        set attemptLimit(value) {
            this.#state.setAttemptLimit(value);
        }

        get attemptNow() {
            return this.#state.settings.attemptNow();
        }

        set attemptNow(value) {
            this.#state.setAttemptNow(value);
        }

        get state() {
            const value = this.#state.state;

            for (let name in outputState) {
                if (outputState.hasOwnProperty(name) && outputState[name] === value)
                    return name;
            }

            return 'unknown';
        }

        get isEnabled() {
            return this.#state.settings.enabled();
        }

        get isMuted() {
            return this.#state.settings.muted();
        }

        get hasData() {
            return this.#state.source != null
        }
    }

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

    instance.settings = function (maxAttemptLimit) {
        if (Object.isFrozen(settings))
            return;

        settings.maxAttemptLimit = maxAttemptLimit;

        Object.freeze(settings);
    };

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

        if (!elem || inSite.common.outputMedia.hasState(elem))
            return;

        const state = new AudioState(elem);

        elem.outputAudio = new OutputAudio(state);
    }

    function createPositionObj(container) {
        const root = container.querySelector('.oa-position');
        if (!root)
            throwInitError();

        const result = {
            from: root.querySelector('.oa-from'),
            thru: root.querySelector('.oa-thru'),
            progress: root.querySelector('.oa-progress .progress'),
        };

        if (!result.from || !result.thru || !result.progress)
            throwInitError();

        return result;
    }

    function createVolumeObj(container) {
        const root = container.querySelector('.oa-volume');
        if (!root)
            throwInitError();

        const result = {
            muteIcon: root.querySelector('.oa-mute > i'),
            progress: root.querySelector('.oa-progress .progress'),
        };

        if (!result.muteIcon || !result.progress)
            throwInitError();

        return result;
    }

    function createAttemptObj(container) {
        const root = container.querySelector('.oa-attempts');
        if (!root)
            throwInitError();

        const result = {
            container: root,
            text: root.querySelector(':scope > div')
        };

        if (!result.text)
            throwInitError();

        return result;
    }

    function createButtonObj(container) {
        const result = {
            play: container.querySelector('button[data-action="play"]'),
            playIcon: null,
            delete: container.querySelector('button[data-action="delete"]'),
            deleteIcon: null,
        };

        if (!result.play)
            throwInitError();

        result.playIcon = result.play.querySelector(':scope > i:last-of-type');

        if (result.delete)
            result.deleteIcon = result.delete.querySelector(':scope > i:last-of-type');

        return result;
    }

    function throwInitError(reason) {
        let text = 'OutpuAudio initialization error';
        if (reason)
            text += ': ' + reason;
        throw new Error(text);
    }
})();
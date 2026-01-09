(function () {
    if (inSite.common.outputVideo)
        return;

    const instance = inSite.common.outputVideo = {};

    const outputState = inSite.common.outputMedia.outputState;
    const eventId = inSite.common.outputMedia.eventId;
    const volumeIcon = inSite.common.outputMedia.volumeIcon;

    const settings = {};

    const eventMapping = (() => {
        const result = {};
        result[eventId.delete] = 'ov-delete';
        result[eventId.dataChanged] = 'ov-datachanged';
        result[eventId.dataLoading] = 'ov-dataloading';
        result[eventId.dataLoaded] = 'ov-dataloaded';
        result[eventId.dataError] = 'ov-dataerror';
        result[eventId.starting] = 'ov-starting';
        result[eventId.muteChanged] = 'ov-mutechanged';
        result[eventId.volumeChanged] = 'ov-volumechanged';
        result[eventId.playing] = 'ov-playing';
        result[eventId.paused] = 'ov-paused';
        result[eventId.stopped] = 'ov-stopped';
        return Object.freeze(result);
    })();

    class VideoState extends inSite.common.outputMedia.BaseState {
        #video;
        #position;
        #volume;
        #button;

        constructor(container) {
            if (!container.classList.contains('insite-output-video'))
                throwInitError();

            const video = createVideoObj(container);
            const position = createPositionObj(container);
            const volume = createVolumeObj(container);
            const button = createButtonObj(container);

            super({
                container: container,
                media: video.element,
                settings: new inSite.common.outputMedia.BaseSettings(container.querySelector('input[type="hidden"]')),
                position: new inSite.common.outputMedia.Slider(position.progress),
                volume: new inSite.common.outputMedia.Slider(volume.progress),

                fnSubmitDelete: container.dataset['delete'],
                autoLoad: container.dataset['autoLoad'] == '1'
            });

            this.#video = video;
            this.#position = position;
            this.#volume = volume;
            this.#button = button;

            this.#button.play.addEventListener('click', this.#onPlayClick.bind(this));

            if (this.#button.fullscreen != null) {
                if (!this.#video.element.requestFullscreen || !document.fullscreenEnabled) {
                    this.#button.fullscreen.remove();
                    this.#button.fullscreen = null;
                } else {
                    this.#button.fullscreen.addEventListener('click', this.#onFullScreenClick.bind(this));
                    this.#video.element.addEventListener('fullscreenchange', this.#onFullScreenChange.bind(this));
                }
            }

            if (this.#button.delete != null) {
                this.#button.delete.disabled = false;
                this.#button.delete.addEventListener('click', this.#onDeleteClick.bind(this));
            }

            this.#volume.muteIcon.addEventListener('click', this.#onMuteClick.bind(this));

            this.setSource(container.dataset['video']);

            this.update();
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

        #onFullScreenClick() {
            this.#video.element.requestFullscreen();
        }

        #onFullScreenChange() {
            const isFullScreen = !!document.fullscreenElement;
            if (this.#video.isFullScreen == isFullScreen)
                return;

            if (isFullScreen) {
                this.#video.container.classList.add('ov-fullscreen');
                this.#video.element.controls = true;
            } else {
                this.#video.container.classList.remove('ov-fullscreen');
                this.#video.element.controls = false;
            }

            this.#video.isFullScreen = isFullScreen;
        }

        #onDeleteClick() {
            this.submitDelete();
        }

        #onMuteClick() {
            this.setMuted(!this.settings.muted());
        }

        updateTime(from, thru) {
            this.#position.from.innerText = from;
            this.#position.thru.innerText = thru;
        }

        #updateVideo() {
            const isActive = this.state == outputState.playing || this.state == outputState.paused || this.state == outputState.stopped;
            if (this.#video.isActive == isActive)
                return;

            if (isActive) {
                this.#video.container.classList.add('ov-active');
                if (this.#button.fullscreen != null)
                    this.#button.fullscreen.disabled = false;
            } else {
                this.#video.container.classList.remove('ov-active');
                if (this.#button.fullscreen != null)
                    this.#button.fullscreen.disabled = true;
            }

            this.#video.isActive = isActive;
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
            this.#updateVideo();
            this.#updateButtons();
        }

        updateVolume(level) {
            this.#volume.muteIcon.className = 'fas fa-' + volumeIcon[level];
        }
    }

    class OutputVideo {
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

    instance.settings = function () {
        if (Object.isFrozen(settings))
            return;

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

        const state = new VideoState(elem);

        elem.outputVideo = new OutputVideo(state);
    }

    function createVideoObj(container) {
        const result = {
            container: container.querySelector(':scope > .ov-video'),
        };

        if (!result.container)
            throwInitError();

        result.isActive = false;
        result.isFullScreen = false;
        result.element = result.container.querySelector(':scope > video');
        result.icon = result.container.querySelector(':scope > .ov-icon');

        if (!result.element || !result.icon)
            throwInitError();

        result.element.disablePictureInPicture = true;
        result.element.controls = false;

        if (typeof result.element.playsInline == 'boolean')
            result.element.playsInline = true;

        return result;
    }

    function createPositionObj(container) {
        const root = container.querySelector('.ov-position');
        if (!root)
            throwInitError();

        const result = {
            from: root.querySelector('.ov-from'),
            thru: root.querySelector('.ov-thru'),
            progress: root.querySelector('.ov-progress .progress'),
        };

        if (!result.from || !result.thru || !result.progress)
            throwInitError();

        return result;
    }

    function createVolumeObj(container) {
        const root = container.querySelector('.ov-volume');
        if (!root)
            throwInitError();

        const result = {
            muteIcon: root.querySelector('.ov-mute > i'),
            progress: root.querySelector('.ov-progress .progress'),
        };

        if (!result.muteIcon || !result.progress)
            throwInitError();

        return result;
    }

    function createButtonObj(container) {
        const result = {
            play: container.querySelector('button[data-action="play"]'),
            playIcon: null,
            delete: container.querySelector('button[data-action="delete"]'),
            deleteIcon: null,
            fullscreen: container.querySelector('button[data-action="fullscreen"]')
        };

        if (!result.play)
            throwInitError();

        result.playIcon = result.play.querySelector(':scope > i:last-of-type');

        if (result.delete)
            result.deleteIcon = result.delete.querySelector(':scope > i:last-of-type');

        return result;
    }

    function throwInitError(reason) {
        let text = 'OutpuVideo initialization error';
        if (reason)
            text += ': ' + reason;
        throw new Error(text);
    }
})();
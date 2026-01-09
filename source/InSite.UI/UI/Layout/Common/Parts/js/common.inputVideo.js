(function () {
    if (inSite.common.inputVideo)
        return;

    const instance = inSite.common.inputVideo = {};
    const inputState = (function () {
        const result = structuredClone(inSite.common.inputMedia.inputState);
        result.cameraWaiting = 100;
        result.cameraPlaying = 101;
        return Object.freeze(result);
    })();
    const eventId = (function () {
        const result = structuredClone(inSite.common.inputMedia.eventId);
        result.cameraStarting = 100;
        result.cameraStarted = 101;
        result.cameraStopped = 103;
        return Object.freeze(result);
    })();
    const settingsStorageKey = 'inSite.common.inputVideo.settings';
    const screenCaptureDeviceId = 'screen-capture';
    const allowScreenCapture = !!navigator.mediaDevices.getDisplayMedia;

    let devices = null;
    const chromeDeviceLabel = new RegExp('^(.+)(\\s\\([0-9a-f]{4}:[0-9a-f]{4}\\))$');

    const buttonId = Object.freeze({
        all: 0,
        startStop: 1,
        playPause: 2,
        settings: 3,
        toggleCamera: 4
    });

    const buttonObj = (function () {
        const result = {};
        result[buttonId.startStop] = false;
        result[buttonId.playPause] = false;
        result[buttonId.settings] = false;
        result[buttonId.toggleCamera] = false;
        return Object.freeze(result);
    })();

    const eventMapping = (() => {
        const result = {};
        result[eventId.starting] = 'iv-starting';
        result[eventId.started] = 'iv-started';
        result[eventId.paused] = 'iv-paused';
        result[eventId.resumed] = 'iv-resumed';
        result[eventId.stopped] = 'iv-stopped';
        result[eventId.uploading] = 'iv-uploading';
        result[eventId.uploaded] = 'iv-uploaded';
        result[eventId.cleared] = 'iv-cleared';
        result[eventId.cameraStarting] = 'iv-camera-starting';
        result[eventId.cameraStarted] = 'iv-camera-started';
        result[eventId.cameraStopped] = 'iv-camera-stopped';
        return Object.freeze(result);
    })();

    const template = Object.freeze({
        settingsGroupItem: createTemplate(
            '<div class="form-check">' +
            '<input class="form-check-input" type="radio">' +
            '<label class="form-check-label"></label>' +
            '</div>')
    });

    const settings = {};

    instance.settings = function (maxRecTime, minBitrate, resolutions, frameRates) {
        if (Object.isFrozen(settings))
            return;

        settings.maxRecTime = maxRecTime;
        settings.minBitrate = minBitrate;
        settings.resolutions = structuredClone(resolutions);
        settings.frameRates = structuredClone(frameRates);

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

    class RecorderType {
        #mime;
        #vCodec;
        #aCodec;

        constructor(mime, video, audio) {
            this.#mime = mime;
            this.#vCodec = video;
            this.#aCodec = audio;
        }

        toString(audio) {
            let result = this.#mime;

            const hasVideo = !!this.#vCodec;
            const hasAudio = audio !== false && this.#aCodec;
            if (hasVideo || hasAudio) {
                result += '; codecs=';

                if (hasVideo)
                    result += this.#vCodec;

                if (hasAudio) {
                    if (hasVideo)
                        result += ',';

                    result += this.#aCodec;
                }
            }

            return result;
        }
    }

    class InputVideoState extends inSite.common.inputMedia.BaseState {
        #config;

        #button;
        #disabledButton;
        #timer;
        #settings = null;
        #camera = null;
        #video;

        constructor(container) {
            if (!container.classList.contains('insite-input-video'))
                throwInitError();

            const timer = getTimer(container);

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

            this.#config = getConfiguration(container.dataset['video']);
            this.#video = createVideoObj(container);
            this.#camera = {
                stream: null,
                timeoutHandler: null
            };
            this.#button = createButtonObj(container);
            this.#disabledButton = $.extend({}, buttonObj);
            this.#timer = timer.element;

            this.#button.start.addEventListener('click', e => this.#onStartClick(e));

            if (this.#button.pause)
                this.#button.pause.addEventListener('click', e => this.#onPauseClick(e));

            if (this.#button.settings) {
                this.#button.settings.addEventListener('show.bs.dropdown', e => this.#onSettingsShow(e));

                this.#settings = {
                    container: createSettingsMenu(this.#button.settings),
                    isRendered: false
                };
            }

            if (this.#button.camera)
                this.#button.camera.addEventListener('click', e => this.#onCameraClick(e));

            this.update();
        }

        getEventName(value) {
            return eventMapping[value];
        }

        getMimeExtMapping() {
            return inSite.common.inputMedia.videoMimeExt;
        }

        update() {
            super.update();
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

        canCameraStart() {
            return this.canStart();
        }

        canCameraStop() {
            return this.state == inputState.cameraPlaying;
        }

        canPause() {
            return this.#button.pause && super.canPause();
        }

        getStream() {
            if (this.#camera.stream && this.#camera.stream.active)
                return new Promise((resolve, reject) => {
                    resolve(this.#camera.stream);
                    this.#camera.stream = null;
                });

            const settings = getSettings();
            const options = {
                video: { resizeMode: { ideal: 'crop-and-scale' } },
                audio: structuredClone(inSite.common.inputMedia.audioConstraints.default)
            };

            let isScreenCapture = false;

            if (settings) {
                if (settings.videoDeviceId) {
                    if (settings.videoDeviceId == screenCaptureDeviceId) {
                        isScreenCapture = allowScreenCapture;
                    } else {
                        options.video.deviceId = settings.videoDeviceId;
                    }
                }

                if (!isScreenCapture && settings.audioDeviceId)
                    options.audio.deviceId = settings.audioDeviceId;
            }

            super.allowAudioCompression(!isScreenCapture);

            if (isScreenCapture && options.audio.autoGainControl === true)
                options.audio.autoGainControl = false;

            const deviceConfig = isScreenCapture ? this.#config.screen : this.#config.camera;
            options.video.height = { max: deviceConfig.resolution.height };
            options.video.frameRate = { ideal: deviceConfig.frameRate };

            return isScreenCapture
                ? navigator.mediaDevices.getDisplayMedia(options)
                : navigator.mediaDevices.getUserMedia(options);
        }

        getMediaRecorder(stream) {
            const deviceConfig = this.dataValue.source === 'screen' ? this.#config.screen : this.#config.camera;

            return new MediaRecorder(stream, {
                videoBitsPerSecond: deviceConfig.bitrate,
                audioBitsPerSecond: this.#config.audioBitrate,
                audioBitrateMode: 'variable',
                mimeType: recorderMimeType.toString(stream.getAudioTracks().length > 0)
            });
        }

        onStartSuccess(stream) {
            const settings = getSettings();

            this.dataValue.source = allowScreenCapture && settings?.videoDeviceId == screenCaptureDeviceId
                ? 'screen'
                : 'camera';
            this.saveDataValue();

            super.onStartSuccess(stream);
        }

        startRecording() {
            if (this.state == inputState.cameraPlaying)
                this.stopCamera(true);

            super.startRecording();
        }

        onStartFailed(error) {
            super.onStartFailed(error);

            showMediaError(error);
        }

        onRecordingStart() {
            super.onRecordingStart();

            const video = this.#addVideoElement();
            video.autoplay = true;
            video.muted = true;
            video.srcObject = this.recording.recorder.stream;
        }

        onRecordingStop() {
            this.#removeVideoElement();

            super.onRecordingStop();
        }

        onRecordingError() {
            super.onRecordingError();

            alert('Video recording error.');
        }

        onWaitingFailed() {
            super.onWaitingFailed();
            this.#removeVideoElement();
        }

        onActiveUpdated() {
            super.onActiveUpdated();

            if (!this.isActive)
                this.#updateButtons();
        }

        #onStartClick() {
            if (this.state == inputState.cameraPlaying)
                this.stopCamera(true);

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

        #onSettingsShow(e) {
            if (!this.#settings.isRendered) {
                if (!devices) {
                    e.preventDefault();

                    requestDevices().then(
                        () => bootstrap.Dropdown.getInstance(this.#button.settings).show(),
                        showMediaError);

                    return;
                } else {
                    renderSettings(this.#settings.container);
                }
            }

            initSettings(this.#settings.container);
        }

        #onCameraClick() {
            if (this.state == inputState.cameraPlaying)
                this.stopCamera();
            else if (this.state == inputState.stopped)
                this.startCamera();
        }

        #updateButtons() {
            const isStopped = this.state == inputState.stopped;
            const isWaiting = this.state == inputState.waiting;
            const isPaused = this.state == inputState.paused;
            const isRecording = this.state == inputState.recording;
            const isUploading = this.state == inputState.uploading;
            const isCameraWaiting = this.state == inputState.cameraWaiting;
            const isCameraPlaying = this.state == inputState.cameraPlaying;

            const start = this.#button.start;
            const startIcon = this.#button.startIcon;
            const pause = this.#button.pause;
            const pauseIcon = this.#button.pauseIcon;
            const settingsButton = this.#button.settings;
            const cameraButton = this.#button.camera;
            const cameraIcon = this.#button.cameraIcon;

            start.disabled = isCameraWaiting || isWaiting || isUploading || this.#disabledButton[buttonId.startStop] == true || isStopped && !this.canStart();

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

            if (isStopped || isCameraWaiting || isCameraPlaying)
                startIcon.className = 'fas fa-circle';
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

            if (settingsButton) {
                const isDisabled = !isStopped || this.#disabledButton[buttonId.settings] == true;

                if (isDisabled && settingsButton.classList.contains('show'))
                    bootstrap.Dropdown.getInstance(settingsButton).hide();

                settingsButton.disabled = isDisabled;
            }

            if (cameraButton) {
                cameraButton.disabled = !isStopped && !isCameraPlaying || this.#disabledButton[buttonId.camera] == true || isStopped && !this.canCameraStart();

                if (isCameraWaiting)
                    cameraIcon.className = 'fas fa-spinner fa-pulse';
                else if (isCameraPlaying)
                    cameraIcon.className = 'far fa-camera-web-slash';
                else
                    cameraIcon.className = 'far fa-camera-web';
            }
        }

        startCamera() {
            if (!this.canCameraStart())
                return;

            this.dispatchEvent(eventId.cameraStarting);

            this.setState(inputState.cameraWaiting);

            this.getStream().then(
                (stream) => {
                    this.#camera.stream = stream;

                    const tracks = this.#camera.stream.getTracks();
                    for (let i = 0; i < tracks.length; i++)
                        tracks[i].onended = () => this.#onCameraTrackEnded();

                    this.#camera.timeoutHandler = setTimeout(() => this.#onCameraDataLoaded(), 2000);

                    const video = this.#addVideoElement();
                    video.muted = true;
                    video.onloadeddata = () => this.#onCameraDataLoaded();
                    video.srcObject = stream;
                },
                (error) => this.onStartFailed(error));

            this.update();
        }

        stopCamera(leaveOpen) {
            if (!this.canCameraStop())
                return;

            if (leaveOpen !== true) {
                const tracks = this.#camera.stream.getTracks();
                for (let i = 0; i < tracks.length; i++)
                    tracks[i].stop();
                this.#camera.stream = null;
            }

            this.#removeVideoElement();

            this.setState(inputState.stopped);
            this.update();
            this.dispatchEvent(eventId.cameraStopped);
        }

        #onCameraDataLoaded() {
            if (this.#camera.timeoutHandler) {
                clearTimeout(this.#camera.timeoutHandler);
                this.#camera.timeoutHandler = null;
            }

            this.#video.element.play();
            this.setState(inputState.cameraPlaying);
            this.update();
            this.dispatchEvent(eventId.cameraStarted);
        }

        #onCameraTrackEnded() {
            if (this.state == inputState.cameraPlaying)
                this.stopCamera();
        }

        #addVideoElement() {
            if (this.#video.element)
                return this.#video.element;

            const video = this.#video.element = document.createElement('video');
            video.disablePictureInPicture = true;
            video.controls = false;

            if (typeof video.playsInline == 'boolean')
                video.playsInline = true;

            this.#video.container.classList.add('iv-active');
            this.#video.container.appendChild(video);
            return video;
        }

        #removeVideoElement() {
            if (!this.#video.element)
                return;

            this.#video.element.pause();
            this.#video.element.remove();
            this.#video.element = null;
            this.#video.container.classList.remove('iv-active');
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

    class InputVideo {
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

            for (let name in state) {
                if (state.hasOwnProperty(name) && state[name] === value)
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
    }

    const recorderMimeType = (() => {
        const types = [
            new RecorderType('video/mp4', 'av1', 'mp4a'),
            new RecorderType('video/webm', 'vp9', 'opus'),
            new RecorderType('video/webm', 'vp8', 'opus'),
            new RecorderType('video/webm'),
            new RecorderType('video/mp4', 'avc1', 'mp4a'),
            new RecorderType('video/mp4', 'h264', 'mp4a'),
            new RecorderType('video/mp4')
        ];

        for (let t of types) {
            if (MediaRecorder.isTypeSupported(t.toString()))
                return t;
        }

        return undefined;
    })();

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

        const state = new InputVideoState(elem);

        elem.inputVideo = new InputVideo(state);
    }

    // initialization

    function createVideoObj(container) {
        const result = {
            container: container.querySelector(':scope > .iv-video'),
            icon: null,
            element: null
        };

        if (!result.container)
            throwInitError();

        result.icon = result.container.querySelector(':scope > .iv-icon');

        return result;
    }

    function createButtonObj(container) {
        const result = {
            start: container.querySelector('button[data-action="start"]'),
            startIcon: null,
            pause: container.querySelector('button[data-action="pause"]'),
            pauseIcon: null,
            settings: container.querySelector('button[data-action="settings"]'),
            settingsMenu: null,
            camera: container.querySelector('button[data-action="camera"]'),
            cameraIcon: null,
        };

        if (!result.start)
            throwInitError();

        result.startIcon = result.start.querySelector(':scope > i:last-of-type');

        if (result.pause)
            result.pauseIcon = result.pause.querySelector(':scope > i:last-of-type');

        if (result.camera)
            result.cameraIcon = result.camera.querySelector(':scope > i:last-of-type');

        return result;
    }

    function createSettingsMenu(button) {
        button.dataset.bsToggle = 'dropdown';
        button.dataset.bsAutoClose = 'outside';
        button.dataset.popperStrategy = 'fixed';
        button.setAttribute('aria-expanded', 'false');

        const buttonContainer = document.createElement('div');
        buttonContainer.className = 'btn-group';
        buttonContainer.role = 'group';

        button.parentNode.insertBefore(buttonContainer, button);
        buttonContainer.appendChild(button);

        const menuContainer = document.createElement('div');
        menuContainer.className = 'dropdown-menu px-4 py-3 text-body-secondary';
        menuContainer.style.maxWidth = '250px';

        buttonContainer.appendChild(menuContainer);

        return menuContainer;
    }

    function getTimer(container) {
        const timerContainer = container.querySelector('[data-time]');
        if (!timerContainer)
            throwInitError();

        const result = {
            element: timerContainer.querySelector(':scope > span:last-of-type'),
            timeLimit: parseInt(timerContainer.dataset['time'])
        };

        if (!result.element)
            throwInitError();

        if (result.timeLimit > settings.maxRecTime)
            result.timeLimit = settings.maxRecTime;

        return result;
    }

    function getConfiguration(value) {
        if (!value)
            throwInitError();

        const array = value.split('.');
        if (array.length != 8)
            throwInitError();

        const result = {
            audioBitrate: inSite.common.inputMedia.getAudioBitrate(array[0]),
            camera: getDeviceSettings(array[1], array[4], array[2], array[5]),
            screen: getDeviceSettings(array[1], array[6], array[3], array[7])
        };

        return Object.freeze(result);

        function getDeviceSettings(mode, resolution, bitrate, frameRate) {
            if (!settings.resolutions.hasOwnProperty(resolution))
                throwInitError();

            if (!settings.frameRates.hasOwnProperty(frameRate))
                throwInitError();

            const result = {
                resolution: settings.resolutions[resolution],
                bitrate: parseInt(bitrate),
                frameRate: settings.frameRates[frameRate],
            };

            if (isNaN(result.bitrate))
                throwInitError();

            if (mode == 'm') {
                if (result.bitrate < settings.minBitrate)
                    throwInitError();
            } else if (mode == 'a') {
                const info = result.resolution.bitrate[frameRate];
                if (result.bitrate < info.min || result.bitrate > info.max)
                    throwInitError();
            } else {
                throwInitError();
            }

            return result;
        }
    }

    function throwInitError(reason) {
        let text = 'InputAudio initialization error';
        if (reason)
            text += ': ' + reason;
        throw new Error(text);
    }

    function createTemplate(html) {
        const result = document.createElement('template');
        result.innerHTML = html;
        return result;
    }

    function showMediaError(error) {
        if (error.name == 'NotAllowedError')
            alert('Access to camera is blocked.\r\nPlease check your media permissions settings.');
        else if (error.name == 'NotFoundError')
            alert('Camera not found.');
        else if (error.name == 'InputVideoError')
            alert(error.message);
        else
            throw error;
    }

    // devices

    function requestDevices() {
        return new Promise(function (resolve, reject) {
            if (devices) {
                resolve(devices);
                return;
            }

            navigator.mediaDevices.getUserMedia({ audio: true, video: true }).then(
                stream => {
                    navigator.mediaDevices.enumerateDevices()
                        .then(
                            list => {
                                let data = getDevices(list);
                                if (data) {
                                    resolve(devices = data);
                                } else {
                                    const error = new Error('Can\'t get the list of devices.');
                                    error.name = 'InputVideoError';
                                    reject(error);
                                }
                            },
                            error => reject(error))
                        .finally(() => stream.getTracks().forEach(t => t.stop()));
                },
                error => reject(error)
            );
        });
    }

    function getDevices(devices) {
        const data = {
            video: [],
            audio: [],
        };

        for (let d of devices) {
            if (d.kind == 'videoinput') {
                if (!d.label)
                    return null;

                data.video.push(d)
            } else if (d.kind == 'audioinput') {
                if (!d.label)
                    return null;

                data.audio.push(d)
            }
        }

        if (allowScreenCapture)
            data.video.push({
                deviceId: screenCaptureDeviceId,
                groupId: 'none',
                kind: "custom",
                label: "Screen Capture"
            });

        return data;
    }

    // settings

    function renderSettings(container) {
        container.replaceChildren();

        const namePrefix = String(Date.now()) + '-';
        appendSettingsGroup(container, namePrefix + 'video', 'Video', devices.video, false);
        appendSettingsGroup(container, namePrefix + 'audio', 'Audio', devices.audio, true);
    }

    function appendSettingsGroup(container, idPrefix, title, devices, isLast) {
        const header = document.createElement('div');
        header.className = 'h6 mb-2';
        header.innerText = title;

        const itemsContainer = document.createElement('div');
        if (!isLast)
            itemsContainer.className = 'mb-3';

        for (let i = 0; i < devices.length; i++)
            appendSettingsGroupItem(itemsContainer, idPrefix + '-' + String(i), idPrefix, devices[i]);

        container.appendChild(header);
        container.appendChild(itemsContainer);
    }

    function appendSettingsGroupItem(container, id, name, device) {
        const item = template.settingsGroupItem.content.cloneNode(true);

        const input = item.querySelector('input');
        input.id = id;
        input.name = name;
        input.value = device.deviceId;
        input.addEventListener('click', onSettingsItemClick)

        const label = item.querySelector('label');
        label.htmlFor = id;
        label.innerText = getLabel(device.label);

        container.appendChild(item);

        function getLabel(value) {
            const parts = chromeDeviceLabel.exec(value);
            if (parts && parts.length == 3)
                return parts[1];

            return value;
        }
    }

    function initSettings(container) {
        const checkedInputs = container.querySelectorAll('input[type="radio"]:checked');
        for (let input of checkedInputs)
            input.checked = false;

        const settings = getSettings();
        if (!settings)
            return;

        if (settings.videoDeviceId) {
            const input = container.querySelector(`input[type="radio"][value="${settings.videoDeviceId}"]`);
            if (input)
                input.checked = true;

            disableSettingsAudio(container, settings.videoDeviceId === screenCaptureDeviceId)
        }

        if (settings.audioDeviceId) {
            const input = container.querySelector(`input[type="radio"][value="${settings.audioDeviceId}"]`);
            if (input)
                input.checked = true;
        }
    }

    function onSettingsItemClick(e) {
        const input = this;
        const settings = getSettings() ?? {};
        const isScreenCaptureBefore = settings.videoDeviceId === screenCaptureDeviceId;

        if (input.name.endsWith('-video'))
            setValue('videoDeviceId');
        else if (input.name.endsWith('-audio'))
            setValue('audioDeviceId');

        setSettings(settings);

        const isScreenCaptureAfter = settings.videoDeviceId === screenCaptureDeviceId;
        if (isScreenCaptureBefore != isScreenCaptureAfter)
            disableSettingsAudio(input, isScreenCaptureAfter);

        function setValue(name) {
            if (settings[name] == input.value) {
                delete settings[name];
                setTimeout(input => input.checked = false, 0, input);
            } else {
                settings[name] = input.value;
            }
        }
    }

    function disableSettingsAudio(element, disable) {
        const audioInputs = element.closest('.dropdown-menu').querySelectorAll('input[type="radio"][name$=-audio]');
        for (let input of audioInputs)
            input.disabled = disable;
    }

    function getSettings() {
        try {
            const value = window.localStorage.getItem(settingsStorageKey);
            if (value)
                return JSON.parse(value);
        } catch (e) {

        }

        return null;
    }

    function setSettings(value) {
        if (value)
            window.localStorage.setItem(settingsStorageKey, JSON.stringify(value));
        else
            window.localStorage.removeItem(settingsStorageKey)
    }
})();
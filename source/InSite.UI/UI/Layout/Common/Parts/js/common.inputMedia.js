(function () {
    if (inSite.common.inputMedia)
        return;

    const instance = inSite.common.inputMedia = {};

    const notImplementedError = 'Not Implemented';
    const uploadApiPath = '/api/assets/files';
    const tickInterval = 50;
    const tickTestInterval = Math.round(1000 / tickInterval);

    const allStates = [];
    const activeStates = [];

    let cleanUpHandler = null;

    const inputState = Object.freeze({
        stopped: 0,
        waiting: 1,
        recording: 2,
        paused: 3,
        uploading: 4,
    });

    const audioMimeExt = Object.freeze({
        'audio/webm': '.webm',
        'audio/ogg': '.ogg',
        'audio/mpeg': '.mp3',
        'audio/mp4': '.m4a',
        'audio/aac': '.aac',
        'audio/opus': '.opus',
        'audio/wav': '.wav',
        'audio/3gpp': '.3gp',
        'audio/3gpp2': '.3g2'
    });

    const videoMimeExt = Object.freeze({
        'video/webm': '.webm',
        'video/x-matroska': '.mkv',
        'video/ogg': '.ogv',
        'video/mp4': '.mp4'
    });

    const audioBitrate = Object.freeze({
        '8': 8000,
        '16': 16000,
        '32': 32000,
        '64': 64000,
        '128': 128000,
        '256': 256000
    });

    const eventId = Object.freeze({
        starting: 0,
        started: 1,
        paused: 2,
        resumed: 3,
        stopped: 4,
        uploading: 5,
        uploaded: 6,
        cleared: 7,
    });

    const audioConstraints = (function () {
        const result = {};
        const supported = navigator.mediaDevices.getSupportedConstraints();
        const other = {
            audioCompression: !!AudioContext
        };
        const desired = {
            echoCancellation: false,
            noiseSuppression: true,
            autoGainControl: !other.audioCompression,
            sampleRate: 44100,
            channelCount: 1,
        };
        const ua = navigator.userAgent;
        const isDesktopSafari = ua.includes('(Macintosh;')
            && ua.includes('Mac OS')
            && ua.includes('Safari')
            && !ua.includes('Chrome')
            && !ua.includes('Edg');

        if (supported.echoCancellation === true && !isDesktopSafari)
            result.echoCancellation = desired.echoCancellation;

        if (supported.noiseSuppression === true)
            result.noiseSuppression = desired.noiseSuppression;

        if (supported.autoGainControl === true)
            result.autoGainControl = desired.autoGainControl;

        if (supported.sampleRate === true)
            result.sampleRate = desired.sampleRate;

        if (supported.channelCount === true)
            result.channelCount = desired.channelCount;

        return Object.freeze({
            supported: supported,
            desired: desired,
            default: result,
            other: other
        });
    })();

    instance.BaseState = class {
        // private fields

        #container;

        #dataInput;
        #dataValue;

        #state;
        #isActive;
        #isRemoved;
        #isReadOnly;
        #isDisabled;
        #isAutoUpload;
        #isAutoSubmit;
        #allowAudioCompression;

        #recording;
        #autoUpload;

        #fnSubmit;
        #uploadType;
        #timeLimit = null;
        #maxTime;

        constructor(options) {
            if (!(options.container instanceof HTMLElement))
                throwInitError();

            if (!(options.input instanceof HTMLInputElement) || options.input.type != 'hidden')
                throwInitError();

            this.#container = options.container;
            this.#dataInput = options.input;

            this.#state = null;
            this.#isActive = false;
            this.#isRemoved = false;
            this.#isReadOnly = options.isReadOnly == true;
            this.#isDisabled = options.isDisabled == true;
            this.#isAutoUpload = options.isAutoUpload == true;
            this.#isAutoSubmit = options.isAutoSubmit == true;
            this.#allowAudioCompression = true;

            this.#uploadType = options.uploadType;

            this.#recording = {
                tickHandler: null,
                tickStart: null,
                tickNumber: null,
                start: null,
                end: null,
                time: 0,
                chunks: null,
                mime: null,
                name: null,
                blob: null,
                recorder: null,
                audioCtx: null,
                isUploaded: false
            };

            this.#fnSubmit = options.fnSubmit ? new Function(options.fnSubmit) : null;

            this.#dataValue = this.#dataInput.value;
            this.#dataValue = this.#dataValue ? JSON.parse(this.#dataValue) : {};

            this.#maxTime = options.maxTime;
            this.#timeLimit = typeof options.timeLimit != 'number' || isNaN(options.timeLimit) || options.timeLimit <= 0
                ? null
                : options.timeLimit;

            addState(this);

            this.setState(inputState.stopped);
        }

        // other public

        get container() {
            return this.#container;
        }

        get state() {
            return this.#state;
        }

        get recording() {
            return this.#recording;
        }

        get timeLimit() {
            return this.#timeLimit;
        }

        get isActive() {
            return this.#isActive;
        }

        get isRemoved() {
            return this.#isRemoved || !this.isInDom();
        }

        get isAutoSubmit() {
            return this.#isAutoSubmit;
        }

        get isReadOnly() {
            return this.#isReadOnly;
        }

        get isDisabled() {
            return this.#isDisabled;
        }

        set isDisabled(value) {
            this.#isDisabled = value === true;
        }

        get dataValue() {
            return this.#dataValue;
        }

        get timeLimit() {
            return this.#timeLimit;
        }

        allowAudioCompression(value) {
            return this.#allowAudioCompression = value !== false;
        }

        saveDataValue() {
            this.#dataInput.value = JSON.stringify(this.#dataValue);
        }

        // recording methods

        getStream() {
            throw new Error(notImplementedError);
        }

        getMediaRecorder(stream) {
            throw new Error(notImplementedError);
        }

        startRecording() {
            if (this.#isActive || !this.canStart())
                return;

            this.dispatchEvent(eventId.starting);

            this.clearRecording();

            this.setState(inputState.waiting);

            this.getStream().then(
                (stream) => this.onStartSuccess(stream),
                (error) => this.onStartFailed(error));

            this.update();
        }

        stopRecording() {
            if (this.canStop())
                this.#recording.recorder.stop();
        }

        pauseRecording() {
            if (this.canPause())
                this.#recording.recorder.pause();
        }

        resumeRecording() {
            if (this.canResume())
                this.#recording.recorder.resume();
        }

        clearRecording() {
            if (this.#isActive)
                return;

            const recording = this.#recording;

            recording.tickStart = null;
            recording.tickNumber = null;
            recording.start = null;
            recording.end = null;
            recording.time = 0;
            recording.chunks = null;
            recording.mime = null;
            recording.name = null;
            recording.blob = null;
            recording.audioCtx = null;
            recording.recorder = null;
            recording.isUploaded = false;

            this.update();
            this.dispatchEvent(eventId.cleared);
        }

        uploadRecording() {
            if (this.#isRemoved || this.#recording.isUploaded == true || !this.#recording.blob)
                return;

            beginUploading(this);

            const file = new File([this.#recording.blob], this.#recording.name);

            if (this.#uploadType == 'api') {
                const formData = new FormData();
                formData.append(file.name, file);

                $.ajax({
                    url: uploadApiPath,
                    type: 'POST',
                    data: formData,
                    cache: false,
                    contentType: false,
                    processData: false,
                    success: (d) => {
                        this.#dataValue.file = d[0];
                        this.saveDataValue();

                        this.#recording.isUploaded = true;

                        endUploading(this);
                    },
                    error: (d) => {
                        console.log('error: ' + JSON.stringify(d));

                        this.setState(inputState.stopped);
                        this.update();

                        showMessage('Failed to upload media data to server.');
                    }
                });

            } else {
                this.#container.querySelectorAll('input[type="file"]').forEach(e => e.remove());

                const form = this.#dataInput.closest('form');
                if (form.enctype !== 'multipart/form-data')
                    form.enctype = 'multipart/form-data';

                const df = new DataTransfer();
                df.items.add(file);

                const fileInput = document.createElement('input'); {
                    fileInput.type = 'file';
                    fileInput.className = 'd-none';
                    fileInput.name = this.#dataInput.name + '$file';

                    this.#dataInput.parentNode.appendChild(fileInput);

                    fileInput.files = df.files;
                }

                this.#recording.isUploaded = true;

                endUploading(this);
            }

            function beginUploading(state) {
                state.dispatchEvent(eventId.uploading);
                state.setState(inputState.uploading);
                state.update();
            }

            function endUploading(state, end) {
                if (end !== true && state.isAutoSubmit && state.submitRecording()) {
                    setTimeout(endUploading, 5000, state, true);
                } else {
                    state.dispatchEvent(eventId.uploaded);
                    state.setState(inputState.stopped);
                    state.update();
                }
            }
        }

        submitRecording() {
            if (this.canSubmit()) {
                this.#fnSubmit();
                return true;
            } else {
                return false;
            }
        }

        // recording events

        onStartSuccess(stream) {
            const recording = this.#recording;
            
            if (audioConstraints.other.audioCompression && this.#allowAudioCompression && stream.getAudioTracks().length > 0) {
                const context = new AudioContext();
                const input = new MediaStreamAudioSourceNode(context, {
                    mediaStream: stream
                });
                const output = new MediaStreamAudioDestinationNode(context, {
                    channelCount: audioConstraints.desired.channelCount
                });
                const compressor = new DynamicsCompressorNode(context, {
                    threshold: -36, // -36 db
                    knee: 19, // 19 db
                    ratio: 3.5, // 3.5/1

                    release: 0.01, // 10 ms
                    attack: 0.002 // 2 ms
                });
                const gain = new GainNode(context, {
                    gain: 5, // 5 db
                });

                // input -> compressor -> gain -> output

                input.connect(compressor);
                compressor.connect(gain);
                gain.connect(output);

                const videoTracks = stream.getVideoTracks();
                for (let track of videoTracks)
                    output.stream.addTrack(track);

                recording.audioCtx = {
                    instance: context,
                    inputNode: input,
                    outputNode: output
                };

                stream = output.stream;
            }

            recording.recorder = this.getMediaRecorder(stream);
            recording.recorder.ondataavailable = (e) => this.onRecordingDataAvailable(e);
            recording.recorder.onstart = () => this.onRecordingStart();
            recording.recorder.onpause = () => this.onRecordingPause();
            recording.recorder.onresume = () => this.onRecordingResume();
            recording.recorder.onstop = () => this.onRecordingStop();
            recording.recorder.onerror = () => this.onRecordingError();
            recording.recorder.start();
        }

        onStartFailed(error) {
            this.onWaitingFailed();
            console.log("Getting user media failed: " + String(error));
        }

        onRecordingStart() {
            this.setState(inputState.recording);

            this.#recording.chunks = [];
            this.#recording.mime = null;
            this.#recording.name = null;
            this.#recording.blob = null;
            this.#recording.isUploaded = false;

            this.#startTick();
            this.update();
            this.dispatchEvent(eventId.started);
        }

        onRecordingPause() {
            this.setState(inputState.paused);
            this.#stopTick();
            this.update();
            this.dispatchEvent(eventId.paused);
        }

        onRecordingResume() {
            this.setState(inputState.recording);
            this.#startTick();
            this.update();
            this.dispatchEvent(eventId.resumed);
        }

        onRecordingDataAvailable(e) {
            if (e.data.size > 0)
                this.#recording.chunks.push(e.data);
        }

        onRecordingStop() {
            this.#stopTick();

            const recording = this.#recording;

            stopTracks(recording.recorder.stream);

            if (recording.audioCtx != null) {
                recording.audioCtx.instance.close();
                stopTracks(recording.audioCtx.inputNode.mediaStream);
            }

            this.setState(inputState.stopped);

            recording.audioCtx = null;
            recording.recorder = null;
            recording.end = new Date();
            recording.mime = this.getMimeType();
            recording.name = this.getFileName();
            recording.blob = this.getBlob();
            recording.chunks = null;

            this.update();
            this.dispatchEvent(eventId.stopped);

            if (this.#isAutoUpload)
                this.uploadRecording();

            function stopTracks(stream) {
                const tracks = stream.getTracks();
                for (let i = 0; i < tracks.length; i++)
                    tracks[i].stop();
            }
        }

        onRecordingError(error) {
            this.onWaitingFailed();
            console.log("Media recording failed: " + String(error));
        }

        onWaitingFailed() {
            this.setState(inputState.stopped);
            this.update();
        }

        // tick methods

        #startTick() {
            const recording = this.#recording;

            recording.tickStart = Date.now();
            recording.tickHandler = setInterval(() => this.#onRecordingTick(), tickInterval);

            if (recording.start == null)
                recording.start = new Date(recording.tickStart);

            if (recording.tickNumber == null)
                recording.tickNumber = 0;
        }

        #stopTick() {
            const recording = this.#recording;

            clearInterval(recording.tickHandler);
            recording.tickHandler = null;

            if (recording.tickStart != null) {
                const stopTime = Date.now();

                if (stopTime > recording.tickStart)
                    recording.time += (stopTime - recording.tickStart) / 1000;

                recording.tickStart = null;
            }
        }

        #onRecordingTick() {
            const recording = this.#recording;

            recording.tickNumber++;

            if (recording.tickNumber == tickTestInterval) {
                if (!this.#isRemoved)
                    this.#isRemoved = !this.isInDom();

                recording.tickNumber = 0;
            }

            if (this.#isRemoved || this.#updateTimeInternal())
                this.stopRecording();
        }

        // state

        setState(value) {
            this.#state = value;

            if (!this.#isRemoved)
                this.#isRemoved = !this.isInDom();

            if (this.#state == inputState.stopped)
                this.#removeActive();
            else
                this.#addActive();
        }

        onStateRemoved() {
            this.stopRecording();
            this.#removeActive();
        }

        canStart() {
            return this.#isDisabled != true
                && this.#isReadOnly != true
                && this.#isRemoved == false
                && (activeStates.length == 0 || this.#isActive);
        }

        canStop() {
            return this.#state == inputState.recording || this.#state == inputState.paused;
        }

        canPause() {
            return this.#state == inputState.recording && this.#recording.recorder.state === 'recording';
        }

        canResume() {
            return this.#state == inputState.paused && this.#recording.recorder.state === 'paused';
        }

        canSubmit() {
            return this.#fnSubmit && !this.#isRemoved && this.#recording.isUploaded == true;
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

        // update UI

        update() {
            this.#updateTimeInternal();
        }

        updateTime(totalTime) {
            throw new Error(notImplementedError);
        }

        #updateTimeInternal() {
            const recording = this.#recording;
            let totalTime = recording.time;

            if (recording.tickStart != null) {
                const tickTime = Date.now();
                if (tickTime > recording.tickStart)
                    totalTime += (tickTime - recording.tickStart) / 1000;
            }

            let isValid = false;

            if (this.#timeLimit != null) {
                totalTime = this.#timeLimit - totalTime;
                if (isValid = totalTime <= 0)
                    totalTime = 0;
            } else {
                if (isValid = totalTime >= this.#maxTime)
                    totalTime = this.#maxTime;
            }

            this.updateTime(totalTime);

            return isValid;
        }

        // helper methods

        getEventName(id) {
            throw new Error(notImplementedError);
        }

        dispatchEvent(id) {
            const name = this.getEventName(id);
            if (name)
                this.#container.dispatchEvent(new CustomEvent(name, {
                    bubbles: true
                }));
            else
                throw new Error('Event name not found: ' + String(value));
        }

        isInDom() {
            return document.contains(this.#container);
        }

        getMimeExtMapping() {
            throw new Error(notImplementedError);
        }

        getMimeType() {
            if (this.#state != inputState.stopped)
                return null;

            const chunks = this.#recording.chunks;
            if (!chunks || chunks.length == 0)
                return raiseError('no data (1)');

            const firstChunk = chunks[0];
            if (firstChunk.size == 0)
                return raiseError('no data (2)');

            const chunkType = String(firstChunk.type);
            if (!chunkType)
                return raiseError('the chunk type is not defined');

            let mimeType = null;

            const parts = chunkType.split(';');
            for (let i = 0; i < parts.length; i++) {
                const value = parts[i].trim().toLowerCase();
                if (value.startsWith('audio/') || value.startsWith('video/')) {
                    mimeType = value;
                    break;
                }
            }

            const mimeExt = this.getMimeExtMapping();

            return mimeType && mimeExt.hasOwnProperty(mimeType) ? mimeType : raiseError(chunkType);

            function raiseError(reason) {
                showMessage('Can\'t identify MIME type: ' + reason);
                return null;
            }
        }

        getFileName() {
            if (this.#state != inputState.stopped || !this.#recording.mime)
                return null;

            const date = this.#recording.start;
            if (!date)
                return null;

            const year = String(date.getFullYear());
            const month = ('0' + String(date.getMonth() + 1)).slice(-2);
            const day = ('0' + String(date.getDate())).slice(-2);
            const hours = ('0' + String(date.getHours())).slice(-2);
            const minutes = ('0' + String(date.getMinutes())).slice(-2);
            const seconds = ('0' + String(date.getSeconds())).slice(-2);
            const ext = this.getMimeExtMapping()[this.#recording.mime];

            return year + month + day + '-' + hours + minutes + seconds + ext;
        }

        getBlob() {
            if (this.#state != inputState.stopped || !this.#recording.mime)
                return null;

            const chunks = this.#recording.chunks;

            return chunks && chunks.length > 0 ? new Blob(chunks, { type: chunks[0].type }) : raiseError('no data');

            function raiseError(reason) {
                showMessage('Can\'t create BLOB: ' + reason);
                return null;
            }
        }
    }

    instance.inputState = inputState;
    instance.eventId = eventId;
    instance.hasState = hasState;
    instance.audioMimeExt = audioMimeExt;
    instance.videoMimeExt = videoMimeExt;
    instance.getAudioBitrate = getAudioBitrate;
    instance.audioConstraints = audioConstraints;

    function getAudioBitrate(value) {
        if (!audioBitrate.hasOwnProperty(value))
            value = '32';

        return audioBitrate[value];
    }

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

    function throwInitError(reason) {
        let text = 'InputMedia initialization error';
        if (reason)
            text += ': ' + reason;
        throw new Error(text);
    }

    function showMessage(message) {
        alert(message);
    }
})();
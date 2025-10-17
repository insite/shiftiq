import { useEffect } from "react";
import { timerHelper } from "@/helpers/timerHelper";
import { useSiteProvider } from "@/contexts/SiteProvider";
import "./SessionTimer.css";

declare global {
    interface Window {
        sessionTime?: {
            startTimer: () => boolean,
            resetTimer: () => void
        }
    }
}

const MINUTE_IN_MS = 60 * 1000;
const BLINK_TIME = 5 * MINUTE_IN_MS;
const SESSION_TIMER_CLASS = "sessionTimer";

let _isBlinking = false;
let _updateHandlerId: number | null = null;
let _sessionTimeoutMinutes: number | null = null;

export default function SessionTimer() {
    const { siteSetting } = useSiteProvider();

    useEffect(() => {
        _sessionTimeoutMinutes = siteSetting.SessionTimeoutMinutes;
    }, [siteSetting]);

    useEffect(() => {
        window.sessionTime = Object.freeze({
            startTimer: start,
            resetTimer: reset
        });

        if (window.sessionTime.startTimer()) {
            reset();
        }

        window.addEventListener("storage", storageHandler);

        return () => {
            window.removeEventListener("storage", storageHandler);
            if (_updateHandlerId) {
                clearInterval(_updateHandlerId);
                _updateHandlerId = null;
            }
        }
    }, []);

    return (
        <span className={SESSION_TIMER_CLASS}>00:00</span>
    );
}

function storageHandler() {
    timerHelper.restoreTimer();
}

function start() {
    if (_updateHandlerId) {
        update();
        return false;
    }
    _updateHandlerId = setInterval(update, 1000) as unknown as number;
    update();
    return true;
}

function update() {
    const loadTime = timerHelper.getLoadTime();
    if (!loadTime || !_sessionTimeoutMinutes) {
        return;
    }

    const sessionTime = _sessionTimeoutMinutes * MINUTE_IN_MS;
    let time = sessionTime - Date.now() + loadTime;

    if (time <= 0 && _updateHandlerId) {
        time = 0;
        _isBlinking = false;
        clearInterval(_updateHandlerId);
        _updateHandlerId = null;
    }

    if (!_isBlinking && time <= BLINK_TIME) {
        updateOutput(output => output.classList.add('blink_me', 'text-danger'));
        _isBlinking = true;
    }

    const minutes = Math.floor(time / MINUTE_IN_MS);
    const seconds = Math.floor((time - minutes * MINUTE_IN_MS) / 1000);
    const value = ('00' + String(minutes)).slice(-2) + ":" + ('00' + String(seconds)).slice(-2);

    updateOutput(output => output.innerHTML = value);
}

function updateOutput(action: (output: Element) => void) {
    const outputs = document.querySelectorAll(`.${SESSION_TIMER_CLASS}`);
    for (const output of outputs) {
        action(output);
    }
}

function reset() {
    timerHelper.resetTimer();

    if (_isBlinking) {
        updateOutput(output => output.classList.remove('blink_me', 'text-danger'));
        _isBlinking = false;
    }
}
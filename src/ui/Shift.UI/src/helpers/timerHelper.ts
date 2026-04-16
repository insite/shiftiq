import { dateTimeHelper } from "./date/dateTimeHelper";

const MINUTE_IN_MS = 60 * 1000;
const STORAGE_ITEM = "inSite.page.loadTime";

let _startTimeInMs: number | null = null;

export const timerHelper = {
    syncWithStorage(): number | null {
        const value = parseInt(window.localStorage.getItem(STORAGE_ITEM) as string);
        _startTimeInMs = value && !isNaN(value) ? value : null;
        return _startTimeInMs;
    },

    resetTimer(sessionRefreshed: string | null | undefined): void {
        const parsedStartTime = dateTimeHelper.parseDateTime(sessionRefreshed, "UTC", "yyyy-mm-dd", "HH:mm:ss z");
        if (parsedStartTime) {
            _startTimeInMs = dateTimeHelper.toDate(parsedStartTime)!.getTime();
            window.localStorage.setItem(STORAGE_ITEM, String(_startTimeInMs));
        }
    },

    getStartTimeInMs(): number | null {
        return _startTimeInMs;
    },

    getTimerText(timeLeftInMs: number): string {
        if (timeLeftInMs <= 0) {
            return "00:00";
        }

        const minutes = Math.floor(timeLeftInMs / MINUTE_IN_MS);
        const seconds = Math.floor((timeLeftInMs - minutes * MINUTE_IN_MS) / 1000);

        return ('00' + String(minutes)).slice(-2) + ":" + ('00' + String(seconds)).slice(-2);
    },
}
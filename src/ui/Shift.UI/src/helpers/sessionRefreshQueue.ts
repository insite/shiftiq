import { shiftClient } from "@/api/shiftClient";

const _refreshIntervalInMs = 5 * 60 * 1000;

let _refreshHandlerId: unknown = null;

async function refreshSession(): Promise<void> {
    await shiftClient.cookie.refreshSession();

    _refreshHandlerId = null;
}

export const sessionRefreshQueue = {
    queue(timeLeftInMs: number) {
        if (_refreshHandlerId || timeLeftInMs === 0) {
            return;
        }

        if (timeLeftInMs < _refreshIntervalInMs * 2) {
            refreshSession();
            return;
        }

        _refreshHandlerId = setTimeout(refreshSession, _refreshIntervalInMs);
    },
};
const STORAGE_ITEM = "inSite.page.loadTime";

let _loadTime: number | null = null;

export const timerHelper = {
    resetTimer() {
        _loadTime = Date.now();
        window.localStorage.setItem(STORAGE_ITEM, String(_loadTime));
    },

    restoreTimer() {
        if (!_loadTime) {
            return;
        }
        const value = parseInt(window.localStorage.getItem(STORAGE_ITEM) as string);
        if (value && !isNaN(value) && _loadTime < value) {
            _loadTime = value;
        }
    },

    getLoadTime(): number | null {
        return _loadTime;
    },
}
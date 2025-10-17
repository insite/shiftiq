export const numberHelper = {
    formatNumber(n: number, digits?: number): string {
        const options = digits != undefined
            ? {
                minimumFractionDigits: digits,
                maximumFractionDigits: digits
            } : undefined;

        return n.toLocaleString("en-CA", options);
    },

    formatInt(input: number | string) {
        const s = typeof input === "number" ? String(input) : String(parseInt(input));
        let result = "";

        for (let i = s.length - 1, k = 0; i >= 0; i--, k++) {
            if (k !== 0 && k % 3 === 0) {
                result = "," + result;
            }
            result = s[i] + result;
        }

        return result;
    },
    formatBytes(n: number, digits: number = 2) {
        if (isNaN(n)) {
            return "0 B";
        }

        const k = 1024;
        const dm = digits < 0 ? 0 : digits;
        const sizes = ['B', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'];

        const i = Math.floor(Math.log(n) / Math.log(k));

        return `${parseFloat((n / Math.pow(k, i)).toFixed(dm))} ${sizes[i]}`;
    }
}
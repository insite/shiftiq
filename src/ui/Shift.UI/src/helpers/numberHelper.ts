export const numberHelper = {
    formatNumber(n: number, digits?: number): string {
        const options = digits != undefined
            ? {
                minimumFractionDigits: digits,
                maximumFractionDigits: digits
            } : undefined;

        return n.toLocaleString("en-CA", options);
    },

    parseInt(input: string | null | undefined): number | null {
        if (!input) {
            return null;
        }

        let s = "";
        for (let i = 0; i < input.length; i++) {
            if (input[i] >= "0" && input[i] <= "9") {
                s += input[i];
            }
            else if (input[i] === ".") {
                break;
            }
        }
        
        const n = Number(s);
        
        return !isNaN(n) ? Math.floor(n) : null;
    },

    formatInt(input: number | string): string {
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

    formatDecimal(input: number, digits: number = 2): string {
        const s = String(input);

        if (s.length <= 4) {
            return digits === 0
                ? "0"
                : digits > 4
                    ? "0." + s.padStart(4, "0")
                    : "0." + s.padStart(4, "0").substring(0, digits);
        }

        let result = "." + (digits > 4 ? s.substring(s.length - 4, s.length) : s.substring(s.length - 4, s.length - (4 - digits)));

        for (let i = s.length - 5, k = 0; i >= 0; i--, k++) {
            if (k !== 0 && k % 3 === 0) {
                result = "," + result;
            }
            result = s[i] + result;
        }

        return result;
    },

    parseDecimal(input: string): number | null {
        const parts = input.replaceAll(",", "").trim().split(".");
        if (parts.length > 2) {
            return null;
        }
        const decimals = parts.length == 1
            ? "0000"
            : parts[1].length > 4
                ? parts[1].substring(0, 4)
                : parts[1].padEnd(4, "0");

        const result = parseInt(parts[0] + decimals);

        return isNaN(result) ? null : result;
    },

    formatBytes(n: number, digits: number = 2): string {
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
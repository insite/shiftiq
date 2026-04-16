export const base64Helper = {
    objectToBase64(obj: object) {
        const jsonString = JSON.stringify(obj);
        const utf8Bytes = new TextEncoder().encode(jsonString);
        const binaryString = String.fromCharCode(...utf8Bytes);

        return btoa(binaryString);
    },

    base64ToObject<T>(base64: string): T {
        const binaryString = atob(base64);
        const utf8Bytes = Uint8Array.from(binaryString, c => c.charCodeAt(0));
        const jsonString = new TextDecoder().decode(utf8Bytes);

        return JSON.parse(jsonString) as T;
    },
}
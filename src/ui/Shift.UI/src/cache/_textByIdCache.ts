const _map: Map<string, string> = new Map();

export const _textByIdCache = {
    clear() {
        _map.clear();
    },

    setText(id: string, text: string) {
        _map.set(id.toLowerCase(), text);
    },

    getText(id: string): string | null {
        return _map.get(id.toLowerCase()) ?? null;
    },
}